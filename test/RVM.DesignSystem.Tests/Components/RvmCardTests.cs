using Bunit;
using RVM.DesignSystem.Components.Card;
using RVM.DesignSystem.Components.Typography;

namespace RVM.DesignSystem.Tests.Components;

public class RvmCardTests : BunitContext
{
    [Fact]
    public void Card_basico_e_so_a_superficie_com_o_conteudo()
    {
        var cortado = Render<RvmCard>(p => p.AddChildContent("<p>Texto</p>"));

        Assert.Equal("cartao", cortado.Find("div.cartao").GetAttribute("class"));
        Assert.Equal("Texto", cortado.Find("div.conteudo p").TextContent);
        Assert.Empty(cortado.FindAll("div.cabecalho"));
        Assert.Empty(cortado.FindAll("img"));
        Assert.Empty(cortado.FindAll("div.acoes"));
    }

    [Fact]
    public void Titulo_sai_como_h3_por_padrao_no_estilo_h6()
    {
        var cortado = Render<RvmCard>(p => p.Add(x => x.Title, "Resumo do mes"));

        var titulo = cortado.Find("div.cabecalho h3");
        Assert.Equal("Resumo do mes", titulo.TextContent);
        Assert.Contains("rvm-text-h6", titulo.GetAttribute("class"));
    }

    [Fact]
    public void Elemento_do_titulo_segue_a_ordem_de_cabecalhos_da_pagina()
    {
        var cortado = Render<RvmCard>(p => p
            .Add(x => x.Title, "Resumo")
            .Add(x => x.TitleElement, RvmTextElement.H2));

        Assert.NotNull(cortado.Find("div.cabecalho h2"));
        Assert.Empty(cortado.FindAll("h3"));
    }

    [Fact]
    public void Subtitulo_sozinho_ja_abre_o_cabecalho()
    {
        var cortado = Render<RvmCard>(p => p.Add(x => x.Subheader, "Atualizado ontem"));

        Assert.Equal("Atualizado ontem", cortado.Find("div.cabecalho p").TextContent);
    }

    [Fact]
    public void Acao_do_cabecalho_sozinha_ja_abre_o_cabecalho()
    {
        var cortado = Render<RvmCard>(p => p.Add(x => x.HeaderAction, "<button class=\"menu\">...</button>"));

        Assert.NotNull(cortado.Find("div.cabecalho button.menu"));
    }

    [Fact]
    public void Midia_no_topo_com_altura_e_alt()
    {
        var cortado = Render<RvmCard>(p => p
            .Add(x => x.MediaSrc, "capa.jpg")
            .Add(x => x.MediaAlt, "Cerejas sobre fundo verde")
            .Add(x => x.MediaHeight, 160));

        var img = cortado.Find("img.midia");
        Assert.Equal("capa.jpg", img.GetAttribute("src"));
        Assert.Equal("Cerejas sobre fundo verde", img.GetAttribute("alt"));
        Assert.Equal("160", img.GetAttribute("height"));
    }

    [Fact]
    public void Midia_sem_alt_e_decorativa()
    {
        var cortado = Render<RvmCard>(p => p.Add(x => x.MediaSrc, "capa.jpg"));

        Assert.Equal(string.Empty, cortado.Find("img.midia").GetAttribute("alt"));
        Assert.Equal("200", cortado.Find("img.midia").GetAttribute("height"));
    }

    [Fact]
    public void Acoes_no_rodape()
    {
        var cortado = Render<RvmCard>(p => p.Add(x => x.Actions, "<button class=\"ler\">Ler mais</button>"));

        Assert.Equal("Ler mais", cortado.Find("div.acoes button.ler").TextContent);
    }

    [Fact]
    public void Classe_e_atributos_do_consumidor_chegam_a_raiz()
    {
        var cortado = Render<RvmCard>(p => p
            .AddUnmatched("class", "minha")
            .AddUnmatched("data-teste", "1"));

        var card = cortado.Find("div.cartao");
        Assert.Equal("cartao minha", card.GetAttribute("class"));
        Assert.Equal("1", card.GetAttribute("data-teste"));
    }
}
