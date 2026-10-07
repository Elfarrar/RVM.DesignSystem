using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using RVM.DesignSystem.Components.Accordion;
using RVM.DesignSystem.Components.AppShell;
using RVM.DesignSystem.Components.BackButton;
using RVM.DesignSystem.Components.Button;
using RVM.DesignSystem.Components.Collapse;
using RVM.DesignSystem.Components.IconButton;
using RVM.DesignSystem.Components.Link;
using RVM.DesignSystem.Components.Menu;
using RVM.DesignSystem.Components.Page;
using RVM.DesignSystem.Components.Sidebar;
using RVM.DesignSystem.Components.Stepper;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>Onda 1 do contrato com o RVM.UI (DSGN-017): menu lateral, pagina e navegacao.</summary>
public class RvmOnda1Tests : BunitContext
{
    public RvmOnda1Tests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private static RenderFragment Html(string html) => b => b.AddMarkupContent(0, html);

    [Fact]
    public void Menu_lateral_recolhe_pelo_botao_e_os_itens_seguem()
    {
        bool? avisado = null;
        var cortado = Render<RvmSidebar>(p => p
            .Add(x => x.Label, "Menu")
            .Add(x => x.Collapsible, true)
            .Add(x => x.CollapsedChanged, EventCallback.Factory.Create<bool>(this, v => avisado = v))
            .Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<RvmNavItem>(0);
                b.AddComponentParameter(1, nameof(RvmNavItem.Href), "painel");
                b.AddComponentParameter(2, nameof(RvmNavItem.Text), "Painel");
                b.AddComponentParameter(3, nameof(RvmNavItem.Icon), RvmIconName.Home);
                b.CloseComponent();
            })));

        Assert.Equal("Menu", cortado.Find("nav").GetAttribute("aria-label"));
        cortado.Find("button.rvm-recolher").Click();

        Assert.True(avisado);
        Assert.Contains("rvm-recolhida", cortado.Find(".rvm-barra-lateral").ClassList);
        Assert.Equal("Painel", cortado.Find("a.rvm-nav-link").GetAttribute("title"));
    }

    [Fact]
    public void Menu_lateral_colorido_e_com_linha_no_ativo()
    {
        var raiz = Render<RvmSidebar>(p => p.Add(x => x.Variant, RvmSidebarVariant.Colored).Add(x => x.ActiveLine, true)).Find(".rvm-barra-lateral");

        Assert.Contains("rvm-colorida", raiz.ClassList);
        Assert.Contains("rvm-linha-ativa", raiz.ClassList);
    }

    [Fact]
    public void Subitem_vira_item_sem_icone_com_contador()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("receber");
        var cortado = Render<RvmSidebar>(p => p.Add(x => x.ChildContent, (RenderFragment)(b =>
        {
            b.OpenComponent<RvmNavSubItem>(0);
            b.AddComponentParameter(1, nameof(RvmNavSubItem.Href), "receber");
            b.AddComponentParameter(2, nameof(RvmNavSubItem.Text), "A receber");
            b.AddComponentParameter(3, nameof(RvmNavSubItem.Badge), 3);
            b.AddComponentParameter(4, nameof(RvmNavSubItem.Match), NavLinkMatch.All);
            b.CloseComponent();
        })));

        var link = cortado.Find("a.rvm-nav-link");
        Assert.Equal("page", link.GetAttribute("aria-current"));
        Assert.Equal("3", link.QuerySelector(".rvm-nav-selo")!.TextContent);
        Assert.NotNull(link.QuerySelector(".rvm-nav-ponto"));
    }

    [Fact]
    public void Barra_do_topo_e_da_pagina_com_os_lados()
    {
        var topo = Render<RvmTopbar>(p => p.Add(x => x.Start, Html("<b>Titulo</b>")).Add(x => x.Actions, Html("<i>acao</i>")).Add(x => x.Profile, Html("<u>eu</u>")));
        var barra = Render<RvmPageToolbar>(p => p.Add(x => x.Start, Html("<b>busca</b>")).Add(x => x.End, Html("<i>filtro</i>")));

        Assert.Equal("Titulo", topo.Find(".rvm-inicio b").TextContent);
        Assert.Equal("eu", topo.Find(".rvm-fim .rvm-perfil u").TextContent);
        Assert.Equal("filtro", barra.Find(".rvm-fim i").TextContent);
    }

    [Fact]
    public void Cabecalho_da_pagina_com_h1_voltar_e_foto_decorativa()
    {
        var voltou = false;
        var cortado = Render<RvmPageHeader>(p => p
            .Add(x => x.Title, "Talhao 12")
            .Add(x => x.Description, "Soja")
            .Add(x => x.ImageUrl, "/f.png")
            .Add(x => x.ShowBack, true)
            .Add(x => x.OnBack, EventCallback.Factory.Create(this, () => voltou = true)));

        Assert.Equal("Talhao 12", cortado.Find("h1").TextContent);
        Assert.Equal("", cortado.Find("img").GetAttribute("alt"));
        cortado.Find("button.rvm-voltar").Click();
        Assert.True(voltou);
    }

    [Fact]
    public void Voltar_e_link_com_href_e_some_do_teclado_sem_ele_quando_desabilitado()
    {
        var link = Render<RvmBackButton>(p => p.Add(x => x.Href, "talhoes").Add(x => x.Label, "Voltar aos talhoes")).Find("a");
        var desabilitado = Render<RvmBackButton>(p => p.Add(x => x.Href, "talhoes").Add(x => x.Disabled, true)).Find(".rvm-voltar");

        Assert.Equal("Voltar aos talhoes", link.GetAttribute("aria-label"));
        Assert.Equal("SPAN", desabilitado.TagName);
        Assert.Equal("true", desabilitado.GetAttribute("aria-disabled"));
    }

    [Fact]
    public void Link_desabilitado_nao_navega()
    {
        var ativo = Render<RvmLink>(p => p.Add(x => x.Href, "x").Add(x => x.Color, RvmColor.Error).AddChildContent("Excluir")).Find("a");
        var desabilitado = Render<RvmLink>(p => p.Add(x => x.Href, "x").Add(x => x.Disabled, true).AddChildContent("Excluir")).Find(".rvm-link");

        Assert.Contains("rvm-error", ativo.ClassList);
        Assert.Equal("SPAN", desabilitado.TagName);
        Assert.False(desabilitado.HasAttribute("href"));
    }

    [Fact]
    public void Botao_de_icone_tem_nome_tipo_e_clique()
    {
        MouseEventArgs? clique = null;
        var botao = Render<RvmIconButton>(p => p
            .Add(x => x.Icon, RvmIconName.Trash)
            .Add(x => x.Label, "Excluir")
            .Add(x => x.Type, RvmButtonType.Submit)
            .Add(x => x.Variant, RvmButtonVariant.Soft)
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, e => clique = e))).Find("button");

        Assert.Equal("Excluir", botao.GetAttribute("aria-label"));
        Assert.Equal("submit", botao.GetAttribute("type"));
        Assert.Contains("rvm-suave", botao.ClassList);
        botao.Click();
        Assert.NotNull(clique);
    }

    [Fact]
    public void Botao_de_menu_abre_os_itens_e_aceita_nome_proprio()
    {
        var cortado = Render<RvmMenuButton>(p => p
            .Add(x => x.Text, "Exportar")
            .Add(x => x.AriaLabel, "Exportar pedidos")
            .Add(x => x.Id, "exportar")
            .AddChildContent<RvmMenuItem>(i => i.Add(x => x.Text, "PDF")));

        var gatilho = cortado.Find("button");
        Assert.Equal("exportar", gatilho.GetAttribute("id"));
        Assert.Equal("Exportar pedidos", gatilho.GetAttribute("aria-label"));
        Assert.Contains("Exportar", gatilho.TextContent);
        gatilho.Click();
        Assert.Equal("PDF", cortado.Find("[role=menuitem]").TextContent.Trim());
    }

    [Fact]
    public void Grupo_de_botoes_e_escolha_unica_com_aria_pressed()
    {
        string? escolhido = "Ler";
        var cortado = Render<RvmButtonGroup<string>>(p => p
            .Add(x => x.Label, "Permissao")
            .Add(x => x.Items, ["Nenhum", "Ler", "Editar"])
            .Add(x => x.Value, escolhido)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, v => escolhido = v))
            .Add(x => x.SelectedColor, v => v == "Editar" ? RvmColor.Success : RvmColor.Primary)
            .Add(x => x.ItemDisabled, v => v == "Nenhum"));

        Assert.Equal("group", cortado.Find("div").GetAttribute("role"));
        var botoes = cortado.FindAll("button");
        Assert.Equal("true", botoes[1].GetAttribute("aria-pressed"));
        Assert.True(botoes[0].HasAttribute("disabled"));

        botoes[2].Click();
        Assert.Equal("Editar", escolhido);
        Assert.Contains("rvm-success", cortado.FindAll("button")[2].ClassList);
    }

    [Fact]
    public void Collapse_esconde_sem_desmontar()
    {
        var cortado = Render<RvmCollapse>(p => p.Add(x => x.Id, "bloco").Add(x => x.Expanded, false).AddChildContent("<input id=\"nome\" />"));

        Assert.True(cortado.Find("#bloco").HasAttribute("hidden"));
        Assert.NotNull(cortado.Find("#nome"));
    }

    [Fact]
    public void Painel_expansivel_mantem_o_corpo_e_aponta_a_regiao()
    {
        bool? aberto = null;
        var cortado = Render<RvmExpansionPanel>(p => p
            .Add(x => x.Title, "Plantio")
            .Add(x => x.HeadingLevel, 4)
            .Add(x => x.ExpandedChanged, EventCallback.Factory.Create<bool>(this, v => aberto = v))
            .AddChildContent("<p class=\"corpo\">Soja</p>"));

        var botao = cortado.Find("h4 button");
        var regiao = cortado.Find("[role=region]");
        Assert.Equal(regiao.GetAttribute("id"), botao.GetAttribute("aria-controls"));
        Assert.True(regiao.HasAttribute("hidden"));
        Assert.NotNull(cortado.Find(".corpo"));

        botao.Click();
        Assert.True(aberto);
        Assert.False(cortado.Find("[role=region]").HasAttribute("hidden"));
    }

    [Fact]
    public void Etapas_marcam_a_atual_e_navegam()
    {
        var etapa = 1;
        var cortado = Render<RvmSteps>(p => p
            .Add(x => x.Steps, ["Arquivo", "Conferencia", "Confirmacao"])
            .Add(x => x.Current, etapa)
            .Add(x => x.CurrentChanged, EventCallback.Factory.Create<int>(this, v => etapa = v))
            .Add(x => x.ShowNavigation, true)
            .Add(x => x.AllowBackNavigation, true)
            .Add(x => x.NumberedTitles, true));

        var itens = cortado.FindAll("li");
        Assert.Equal("step", itens[1].GetAttribute("aria-current"));
        Assert.Equal("2. Conferencia", itens[1].QuerySelector(".rvm-titulo-etapa")!.TextContent);

        cortado.Find("button.rvm-voltar-etapa").Click();
        Assert.Equal(0, etapa);
    }

    [Fact]
    public void Etapas_sem_liberar_nao_avancam()
    {
        var cortado = Render<RvmSteps>(p => p.Add(x => x.Steps, ["A", "B"]).Add(x => x.ShowNavigation, true).Add(x => x.CanAdvance, false));

        var botoes = cortado.FindAll(".rvm-navegacao-etapas button");
        Assert.True(botoes[0].HasAttribute("disabled"));
        Assert.True(botoes[1].HasAttribute("disabled"));
    }

    [Fact]
    public void Indicador_de_passo_atual()
    {
        var atual = Render<RvmStepIndicator>(p => p.Add(x => x.Number, 2).Add(x => x.Active, true)).Find("span");
        var texto = Render<RvmStepIndicator>(p => p.Add(x => x.Text, "...")).Find("span");

        Assert.Equal("2", atual.TextContent);
        Assert.Equal("step", atual.GetAttribute("aria-current"));
        Assert.Equal("...", texto.TextContent);
    }

    [Fact]
    public void Ficha_de_detalhe_com_perfil_nomeado_e_atividade()
    {
        var cortado = Render<RvmDetailProfileLayout>(p => p
            .Add(x => x.Name, "Agro Sul Graos")
            .Add(x => x.Details, Html("<dt>Cidade</dt><dd>Rio Verde</dd>"))
            .Add(x => x.Activity, Html("<li>Pedido faturado</li>"))
            .AddChildContent("<p class=\"miolo\">abas</p>"));

        var perfil = cortado.Find("aside");
        Assert.Equal(cortado.Find("h2").GetAttribute("id"), perfil.GetAttribute("aria-labelledby"));
        Assert.Equal("AS", cortado.Find(".rvm-iniciais").TextContent);
        Assert.Equal("Rio Verde", cortado.Find("dl dd").TextContent);
        Assert.Equal("Atividade recente", cortado.Find("h3").TextContent);
        Assert.NotNull(cortado.Find(".miolo"));
    }
}
