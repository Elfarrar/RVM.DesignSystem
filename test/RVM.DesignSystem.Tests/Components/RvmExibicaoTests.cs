using Bunit;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem;
using RVM.DesignSystem.Components.Badge;
using RVM.DesignSystem.Components.Breadcrumbs;
using RVM.DesignSystem.Components.Pagination;
using RVM.DesignSystem.Components.Tooltip;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

public class RvmBadgeTests : BunitContext
{
    [Fact]
    public void Sem_conteudo_sai_uma_pilula_primary_solta()
    {
        var cortado = Render<RvmBadge>(p => p.Add(x => x.Content, "Novo"));

        Assert.Equal("rvm-badge-raiz", cortado.Find("span").GetAttribute("class"));
        var marca = cortado.Find(".rvm-badge");
        Assert.Equal("rvm-badge rvm-pilula rvm-primary rvm-solto", marca.GetAttribute("class"));
        Assert.Equal("Novo", marca.TextContent);
    }

    [Theory]
    [InlineData(5, 99, "5")]
    [InlineData(99, 99, "99")]
    [InlineData(100, 99, "99+")]
    [InlineData(1000, 999, "999+")]
    public void Contador_respeita_o_maximo(int contagem, int maximo, string esperado)
    {
        var cortado = Render<RvmBadge>(p => p.Add(x => x.Count, contagem).Add(x => x.Max, maximo).Add(x => x.Content, "ignorado"));

        Assert.Equal(esperado, cortado.Find(".rvm-badge").TextContent);
    }

    [Fact]
    public void Ponto_nao_tem_texto_e_e_escondido_do_leitor()
    {
        var cortado = Render<RvmBadge>(p => p.Add(x => x.Dot, true).Add(x => x.Content, "3"));

        var marca = cortado.Find(".rvm-badge");
        Assert.Contains("rvm-ponto", marca.GetAttribute("class"));
        Assert.Equal(string.Empty, marca.TextContent);
        Assert.Equal("true", marca.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Com_label_o_numero_vira_desenho_e_o_leitor_ouve_o_label()
    {
        var cortado = Render<RvmBadge>(p => p.Add(x => x.Count, 3).Add(x => x.Label, "3 mensagens nao lidas"));

        Assert.Equal("true", cortado.Find(".rvm-badge").GetAttribute("aria-hidden"));
        Assert.Equal("3 mensagens nao lidas", cortado.Find(".rvm-so-leitor").TextContent);
    }

    [Theory]
    [InlineData(RvmColor.Primary, "rvm-primary")]
    [InlineData(RvmColor.Secondary, "rvm-secondary")]
    [InlineData(RvmColor.Info, "rvm-info")]
    [InlineData(RvmColor.Success, "rvm-success")]
    [InlineData(RvmColor.Warning, "rvm-warning")]
    [InlineData(RvmColor.Error, "rvm-error")]
    public void Papel_de_cor_vira_classe(RvmColor cor, string classe)
    {
        var cortado = Render<RvmBadge>(p => p.Add(x => x.Color, cor).Add(x => x.Content, "1"));

        Assert.Contains(classe, cortado.Find(".rvm-badge").GetAttribute("class"));
    }

    [Fact]
    public void Com_conteudo_fica_sobreposto_na_ancora()
    {
        var cortado = Render<RvmBadge>(p => p
            .Add(x => x.Count, 999)
            .Add(x => x.Max, 999)
            .AddChildContent("<svg class=\"envelope\"></svg>")
            .AddUnmatched("class", "minha")
            .AddUnmatched("data-teste", "x"));

        var ancora = cortado.Find("span.rvm-badge-ancora");
        Assert.Equal("rvm-badge-ancora minha", ancora.GetAttribute("class"));
        Assert.Equal("x", ancora.GetAttribute("data-teste"));
        Assert.NotNull(ancora.QuerySelector("svg.envelope"));
        Assert.Contains("rvm-sobreposto", cortado.Find(".rvm-badge").GetAttribute("class"));
    }
}

public class RvmBreadcrumbsTests : BunitContext
{
    private static readonly RvmBreadcrumbItem[] Trilha =
    [
        new("Inicio", "/", RvmIconName.Home),
        new("Relatorios", Disabled: true),
        new("Safra", "/safra"),
        new("Talhao 12", "/talhao/12")
    ];

    [Fact]
    public void E_um_nav_com_lista_ordenada_e_nome()
    {
        var cortado = Render<RvmBreadcrumbs>(p => p.Add(x => x.Items, Trilha));

        var nav = cortado.Find("nav");
        Assert.Equal("Trilha de navegacao", nav.GetAttribute("aria-label"));
        Assert.Equal("rvm-trilha", nav.GetAttribute("class"));
        Assert.Equal(4, cortado.FindAll("ol > li").Count);
    }

    [Fact]
    public void O_ultimo_passo_e_a_pagina_atual_e_nao_e_link()
    {
        var cortado = Render<RvmBreadcrumbs>(p => p.Add(x => x.Items, Trilha));

        var atual = cortado.Find("[aria-current=page]");
        Assert.Equal("Talhao 12", atual.TextContent.Trim());
        Assert.Equal("SPAN", atual.TagName);
        Assert.DoesNotContain(cortado.FindAll("a"), a => a.GetAttribute("href") == "/talhao/12");
    }

    [Fact]
    public void Passos_com_href_viram_link_e_desabilitado_nao()
    {
        var cortado = Render<RvmBreadcrumbs>(p => p.Add(x => x.Items, Trilha));

        var links = cortado.FindAll("a.rvm-ligacao");
        Assert.Equal(["/", "/safra"], links.Select(a => a.GetAttribute("href")));
        var desabilitado = cortado.Find(".rvm-desabilitado");
        Assert.Equal("true", desabilitado.GetAttribute("aria-disabled"));
        Assert.Equal("Relatorios", desabilitado.TextContent.Trim());
    }

    [Fact]
    public void Passo_sem_href_e_texto_simples()
    {
        var cortado = Render<RvmBreadcrumbs>(p => p.Add(x => x.Items, new RvmBreadcrumbItem[] { new("Solto"), new("Atual") }));

        var primeiro = cortado.FindAll("li")[0].QuerySelector(".rvm-rotulo")!;
        Assert.Equal("rvm-rotulo", primeiro.GetAttribute("class"));
    }

    [Fact]
    public void Separadores_ficam_entre_os_passos_e_sao_desenho()
    {
        var cortado = Render<RvmBreadcrumbs>(p => p.Add(x => x.Items, Trilha));

        var separadores = cortado.FindAll(".rvm-separador");
        Assert.Equal(3, separadores.Count);
        Assert.All(separadores, s => Assert.Equal("true", s.GetAttribute("aria-hidden")));
        Assert.All(separadores, s => Assert.Equal("/", s.TextContent.Trim()));
    }

    [Fact]
    public void Separador_seta_usa_o_icone()
    {
        var cortado = Render<RvmBreadcrumbs>(p => p
            .Add(x => x.Items, Trilha)
            .Add(x => x.Separator, RvmBreadcrumbSeparator.Chevron)
            .Add(x => x.AriaLabel, "Onde voce esta")
            .AddUnmatched("class", "minha"));

        Assert.All(cortado.FindAll(".rvm-separador"), s => Assert.NotNull(s.QuerySelector("svg")));
        Assert.Equal("Onde voce esta", cortado.Find("nav").GetAttribute("aria-label"));
        Assert.Equal("rvm-trilha minha", cortado.Find("nav").GetAttribute("class"));
    }

    [Fact]
    public void Icone_do_passo_aparece_antes_do_texto()
    {
        var cortado = Render<RvmBreadcrumbs>(p => p.Add(x => x.Items, Trilha));

        Assert.NotNull(cortado.Find("a[href='/']").QuerySelector("svg"));
        Assert.Null(cortado.Find("a[href='/safra']").QuerySelector("svg"));
    }
}

public class RvmPaginationTests : BunitContext
{
    private static string Sequencia(IRenderedComponent<RvmPagination> cortado)
        => string.Join(' ', cortado.FindAll("li > .rvm-numero, li > .rvm-reticencias").Select(e => e.TextContent.Trim()));

    [Theory]
    [InlineData(1, 4, "1 2 3 4")]
    [InlineData(1, 5, "1 2 … 5")]
    [InlineData(1, 13, "1 2 … 13")]
    [InlineData(7, 13, "1 … 6 7 8 … 13")]
    [InlineData(4, 13, "1 2 3 4 5 … 13")]
    [InlineData(13, 13, "1 … 12 13")]
    [InlineData(99, 13, "1 … 12 13")]
    public void Reticencias_so_onde_pula_mais_de_uma_pagina(int pagina, int total, string esperado)
    {
        var cortado = Render<RvmPagination>(p => p.Add(x => x.Page, pagina).Add(x => x.Count, total));

        Assert.Equal(esperado, Sequencia(cortado));
    }

    [Fact]
    public void Bordas_e_vizinhos_configuraveis()
    {
        var cortado = Render<RvmPagination>(p => p
            .Add(x => x.Page, 1).Add(x => x.Count, 13)
            .Add(x => x.BoundaryCount, 3).Add(x => x.SiblingCount, 0));

        Assert.Equal("1 2 3 … 11 12 13", Sequencia(cortado));
    }

    [Fact]
    public void Sem_paginas_so_as_setas_e_desabilitadas()
    {
        var cortado = Render<RvmPagination>(p => p.Add(x => x.Count, 0));

        Assert.Empty(cortado.FindAll(".rvm-numero"));
        Assert.All(cortado.FindAll("button.rvm-seta"), b => Assert.True(b.HasAttribute("disabled")));
    }

    [Fact]
    public void Pagina_atual_tem_aria_current_e_nome()
    {
        var cortado = Render<RvmPagination>(p => p.Add(x => x.Page, 2).Add(x => x.Count, 5));

        var atual = cortado.Find("[aria-current=page]");
        Assert.Equal("Pagina 2", atual.GetAttribute("aria-label"));
        Assert.Contains("rvm-atual", atual.GetAttribute("class"));
        Assert.Single(cortado.FindAll("[aria-current]"));
        Assert.Equal("Paginacao", cortado.Find("nav").GetAttribute("aria-label"));
    }

    [Fact]
    public void Clicar_numero_e_setas_muda_a_pagina()
    {
        var escolhida = 0;
        var cortado = Render<RvmPagination>(p => p
            .Add(x => x.Page, 2).Add(x => x.Count, 5)
            .Add(x => x.PageChanged, EventCallback.Factory.Create<int>(this, v => escolhida = v)));

        cortado.Find("button[aria-label='Pagina 4']").Click();
        Assert.Equal(4, escolhida);
        Assert.Equal("Pagina 4", cortado.Find("[aria-current=page]").GetAttribute("aria-label"));

        cortado.Find("button[aria-label='Proxima pagina']").Click();
        Assert.Equal(5, escolhida);
        Assert.True(cortado.Find("button[aria-label='Proxima pagina']").HasAttribute("disabled"));

        cortado.Find("button[aria-label='Pagina anterior']").Click();
        Assert.Equal(4, escolhida);
    }

    [Fact]
    public void Pagina_fora_da_faixa_vale_como_a_ultima()
    {
        var escolhida = 0;
        var cortado = Render<RvmPagination>(p => p
            .Add(x => x.Page, 99).Add(x => x.Count, 13)
            .Add(x => x.PageChanged, EventCallback.Factory.Create<int>(this, v => escolhida = v)));

        Assert.Equal("Pagina 13", cortado.Find("[aria-current=page]").GetAttribute("aria-label"));
        cortado.Find("button[aria-label='Pagina anterior']").Click();
        Assert.Equal(12, escolhida);
    }

    [Fact]
    public void Clicar_na_atual_nao_dispara()
    {
        var disparos = 0;
        var cortado = Render<RvmPagination>(p => p
            .Add(x => x.Page, 2).Add(x => x.Count, 5)
            .Add(x => x.PageChanged, EventCallback.Factory.Create<int>(this, _ => disparos++)));

        cortado.Find("[aria-current=page]").Click();

        Assert.Equal(0, disparos);
    }

    [Fact]
    public void Na_primeira_pagina_a_seta_anterior_fica_desabilitada()
    {
        var cortado = Render<RvmPagination>(p => p.Add(x => x.Page, 1).Add(x => x.Count, 5));

        Assert.True(cortado.Find("button[aria-label='Pagina anterior']").HasAttribute("disabled"));
        Assert.False(cortado.Find("button[aria-label='Proxima pagina']").HasAttribute("disabled"));
    }

    [Fact]
    public void Desabilitada_nao_navega_e_desabilita_tudo()
    {
        var disparos = 0;
        var cortado = Render<RvmPagination>(p => p
            .Add(x => x.Page, 2).Add(x => x.Count, 5).Add(x => x.Disabled, true)
            .Add(x => x.PageChanged, EventCallback.Factory.Create<int>(this, _ => disparos++)));

        Assert.All(cortado.FindAll("button"), b => Assert.True(b.HasAttribute("disabled")));
        Assert.Equal(0, disparos);
    }

    [Theory]
    [InlineData(RvmPaginationVariant.Text, RvmPaginationShape.Circular, RvmSize.Medium, RvmColor.Primary, "rvm-paginacao rvm-texto rvm-circular rvm-medio rvm-primary")]
    [InlineData(RvmPaginationVariant.Outlined, RvmPaginationShape.Rounded, RvmSize.Small, RvmColor.Secondary, "rvm-paginacao rvm-contorno rvm-arredondado rvm-pequeno rvm-secondary")]
    [InlineData(RvmPaginationVariant.Text, RvmPaginationShape.Circular, RvmSize.Large, RvmColor.Info, "rvm-paginacao rvm-texto rvm-circular rvm-grande rvm-info")]
    [InlineData(RvmPaginationVariant.Text, RvmPaginationShape.Circular, RvmSize.Medium, RvmColor.Success, "rvm-paginacao rvm-texto rvm-circular rvm-medio rvm-success")]
    [InlineData(RvmPaginationVariant.Text, RvmPaginationShape.Circular, RvmSize.Medium, RvmColor.Warning, "rvm-paginacao rvm-texto rvm-circular rvm-medio rvm-warning")]
    [InlineData(RvmPaginationVariant.Text, RvmPaginationShape.Circular, RvmSize.Medium, RvmColor.Error, "rvm-paginacao rvm-texto rvm-circular rvm-medio rvm-error")]
    public void Estilo_forma_tamanho_e_cor_viram_classes(
        RvmPaginationVariant variante, RvmPaginationShape forma, RvmSize tamanho, RvmColor cor, string esperado)
    {
        var cortado = Render<RvmPagination>(p => p
            .Add(x => x.Count, 3).Add(x => x.Variant, variante).Add(x => x.Shape, forma)
            .Add(x => x.Size, tamanho).Add(x => x.Color, cor));

        Assert.Equal(esperado, cortado.Find("nav").GetAttribute("class"));
    }

    [Fact]
    public void Sem_cor_a_atual_e_neutra_e_atributos_vao_ao_nav()
    {
        var cortado = Render<RvmPagination>(p => p
            .Add(x => x.Count, 3).Add(x => x.Color, (RvmColor?)null)
            .AddUnmatched("class", "minha").AddUnmatched("data-teste", "x"));

        var nav = cortado.Find("nav");
        Assert.Equal("rvm-paginacao rvm-texto rvm-circular rvm-medio rvm-neutro minha", nav.GetAttribute("class"));
        Assert.Equal("x", nav.GetAttribute("data-teste"));
    }
}

public class RvmTooltipTests : BunitContext
{
    [Fact]
    public void A_dica_tem_role_tooltip_e_o_id_chega_ao_conteudo()
    {
        var cortado = Render<RvmTooltip>(p => p
            .Add(x => x.Text, "Copiar link")
            .Add(x => x.ChildContent, (RenderFragment<string>)(id => b =>
            {
                b.OpenElement(0, "button");
                b.AddAttribute(1, "aria-describedby", id);
                b.AddContent(2, "Copiar");
                b.CloseElement();
            })));

        var dica = cortado.Find("[role=tooltip]");
        Assert.Equal("Copiar link", dica.TextContent);
        Assert.StartsWith("rvm-dica-", dica.Id);
        Assert.Equal(dica.Id, cortado.Find("button").GetAttribute("aria-describedby"));
        Assert.Equal("rvm-dica rvm-acima rvm-com-seta", dica.GetAttribute("class"));
    }

    [Fact]
    public void Id_informado_vence_o_gerado()
    {
        var cortado = Render<RvmTooltip>(p => p.Add(x => x.Text, "x").Add(x => x.Id, "minha-dica"));

        Assert.Equal("minha-dica", cortado.Find("[role=tooltip]").Id);
    }

    [Fact]
    public void Cada_instancia_gera_um_id_proprio()
    {
        var a = Render<RvmTooltip>(p => p.Add(x => x.Text, "a"));
        var b = Render<RvmTooltip>(p => p.Add(x => x.Text, "b"));

        Assert.NotEqual(a.Find("[role=tooltip]").Id, b.Find("[role=tooltip]").Id);
    }

    [Theory]
    [InlineData(RvmTooltipPlacement.Top, true, "rvm-dica rvm-acima rvm-com-seta")]
    [InlineData(RvmTooltipPlacement.Bottom, true, "rvm-dica rvm-abaixo rvm-com-seta")]
    [InlineData(RvmTooltipPlacement.Left, false, "rvm-dica rvm-esquerda rvm-sem-seta")]
    [InlineData(RvmTooltipPlacement.Right, true, "rvm-dica rvm-direita rvm-com-seta")]
    public void Posicao_e_seta_viram_classes(RvmTooltipPlacement posicao, bool seta, string esperado)
    {
        var cortado = Render<RvmTooltip>(p => p.Add(x => x.Text, "x").Add(x => x.Placement, posicao).Add(x => x.Arrow, seta));

        Assert.Equal(esperado, cortado.Find("[role=tooltip]").GetAttribute("class"));
    }

    [Fact]
    public void Esc_dispensa_e_sair_rearma()
    {
        var cortado = Render<RvmTooltip>(p => p.Add(x => x.Text, "x").AddUnmatched("class", "minha"));
        var raiz = cortado.Find("span.rvm-com-dica");
        Assert.Equal("rvm-com-dica minha", raiz.GetAttribute("class"));

        raiz.KeyDown(key: "a");
        Assert.DoesNotContain("rvm-dispensada", cortado.Find("span.rvm-com-dica").GetAttribute("class"));

        cortado.Find("span.rvm-com-dica").KeyDown(key: "Escape");
        Assert.Contains("rvm-dispensada", cortado.Find("span.rvm-com-dica").GetAttribute("class"));

        cortado.Find("span.rvm-com-dica").MouseLeave();
        Assert.DoesNotContain("rvm-dispensada", cortado.Find("span.rvm-com-dica").GetAttribute("class"));

        cortado.Find("span.rvm-com-dica").KeyDown(key: "Escape");
        cortado.Find("span.rvm-com-dica").FocusOut();
        Assert.DoesNotContain("rvm-dispensada", cortado.Find("span.rvm-com-dica").GetAttribute("class"));
    }
}
