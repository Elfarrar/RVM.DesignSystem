using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using RVM.DesignSystem.Components.Chart;

namespace RVM.DesignSystem.Tests.Components;

public sealed record Venda(string Mes, double Receita, double Despesa);

public class RvmChartFormatTests
{
    [Theory]
    [InlineData(0, "0")]
    [InlineData(950, "950")]
    [InlineData(12.5, "12,5")]
    [InlineData(1500, "1,5 mil")]
    [InlineData(60000, "60 mil")]
    [InlineData(-2500, "-2,5 mil")]
    [InlineData(1_240_000, "1,2 mi")]
    [InlineData(3_000_000_000, "3 bi")]
    public void Compacto_em_pt_BR(double valor, string esperado) => Assert.Equal(esperado, RvmChartFormat.Compact(valor));

    [Theory]
    [InlineData(1250.5, "1.250,5")]
    [InlineData(42, "42")]
    [InlineData(1_000_000, "1.000.000")]
    public void Numero_com_milhar(double valor, string esperado) => Assert.Equal(esperado, RvmChartFormat.Number(valor));
}

public class RvmColumnChartTests : BunitContext
{
    private static readonly Venda[] Vendas =
    [
        new("Jan", 28_000, 18_000),
        new("Fev", 34_000, 21_000),
        new("Mar", 31_000, 24_500)
    ];

    public RvmColumnChartTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<RvmColumnChart<Venda>> Grafico(Action<ComponentParameterCollectionBuilder<RvmColumnChart<Venda>>>? extra = null, bool duasSeries = true)
        => Render<RvmColumnChart<Venda>>(p =>
        {
            p.Add(x => x.Items, Vendas)
             .Add(x => x.Label, v => v.Mes)
             .Add(x => x.AriaLabel, "Receita e despesa")
             .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Receita").Add(x => x.Value, v => v.Receita));
            if (duasSeries)
            {
                p.AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Despesa").Add(x => x.Value, v => v.Despesa).Add(x => x.Color, RvmColor.Error));
            }

            extra?.Invoke(p);
        });

    [Fact]
    public void Desenha_uma_coluna_por_item_e_serie_com_eixo_e_grade()
    {
        var cortado = Grafico();

        Assert.Equal(6, cortado.FindAll("rect.rvm-grafico-barra").Count);
        Assert.Equal(3, cortado.FindAll("rect.rvm-cor-primary").Count);
        Assert.Equal(3, cortado.FindAll("rect.rvm-cor-error").Count);
        Assert.Equal("true", cortado.Find("svg").GetAttribute("aria-hidden"));
        // Escala 0 a 40 mil de 10 em 10: cinco linhas de grade.
        Assert.Equal(5, cortado.FindAll("line.rvm-grafico-grade").Count);
        var textos = cortado.FindAll("text").Select(t => t.TextContent).ToList();
        Assert.Contains("40 mil", textos);
        Assert.Contains("Fev", textos);
    }

    [Fact]
    public void Tabela_de_dados_e_legenda_repetem_o_que_o_desenho_mostra()
    {
        var cortado = Grafico();

        var tabela = cortado.Find("table");
        Assert.Equal("Dados do grafico: Receita e despesa", tabela.QuerySelector("caption")!.TextContent);
        Assert.Equal(["Categoria", "Receita", "Despesa"], tabela.QuerySelectorAll("thead th").Select(t => t.TextContent));
        Assert.Equal(["Fev", "34 mil", "21 mil"], tabela.QuerySelectorAll("tbody tr")[1].Children.Select(c => c.TextContent));
        Assert.Equal(["Receita", "Despesa"], cortado.FindAll(".rvm-grafico-legenda li span:not(.rvm-grafico-marca)").Select(s => s.TextContent));

        var camada = cortado.Find(".rvm-grafico-camada");
        Assert.Equal("0", camada.GetAttribute("tabindex"));
        Assert.Equal("Receita e despesa", camada.GetAttribute("aria-label"));
    }

    [Fact]
    public void Uma_serie_so_nao_mostra_legenda_e_ShowGrid_tira_a_grade()
    {
        var cortado = Grafico(p => p.Add(x => x.ShowGrid, false), duasSeries: false);

        Assert.Empty(cortado.FindAll(".rvm-grafico-legenda"));
        Assert.Empty(cortado.FindAll("line.rvm-grafico-grade"));
    }

    [Fact]
    public void Setas_percorrem_as_categorias_e_anunciam_os_valores()
    {
        var cortado = Grafico();
        var anuncio = () => cortado.Find("[aria-live=polite]").TextContent;

        cortado.Find(".rvm-grafico-camada").KeyDown(key: "ArrowRight");
        Assert.Equal("Jan: Receita 28 mil; Despesa 18 mil", anuncio());
        Assert.Equal("Jan", cortado.Find(".rvm-grafico-dica-titulo").TextContent);
        Assert.Single(cortado.FindAll("rect.rvm-grafico-faixa-ativa"));

        cortado.Find(".rvm-grafico-camada").KeyDown(key: "ArrowLeft");
        Assert.StartsWith("Mar:", anuncio());
        cortado.Find(".rvm-grafico-camada").KeyDown(key: "Home");
        Assert.StartsWith("Jan:", anuncio());
        cortado.Find(".rvm-grafico-camada").KeyDown(key: "End");
        Assert.StartsWith("Mar:", anuncio());
        cortado.Find(".rvm-grafico-camada").KeyDown(key: "x");
        Assert.StartsWith("Mar:", anuncio());

        cortado.Find(".rvm-grafico-camada").KeyDown(key: "Escape");
        Assert.Equal("", anuncio());
        Assert.Empty(cortado.FindAll(".rvm-grafico-dica"));
    }

    [Fact]
    public void Ponteiro_ativa_a_categoria_sob_ele_e_sair_limpa()
    {
        var cortado = Grafico();

        // Largura padrao 600: a ultima categoria fica no terco direito do desenho.
        cortado.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = 560, OffsetY = 100 });
        Assert.Equal("Mar", cortado.Find(".rvm-grafico-dica-titulo").TextContent);
        Assert.Contains("rvm-dica-a-esquerda", cortado.Find(".rvm-grafico-dica").ClassName);

        cortado.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = 2, OffsetY = 100 });
        Assert.Empty(cortado.FindAll(".rvm-grafico-dica"));

        cortado.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = 300, OffsetY = 100 });
        cortado.Find(".rvm-grafico-camada").PointerLeave();
        Assert.Empty(cortado.FindAll(".rvm-grafico-dica"));
    }

    [Fact]
    public void Empilhado_soma_as_series_na_mesma_coluna()
    {
        var cortado = Grafico(p => p.Add(x => x.Stacked, true));

        var barras = cortado.FindAll("rect.rvm-grafico-barra");
        // Jan: as duas partes na mesma posicao x, uma em cima da outra.
        Assert.Equal(barras[0].GetAttribute("x"), barras[1].GetAttribute("x"));
        var topoDaPrimeira = double.Parse(barras[0].GetAttribute("y")!, CultureInfo.InvariantCulture);
        var baseDaSegunda = double.Parse(barras[1].GetAttribute("y")!, CultureInfo.InvariantCulture)
                            + double.Parse(barras[1].GetAttribute("height")!, CultureInfo.InvariantCulture);
        Assert.Equal(topoDaPrimeira, baseDaSegunda, 1);
        // Maior soma 55,5 mil: o eixo vai a 60 mil.
        Assert.Contains("60 mil", cortado.FindAll("text").Select(t => t.TextContent));
    }

    [Fact]
    public void Numeros_em_atributo_saem_com_ponto_mesmo_em_pt_BR()
    {
        var antes = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("pt-BR");
        try
        {
            var cortado = Grafico();

            Assert.All(cortado.FindAll("rect.rvm-grafico-barra"), r =>
            {
                Assert.DoesNotContain(",", r.GetAttribute("x"));
                Assert.DoesNotContain(",", r.GetAttribute("height"));
            });
            Assert.DoesNotContain(",", cortado.Find("svg").GetAttribute("viewBox"));
        }
        finally
        {
            CultureInfo.CurrentCulture = antes;
        }
    }

    [Fact]
    public async Task Largura_medida_pelo_JS_redesenha_na_largura_real()
    {
        var cortado = Grafico();
        Assert.Equal("0 0 600 300", cortado.Find("svg").GetAttribute("viewBox"));

        await cortado.InvokeAsync(() => cortado.Instance.DefinirLargura(420));

        Assert.Equal("0 0 420 300", cortado.Find("svg").GetAttribute("viewBox"));
    }

    [Fact]
    public void Formato_proprio_vale_no_eixo_e_na_dica_e_rotulo_escapa_html()
    {
        var cortado = Render<RvmColumnChart<Venda>>(p => p
            .Add(x => x.Items, [new Venda("<b>Jan</b>", 10, 0)])
            .Add(x => x.Label, v => v.Mes)
            .Add(x => x.AriaLabel, "Vendas")
            .Add(x => x.ValueFormat, v => $"R$ {v}")
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Receita").Add(x => x.Value, v => v.Receita)));

        Assert.Contains("R$ 10", cortado.FindAll("text").Select(t => t.TextContent));
        Assert.Contains(cortado.FindAll("text"), t => t.TextContent == "<b>Jan</b>");
        Assert.Empty(cortado.FindAll("svg b"));
    }

    [Fact]
    public void Sem_itens_nao_quebra_e_teclado_nao_faz_nada()
    {
        var cortado = Render<RvmColumnChart<Venda>>(p => p
            .Add(x => x.Items, [])
            .Add(x => x.AriaLabel, "Vazio")
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Receita").Add(x => x.Value, v => v.Receita)));

        Assert.Empty(cortado.FindAll("rect.rvm-grafico-barra"));
        cortado.Find(".rvm-grafico-camada").KeyDown(key: "ArrowRight");
        Assert.Empty(cortado.FindAll(".rvm-grafico-dica"));
    }

    [Fact]
    public void Serie_fora_de_grafico_explica_o_erro()
    {
        var erro = Assert.Throws<InvalidOperationException>(() => Render<RvmChartSeries<Venda>>(p => p.Add(x => x.Name, "Solta")));
        Assert.Contains("grafico", erro.Message);
    }

    [Fact]
    public void Atributos_extras_vao_para_a_figura()
    {
        var cortado = Grafico(p => p.AddUnmatched("class", "meu").AddUnmatched("data-teste", "x"));

        Assert.Equal("rvm-grafico rvm-animado meu", cortado.Find("figure[data-teste=x]").ClassName);
    }
}

public class RvmBarChartTests : BunitContext
{
    public RvmBarChartTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Barras_horizontais_com_nome_longo_cortado_so_no_desenho()
    {
        var nomeLongo = "Talhao do cerrado aberto na safra de dois mil e vinte e seis, perto do corrego";
        var cortado = Render<RvmBarChart<Venda>>(p => p
            .Add(x => x.Items, [new Venda(nomeLongo, 30, 0), new Venda("Sede", 50, 0)])
            .Add(x => x.Label, v => v.Mes)
            .Add(x => x.AriaLabel, "Produtividade")
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Produtividade").Add(x => x.Value, v => v.Receita)));

        var barras = cortado.FindAll("rect.rvm-grafico-barra");
        Assert.Equal(2, barras.Count);
        Assert.True(double.Parse(barras[1].GetAttribute("width")!, CultureInfo.InvariantCulture)
                    > double.Parse(barras[0].GetAttribute("width")!, CultureInfo.InvariantCulture));
        Assert.Contains(cortado.FindAll("text"), t => t.TextContent.EndsWith('…'));
        Assert.Contains(nomeLongo, cortado.Find("table tbody").TextContent);

        cortado.Find(".rvm-grafico-camada").KeyDown(key: "ArrowDown");
        Assert.Equal(nomeLongo, cortado.Find(".rvm-grafico-dica-titulo").TextContent);

        // Ponteiro na segunda metade da altura: a segunda categoria.
        cortado.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = 300, OffsetY = 220 });
        Assert.Equal("Sede", cortado.Find(".rvm-grafico-dica-titulo").TextContent);
        cortado.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = 300, OffsetY = 2 });
        Assert.Empty(cortado.FindAll(".rvm-grafico-dica"));
    }

    [Fact]
    public void Empilhadas_e_negativas_partem_do_zero()
    {
        var cortado = Render<RvmBarChart<Venda>>(p => p
            .Add(x => x.Items, [new Venda("Saldo", 40, -20)])
            .Add(x => x.Label, v => v.Mes)
            .Add(x => x.AriaLabel, "Saldo")
            .Add(x => x.Stacked, true)
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Entradas").Add(x => x.Value, v => v.Receita))
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Saidas").Add(x => x.Value, v => v.Despesa)));

        var barras = cortado.FindAll("rect.rvm-grafico-barra");
        var fimDaNegativa = double.Parse(barras[1].GetAttribute("x")!, CultureInfo.InvariantCulture)
                            + double.Parse(barras[1].GetAttribute("width")!, CultureInfo.InvariantCulture);
        Assert.Equal(double.Parse(barras[0].GetAttribute("x")!, CultureInfo.InvariantCulture), fimDaNegativa, 1);
        Assert.Contains("-20", cortado.FindAll("text").Select(t => t.TextContent));
        Assert.Equal(2, cortado.FindAll(".rvm-grafico-legenda li").Count);
    }
}

public class RvmHistogramTests : BunitContext
{
    public RvmHistogramTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Sturges_define_as_faixas_e_as_contagens_somam_o_total()
    {
        double[] valores = [.. Enumerable.Range(0, 100).Select(i => (double)i)];
        var cortado = Render<RvmHistogram>(p => p.Add(x => x.Items, valores).Add(x => x.AriaLabel, "Distribuicao"));

        // 1 + log2(100) = 7,6 -> 8 faixas.
        Assert.Equal(8, cortado.FindAll("rect.rvm-grafico-barra").Count);
        var contagens = cortado.FindAll("table tbody td").Select(t => int.Parse(t.TextContent, CultureInfo.InvariantCulture));
        Assert.Equal(100, contagens.Sum());
        Assert.Equal(["Faixa", "Frequencia"], cortado.FindAll("table thead th").Select(t => t.TextContent));
        Assert.Empty(cortado.FindAll(".rvm-grafico-legenda"));
    }

    [Fact]
    public void Largura_fixa_e_nomes_proprios_na_dica()
    {
        var cortado = Render<RvmHistogram>(p => p
            .Add(x => x.Items, [12.2, 12.8, 13.1, 14.9, 15.0])
            .Add(x => x.AriaLabel, "Umidade")
            .Add(x => x.BinWidth, 1)
            .Add(x => x.SeriesName, "Cargas")
            .Add(x => x.Color, RvmColor.Info)
            .Add(x => x.BinFormat, v => $"{v}%"));

        // 12 a 16, de 1 em 1: 12-13, 13-14, 14-15, 15-16.
        Assert.Equal(4, cortado.FindAll("rect.rvm-cor-info").Count);
        cortado.Find(".rvm-grafico-camada").KeyDown(key: "Home");
        Assert.Equal("12% a 13%: Cargas 2", cortado.Find("[aria-live=polite]").TextContent);

        cortado.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = 570, OffsetY = 100 });
        Assert.Equal("15% a 16%", cortado.Find(".rvm-grafico-dica-titulo").TextContent);
        cortado.Find(".rvm-grafico-camada").PointerMove(new PointerEventArgs { OffsetX = 1, OffsetY = 100 });
        Assert.Empty(cortado.FindAll(".rvm-grafico-dica"));
    }

    [Fact]
    public void Valores_iguais_ou_nenhum_valor_nao_quebram()
    {
        var iguais = Render<RvmHistogram>(p => p.Add(x => x.Items, [5d, 5d, 5d]).Add(x => x.AriaLabel, "Iguais").Add(x => x.Bins, 3));
        Assert.Equal("3", iguais.FindAll("table tbody td")[0].TextContent);

        var vazio = Render<RvmHistogram>(p => p.Add(x => x.Items, [double.NaN]).Add(x => x.AriaLabel, "Vazio"));
        Assert.Empty(vazio.FindAll("rect.rvm-grafico-barra"));
    }
}
