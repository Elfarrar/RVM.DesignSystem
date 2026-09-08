using System.Xml.Linq;
using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Tests.Documentacao;

/// <summary>
/// Guarda o XML de documentacao que o site publica.
/// </summary>
/// <remarks>
/// <b>Por que o XML e commitado, e nao gerado no build do site.</b> A copia dele para o wwwroot
/// acontece por um target do MSBuild que roda depois do compilador escrever o arquivo — e nessa
/// altura os <i>static web assets</i> do projeto ja foram enumerados. O resultado e traicoeiro:
/// em maquina de desenvolvimento funciona, porque o arquivo sobrou do build anterior; num build
/// LIMPO ele fica de fora do publish, e a tabela de API do site aparece vazia. Ou seja, quebra
/// exatamente no CI, e passa em todo lugar onde alguem olharia.
///
/// <para>
/// A saida foi a mesma dos icones: o artefato e <b>commitado</b>, o build local o mantem fresco,
/// e <b>este teste fecha o buraco que sobra</b> — alguem acrescentar um parametro e esquecer de
/// commitar o XML atualizado. Sem ele, a doc do parametro novo sumiria em silencio.
/// </para>
/// </remarks>
public class XmlDeDocTests
{
    private static DirectoryInfo Raiz()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "RVM.DesignSystem.slnx")))
        {
            dir = dir.Parent;
        }

        Assert.True(dir is not null, "Nao achei a raiz do repositorio.");
        return dir!;
    }

    private static string CaminhoDoXml() => Path.Combine(
        Raiz().FullName, "src", "RVM.DesignSystem.Docs", "wwwroot", "api", "RVM.DesignSystem.xml");

    [Fact]
    public void O_XML_publicado_existe()
    {
        Assert.True(
            File.Exists(CaminhoDoXml()),
            "O XML de doc nao esta em src/RVM.DesignSystem.Docs/wwwroot/api/. " +
            "Rode `dotnet build -c Release` e commite o arquivo — o site le ele para montar a tabela de API.");
    }

    [Fact]
    public void Todo_parametro_publico_tem_documentacao_no_XML_publicado()
    {
        var doc = XDocument.Load(CaminhoDoXml());

        var documentados = doc.Descendants("member")
            .Where(m => m.Attribute("name") is not null && m.Element("summary") is not null)
            .Select(m => m.Attribute("name")!.Value)
            .ToHashSet(StringComparer.Ordinal);

        var componentes = typeof(RvmDesignSystemOptions).Assembly.GetTypes()
            .Where(t => t.IsPublic && typeof(IComponent).IsAssignableFrom(t))
            .ToList();

        Assert.NotEmpty(componentes);

        var faltando = new List<string>();

        foreach (var componente in componentes)
        {
            foreach (var p in componente.GetProperties())
            {
                if (!p.GetCustomAttributes(inherit: true).Any(a => a is ParameterAttribute))
                {
                    continue;
                }

                var declarante = p.DeclaringType ?? componente;

                // So o que E NOSSO. Value, ValueChanged, ValueExpression e DisplayName vem do
                // InputBase<T> do proprio Blazor e sao documentados na assembly da Microsoft —
                // cobrar summary deles aqui seria cobrar de codigo que nao escrevemos.
                if (declarante.Assembly != componente.Assembly)
                {
                    continue;
                }

                var nomeDoTipo = declarante.IsGenericType
                    ? $"{declarante.Namespace}.{declarante.Name}"
                    : declarante.FullName ?? declarante.Name;

                var chave = $"P:{nomeDoTipo}.{p.Name}";

                if (!documentados.Contains(chave))
                {
                    faltando.Add($"  {componente.Name}.{p.Name}  (procurado como {chave})");
                }
            }
        }

        Assert.True(
            faltando.Count == 0,
            "Parametros sem entrada no XML publicado. Ou falta o /// <summary>, ou o XML em "
            + "wwwroot/api/ esta velho — rode `dotnet build -c Release` e commite:\n"
            + string.Join("\n", faltando));
    }
}
