using Bunit;
using RVM.DesignSystem.Components.Chart;

namespace RVM.DesignSystem.Tests.Components;

// Achados do review independente dos graficos (DSGN-010).
public class RvmGraficosReviewTests : BunitContext
{
    public RvmGraficosReviewTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Linha_usa_a_faixa_dos_dados_e_so_a_area_parte_do_zero()
    {
        Venda[] dados = [new("Jan", 950, 0), new("Fev", 1010, 0), new("Mar", 1050, 0)];

        var linha = Render<RvmLineChart<Venda>>(p => p.Add(x => x.Items, dados).Add(x => x.AriaLabel, "Linha")
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "V").Add(x => x.Value, v => v.Receita)));
        var rotulosDaLinha = linha.FindAll("text").Select(t => t.TextContent).ToList();
        Assert.DoesNotContain("0", rotulosDaLinha);
        // Eixo de 940 a 1.060 de 20 em 20: cada marca com o seu rotulo, sem "1 mil" repetido.
        Assert.Equal(["940", "960", "980", "1 mil", "1,02 mil", "1,04 mil", "1,06 mil"], rotulosDaLinha.Take(7));

        var area = Render<RvmAreaChart<Venda>>(p => p.Add(x => x.Items, dados).Add(x => x.AriaLabel, "Area")
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "V").Add(x => x.Value, v => v.Receita)));
        Assert.Contains("0", area.FindAll("text").Select(t => t.TextContent));
    }

    [Fact]
    public void Base_do_eixo_escreve_zero_e_nao_zero_mil()
    {
        var colunas = Render<RvmColumnChart<Venda>>(p => p
            .Add(x => x.Items, [new Venda("Jan", 20_000, 0), new Venda("Fev", 60_000, 0)])
            .Add(x => x.Label, v => v.Mes).Add(x => x.AriaLabel, "Colunas")
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "V").Add(x => x.Value, v => v.Receita)));
        var rotulos = colunas.FindAll("text").Select(t => t.TextContent).ToList();

        Assert.Contains("0", rotulos);
        Assert.DoesNotContain("0 mil", rotulos);
        Assert.Contains("60 mil", rotulos);
    }

    [Fact]
    public void Zero_nao_sai_com_sinal_nem_com_sufixo()
    {
        // Aproximado, a marca da base cai em -1e-14 em vez de zero cravado (achado na foto do zoom).
        Assert.Equal("0", RvmChartFormat.Compact(-1e-14));
        Assert.Equal("0", RvmChartFormat.Number(-0.0001));
        Assert.Equal("0", RvmChartFormat.Compact(-0.0));
    }

    [Fact]
    public async Task Layout_guardado_e_refeito_quando_os_dados_ou_a_largura_mudam()
    {
        var cortado = Render<RvmColumnChart<Venda>>(p => p.Add(x => x.Items, [new Venda("Jan", 10, 0)]).Add(x => x.AriaLabel, "Colunas")
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "V").Add(x => x.Value, v => v.Receita)));
        Assert.Single(cortado.FindAll("rect.rvm-grafico-barra"));

        // Mover o ponteiro renderiza de novo sem mudar parametros: o layout em cache continua certo.
        cortado.Find(".rvm-grafico-camada").PointerMove(new Microsoft.AspNetCore.Components.Web.PointerEventArgs { OffsetX = 300, OffsetY = 100 });
        Assert.Single(cortado.FindAll("rect.rvm-grafico-barra"));

        cortado.Render(p => p.Add(x => x.Items, [new Venda("Jan", 10, 0), new Venda("Fev", 20, 0)]));
        Assert.Equal(2, cortado.FindAll("rect.rvm-grafico-barra").Count);

        var xAntes = cortado.FindAll("rect.rvm-grafico-barra")[1].GetAttribute("x");
        await cortado.InvokeAsync(() => cortado.Instance.DefinirLargura(300));
        Assert.NotEqual(xAntes, cortado.FindAll("rect.rvm-grafico-barra")[1].GetAttribute("x"));
    }

    [Fact]
    public void Histograma_com_faixa_minuscula_tem_teto_de_faixas()
    {
        var cortado = Render<RvmHistogram>(p => p.Add(x => x.Items, [0d, 1_000_000d]).Add(x => x.AriaLabel, "Teto").Add(x => x.BinWidth, 0.001));

        Assert.Equal(500, cortado.FindAll("rect.rvm-grafico-barra").Count);
        Assert.Equal(2, cortado.FindAll("table tbody td").Sum(t => int.Parse(t.TextContent.Replace(".", ""), System.Globalization.CultureInfo.InvariantCulture)));
    }
}
