using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RVM.DesignSystem.Components.Table;

namespace RVM.DesignSystem.Tests.Components;

public sealed record Talhao(string Nome, int Hectares, DateOnly Plantio);

public class RvmTableTests : BunitContext
{
    private static readonly Talhao[] Talhoes =
    [
        new("Bravo", 30, new DateOnly(2026, 10, 3)),
        new("Alfa", 120, new DateOnly(2026, 9, 20)),
        new("Charlie", 75, new DateOnly(2026, 11, 1))
    ];

    public RvmTableTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<RvmTable<Talhao>> Tabela(
        Action<ComponentParameterCollectionBuilder<RvmTable<Talhao>>>? extra = null,
        IEnumerable<Talhao>? itens = null)
        => Render<RvmTable<Talhao>>(p =>
        {
            p.Add(x => x.Items, itens ?? Talhoes)
             .Add(x => x.Caption, "Talhoes da fazenda")
             .AddChildContent<RvmTableColumn<Talhao>>(c => c.Add(x => x.Title, "Talhao").Add(x => x.Value, t => t.Nome).Add(x => x.Sortable, true))
             .AddChildContent<RvmTableColumn<Talhao>>(c => c.Add(x => x.Title, "Area").Add(x => x.Value, t => t.Hectares).Add(x => x.Sortable, true).Add(x => x.Align, RvmTableAlign.End))
             .AddChildContent<RvmTableColumn<Talhao>>(c => c.Add(x => x.Title, "Plantio").Add(x => x.Value, t => t.Plantio).Add(x => x.Format, "dd/MM/yyyy"));
            extra?.Invoke(p);
        });

    private static string[] Coluna(IRenderedComponent<RvmTable<Talhao>> cortado, int indice)
        => [.. cortado.FindAll("tbody tr").Select(l => l.QuerySelectorAll("td")[indice].TextContent.Trim())];

    [Fact]
    public void E_uma_tabela_nativa_com_legenda_e_titulos_de_coluna()
    {
        var cortado = Tabela();

        Assert.Equal("Talhoes da fazenda", cortado.Find("caption").TextContent);
        Assert.Equal(["Talhao", "Area", "Plantio"], cortado.FindAll("th[scope=col]").Select(t => t.TextContent.Trim()));
        Assert.Equal(["Bravo", "Alfa", "Charlie"], Coluna(cortado, 0));
        Assert.Equal("Talhoes da fazenda", cortado.Find(".rvm-rolagem").GetAttribute("aria-label"));
        Assert.Equal("0", cortado.Find(".rvm-rolagem").GetAttribute("tabindex"));
        Assert.Contains("rvm-fim", cortado.FindAll("tbody tr")[0].QuerySelectorAll("td")[1].ClassName);
    }

    [Fact]
    public void Valor_formatado_sai_na_cultura_corrente()
    {
        var antes = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("pt-BR");
        try
        {
            var cortado = Render<RvmTable<Talhao>>(p => p
                .Add(x => x.Items, Talhoes)
                .Add(x => x.Caption, "Talhoes")
                .AddChildContent<RvmTableColumn<Talhao>>(c => c.Add(x => x.Title, "Produtividade").Add(x => x.Value, t => t.Hectares / 8d).Add(x => x.Format, "N2")));

            Assert.Equal("3,75", Coluna(cortado, 0)[0]);
        }
        finally
        {
            CultureInfo.CurrentCulture = antes;
        }
    }

    [Fact]
    public void Titulo_ordena_crescente_decrescente_e_volta_ao_original()
    {
        var cortado = Tabela();
        Assert.Empty(cortado.FindAll("th[aria-sort]"));
        Assert.Empty(cortado.FindAll("th")[2].QuerySelectorAll("button"));

        cortado.FindAll("th button")[1].Click();
        Assert.Equal("ascending", cortado.FindAll("th")[1].GetAttribute("aria-sort"));
        Assert.Equal(["30", "75", "120"], Coluna(cortado, 1));

        cortado.FindAll("th button")[1].Click();
        Assert.Equal("descending", cortado.FindAll("th")[1].GetAttribute("aria-sort"));
        Assert.Equal(["120", "75", "30"], Coluna(cortado, 1));

        cortado.FindAll("th button")[1].Click();
        Assert.Empty(cortado.FindAll("th[aria-sort]"));
        Assert.Equal(["30", "120", "75"], Coluna(cortado, 1));
    }

    [Fact]
    public void Ordenar_outra_coluna_comeca_crescente_e_so_ela_tem_aria_sort()
    {
        var cortado = Tabela();

        cortado.FindAll("th button")[1].Click();
        cortado.FindAll("th button")[1].Click();
        cortado.FindAll("th button")[0].Click();

        Assert.Single(cortado.FindAll("th[aria-sort]"));
        Assert.Equal("ascending", cortado.FindAll("th")[0].GetAttribute("aria-sort"));
        Assert.Equal(["Alfa", "Bravo", "Charlie"], Coluna(cortado, 0));
    }

    [Fact]
    public void Ordenacao_inicial_vale_desde_o_primeiro_render()
    {
        var cortado = Render<RvmTable<Talhao>>(p => p
            .Add(x => x.Items, Talhoes)
            .Add(x => x.Caption, "Talhoes")
            .AddChildContent<RvmTableColumn<Talhao>>(c => c.Add(x => x.Title, "Talhao").Add(x => x.Value, t => t.Nome).Add(x => x.Sortable, true).Add(x => x.InitialSort, RvmSortDirection.Descending)));

        Assert.Equal("descending", cortado.Find("th").GetAttribute("aria-sort"));
        Assert.Equal(["Charlie", "Bravo", "Alfa"], Coluna(cortado, 0));
    }

    [Fact]
    public void Marcar_linha_avisa_e_destaca()
    {
        IReadOnlyCollection<Talhao>? recebidos = null;
        var cortado = Tabela(p => p
            .Add(x => x.Selectable, true)
            .Add(x => x.SelectedItemsChanged, EventCallback.Factory.Create<IReadOnlyCollection<Talhao>>(this, s => recebidos = s)));

        var caixas = cortado.FindAll("tbody input[type=checkbox]");
        Assert.Equal(["Selecionar Bravo", "Selecionar Alfa", "Selecionar Charlie"], caixas.Select(c => c.GetAttribute("aria-label")));

        caixas[1].Change(true);

        Assert.Equal([Talhoes[1]], recebidos);
        Assert.Contains("rvm-marcada", cortado.FindAll("tbody tr")[1].ClassName);
        Assert.Contains("rvm-indeterminado", cortado.Find("thead .rvm-checkbox").ClassName);

        cortado.FindAll("tbody input[type=checkbox]")[1].Change(false);
        Assert.Empty(recebidos!);
        Assert.DoesNotContain("rvm-marcada", cortado.FindAll("tbody tr")[1].ClassName);
    }

    [Fact]
    public void Caixa_do_cabecalho_marca_e_desmarca_todas()
    {
        IReadOnlyCollection<Talhao>? recebidos = null;
        var cortado = Tabela(p => p
            .Add(x => x.Selectable, true)
            .Add(x => x.SelectedItemsChanged, EventCallback.Factory.Create<IReadOnlyCollection<Talhao>>(this, s => recebidos = s)));

        var todas = cortado.Find("thead input[type=checkbox]");
        Assert.Equal("Selecionar todas as linhas", todas.GetAttribute("aria-label"));
        todas.Change(true);

        Assert.Equal(3, recebidos!.Count);
        Assert.Equal(3, cortado.FindAll("tr.rvm-marcada").Count);
        Assert.True(cortado.Find("thead input[type=checkbox]").HasAttribute("checked"));

        cortado.Find("thead input[type=checkbox]").Change(false);
        Assert.Empty(recebidos);
    }

    [Fact]
    public void Selecao_de_fora_marca_as_linhas_e_rotulo_proprio_vale()
    {
        var cortado = Tabela(p => p
            .Add(x => x.Selectable, true)
            .Add(x => x.SelectedItems, [Talhoes[2]])
            .Add(x => x.RowLabel, t => $"Marcar o talhao {t.Nome}"));

        Assert.Single(cortado.FindAll("tr.rvm-marcada"));
        Assert.Equal("Marcar o talhao Charlie", cortado.FindAll("tbody input[type=checkbox]")[2].GetAttribute("aria-label"));

        cortado.Render(p => p.Add(x => x.SelectedItems, [Talhoes[0], Talhoes[1]]));
        Assert.Equal(2, cortado.FindAll("tr.rvm-marcada").Count);
    }

    [Fact]
    public void Sem_linhas_mostra_o_aviso_ocupando_todas_as_colunas()
    {
        var cortado = Tabela(p => p.Add(x => x.Selectable, true), itens: []);

        var vazia = cortado.Find("tr.rvm-linha-vazia td");
        Assert.Equal("Nenhum registro para mostrar.", vazia.TextContent);
        Assert.Equal("4", vazia.GetAttribute("colspan"));
        Assert.True(cortado.Find("thead input[type=checkbox]").HasAttribute("disabled"));
    }

    [Fact]
    public void Densa_e_conteudo_livre_na_celula()
    {
        var cortado = Render<RvmTable<Talhao>>(p => p
            .Add(x => x.Items, Talhoes)
            .Add(x => x.Caption, "Talhoes")
            .Add(x => x.Dense, true)
            .AddChildContent<RvmTableColumn<Talhao>>(c => c
                .Add(x => x.Title, "Talhao")
                .Add(x => x.ChildContent, (RenderFragment<Talhao>)(t => b => { b.OpenElement(0, "strong"); b.AddContent(1, t.Nome); b.CloseElement(); }))));

        Assert.Contains("rvm-densa", cortado.Find(".rvm-tabela").ClassName);
        Assert.Equal("Bravo", cortado.Find("tbody strong").TextContent);
        Assert.Empty(cortado.FindAll("th button"));
    }

    [Fact]
    public void Celulas_recebem_o_atributo_de_escopo_do_css()
    {
        var cortado = Tabela();

        Assert.Contains(cortado.Find("tbody td").Attributes, a => a.Name.StartsWith("b-", StringComparison.Ordinal));
        Assert.Contains(cortado.Find("th").Attributes, a => a.Name.StartsWith("b-", StringComparison.Ordinal));
    }

    [Fact]
    public void Atributos_extras_vao_para_a_raiz()
    {
        var cortado = Tabela(p => p.AddUnmatched("class", "minha").AddUnmatched("data-teste", "x"));

        var raiz = cortado.Find("[data-teste=x]");
        Assert.Contains("rvm-tabela", raiz.ClassName);
        Assert.Contains("minha", raiz.ClassName);
    }

    [Fact]
    public void Coluna_fora_da_tabela_explica_o_erro()
    {
        var erro = Assert.Throws<InvalidOperationException>(() => Render<RvmTableColumn<Talhao>>(p => p.Add(x => x.Title, "Solta")));
        Assert.Contains("RvmTable", erro.Message);
    }

    [Fact]
    public void Titulo_novo_da_coluna_aparece_no_mesmo_render_e_coluna_removida_some()
    {
        var cortado = Render<TabelaComColunaVariavel>(p => p.Add(x => x.Titulo, "Area"));
        Assert.Equal(["Talhao", "Area"], cortado.FindAll("th").Select(t => t.TextContent.Trim()));

        cortado.Render(p => p.Add(x => x.Titulo, "Hectares"));
        Assert.Equal(["Talhao", "Hectares"], cortado.FindAll("th").Select(t => t.TextContent.Trim()));

        cortado.Render(p => p.Add(x => x.Titulo, null));
        Assert.Equal(["Talhao"], cortado.FindAll("th").Select(t => t.TextContent.Trim()));
        Assert.Single(cortado.Find("tbody tr").QuerySelectorAll("td"));
    }

    private sealed class TabelaComColunaVariavel : ComponentBase
    {
        [Parameter] public string? Titulo { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<RvmTable<Talhao>>(0);
            builder.AddComponentParameter(1, nameof(RvmTable<Talhao>.Items), Talhoes);
            builder.AddComponentParameter(2, nameof(RvmTable<Talhao>.Caption), "Talhoes");
            builder.AddComponentParameter(3, nameof(RvmTable<Talhao>.ChildContent), (RenderFragment)(b =>
            {
                b.OpenComponent<RvmTableColumn<Talhao>>(0);
                b.AddComponentParameter(1, nameof(RvmTableColumn<Talhao>.Title), "Talhao");
                b.AddComponentParameter(2, nameof(RvmTableColumn<Talhao>.Value), (Func<Talhao, object?>)(t => t.Nome));
                b.CloseComponent();
                if (Titulo is not null)
                {
                    b.OpenComponent<RvmTableColumn<Talhao>>(3);
                    b.AddComponentParameter(4, nameof(RvmTableColumn<Talhao>.Title), Titulo);
                    b.AddComponentParameter(5, nameof(RvmTableColumn<Talhao>.Value), (Func<Talhao, object?>)(t => t.Hectares));
                    b.CloseComponent();
                }
            }));
            builder.CloseComponent();
        }
    }
}

public class RvmDataGridTests : BunitContext
{
    private static readonly Talhao[] Treze =
        [.. Enumerable.Range(1, 13).Select(i => new Talhao($"Talhao {i:00}", i * 10, new DateOnly(2026, 9, i)))];

    public RvmDataGridTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<RvmDataGrid<Talhao>> Grade(
        Action<ComponentParameterCollectionBuilder<RvmDataGrid<Talhao>>>? extra = null,
        IEnumerable<Talhao>? itens = null,
        int tamanho = 5)
        => Render<RvmDataGrid<Talhao>>(p =>
        {
            p.Add(x => x.Items, itens ?? Treze)
             .Add(x => x.Caption, "Talhoes")
             .Add(x => x.PageSize, tamanho)
             .AddChildContent<RvmTableColumn<Talhao>>(c => c.Add(x => x.Title, "Talhao").Add(x => x.Value, t => t.Nome).Add(x => x.Sortable, true))
             .AddChildContent<RvmTableColumn<Talhao>>(c => c.Add(x => x.Title, "Area").Add(x => x.Value, t => t.Hectares).Add(x => x.Filterable, false));
            extra?.Invoke(p);
        });

    private static string[] Nomes(IRenderedComponent<RvmDataGrid<Talhao>> cortado)
        => [.. cortado.FindAll("tbody tr").Select(l => l.QuerySelector("td")!.TextContent.Trim())];

    [Fact]
    public void Pagina_mostra_o_intervalo_e_anda_pelas_setas()
    {
        var paginas = new List<int>();
        var cortado = Grade(p => p.Add(x => x.PageChanged, EventCallback.Factory.Create<int>(this, paginas.Add)));

        Assert.Contains("rvm-grade-dados", cortado.Find(".rvm-tabela").ClassName);
        Assert.Equal(5, cortado.FindAll("tbody tr").Count);
        Assert.Equal("1–5 de 13", cortado.Find(".rvm-intervalo").TextContent);
        Assert.Equal("status", cortado.Find(".rvm-intervalo").GetAttribute("role"));
        Assert.True(cortado.Find("button[aria-label='Pagina anterior']").HasAttribute("disabled"));

        cortado.Find("button[aria-label='Proxima pagina']").Click();
        Assert.Equal("6–10 de 13", cortado.Find(".rvm-intervalo").TextContent);
        Assert.Equal("Talhao 06", Nomes(cortado)[0]);

        cortado.Find("button[aria-label='Proxima pagina']").Click();
        Assert.Equal("11–13 de 13", cortado.Find(".rvm-intervalo").TextContent);
        Assert.Equal(3, cortado.FindAll("tbody tr").Count);
        Assert.True(cortado.Find("button[aria-label='Proxima pagina']").HasAttribute("disabled"));

        cortado.Find("button[aria-label='Pagina anterior']").Click();
        Assert.Equal([2, 3, 2], paginas);
    }

    [Fact]
    public void Novo_render_sem_bind_nao_volta_para_a_primeira_pagina()
    {
        var cortado = Grade();
        cortado.Find("button[aria-label='Proxima pagina']").Click();

        cortado.Render(p => p.Add(x => x.Caption, "Talhoes da fazenda"));

        Assert.Equal("6–10 de 13", cortado.Find(".rvm-intervalo").TextContent);
    }

    [Fact]
    public void Linhas_por_pagina_troca_o_tamanho_e_volta_ao_inicio()
    {
        var tamanho = 0;
        var cortado = Grade(p => p
            .Add(x => x.Page, 3)
            .Add(x => x.PageSizeChanged, EventCallback.Factory.Create<int>(this, t => tamanho = t)));

        var rotulo = cortado.Find("label.rvm-rotulo-tamanho");
        var select = cortado.Find("select");
        Assert.Equal(select.Id, rotulo.GetAttribute("for"));
        Assert.Equal(["5", "10", "25"], select.QuerySelectorAll("option").Select(o => o.TextContent));

        select.Change("10");

        Assert.Equal(10, tamanho);
        Assert.Equal("1–10 de 13", cortado.Find(".rvm-intervalo").TextContent);
    }

    [Fact]
    public void Tamanho_fora_das_opcoes_entra_na_lista_e_valor_invalido_e_ignorado()
    {
        var cortado = Grade(tamanho: 7);

        Assert.Equal(["5", "7", "10", "25"], cortado.FindAll("option").Select(o => o.TextContent));
        cortado.Find("select").Change("abc");
        Assert.Equal("1–7 de 13", cortado.Find(".rvm-intervalo").TextContent);
    }

    [Fact]
    public void Pagina_alem_do_fim_mostra_a_ultima()
    {
        var cortado = Grade(p => p.Add(x => x.Page, 9));

        Assert.Equal("11–13 de 13", cortado.Find(".rvm-intervalo").TextContent);
    }

    [Fact]
    public void Filtro_por_coluna_procura_sem_diferenciar_maiuscula_e_volta_a_primeira_pagina()
    {
        var cortado = Grade();
        cortado.Find("button[aria-label='Proxima pagina']").Click();

        var filtros = cortado.FindAll("input.rvm-filtro");
        Assert.Single(filtros);
        Assert.Equal("Filtrar por Talhao", filtros[0].GetAttribute("aria-label"));

        filtros[0].Input("HAO 1");

        Assert.Equal(["Talhao 10", "Talhao 11", "Talhao 12", "Talhao 13"], Nomes(cortado));
        Assert.Equal("1–4 de 4", cortado.Find(".rvm-intervalo").TextContent);

        cortado.Find("input.rvm-filtro").Input("zzz");
        Assert.Equal("Nenhum registro encontrado para esse filtro.", cortado.Find("tr.rvm-linha-vazia td").TextContent);
        Assert.Equal("0–0 de 0", cortado.Find(".rvm-intervalo").TextContent);

        cortado.Find("input.rvm-filtro").Input("");
        Assert.Equal(5, cortado.FindAll("tbody tr").Count);
    }

    [Fact]
    public void Grade_vazia_sem_filtro_usa_o_texto_de_sem_registros()
    {
        var cortado = Grade(itens: []);

        Assert.Equal("Nenhum registro para mostrar.", cortado.Find("tr.rvm-linha-vazia td").TextContent);
    }

    [Fact]
    public void Ordenar_volta_para_a_primeira_pagina()
    {
        var cortado = Grade();
        cortado.Find("button[aria-label='Proxima pagina']").Click();

        cortado.Find("th button").Click();
        cortado.Find("th button").Click();

        Assert.Equal("1–5 de 13", cortado.Find(".rvm-intervalo").TextContent);
        Assert.Equal("Talhao 13", Nomes(cortado)[0]);
    }

    [Fact]
    public void Marcar_todas_na_grade_vale_so_para_a_pagina()
    {
        IReadOnlyCollection<Talhao>? recebidos = null;
        var cortado = Grade(p => p
            .Add(x => x.Selectable, true)
            .Add(x => x.SelectedItemsChanged, EventCallback.Factory.Create<IReadOnlyCollection<Talhao>>(this, s => recebidos = s)));

        var todas = cortado.Find("thead input[type=checkbox]");
        Assert.Equal("Selecionar todas as linhas da pagina", todas.GetAttribute("aria-label"));
        todas.Change(true);

        Assert.Equal(5, recebidos!.Count);
        cortado.Find("button[aria-label='Proxima pagina']").Click();
        Assert.Empty(cortado.FindAll("tr.rvm-marcada"));
        Assert.False(cortado.Find("thead input[type=checkbox]").HasAttribute("checked"));
    }
}
