using Bunit;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.AppShell;
using RVM.DesignSystem.Components.Avatar;
using RVM.DesignSystem.Components.Calendar;
using RVM.DesignSystem.Components.Card;
using RVM.DesignSystem.Components.Lists;
using RVM.DesignSystem.Components.Menu;
using RVM.DesignSystem.Components.Tabs;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>Cartao, item de lista, menu, grupo de avatares, calendario, abas e casca do contrato com o RVM.UI (DSGN-017).</summary>
public class RvmEstruturaDoContratoTests : BunitContext
{
    public RvmEstruturaDoContratoTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private static RenderFragment Html(string html) => b => b.AddMarkupContent(0, html);

    [Fact]
    public void Cartao_com_cabecalho_livre_rodape_respiro_e_variante()
    {
        var cortado = Render<RvmCard>(p => p
            .Add(x => x.Title, "Ignorado")
            .Add(x => x.Header, Html("<h2 class=\"meu\">Resumo</h2>"))
            .Add(x => x.Footer, Html("<strong>Total</strong>"))
            .Add(x => x.Padding, RvmCardPadding.None)
            .Add(x => x.Variant, RvmCardVariant.Outlined)
            .AddChildContent("corpo"));

        Assert.Equal("Resumo", cortado.Find(".rvm-cabecalho .meu").TextContent);
        Assert.DoesNotContain("Ignorado", cortado.Markup);
        Assert.Equal("Total", cortado.Find(".rvm-rodape strong").TextContent);
        var raiz = cortado.Find(".rvm-cartao");
        Assert.Contains("rvm-respiro-nenhum", raiz.ClassList);
        Assert.Contains("rvm-contornado", raiz.ClassList);
    }

    [Fact]
    public void Item_de_lista_com_titulo_valor_e_variacao()
    {
        var cortado = Render<RvmList>(p => p.AddChildContent<RvmListItem>(i => i
            .Add(x => x.Title, "Receita")
            .Add(x => x.Value, "R$ 1.200,00")
            .Add(x => x.ValueSubtitle, "no mes")
            .Add(x => x.Trend, RvmTrend.Down)
            .Add(x => x.TrendLabel, "10%")
            .Add(x => x.Layout, RvmListItemLayout.OneColumn)));

        Assert.Equal("Receita", cortado.Find(".rvm-primario").TextContent);
        Assert.Equal("R$ 1.200,00", cortado.Find(".rvm-valor").TextContent);
        var variacao = cortado.Find(".rvm-variacao");
        Assert.Contains("rvm-caiu", variacao.ClassList);
        Assert.Contains("caiu", variacao.TextContent);
        Assert.Contains("rvm-uma-coluna", cortado.Find("li").ClassList);
    }

    [Fact]
    public void Item_de_lista_com_subtitulo_em_conteudo()
        => Assert.NotNull(Render<RvmList>(p => p.AddChildContent<RvmListItem>(i => i.Add(x => x.Title, "Pedido").Add(x => x.SecondaryText, "x").Add(x => x.SubtitleContent, Html("<span class=\"selo\">Pago</span>")))).Find(".rvm-secundario .selo"));

    [Fact]
    public void Menu_so_de_icone_com_id_e_aberto_acima()
    {
        var cortado = Render<RvmMenu>(p => p
            .Add(x => x.Label, "Mais acoes")
            .Add(x => x.Trigger, RvmMenuTrigger.Icon)
            .Add(x => x.Id, "acoes")
            .Add(x => x.Placement, RvmMenuPlacement.TopEnd)
            .AddChildContent<RvmMenuItem>(i => i.Add(x => x.Text, "Editar")));

        var gatilho = cortado.Find("button");
        Assert.Equal("acoes", gatilho.GetAttribute("id"));
        Assert.Equal("Mais acoes", gatilho.GetAttribute("aria-label"));
        Assert.Equal("", gatilho.TextContent.Trim());

        gatilho.Click();
        var lista = cortado.Find("[role=menu]");
        Assert.Contains("rvm-acima", lista.ClassList);
        Assert.Contains("rvm-fim", lista.ClassList);
    }

    [Fact]
    public void Menu_com_texto_proprio_no_gatilho()
        => Assert.Equal("Exportar", Render<RvmMenu>(p => p.Add(x => x.Label, "Acoes").Add(x => x.Text, "Exportar")).Find("button").TextContent.Trim());

    [Fact]
    public void Grupo_de_avatares_por_lista_mostra_ate_o_maximo_e_o_resto()
    {
        var cortado = Render<RvmAvatarGroup>(p => p
            .Add(x => x.Label, "Equipe")
            .Add(x => x.Items, [new RvmAvatarItem("Ana Lima"), new RvmAvatarItem("Bia Souza", "/b.png"), new RvmAvatarItem("Caio Reis")])
            .Add(x => x.Max, 2));

        Assert.Equal("AL", cortado.Find(".rvm-iniciais").TextContent);
        Assert.Equal("Bia Souza", cortado.Find("img").GetAttribute("alt"));
        Assert.Contains("+1", cortado.Markup);
    }

    [Fact]
    public void Calendario_preso_avisa_o_Esc()
    {
        var esc = false;
        var cortado = Render<RvmCalendar>(p => p.Add(x => x.TrapFocus, true).Add(x => x.OnEscape, EventCallback.Factory.Create(this, () => esc = true)));

        cortado.Find(".rvm-grade").KeyDown("Escape");

        Assert.True(esc);
    }

    [Fact]
    public void Calendario_solto_ignora_o_Esc()
    {
        var esc = false;
        var cortado = Render<RvmCalendar>(p => p.Add(x => x.OnEscape, EventCallback.Factory.Create(this, () => esc = true)));

        cortado.Find(".rvm-grade").KeyDown("Escape");

        Assert.False(esc);
    }

    [Fact]
    public void Abas_por_valor_com_texto_contador_ids_e_cor()
    {
        string? escolhida = null;
        var cortado = Render<RvmTabs>(p => p
            .Add(x => x.Label, "Pedidos")
            .Add(x => x.Value, "pagos")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, v => escolhida = v))
            .Add(x => x.Color, RvmColor.Success)
            .Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<RvmTab>(0);
                b.AddComponentParameter(1, nameof(RvmTab.Text), "Abertos");
                b.AddComponentParameter(2, nameof(RvmTab.Value), "abertos");
                b.AddComponentParameter(3, nameof(RvmTab.Count), 3);
                b.AddComponentParameter(4, nameof(RvmTab.Id), "aba-abertos");
                b.CloseComponent();
                b.OpenComponent<RvmTab>(5);
                b.AddComponentParameter(6, nameof(RvmTab.Text), "Pagos");
                b.AddComponentParameter(7, nameof(RvmTab.Value), "pagos");
                b.AddComponentParameter(8, nameof(RvmTab.PanelId), "painel-pagos");
                b.CloseComponent();
            })));

        Assert.Equal("Pedidos", cortado.Find("[role=tablist]").GetAttribute("aria-label"));
        Assert.Contains("rvm-success", cortado.Find(".rvm-abas").ClassList);
        var abas = cortado.FindAll("[role=tab]");
        Assert.Equal("aba-abertos", abas[0].GetAttribute("id"));
        Assert.Equal("3", abas[0].QuerySelector(".rvm-contador")!.TextContent);
        Assert.Equal("true", abas[1].GetAttribute("aria-selected"));
        Assert.Equal("painel-pagos", abas[1].GetAttribute("aria-controls"));
        Assert.Empty(cortado.FindAll("[role=tabpanel]"));

        abas[0].Click();
        Assert.Equal("abertos", escolhida);
    }

    [Fact]
    public void Casca_com_lateral_pronta_id_e_cortina_que_fecha()
    {
        var cortado = Render<RvmAppShell>(p => p
            .Add(x => x.Sidebar, Html("<div class=\"minha-lateral\">Menu</div>"))
            .Add(x => x.Id, "lateral")
            .Add(x => x.CloseMenuLabel, "Fechar navegacao"));

        Assert.Equal("lateral", cortado.Find("aside").GetAttribute("id"));
        Assert.NotNull(cortado.Find("aside .minha-lateral"));
        Assert.Empty(cortado.FindAll("aside nav"));

        cortado.Find("button.rvm-so-estreita").Click();
        var cortina = cortado.Find("button.rvm-fundo-menu");
        Assert.Equal("Fechar navegacao", cortina.GetAttribute("aria-label"));
        cortina.Click();
        Assert.Empty(cortado.FindAll(".rvm-fundo-menu"));
    }
}

public class RvmReviewOnda0ListaTests : BunitContext
{
    [Fact]
    public void Title_com_conteudo_continua_dica_do_item()
    {
        var item = Render<RvmList>(p => p.AddChildContent<RvmListItem>(i => i.Add(x => x.Title, "Dica").AddChildContent("Pedido 12"))).Find("li");

        Assert.Equal("Dica", item.GetAttribute("title"));
        Assert.Contains("Pedido 12", item.TextContent);
    }
}

public class RvmReviewOnda0MenuTests : BunitContext
{
    [Fact]
    public void Aria_label_do_consumidor_chega_ao_gatilho_com_texto()
        => Assert.Equal("Mover Agro para outra etapa", Render<RvmMenu>(p => p.Add(x => x.Label, "Mover").AddUnmatched("aria-label", "Mover Agro para outra etapa")).Find("button").GetAttribute("aria-label"));
}
