using System.Text.RegularExpressions;
using Bunit;
using RVM.DesignSystem.Components.Typography;

namespace RVM.DesignSystem.Tests.Components;

public class RvmTypographyTests : BunitContext
{
    [Fact]
    public void Sem_parametro_sai_um_paragrafo_no_estilo_body1()
    {
        var cortado = Render<RvmTypography>(p => p.AddChildContent("oi"));

        var elemento = cortado.Find("p");
        Assert.Equal("rvm-text-body1", elemento.GetAttribute("class"));
        Assert.Equal("oi", elemento.TextContent);
    }

    [Theory]
    [InlineData(RvmTypographyVariant.H1, "h1", "rvm-text-h1")]
    [InlineData(RvmTypographyVariant.H6, "h6", "rvm-text-h6")]
    [InlineData(RvmTypographyVariant.Subtitle1, "p", "rvm-text-subtitle1")]
    [InlineData(RvmTypographyVariant.Caption, "span", "rvm-text-caption")]
    [InlineData(RvmTypographyVariant.Overline, "span", "rvm-text-overline")]
    public void O_estilo_escolhe_a_tag_quando_ninguem_manda_outra(
        RvmTypographyVariant variante, string tag, string classe)
    {
        var cortado = Render<RvmTypography>(p => p.Add(x => x.Variant, variante).AddChildContent("x"));

        var elemento = cortado.Find(tag);
        Assert.Equal(classe, elemento.GetAttribute("class"));
    }

    [Fact]
    public void Estilo_e_semantica_sao_separados()
    {
        // O caso real: um titulo de secao que precisa do TAMANHO de h5 e continua sendo o h2 da
        // ordem do documento. Misturar os dois quebra a navegacao por cabecalho no leitor de tela.
        var cortado = Render<RvmTypography>(p => p
            .Add(x => x.Variant, RvmTypographyVariant.H5)
            .Add(x => x.As, RvmTextElement.H2)
            .AddChildContent("Secao"));

        var elemento = cortado.Find("h2");
        Assert.Equal("rvm-text-h5", elemento.GetAttribute("class"));
    }

    [Fact]
    public void Papel_do_texto_vira_token_de_cor()
    {
        var cortado = Render<RvmTypography>(p => p
            .Add(x => x.Color, RvmTextColor.Secondary)
            .AddChildContent("apoio"));

        Assert.Contains("var(--rvm-color-text-secondary)", cortado.Find("p").GetAttribute("style"));
    }

    [Fact]
    public void Sem_papel_de_cor_o_elemento_nao_ganha_style()
    {
        var cortado = Render<RvmTypography>(p => p.AddChildContent("herda"));

        Assert.False(cortado.Find("p").HasAttribute("style"));
    }

    [Fact]
    public void Classe_e_style_do_consumidor_somam_em_vez_de_substituir()
    {
        var cortado = Render<RvmTypography>(p => p
            .Add(x => x.Color, RvmTextColor.Primary)
            .AddUnmatched("class", "minha-classe")
            .AddUnmatched("style", "margin: 0;")
            .AddChildContent("x"));

        var elemento = cortado.Find("p");
        Assert.Equal("rvm-text-body1 minha-classe", elemento.GetAttribute("class"));
        Assert.Contains("var(--rvm-color-text-primary)", elemento.GetAttribute("style"));
        Assert.Contains("margin: 0;", elemento.GetAttribute("style"));
    }

    [Fact]
    public void Atributos_extras_chegam_ao_elemento()
    {
        var cortado = Render<RvmTypography>(p => p
            .AddUnmatched("id", "titulo")
            .AddUnmatched("data-teste", "1")
            .AddChildContent("x"));

        var elemento = cortado.Find("p");
        Assert.Equal("titulo", elemento.GetAttribute("id"));
        Assert.Equal("1", elemento.GetAttribute("data-teste"));
    }

    [Fact]
    public void Todo_estilo_da_escala_aponta_para_uma_classe_que_existe_no_CSS()
    {
        // Sem este teste, um estilo novo com sufixo errado renderiza uma classe inexistente e o
        // texto sai no tamanho do body sem ninguem notar — o HTML fica "certo", so a fonte erra.
        var css = File.ReadAllText(RaizDoRepositorio.Biblioteca("wwwroot", "rvm-design-system.css"));
        var noCss = Regex.Matches(css, @"\.rvm-text-([a-z0-9-]+)\s*\{")
            .Select(m => m.Groups[1].Value)
            .ToHashSet();

        var faltando = Enum.GetValues<RvmTypographyVariant>()
            .Select(v => new
            {
                Variante = v,
                Classe = Render<RvmTypography>(p => p.Add(x => x.Variant, v).AddChildContent("x"))
                    .Find("*").GetAttribute("class")!
            })
            .Where(x => !noCss.Contains(x.Classe.Replace("rvm-text-", string.Empty)))
            .ToArray();

        Assert.Empty(faltando);
        Assert.Equal(24, Enum.GetValues<RvmTypographyVariant>().Length);
    }
}
