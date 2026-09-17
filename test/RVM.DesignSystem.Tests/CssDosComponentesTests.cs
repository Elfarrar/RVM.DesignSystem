using System.Text.RegularExpressions;

namespace RVM.DesignSystem.Tests;

/// <summary>
/// Guarda do contrato do `CLAUDE.md`: "CSS isolado por componente, consumindo SO custom properties.
/// Hex literal em CSS de componente e bug — quebra o tema escuro." Um teste, porque revisao humana
/// nao pega isso de forma confiavel em 35 componentes.
/// </summary>
public class CssDosComponentesTests
{
    private static readonly string PastaDeComponentes = RaizDoRepositorio.Biblioteca("Components");

    [Fact]
    public void Nenhum_css_de_componente_escreve_cor_literal()
    {
        var infratores = Arquivos()
            .SelectMany(arquivo => SemComentarios(File.ReadAllText(arquivo)).Split('\n')
                .Select((linha, i) => (arquivo, numero: i + 1, linha))
                .Where(x => Regex.IsMatch(x.linha, @"#[0-9a-fA-F]{3,8}\b")
                            || Regex.IsMatch(x.linha, @"\b(rgb|rgba|hsl|hsla)\s*\(")))
            .Select(x => $"{Path.GetFileName(x.arquivo)}:{x.numero} -> {x.linha.Trim()}")
            .ToArray();

        Assert.Empty(infratores);
    }

    [Fact]
    public void Toda_classe_de_componente_tem_o_prefixo_rvm()
    {
        // CSS isolado impede o componente de VAZAR, mas nao de RECEBER regra global: uma classe
        // `.conteudo` no CSS do consumidor casava com o `.conteudo` do dialogo e do card (mordeu duas
        // vezes no proprio site). Prefixo `rvm-` em toda classe interna — decisao do Rafael, 17/09/2026.
        var infratores = Arquivos()
            .SelectMany(arquivo => Regex.Matches(SemComentarios(File.ReadAllText(arquivo)), @"\.([a-zA-Z][\w-]*)")
                .Select(m => m.Groups[1].Value)
                .Where(classe => !classe.StartsWith("rvm-", StringComparison.Ordinal))
                .Select(classe => $"{Path.GetFileName(arquivo)} -> .{classe}"))
            .Distinct()
            .ToArray();

        Assert.Empty(infratores);
    }

    [Fact]
    public void A_guarda_ignora_hex_em_comentario_mas_continua_pegando_hex_em_declaracao()
    {
        // Prova nos dois sentidos: ignorar comentario nao pode ter cegado a guarda.
        const string css = "/* medido no kit: #626B9C */\n.a { color: var(--rvm-x); }\n.b { color: #FF0000; }";

        var linhas = SemComentarios(css).Split('\n');

        Assert.DoesNotMatch(@"#[0-9a-fA-F]{3,8}", linhas[0]);
        Assert.DoesNotMatch(@"#[0-9a-fA-F]{3,8}", linhas[1]);
        Assert.Matches(@"#[0-9a-fA-F]{3,8}", linhas[2]);
    }

    [Fact]
    public void Todo_componente_com_razor_tem_o_css_isolado_ao_lado()
    {
        // Componente sem `.razor.css` ate pode existir (o RvmTypography e C# puro e usa as classes
        // da escala), mas `.razor` sem CSS costuma ser esquecimento — entao a excecao e nomeada.
        // Os itens do menu sao pecas do RvmAppShell e so existem dentro dele: o estilo mora no CSS
        // da moldura (::deep), que precisa enxergar o estado recolhido e o modo estreito juntos.
        // Os graficos derivados so desenham SVG dentro da figura da RvmChartBase, que tem o CSS (::deep).
        string[] semCssPorProjeto = ["RvmNavGroup.razor", "RvmNavItem.razor", "RvmNavSection.razor",
            "RvmColumnChart.razor", "RvmBarChart.razor", "RvmHistogram.razor",
            "RvmLineChart.razor", "RvmAreaChart.razor", "RvmScatterChart.razor"];

        var faltando = Directory.Exists(PastaDeComponentes)
            ? Directory.GetFiles(PastaDeComponentes, "*.razor", SearchOption.AllDirectories)
                .Where(r => !File.Exists(r + ".css"))
                .Select(Path.GetFileName)
                .Where(nome => !semCssPorProjeto.Contains(nome))
                .ToArray()
            : [];

        Assert.Empty(faltando);
    }

    /// <summary>
    /// Apaga o conteudo dos comentarios preservando as quebras de linha — assim o numero de linha do
    /// infrator continua certo. Comentario que CITA um hex medido do kit e documentacao, nao cor
    /// aplicada; a primeira versao desta guarda reprovou exatamente isso no RvmButton.
    /// </summary>
    internal static string SemComentarios(string css)
        => Regex.Replace(css, @"/\*.*?\*/", m => Regex.Replace(m.Value, "[^\n]", " "), RegexOptions.Singleline);

    private static IEnumerable<string> Arquivos()
        => Directory.Exists(PastaDeComponentes)
            ? Directory.GetFiles(PastaDeComponentes, "*.razor.css", SearchOption.AllDirectories)
            : [];
}
