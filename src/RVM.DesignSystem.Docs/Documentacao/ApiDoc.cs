using System.Net.Http.Json;
using System.Xml.Linq;

namespace RVM.DesignSystem.Docs.Documentacao;

/// <summary>Um parâmetro público de um componente, como aparece na tabela de API.</summary>
/// <param name="Nome">Nome do parâmetro.</param>
/// <param name="Tipo">Tipo declarado, já legível (sem namespace).</param>
/// <param name="Padrao">Valor padrão, ou <c>null</c> quando não há.</param>
/// <param name="Descricao">O <c>&lt;summary&gt;</c> da documentação XML.</param>
/// <param name="Obrigatorio">Verdadeiro quando marcado com <c>EditorRequired</c>.</param>
public sealed record ParametroDoc(
    string Nome,
    string Tipo,
    string? Padrao,
    string Descricao,
    bool Obrigatorio);

/// <summary>
/// Lê a documentação XML da biblioteca e devolve a tabela de API de cada componente.
/// </summary>
/// <remarks>
/// A tabela é <b>gerada do código</b>, não escrita à mão (`07` § Anatomia de uma página). Doc
/// escrita à mão desatualiza no primeiro parâmetro novo — e desatualiza em silêncio, que é o
/// pior modo de falhar: ninguém percebe até alguém seguir a doc e não funcionar.
///
/// <para>
/// O XML dá as <b>descrições</b>; a reflexão dá <b>tipo, padrão e obrigatoriedade</b>. Nenhum
/// dos dois sozinho basta: o XML não conhece o valor padrão, e a reflexão não conhece a prosa.
/// </para>
/// </remarks>
public sealed class ApiDoc(HttpClient http)
{
    private Dictionary<string, string>? _resumos;

    /// <summary>Carrega o XML uma vez por sessão.</summary>
    private async Task<Dictionary<string, string>> ResumosAsync()
    {
        if (_resumos is not null)
        {
            return _resumos;
        }

        try
        {
            var xml = await http.GetStringAsync("api/RVM.DesignSystem.xml");
            var doc = XDocument.Parse(xml);

            _resumos = doc.Descendants("member")
                .Where(m => m.Attribute("name") is not null && m.Element("summary") is not null)
                .ToDictionary(
                    m => m.Attribute("name")!.Value,
                    m => Normalizar(m.Element("summary")!.Value),
                    StringComparer.Ordinal);
        }
        catch (Exception e) when (e is HttpRequestException or System.Xml.XmlException)
        {
            // O site tem que abrir mesmo sem o XML. Sem ele a tabela sai com os tipos e sem as
            // descrições, o que ainda é útil — melhor que uma página em branco.
            _resumos = [];
        }

        return _resumos;
    }

    /// <summary>Monta a tabela de API de um componente.</summary>
    /// <param name="tipo">O tipo do componente.</param>
    /// <returns>Os parâmetros públicos, em ordem alfabética.</returns>
    public async Task<IReadOnlyList<ParametroDoc>> ParametrosAsync(Type tipo)
    {
        ArgumentNullException.ThrowIfNull(tipo);

        var resumos = await ResumosAsync();

        // Uma instância só para ler os valores padrão das propriedades — é a única forma de
        // saber que Variant nasce Primary sem repetir isso na doc à mão.
        object? padrao = null;
        try
        {
            padrao = Activator.CreateInstance(tipo);
        }
        catch (Exception e) when (e is MissingMethodException or InvalidOperationException)
        {
            // Componente genérico sem construtor sem parâmetros: seguimos sem os padrões.
        }

        var parametros = new List<ParametroDoc>();

        foreach (var p in tipo.GetProperties())
        {
            var ehParametro = p.GetCustomAttributes(inherit: true)
                .Any(a => a.GetType().Name == "ParameterAttribute");

            if (!ehParametro)
            {
                continue;
            }

            var obrigatorio = p.GetCustomAttributes(inherit: true)
                .Any(a => a.GetType().Name == "EditorRequiredAttribute");

            // A chave do XML usa o tipo DECLARANTE, não o tipo consultado — parâmetro herdado
            // (Error, Help, vindos da RvmInputBase) mora na doc da base.
            var declarante = p.DeclaringType ?? tipo;
            var chave = $"P:{NomeXml(declarante)}.{p.Name}";
            resumos.TryGetValue(chave, out var descricao);

            // Os parametros herdados do InputBase<T> do Blazor nao estao no NOSSO XML — a doc
            // deles vive na assembly da Microsoft, que o site nao carrega. Deixa-los como "—"
            // seria pior que inutil: `Value` e `ValueChanged` sao os parametros mais
            // importantes de um campo, e apareceriam justamente como os sem explicacao.
            descricao ??= DescricaoHerdada(p.Name);

            parametros.Add(new ParametroDoc(
                p.Name,
                NomeLegivel(p.PropertyType),
                ValorPadrao(padrao, p),
                descricao ?? "—",
                obrigatorio));
        }

        return parametros
            .OrderByDescending(x => x.Obrigatorio)
            .ThenBy(x => x.Nome, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>Descricao dos parametros que vem do <c>InputBase</c> do Blazor.</summary>
    private static string? DescricaoHerdada(string nome) => nome switch
    {
        "Value" => "O valor do campo. Use com @bind-Value.",
        "ValueChanged" => "Disparado quando o valor muda. O @bind-Value liga sozinho.",
        "ValueExpression" => "Expressao do valor, usada pela validacao. O @bind-Value fornece; fora de um EditForm ela e sintetizada.",
        "DisplayName" => "Nome do campo nas mensagens de validacao.",
        "AdditionalAttributes" => "Qualquer atributo extra vai para o elemento.",
        _ => null,
    };

    private static string? ValorPadrao(object? instancia, System.Reflection.PropertyInfo p)
    {
        if (instancia is null || !p.CanRead)
        {
            return null;
        }

        try
        {
            var v = p.GetValue(instancia);

            return v switch
            {
                null => null,
                bool b => b ? "true" : "false",
                string s when s.Length == 0 => null,
                // Id gerado por Guid muda a cada instância — mostrar um valor concreto
                // sugeriria que ele é fixo, o que é pior que não mostrar nada.
                string s when s.StartsWith("rvm-", StringComparison.Ordinal) => "gerado",
                string s => s,
                Enum e => e.ToString(),
                int or long or double or decimal => v.ToString(),

                // Qualquer outro tipo cai aqui, e o ToString() dele quase nunca é um valor:
                // um EventCallback devolve "Microsoft.AspNetCore.Components.EventCallback`1",
                // que na coluna "Padrão" é pior que vazio — parece um valor e não é.
                _ => null,
            };
        }
        catch (Exception e) when (e is System.Reflection.TargetInvocationException or NotSupportedException)
        {
            return null;
        }
    }

    /// <summary>Nome do tipo como o compilador escreve no XML (com ` para genéricos).</summary>
    private static string NomeXml(Type t) =>
        t.IsGenericType ? $"{t.Namespace}.{t.Name}" : t.FullName ?? t.Name;

    /// <summary>Nome do tipo como uma pessoa escreveria em C#.</summary>
    private static string NomeLegivel(Type t)
    {
        var nullable = Nullable.GetUnderlyingType(t);
        if (nullable is not null)
        {
            return NomeLegivel(nullable) + "?";
        }

        // ⚠️ IsGenericType e true tambem para tipo ANINHADO dentro de um generico — o
        // `RvmRadioGroup<T>.Option` e "generico" sem ter parametro proprio, e o nome dele nao
        // tem crase. Sem esta guarda, IndexOf devolve -1, o slice estoura com
        // ArgumentOutOfRangeException e a PAGINA INTEIRA quebra: o Blazor mostra a barra de erro
        // e a tabela de API some. Descoberto em 08/09/2026 pela varredura de acessibilidade, que
        // pegou o banner de erro visivel — nao pelo teste unitario.
        var crase = t.Name.IndexOf('`', StringComparison.Ordinal);

        if (t.IsGenericType && crase >= 0)
        {
            var nome = t.Name[..crase];
            var args = string.Join(", ", t.GetGenericArguments().Select(NomeLegivel));
            return $"{nome}<{args}>";
        }

        return t.Name switch
        {
            "String" => "string",
            "Boolean" => "bool",
            "Int32" => "int",
            "Object" => "object",
            _ => t.Name,
        };
    }

    /// <summary>Tira a indentação que o compilador deixa no XML.</summary>
    private static string Normalizar(string texto) =>
        string.Join(' ', texto.Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0));
}
