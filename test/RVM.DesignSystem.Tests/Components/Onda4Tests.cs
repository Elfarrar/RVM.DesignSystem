using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RVM.DesignSystem.Components;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>
/// Comportamento e acessibilidade dos componentes de dados (onda 4).
/// </summary>
public class Onda4Tests : BunitContext
{
    public Onda4Tests()
    {
        // O grid importa um modulo JS para o estado indeterminado da caixa de "selecionar
        // todas"; o date picker chama FocusAsync. Sem o modo solto, os dois estourariam.
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private sealed record Pedido(int Numero, string Cliente, decimal Total, DateTime Emissao);

    private static readonly IReadOnlyList<Pedido> Pedidos =
    [
        new(3, "Construtora Alvorada", 1500.50m, new DateTime(2026, 3, 15)),
        new(1, "Aurora Materiais", 220.00m, new DateTime(2026, 1, 4)),
        new(2, "Zeta Engenharia", 980.25m, new DateTime(2026, 2, 20)),
    ];

    private static RenderFragment Grid(
        RvmSelectionMode selecao = RvmSelectionMode.None,
        bool carregando = false,
        string? erro = null,
        IReadOnlyList<Pedido>? itens = null,
        Action<IReadOnlyList<Pedido>>? aoSelecionar = null) => builder =>
    {
        builder.OpenComponent<RvmDataGrid<Pedido>>(0);
        builder.AddComponentParameter(1, nameof(RvmDataGrid<Pedido>.Caption), "Pedidos");
        builder.AddComponentParameter(2, nameof(RvmDataGrid<Pedido>.Items), itens ?? Pedidos);
        builder.AddComponentParameter(3, nameof(RvmDataGrid<Pedido>.SelectionMode), selecao);
        builder.AddComponentParameter(4, nameof(RvmDataGrid<Pedido>.Loading), carregando);
        builder.AddComponentParameter(5, nameof(RvmDataGrid<Pedido>.Error), erro);
        builder.AddComponentParameter(6, nameof(RvmDataGrid<Pedido>.RowLabel),
            (Func<Pedido, string>)(p => $"pedido {p.Numero}"));

        if (aoSelecionar is not null)
        {
            builder.AddComponentParameter(7, nameof(RvmDataGrid<Pedido>.SelectionChanged),
                EventCallback.Factory.Create<IReadOnlyList<Pedido>>(builder, aoSelecionar));
        }

        builder.AddAttribute(8, nameof(RvmDataGrid<Pedido>.Columns), (RenderFragment)(b =>
        {
            b.OpenComponent<RvmColumn<Pedido>>(0);
            b.AddComponentParameter(1, nameof(RvmColumn<Pedido>.Title), "Número");
            b.AddComponentParameter(2, nameof(RvmColumn<Pedido>.Field),
                (System.Linq.Expressions.Expression<Func<Pedido, object?>>)(p => p.Numero));
            b.AddComponentParameter(3, nameof(RvmColumn<Pedido>.Sortable), true);
            b.CloseComponent();

            b.OpenComponent<RvmColumn<Pedido>>(10);
            b.AddComponentParameter(11, nameof(RvmColumn<Pedido>.Title), "Cliente");
            b.AddComponentParameter(12, nameof(RvmColumn<Pedido>.Field),
                (System.Linq.Expressions.Expression<Func<Pedido, object?>>)(p => p.Cliente));
            b.AddComponentParameter(13, nameof(RvmColumn<Pedido>.Sortable), true);
            b.CloseComponent();

            b.OpenComponent<RvmColumn<Pedido>>(20);
            b.AddComponentParameter(21, nameof(RvmColumn<Pedido>.Title), "Total");
            b.AddComponentParameter(22, nameof(RvmColumn<Pedido>.Field),
                (System.Linq.Expressions.Expression<Func<Pedido, object?>>)(p => p.Total));
            b.AddComponentParameter(23, nameof(RvmColumn<Pedido>.Format), "C");
            b.AddComponentParameter(24, nameof(RvmColumn<Pedido>.Align), RvmAlignment.End);
            b.CloseComponent();
        }));

        builder.CloseComponent();
    };

    // ---------- RvmDataGrid: semantica ----------

    [Fact]
    public void O_grid_e_uma_TABELA_semantica_e_NAO_um_role_grid()
    {
        // role="grid" DESLIGA o modo de tabela do leitor de tela (Ctrl+Alt+setas, anuncio de
        // cabecalho ao mudar de coluna) e obriga a usar a navegacao que nos escrevermos — que
        // sera pior. O APG reserva `grid` para tabela INTERATIVA; aqui a celula e dado.
        var cut = Render(Grid());

        var tabela = cut.Find("table");
        Assert.Null(tabela.GetAttribute("role"));
        Assert.All(cut.FindAll("th"), th => Assert.Equal("col", th.GetAttribute("scope")));
    }

    [Fact]
    public void A_legenda_diz_a_ORDENACAO_corrente()
    {
        // Quem nao ve a seta do cabecalho precisa saber por que as linhas estao nessa ordem
        // ANTES de comecar a le-las.
        var cut = Render(Grid());

        Assert.Equal("Pedidos", cut.Find("caption").TextContent);

        cut.Find("th:has(button) button").Click();

        Assert.Contains("ordenada por Número, crescente",
            cut.Find("caption").TextContent, StringComparison.Ordinal);
    }

    // ---------- RvmDataGrid: ordenacao ----------

    [Fact]
    public void Coluna_ordenavel_nasce_com_aria_sort_NONE_e_nao_sem_o_atributo()
    {
        // "none" e o que faz o leitor de tela anunciar que a coluna PODE ser ordenada. A
        // ausencia do atributo nao diz nada.
        var cut = Render(Grid());

        var cabecalhos = cut.FindAll("th");
        Assert.Equal("none", cabecalhos[0].GetAttribute("aria-sort"));
        Assert.Equal("none", cabecalhos[1].GetAttribute("aria-sort"));

        // A coluna sem Sortable nao tem o atributo: ela nao pode ser ordenada.
        Assert.Null(cabecalhos[2].GetAttribute("aria-sort"));
    }

    [Fact]
    public void Ordenar_reordena_as_linhas_e_marca_o_aria_sort()
    {
        var cut = Render(Grid());

        cut.Find("th:has(button) button").Click();

        Assert.Equal("ascending", cut.FindAll("th")[0].GetAttribute("aria-sort"));

        var numeros = cut.FindAll("tbody tr td:first-child").Select(td => td.TextContent).ToList();
        Assert.Equal(["1", "2", "3"], numeros);
    }

    [Fact]
    public void O_segundo_clique_inverte_e_o_terceiro_NAO_volta_para_sem_ordenacao()
    {
        // Ciclo de dois estados: voltar ao "sem ordenacao" devolveria a ordem de origem, que
        // quase nunca e a que a pessoa quer — e ela precisaria clicar de novo para sair de la.
        var cut = Render(Grid());

        cut.Find("th:has(button) button").Click();
        cut.Find("th:has(button) button").Click();
        Assert.Equal("descending", cut.FindAll("th")[0].GetAttribute("aria-sort"));

        cut.Find("th:has(button) button").Click();
        Assert.Equal("ascending", cut.FindAll("th")[0].GetAttribute("aria-sort"));
    }

    [Fact]
    public void O_rotulo_do_botao_diz_o_que_o_clique_VAI_FAZER()
    {
        // O estado atual ja vem do aria-sort do th; repeti-lo no botao faria o leitor dizer a
        // mesma coisa duas vezes.
        var cut = Render(Grid());

        var botao = cut.Find("th:has(button) button");
        Assert.Equal("Ordenar por Número, crescente", botao.GetAttribute("aria-label"));

        botao.Click();

        Assert.Equal("Ordenar por Número, decrescente",
            cut.Find("th:has(button) button").GetAttribute("aria-label"));
    }

    [Fact]
    public void Coluna_Sortable_SEM_Field_falha_ALTO()
    {
        var e = Assert.Throws<InvalidOperationException>(() => Render((RenderFragment)(builder =>
        {
            builder.OpenComponent<RvmDataGrid<Pedido>>(0);
            builder.AddComponentParameter(1, nameof(RvmDataGrid<Pedido>.Caption), "x");
            builder.AddComponentParameter(2, nameof(RvmDataGrid<Pedido>.Items), Pedidos);
            builder.AddAttribute(3, nameof(RvmDataGrid<Pedido>.Columns), (RenderFragment)(b =>
            {
                b.OpenComponent<RvmColumn<Pedido>>(0);
                b.AddComponentParameter(1, nameof(RvmColumn<Pedido>.Title), "Sem campo");
                b.AddComponentParameter(2, nameof(RvmColumn<Pedido>.Sortable), true);
                b.CloseComponent();
            }));
            builder.CloseComponent();
        })));

        Assert.Contains("Sortable", e.Message, StringComparison.Ordinal);
    }

    // ---------- RvmDataGrid: formatacao ----------

    [Fact]
    public void O_valor_e_formatado_com_cultura_EXPLICITA()
    {
        // Sem cultura explicita, o mesmo numero sai com ponto ou virgula dependendo de onde o
        // servidor esta hospedado (`CLAUDE.md` § Convencoes).
        var cut = Render(Grid());

        Assert.Contains("1.500,50", cut.Markup, StringComparison.Ordinal);
    }

    // ---------- RvmDataGrid: selecao ----------

    [Fact]
    public void Cada_caixa_de_selecao_diz_QUAL_linha_ela_marca()
    {
        // Sem isto, uma tabela de 20 linhas da 20 caixas chamadas "Selecionar", e quem nao ve a
        // tela nao sabe o que esta marcando.
        var cut = Render(Grid(RvmSelectionMode.Multiple));

        var caixas = cut.FindAll("tbody input[type=checkbox]");
        Assert.Equal(3, caixas.Count);
        Assert.Equal("Selecionar pedido 3", caixas[0].GetAttribute("aria-label"));
    }

    [Fact]
    public void Selecao_UNICA_usa_radio_e_nao_checkbox()
    {
        // O controle nativo ja comunica "uma" ou "varias" ao leitor de tela — sem precisar de
        // nenhum texto explicando.
        var cut = Render(Grid(RvmSelectionMode.Single));

        Assert.Equal(3, cut.FindAll("tbody input[type=radio]").Count);
        Assert.Empty(cut.FindAll("tbody input[type=checkbox]"));
    }

    [Fact]
    public void Selecionar_todas_age_so_sobre_A_PAGINA()
    {
        // "Selecionar todos" que alcanca linhas invisiveis e como se apagam registros por
        // engano. Aqui todas cabem numa pagina, e o teste crava que sao as visiveis.
        IReadOnlyList<Pedido>? selecionados = null;
        var cut = Render(Grid(RvmSelectionMode.Multiple, aoSelecionar: s => selecionados = s));

        cut.Find("thead input[type=checkbox]").Change(true);

        Assert.NotNull(selecionados);
        Assert.Equal(3, selecionados!.Count);
    }

    [Fact]
    public void A_linha_selecionada_e_anunciada_por_aria_selected()
    {
        var cut = Render((RenderFragment)(builder =>
        {
            builder.OpenComponent<RvmDataGrid<Pedido>>(0);
            builder.AddComponentParameter(1, nameof(RvmDataGrid<Pedido>.Caption), "Pedidos");
            builder.AddComponentParameter(2, nameof(RvmDataGrid<Pedido>.Items), Pedidos);
            builder.AddComponentParameter(3, nameof(RvmDataGrid<Pedido>.SelectionMode), RvmSelectionMode.Multiple);
            // A segunda linha ja chega selecionada.
            builder.AddComponentParameter(4, nameof(RvmDataGrid<Pedido>.Selection),
                (IReadOnlyList<Pedido>)new[] { Pedidos[1] });
            builder.AddAttribute(5, nameof(RvmDataGrid<Pedido>.Columns), (RenderFragment)(b =>
            {
                b.OpenComponent<RvmColumn<Pedido>>(0);
                b.AddComponentParameter(1, nameof(RvmColumn<Pedido>.Title), "Cliente");
                b.AddComponentParameter(2, nameof(RvmColumn<Pedido>.Field),
                    (System.Linq.Expressions.Expression<Func<Pedido, object?>>)(p => p.Cliente));
                b.CloseComponent();
            }));
            builder.CloseComponent();
        }));

        var linhas = cut.FindAll("tbody tr");
        Assert.Equal("false", linhas[0].GetAttribute("aria-selected"));
        Assert.Equal("true", linhas[1].GetAttribute("aria-selected"));

        // E a linha tem destaque VISUAL alem do atributo: numa tabela com 3 de 20 marcadas, a
        // caixa marcada sozinha e pequena demais para a pessoa achar o que selecionou.
        Assert.Contains("rvm-data-grid__linha--selecionada",
            linhas[1].GetAttribute("class")!, StringComparison.Ordinal);
    }

    [Fact]
    public void Sem_selecao_a_linha_NAO_carrega_aria_selected()
    {
        // aria-selected numa tabela que nao tem selecao faz o leitor anunciar "nao selecionado"
        // em toda linha — ruido puro.
        var cut = Render(Grid());

        Assert.All(cut.FindAll("tbody tr"),
            tr => Assert.Null(tr.GetAttribute("aria-selected")));
    }

    // ---------- RvmDataGrid: os tres estados que nao sao "a lista" ----------

    [Fact]
    public void Carregando_mostra_esqueleto_NAS_LINHAS_e_marca_aria_busy()
    {
        var cut = Render(Grid(carregando: true));

        Assert.Equal("true", cut.Find("table").GetAttribute("aria-busy"));
        Assert.NotEmpty(cut.FindAll("tbody .rvm-skeleton"));
    }

    [Fact]
    public void Lista_vazia_cai_no_estado_vazio_e_NAO_numa_tabela_em_branco()
    {
        var cut = Render(Grid(itens: []));

        Assert.Contains("Nada por aqui ainda", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void Com_erro_a_lista_da_lugar_ao_estado_de_ERRO()
    {
        var cut = Render(Grid(erro: "A consulta demorou demais."));

        Assert.Contains("Não foi possível carregar", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("A consulta demorou demais.", cut.Markup, StringComparison.Ordinal);
    }

    // ---------- RvmPagination ----------

    [Fact]
    public void A_paginacao_resume_por_ITEM_e_nao_por_pagina()
    {
        // "21–40 de 137" informa mais que "pagina 2 de 7": diz quantos itens existem e onde a
        // pessoa esta dentro deles.
        var cut = Render<RvmPagination>(p => p
            .Add(x => x.Page, 2)
            .Add(x => x.PageSize, 20)
            .Add(x => x.TotalItems, 137));

        Assert.Equal("21–40 de 137", cut.Find("[role=status]").TextContent);
    }

    [Fact]
    public void A_ultima_pagina_resume_ate_o_TOTAL_e_nao_alem()
    {
        var cut = Render<RvmPagination>(p => p
            .Add(x => x.Page, 7)
            .Add(x => x.PageSize, 20)
            .Add(x => x.TotalItems, 137));

        Assert.Equal("121–137 de 137", cut.Find("[role=status]").TextContent);
    }

    [Fact]
    public void Sem_itens_o_resumo_diz_isso_em_vez_de_zero_a_zero()
    {
        var cut = Render<RvmPagination>(p => p.Add(x => x.TotalItems, 0));

        Assert.Equal("Nenhum item", cut.Find("[role=status]").TextContent);
    }

    [Fact]
    public void Os_botoes_dizem_o_DESTINO_e_nao_o_simbolo()
    {
        // Quem ouve "botao sinal de maior" nao sabe para onde vai.
        var cut = Render<RvmPagination>(p => p
            .Add(x => x.Page, 3)
            .Add(x => x.PageSize, 10)
            .Add(x => x.TotalItems, 100));

        var rotulos = cut.FindAll("button").Select(b => b.GetAttribute("aria-label")).ToList();

        Assert.Contains("Primeira página", rotulos);
        Assert.Contains("Página anterior, 2", rotulos);
        Assert.Contains("Página seguinte, 4", rotulos);
        Assert.Contains("Última página", rotulos);
    }

    [Fact]
    public void A_pagina_atual_e_marcada_por_aria_current()
    {
        var cut = Render<RvmPagination>(p => p
            .Add(x => x.Page, 3)
            .Add(x => x.PageSize, 10)
            .Add(x => x.TotalItems, 100));

        var atual = cut.Find("[aria-current=page]");
        Assert.Equal("3", atual.TextContent.Trim());
    }

    [Fact]
    public void A_janela_de_numeros_nao_ENCOLHE_perto_das_pontas()
    {
        // Ela desliza para dentro em vez de mostrar menos numeros: assim a fileira nao muda de
        // largura ao navegar e os botoes nao dancam debaixo do cursor.
        var naPrimeira = Render<RvmPagination>(p => p
            .Add(x => x.Page, 1).Add(x => x.PageSize, 10).Add(x => x.TotalItems, 100));

        var noMeio = Render<RvmPagination>(p => p
            .Add(x => x.Page, 5).Add(x => x.PageSize, 10).Add(x => x.TotalItems, 100));

        Assert.Equal(
            naPrimeira.FindAll("[aria-current], button[aria-label^='Página ']").Count,
            noMeio.FindAll("[aria-current], button[aria-label^='Página ']").Count);
    }

    [Fact]
    public void Trocar_o_tamanho_da_pagina_VOLTA_para_a_primeira()
    {
        // Com 100 por pagina, a pagina 7 de 20-em-20 nao existe mais. A primeira e o unico
        // destino que sempre existe.
        var paginas = new List<int>();

        var cut = Render<RvmPagination>(p => p
            .Add(x => x.Page, 7)
            .Add(x => x.PageSize, 20)
            .Add(x => x.TotalItems, 137)
            .Add(x => x.PageChanged, EventCallback.Factory.Create<int>(this, paginas.Add)));

        cut.Find("select").Change("100");

        Assert.Contains(1, paginas);
    }

    // ---------- RvmFilterBar ----------

    [Fact]
    public void A_barra_de_filtros_conta_os_ativos_e_diz_do_QUE()
    {
        var cut = Render<RvmFilterBar>(p => p
            .Add(x => x.ActiveCount, 3)
            .AddChildContent("<input />"));

        Assert.Contains("3 filtro(s) aplicado(s)", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void Os_campos_ficam_no_DOM_mesmo_fechados()
    {
        // Esconder com `hidden` mantem o aria-controls apontando para algo que existe, e um
        // campo preenchido nao perde o valor ao colapsar a barra.
        var cut = Render<RvmFilterBar>(p => p
            .Add(x => x.Expanded, false)
            .AddChildContent("<input id='meu-filtro' />"));

        var campos = cut.Find("[id^=rvm-filter-bar-]");
        Assert.True(campos.HasAttribute("hidden"));
        Assert.NotNull(cut.Find("#meu-filtro"));

        Assert.Equal("false", cut.Find("button[aria-expanded]").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void O_botao_de_limpar_so_aparece_com_filtro_ativo()
    {
        var semFiltro = Render<RvmFilterBar>(p => p
            .Add(x => x.OnClear, EventCallback.Factory.Create(this, () => { })));

        Assert.Single(semFiltro.FindAll("button"));

        var comFiltro = Render<RvmFilterBar>(p => p
            .Add(x => x.ActiveCount, 2)
            .Add(x => x.OnClear, EventCallback.Factory.Create(this, () => { })));

        Assert.Equal(2, comFiltro.FindAll("button").Count);
    }

    // ---------- RvmDatePicker ----------

    [Fact]
    public void O_campo_de_data_e_TEXTO_e_nao_type_date()
    {
        // type="date" impoe o formato do LOCALE DO NAVEGADOR: quem esta com o Windows em ingles
        // ve mm/dd/yyyy num sistema em portugues e digita 03/15 achando que e 15 de marco.
        var cut = Render<RvmDatePicker>(p => p.Add(x => x.Label, "Emissão"));

        Assert.Equal("text", cut.Find("input").GetAttribute("type"));
        Assert.Equal("dd/mm/aaaa", cut.Find("input").GetAttribute("placeholder"));
    }

    [Fact]
    public void Digitar_dd_MM_yyyy_define_a_data_SEM_abrir_o_calendario()
    {
        DateTime? escolhida = null;

        var cut = Render<RvmDatePicker>(p => p
            .Add(x => x.Label, "Emissão")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<DateTime?>(this, d => escolhida = d)));

        cut.Find("input").Change("15/03/2026");

        Assert.Equal(new DateTime(2026, 3, 15), escolhida);
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Texto_que_nao_e_data_MOSTRA_o_erro_sem_apagar_o_que_foi_digitado()
    {
        // Limpar o campo aqui esconderia o erro e o dado ao mesmo tempo.
        var cut = Render<RvmDatePicker>(p => p.Add(x => x.Label, "Emissão"));

        cut.Find("input").Change("31/02/2026");

        Assert.Equal("true", cut.Find("input").GetAttribute("aria-invalid"));
        Assert.Contains("Data inválida", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void O_calendario_nomeia_cada_dia_POR_EXTENSO()
    {
        // Um botao chamado "15" numa grade de 42 botoes nao diz de que mes nem de que ano.
        var cut = Render<RvmDatePicker>(p => p
            .Add(x => x.Label, "Emissão")
            .Add(x => x.Value, new DateTime(2026, 3, 15)));

        cut.Find("button[aria-expanded]").Click();

        var dias = cut.FindAll("[role=dialog] tbody button")
            .Select(b => b.GetAttribute("aria-label"))
            .ToList();

        Assert.Contains("15 de março de 2026", dias);
    }

    [Fact]
    public void A_semana_do_calendario_comeca_no_DOMINGO()
    {
        var cut = Render<RvmDatePicker>(p => p.Add(x => x.Label, "Emissão"));

        cut.Find("button[aria-expanded]").Click();

        var primeiro = cut.FindAll("[role=dialog] thead th")[0];
        Assert.Equal("domingo", primeiro.GetAttribute("abbr"));
    }

    [Fact]
    public void O_titulo_do_mes_capitaliza_SO_a_primeira_letra()
    {
        // "Março De 2026" era o que saia: o ToTitleCase e regra de ingles e sobe cada palavra.
        // Em portugues a preposicao fica minuscula.
        var cut = Render<RvmDatePicker>(p => p
            .Add(x => x.Label, "Emissão")
            .Add(x => x.Value, new DateTime(2026, 3, 15)));

        cut.Find("button[aria-expanded]").Click();

        Assert.Equal("Março de 2026", cut.Find(".rvm-date-picker__titulo").TextContent);
    }

    [Fact]
    public void ESC_fecha_o_calendario()
    {
        var cut = Render<RvmDatePicker>(p => p.Add(x => x.Label, "Emissão"));

        cut.Find("button[aria-expanded]").Click();
        Assert.NotEmpty(cut.FindAll("[role=dialog]"));

        cut.Find("[role=dialog]").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Data_fora_do_minimo_e_maximo_fica_DESABILITADA_na_grade()
    {
        var cut = Render<RvmDatePicker>(p => p
            .Add(x => x.Label, "Emissão")
            .Add(x => x.Value, new DateTime(2026, 3, 15))
            .Add(x => x.Min, new DateTime(2026, 3, 10))
            .Add(x => x.Max, new DateTime(2026, 3, 20)));

        cut.Find("button[aria-expanded]").Click();

        var dia5 = cut.FindAll("[role=dialog] tbody button")
            .First(b => b.GetAttribute("aria-label") == "05 de março de 2026");

        Assert.True(dia5.HasAttribute("disabled"));
    }

    // ---------- RvmAutocomplete ----------

    [Fact]
    public void O_autocomplete_e_uma_COMBOBOX_completa()
    {
        var cut = Render<RvmAutocomplete<string>>(p => p
            .Add(x => x.Label, "Cliente")
            .Add(x => x.SearchAsync, (t, ct) => Task.FromResult<IReadOnlyList<string>>([])));

        var campo = cut.Find("input");
        Assert.Equal("combobox", campo.GetAttribute("role"));
        Assert.Equal("false", campo.GetAttribute("aria-expanded"));
        Assert.Equal("list", campo.GetAttribute("aria-autocomplete"));
        Assert.False(string.IsNullOrWhiteSpace(campo.GetAttribute("aria-controls")));
    }

    [Fact]
    public void A_regiao_de_anuncio_existe_MESMO_SEM_BUSCA()
    {
        // Mesma regra do RvmToastHost: criada junto com o resultado, ela nao seria observada e
        // a contagem nao seria anunciada.
        var cut = Render<RvmAutocomplete<string>>(p => p
            .Add(x => x.Label, "Cliente")
            .Add(x => x.SearchAsync, (t, ct) => Task.FromResult<IReadOnlyList<string>>([])));

        var regiao = cut.Find("[role=status]");
        Assert.Equal("polite", regiao.GetAttribute("aria-live"));
    }

    [Fact]
    public async Task A_busca_ANTERIOR_e_cancelada_quando_a_pessoa_digita_de_novo()
    {
        // Sem isso, digitar "sao paulo" dispara varias buscas e a que responder por ultimo
        // vence — podendo ser a de "sao".
        var cancelamentos = 0;

        var cut = Render<RvmAutocomplete<string>>(p => p
            .Add(x => x.Label, "Cidade")
            .Add(x => x.DebounceMs, 20)
            .Add(x => x.SearchAsync, async (termo, ct) =>
            {
                try
                {
                    await Task.Delay(300, ct);
                }
                catch (OperationCanceledException)
                {
                    Interlocked.Increment(ref cancelamentos);
                    throw;
                }

                return [termo];
            }));

        cut.Find("input").Input("sao");
        await Task.Delay(60);
        cut.Find("input").Input("sao paulo");
        await Task.Delay(400);

        Assert.True(cancelamentos > 0, "A busca anterior não foi cancelada.");
    }

    [Fact]
    public async Task Abaixo_do_minimo_de_caracteres_a_busca_NEM_ACONTECE()
    {
        var buscas = 0;

        var cut = Render<RvmAutocomplete<string>>(p => p
            .Add(x => x.Label, "Cidade")
            .Add(x => x.MinLength, 3)
            .Add(x => x.DebounceMs, 10)
            .Add(x => x.SearchAsync, (termo, ct) =>
            {
                Interlocked.Increment(ref buscas);
                return Task.FromResult<IReadOnlyList<string>>([termo]);
            }));

        cut.Find("input").Input("sa");
        await Task.Delay(80);

        Assert.Equal(0, buscas);
    }
}
