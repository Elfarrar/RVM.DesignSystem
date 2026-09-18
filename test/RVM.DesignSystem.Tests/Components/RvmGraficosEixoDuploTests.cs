using System.Globalization;
using Microsoft.AspNetCore.Components;
using Bunit;
using RVM.DesignSystem.Components.Chart;

namespace RVM.DesignSystem.Tests.Components;

// Eixo duplo (DSGN-011, fatia 2): duas unidades no mesmo grafico — receita em reais a esquerda, margem
// em porcento a direita. A prova e a serie pequena ocupar a altura toda, e nao virar um risco no chao.
public class RvmGraficosEixoDuploTests : BunitContext
{
    public RvmGraficosEixoDuploTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private record Mes(string Nome, double Receita, double Margem);

    private static readonly Mes[] Dados =
        [new("Jan", 40_000, 12), new("Fev", 60_000, 18), new("Mar", 50_000, 9)];

    private IRenderedComponent<RvmColumnChart<Mes>> Colunas(RvmChartAxis eixoDaMargem = RvmChartAxis.Secondary, bool empilhado = false)
        => Render<RvmColumnChart<Mes>>(p => p
            .Add(x => x.Items, Dados).Add(x => x.Label, m => m.Nome).Add(x => x.AriaLabel, "Receita e margem")
            .Add(x => x.Stacked, empilhado)
            .Add(x => x.SecondaryValueFormat, v => v.ToString("0", CultureInfo.InvariantCulture) + "%")
            .AddChildContent<RvmChartSeries<Mes>>(s => s.Add(x => x.Name, "Receita").Add(x => x.Value, m => m.Receita))
            .AddChildContent<RvmChartSeries<Mes>>(s => s.Add(x => x.Name, "Margem").Add(x => x.Value, m => m.Margem).Add(x => x.Axis, eixoDaMargem)));

    private static List<(double X, string Texto)> Rotulos(IRenderedComponent<IComponent> grafico)
        => [.. grafico.FindAll("text").Select(t => (double.Parse(t.GetAttribute("x")!, CultureInfo.InvariantCulture), t.TextContent))];

    [Fact]
    public void Serie_no_eixo_direito_ganha_escala_rotulos_e_legenda_propria()
    {
        var grafico = Colunas();
        var rotulos = Rotulos(grafico);

        // A direita, no formato do eixo secundario; a esquerda, o compacto de sempre.
        Assert.Contains(rotulos, r => r.Texto == "20%");
        Assert.Contains(rotulos, r => r.Texto == "60 mil");
        var direita = rotulos.Where(r => r.Texto.EndsWith('%')).Select(r => r.X).Min();
        Assert.All(rotulos.Where(r => r.Texto.EndsWith("mil")), r => Assert.True(r.X < direita));

        var legenda = grafico.FindAll(".rvm-grafico-legenda li").Select(li => li.TextContent).ToList();
        Assert.Contains(legenda, t => t.Contains("Receita") && t.Contains("eixo esquerdo"));
        Assert.Contains(legenda, t => t.Contains("Margem") && t.Contains("eixo direito"));

        // A tabela do leitor de tela diz de que eixo o numero veio.
        Assert.Equal(["Categoria", "Receita", "Margem (eixo direito)"],
            grafico.FindAll("table thead th").Select(th => th.TextContent));
    }

    [Fact]
    public void A_serie_pequena_usa_a_altura_toda_em_vez_de_sumir()
    {
        var comDois = Colunas();
        var comUm = Colunas(RvmChartAxis.Primary);

        double AlturaDaMargemEm(IRenderedComponent<RvmColumnChart<Mes>> g)
            // Series na ordem: a segunda coluna de cada grupo e a da margem.
            => g.FindAll("rect.rvm-grafico-barra").Where((_, i) => i % 2 == 1)
                .Select(r => double.Parse(r.GetAttribute("height")!, CultureInfo.InvariantCulture)).Max();

        Assert.True(AlturaDaMargemEm(comDois) > 200, "a margem deve usar a escala propria");
        Assert.True(AlturaDaMargemEm(comUm) < 5, "no eixo unico ela e mesmo um risco no chao");
    }

    [Fact]
    public void Dica_e_empilhamento_seguem_o_eixo_de_cada_serie()
    {
        var grafico = Colunas(empilhado: true);
        grafico.Find(".rvm-grafico-camada").KeyDown(key: "Home");

        var dica = grafico.FindAll(".rvm-grafico-dica-linha").Select(l => l.TextContent).ToList();
        Assert.Contains(dica, t => t.Contains("Receita") && t.Contains("40 mil"));
        Assert.Contains(dica, t => t.Contains("Margem") && t.Contains("12%"));

        // Empilhado com dois eixos: cada lado empilha o seu, entao as duas series partem da base do plot
        // (a margem nao vira um degrau em cima da receita) e a margem usa a altura da escala dela.
        var barras = grafico.FindAll("rect.rvm-grafico-barra")
            .Select(b => (Y: double.Parse(b.GetAttribute("y")!, CultureInfo.InvariantCulture),
                          Altura: double.Parse(b.GetAttribute("height")!, CultureInfo.InvariantCulture)))
            .ToList();
        var base_ = barras.Max(b => b.Y + b.Altura);
        Assert.All(barras, b => Assert.Equal(base_, b.Y + b.Altura, 1));
        Assert.All(barras.Where((_, i) => i % 2 == 1), b => Assert.True(b.Altura > 100));
    }

    [Fact]
    public void Sem_serie_no_eixo_direito_nada_muda()
    {
        var rotulos = Rotulos(Colunas(RvmChartAxis.Primary));

        Assert.DoesNotContain(rotulos, r => r.Texto.EndsWith('%'));
        Assert.DoesNotContain(Colunas(RvmChartAxis.Primary).FindAll(".rvm-grafico-legenda li").Select(li => li.TextContent),
            t => t.Contains("eixo"));
    }

    [Fact]
    public void Todas_no_eixo_direito_continua_sendo_um_eixo_so()
    {
        var grafico = Render<RvmColumnChart<Mes>>(p => p
            .Add(x => x.Items, Dados).Add(x => x.Label, m => m.Nome).Add(x => x.AriaLabel, "So a direita")
            .Add(x => x.SecondaryValueFormat, v => v + "%")
            .AddChildContent<RvmChartSeries<Mes>>(s => s.Add(x => x.Name, "Margem").Add(x => x.Value, m => m.Margem).Add(x => x.Axis, RvmChartAxis.Secondary)));

        // Dois eixos identicos ocupariam as duas bordas para dizer a mesma coisa.
        Assert.DoesNotContain(Rotulos(grafico), r => r.Texto.EndsWith('%'));
        Assert.Contains(Rotulos(grafico), r => r.Texto == "20");
    }

    [Fact]
    public void Nas_barras_o_segundo_eixo_vai_para_CIMA_e_nao_para_a_direita()
    {
        var barras = Render<RvmBarChart<Mes>>(p => p
            .Add(x => x.Items, Dados).Add(x => x.Label, m => m.Nome).Add(x => x.AriaLabel, "Receita e margem")
            .Add(x => x.SecondaryValueFormat, v => v.ToString("0", CultureInfo.InvariantCulture) + "%")
            .AddChildContent<RvmChartSeries<Mes>>(s => s.Add(x => x.Name, "Receita").Add(x => x.Value, m => m.Receita))
            .AddChildContent<RvmChartSeries<Mes>>(s => s.Add(x => x.Name, "Margem").Add(x => x.Value, m => m.Margem).Add(x => x.Axis, RvmChartAxis.Secondary)));

        var rotulos = barras.FindAll("text")
            .Select(t => (Y: double.Parse(t.GetAttribute("y")!, CultureInfo.InvariantCulture), Texto: t.TextContent))
            .ToList();

        // Aqui o eixo de valores e o horizontal: o segundo eixo e uma linha de rotulos no TOPO.
        var emPorcento = rotulos.Where(r => r.Texto.EndsWith('%')).ToList();
        Assert.NotEmpty(emPorcento);
        Assert.All(emPorcento, r => Assert.True(r.Y < 40, "o eixo secundario das barras fica no topo"));
        Assert.All(rotulos.Where(r => r.Texto.EndsWith("mil")), r => Assert.True(r.Y > 250));

        // E a legenda e a tabela chamam os eixos pelo que eles sao nas barras.
        var legenda = barras.FindAll(".rvm-grafico-legenda li").Select(li => li.TextContent).ToList();
        Assert.Contains(legenda, t => t.Contains("Receita") && t.Contains("eixo de baixo"));
        Assert.Contains(legenda, t => t.Contains("Margem") && t.Contains("eixo de cima"));
        Assert.Equal(["Categoria", "Receita", "Margem (eixo de cima)"],
            barras.FindAll("table thead th").Select(th => th.TextContent));

        // A margem de cima abriu espaco para esses rotulos: a barra nao comeca colada neles.
        var topoDaPrimeiraBarra = barras.FindAll("rect.rvm-grafico-barra")
            .Select(r => double.Parse(r.GetAttribute("y")!, CultureInfo.InvariantCulture)).Min();
        Assert.True(topoDaPrimeiraBarra > 16);
    }

    [Fact]
    public void Nas_barras_a_serie_pequena_tambem_usa_o_comprimento_todo()
    {
        var comDois = Render<RvmBarChart<Mes>>(p => p
            .Add(x => x.Items, Dados).Add(x => x.Label, m => m.Nome).Add(x => x.AriaLabel, "Receita e margem")
            .AddChildContent<RvmChartSeries<Mes>>(s => s.Add(x => x.Name, "Receita").Add(x => x.Value, m => m.Receita))
            .AddChildContent<RvmChartSeries<Mes>>(s => s.Add(x => x.Name, "Margem").Add(x => x.Value, m => m.Margem).Add(x => x.Axis, RvmChartAxis.Secondary)));

        var comprimentoDaMargem = comDois.FindAll("rect.rvm-grafico-barra").Where((_, i) => i % 2 == 1)
            .Select(r => double.Parse(r.GetAttribute("width")!, CultureInfo.InvariantCulture)).Max();

        Assert.True(comprimentoDaMargem > 300, "a margem em porcento tem a escala dela");
    }

    [Fact]
    public void Na_linha_cada_serie_desenha_na_escala_do_seu_eixo()
    {
        var linha = Render<RvmLineChart<Mes>>(p => p
            .Add(x => x.Items, Dados).Add(x => x.Label, m => m.Nome).Add(x => x.AriaLabel, "Receita e margem")
            .Add(x => x.ShowPoints, true)
            .Add(x => x.SecondaryValueFormat, v => v + "%")
            .AddChildContent<RvmChartSeries<Mes>>(s => s.Add(x => x.Name, "Receita").Add(x => x.Value, m => m.Receita))
            .AddChildContent<RvmChartSeries<Mes>>(s => s.Add(x => x.Name, "Margem").Add(x => x.Value, m => m.Margem).Add(x => x.Axis, RvmChartAxis.Secondary)));

        var pontos = linha.FindAll("circle.rvm-grafico-ponto")
            .Select(c => double.Parse(c.GetAttribute("cy")!, CultureInfo.InvariantCulture)).ToList();
        // Tres pontos por serie: os da margem (9 a 18) percorrem a altura, nao ficam colados na base.
        Assert.Equal(6, pontos.Count);
        Assert.True(pontos[3..].Max() - pontos[3..].Min() > 100);
    }
}
