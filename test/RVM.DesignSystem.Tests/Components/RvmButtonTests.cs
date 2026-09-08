using Bunit;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components;

namespace RVM.DesignSystem.Tests.Components;

public class RvmButtonTests : BunitContext
{
    [Fact]
    public void Renderiza_o_conteudo_e_as_classes_padrao()
    {
        var cut = Render<RvmButton>(p => p.AddChildContent("Salvar"));

        var botao = cut.Find("button");
        Assert.Contains("Salvar", botao.TextContent);
        Assert.Contains("rvm-button", botao.ClassList);
        Assert.Contains("rvm-button--primary", botao.ClassList);
        Assert.Contains("rvm-button--medium", botao.ClassList);
    }

    [Fact]
    public void O_tipo_padrao_e_button_e_nao_submit()
    {
        // O HTML puro assume submit quando o tipo e omitido, e isso e uma das causas mais
        // comuns de "a pagina recarregou sozinha". Este teste guarda o padrao seguro.
        var cut = Render<RvmButton>();

        Assert.Equal("button", cut.Find("button").GetAttribute("type"));
    }

    [Theory]
    [InlineData(RvmButtonType.Submit, "submit")]
    [InlineData(RvmButtonType.Reset, "reset")]
    [InlineData(RvmButtonType.Button, "button")]
    public void O_tipo_HTML_acompanha_o_parametro(RvmButtonType tipo, string esperado)
    {
        var cut = Render<RvmButton>(p => p.Add(x => x.Type, tipo));

        Assert.Equal(esperado, cut.Find("button").GetAttribute("type"));
    }

    [Theory]
    [InlineData(RvmButtonVariant.Primary, "rvm-button--primary")]
    [InlineData(RvmButtonVariant.Secondary, "rvm-button--secondary")]
    [InlineData(RvmButtonVariant.Ghost, "rvm-button--ghost")]
    [InlineData(RvmButtonVariant.Danger, "rvm-button--danger")]
    public void Cada_variante_tem_sua_classe(RvmButtonVariant variante, string classe)
    {
        var cut = Render<RvmButton>(p => p.Add(x => x.Variant, variante));

        Assert.Contains(classe, cut.Find("button").ClassList);
    }

    [Fact]
    public void A_classe_do_consumidor_SOMA_com_as_internas()
    {
        // Regra do CLAUDE.md: `Class` soma, nunca descarta. Se alguem trocar o join por uma
        // atribuicao, este teste cai.
        var cut = Render<RvmButton>(p => p.Add(x => x.Class, "meu-botao"));

        var classes = cut.Find("button").ClassList;
        Assert.Contains("meu-botao", classes);
        Assert.Contains("rvm-button", classes);
        Assert.Contains("rvm-button--primary", classes);
    }

    [Fact]
    public void Dispara_OnClick()
    {
        var cliques = 0;
        var cut = Render<RvmButton>(p => p.Add(x => x.OnClick, EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, () => cliques++)));

        cut.Find("button").Click();

        Assert.Equal(1, cliques);
    }

    [Fact]
    public void NAO_dispara_OnClick_quando_desabilitado()
    {
        var cliques = 0;
        var cut = Render<RvmButton>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.OnClick, EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, () => cliques++)));

        cut.Find("button").Click();

        Assert.Equal(0, cliques);
    }

    [Fact]
    public void NAO_dispara_OnClick_quando_carregando()
    {
        // O caso real: duplo-clique impaciente num "Salvar" gerando dois registros.
        var cliques = 0;
        var cut = Render<RvmButton>(p => p
            .Add(x => x.Loading, true)
            .Add(x => x.OnClick, EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, () => cliques++)));

        cut.Find("button").Click();
        cut.Find("button").Click();

        Assert.Equal(0, cliques);
    }

    [Fact]
    public void Carregando_marca_aria_busy_e_desabilita()
    {
        var cut = Render<RvmButton>(p => p.Add(x => x.Loading, true));

        var botao = cut.Find("button");
        Assert.Equal("true", botao.GetAttribute("aria-busy"));
        Assert.True(botao.HasAttribute("disabled"));
    }

    [Fact]
    public void Carregando_troca_o_texto_quando_LoadingText_e_informado()
    {
        var cut = Render<RvmButton>(p => p
            .Add(x => x.Loading, true)
            .Add(x => x.LoadingText, "Salvando...")
            .AddChildContent("Salvar"));

        Assert.Contains("Salvando...", cut.Find("button").TextContent);
        Assert.DoesNotContain("Salvar</span>", cut.Markup);
    }

    [Fact]
    public void Carregando_sem_LoadingText_mantem_o_conteudo()
    {
        var cut = Render<RvmButton>(p => p
            .Add(x => x.Loading, true)
            .AddChildContent("Salvar"));

        Assert.Contains("Salvar", cut.Find("button").TextContent);
    }

    [Fact]
    public void O_spinner_e_escondido_do_leitor_de_tela()
    {
        // O texto visivel ja comunica o estado; anunciar o spinner faria o leitor repetir.
        var cut = Render<RvmButton>(p => p.Add(x => x.Loading, true));

        Assert.Equal("true", cut.Find(".rvm-button__spinner").GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Atributo_extra_chega_no_elemento()
    {
        var cut = Render<RvmButton>(p => p.AddUnmatched("data-teste", "abc"));

        Assert.Equal("abc", cut.Find("button").GetAttribute("data-teste"));
    }

    [Fact]
    public void FullWidth_acrescenta_a_classe()
    {
        var cut = Render<RvmButton>(p => p.Add(x => x.FullWidth, true));

        Assert.Contains("rvm-button--full", cut.Find("button").ClassList);
    }
}
