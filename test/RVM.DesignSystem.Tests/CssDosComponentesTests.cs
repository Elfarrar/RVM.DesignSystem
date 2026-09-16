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
            .SelectMany(arquivo => File.ReadLines(arquivo)
                .Select((linha, i) => (arquivo, numero: i + 1, linha))
                .Where(x => Regex.IsMatch(x.linha, @"#[0-9a-fA-F]{3,8}\b")
                            || Regex.IsMatch(x.linha, @"\b(rgb|rgba|hsl|hsla)\s*\(")))
            .Select(x => $"{Path.GetFileName(x.arquivo)}:{x.numero} -> {x.linha.Trim()}")
            .ToArray();

        Assert.Empty(infratores);
    }

    [Fact]
    public void Todo_componente_com_razor_tem_o_css_isolado_ao_lado()
    {
        // Componente sem `.razor.css` ate pode existir (o RvmTypography e C# puro e usa as classes
        // da escala), mas `.razor` sem CSS costuma ser esquecimento — entao a excecao e nomeada.
        string[] semCssPorProjeto = [];

        var faltando = Directory.Exists(PastaDeComponentes)
            ? Directory.GetFiles(PastaDeComponentes, "*.razor", SearchOption.AllDirectories)
                .Where(r => !File.Exists(r + ".css"))
                .Select(Path.GetFileName)
                .Where(nome => !semCssPorProjeto.Contains(nome))
                .ToArray()
            : [];

        Assert.Empty(faltando);
    }

    private static IEnumerable<string> Arquivos()
        => Directory.Exists(PastaDeComponentes)
            ? Directory.GetFiles(PastaDeComponentes, "*.razor.css", SearchOption.AllDirectories)
            : [];
}
