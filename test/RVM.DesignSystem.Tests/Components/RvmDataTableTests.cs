using Bunit;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Table;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

public sealed record Lavoura(string Nome, string Cultura, decimal Hectares);

/// <summary>RvmDataTable, RvmColumn, RvmTableLayout, RvmRow e RvmHeaderCell do contrato com o RVM.UI (DSGN-017).</summary>
public class RvmDataTableTests : BunitContext
{
    private static readonly Lavoura[] Lavouras =
    [
        new("Talhao Norte", "Soja", 120.5m),
        new("Baixada", "Milho", 64m),
        new("Sede", "Cafe", 18.2m)
    ];

    public RvmDataTableTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    // Talhao (texto, destaque, ordenavel) · Cultura (texto) · Area (ChildContent com <td>, ordenavel, no fim).
    private static RenderFragment Colunas => b =>
    {
        b.OpenComponent<RvmColumn<Lavoura>>(0);
        b.AddComponentParameter(1, nameof(RvmColumn<Lavoura>.Title), "Talhao");
        b.AddComponentParameter(2, nameof(RvmColumn<Lavoura>.Text), (Func<Lavoura, string>)(l => l.Nome));
        b.AddComponentParameter(3, nameof(RvmColumn<Lavoura>.Highlight), true);
        b.AddComponentParameter(4, nameof(RvmColumn<Lavoura>.Sortable), true);
        b.CloseComponent();

        b.OpenComponent<RvmColumn<Lavoura>>(10);
        b.AddComponentParameter(11, nameof(RvmColumn<Lavoura>.Title), "Cultura");
        b.AddComponentParameter(12, nameof(RvmColumn<Lavoura>.Text), (Func<Lavoura, string>)(l => l.Cultura));
        b.CloseComponent();

        b.OpenComponent<RvmColumn<Lavoura>>(20);
        b.AddComponentParameter(21, nameof(RvmColumn<Lavoura>.Title), "Area");
        b.AddComponentParameter(22, nameof(RvmColumn<Lavoura>.Sortable), true);
        b.AddComponentParameter(23, nameof(RvmColumn<Lavoura>.Align), RvmAlign.End);
        b.AddComponentParameter(24, nameof(RvmColumn<Lavoura>.ChildContent), (RenderFragment<Lavoura>)(l => c =>
        {
            c.OpenElement(0, "td");
            c.AddAttribute(1, "class", "celula-area");
            c.AddContent(2, $"{l.Hectares} ha");
            c.CloseElement();
        }));
        b.CloseComponent();
    };

    private IRenderedComponent<RvmDataTable<Lavoura>> Tabela(
        Action<ComponentParameterCollectionBuilder<RvmDataTable<Lavoura>>>? extra = null,
        IReadOnlyList<Lavoura>? itens = null,
        bool semItens = false)
        => Render<RvmDataTable<Lavoura>>(p =>
        {
            p.Add(x => x.Items, semItens ? null : itens ?? Lavouras)
             .Add(x => x.Caption, "Talhoes da safra")
             .Add(x => x.Columns, Colunas);
            extra?.Invoke(p);
        });

    private static string[] Coluna(IRenderedComponent<RvmDataTable<Lavoura>> cortado, int indice)
        => [.. cortado.FindAll("tbody tr").Select(l => l.QuerySelectorAll("td")[indice].TextContent.Trim())];

    [Fact]
    public void E_uma_tabela_nativa_com_legenda_titulos_e_uma_linha_por_item()
    {
        var cortado = Tabela();

        Assert.Equal("Talhoes da safra", cortado.Find("caption").TextContent);
        Assert.Equal(["Talhao", "Cultura", "Area"], cortado.FindAll("thead th[scope=col]").Select(t => t.TextContent.Trim()));
        Assert.Equal(["Talhao Norte", "Baixada", "Sede"], Coluna(cortado, 0));
        Assert.Equal(["Soja", "Milho", "Cafe"], Coluna(cortado, 1));
        Assert.Equal("120.5 ha", cortado.Find("tbody tr td.celula-area").TextContent);
        Assert.Equal("Talhoes da safra", cortado.Find(".rvm-rolagem").GetAttribute("aria-label"));
        Assert.Equal("0", cortado.Find(".rvm-rolagem").GetAttribute("tabindex"));
    }

    [Fact]
    public void Destaque_e_alinhamento_chegam_na_celula_de_texto_e_no_titulo()
    {
        var cortado = Tabela();
        var primeira = cortado.FindAll("tbody tr")[0].QuerySelectorAll("td");
        var titulos = cortado.FindAll("thead th");

        Assert.Contains("rvm-destaque", primeira[0].ClassList);
        Assert.DoesNotContain("rvm-destaque", primeira[1].ClassList);
        Assert.Contains("rvm-inicio", primeira[0].ClassList);
        Assert.Contains("rvm-fim", titulos[2].ClassList);
    }

    [Fact]
    public void Aria_sort_diz_o_sentido_na_ordenada_none_nas_ordenaveis_e_some_nas_outras()
    {
        var cortado = Tabela(p => p.Add(x => x.SortedBy, "Area").Add(x => x.Direction, RvmSortDirection.Descending));
        var titulos = cortado.FindAll("thead th");

        Assert.Equal("none", titulos[0].GetAttribute("aria-sort"));
        Assert.Null(titulos[1].GetAttribute("aria-sort"));
        Assert.Equal("descending", titulos[2].GetAttribute("aria-sort"));
        Assert.Empty(titulos[1].QuerySelectorAll("button"));
        Assert.Contains("rvm-ordenada", titulos[2].QuerySelector("button")!.ClassList);
    }

    [Fact]
    public void Clicar_no_titulo_avisa_OnSort_e_nao_reordena_sozinha()
    {
        (string Coluna, RvmSortDirection Direcao)? pedido = null;
        var cortado = Tabela(p => p.Add(x => x.OnSort, (ValueTuple<string, RvmSortDirection> v) => pedido = v));

        cortado.FindAll("thead th")[0].QuerySelector("button")!.Click();

        Assert.Equal(("Talhao", RvmSortDirection.Ascending), pedido);
        Assert.Equal(["Talhao Norte", "Baixada", "Sede"], Coluna(cortado, 0));
    }

    [Theory]
    [InlineData(RvmSortDirection.Ascending, RvmSortDirection.Descending)]
    [InlineData(RvmSortDirection.Descending, RvmSortDirection.Ascending)]
    public void O_ciclo_alterna_crescente_e_decrescente_na_coluna_ordenada(RvmSortDirection atual, RvmSortDirection esperada)
    {
        (string Coluna, RvmSortDirection Direcao)? pedido = null;
        var cortado = Tabela(p => p
            .Add(x => x.SortedBy, "Area")
            .Add(x => x.Direction, atual)
            .Add(x => x.OnSort, (ValueTuple<string, RvmSortDirection> v) => pedido = v));

        cortado.FindAll("thead th")[2].QuerySelector("button")!.Click();

        Assert.Equal(("Area", esperada), pedido);
    }

    [Fact]
    public void Mudar_SortedBy_e_Items_redesenha_cabecalho_e_linhas()
    {
        var cortado = Tabela(p => p.Add(x => x.SortedBy, "Talhao").Add(x => x.Direction, RvmSortDirection.Ascending));

        cortado.Render(p => p
            .Add(x => x.SortedBy, "Area")
            .Add(x => x.Direction, RvmSortDirection.Ascending)
            .Add(x => x.Items, [.. Lavouras.OrderBy(l => l.Hectares)]));

        var titulos = cortado.FindAll("thead th");
        Assert.Equal("none", titulos[0].GetAttribute("aria-sort"));
        Assert.Equal("ascending", titulos[2].GetAttribute("aria-sort"));
        Assert.Equal(["Sede", "Baixada", "Talhao Norte"], Coluna(cortado, 0));
    }

    [Fact]
    public void Selecao_marca_a_linha_escolhida_com_aria_selected()
    {
        var cortado = Tabela(p => p.Add(x => x.Selectable, true).Add(x => x.IsSelected, l => l.Nome == "Baixada"));
        var linhas = cortado.FindAll("tbody tr");

        Assert.Equal(["false", "true", "false"], linhas.Select(l => l.GetAttribute("aria-selected")));
        Assert.Contains("rvm-linha-escolhida", linhas[1].ClassList);
        Assert.DoesNotContain("rvm-linha-escolhida", linhas[0].ClassList);
    }

    [Fact]
    public void Sem_Selectable_nao_ha_aria_selected_nem_marca()
    {
        var cortado = Tabela(p => p.Add(x => x.IsSelected, _ => true));

        Assert.All(cortado.FindAll("tbody tr"), l =>
        {
            Assert.Null(l.GetAttribute("aria-selected"));
            Assert.DoesNotContain("rvm-linha-escolhida", l.ClassList);
        });
    }

    [Fact]
    public void Lista_vazia_ou_nula_mostra_o_vazio_padrao_ou_o_do_app()
    {
        var nula = Tabela(semItens: true);
        var doApp = Tabela(itens: [], extra: p => p.Add(x => x.Empty, b => b.AddMarkupContent(0, "<p class=\"vazio-app\">Cadastre o primeiro talhao</p>")));

        Assert.Contains("Nada para mostrar aqui ainda.", nula.Find(".rvm-linha-estado").TextContent);
        Assert.Equal("Cadastre o primeiro talhao", doApp.Find(".vazio-app").TextContent);
        Assert.Equal("99", nula.Find(".rvm-linha-estado td").GetAttribute("colspan"));
    }

    [Fact]
    public void Carregando_e_erro_tomam_o_corpo_e_erro_vence()
    {
        var carregando = Tabela(p => p.Add(x => x.Loading, true).Add(x => x.LoadingText, "Buscando talhoes..."));
        var erro = Tabela(p => p.Add(x => x.Loading, true).Add(x => x.Error, true).Add(x => x.ErrorTitle, "Falhou"));

        Assert.Equal("status", carregando.Find(".rvm-estado-carregando").GetAttribute("role"));
        Assert.Contains("Buscando talhoes...", carregando.Find(".rvm-linha-estado").TextContent);
        Assert.Empty(carregando.FindAll("tbody tr.rvm-linha-dados"));
        Assert.Contains("Falhou", erro.Find(".rvm-linha-estado").TextContent);
        Assert.Empty(erro.FindAll(".rvm-estado-carregando"));
        // O cabecalho continua: quem usa leitor de tela sabe que tabela esta carregando.
        Assert.Equal(3, carregando.FindAll("thead th").Count);
    }

    [Fact]
    public void Mascote_do_carregando_e_conteudo_de_erro_do_app()
    {
        var carregando = Tabela(p => p.Add(x => x.Loading, true).Add(x => x.LoadingMascot, RvmMascotName.RobotError));
        var erro = Tabela(p => p.Add(x => x.Error, true).Add(x => x.ErrorContent, b => b.AddMarkupContent(0, "<p class=\"erro-app\">Sem conexao</p>")));

        Assert.NotEmpty(carregando.FindAll(".rvm-estado-carregando svg"));
        Assert.Equal("Sem conexao", erro.Find(".erro-app").TextContent);
    }

    [Fact]
    public void Header_Toolbar_Footer_Class_e_atributos_vao_para_a_casca()
    {
        var cortado = Tabela(p => p
            .Add(x => x.Header, b => b.AddContent(0, "Talhoes"))
            .Add(x => x.Toolbar, b => b.AddContent(0, "Exportar"))
            .Add(x => x.Footer, b => b.AddContent(0, "3 talhoes"))
            .Add(x => x.Class, "minha")
            .AddUnmatched("data-teste", "tabela"));
        var raiz = cortado.Find(".rvm-tabela-montada");

        Assert.Equal("Talhoes", raiz.QuerySelector(".rvm-titulo-tabela")!.TextContent);
        Assert.Equal("Exportar", raiz.QuerySelector(".rvm-acoes-tabela")!.TextContent);
        Assert.Equal("3 talhoes", raiz.QuerySelector(".rvm-rodape-tabela")!.TextContent);
        Assert.Contains("minha", raiz.ClassList);
        Assert.Equal("tabela", raiz.GetAttribute("data-teste"));
    }

    [Fact]
    public void Coluna_fora_da_tabela_e_erro_de_programacao()
    {
        var erro = Assert.Throws<InvalidOperationException>(() =>
            Render<RvmColumn<Lavoura>>(p => p.Add(x => x.Title, "Talhao")));

        Assert.Contains("RvmDataTable", erro.Message);
    }

    [Fact]
    public void Layout_montado_a_mao_com_linha_e_titulo()
    {
        var cortado = Render<RvmTableLayout>(p => p
            .Add(x => x.Caption, "Chuva do mes")
            .Add(x => x.Head, (RenderFragment)(b =>
            {
                b.OpenComponent<RvmHeaderCell>(0);
                b.AddComponentParameter(1, nameof(RvmHeaderCell.ChildContent), (RenderFragment)(c => c.AddContent(0, "Dia")));
                b.CloseComponent();
            }))
            .Add(x => x.Body, (RenderFragment)(b =>
            {
                b.OpenComponent<RvmRow>(0);
                b.AddComponentParameter(1, nameof(RvmRow.Class), "extra");
                b.AddComponentParameter(2, "data-dia", "1");
                b.AddComponentParameter(3, nameof(RvmRow.ChildContent), (RenderFragment)(c => c.AddMarkupContent(0, "<td>12 mm</td>")));
                b.CloseComponent();
            })));

        Assert.Equal("Chuva do mes", cortado.Find("caption").TextContent);
        Assert.Equal("Dia", cortado.Find("thead tr th[scope=col]").TextContent.Trim());
        var linha = cortado.Find("tbody tr");
        Assert.Equal("12 mm", linha.TextContent.Trim());
        Assert.Contains("rvm-linha-dados", linha.ClassList);
        Assert.Contains("extra", linha.ClassList);
        Assert.Equal("1", linha.GetAttribute("data-dia"));
        Assert.Null(linha.GetAttribute("aria-selected"));
    }

    [Fact]
    public void Layout_sem_Head_nao_tem_thead_e_IsEmpty_esconde_o_Body()
    {
        var cortado = Render<RvmTableLayout>(p => p
            .Add(x => x.Caption, "Chuva")
            .Add(x => x.IsEmpty, true)
            .Add(x => x.EmptyText, "Sem chuva registrada")
            .Add(x => x.Body, (RenderFragment)(b => b.AddMarkupContent(0, "<tr class=\"nao-deve\"><td>x</td></tr>"))));

        Assert.Empty(cortado.FindAll("thead"));
        Assert.Empty(cortado.FindAll(".nao-deve"));
        Assert.Contains("Sem chuva registrada", cortado.Markup);
    }

    [Fact]
    public void Linha_selecionavel_diz_false_quando_nao_escolhida()
    {
        var linha = Render<RvmRow>(p => p.Add(x => x.Selectable, true)).Find("tr");

        Assert.Equal("false", linha.GetAttribute("aria-selected"));
    }

    [Fact]
    public void Titulo_ordenavel_e_botao_com_seta_e_avisa_a_direcao_nova()
    {
        RvmSortDirection? nova = null;
        var cortado = Render<RvmHeaderCell>(p => p
            .Add(x => x.Sortable, true)
            .Add(x => x.Align, RvmAlign.Center)
            .Add(x => x.Class, "minha")
            .Add(x => x.DirectionChanged, (RvmSortDirection d) => nova = d)
            .AddChildContent("Area"));
        var th = cortado.Find("th");

        Assert.Equal("col", th.GetAttribute("scope"));
        Assert.Equal("none", th.GetAttribute("aria-sort"));
        Assert.Contains("rvm-centro", th.ClassList);
        Assert.Contains("minha", th.ClassList);
        Assert.Equal("button", cortado.Find("button").GetAttribute("type"));
        Assert.Equal("Area", cortado.Find("button span").TextContent);
        Assert.NotEmpty(cortado.FindAll("button .rvm-seta"));

        cortado.Find("button").Click();
        Assert.Equal(RvmSortDirection.Ascending, nova);
    }

    [Fact]
    public void Titulo_comum_nao_tem_botao_nem_aria_sort()
    {
        var th = Render<RvmHeaderCell>(p => p.AddChildContent("Cultura").AddUnmatched("data-coluna", "cultura")).Find("th");

        Assert.Null(th.GetAttribute("aria-sort"));
        Assert.Empty(th.QuerySelectorAll("button"));
        Assert.Equal("Cultura", th.TextContent.Trim());
        Assert.Equal("cultura", th.GetAttribute("data-coluna"));
        Assert.Contains("rvm-inicio", th.ClassList);
    }

    [Fact]
    public void Itens_iguais_nao_derrubam_a_tabela()
    {
        // Record com igualdade por valor: duas linhas iguais davam "More than one sibling has the same key" com @key.
        var cortado = Tabela(itens: [Lavouras[0], Lavouras[0] with { }]);

        Assert.Equal(2, cortado.FindAll("tbody tr").Count);
    }
}
