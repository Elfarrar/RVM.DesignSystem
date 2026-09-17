using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RVM.DesignSystem;
using RVM.DesignSystem.Components.Button;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

public class RvmButtonTests : BunitContext
{
    [Fact]
    public void Sem_parametro_sai_um_botao_preenchido_medio_primario()
    {
        var cortado = Render<RvmButton>(p => p.AddChildContent("Salvar"));

        var botao = cortado.Find("button");
        Assert.Equal("rvm-botao rvm-preenchido rvm-medio rvm-primary", botao.GetAttribute("class"));
        Assert.Equal("Salvar", botao.TextContent.Trim());
    }

    [Fact]
    public void O_type_padrao_e_button_e_nao_submit()
    {
        // O padrao do HTML e submit: um "cancelar" que esqueca disso envia o formulario sem querer.
        var cortado = Render<RvmButton>(p => p.AddChildContent("Cancelar"));

        Assert.Equal("button", cortado.Find("button").GetAttribute("type"));
    }

    [Fact]
    public void Quem_envia_o_formulario_declara()
    {
        var cortado = Render<RvmButton>(p => p
            .Add(x => x.Type, RvmButtonType.Submit)
            .AddChildContent("Enviar"));

        Assert.Equal("submit", cortado.Find("button").GetAttribute("type"));
    }

    [Theory]
    [InlineData(RvmButtonVariant.Contained, "rvm-preenchido")]
    [InlineData(RvmButtonVariant.Outlined, "rvm-contorno")]
    [InlineData(RvmButtonVariant.Text, "rvm-texto")]
    public void O_peso_visual_vira_classe(RvmButtonVariant variante, string classe)
    {
        var cortado = Render<RvmButton>(p => p.Add(x => x.Variant, variante).AddChildContent("x"));

        Assert.Contains(classe, cortado.Find("button").GetAttribute("class"));
    }

    [Theory]
    [InlineData(RvmSize.Small, "rvm-pequeno")]
    [InlineData(RvmSize.Medium, "rvm-medio")]
    [InlineData(RvmSize.Large, "rvm-grande")]
    public void O_tamanho_vira_classe(RvmSize tamanho, string classe)
    {
        var cortado = Render<RvmButton>(p => p.Add(x => x.Size, tamanho).AddChildContent("x"));

        Assert.Contains(classe, cortado.Find("button").GetAttribute("class"));
    }

    [Theory]
    [InlineData(RvmColor.Primary, "rvm-primary")]
    [InlineData(RvmColor.Secondary, "rvm-secondary")]
    [InlineData(RvmColor.Error, "rvm-error")]
    public void O_papel_de_cor_vira_classe(RvmColor cor, string classe)
    {
        var cortado = Render<RvmButton>(p => p.Add(x => x.Color, cor).AddChildContent("x"));

        Assert.Contains(classe, cortado.Find("button").GetAttribute("class"));
    }

    [Fact]
    public async Task O_clique_chama_quem_pediu()
    {
        var cliques = 0;
        var cortado = Render<RvmButton>(p => p
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => cliques++))
            .AddChildContent("Salvar"));

        await cortado.Find("button").ClickAsync(new MouseEventArgs());

        Assert.Equal(1, cliques);
    }

    [Fact]
    public async Task Desabilitado_nao_clica_nem_recebe_foco()
    {
        var cliques = 0;
        var cortado = Render<RvmButton>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => cliques++))
            .AddChildContent("Salvar"));

        var botao = cortado.Find("button");
        Assert.True(botao.HasAttribute("disabled"));

        await botao.ClickAsync(new MouseEventArgs());

        Assert.Equal(0, cliques);
    }

    [Fact]
    public async Task Carregando_bloqueia_o_clique_e_anuncia_a_espera()
    {
        // Dois cliques enquanto carrega viram duas requisicoes — e o usuario nao tem como saber
        // que a primeira ainda esta em curso se nada for anunciado.
        var cliques = 0;
        var cortado = Render<RvmButton>(p => p
            .Add(x => x.Loading, true)
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => cliques++))
            .AddChildContent("Salvando"));

        var botao = cortado.Find("button");
        Assert.True(botao.HasAttribute("disabled"));
        Assert.Equal("true", botao.GetAttribute("aria-busy"));
        Assert.NotEmpty(cortado.FindAll("span.rvm-girando svg"));

        await botao.ClickAsync(new MouseEventArgs());

        Assert.Equal(0, cliques);
    }

    [Fact]
    public void Sem_carregar_nao_ha_aria_busy()
    {
        var cortado = Render<RvmButton>(p => p.AddChildContent("Salvar"));

        Assert.False(cortado.Find("button").HasAttribute("aria-busy"));
    }

    [Fact]
    public void Icone_dos_dois_lados()
    {
        var cortado = Render<RvmButton>(p => p
            .Add(x => x.StartIcon, RvmIconName.Plus)
            .Add(x => x.EndIcon, RvmIconName.ChevronRight)
            .AddChildContent("Novo"));

        Assert.Equal(2, cortado.FindAll("svg").Count);
    }

    [Fact]
    public void Enquanto_carrega_o_icone_da_direita_sai_de_cena()
    {
        // Ficariam tres coisas na linha (girando, rotulo, icone) e o botao mudaria de largura no
        // meio da espera.
        var cortado = Render<RvmButton>(p => p
            .Add(x => x.Loading, true)
            .Add(x => x.EndIcon, RvmIconName.ChevronRight)
            .AddChildContent("Salvando"));

        Assert.Single(cortado.FindAll("svg"));
        Assert.NotEmpty(cortado.FindAll("span.rvm-girando"));
    }

    [Fact]
    public void Classe_e_atributos_do_consumidor_chegam_ao_botao()
    {
        var cortado = Render<RvmButton>(p => p
            .AddUnmatched("class", "minha")
            .AddUnmatched("data-teste", "1")
            .AddChildContent("x"));

        var botao = cortado.Find("button");
        Assert.Equal("rvm-botao rvm-preenchido rvm-medio rvm-primary minha", botao.GetAttribute("class"));
        Assert.Equal("1", botao.GetAttribute("data-teste"));
    }
}
