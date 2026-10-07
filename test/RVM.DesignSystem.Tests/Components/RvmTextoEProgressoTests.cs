using Bunit;
using RVM.DesignSystem.Components.Label;
using RVM.DesignSystem.Components.Progress;
using RVM.DesignSystem.Components.Typography;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>RvmSpinner, RvmProgressBar, RvmLabel e RvmText — contrato com o RVM.UI (DSGN-017).</summary>
public class RvmTextoEProgressoTests : BunitContext
{
    // --- RvmSpinner ---

    [Fact]
    public void Spinner_e_um_status_com_o_texto_so_para_o_leitor_por_padrao()
    {
        var cortado = Render<RvmSpinner>();

        var raiz = cortado.Find("[role='status']");
        Assert.Equal("Carregando", raiz.TextContent.Trim());
        Assert.NotNull(cortado.Find(".rvm-so-leitor"));
        // O anel nao repete o nome: fica fora da arvore de acessibilidade.
        Assert.Equal("true", cortado.Find("[role='progressbar']").GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Spinner_com_ShowLabel_mostra_o_texto_e_Centered_centraliza()
    {
        var cortado = Render<RvmSpinner>(p => p
            .Add(x => x.Label, "Carregando talhoes")
            .Add(x => x.ShowLabel, true)
            .Add(x => x.Centered, true)
            .Add(x => x.Size, RvmSize.Large)
            .Add(x => x.Class, "extra")
            .AddUnmatched("data-teste", "s"));

        var raiz = cortado.Find("[role='status']");
        Assert.Empty(cortado.FindAll(".rvm-so-leitor"));
        Assert.Contains("Carregando talhoes", raiz.TextContent);
        Assert.Contains("rvm-centralizado", raiz.ClassList);
        Assert.Contains("extra", raiz.ClassList);
        Assert.Equal("s", raiz.GetAttribute("data-teste"));
        Assert.Contains("rvm-grande", cortado.Find("[role='progressbar']").ClassList);
    }

    // --- RvmProgressBar ---

    [Theory]
    [InlineData(42.4, "42")]
    [InlineData(150, "100")]
    [InlineData(-3, "0")]
    [InlineData(double.NaN, "0")]
    public void ProgressBar_limita_o_valor_entre_0_e_100(double valor, string esperado)
    {
        var cortado = Render<RvmProgressBar>(p => p.Add(x => x.Value, valor).Add(x => x.AriaLabel, "Meta"));

        var barra = cortado.Find("[role='progressbar']");
        Assert.Equal(esperado, barra.GetAttribute("aria-valuenow"));
        Assert.Equal("0", barra.GetAttribute("aria-valuemin"));
        Assert.Equal("100", barra.GetAttribute("aria-valuemax"));
        Assert.Equal($"{esperado}%", barra.GetAttribute("aria-valuetext"));
        Assert.Equal("Meta", barra.GetAttribute("aria-label"));
    }

    [Fact]
    public void ProgressBar_mostra_rotulo_e_valor_e_o_rotulo_vira_o_nome()
    {
        var cortado = Render<RvmProgressBar>(p => p
            .Add(x => x.Value, 72.6)
            .Add(x => x.Label, "Colheita da soja")
            .Add(x => x.AriaLabel, "ignorado")
            .Add(x => x.ShowValue, true)
            .Add(x => x.LabelVariant, RvmProgressLabelVariant.Amount)
            .Add(x => x.Color, RvmColor.Success)
            .Add(x => x.Size, RvmSize.Small));

        Assert.Equal("Colheita da soja", cortado.Find("[role='progressbar']").GetAttribute("aria-label"));
        Assert.Contains("rvm-quantia", cortado.Find(".rvm-rotulo").ClassList);
        Assert.Equal("73%", cortado.Find(".rvm-valor").TextContent);
        Assert.Equal("width: 72.6%", cortado.Find(".rvm-barra").GetAttribute("style"));
        var raiz = cortado.Find(".rvm-barra-de-progresso");
        Assert.Contains("rvm-success", raiz.ClassList);
        Assert.Contains("rvm-pequeno", raiz.ClassList);
        Assert.Equal("true", cortado.Find(".rvm-cabecalho").GetAttribute("aria-hidden"));
    }

    [Fact]
    public void ProgressBar_sem_valor_nao_tem_aria_valuenow_nem_preenchimento()
    {
        var cortado = Render<RvmProgressBar>(p => p
            .Add(x => x.Value, 50)
            .Add(x => x.Label, "Umidade")
            .Add(x => x.NoValue, true)
            .Add(x => x.ShowValue, true)
            .Add(x => x.Surface, RvmSurface.Dark));

        var barra = cortado.Find("[role='progressbar']");
        Assert.False(barra.HasAttribute("aria-valuenow"));
        Assert.False(barra.HasAttribute("aria-valuemin"));
        Assert.Equal("Nao informado", barra.GetAttribute("aria-valuetext"));
        Assert.Empty(cortado.FindAll(".rvm-barra"));
        Assert.Equal("Nao informado", cortado.Find(".rvm-valor").TextContent);
        var raiz = cortado.Find(".rvm-barra-de-progresso");
        Assert.Contains("rvm-superficie-escura", raiz.ClassList);
        Assert.Contains("rvm-sem-valor", raiz.ClassList);
    }

    [Fact]
    public void ProgressBar_sem_Label_nem_AriaLabel_lanca()
    {
        Assert.Throws<ArgumentException>(() => Render<RvmProgressBar>(p => p.Add(x => x.Value, 10)));
    }

    // --- RvmLabel ---

    [Fact]
    public void Label_e_um_span_suave_no_accent_por_padrao()
    {
        var cortado = Render<RvmLabel>(p => p.AddChildContent("Pago"));

        var raiz = cortado.Find("span");
        Assert.Equal("Pago", raiz.TextContent);
        Assert.Equal("rvm-etiqueta rvm-suave rvm-media rvm-primary", raiz.GetAttribute("class"));
        Assert.Empty(cortado.FindAll("label"));
    }

    [Theory]
    [InlineData(RvmColor.Danger, "rvm-error")]
    [InlineData(RvmColor.Neutral, "rvm-secondary")]
    [InlineData(RvmColor.Warning, "rvm-warning")]
    public void Label_cheia_usa_o_papel_e_o_tamanho(RvmColor cor, string classe)
    {
        var cortado = Render<RvmLabel>(p => p
            .Add(x => x.Variant, RvmLabelVariant.Solid)
            .Add(x => x.Color, cor)
            .Add(x => x.Size, RvmSize.Large)
            .Add(x => x.Class, "extra")
            .AddUnmatched("title", "Status")
            .AddChildContent("x"));

        var raiz = cortado.Find("span");
        Assert.Equal($"rvm-etiqueta rvm-cheia rvm-grande {classe} extra", raiz.GetAttribute("class"));
        Assert.Equal("Status", raiz.GetAttribute("title"));
    }

    // --- RvmText ---

    [Fact]
    public void Text_sem_parametro_e_paragrafo_body2_na_cor_principal()
    {
        var cortado = Render<RvmText>(p => p.AddChildContent("oi"));

        var elemento = cortado.Find("p");
        Assert.Equal("rvm-text-body2", elemento.GetAttribute("class"));
        Assert.Equal("color: var(--rvm-color-text-primary);", elemento.GetAttribute("style"));
    }

    [Theory]
    [InlineData(RvmTextVariant.DisplayXl, "h1", "rvm-text-h3")]
    [InlineData(RvmTextVariant.DisplayL, "h2", "rvm-text-h4")]
    [InlineData(RvmTextVariant.DisplayM, "h3", "rvm-text-h5")]
    [InlineData(RvmTextVariant.DisplayS, "h4", "rvm-text-h6")]
    [InlineData(RvmTextVariant.TextXl, "p", "rvm-text-subtitle1")]
    [InlineData(RvmTextVariant.TextL, "p", "rvm-text-input")]
    [InlineData(RvmTextVariant.TextS, "p", "rvm-text-caption")]
    [InlineData(RvmTextVariant.TextXs, "p", "rvm-text-tooltip")]
    public void Text_cada_nivel_vai_para_um_estilo_da_escala(RvmTextVariant variante, string tag, string classe)
    {
        var cortado = Render<RvmText>(p => p.Add(x => x.Variant, variante).AddChildContent("x"));

        Assert.Equal(classe, cortado.Find(tag).GetAttribute("class"));
    }

    [Theory]
    [InlineData(RvmTextElement.H5, "h5")]
    [InlineData(RvmTextElement.H6, "h6")]
    [InlineData(RvmTextElement.H1, "h1")]
    [InlineData(RvmTextElement.H2, "h2")]
    [InlineData(RvmTextElement.H3, "h3")]
    [InlineData(RvmTextElement.H4, "h4")]
    [InlineData(RvmTextElement.P, "p")]
    [InlineData(RvmTextElement.Span, "span")]
    [InlineData(RvmTextElement.Div, "div")]
    [InlineData(RvmTextElement.Label, "label")]
    public void Text_Element_escolhe_a_tag(RvmTextElement elemento, string tag)
    {
        var cortado = Render<RvmText>(p => p
            .Add(x => x.Variant, RvmTextVariant.DisplayXl)
            .Add(x => x.Element, elemento)
            .AddChildContent("x"));

        Assert.Equal("rvm-text-h3", cortado.Find(tag).GetAttribute("class"));
    }

    [Theory]
    [InlineData(RvmFontWeight.Regular, "var(--rvm-font-weight-regular)")]
    [InlineData(RvmFontWeight.Medium, "var(--rvm-font-weight-medium)")]
    [InlineData(RvmFontWeight.SemiBold, "600")]
    [InlineData(RvmFontWeight.Bold, "700")]
    public void Text_Weight_vai_no_style(RvmFontWeight peso, string valor)
    {
        var cortado = Render<RvmText>(p => p
            .Add(x => x.Weight, peso)
            .Add(x => x.Color, RvmTextColor.Inherit)
            .AddChildContent("x"));

        Assert.Equal($"font-weight: {valor};", cortado.Find("p").GetAttribute("style"));
    }

    [Fact]
    public void Text_mescla_cor_peso_class_e_style_do_consumidor()
    {
        var cortado = Render<RvmText>(p => p
            .Add(x => x.Color, RvmTextColor.Danger)
            .Add(x => x.Weight, RvmFontWeight.Medium)
            .Add(x => x.Class, "extra")
            .AddUnmatched("style", "margin: 0")
            .AddUnmatched("data-x", "1")
            .AddChildContent("x"));

        var elemento = cortado.Find("p");
        Assert.Equal("rvm-text-body2 extra", elemento.GetAttribute("class"));
        Assert.Equal("color: var(--rvm-color-error-text); font-weight: var(--rvm-font-weight-medium); margin: 0",
            elemento.GetAttribute("style"));
        Assert.Equal("1", elemento.GetAttribute("data-x"));
    }

    [Fact]
    public void Text_herdando_cor_sem_peso_nao_escreve_style()
    {
        var cortado = Render<RvmText>(p => p.Add(x => x.Color, RvmTextColor.Inherit).AddChildContent("x"));

        Assert.False(cortado.Find("p").HasAttribute("style"));
    }
}
