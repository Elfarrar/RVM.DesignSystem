using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Tests.Ia;

/// <summary>
/// Escreve o <c>llms.txt</c> (indice, formato de llmstxt.org) e o <c>llms-full.txt</c> (a API inteira)
/// a partir da propria biblioteca: reflexao sobre os <c>[Parameter]</c> e o XML doc gerado no build.
/// Nada e escrito a mao, entao componente novo entra sozinho e o teste de sincronia avisa (DSGN-016).
/// </summary>
internal sealed class ApiParaIa
{
    internal const string Site = "https://design.rvmit.com.br";

    private readonly Assembly _biblioteca;
    private readonly Dictionary<string, string> _documentacao;
    private readonly IReadOnlyDictionary<string, (string Rota, string Resumo)> _site;
    private readonly NullabilityInfoContext _nulabilidade = new();

    /// <param name="biblioteca">O assembly <c>RVM.DesignSystem</c>.</param>
    /// <param name="xmlDoc">O <c>RVM.DesignSystem.xml</c> gerado no build.</param>
    /// <param name="site">Do catalogo do site: nome do componente ("RvmButton") para a rota da pagina e o
    /// resumo do menu. O resumo cobre o componente feito so de <c>.razor</c>, que nao tem XML doc.</param>
    internal ApiParaIa(Assembly biblioteca, XDocument xmlDoc, IReadOnlyDictionary<string, (string Rota, string Resumo)> site)
    {
        _biblioteca = biblioteca;
        _site = site;
        _documentacao = xmlDoc.Descendants("member")
            .Where(m => m.Element("summary") is not null)
            .ToDictionary(m => (string)m.Attribute("name")!, m => Texto(m.Element("summary")!));
    }

    internal IReadOnlyList<Type> Componentes()
        => _biblioteca.GetExportedTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IComponent).IsAssignableFrom(t)
                        && t.Name.StartsWith("Rvm", StringComparison.Ordinal))
            .OrderBy(t => t.Name, StringComparer.Ordinal)
            .ToArray();

    internal string GerarIndice()
    {
        var sb = new StringBuilder();
        sb.Append(Cabecalho());
        sb.Append("\n## Arquivos para agentes\n\n");
        sb.Append($"- [API completa]({Site}/llms-full.txt): regras de uso, todos os componentes com parametros, tipos e padroes, enums e tokens\n");
        sb.Append($"- [Tokens DTCG]({Site}/tokens.json): os tokens no formato W3C Design Tokens, tema claro e escuro\n");
        sb.Append("\n## Componentes\n\n");
        foreach (var tipo in Componentes())
        {
            var rota = Rota(tipo);
            var resumo = Resumo(tipo);
            sb.Append(rota is null ? $"- `{NomeDoTipo(tipo)}`" : $"- [{NomeDoTipo(tipo)}]({Site}/{rota})");
            sb.Append(resumo.Length > 0 ? $": {resumo}\n" : "\n");
        }

        return sb.ToString();
    }

    internal string GerarCompleto(IReadOnlyList<TokensDtcg.Token> tokens)
    {
        var sb = new StringBuilder();
        sb.Append(Cabecalho());
        sb.Append(Regras);

        sb.Append("\n## Componentes\n");
        foreach (var tipo in Componentes())
        {
            sb.Append($"\n### {NomeDoTipo(tipo)}\n\n");
            sb.Append($"`@using {tipo.Namespace}`");
            var rota = Rota(tipo);
            if (rota is not null)
            {
                sb.Append($" · exemplos: {Site}/{rota}");
            }

            sb.Append("\n\n");
            var resumo = Resumo(tipo);
            if (resumo.Length > 0)
            {
                sb.Append(resumo).Append("\n\n");
            }

            var parametros = Parametros(tipo);
            if (parametros.Count == 0)
            {
                sb.Append("Sem parametros.\n");
                continue;
            }

            sb.Append("| Parametro | Tipo | Padrao | Descricao |\n|---|---|---|---|\n");
            foreach (var (nome, tipoDoParametro, padrao, descricao) in parametros)
            {
                sb.Append($"| `{nome}` | `{tipoDoParametro}` | {padrao} | {Celula(descricao)} |\n");
            }
        }

        sb.Append("\n## Enums\n\n");
        foreach (var tipo in _biblioteca.GetExportedTypes().Where(t => t.IsEnum).OrderBy(t => t.Name, StringComparer.Ordinal))
        {
            var resumo = Resumo(tipo);
            sb.Append($"- `{tipo.Name}` (`{tipo.Namespace}`){(resumo.Length > 0 ? ": " + resumo : "")}\n");
            sb.Append("  Valores: ").Append(string.Join(", ", Enum.GetNames(tipo).Select(n => $"`{n}`"))).Append('\n');
        }

        sb.Append("\n## Tipos de apoio\n\nClasses e records usados nos parametros dos componentes.\n");
        foreach (var tipo in _biblioteca.GetExportedTypes()
                     .Where(t => !t.IsEnum && !t.Name.StartsWith('_') && !typeof(IComponent).IsAssignableFrom(t) && !t.IsNested
                                 && !(t.IsAbstract && t.IsSealed && t.Name.EndsWith("Extensions", StringComparison.Ordinal)))
                     .OrderBy(t => t.Name, StringComparer.Ordinal))
        {
            sb.Append($"\n### {NomeDoTipo(tipo)}\n\n`{tipo.Namespace}`");
            var resumo = Resumo(tipo);
            sb.Append(resumo.Length > 0 ? " · " + resumo : "").Append('\n');
            foreach (var propriedade in tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                var doc = Documentacao("P", propriedade);
                sb.Append($"- `{propriedade.Name}`: `{NomeDoTipo(propriedade)}`{(doc.Length > 0 ? " — " + doc : "")}\n");
            }

            foreach (var metodo in tipo.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                         .Where(m => !m.IsSpecialName && !m.Name.Contains('<', StringComparison.Ordinal)
                                     && m.Name is not ("Equals" or "GetHashCode" or "ToString" or "Deconstruct" or "PrintMembers")))
            {
                var assinatura = string.Join(", ", metodo.GetParameters().Select(p => $"{NomeDoTipo(p.ParameterType)} {p.Name}"));
                sb.Append($"- `{NomeDoTipo(metodo.ReturnType)} {metodo.Name}({assinatura})`\n");
            }
        }

        sb.Append("\n## Tokens\n\nUse sempre `var(--rvm-...)`. Coluna escuro vazia = o valor nao muda no tema escuro. ");
        sb.Append($"Os mesmos tokens em formato DTCG: {Site}/tokens.json\n\n");
        sb.Append("| Variavel CSS | Claro | Escuro |\n|---|---|---|\n");
        foreach (var token in tokens)
        {
            sb.Append($"| `{token.Variavel}` | `{token.Claro}` | {(token.Escuro is null ? "" : $"`{token.Escuro}`")} |\n");
        }

        return sb.ToString();
    }

    internal IReadOnlyList<(string Nome, string Tipo, string Padrao, string Descricao)> Parametros(Type componente)
    {
        var instancia = Instanciar(componente);
        var outra = Instanciar(componente);

        // Do tipo fechado (RvmDataGrid<object>): propriedade do tipo aberto nao pode ser lida.
        return (instancia?.GetType() ?? componente).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(p =>
            {
                var obrigatorio = p.GetCustomAttribute<EditorRequiredAttribute>() is not null;
                var padrao = obrigatorio ? "obrigatorio" : Padrao(p, instancia, outra);
                var descricao = Documentacao("P", p);
                if (descricao.Length == 0 && p.Name == "AdditionalAttributes")
                {
                    descricao = "Atributos HTML extras (`class`, `style`, `id`, `aria-*`...), repassados ao elemento raiz.";
                }

                return (p.Name, NomeDoTipo(p), padrao, descricao);
            })
            .ToArray();
    }

    /// <summary>
    /// O valor inicial da propriedade, lido de uma instancia nova. Se duas instancias discordam (id
    /// gerado, data de hoje), o valor nao e padrao de verdade e sai como "—" — senao o arquivo mudaria
    /// a cada geracao.
    /// </summary>
    private static string Padrao(PropertyInfo propriedade, object? instancia, object? outra)
    {
        if (instancia is null || outra is null)
        {
            return "—";
        }

        var valor = Formatar(propriedade.GetValue(instancia));
        return valor == Formatar(propriedade.GetValue(outra)) ? valor : "—";
    }

    private static string Formatar(object? valor) => valor switch
    {
        null => "—",
        bool b => b ? "`true`" : "`false`",
        string s => s.Length == 0 ? "`\"\"`" : $"`\"{s}\"`",
        Enum e => $"`{e.GetType().Name}.{e}`",
        IFormattable f when valor.GetType().IsPrimitive || valor is decimal or TimeSpan
            => $"`{f.ToString(null, CultureInfo.InvariantCulture)}`",
        _ => "—"
    };

    private static object? Instanciar(Type componente)
    {
        try
        {
            var concreto = componente.IsGenericTypeDefinition
                ? componente.MakeGenericType(componente.GetGenericArguments().Select(_ => typeof(object)).ToArray())
                : componente;
            return Activator.CreateInstance(concreto);
        }
        catch (ArgumentException)
        {
            // Restricao generica que object nao satisfaz: fica sem os padroes, mas com o resto.
            return null;
        }
    }

    private string Resumo(Type tipo)
        => _documentacao.GetValueOrDefault("T:" + tipo.FullName)
           ?? (_site.TryGetValue(NomeSemGenerico(tipo), out var pagina) ? pagina.Resumo : "");

    /// <summary>
    /// A pagina do componente, ou a do pai pelo prefixo mais longo: <c>RvmSnackbarHost</c> e
    /// <c>RvmMenuItem</c> sao mostrados nas paginas Snackbar e Menu.
    /// </summary>
    private string? Rota(Type tipo)
    {
        var nome = NomeSemGenerico(tipo);
        return _site.Where(p => nome.StartsWith(p.Key, StringComparison.Ordinal))
            .OrderByDescending(p => p.Key.Length)
            .Select(p => p.Value.Rota)
            .FirstOrDefault();
    }

    private string Documentacao(string prefixo, PropertyInfo propriedade)
    {
        var declarante = propriedade.DeclaringType!;
        if (declarante.IsGenericType)
        {
            declarante = declarante.GetGenericTypeDefinition();
        }

        return _documentacao.GetValueOrDefault($"{prefixo}:{declarante.FullName}.{propriedade.Name}", "");
    }

    private string NomeDoTipo(PropertyInfo propriedade)
    {
        var nome = NomeDoTipo(propriedade.PropertyType);
        return !propriedade.PropertyType.IsValueType
               && _nulabilidade.Create(propriedade).ReadState == NullabilityState.Nullable
            ? nome + "?"
            : nome;
    }

    internal static string NomeDoTipo(Type tipo)
    {
        if (Nullable.GetUnderlyingType(tipo) is { } subjacente)
        {
            return NomeDoTipo(subjacente) + "?";
        }

        if (tipo.IsArray)
        {
            return NomeDoTipo(tipo.GetElementType()!) + "[]";
        }

        var apelido = tipo == typeof(bool) ? "bool"
            : tipo == typeof(int) ? "int"
            : tipo == typeof(long) ? "long"
            : tipo == typeof(double) ? "double"
            : tipo == typeof(float) ? "float"
            : tipo == typeof(decimal) ? "decimal"
            : tipo == typeof(string) ? "string"
            : tipo == typeof(object) ? "object"
            : tipo == typeof(void) ? "void"
            : null;
        if (apelido is not null)
        {
            return apelido;
        }

        if (!tipo.IsGenericType)
        {
            return tipo.Name;
        }

        var nome = tipo.Name[..tipo.Name.IndexOf('`', StringComparison.Ordinal)];
        return $"{nome}<{string.Join(", ", tipo.GetGenericArguments().Select(NomeDoTipo))}>";
    }

    private static string NomeSemGenerico(Type tipo)
        => tipo.IsGenericType ? tipo.Name[..tipo.Name.IndexOf('`', StringComparison.Ordinal)] : tipo.Name;

    /// <summary>
    /// Texto corrido do summary: <c>&lt;see cref&gt;</c> vira o nome curto, <c>&lt;c&gt;</c> vira
    /// codigo em crase, e o espaco da indentacao do XML some.
    /// </summary>
    internal static string Texto(XElement elemento)
    {
        var sb = new StringBuilder();
        foreach (var no in elemento.Nodes())
        {
            switch (no)
            {
                case XText texto:
                    sb.Append(texto.Value);
                    break;
                case XElement { Name.LocalName: "see" or "seealso" } see:
                    sb.Append('`').Append(NomeDaReferencia(see)).Append('`');
                    break;
                case XElement { Name.LocalName: "c" or "paramref" or "typeparamref" } codigo:
                    sb.Append('`').Append(codigo.Attribute("name")?.Value ?? codigo.Value).Append('`');
                    break;
                case XElement outro:
                    sb.Append(' ').Append(Texto(outro)).Append(' ');
                    break;
            }
        }

        return Regex.Replace(sb.ToString(), @"\s+", " ").Trim();
    }

    private static string NomeDaReferencia(XElement see)
    {
        var cref = see.Attribute("cref")?.Value ?? see.Attribute("langword")?.Value ?? see.Value;
        if (cref.Length > 2 && cref[1] == ':')
        {
            var tipoDeMembro = cref[0];
            var caminho = cref[2..];
            var parenteses = caminho.IndexOf('(', StringComparison.Ordinal);
            if (parenteses >= 0)
            {
                caminho = caminho[..parenteses];
            }

            var partes = caminho.Split('.');
            cref = tipoDeMembro is 'T' or 'N' ? partes[^1] : string.Join('.', partes[^2..]);
            cref = Regex.Replace(cref, @"`\d+", "");
        }

        return cref;
    }

    private static string Celula(string texto) => texto.Replace("|", "\\|", StringComparison.Ordinal);

    private static string Cabecalho() =>
        """
        # RVM Design System

        > Biblioteca de componentes Blazor (Server e WebAssembly), em C#, distribuida no pacote NuGet `RVM.DesignSystem`. Visual derivado do NEATLAB - Super Admin Dashboard UI Design Kit, de hello.uiworld (Figma Community), sob CC BY 4.0 — o credito e obrigatorio. Codigo sob MIT.

        Componentes com prefixo `Rvm`, API em ingles, texto ao usuario em portugues do Brasil. Toda cor, espaco, raio, sombra e tipografia vem de custom properties `--rvm-*`, com tema claro e escuro. Documentacao com exemplos: https://design.rvmit.com.br

        """;

    private const string Regras =
        """

        ## Como instalar

        1. `dotnet add package RVM.DesignSystem` (feed BaGet do ecossistema RVM).
        2. No `Program.cs`: `builder.Services.AddRvmDesignSystem();` — registra o `RvmSnackbarService`.
        3. No `<head>` (index.html ou App.razor): `<link rel="stylesheet" href="_content/RVM.DesignSystem/rvm-design-system.css" />` e `<script src="_content/RVM.DesignSystem/rvm-theme.js"></script>`.
        4. `class="rvm-root"` no `<body>` (fonte, cor de texto e foco visivel) e o layout dentro de `<RvmThemeProvider @bind-Theme="_tema">`.
        5. `@using` dos namespaces em `_Imports.razor`: cada componente mora em `RVM.DesignSystem.Components.<Pasta>` (indicado em cada componente abaixo); enums comuns em `RVM.DesignSystem`, icones em `RVM.DesignSystem.Icons`, tema em `RVM.DesignSystem.Theming`.

        ## Regras que um agente precisa seguir

        - Use o componente `Rvm*` em vez de HTML cru estilizado. Nao misture MudBlazor nem outra biblioteca de componentes.
        - Nunca escreva cor literal (`#hex`, `rgb()`) em CSS: use `var(--rvm-...)`. Cor literal quebra o tema escuro.
        - Cor de TEXTO e `--rvm-color-<papel>-text`, nunca `-main`: so o primary passa contraste AA como texto no `-main`.
        - Texto sobre fundo `-main` usa `--rvm-color-<papel>-contrast`.
        - Espaco por `--rvm-space-N` (N = 1, 2, 3, 4, 5, 6, 8, 10, 12; passo de 4 px); raio por `--rvm-radius-*`; sombra por `--rvm-shadow-1` a `--rvm-shadow-5`.
        - Tipografia pelas classes `rvm-text-<estilo>` (h1-h6, subtitle1, subtitle2, body1, body2, caption, overline, button-lg/md/sm...) ou pelo `RvmTypography`.
        - Cor, tamanho e variante por enum (`RvmColor`, `RvmSize`, `Rvm*Variant`), nunca por string.
        - Atributo HTML extra (`class`, `style`, `id`, `data-*`, `aria-*`) passa direto: todo componente repassa `AdditionalAttributes` ao elemento raiz.
        - Classe CSS propria do consumidor nao deve comecar com `rvm-` (prefixo reservado da biblioteca).
        - Icones: `<RvmIcon Name="RvmIconName.X" />`, conjunto Tabler (lista em `RvmIconName`).
        - Texto visivel ao usuario em portugues do Brasil, explicativo, sem codigo interno.
        - Funciona em Blazor Server e WebAssembly; nenhum componente precisa de JS para o estado inicial.

        """;
}
