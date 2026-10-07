using System.Globalization;
using Bunit;
using RVM.DesignSystem.Components.Chart;

namespace RVM.DesignSystem.Tests.Components;

// Onda 5 da DSGN-017: legenda solta, Layout do grafico de barras e Palette/Total da pizza.
public class RvmChartLegendTests : BunitContext
{
    private static readonly RvmChartLegendEntry[] Culturas =
        [new("Soja", "60%"), new("Milho", "30%"), new("Feijao")];

    private IRenderedComponent<RvmChartLegend> Legenda(Action<ComponentParameterCollectionBuilder<RvmChartLegend>>? extra = null)
        => Render<RvmChartLegend>(p =>
        {
            p.Add(x => x.Items, Culturas);
            extra?.Invoke(p);
        });

    private static string[] Classes(IRenderedComponent<RvmChartLegend> legenda)
        => [.. legenda.FindAll("li").Select(li => li.ClassName!)];

    [Fact]
    public void Lista_com_marca_escondida_nome_e_valor()
    {
        var legenda = Legenda();

        var itens = legenda.FindAll("ul.rvm-legenda-do-grafico > li");
        Assert.Equal(3, itens.Count);
        Assert.All(itens, li => Assert.Equal("true", li.QuerySelector(".rvm-legenda-do-grafico-marca")!.GetAttribute("aria-hidden")));
        Assert.Equal("Soja60%", itens[0].TextContent.Replace(" ", "").Trim());
        Assert.Equal(["60%", "30%"], legenda.FindAll("strong").Select(s => s.TextContent));
        Assert.Null(itens[2].QuerySelector("strong"));
        Assert.DoesNotContain("empilhada", legenda.Find("ul").ClassName);
    }

    [Fact]
    public void Empilhada_classe_e_atributos_vao_na_raiz()
    {
        var legenda = Legenda(p => p.Add(x => x.Stacked, true).Add(x => x.Class, "minha").AddUnmatched("data-teste", "x"));

        var raiz = legenda.Find("ul");
        Assert.Contains("rvm-legenda-do-grafico-empilhada", raiz.ClassName);
        Assert.Contains("minha", raiz.ClassName);
        Assert.Equal("x", raiz.GetAttribute("data-teste"));
    }

    [Fact]
    public void Itens_nulos_nao_quebram()
    {
        var legenda = Render<RvmChartLegend>(p => p.Add(x => x.Items, null!));
        Assert.Empty(legenda.FindAll("li"));
    }

    [Fact]
    public void Padrao_e_o_radial_em_cores_cheias_igual_a_pizza_padrao()
    {
        Assert.Equal(["rvm-cor-primary", "rvm-cor-success", "rvm-cor-warning"], Classes(Legenda()));
    }

    [Fact]
    public void Monocromatico_segue_os_degraus_pela_quantidade()
    {
        RvmChartLegendEntry[] Entradas(int n) => [.. Enumerable.Range(1, n).Select(i => new RvmChartLegendEntry($"Item {i}"))];
        string[] Mono(int n) => Classes(Render<RvmChartLegend>(p => p
            .Add(x => x.Items, Entradas(n)).Add(x => x.Palette, RvmChartPalette.Monochromatic)));

        Assert.Equal(["rvm-cor-primary"], Mono(1));
        Assert.Equal(["rvm-cor-primary", "rvm-cor-primary-100"], Mono(2));
        Assert.Equal(["rvm-cor-primary", "rvm-cor-primary-400", "rvm-cor-primary-300", "rvm-cor-primary-200", "rvm-cor-primary-100"], Mono(5));
        // Mais fatias do que degraus: volta as cores dos papeis.
        Assert.Equal("rvm-cor-secondary", Mono(6)[5]);
    }

    [Theory]
    [InlineData(RvmLegendRamp.Bar)]
    [InlineData(RvmLegendRamp.Line)]
    public void Barra_e_linha_usam_as_cores_de_serie_e_ignoram_a_paleta(RvmLegendRamp rampa)
    {
        var legenda = Legenda(p => p.Add(x => x.Ramp, rampa).Add(x => x.Palette, RvmChartPalette.Monochromatic));
        Assert.Equal(["rvm-cor-primary", "rvm-cor-success", "rvm-cor-warning"], Classes(legenda));
    }

    [Fact]
    public void Medidor_e_divergente_tem_rampa_propria()
    {
        var medidor = Legenda(p => p.Add(x => x.Ramp, RvmLegendRamp.Meter).Add(x => x.Palette, RvmChartPalette.Monochromatic));
        Assert.Equal(["rvm-cor-primary", "rvm-cor-primary-100", "rvm-cor-primary-300"], Classes(medidor));

        var divergente = Render<RvmChartLegend>(p => p
            .Add(x => x.Items, [new RvmChartLegendEntry("Entradas"), new RvmChartLegendEntry("Saidas")])
            .Add(x => x.Ramp, RvmLegendRamp.Diverging));
        Assert.Equal(["rvm-cor-primary", "rvm-cor-error"], Classes(divergente));
    }
}

public class RvmBarChartLayoutTests : BunitContext
{
    private static readonly Venda[] Fluxo = [new("Jan", 40, 10), new("Fev", 25, 30), new("Mar", 60, 5)];

    public RvmBarChartLayoutTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<RvmBarChart<Venda>> Barras(Action<ComponentParameterCollectionBuilder<RvmBarChart<Venda>>>? extra = null, bool duas = true, IEnumerable<Venda>? itens = null)
        => Render<RvmBarChart<Venda>>(p =>
        {
            p.Add(x => x.Items, itens ?? Fluxo).Add(x => x.Label, v => v.Mes).Add(x => x.AriaLabel, "Fluxo de caixa")
                .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Entradas").Add(x => x.Value, v => v.Receita));
            if (duas)
            {
                p.AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Saidas").Add(x => x.Value, v => v.Despesa));
            }

            extra?.Invoke(p);
        });

    private static double Num(AngleSharp.Dom.IElement e, string atributo) => double.Parse(e.GetAttribute(atributo)!, CultureInfo.InvariantCulture);

    private static string[] Rotulos(IRenderedComponent<RvmBarChart<Venda>> grafico) => [.. grafico.FindAll("text").Select(t => t.TextContent)];

    [Fact]
    public void Padrao_e_o_horizontal_de_sempre()
    {
        Assert.Equal(RvmBarLayout.Horizontal, new RvmBarChart<Venda>().Layout);

        var padrao = Barras();
        var horizontal = Barras(p => p.Add(x => x.Layout, RvmBarLayout.Horizontal));

        Assert.Equal(padrao.FindAll("rect.rvm-grafico-barra").Select(r => r.OuterHtml), horizontal.FindAll("rect.rvm-grafico-barra").Select(r => r.OuterHtml));
        Assert.All(padrao.FindAll("rect.rvm-grafico-barra"), r => Assert.Contains("rvm-grafico-da-esquerda", r.ClassName));
    }

    [Fact]
    public void Vertical_desenha_as_mesmas_colunas_do_column_chart()
    {
        var barras = Barras(p => p.Add(x => x.Layout, RvmBarLayout.Vertical));
        var colunas = Render<RvmColumnChart<Venda>>(p => p
            .Add(x => x.Items, Fluxo).Add(x => x.Label, v => v.Mes).Add(x => x.AriaLabel, "Fluxo de caixa").Add(x => x.MaxBarWidth, 24)
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Entradas").Add(x => x.Value, v => v.Receita))
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Saidas").Add(x => x.Value, v => v.Despesa)));

        string[] Geometria(IEnumerable<AngleSharp.Dom.IElement> rects) => [.. rects.Select(r => $"{r.GetAttribute("x")} {r.GetAttribute("y")} {r.GetAttribute("width")} {r.GetAttribute("height")}")];
        Assert.Equal(Geometria(colunas.FindAll("rect.rvm-grafico-barra")), Geometria(barras.FindAll("rect.rvm-grafico-barra")));
        Assert.All(barras.FindAll("rect.rvm-grafico-barra"), r => Assert.Contains("rvm-grafico-de-baixo", r.ClassName));
        Assert.Contains("Jan", Rotulos(barras));
    }

    [Fact]
    public void Vertical_respeita_ShowAxis_e_tem_zoom()
    {
        var semEixo = Barras(p => p.Add(x => x.Layout, RvmBarLayout.Vertical).Add(x => x.ShowAxis, false));
        Assert.DoesNotContain("60", Rotulos(semEixo));

        var comZoom = Barras(p => p.Add(x => x.Layout, RvmBarLayout.Vertical).Add(x => x.Zoomable, true));
        Assert.NotEmpty(comZoom.FindAll("clipPath"));
    }

    [Fact]
    public void Vertical_com_eixo_secundario_e_empilhado()
    {
        var grafico = Render<RvmBarChart<Venda>>(p => p
            .Add(x => x.Items, Fluxo).Add(x => x.Label, v => v.Mes).Add(x => x.AriaLabel, "Dois eixos")
            .Add(x => x.Layout, RvmBarLayout.Vertical)
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Entradas").Add(x => x.Value, v => v.Receita))
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Saidas").Add(x => x.Value, v => v.Despesa * 1000).Add(x => x.Axis, RvmChartAxis.Secondary)));
        Assert.Contains("Saidas (eixo direito)", grafico.Find("table thead").TextContent);
        Assert.Contains(Rotulos(grafico), t => t.Contains("mil"));

        var empilhado = Barras(p => p.Add(x => x.Layout, RvmBarLayout.Vertical).Add(x => x.Stacked, true));
        var rects = empilhado.FindAll("rect.rvm-grafico-barra");
        Assert.Equal(Num(rects[0], "x"), Num(rects[1], "x"));
        Assert.Equal(Num(rects[0], "y"), Num(rects[1], "y") + Num(rects[1], "height"), 1);
    }

    [Fact]
    public void Divergente_de_duas_series_poe_a_primeira_a_direita_e_a_segunda_a_esquerda()
    {
        var grafico = Barras(p => p.Add(x => x.Layout, RvmBarLayout.Diverging));

        var zero = grafico.Find("line.rvm-grafico-eixo");
        var centro = Num(zero, "x1");
        var rects = grafico.FindAll("rect.rvm-grafico-barra");
        Assert.Equal(6, rects.Count);
        for (var i = 0; i < rects.Count; i += 2)
        {
            Assert.Equal(centro, Num(rects[i], "x"), 1);
            Assert.Equal(centro, Num(rects[i + 1], "x") + Num(rects[i + 1], "width"), 1);
            Assert.Contains("rvm-cor-primary", rects[i].ClassName);
            Assert.Contains("rvm-cor-error", rects[i + 1].ClassName);
            Assert.Contains("rvm-grafico-da-direita", rects[i + 1].ClassName);
        }

        // O eixo e simetrico e mostra o tamanho dos dois lados; a tabela segue com o valor informado.
        Assert.DoesNotContain(Rotulos(grafico), t => t.StartsWith('-'));
        Assert.Equal(2, Rotulos(grafico).Count(t => t == "50"));
        Assert.Equal(["rvm-cor-primary", "rvm-cor-error"], grafico.FindAll(".rvm-grafico-legenda li").Select(li => li.ClassName));
        Assert.Equal(["Jan", "40", "10"], grafico.FindAll("table tbody tr")[0].Children.Select(c => c.TextContent));
    }

    [Fact]
    public void Divergente_respeita_a_cor_da_serie_e_ignora_o_empilhado_e_o_eixo_secundario()
    {
        var grafico = Render<RvmBarChart<Venda>>(p => p
            .Add(x => x.Items, Fluxo).Add(x => x.Label, v => v.Mes).Add(x => x.AriaLabel, "Fluxo")
            .Add(x => x.Layout, RvmBarLayout.Diverging).Add(x => x.Stacked, true)
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Entradas").Add(x => x.Value, v => v.Receita).Add(x => x.Color, RvmColor.Success))
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Saidas").Add(x => x.Value, v => v.Despesa).Add(x => x.Axis, RvmChartAxis.Secondary)));

        var rects = grafico.FindAll("rect.rvm-grafico-barra");
        Assert.Contains("rvm-cor-success", rects[0].ClassName);
        Assert.Contains("rvm-cor-error", rects[1].ClassName);
        // Lado a lado, nao empilhadas: cada uma na sua altura.
        Assert.NotEqual(Num(rects[0], "y"), Num(rects[1], "y"));
        Assert.DoesNotContain("eixo de cima", grafico.Find("table thead").TextContent);
    }

    [Fact]
    public void Divergente_de_uma_serie_vai_pelo_sinal()
    {
        var grafico = Barras(p => p.Add(x => x.Layout, RvmBarLayout.Diverging), duas: false,
            itens: [new Venda("Soja", 30, 0), new Venda("Milho", -12, 0)]);

        var centro = Num(grafico.Find("line.rvm-grafico-eixo"), "x1");
        var rects = grafico.FindAll("rect.rvm-grafico-barra");
        Assert.Equal(centro, Num(rects[0], "x"), 1);
        Assert.Equal(centro, Num(rects[1], "x") + Num(rects[1], "width"), 1);
        Assert.Contains("rvm-cor-primary", rects[1].ClassName);
        Assert.Contains(Rotulos(grafico), t => t.StartsWith('-'));
        // Uma serie so, a legenda embutida nao aparece (como nos outros arranjos).
        Assert.Empty(grafico.FindAll(".rvm-grafico-legenda"));
    }

    [Fact]
    public void Divergente_sem_valor_nao_quebra()
    {
        var grafico = Barras(p => p.Add(x => x.Layout, RvmBarLayout.Diverging), itens: [new Venda("Jan", 0, 0)]);
        Assert.Equal(2, grafico.FindAll("rect.rvm-grafico-barra").Count);
    }
}

public class RvmPieChartPaletaTotalTests : BunitContext
{
    // Sorgo zerado nao desenha fatia, mas conta na rampa (a quantidade e a dos itens, como no RVM.UI).
    private static readonly Venda[] Culturas = [new("Soja", 600, 0), new("Milho", 300, 0), new("Sorgo", 0, 0), new("Feijao", 100, 0)];

    public RvmPieChartPaletaTotalTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<RvmPieChart<Venda>> Pizza(Action<ComponentParameterCollectionBuilder<RvmPieChart<Venda>>>? extra = null)
        => Render<RvmPieChart<Venda>>(p =>
        {
            p.Add(x => x.Items, Culturas).Add(x => x.Label, v => v.Mes).Add(x => x.Value, v => v.Receita).Add(x => x.AriaLabel, "Area por cultura");
            extra?.Invoke(p);
        });

    private static string Cor(AngleSharp.Dom.IElement e) => e.ClassList.Single(c => c.StartsWith("rvm-cor-", StringComparison.Ordinal));

    [Fact]
    public void Sem_paleta_o_desenho_de_sempre_cores_dos_papeis_na_ordem()
    {
        Assert.Equal(RvmChartPalette.FullColor, new RvmPieChart<Venda>().Palette);

        var pizza = Pizza();
        Assert.Equal(["rvm-cor-primary", "rvm-cor-success", "rvm-cor-info"], pizza.FindAll("path.rvm-grafico-fatia").Select(Cor));
    }

    [Fact]
    public void Monocromatico_pinta_fatia_legenda_e_dica_com_o_mesmo_degrau()
    {
        var pizza = Pizza(p => p.Add(x => x.Palette, RvmChartPalette.Monochromatic));

        string[] esperado = ["rvm-cor-primary", "rvm-cor-primary-300", "rvm-cor-primary-100"];
        Assert.Equal(esperado, pizza.FindAll("path.rvm-grafico-fatia").Select(Cor));
        Assert.Equal(esperado, pizza.FindAll(".rvm-grafico-legenda li").Select(li => li.ClassName));

        pizza.Find(".rvm-grafico-camada").KeyDown(key: "ArrowRight");
        pizza.Find(".rvm-grafico-camada").KeyDown(key: "ArrowRight");
        Assert.Contains("rvm-cor-primary-300", pizza.Find(".rvm-grafico-dica-linha").ClassName);

        // A legenda solta com a mesma paleta da as mesmas cores, posicao a posicao.
        var legenda = Render<RvmChartLegend>(p => p
            .Add(x => x.Items, [.. Culturas.Select(c => new RvmChartLegendEntry(c.Mes))])
            .Add(x => x.Palette, RvmChartPalette.Monochromatic));
        Assert.Equal(["rvm-cor-primary", "rvm-cor-primary-300", "rvm-cor-primary-200", "rvm-cor-primary-100"], legenda.FindAll("li").Select(li => li.ClassName));
    }

    [Fact]
    public void SliceColor_vence_a_paleta()
    {
        var pizza = Pizza(p => p.Add(x => x.Palette, RvmChartPalette.Monochromatic).Add(x => x.SliceColor, _ => RvmColor.Warning));
        Assert.All(pizza.FindAll("path.rvm-grafico-fatia"), f => Assert.Equal("rvm-cor-warning", Cor(f)));
    }

    [Fact]
    public void Total_e_o_100_e_o_resto_fica_na_trilha()
    {
        var pizza = Pizza(p => p.Add(x => x.Total, 2000));

        Assert.Equal(["30%", "15%", "5%"], pizza.FindAll(".rvm-grafico-legenda strong").Select(s => s.TextContent));
        Assert.Equal(["Soja", "600", "30%"], pizza.FindAll("table tbody tr")[0].Children.Select(c => c.TextContent));
        Assert.Single(pizza.FindAll("path.rvm-grafico-trilha"));
        Assert.Empty(pizza.FindAll("circle.rvm-grafico-teia"));

        // Na rosca a trilha e um anel: dois circulos, o de dentro furado pelo evenodd.
        var rosca = Pizza(p => p.Add(x => x.Total, 2000).Add(x => x.Donut, true));
        Assert.Equal(2, rosca.Find("path.rvm-grafico-trilha").GetAttribute("d")!.Split('M').Length - 1);

        // Total igual a soma: sem trilha, como sem Total.
        Assert.Empty(Pizza(p => p.Add(x => x.Total, 1000)).FindAll("path.rvm-grafico-trilha"));
    }

    [Fact]
    public void Total_com_tudo_zerado_mostra_so_a_trilha()
    {
        var pizza = Render<RvmPieChart<Venda>>(p => p
            .Add(x => x.Items, [new Venda("Colhido", 0, 0)]).Add(x => x.Value, v => v.Receita)
            .Add(x => x.AriaLabel, "Colheita").Add(x => x.Total, 100));

        Assert.Empty(pizza.FindAll("path.rvm-grafico-fatia"));
        Assert.Single(pizza.FindAll("path.rvm-grafico-trilha"));
    }

    [Fact]
    public void Total_menor_que_a_soma_e_erro()
    {
        var erro = Assert.Throws<ArgumentOutOfRangeException>(() => Pizza(p => p.Add(x => x.Total, 500)));
        Assert.Equal("Total", erro.ParamName);
    }
}
