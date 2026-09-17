using Bunit;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Accordion;
using RVM.DesignSystem.Components.Lists;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

public class RvmListTests : BunitContext
{
    private IRenderedComponent<RvmList> Lista(RenderFragment itens, bool densa = false)
        => Render<RvmList>(p => p.Add(x => x.Dense, densa).Add(x => x.AriaLabel, "Produtores").Add(x => x.ChildContent, itens));

    private static RenderFragment Item(Action<RenderTreeBuilderItem> configurar) => b =>
    {
        b.OpenComponent<RvmListItem>(0);
        configurar(new RenderTreeBuilderItem(b));
        b.CloseComponent();
    };

    /// <summary>Atalho para montar atributos de um RvmListItem sem repetir a sequencia.</summary>
    private sealed class RenderTreeBuilderItem(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder b)
    {
        private int _seq = 1;

        public RenderTreeBuilderItem Com(string nome, object? valor)
        {
            b.AddAttribute(_seq++, nome, valor);
            return this;
        }
    }

    [Fact]
    public void Lista_nativa_com_nome_e_item_de_texto_parado()
    {
        var cortado = Lista(Item(i => i
            .Com(nameof(RvmListItem.ChildContent), (RenderFragment)(c => c.AddContent(0, "Caroline Black")))
            .Com(nameof(RvmListItem.SecondaryText), "Sweet dessert brownie")
            .Com(nameof(RvmListItem.Icon), RvmIconName.Clock)));

        var ul = cortado.Find("ul");
        Assert.Equal("lista", ul.GetAttribute("class"));
        Assert.Equal("Produtores", ul.GetAttribute("aria-label"));
        var li = cortado.Find("li");
        Assert.Equal("item duas-linhas", li.GetAttribute("class"));
        Assert.Equal("Caroline Black", cortado.Find(".primario").TextContent);
        Assert.Equal("Sweet dessert brownie", cortado.Find(".secundario").TextContent);
        Assert.Equal("true", cortado.Find(".icone").GetAttribute("aria-hidden"));
        Assert.Empty(cortado.FindAll("a, button"));
        Assert.Equal("DIV", cortado.Find(".principal").TagName);
    }

    [Fact]
    public void Href_vira_link_e_selecionado_e_a_pagina_atual()
    {
        var cortado = Lista(Item(i => i
            .Com(nameof(RvmListItem.Href), "/talhoes")
            .Com(nameof(RvmListItem.Selected), true)
            .Com(nameof(RvmListItem.ChildContent), (RenderFragment)(c => c.AddContent(0, "Talhoes")))), densa: true);

        var link = cortado.Find("a.principal");
        Assert.Equal("/talhoes", link.GetAttribute("href"));
        Assert.Equal("page", link.GetAttribute("aria-current"));
        Assert.Equal("item denso selecionado", cortado.Find("li").GetAttribute("class"));
    }

    [Fact]
    public void Link_desabilitado_perde_o_href()
    {
        var cortado = Lista(Item(i => i
            .Com(nameof(RvmListItem.Href), "/talhoes")
            .Com(nameof(RvmListItem.Disabled), true)));

        var link = cortado.Find("a.principal");
        Assert.Null(link.GetAttribute("href"));
        Assert.Equal("true", link.GetAttribute("aria-disabled"));
    }

    [Fact]
    public void Onclick_vira_botao_e_acao_do_fim_fica_fora_dele()
    {
        var cliques = 0;
        var cortado = Lista(Item(i => i
            .Com(nameof(RvmListItem.OnClick), EventCallback.Factory.Create(this, () => cliques++))
            .Com(nameof(RvmListItem.StartContent), (RenderFragment)(c => c.AddMarkupContent(0, "<img alt=\"\" src=\"a.png\" />")))
            .Com(nameof(RvmListItem.EndContent), (RenderFragment)(c => c.AddMarkupContent(0, "<button class=\"favoritar\">Favoritar</button>")))));

        var botao = cortado.Find("button.principal");
        Assert.Equal("false", botao.GetAttribute("aria-pressed"));
        Assert.NotNull(botao.QuerySelector(".inicio img"));
        Assert.Null(botao.QuerySelector(".favoritar"));
        Assert.NotNull(cortado.Find(".fim .favoritar"));

        botao.Click();
        Assert.Equal(1, cliques);
    }

    [Fact]
    public void Botao_desabilitado_nao_dispara()
    {
        var cliques = 0;
        var cortado = Lista(Item(i => i
            .Com(nameof(RvmListItem.OnClick), EventCallback.Factory.Create(this, () => cliques++))
            .Com(nameof(RvmListItem.Selected), true)
            .Com(nameof(RvmListItem.Disabled), true)
            .Com("class", "minha")));

        var botao = cortado.Find("button.principal");
        Assert.True(botao.HasAttribute("disabled"));
        Assert.Equal("true", botao.GetAttribute("aria-pressed"));
        Assert.Equal("item selecionado desabilitado minha", cortado.Find("li").GetAttribute("class"));
        botao.Click();
        Assert.Equal(0, cliques);
    }

    [Fact]
    public void Sublista_abre_e_fecha_pelo_item_e_avisa()
    {
        var estados = new List<bool>();
        var cortado = Lista(Item(i => i
            .Com(nameof(RvmListItem.ChildContent), (RenderFragment)(c => c.AddContent(0, "Fazendas")))
            .Com(nameof(RvmListItem.ExpandedChanged), EventCallback.Factory.Create<bool>(this, estados.Add))
            .Com(nameof(RvmListItem.NestedContent), (RenderFragment)(c => c.AddMarkupContent(0, "<ul><li>Santa Clara</li></ul>")))));

        var botao = cortado.Find("button.principal");
        Assert.Equal("false", botao.GetAttribute("aria-expanded"));
        Assert.Null(botao.GetAttribute("aria-controls"));
        Assert.Empty(cortado.FindAll(".aninhada"));

        botao.Click();
        botao = cortado.Find("button.principal");
        var sublista = cortado.Find(".aninhada");
        Assert.Equal("true", botao.GetAttribute("aria-expanded"));
        Assert.Equal(sublista.Id, botao.GetAttribute("aria-controls"));
        Assert.Contains("Santa Clara", sublista.TextContent);

        cortado.Find("button.principal").Click();
        Assert.Empty(cortado.FindAll(".aninhada"));
        Assert.Equal([true, false], estados);
    }

    [Fact]
    public void Sublista_comeca_aberta_e_desabilitada_nao_alterna()
    {
        var cortado = Lista(Item(i => i
            .Com(nameof(RvmListItem.Expanded), true)
            .Com(nameof(RvmListItem.Disabled), true)
            .Com(nameof(RvmListItem.NestedContent), (RenderFragment)(c => c.AddContent(0, "filhos")))));

        Assert.NotEmpty(cortado.FindAll(".aninhada"));
        cortado.Find("button.principal").Click();
        Assert.NotEmpty(cortado.FindAll(".aninhada"));
    }

    [Fact]
    public void Item_solto_sem_lista_funciona()
    {
        var cortado = Render<RvmListItem>(p => p.AddChildContent("Solto"));

        Assert.Equal("item", cortado.Find("li").GetAttribute("class"));
    }
}

public class RvmAccordionTests : BunitContext
{
    private IRenderedComponent<RvmAccordion> Acordeao(bool exclusivo = false, RvmAccordionVariant variante = RvmAccordionVariant.Standard,
        bool primeiroAberto = false, bool segundoDesabilitado = false, List<(string, bool)>? eventos = null, int nivel = 3)
        => Render<RvmAccordion>(p => p
            .Add(x => x.Exclusive, exclusivo)
            .Add(x => x.Variant, variante)
            .Add(x => x.HeadingLevel, nivel)
            .Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                void Painel(int seq, string titulo, bool aberto = false, bool desabilitado = false)
                {
                    b.OpenComponent<RvmAccordionPanel>(seq);
                    b.AddAttribute(seq + 1, nameof(RvmAccordionPanel.Title), titulo);
                    b.AddAttribute(seq + 2, nameof(RvmAccordionPanel.SecondaryTitle), "Detalhe");
                    b.AddAttribute(seq + 3, nameof(RvmAccordionPanel.Expanded), aberto);
                    b.AddAttribute(seq + 4, nameof(RvmAccordionPanel.Disabled), desabilitado);
                    b.AddAttribute(seq + 5, nameof(RvmAccordionPanel.ExpandedChanged),
                        EventCallback.Factory.Create<bool>(this, v => eventos?.Add((titulo, v))));
                    b.AddAttribute(seq + 6, nameof(RvmAccordionPanel.ChildContent), (RenderFragment)(c => c.AddContent(0, $"Conteudo {titulo}")));
                    b.CloseComponent();
                }

                Painel(0, "Plantio", primeiroAberto);
                Painel(10, "Aplicacoes", desabilitado: segundoDesabilitado);
                Painel(20, "Colheita");
            })));

    [Fact]
    public void Cada_titulo_e_um_botao_dentro_de_cabecalho_e_fechado_nao_renderiza_conteudo()
    {
        var cortado = Acordeao();

        Assert.Equal("acordeao padrao", cortado.Find("div.acordeao").GetAttribute("class"));
        var botoes = cortado.FindAll("h3 > button.gatilho");
        Assert.Equal(3, botoes.Count);
        Assert.All(botoes, b => Assert.Equal("false", b.GetAttribute("aria-expanded")));
        Assert.All(botoes, b => Assert.Null(b.GetAttribute("aria-controls")));
        Assert.Empty(cortado.FindAll("[role=region]"));
        Assert.Equal("Detalhe", cortado.Find(".subtitulo").TextContent);
    }

    [Fact]
    public void Abrir_mostra_a_regiao_ligada_ao_botao()
    {
        var eventos = new List<(string, bool)>();
        var cortado = Acordeao(eventos: eventos);

        cortado.FindAll("button.gatilho")[2].Click();

        var botao = cortado.FindAll("button.gatilho")[2];
        var regiao = cortado.Find("[role=region]");
        Assert.Equal("true", botao.GetAttribute("aria-expanded"));
        Assert.Equal(regiao.Id, botao.GetAttribute("aria-controls"));
        Assert.Equal(botao.Id, regiao.GetAttribute("aria-labelledby"));
        Assert.Equal("Conteudo Colheita", regiao.TextContent.Trim());
        Assert.Contains("aberto", cortado.FindAll(".painel")[2].GetAttribute("class"));
        Assert.Equal([("Colheita", true)], eventos);
    }

    [Fact]
    public void Sem_exclusivo_varios_ficam_abertos()
    {
        var cortado = Acordeao(primeiroAberto: true);

        cortado.FindAll("button.gatilho")[2].Click();

        Assert.Equal(2, cortado.FindAll("[role=region]").Count);
    }

    [Fact]
    public void Exclusivo_fecha_os_outros_e_avisa_cada_um()
    {
        var eventos = new List<(string, bool)>();
        var cortado = Acordeao(exclusivo: true, primeiroAberto: true, eventos: eventos);

        cortado.FindAll("button.gatilho")[2].Click();

        var regioes = cortado.FindAll("[role=region]");
        Assert.Single(regioes);
        Assert.Equal("Conteudo Colheita", regioes[0].TextContent.Trim());
        Assert.Equal([("Colheita", true), ("Plantio", false)], eventos);
    }

    [Fact]
    public void Desabilitado_nao_abre()
    {
        var cortado = Acordeao(segundoDesabilitado: true);

        var botao = cortado.FindAll("button.gatilho")[1];
        Assert.True(botao.HasAttribute("disabled"));
        botao.Click();

        Assert.Empty(cortado.FindAll("[role=region]"));
        Assert.Contains("desabilitado", cortado.FindAll(".painel")[1].GetAttribute("class"));
    }

    [Fact]
    public void Preenchido_usa_mais_e_menos_e_o_nivel_do_cabecalho_e_configuravel()
    {
        var cortado = Acordeao(variante: RvmAccordionVariant.Filled, primeiroAberto: true, nivel: 9);

        Assert.Equal("acordeao preenchido", cortado.Find("div.acordeao").GetAttribute("class"));
        Assert.Equal(3, cortado.FindAll("h6 > button").Count);
        // O icone aberto (menos) difere do fechado (mais): comparar o desenho dos dois.
        var aberto = cortado.FindAll(".marca")[0].InnerHtml;
        var fechado = cortado.FindAll(".marca")[1].InnerHtml;
        Assert.NotEqual(aberto, fechado);
    }

    [Fact]
    public void Painel_segue_o_parametro_so_quando_ele_muda()
    {
        var aberto = false;
        RenderFragment Conteudo() => b =>
        {
            b.OpenComponent<RvmAccordionPanel>(0);
            b.AddAttribute(1, nameof(RvmAccordionPanel.Title), "Unico");
            b.AddAttribute(2, nameof(RvmAccordionPanel.Expanded), aberto);
            b.AddAttribute(3, nameof(RvmAccordionPanel.Icon), RvmIconName.Calendar);
            b.CloseComponent();
        };
        var cortado = Render<RvmAccordion>(p => p.Add(x => x.ChildContent, Conteudo()).AddUnmatched("class", "minha"));
        Assert.Equal("acordeao padrao minha", cortado.Find("div.acordeao").GetAttribute("class"));
        Assert.NotNull(cortado.Find(".icone svg"));

        // Aberto pelo clique, sem @bind: um re-render do pai com o mesmo Expanded=false nao fecha.
        cortado.Find("button.gatilho").Click();
        cortado.Render(p => p.Add(x => x.ChildContent, Conteudo()));
        Assert.NotEmpty(cortado.FindAll("[role=region]"));

        // Mudou de verdade para true e depois para false: segue.
        aberto = true;
        cortado.Render(p => p.Add(x => x.ChildContent, Conteudo()));
        aberto = false;
        cortado.Render(p => p.Add(x => x.ChildContent, Conteudo()));
        Assert.Empty(cortado.FindAll("[role=region]"));
    }

    [Fact]
    public void Cabecalho_recebe_o_escopo_do_css_isolado()
    {
        // h{n} criado pelo RenderTreeBuilder nao recebe o atributo b-xxxx, e o CSS do painel nao pegava.
        var cortado = Acordeao(nivel: 4);

        var cabecalho = cortado.Find("h4.cabecalho");
        Assert.Contains(cabecalho.Attributes, a => a.Name.StartsWith("b-", StringComparison.Ordinal));
    }

    [Fact]
    public void Painel_solto_sem_acordeao_funciona()
    {
        var cortado = Render<RvmAccordionPanel>(p => p.Add(x => x.Title, "Solto").AddUnmatched("class", "minha"));

        Assert.NotNull(cortado.Find("h3 > button"));
        Assert.Equal("painel minha", cortado.Find("div.painel").GetAttribute("class"));
        cortado.Find("button").Click();
        Assert.NotEmpty(cortado.FindAll("[role=region]"));
    }
}
