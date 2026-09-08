using System.Text.RegularExpressions;
using RVM.DesignSystem.Theming;

namespace RVM.DesignSystem.Tests.Theming;

/// <summary>
/// O portao que impede componente de cravar cor (`04-modelo-de-dados.md` § Tres camadas).
/// </summary>
/// <remarks>
/// <b>Desvio declarado do `04`.</b> A spec descreve tres camadas de token e a regra
/// "componente le semantico, nunca primitivo", com um teste que varre o CSS atras de
/// <c>--rvm-blue-500</c>. Na implementacao <b>nao existe camada primitiva em CSS</b>: a paleta e
/// derivada em C# por <see cref="RvmTheme.FromSeed"/> e so os papeis semanticos chegam ao
/// navegador. Um <c>--rvm-blue-500</c> nao tem como aparecer, porque nunca e emitido.
///
/// A regra continua valendo, numa forma mais forte e verificavel: <b>CSS de componente so pode
/// referenciar token que existe, e nao pode cravar cor literal.</b> Isso pega o defeito original
/// (botao que fica azul no Fiscal, que e teal) e mais dois que a regra da spec deixava passar —
/// o <c>#0288d1</c> escrito direto e o <c>var(--rvm-color-primry)</c> com erro de digitacao, que
/// falha em silencio porque CSS ignora custom property inexistente.
/// </remarks>
public class CssDeComponenteTests
{
    private static readonly Regex ReferenciaDeToken =
        new(@"var\(\s*(--rvm-[a-z0-9-]+)", RegexOptions.Compiled);

    private static readonly Regex CorLiteral =
        new(@"(#[0-9a-fA-F]{3,8}\b|\brgba?\(|\bhsla?\(|\boklch\(|\boklab\()", RegexOptions.Compiled);

    /// <summary>Nomes de token que o componente pode usar: os das escalas e os do tema.</summary>
    private static HashSet<string> TokensConhecidos()
    {
        var tokens = new HashSet<string>(StringComparer.Ordinal);

        foreach (Match m in Regex.Matches(File.ReadAllText(ArquivoDeTokens()), @"(--rvm-[a-z0-9-]+)\s*:"))
        {
            tokens.Add(m.Groups[1].Value);
        }

        foreach (Match m in Regex.Matches(RvmThemes.Rvm.ToCss(), @"(--rvm-[a-z0-9-]+)\s*:"))
        {
            tokens.Add(m.Groups[1].Value);
        }

        return tokens;
    }

    /// <summary>Acha a raiz do repositorio subindo a partir da pasta do assembly de teste.</summary>
    private static DirectoryInfo RaizDoRepositorio()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "RVM.DesignSystem.slnx")))
        {
            dir = dir.Parent;
        }

        Assert.True(dir is not null, "Nao achei a raiz do repositorio a partir de " + AppContext.BaseDirectory);
        return dir!;
    }

    private static string ArquivoDeTokens() =>
        Path.Combine(RaizDoRepositorio().FullName, "src", "RVM.DesignSystem", "wwwroot", "rvm-tokens.css");

    private static string[] CssDeComponentes() =>
        Directory.GetFiles(
            Path.Combine(RaizDoRepositorio().FullName, "src", "RVM.DesignSystem"),
            "*.razor.css",
            SearchOption.AllDirectories);

    /// <summary>A regra, isolada para poder ser testada com entrada ruim de proposito.</summary>
    internal static List<string> Violacoes(string css, string rotulo, ISet<string> conhecidos)
    {
        var problemas = new List<string>();

        foreach (Match m in ReferenciaDeToken.Matches(css))
        {
            var token = m.Groups[1].Value;
            if (!conhecidos.Contains(token))
            {
                problemas.Add($"  {rotulo}: usa {token}, que nao existe em nenhuma camada de token");
            }
        }

        foreach (var linha in css.Split('\n'))
        {
            // Comentario pode citar um hex ao explicar de onde a cor veio; nao e a cor aplicada.
            var semComentario = Regex.Replace(linha, @"/\*.*?\*/", string.Empty);
            if (CorLiteral.IsMatch(semComentario))
            {
                problemas.Add($"  {rotulo}: crava cor literal em '{linha.Trim()}' — use var(--rvm-color-*)");
            }
        }

        return problemas;
    }

    [Fact]
    public void Nenhum_componente_crava_cor_nem_usa_token_inexistente()
    {
        var conhecidos = TokensConhecidos();
        var problemas = new List<string>();

        foreach (var arquivo in CssDeComponentes())
        {
            problemas.AddRange(Violacoes(File.ReadAllText(arquivo), Path.GetFileName(arquivo), conhecidos));
        }

        Assert.True(problemas.Count == 0, $"CSS de componente com problema:\n{string.Join("\n", problemas)}");
    }

    [Fact]
    public void O_portao_esta_realmente_ligado_nos_arquivos_certos()
    {
        // Enquanto nao ha componente, o teste acima passa sem varrer nada — e portao que passa
        // sem medir e pior que portao nenhum, porque da confianca falsa. Este teste garante que
        // a descoberta funciona: se a estrutura de pastas mudar, ele cai antes.
        Assert.True(File.Exists(ArquivoDeTokens()), "Nao achei o rvm-tokens.css.");
        Assert.NotEmpty(TokensConhecidos());
        Assert.Contains("--rvm-space-4", TokensConhecidos());
        Assert.Contains("--rvm-color-primary", TokensConhecidos());
    }

    [Theory]
    [InlineData("a{color:#0288d1;}", "cor literal em hex")]
    [InlineData("a{background:rgb(2 136 209);}", "cor literal em rgb()")]
    [InlineData("a{color:oklch(52% 0.16 245);}", "cor literal em oklch()")]
    [InlineData("a{color:var(--rvm-color-primry);}", "token com erro de digitacao")]
    [InlineData("a{color:var(--rvm-blue-500);}", "token primitivo, que nao existe")]
    public void O_portao_REPROVA_o_que_tem_que_reprovar(string css, string oQue)
    {
        var problemas = Violacoes(css, "teste.razor.css", TokensConhecidos());

        Assert.True(problemas.Count > 0, $"O portao deixou passar: {oQue}.");
    }

    [Fact]
    public void O_portao_APROVA_css_correto()
    {
        const string css = """
            .rvm-button {
                /* referencia a marca por papel, nunca por cor: #641974 nao aparece aqui */
                background: var(--rvm-color-primary);
                color: var(--rvm-color-on-primary);
                padding: var(--rvm-space-2) var(--rvm-space-4);
                border-radius: var(--rvm-radius-md);
                transition: background var(--rvm-motion-duration-fast) var(--rvm-motion-ease-standard);
            }
            """;

        Assert.Empty(Violacoes(css, "ok.razor.css", TokensConhecidos()));
    }
}
