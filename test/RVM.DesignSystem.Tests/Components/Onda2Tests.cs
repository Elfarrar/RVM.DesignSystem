using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using RVM.DesignSystem.Components;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>
/// Comportamento e acessibilidade dos componentes de layout e navegacao (onda 2).
/// </summary>
/// <remarks>
/// O criterio para um teste entrar aqui e o mesmo da onda 1: prova algo que o bUnit consegue
/// provar — marcacao, ARIA, callbacks — e que quebraria em silencio. O que so o navegador sabe
/// (o que e visivel, o que tem foco de verdade, o que o leitor de tela fala) fica com o axe no
/// E2E. Um nao substitui o outro.
/// </remarks>
public class Onda2Tests : BunitContext
{
    public Onda2Tests()
    {
        // O RvmTabs chama ElementReference.FocusAsync ao navegar por setas. Sem o modo solto,
        // toda interacao de teclado estouraria por falta de handler de JS registrado.
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    // ---------- RvmStack ----------

    [Fact]
    public void Stack_traduz_cada_opcao_numa_classe_da_escala()
    {
        var cut = Render<RvmStack>(p => p
            .Add(x => x.Orientation, RvmOrientation.Horizontal)
            .Add(x => x.Gap, RvmSpacing.Lg)
            .Add(x => x.Justify, RvmJustify.SpaceBetween));

        var classe = cut.Find("div").GetAttribute("class")!;

        Assert.Contains("rvm-stack--horizontal", classe, StringComparison.Ordinal);
        Assert.Contains("rvm-stack--gap-lg", classe, StringComparison.Ordinal);

        // SpaceBetween vira space-between, e nao spacebetween: e o que faz o nome do enum e o
        // nome da classe CSS continuarem casando quando alguem acrescentar um membro novo.
        Assert.Contains("rvm-stack--justify-space-between", classe, StringComparison.Ordinal);
    }

    [Fact]
    public void A_classe_do_consumidor_SOMA_com_as_internas()
    {
        // Regra do `CLAUDE.md` § Convencoes. Vale para todo componente; o Stack e a amostra.
        var cut = Render<RvmStack>(p => p.Add(x => x.Class, "minha-classe"));

        var classe = cut.Find("div").GetAttribute("class")!;

        Assert.Contains("rvm-stack", classe, StringComparison.Ordinal);
        Assert.Contains("minha-classe", classe, StringComparison.Ordinal);

        // E entra por ULTIMO: com especificidade igual, o CSS do consumidor vence o nosso.
        Assert.EndsWith("minha-classe", classe, StringComparison.Ordinal);
    }

    // ---------- RvmGrid ----------

    [Fact]
    public void Grid_faz_cada_faixa_HERDAR_a_anterior_quando_nao_e_informada()
    {
        // Quem escreve so `Columns=1 ColumnsLg=3` espera 1 coluna no celular e 3 no desktop.
        // Sem a heranca, o layout voltaria para 1 coluna entre 640 e 1024 — uma faixa inteira
        // de larguras errada, e justamente a do tablet, que ninguem testa.
        var cut = Render<RvmGrid>(p => p
            .Add(x => x.Columns, 1)
            .Add(x => x.ColumnsLg, 3));

        var estilo = cut.Find("div").GetAttribute("style")!;

        Assert.Contains("--rvm-grid-columns:1", estilo, StringComparison.Ordinal);
        Assert.Contains("--rvm-grid-columns-sm:1", estilo, StringComparison.Ordinal);
        Assert.Contains("--rvm-grid-columns-md:1", estilo, StringComparison.Ordinal);
        Assert.Contains("--rvm-grid-columns-lg:3", estilo, StringComparison.Ordinal);
        Assert.Contains("--rvm-grid-columns-xl:3", estilo, StringComparison.Ordinal);
    }

    // ---------- RvmDivider ----------

    [Fact]
    public void Divisor_SEM_rotulo_e_decorativo_e_some_do_leitor_de_tela()
    {
        var cut = Render<RvmDivider>();

        var hr = cut.Find("hr");
        Assert.Equal("true", hr.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Divisor_COM_rotulo_vira_separador_semantico()
    {
        // Com rotulo o traco carrega informacao ("ou", "Ontem"): quem nao ve a linha ainda
        // precisa saber que a lista mudou de grupo.
        var cut = Render<RvmDivider>(p => p.Add(x => x.Label, "ou"));

        var div = cut.Find("div");
        Assert.Equal("separator", div.GetAttribute("role"));
        Assert.Equal("horizontal", div.GetAttribute("aria-orientation"));
        Assert.Null(div.GetAttribute("aria-hidden"));
        Assert.Contains("ou", cut.Markup, StringComparison.Ordinal);
    }

    // ---------- RvmCard ----------

    [Fact]
    public void Card_sem_acao_e_uma_div_e_NAO_entra_na_ordem_de_tabulacao()
    {
        var cut = Render<RvmCard>(p => p.AddChildContent("texto"));

        Assert.NotNull(cut.Find("div.rvm-card"));
        Assert.Empty(cut.FindAll("button"));
        Assert.Empty(cut.FindAll("a"));
    }

    [Fact]
    public void Card_clicavel_e_um_BOTAO_de_verdade()
    {
        // Uma div com onclick clica com o mouse e falha em tudo o mais: nao recebe foco, nao
        // responde a Enter nem a Espaco, e nao aparece na lista de botoes do leitor de tela.
        var clicado = false;

        var cut = Render<RvmCard>(p => p
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => clicado = true))
            .AddChildContent("texto"));

        var botao = cut.Find("button.rvm-card");
        Assert.Equal("button", botao.GetAttribute("type"));

        botao.Click();
        Assert.True(clicado);
    }

    [Fact]
    public void Card_com_destino_e_um_LINK_de_verdade()
    {
        var cut = Render<RvmCard>(p => p
            .Add(x => x.Href, "produtos/42")
            .AddChildContent("texto"));

        Assert.Equal("produtos/42", cut.Find("a.rvm-card").GetAttribute("href"));
    }

    [Fact]
    public void Card_limita_a_elevacao_em_vez_de_estourar()
    {
        // Sombra errada e um detalhe; excecao em producao e uma tela em branco.
        var cut = Render<RvmCard>(p => p.Add(x => x.Elevation, 99));

        Assert.Contains("rvm-card--elev-5", cut.Find("div").GetAttribute("class")!, StringComparison.Ordinal);
    }

    // ---------- RvmChip ----------

    [Fact]
    public void Chip_selecionavel_E_removivel_NAO_aninha_botao_dentro_de_botao()
    {
        // HTML proibe elemento interativo aninhado, e o efeito pratico nao e um aviso do
        // validador: e o Enter disparando as duas acoes de uma vez.
        var cut = Render<RvmChip>(p => p
            .Add(x => x.Selectable, true)
            .Add(x => x.OnRemove, EventCallback.Factory.Create<MouseEventArgs>(this, () => { }))
            .AddChildContent("Vencidos"));

        var botoes = cut.FindAll("button");
        Assert.Equal(2, botoes.Count);

        // Nenhum dos dois esta dentro do outro: sao irmaos.
        Assert.Empty(cut.FindAll("button button"));
    }

    [Fact]
    public void Chip_selecionavel_anuncia_o_estado_por_aria_pressed()
    {
        var cut = Render<RvmChip>(p => p
            .Add(x => x.Selectable, true)
            .AddChildContent("Ativos"));

        var botao = cut.Find("button.rvm-chip__acao");
        Assert.Equal("false", botao.GetAttribute("aria-pressed"));

        botao.Click();
        Assert.Equal("true", cut.Find("button.rvm-chip__acao").GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Chip_removivel_da_nome_PROPRIO_ao_botao_de_remover()
    {
        // Numa lista de oito chips, oito botoes chamados "Remover" sao indistinguiveis para
        // quem nao ve a tela.
        var cut = Render<RvmChip>(p => p
            .Add(x => x.OnRemove, EventCallback.Factory.Create<MouseEventArgs>(this, () => { }))
            .Add(x => x.RemoveLabel, "Remover o filtro Vencidos")
            .AddChildContent("Vencidos"));

        Assert.Equal("Remover o filtro Vencidos",
            cut.Find("button.rvm-chip__remover").GetAttribute("aria-label"));
    }

    // ---------- RvmAvatar ----------

    [Fact]
    public void Avatar_com_iniciais_anuncia_o_NOME_INTEIRO_e_esconde_as_letras()
    {
        // "MS" anunciado letra por letra nao identifica ninguem.
        var cut = Render<RvmAvatar>(p => p.Add(x => x.Name, "Maria Aparecida Silva"));

        var raiz = cut.Find("span.rvm-avatar");
        Assert.Equal("img", raiz.GetAttribute("role"));
        Assert.Equal("Maria Aparecida Silva", raiz.GetAttribute("aria-label"));
        Assert.Equal("true", cut.Find("span.rvm-avatar__iniciais").GetAttribute("aria-hidden"));
        Assert.Equal("MS", cut.Find("span.rvm-avatar__iniciais").TextContent);
    }

    [Fact]
    public void Avatar_com_imagem_poe_o_nome_no_alt_e_NAO_repete_no_aria_label()
    {
        // Com os dois, o leitor de tela anunciaria o nome duas vezes.
        var cut = Render<RvmAvatar>(p => p
            .Add(x => x.Name, "Ana Silva")
            .Add(x => x.Src, "/fotos/ana.jpg"));

        Assert.Equal("Ana Silva", cut.Find("img").GetAttribute("alt"));
        Assert.Null(cut.Find("span.rvm-avatar").GetAttribute("aria-label"));
    }

    [Fact]
    public void Avatar_SEM_nome_e_decorativo()
    {
        var cut = Render<RvmAvatar>();

        var raiz = cut.Find("span.rvm-avatar");
        Assert.Equal("true", raiz.GetAttribute("aria-hidden"));
        Assert.Null(raiz.GetAttribute("role"));
    }

    [Fact]
    public void Grupo_de_avatares_e_uma_LISTA_e_o_excedente_tem_nome()
    {
        var cut = Render<RvmAvatarGroup>(p => p
            .Add(x => x.Max, 2)
            .Add(x => x.People,
            [
                new RvmAvatarGroup.Pessoa("Ana Silva"),
                new RvmAvatarGroup.Pessoa("Bruno Costa"),
                new RvmAvatarGroup.Pessoa("Carla Dias"),
                new RvmAvatarGroup.Pessoa("Diego Reis"),
            ]));

        Assert.NotNull(cut.Find("ul.rvm-avatar-group"));

        var excedente = cut.Find("span.rvm-avatar-group__excedente");
        Assert.Equal("mais 2", excedente.GetAttribute("aria-label"));
        Assert.Contains("+2", excedente.TextContent, StringComparison.Ordinal);
    }

    [Fact]
    public void Grupo_mostra_a_pessoa_em_vez_de_um_mais_um()
    {
        // "+1" ocupa o mesmo espaco de um avatar e diz menos.
        var cut = Render<RvmAvatarGroup>(p => p
            .Add(x => x.Max, 2)
            .Add(x => x.People,
            [
                new RvmAvatarGroup.Pessoa("Ana Silva"),
                new RvmAvatarGroup.Pessoa("Bruno Costa"),
                new RvmAvatarGroup.Pessoa("Carla Dias"),
            ]));

        Assert.Empty(cut.FindAll("span.rvm-avatar-group__excedente"));
        Assert.Equal(3, cut.FindAll("li.rvm-avatar-group__item").Count);
    }

    // ---------- RvmTabs ----------

    private static RenderFragment TresAbas(string? ativa) => builder =>
    {
        builder.OpenComponent<RvmTabs>(0);
        builder.AddComponentParameter(1, nameof(RvmTabs.Label), "Seções");
        builder.AddComponentParameter(2, nameof(RvmTabs.Value), ativa);
        builder.AddAttribute(3, nameof(RvmTabs.ChildContent), (RenderFragment)(b =>
        {
            var i = 0;

            foreach (var (id, rotulo, desabilitada) in new[]
            {
                ("geral", "Geral", false),
                ("fiscal", "Fiscal", false),
                ("extra", "Extra", true),
            })
            {
                b.OpenComponent<RvmTab>(i++);
                b.AddComponentParameter(i++, nameof(RvmTab.Id), id);
                b.AddComponentParameter(i++, nameof(RvmTab.Label), rotulo);
                b.AddComponentParameter(i++, nameof(RvmTab.Disabled), desabilitada);
                b.AddAttribute(i++, nameof(RvmTab.ChildContent),
                    (RenderFragment)(c => c.AddContent(0, $"painel {id}")));
                b.CloseComponent();
            }
        }));
        builder.CloseComponent();
    };

    [Fact]
    public void Abas_montam_a_estrutura_ARIA_completa()
    {
        var cut = Render(TresAbas("geral"));

        Assert.Equal("Seções", cut.Find("[role=tablist]").GetAttribute("aria-label"));

        var primeira = cut.Find("#rvm-tab-geral");
        Assert.Equal("tab", primeira.GetAttribute("role"));
        Assert.Equal("true", primeira.GetAttribute("aria-selected"));
        Assert.Equal("rvm-tabpanel-geral", primeira.GetAttribute("aria-controls"));

        var painel = cut.Find("#rvm-tabpanel-geral");
        Assert.Equal("tabpanel", painel.GetAttribute("role"));
        Assert.Equal("rvm-tab-geral", painel.GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void Abas_usam_tabindex_MOVEL_e_nao_uma_parada_por_aba()
    {
        // Numa tablist, Tab entra na lista e sai dela; quem navega dentro sao as setas. Sem o
        // tabindex movel, uma tela com oito abas obriga a oito paradas antes do conteudo.
        var cut = Render(TresAbas("geral"));

        Assert.Equal("0", cut.Find("#rvm-tab-geral").GetAttribute("tabindex"));
        Assert.Equal("-1", cut.Find("#rvm-tab-fiscal").GetAttribute("tabindex"));
    }

    [Fact]
    public void So_o_painel_da_aba_ativa_existe_no_DOM()
    {
        var cut = Render(TresAbas("geral"));

        Assert.Contains("painel geral", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("painel fiscal", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void Seta_para_a_direita_troca_de_aba_e_PULA_a_desabilitada()
    {
        var cut = Render(TresAbas("geral"));

        cut.Find("#rvm-tab-geral").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Assert.Equal("true", cut.Find("#rvm-tab-fiscal").GetAttribute("aria-selected"));

        // A terceira esta desabilitada: a seta volta para a primeira, sem parar nela.
        cut.Find("#rvm-tab-fiscal").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Assert.Equal("true", cut.Find("#rvm-tab-geral").GetAttribute("aria-selected"));
    }

    [Fact]
    public void End_vai_para_a_ultima_aba_HABILITADA()
    {
        var cut = Render(TresAbas("geral"));

        cut.Find("#rvm-tab-geral").KeyDown(new KeyboardEventArgs { Key = "End" });

        Assert.Equal("true", cut.Find("#rvm-tab-fiscal").GetAttribute("aria-selected"));
    }

    [Fact]
    public void Sem_selecao_explicita_a_primeira_aba_HABILITADA_assume()
    {
        var cut = Render(TresAbas(null));

        Assert.Equal("true", cut.Find("#rvm-tab-geral").GetAttribute("aria-selected"));
    }

    [Fact]
    public void Duas_abas_com_o_mesmo_Id_falham_ALTO()
    {
        // Ids repetidos fariam aria-controls apontar para o elemento errado e as setas pularem
        // uma aba — dois defeitos que so aparecem para quem usa leitor de tela.
        var e = Assert.Throws<InvalidOperationException>(() => Render((RenderFragment)(builder =>
        {
            builder.OpenComponent<RvmTabs>(0);
            builder.AddComponentParameter(1, nameof(RvmTabs.Label), "x");
            builder.AddAttribute(2, nameof(RvmTabs.ChildContent), (RenderFragment)(b =>
            {
                for (var i = 0; i < 2; i++)
                {
                    b.OpenComponent<RvmTab>(i * 10);
                    b.AddComponentParameter(i * 10 + 1, nameof(RvmTab.Id), "repetido");
                    b.AddComponentParameter(i * 10 + 2, nameof(RvmTab.Label), "x");
                    b.CloseComponent();
                }
            }));
            builder.CloseComponent();
        })));

        Assert.Contains("repetido", e.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RvmTab_fora_de_um_RvmTabs_falha_em_vez_de_renderizar_painel_solto()
    {
        var e = Assert.Throws<InvalidOperationException>(() =>
            Render<RvmTab>(p => p.Add(x => x.Id, "solta").Add(x => x.Label, "Solta")));

        Assert.Contains("RvmTabs", e.Message, StringComparison.Ordinal);
    }

    // ---------- RvmBreadcrumb ----------

    [Fact]
    public void Trilha_marca_o_ultimo_item_como_pagina_atual_e_NAO_como_link()
    {
        var cut = Render<RvmBreadcrumb>(p => p.Add(x => x.Items,
        [
            new RvmBreadcrumb.Item("Início", ""),
            new RvmBreadcrumb.Item("Produtos", "produtos"),
            new RvmBreadcrumb.Item("Cadastro"),
        ]));

        Assert.Equal("Trilha de navegação", cut.Find("nav").GetAttribute("aria-label"));

        var atual = cut.Find("[aria-current=page]");
        Assert.Equal("Cadastro", atual.TextContent);
        Assert.Equal("SPAN", atual.TagName);

        // Dois links, e nao tres: o item atual e onde a pessoa esta, nao um lugar para ir.
        Assert.Equal(2, cut.FindAll("a.rvm-breadcrumb__link").Count);
    }

    [Fact]
    public void Trilha_longa_colapsa_no_MEIO_e_o_botao_revela_o_resto()
    {
        // Colapsar o fim deixaria a pessoa sem saber onde esta, que e a unica coisa que a
        // trilha precisa dizer.
        var cut = Render<RvmBreadcrumb>(p => p
            .Add(x => x.MaxItems, 3)
            .Add(x => x.Items,
            [
                new RvmBreadcrumb.Item("Início", ""),
                new RvmBreadcrumb.Item("A", "a"),
                new RvmBreadcrumb.Item("B", "b"),
                new RvmBreadcrumb.Item("C", "c"),
                new RvmBreadcrumb.Item("Atual"),
            ]));

        Assert.Contains("Início", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Atual", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain(">B<", cut.Markup, StringComparison.Ordinal);

        // As reticencias sao um BOTAO: um texto cortado que so o mouse revela e informacao
        // perdida para quem navega por teclado.
        cut.Find("button.rvm-breadcrumb__mais").Click();

        Assert.Contains(">B<", cut.Markup, StringComparison.Ordinal);
    }

    // ---------- RvmNavItem ----------

    [Fact]
    public void Item_de_navegacao_na_rota_atual_ganha_aria_current()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("produtos");

        var cut = Render<RvmNavItem>(p => p
            .Add(x => x.Text, "Produtos")
            .Add(x => x.Href, "produtos"));

        Assert.Equal("page", cut.Find("a").GetAttribute("aria-current"));
    }

    [Fact]
    public void Item_de_navegacao_fica_ativo_tambem_nas_rotas_FILHAS()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("produtos/42/editar");

        var cut = Render<RvmNavItem>(p => p
            .Add(x => x.Text, "Produtos")
            .Add(x => x.Href, "produtos"));

        Assert.Equal("page", cut.Find("a").GetAttribute("aria-current"));
    }

    [Fact]
    public void O_item_da_RAIZ_nao_fica_aceso_em_toda_pagina()
    {
        // "" e prefixo de toda rota que existe. Sem o tratamento, "Início" ficaria ativo no
        // site inteiro — o defeito mais comum de menu lateral feito a mao.
        Services.GetRequiredService<NavigationManager>().NavigateTo("produtos");

        var cut = Render<RvmNavItem>(p => p
            .Add(x => x.Text, "Início")
            .Add(x => x.Href, ""));

        Assert.Null(cut.Find("a").GetAttribute("aria-current"));
    }

    [Fact]
    public void Item_com_prefixo_parecido_NAO_e_confundido()
    {
        // "produtos-antigos" comeca com "produtos", mas nao e filha dela.
        Services.GetRequiredService<NavigationManager>().NavigateTo("produtos-antigos");

        var cut = Render<RvmNavItem>(p => p
            .Add(x => x.Text, "Produtos")
            .Add(x => x.Href, "produtos"));

        Assert.Null(cut.Find("a").GetAttribute("aria-current"));
    }

    [Fact]
    public void Item_com_contador_diz_do_QUE_e_o_contador()
    {
        // Sem isso o leitor de tela anuncia "Mensagens" e depois "3" solto.
        var cut = Render<RvmNavItem>(p => p
            .Add(x => x.Text, "Mensagens")
            .Add(x => x.Href, "mensagens")
            .Add(x => x.Badge, "3")
            .Add(x => x.BadgeLabel, "{0} mensagens não lidas"));

        Assert.Contains("3 mensagens não lidas", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void Item_SEM_icone_ganha_a_inicial_para_nao_sumir_na_barra_colapsada()
    {
        // A biblioteca oferece o botao de colapsar, entao o estado colapsado e responsabilidade
        // dela: sem a inicial, um menu cujos itens nao tem Icon vira uma coluna de linhas em
        // branco, cada uma clicavel e nenhuma identificavel.
        var cut = Render<RvmNavItem>(p => p
            .Add(x => x.Text, "relatorios")
            .Add(x => x.Href, "relatorios"));

        var inicial = cut.Find("span.rvm-nav-item__inicial");
        Assert.Equal("R", inicial.TextContent);

        // aria-hidden: o texto completo continua no DOM, e uma letra solta nao identifica nada.
        Assert.Equal("true", inicial.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Item_COM_icone_nao_ganha_inicial()
    {
        var cut = Render<RvmNavItem>(p => p
            .Add(x => x.Text, "Início")
            .Add(x => x.Href, "")
            .Add(x => x.Icon, "house"));

        Assert.Empty(cut.FindAll("span.rvm-nav-item__inicial"));
    }

    [Fact]
    public void Item_com_sub_itens_vira_expansor_com_aria_expanded()
    {
        var cut = Render<RvmNavItem>(p => p
            .Add(x => x.Text, "Cadastros")
            .AddChildContent("<li>filho</li>"));

        var botao = cut.Find("button");
        Assert.Equal("false", botao.GetAttribute("aria-expanded"));

        botao.Click();
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-expanded"));
        Assert.Contains("filho", cut.Markup, StringComparison.Ordinal);
    }

    // ---------- RvmAppShell + Topbar + Sidebar ----------

    private static RenderFragment Casca(RvmDensity densidade = RvmDensity.Comfortable) => builder =>
    {
        builder.OpenComponent<RvmAppShell>(0);
        builder.AddComponentParameter(1, nameof(RvmAppShell.Density), densidade);
        builder.AddAttribute(2, nameof(RvmAppShell.Topbar), (RenderFragment)(b =>
        {
            b.OpenComponent<RvmTopbar>(0);
            b.CloseComponent();
        }));
        builder.AddAttribute(3, nameof(RvmAppShell.Sidebar), (RenderFragment)(b =>
        {
            b.OpenComponent<RvmSidebar>(0);
            b.AddAttribute(1, nameof(RvmSidebar.ChildContent), (RenderFragment)(c =>
            {
                c.OpenComponent<RvmNavItem>(0);
                c.AddComponentParameter(1, nameof(RvmNavItem.Text), "Início");
                c.AddComponentParameter(2, nameof(RvmNavItem.Href), "");
                c.CloseComponent();
            }));
            b.CloseComponent();
        }));
        builder.AddAttribute(4, nameof(RvmAppShell.ChildContent),
            (RenderFragment)(b => b.AddMarkupContent(0, "<h1>Conteúdo</h1>")));
        builder.CloseComponent();
    };

    [Fact]
    public void A_casca_entrega_os_tres_marcos_de_pagina()
    {
        // header, nav e main sao o que faz o leitor de tela oferecer "pular para a navegacao"
        // e "pular para o conteudo" sem ninguem escrever nada para isso.
        var cut = Render(Casca());

        Assert.NotNull(cut.Find("header"));
        Assert.NotNull(cut.Find("nav"));
        Assert.NotNull(cut.Find("main"));
    }

    [Fact]
    public void O_link_de_pulo_e_o_PRIMEIRO_tabulavel_e_aponta_para_o_main()
    {
        var cut = Render(Casca());

        var primeiro = cut.Find("a");
        Assert.Equal("Pular para o conteúdo", primeiro.TextContent.Trim());
        Assert.Equal($"#{RvmShellContext.ContentId}", primeiro.GetAttribute("href"));

        // E o alvo existe: um link de pulo que nao leva a lugar nenhum passa despercebido para
        // sempre, porque continua parecendo certo.
        Assert.Equal(RvmShellContext.ContentId, cut.Find("main").GetAttribute("id"));
    }

    [Fact]
    public void O_botao_de_menu_e_a_gaveta_CONCORDAM()
    {
        // Os dois vivem em componentes irmaos. Sem o RvmShellContext, o consumidor teria que
        // ligar um no outro na mao — o codigo duplicado que esta onda existe para eliminar.
        var cut = Render(Casca());

        var botao = cut.Find("button.rvm-topbar__menu");
        Assert.Equal("false", botao.GetAttribute("aria-expanded"));
        Assert.Equal("Abrir o menu", botao.GetAttribute("aria-label"));
        Assert.Empty(cut.FindAll(".rvm-app-shell__veu"));

        botao.Click();

        Assert.Equal("true", cut.Find("button.rvm-topbar__menu").GetAttribute("aria-expanded"));
        Assert.Equal("Fechar o menu", cut.Find("button.rvm-topbar__menu").GetAttribute("aria-label"));
        Assert.Contains("rvm-sidebar--aberta", cut.Find("nav").GetAttribute("class")!, StringComparison.Ordinal);
        Assert.NotNull(cut.Find(".rvm-app-shell__veu"));
    }

    [Fact]
    public void Clicar_no_veu_fecha_a_gaveta()
    {
        var cut = Render(Casca());

        cut.Find("button.rvm-topbar__menu").Click();
        cut.Find(".rvm-app-shell__veu").Click();

        Assert.Empty(cut.FindAll(".rvm-app-shell__veu"));
    }

    [Fact]
    public void Topbar_SEM_casca_nao_mostra_um_botao_de_menu_que_nao_faz_nada()
    {
        var cut = Render<RvmTopbar>();

        Assert.Empty(cut.FindAll("button.rvm-topbar__menu"));
    }

    [Fact]
    public void O_botao_de_colapsar_muda_de_NOME_conforme_o_que_vai_fazer()
    {
        // Um nome fixo ("alternar a barra lateral") obriga quem nao ve a tela a adivinhar o
        // que o botao vai fazer.
        var cut = Render(Casca());

        var botao = cut.Find("button.rvm-sidebar__colapsar");
        Assert.Equal("Recolher a barra lateral", botao.GetAttribute("aria-label"));
        Assert.Equal("true", botao.GetAttribute("aria-expanded"));

        botao.Click();

        var depois = cut.Find("button.rvm-sidebar__colapsar");
        Assert.Equal("Expandir a barra lateral", depois.GetAttribute("aria-label"));
        Assert.Equal("false", depois.GetAttribute("aria-expanded"));
        Assert.Contains("rvm-sidebar--colapsada", cut.Find("nav").GetAttribute("class")!, StringComparison.Ordinal);
    }

    // ---------- Densidade (RF-05) ----------

    [Fact]
    public void A_densidade_confortavel_NAO_emite_atributo_inerte()
    {
        var cut = Render(Casca());

        Assert.Null(cut.Find("div.rvm-app-shell").GetAttribute("data-rvm-density"));
    }

    [Fact]
    public void A_densidade_compacta_e_uma_troca_na_CAMADA_DE_TOKEN()
    {
        // Um atributo na raiz, e nada mais. Nenhum componente sabe que densidade existe: eles
        // leem --rvm-control-*, e um componente escrito depois ja nasce obedecendo.
        var cut = Render(Casca(RvmDensity.Compact));

        Assert.Equal("compact", cut.Find("div.rvm-app-shell").GetAttribute("data-rvm-density"));
    }

    [Fact]
    public void Os_controles_da_onda_1_leem_o_token_de_densidade()
    {
        // O portao que garante que a densidade continua ligada. Se alguem trocar o padding
        // vertical de volta para a escala de espacamento, a compacta para de funcionar em
        // silencio: o atributo continua no HTML e nada muda na tela.
        var raiz = new DirectoryInfo(AppContext.BaseDirectory);

        while (raiz is not null && !File.Exists(Path.Combine(raiz.FullName, "RVM.DesignSystem.slnx")))
        {
            raiz = raiz.Parent;
        }

        Assert.NotNull(raiz);

        var basicos = Path.Combine(raiz!.FullName, "src", "RVM.DesignSystem", "Components", "Basicos");

        foreach (var arquivo in new[]
        {
            "RvmButton.razor.css", "RvmTextField.razor.css",
            "RvmTextArea.razor.css", "RvmSelect.razor.css",
        })
        {
            var css = File.ReadAllText(Path.Combine(basicos, arquivo));

            Assert.True(
                css.Contains("--rvm-control-padding-y-", StringComparison.Ordinal),
                $"{arquivo} nao usa nenhum token de densidade: a densidade compacta nao tem efeito nele.");
        }
    }
}
