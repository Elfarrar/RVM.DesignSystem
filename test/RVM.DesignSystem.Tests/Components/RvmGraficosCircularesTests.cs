using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RVM.DesignSystem.Components.Chart;

namespace RVM.DesignSystem.Tests.Components;

public class RvmPieChartTests : BunitContext
{
    private static readonly Venda[] Culturas = [new("Soja", 600, 0), new("Milho", 300, 0), new("Sorgo", 0, 0), new("Feijao", 100, 0)];

    public RvmPieChartTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<RvmPieChart<Venda>> Pizza(Action<ComponentParameterCollectionBuilder<RvmPieChart<Venda>>>? extra = null)
        => Render<RvmPieChart<Venda>>(p =>
        {
            p.Add(x => x.Items, Culturas).Add(x => x.Label, v => v.Mes).Add(x => x.Value, v => v.Receita).Add(x => x.AriaLabel, "Area por cultura");
            extra?.Invoke(p);
        });

    [Fact]
    public void Uma_fatia_por_valor_positivo_com_percentual_na_legenda_e_na_tabela()
    {
        var cortado = Pizza();

        var fatias = cortado.FindAll("path.rvm-grafico-fatia");
        Assert.Equal(3, fatias.Count);
        Assert.Contains(" A ", fatias[0].GetAttribute("d"));
        Assert.StartsWith("M 300 150 L", fatias[0].GetAttribute("d"));
        Assert.Equal(["60%", "30%", "10%"], cortado.FindAll(".rvm-grafico-legenda strong").Select(s => s.TextContent));
        Assert.Equal(["Sorgo", "0", "0%"], cortado.FindAll("table tbody tr")[2].Children.Select(c => c.TextContent));
    }

    [Fact]
    public void Rosca_tem_anel_e_conteudo_no_centro()
    {
        var cortado = Pizza(p => p.Add(x => x.Donut, true)
            .Add(x => x.CenterContent, (RenderFragment)(b => b.AddMarkupContent(0, "<span>1.000 ha</span>"))));

        Assert.DoesNotContain(" L 300 150", cortado.Find("path.rvm-grafico-fatia").GetAttribute("d"));
        Assert.Equal("1.000 ha", cortado.Find(".rvm-grafico-centro").TextContent);

        var semRosca = Pizza(p => p.Add(x => x.CenterContent, (RenderFragment)(b => b.AddMarkupContent(0, "<span>x</span>"))));
        Assert.Empty(semRosca.FindAll(".rvm-grafico-centro"));
    }

    [Fact]
    public void Teclado_e_ponteiro_ativam_a_fatia_que_se_afasta()
    {
        var cortado = Pizza();
        var dAntes = cortado.FindAll("path.rvm-grafico-fatia")[1].GetAttribute("d");

        cortado.Find(".rvm-grafico-camada").KeyDown(key: "ArrowRight");
        cortado.Find(".rvm-grafico-camada").KeyDown(key: "ArrowRight");
        Assert.Equal("Milho: 300 (30%)", cortado.Find("[aria-live=polite]").TextContent);
        Assert.NotEqual(dAntes, cortado.FindAll("path.rvm-grafico-fatia")[1].GetAttribute("d"));

        // Direita do centro, um pouco abaixo: a fatia da Soja (0 a 216 graus).
        cortado.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = 380, OffsetY = 170 });
        Assert.Equal("Soja", cortado.Find(".rvm-grafico-dica-titulo").TextContent);
        // Fora do circulo.
        cortado.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = 20, OffsetY = 20 });
        Assert.Empty(cortado.FindAll(".rvm-grafico-dica"));
    }

    [Fact]
    public void Cor_por_fatia_uma_fatia_so_e_total_zero()
    {
        var cor = Pizza(p => p.Add(x => x.SliceColor, v => v.Mes == "Soja" ? RvmColor.Error : RvmColor.Info));
        Assert.Single(cor.FindAll("path.rvm-cor-error"));

        var inteira = Render<RvmPieChart<Venda>>(p => p.Add(x => x.Items, [new Venda("Tudo", 5, 0)]).Add(x => x.Value, v => v.Receita).Add(x => x.AriaLabel, "Uma"));
        Assert.Equal(["100%"], inteira.FindAll(".rvm-grafico-legenda strong").Select(s => s.TextContent));

        var zerada = Render<RvmPieChart<Venda>>(p => p.Add(x => x.Items, [new Venda("Nada", 0, 0)]).Add(x => x.Value, v => v.Receita).Add(x => x.AriaLabel, "Zero"));
        Assert.Empty(zerada.FindAll("path.rvm-grafico-fatia"));
        Assert.Single(zerada.FindAll("circle.rvm-grafico-teia"));
        zerada.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = 300, OffsetY = 100 });
        Assert.Empty(zerada.FindAll(".rvm-grafico-dica"));
    }

    [Fact]
    public void Rosca_ignora_o_ponteiro_no_furo_do_meio()
    {
        var cortado = Pizza(p => p.Add(x => x.Donut, true));

        cortado.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = 305, OffsetY = 150 });
        Assert.Empty(cortado.FindAll(".rvm-grafico-dica"));
    }
}

public class RvmRadarChartTests : BunitContext
{
    private static readonly Venda[] Criterios = [new("Solo", 8, 6), new("Agua", 9, 5), new("Custo", 6, 7), new("Logistica", 7, 9)];

    public RvmRadarChartTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<RvmRadarChart<Venda>> Radar(Action<ComponentParameterCollectionBuilder<RvmRadarChart<Venda>>>? extra = null, IEnumerable<Venda>? itens = null)
        => Render<RvmRadarChart<Venda>>(p =>
        {
            p.Add(x => x.Items, itens ?? Criterios).Add(x => x.Label, v => v.Mes).Add(x => x.AriaLabel, "Avaliacao")
             .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Norte").Add(x => x.Value, v => v.Receita))
             .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Varzea").Add(x => x.Value, v => v.Despesa));
            extra?.Invoke(p);
        });

    [Fact]
    public void Grade_em_niveis_um_poligono_por_serie_e_rotulos_dos_eixos()
    {
        var cortado = Radar(p => p.Add(x => x.Max, 10));

        Assert.Equal(5, cortado.FindAll("polygon.rvm-grafico-teia").Count);
        Assert.Equal(4, cortado.FindAll("line.rvm-grafico-teia").Count);
        var poligonos = cortado.FindAll("polygon.rvm-grafico-poligono");
        Assert.Equal(2, poligonos.Count);
        Assert.Equal(4, poligonos[0].GetAttribute("points")!.Split(' ').Length);
        Assert.Equal(["Solo", "Agua", "Custo", "Logistica"], cortado.FindAll("text").Select(t => t.TextContent));
        Assert.Equal(["Eixo", "Norte", "Varzea"], cortado.FindAll("table thead th").Select(t => t.TextContent));
    }

    [Fact]
    public void Eixo_ativo_marca_os_valores_e_anuncia()
    {
        var cortado = Radar(p => p.Add(x => x.GridLevels, 2).Add(x => x.ShowGrid, true));

        cortado.Find(".rvm-grafico-camada").KeyDown(key: "End");
        Assert.Equal("Logistica: Norte 7; Varzea 9", cortado.Find("[aria-live=polite]").TextContent);
        Assert.Equal(2, cortado.FindAll("circle.rvm-grafico-ponto").Count);
        Assert.Single(cortado.FindAll("line.rvm-grafico-eixo-ativo"));

        // Alto do centro: o primeiro eixo (Solo).
        cortado.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = 300, OffsetY = 30 });
        Assert.Equal("Solo", cortado.Find(".rvm-grafico-dica-titulo").TextContent);
        cortado.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = 2, OffsetY = 2 });
        Assert.Empty(cortado.FindAll(".rvm-grafico-dica"));
    }

    [Fact]
    public void Menos_de_tres_eixos_nao_desenha_nem_navega()
    {
        var cortado = Radar(itens: [new Venda("A", 1, 1), new Venda("B", 2, 2)]);

        Assert.Empty(cortado.FindAll("polygon"));
        cortado.Find(".rvm-grafico-camada").KeyDown(key: "ArrowRight");
        Assert.Empty(cortado.FindAll(".rvm-grafico-dica"));
    }
}
