using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using RVM.DesignSystem.Components.AppShell;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

public class RvmAppShellTests : BunitContext
{
    public RvmAppShellTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private NavigationManager Navegacao => Services.GetRequiredService<NavigationManager>();

    private IRenderedComponent<RvmAppShell> Moldura(Action<ComponentParameterCollectionBuilder<RvmAppShell>>? extra = null)
        => Render<RvmAppShell>(p =>
        {
            p.Add(x => x.Brand, (RenderFragment)(b => b.AddContent(0, "RVM Agro")))
             .Add(x => x.Navigation, (RenderFragment)(b =>
             {
                 b.OpenComponent<RvmNavItem>(0);
                 b.AddComponentParameter(1, nameof(RvmNavItem.Href), "");
                 b.AddComponentParameter(2, nameof(RvmNavItem.Text), "Inicio");
                 b.AddComponentParameter(3, nameof(RvmNavItem.Icon), (RvmIconName?)RvmIconName.Home);
                 b.AddComponentParameter(4, nameof(RvmNavItem.Match), RvmNavMatch.All);
                 b.CloseComponent();
                 b.OpenComponent<RvmNavSection>(5);
                 b.AddComponentParameter(6, nameof(RvmNavSection.Title), "Operacao");
                 b.AddComponentParameter(7, nameof(RvmNavSection.ChildContent), (RenderFragment)(c =>
                 {
                     c.OpenComponent<RvmNavItem>(0);
                     c.AddComponentParameter(1, nameof(RvmNavItem.Href), "talhoes");
                     c.AddComponentParameter(2, nameof(RvmNavItem.Text), "Talhoes");
                     c.AddComponentParameter(3, nameof(RvmNavItem.Icon), (RvmIconName?)RvmIconName.File);
                     c.AddComponentParameter(4, nameof(RvmNavItem.Badge), "3");
                     c.CloseComponent();
                 }));
                 b.CloseComponent();
                 b.OpenComponent<RvmNavGroup>(8);
                 b.AddComponentParameter(9, nameof(RvmNavGroup.Text), "Lavouras");
                 b.AddComponentParameter(10, nameof(RvmNavGroup.Icon), RvmIconName.Calendar);
                 b.AddComponentParameter(11, nameof(RvmNavGroup.ChildContent), (RenderFragment)(c =>
                 {
                     c.OpenComponent<RvmNavItem>(0);
                     c.AddComponentParameter(1, nameof(RvmNavItem.Href), "lavouras/soja");
                     c.AddComponentParameter(2, nameof(RvmNavItem.Text), "Soja");
                     c.CloseComponent();
                 }));
                 b.CloseComponent();
             }))
             .Add(x => x.TopBar, (RenderFragment)(b => b.AddContent(0, "Busca")))
             .Add(x => x.Footer, (RenderFragment)(b => b.AddContent(0, "© 2026")))
             .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Pagina")));
            extra?.Invoke(p);
        });

    [Fact]
    public void Tem_marcos_navegacao_nomeada_e_atalho_para_o_conteudo()
    {
        var cortado = Moldura();

        var principal = cortado.Find("main");
        Assert.Equal("Pagina", principal.TextContent.Trim());
        Assert.Equal($"#{principal.Id}", cortado.Find("a.rvm-pular").GetAttribute("href"));
        Assert.Equal("Menu principal", cortado.Find("nav").GetAttribute("aria-label"));
        Assert.Equal("RVM Agro", cortado.Find(".rvm-area-marca").TextContent);
        Assert.Contains("Busca", cortado.Find("header").TextContent);
        Assert.Equal("© 2026", cortado.Find("footer").TextContent);

        var aside = cortado.Find("aside");
        Assert.All(cortado.FindAll("button.rvm-alternar"), b => Assert.Equal(aside.Id, b.GetAttribute("aria-controls")));
    }

    [Fact]
    public void Item_da_pagina_atual_tem_aria_current_e_destaque()
    {
        Navegacao.NavigateTo("talhoes/12");
        var cortado = Moldura();

        var talhoes = cortado.Find("a[href=talhoes]");
        Assert.Equal("page", talhoes.GetAttribute("aria-current"));
        Assert.Contains("rvm-ativo", talhoes.ClassName);
        Assert.False(cortado.Find("a[href='']").HasAttribute("aria-current"));
        Assert.Equal("3", talhoes.QuerySelector(".rvm-nav-selo")!.TextContent);

        Navegacao.NavigateTo("/");
        Assert.Equal("page", cortado.Find("a[href='']").GetAttribute("aria-current"));
        Assert.False(cortado.Find("a[href=talhoes]").HasAttribute("aria-current"));
    }

    [Theory]
    [InlineData("talhoes", RvmNavMatch.Prefix, "talhoes", true)]
    [InlineData("talhoes", RvmNavMatch.Prefix, "talhoes/12?aba=mapa", true)]
    [InlineData("talhoes", RvmNavMatch.Prefix, "talhoes-velhos", false)]
    [InlineData("talhoes/", RvmNavMatch.Prefix, "talhoes/12", true)]
    [InlineData("talhoes", RvmNavMatch.All, "talhoes/12", false)]
    [InlineData("talhoes", RvmNavMatch.All, "TALHOES/#topo", true)]
    public void Criterio_de_pagina_atual_segue_o_NavLink(string href, RvmNavMatch criterio, string atual, bool ativo)
    {
        Navegacao.NavigateTo(atual);
        var cortado = Render<RvmNavItem>(p => p.Add(x => x.Href, href).Add(x => x.Text, "Item").Add(x => x.Match, criterio));

        Assert.Equal(ativo, cortado.Find("a").HasAttribute("aria-current"));
    }

    [Fact]
    public void Item_sem_icone_leva_o_circulo_de_subitem()
    {
        var cortado = Render<RvmNavItem>(p => p.Add(x => x.Href, "soja").Add(x => x.Text, "Soja").AddUnmatched("data-teste", "x"));

        Assert.NotNull(cortado.Find("a[data-teste=x] .rvm-nav-ponto"));
        Assert.Contains("rvm-subitem", cortado.Find("a").ClassName);
    }

    [Fact]
    public void Recolher_avisa_e_deixa_o_texto_como_nome_dos_links()
    {
        var recolhida = false;
        var cortado = Moldura(p => p.Add(x => x.CollapsedChanged, EventCallback.Factory.Create<bool>(this, v => recolhida = v)));

        var botao = cortado.Find("button[aria-label='Recolher menu']");
        Assert.Equal("false", botao.GetAttribute("aria-pressed"));
        botao.Click();

        Assert.True(recolhida);
        Assert.Contains("rvm-recolhida", cortado.Find(".rvm-casca").ClassName);
        Assert.Equal("true", cortado.Find("button[aria-label='Recolher menu']").GetAttribute("aria-pressed"));
        var inicio = cortado.Find("a[href='']");
        Assert.Contains("rvm-recolhido", inicio.ClassName);
        Assert.Equal("Inicio", inicio.GetAttribute("title"));
        Assert.Equal("Inicio", inicio.QuerySelector(".rvm-nav-texto")!.TextContent);
        Assert.Contains("rvm-recolhida", cortado.Find("li.rvm-nav-secao").ClassName);
    }

    [Fact]
    public void Grupo_abre_e_fecha_os_subitens()
    {
        var cortado = Moldura();

        var grupo = cortado.Find("button.rvm-nav-grupo");
        Assert.Equal("false", grupo.GetAttribute("aria-expanded"));
        Assert.False(grupo.HasAttribute("aria-controls"));
        Assert.Empty(cortado.FindAll("a[href='lavouras/soja']"));

        grupo.Click();
        grupo = cortado.Find("button.rvm-nav-grupo");
        Assert.Equal("true", grupo.GetAttribute("aria-expanded"));
        Assert.Equal(grupo.GetAttribute("aria-controls"), cortado.Find("ul.rvm-nav-subitens").Id);
        Assert.Null(cortado.Find("a[href='lavouras/soja']").GetAttribute("title"));

        cortado.Find("button.rvm-nav-grupo").Click();
        Assert.Empty(cortado.FindAll("a[href='lavouras/soja']"));
    }

    [Fact]
    public void Grupo_com_menu_recolhido_expande_o_menu_e_abre()
    {
        var cortado = Moldura(p => p.Add(x => x.Collapsed, true));

        cortado.Find("button.rvm-nav-grupo").Click();

        Assert.DoesNotContain("rvm-recolhida", cortado.Find(".rvm-casca").ClassName);
        Assert.Equal("true", cortado.Find("button.rvm-nav-grupo").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void Gaveta_abre_pelo_botao_e_fecha_com_Esc_fundo_e_navegacao()
    {
        var estados = new List<bool>();
        var cortado = Moldura(p => p.Add(x => x.MenuOpenChanged, EventCallback.Factory.Create<bool>(this, estados.Add)));

        var abrir = cortado.Find("button[aria-label='Abrir menu']");
        Assert.Equal("false", abrir.GetAttribute("aria-expanded"));
        Assert.Empty(cortado.FindAll(".rvm-fundo-menu"));

        abrir.Click();
        Assert.Contains("rvm-menu-aberto", cortado.Find(".rvm-casca").ClassName);
        Assert.Equal("true", cortado.Find("button[aria-label='Abrir menu']").GetAttribute("aria-expanded"));
        cortado.Find("aside").KeyDown(key: "a");
        Assert.Contains("rvm-menu-aberto", cortado.Find(".rvm-casca").ClassName);
        cortado.Find("aside").KeyDown(key: "Escape");
        Assert.DoesNotContain("rvm-menu-aberto", cortado.Find(".rvm-casca").ClassName);

        cortado.Find("button[aria-label='Abrir menu']").Click();
        cortado.Find(".rvm-fundo-menu").Click();
        Assert.Empty(cortado.FindAll(".rvm-fundo-menu"));

        // Esc com o foco ainda no botao (antes do JS levar o foco a gaveta) tambem fecha.
        cortado.Find("button[aria-label='Abrir menu']").Click();
        cortado.Find("button[aria-label='Abrir menu']").KeyDown(key: "Escape");
        Assert.Empty(cortado.FindAll(".rvm-fundo-menu"));

        cortado.Find("button[aria-label='Abrir menu']").Click();
        Navegacao.NavigateTo("talhoes");
        cortado.WaitForAssertion(() => Assert.Empty(cortado.FindAll(".rvm-fundo-menu")));

        Assert.Equal([true, false, true, false, true, false, true, false], estados);
    }

    [Fact]
    public void Navegar_com_a_gaveta_fechada_nao_avisa_nada()
    {
        var avisos = 0;
        Moldura(p => p.Add(x => x.MenuOpenChanged, EventCallback.Factory.Create<bool>(this, _ => avisos++)));

        Navegacao.NavigateTo("talhoes");

        Assert.Equal(0, avisos);
    }

    [Fact]
    public void Atributos_extras_e_classe_vao_para_a_raiz()
    {
        var cortado = Moldura(p => p.AddUnmatched("class", "minha").AddUnmatched("style", "height: 520px").AddUnmatched("data-teste", "x"));

        var raiz = cortado.Find("[data-teste=x]");
        Assert.Equal("rvm-casca minha", raiz.ClassName);
        Assert.Equal("height: 520px", raiz.GetAttribute("style"));
    }

    [Fact]
    public async Task Descartar_libera_a_assinatura_de_navegacao()
    {
        var avisos = 0;
        var cortado = Moldura(p => p
            .Add(x => x.MenuOpen, true)
            .Add(x => x.MenuOpenChanged, EventCallback.Factory.Create<bool>(this, _ => avisos++)));

        await cortado.Instance.DisposeAsync();
        Navegacao.NavigateTo("talhoes");

        Assert.Equal(0, avisos);
    }
}
