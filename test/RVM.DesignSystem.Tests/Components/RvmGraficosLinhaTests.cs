using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using RVM.DesignSystem.Components.Chart;
using RVM.DesignSystem.Components.Select;
using RVM.DesignSystem.Components.Table;

namespace RVM.DesignSystem.Tests.Components;

public class IdsDeComponentesGenericosTests : BunitContext
{
    public IdsDeComponentesGenericosTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    // Campo estatico de classe generica existe um por tipo: dois componentes com TItem/TValue diferentes
    // nasciam com o mesmo id (o degrade de uma area apontava para o de outro grafico).
    [Fact]
    public void Graficos_de_tipos_diferentes_nao_repetem_id()
    {
        var a = Render<RvmAreaChart<Venda>>(p => p.Add(x => x.Items, [new Venda("Jan", 1, 2)]).Add(x => x.AriaLabel, "A")
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "S").Add(x => x.Value, v => v.Receita)));
        var b = Render<RvmAreaChart<string>>(p => p.Add(x => x.Items, ["Jan"]).Add(x => x.AriaLabel, "B")
            .AddChildContent<RvmChartSeries<string>>(s => s.Add(x => x.Name, "S").Add(x => x.Value, v => v.Length)));

        Assert.NotEqual(a.Find("linearGradient").Id, b.Find("linearGradient").Id);
        Assert.NotEqual(a.Find(".rvm-grafico-camada").GetAttribute("aria-describedby"), b.Find(".rvm-grafico-camada").GetAttribute("aria-describedby"));
    }

    [Fact]
    public void Selects_e_tabelas_de_tipos_diferentes_nao_repetem_id()
    {
        var texto = Render<RvmSelect<string>>(p => p.Add(x => x.Items, ["a"]).Add(x => x.Label, "Texto"));
        var numero = Render<RvmSelect<int>>(p => p.Add(x => x.Items, [1]).Add(x => x.Label, "Numero"));
        Assert.NotEqual(texto.Find("[role=combobox]").Id, numero.Find("[role=combobox]").Id);

        var t1 = Render<RvmDataGrid<Venda>>(p => p.Add(x => x.Items, [new Venda("Jan", 1, 2)]).Add(x => x.Caption, "T1")
            .AddChildContent<RvmTableColumn<Venda>>(c => c.Add(x => x.Title, "Mes").Add(x => x.Value, v => v.Mes)));
        var t2 = Render<RvmDataGrid<string>>(p => p.Add(x => x.Items, ["x"]).Add(x => x.Caption, "T2")
            .AddChildContent<RvmTableColumn<string>>(c => c.Add(x => x.Title, "Valor").Add(x => x.Value, v => v)));
        Assert.NotEqual(t1.Find("select").Id, t2.Find("select").Id);
    }
}

public class RvmLineChartTests : BunitContext
{
    private static readonly Venda[] Vendas = [new("Jan", 10, 5), new("Fev", 30, 8), new("Mar", 20, 12), new("Abr", 40, 9)];

    public RvmLineChartTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<RvmLineChart<Venda>> Linha(Action<ComponentParameterCollectionBuilder<RvmLineChart<Venda>>>? extra = null)
        => Render<RvmLineChart<Venda>>(p =>
        {
            p.Add(x => x.Items, Vendas).Add(x => x.Label, v => v.Mes).Add(x => x.AriaLabel, "Vendas")
             .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Receita").Add(x => x.Value, v => v.Receita))
             .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Despesa").Add(x => x.Value, v => v.Despesa));
            extra?.Invoke(p);
        });

    [Fact]
    public void Reta_liga_os_pontos_com_segmentos_e_so_mostra_pontos_no_ativo()
    {
        var cortado = Linha();

        var caminhos = cortado.FindAll("path.rvm-grafico-linha");
        Assert.Equal(2, caminhos.Count);
        Assert.StartsWith("M ", caminhos[0].GetAttribute("d"));
        Assert.Equal(3, caminhos[0].GetAttribute("d")!.Split(" L ").Length - 1);
        Assert.Empty(cortado.FindAll("circle"));
        Assert.Empty(cortado.FindAll("path.rvm-grafico-preenchimento"));

        cortado.Find(".rvm-grafico-camada").KeyDown(key: "ArrowRight");
        Assert.Equal(2, cortado.FindAll("circle.rvm-grafico-ponto").Count);
        Assert.Equal("Jan: Receita 10; Despesa 5", cortado.Find("[aria-live=polite]").TextContent);
        Assert.Contains("rvm-dica-a-direita", cortado.Find(".rvm-grafico-dica").ClassName);
    }

    [Fact]
    public void Suave_usa_cubicas_e_ShowPoints_marca_todos()
    {
        var cortado = Linha(p => p.Add(x => x.Smooth, true).Add(x => x.ShowPoints, true));

        Assert.Contains(" C ", cortado.Find("path.rvm-grafico-linha").GetAttribute("d"));
        Assert.Equal(8, cortado.FindAll("circle.rvm-grafico-ponto").Count);
    }

    [Fact]
    public void Suave_nao_passa_do_maior_valor_entre_vizinhos()
    {
        // Monotona: entre 10 e 30 os pontos de controle ficam dentro da faixa, sem pico inventado.
        var cortado = Linha(p => p.Add(x => x.Smooth, true));
        var d = cortado.Find("path.rvm-grafico-linha").GetAttribute("d")!;
        var ys = d.Replace("M ", "").Replace("C ", "").Split(',', ' ')
            .Where(t => t.Length > 0)
            .Select(t => double.Parse(t, CultureInfo.InvariantCulture))
            .Where((_, i) => i % 2 == 1)
            .ToList();
        // O maior valor (40) fica no topo da grade, em y = 16: nenhum ponto de controle pode subir alem.
        var maisAlto = ys.Min();
        Assert.True(maisAlto >= 15.99, $"Curva subiu acima do maior valor: y={maisAlto}");
    }

    [Fact]
    public void Ponteiro_escolhe_a_categoria_mais_perto()
    {
        var cortado = Linha();

        cortado.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = 575, OffsetY = 50 });
        Assert.Equal("Abr", cortado.Find(".rvm-grafico-dica-titulo").TextContent);
        cortado.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = 1, OffsetY = 50 });
        Assert.Empty(cortado.FindAll(".rvm-grafico-dica"));
    }

    [Fact]
    public void Um_ponto_so_fica_no_meio_e_marcado()
    {
        var cortado = Render<RvmLineChart<Venda>>(p => p.Add(x => x.Items, [new Venda("Jan", 10, 0)]).Add(x => x.AriaLabel, "Um")
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Receita").Add(x => x.Value, v => v.Receita)));

        Assert.Single(cortado.FindAll("circle.rvm-grafico-ponto"));
        cortado.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = 300, OffsetY = 50 });
        Assert.Equal("1", cortado.Find(".rvm-grafico-dica-titulo").TextContent);
    }
}

public class RvmAreaChartTests : BunitContext
{
    public RvmAreaChartTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Area_e_suave_por_padrao_com_degrade_fechado_na_base()
    {
        var cortado = Render<RvmAreaChart<Venda>>(p => p
            .Add(x => x.Items, [new Venda("Jan", 10, 0), new Venda("Fev", 30, 0), new Venda("Mar", 20, 0)])
            .Add(x => x.Label, v => v.Mes)
            .Add(x => x.AriaLabel, "Acumulado")
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Receita").Add(x => x.Value, v => v.Receita).Add(x => x.Color, RvmColor.Success)));

        var preenchimento = cortado.Find("path.rvm-grafico-preenchimento");
        Assert.EndsWith(" Z", preenchimento.GetAttribute("d"));
        Assert.Equal($"url(#{cortado.Find("linearGradient").Id})", preenchimento.GetAttribute("fill"));
        Assert.Contains(" C ", cortado.Find("path.rvm-grafico-linha").GetAttribute("d"));
        Assert.NotNull(cortado.Find("g.rvm-cor-success linearGradient"));
        Assert.Contains("0", cortado.FindAll("text").Select(t => t.TextContent));
    }

    [Fact]
    public void Suave_pode_ser_desligado_e_sem_dados_nao_quebra()
    {
        var reta = Render<RvmAreaChart<Venda>>(p => p.Add(x => x.Items, [new Venda("Jan", 1, 0), new Venda("Fev", 2, 0), new Venda("Mar", 3, 0)])
            .Add(x => x.AriaLabel, "Reta").Add(x => x.Smooth, false)
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Receita").Add(x => x.Value, v => v.Receita)));
        Assert.DoesNotContain(" C ", reta.Find("path.rvm-grafico-linha").GetAttribute("d"));

        var vazio = Render<RvmAreaChart<Venda>>(p => p.Add(x => x.Items, []).Add(x => x.AriaLabel, "Vazio")
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Receita").Add(x => x.Value, v => v.Receita)));
        Assert.Equal("", vazio.Find("path.rvm-grafico-preenchimento").GetAttribute("d"));
        vazio.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = 300, OffsetY = 50 });
        Assert.Empty(vazio.FindAll(".rvm-grafico-dica"));
    }
}

public class RvmScatterChartTests : BunitContext
{
    public RvmScatterChartTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<RvmScatterChart<Venda>> Dispersao(bool comRotulo = true)
        => Render<RvmScatterChart<Venda>>(p =>
        {
            p.Add(x => x.Items, [new Venda("Norte", 300, 62), new Venda("Sede", 100, 40), new Venda("Varzea", 200, 55)])
             .Add(x => x.AriaLabel, "Adubo e produtividade")
             .Add(x => x.XLabel, "Adubo")
             .Add(x => x.YLabel, "Produtividade")
             .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Safra").Add(x => x.X, v => v.Receita).Add(x => x.Value, v => v.Despesa));
            if (comRotulo) p.Add(x => x.Label, v => v.Mes);
        });

    [Fact]
    public void Pontos_ordenados_por_X_no_teclado_e_na_tabela()
    {
        var cortado = Dispersao();

        Assert.Equal(3, cortado.FindAll("circle.rvm-grafico-ponto-cheio").Count);
        Assert.Equal(["Item", "Adubo", "Produtividade"], cortado.FindAll("table thead th").Select(t => t.TextContent));
        Assert.Equal("Sede (Safra)", cortado.FindAll("table tbody th")[0].TextContent);

        cortado.Find(".rvm-grafico-camada").KeyDown(key: "ArrowRight");
        Assert.Equal("Sede (Safra): Adubo 100; Produtividade 40", cortado.Find("[aria-live=polite]").TextContent);
        Assert.Single(cortado.FindAll("circle.rvm-grafico-ponto-ativo"));
    }

    [Fact]
    public void Ponteiro_ativa_o_ponto_mais_perto_ate_24_px()
    {
        var cortado = Dispersao(comRotulo: false);
        var ponto = cortado.FindAll("circle")[2];
        var cx = double.Parse(ponto.GetAttribute("cx")!, CultureInfo.InvariantCulture);
        var cy = double.Parse(ponto.GetAttribute("cy")!, CultureInfo.InvariantCulture);

        cortado.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = cx + 10, OffsetY = cy + 10 });
        Assert.Equal("Safra", cortado.Find(".rvm-grafico-dica-titulo").TextContent);

        cortado.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = cx + 40, OffsetY = cy + 40 });
        Assert.Empty(cortado.FindAll(".rvm-grafico-dica"));
        Assert.Equal("Serie", cortado.Find("table thead th").TextContent);
    }
}
