using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Chart;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>Rosca, medidor, bolhas e rosca de varias camadas (contrato com o RVM.UI, DSGN-017).</summary>
public class RvmGraficosRadiaisTests : BunitContext
{
    private static readonly RvmChartSlice[] Culturas = [new("Soja", 600), new("Milho", 300), new("Sorgo", 0), new("Feijao", 100)];

    public RvmGraficosRadiaisTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<RvmDonutChart> Rosca(Action<ComponentParameterCollectionBuilder<RvmDonutChart>>? extra = null, RvmChartSlice[]? fatias = null)
        => Render<RvmDonutChart>(p =>
        {
            p.Add(x => x.Label, "Area por cultura").Add(x => x.Slices, fatias ?? Culturas);
            extra?.Invoke(p);
        });

    private static double Numero(string? texto) => double.Parse(texto!, CultureInfo.InvariantCulture);

    [Fact]
    public void Rosca_desenha_uma_fatia_por_valor_positivo_e_a_tabela_traz_todas()
    {
        var cortado = Rosca();

        Assert.Equal(3, cortado.FindAll("path.rvm-radial-fatia").Count);
        Assert.Equal("Area por cultura", cortado.Find("figure").GetAttribute("aria-label"));
        Assert.Equal("true", cortado.Find("svg").GetAttribute("aria-hidden"));
        Assert.Contains("Area por cultura", cortado.Find("table caption").TextContent);
        var linhas = cortado.FindAll("table tbody tr");
        Assert.Equal(4, linhas.Count);
        Assert.Equal(["Soja", "600", "60%"], linhas[0].Children.Select(c => c.TextContent));
        Assert.Equal(["Sorgo", "0", "0%"], linhas[2].Children.Select(c => c.TextContent));
    }

    [Fact]
    public void Total_vira_o_cem_por_cento_e_ganha_linha_na_tabela()
    {
        var cortado = Rosca(p => p.Add(x => x.Total, 12000)
            .Add(x => x.FormatValue, v => $"{v:0} sc"), [new RvmChartSlice("Colhido", 9000)]);

        var linhas = cortado.FindAll("table tbody tr");
        Assert.Equal(["Colhido", "9000 sc", "75%"], linhas[0].Children.Select(c => c.TextContent));
        Assert.Equal(["Total", "12000 sc", "100%"], linhas[1].Children.Select(c => c.TextContent));
        Assert.Single(cortado.FindAll("path.rvm-radial-trilha"));
        Assert.Contains("Colhido: 9000 sc (75%)", cortado.Find(".rvm-radial-alvo").GetAttribute("aria-label"));
    }

    [Fact]
    public void Total_menor_que_a_soma_vale_a_soma()
    {
        var cortado = Rosca(p => p.Add(x => x.Total, 10));

        Assert.Equal("60%", cortado.FindAll("table tbody tr")[0].Children[2].TextContent);
        Assert.Equal("1.000", cortado.FindAll("table tbody tr")[4].Children[1].TextContent);
    }

    [Fact]
    public void Cor_propria_so_em_hexadecimal_senao_cai_na_paleta()
    {
        var cortado = Rosca(fatias:
            [new RvmChartSlice("Em dia", 5, "#2E7D32"), new RvmChartSlice("Atrasada", 3, "red; background: url(x)"), new RvmChartSlice("Curta", 2, "#abc")]);

        var estilos = cortado.FindAll("path.rvm-radial-fatia").Select(f => f.GetAttribute("style")).ToList();
        Assert.Equal("--rvm-radial-cor: #2E7D32", estilos[0]);
        Assert.DoesNotContain("url", estilos[1]);
        Assert.Contains("color-mix(in srgb, var(--rvm-color-primary-text)", estilos[1]);
        Assert.Equal("--rvm-radial-cor: #abc", estilos[2]);
    }

    [Fact]
    public void Monocromatica_desce_em_degraus_do_primary_e_a_colorida_usa_os_papeis()
    {
        var mono = Rosca().FindAll("path.rvm-radial-fatia").Select(f => f.GetAttribute("style")).ToList();
        // A tabela de degraus da RvmChartLegend e da RvmPieChart (4 fatias: 500, 300, 200, 100), para a legenda solta
        // dar a mesma cor; a terceira desenhada e a quarta fatia (o Sorgo, zerado, nao desenha).
        Assert.Equal("--rvm-radial-cor: var(--rvm-color-primary-text)", mono[0]);
        Assert.Contains("var(--rvm-color-primary-text) 26%", mono[2]);

        var cores = Rosca(p => p.Add(x => x.Palette, RvmChartPalette.FullColor))
            .FindAll("path.rvm-radial-fatia").Select(f => f.GetAttribute("style")).ToList();
        Assert.Equal("--rvm-radial-cor: var(--rvm-color-primary-text)", cores[0]);
        Assert.Equal("--rvm-radial-cor: var(--rvm-color-success-text)", cores[1]);
        // A cor segue o indice da fatia (o Sorgo, zerado, fica com o warning sem desenhar).
        Assert.Equal("--rvm-radial-cor: var(--rvm-color-info-text)", cores[2]);
    }

    [Fact]
    public void Meia_rosca_corta_o_quadro_e_tracejada_parte_o_anel()
    {
        var meia = Rosca(p => p.Add(x => x.Shape, RvmDonutShape.Half));
        Assert.Equal("0 0 248 124", meia.Find("svg").GetAttribute("viewBox"));
        Assert.Contains("rvm-rosca-meia", meia.Find("figure").ClassList);

        var tracejada = Rosca(p => p.Add(x => x.Shape, RvmDonutShape.Half).Add(x => x.Dashed, true));
        Assert.Equal(17, tracejada.FindAll("path.rvm-radial-trilha").Count);
        Assert.True(tracejada.FindAll("path.rvm-radial-fatia").Count > 3);
    }

    [Fact]
    public void Espessura_grossa_e_mais_larga_que_a_fina()
    {
        var fina = Numero(Rosca().Find("path.rvm-radial-fatia").GetAttribute("stroke-width"));
        var grossa = Numero(Rosca(p => p.Add(x => x.Thickness, RvmDonutThickness.Thick)).Find("path.rvm-radial-fatia").GetAttribute("stroke-width"));

        Assert.True(grossa > fina * 2);
    }

    [Fact]
    public void Centro_traz_o_valor_e_o_detalhe_escondidos_do_leitor_de_tela()
    {
        var cortado = Rosca(p => p.Add(x => x.CenterValue, "1.240")
            .Add(x => x.CenterDetail, (RenderFragment)(b => b.AddContent(0, "hectares"))));

        var centro = cortado.Find(".rvm-radial-centro");
        Assert.Equal("true", centro.GetAttribute("aria-hidden"));
        Assert.Equal("1.240", cortado.Find(".rvm-rosca-valor").TextContent);
        Assert.Equal("hectares", cortado.Find(".rvm-rosca-detalhe").TextContent);
        Assert.Empty(Rosca().FindAll(".rvm-radial-centro"));
    }

    [Fact]
    public void Dica_por_fatia_focavel_e_some_com_ShowTooltip_desligado()
    {
        var cortado = Rosca();
        var alvos = cortado.FindAll(".rvm-radial-alvo");
        Assert.Equal(3, alvos.Count);
        Assert.All(alvos, a => Assert.Equal("0", a.GetAttribute("tabindex")));
        Assert.Equal("Soja: 600 (60%)", alvos[0].GetAttribute("aria-label"));
        Assert.Equal("Soja", alvos[0].QuerySelector(".rvm-radial-dica-titulo")!.TextContent);

        Assert.Empty(Rosca(p => p.Add(x => x.ShowTooltip, false)).FindAll(".rvm-radial-alvo"));
    }

    [Fact]
    public void Animacao_classe_e_atributos_chegam_a_raiz()
    {
        var cortado = Rosca(p => p.Add(x => x.Class, "meu").AddUnmatched("data-teste", "rosca"));
        var raiz = cortado.Find("figure");
        Assert.Contains("rvm-radial-animado", raiz.ClassList);
        Assert.Contains("meu", raiz.ClassList);
        Assert.Equal("rosca", raiz.GetAttribute("data-teste"));
        Assert.NotNull(cortado.Find("mask circle.rvm-radial-varredura"));

        Assert.DoesNotContain("rvm-radial-animado", Rosca(p => p.Add(x => x.Animated, false)).Find("figure").ClassList);
    }

    [Fact]
    public void Erro_vence_carregando_que_vence_vazio()
    {
        var erro = Rosca(p => p.Add(x => x.Error, true).Add(x => x.Loading, true).Add(x => x.ErrorTitle, "Falhou a busca"));
        Assert.Contains("Falhou a busca", erro.Markup);
        Assert.Empty(erro.FindAll("svg.rvm-radial-svg"));

        var carregando = Rosca(p => p.Add(x => x.Loading, true).Add(x => x.IsEmpty, true).Add(x => x.LoadingText, "Lendo os talhoes"));
        Assert.Contains("Lendo os talhoes", carregando.Find("[role=status]").TextContent);

        var vazio = Rosca(p => p.Add(x => x.IsEmpty, true).Add(x => x.EmptyText, "Sem culturas"));
        Assert.Contains("Sem culturas", vazio.Markup);
        Assert.Equal("Area por cultura", vazio.Find("figure").GetAttribute("aria-label"));
    }

    [Fact]
    public void Fatias_sem_valor_contam_como_vazio_e_o_vazio_proprio_vence_o_texto()
    {
        var cortado = Rosca(p => p
            .Add(x => x.Empty, (RenderFragment)(b => b.AddMarkupContent(0, "<p class=\"meu-vazio\">Plante algo</p>"))), [new RvmChartSlice("Soja", 0)]);

        Assert.NotNull(cortado.Find(".meu-vazio"));
        Assert.Empty(cortado.FindAll("svg.rvm-radial-svg"));
    }

    [Fact]
    public void Mascote_no_erro_e_no_carregando()
    {
        var erro = Rosca(p => p.Add(x => x.Error, true).Add(x => x.ErrorMascot, RvmMascotName.RobotError));
        Assert.NotEmpty(erro.FindAll(".rvm-mascote, [class*=mascote], svg"));

        var conteudo = Rosca(p => p.Add(x => x.Error, true)
            .Add(x => x.ErrorContent, (RenderFragment)(b => b.AddMarkupContent(0, "<p class=\"meu-erro\">Tente de novo</p>"))));
        Assert.NotNull(conteudo.Find(".meu-erro"));
    }

    [Fact]
    public void Medidor_le_o_percentual_do_total_ou_a_soma()
    {
        var comTotal = Render<RvmMeterChart>(p => p.Add(x => x.Label, "Armazem")
            .Add(x => x.Slices, [new RvmChartSlice("Soja", 25), new RvmChartSlice("Milho", 10)]).Add(x => x.Total, 50)
            .Add(x => x.FormatValue, v => $"{v:0} t"));
        Assert.Equal("70%", comTotal.Find(".rvm-medidor-valor").TextContent);
        Assert.Equal("35 t de 50 t", comTotal.Find(".rvm-medidor-detalhe").TextContent);
        Assert.Equal(2, comTotal.FindAll("path.rvm-radial-fatia").Count);
        Assert.Contains("rvm-medidor", comTotal.Find("figure").ClassList);

        var semTotal = Render<RvmMeterChart>(p => p.Add(x => x.Label, "Faturamento")
            .Add(x => x.Slices, [new RvmChartSlice("Julho", 30), new RvmChartSlice("Agosto", 20)]));
        Assert.Equal("50", semTotal.Find(".rvm-medidor-valor").TextContent);
        Assert.Equal("no total", semTotal.Find(".rvm-medidor-detalhe").TextContent);
    }

    [Fact]
    public void Bolhas_tem_area_proporcional_ao_valor_e_o_percentual_dentro()
    {
        var cortado = Render<RvmBubbleChart>(p => p.Add(x => x.Label, "Receita por canal")
            .Add(x => x.Slices, [new RvmChartSlice("Direta", 400), new RvmChartSlice("Cooperativa", 100), new RvmChartSlice("Zerado", 0), new RvmChartSlice("Futuro", 1)]));

        var raios = cortado.FindAll("circle.rvm-radial-bolha").Select(c => Numero(c.GetAttribute("r"))).ToList();
        Assert.Equal(3, raios.Count);
        Assert.Equal(raios[0] / 2, raios[1], 3);
        // A menor tem piso de 30% do diametro maximo para o numero caber.
        Assert.Equal(raios[0] * 0.3, raios[2], 3);
        Assert.Equal(["79,8%", "20%", "0,2%"], cortado.FindAll("text.rvm-radial-rotulo").Select(t => t.TextContent));
        Assert.Equal(3, cortado.FindAll(".rvm-radial-alvo").Count);
    }

    [Fact]
    public void Camadas_enchem_ate_o_valor_sobre_o_maximo()
    {
        var cortado = Render<RvmMultilayerDonutChart>(p => p.Add(x => x.Label, "Colheita por talhao")
            .Add(x => x.Slices, [new RvmChartSlice("T1", 92), new RvmChartSlice("T2", 150), new RvmChartSlice("T3", 0)])
            .Add(x => x.Maximum, 100).Add(x => x.CenterValue, "68%"));

        Assert.Equal(3, cortado.FindAll("circle.rvm-radial-trilha").Count);
        var aneis = cortado.FindAll("circle.rvm-radial-fatia");
        Assert.Equal(2, aneis.Count);
        Assert.Equal("0.92 1", aneis[0].GetAttribute("stroke-dasharray"));
        Assert.Equal("1 1", aneis[1].GetAttribute("stroke-dasharray"));
        Assert.True(Numero(aneis[0].GetAttribute("r")) > Numero(aneis[1].GetAttribute("r")));
        var linhas = cortado.FindAll("table tbody tr");
        Assert.Equal("92%", linhas[0].Children[2].TextContent);
        Assert.Equal(["Maximo", "100", "100%"], linhas[3].Children.Select(c => c.TextContent));
        Assert.Equal("68%", cortado.Find(".rvm-camadas-valor").TextContent);
    }

    [Fact]
    public void Camadas_sem_maximo_usam_o_maior_valor_e_ignoram_o_total()
    {
        var cortado = Render<RvmMultilayerDonutChart>(p => p.Add(x => x.Label, "Clientes")
            .Add(x => x.Slices, [new RvmChartSlice("Novos", 800), new RvmChartSlice("Indicados", 200)]).Add(x => x.Total, 5000));

        Assert.Equal(["1 1", "0.25 1"], cortado.FindAll("circle.rvm-radial-fatia").Select(c => c.GetAttribute("stroke-dasharray")));
        Assert.Equal(2, cortado.FindAll("table tbody tr").Count);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Valor_nao_finito_vale_zero_e_a_rosca_tracejada_termina(double valor)
    {
        var cortado = Rosca(p => p.Add(x => x.Dashed, true),
            [new("Soja", 40), new("Milho", valor)]);

        Assert.DoesNotContain("NaN", cortado.Markup);
        Assert.DoesNotContain("Infinity", cortado.Markup);
    }
}
