using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Bunit;
using RVM.DesignSystem.Components.Chart;

namespace RVM.DesignSystem.Tests.Components;

// Zoom e arrasto (DSGN-011, fatia 3). O zoom nao e transformacao do desenho: e a janela do dominio que
// encolhe, entao a prova esta nos rotulos do eixo e na quantidade de categorias escritas.
public class RvmGraficosZoomTests : BunitContext
{
    public RvmGraficosZoomTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private record Dia(string Nome, double Valor);

    private static readonly Dia[] Dados =
        [.. Enumerable.Range(1, 20).Select(i => new Dia($"Dia {i}", 100 + i * 10))];

    private IRenderedComponent<RvmColumnChart<Dia>> Colunas(bool zoom = true)
        => Render<RvmColumnChart<Dia>>(p => p
            .Add(x => x.Items, Dados).Add(x => x.Label, d => d.Nome).Add(x => x.AriaLabel, "Movimento por dia")
            .Add(x => x.Zoomable, zoom)
            .AddChildContent<RvmChartSeries<Dia>>(s => s.Add(x => x.Name, "Valor").Add(x => x.Value, d => d.Valor)));

    private static List<string> Categorias(IRenderedComponent<RvmColumnChart<Dia>> g)
        => [.. g.FindAll("text").Select(t => t.TextContent).Where(t => t.StartsWith("Dia ", StringComparison.Ordinal))];

    private static List<double> MarcasDoEixo(IRenderedComponent<RvmColumnChart<Dia>> g)
        => [.. g.FindAll("text").Select(t => t.TextContent)
                .Where(t => double.TryParse(t, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                .Select(t => double.Parse(t, CultureInfo.InvariantCulture))];

    [Fact]
    public async Task A_roda_aproxima_o_eixo_horizontal_em_volta_do_ponteiro()
    {
        var grafico = Colunas();
        var antes = Categorias(grafico);

        // Roda para cima no meio do desenho.
        await grafico.InvokeAsync(() => grafico.Instance.RodarNoGrafico(-100, 300, 150, shift: false));

        var depois = Categorias(grafico);
        Assert.True(depois.Count < antes.Count, "aproximado, cabem menos dias na mesma largura");
        Assert.Contains(depois, d => antes.Contains(d));
        // O que sobrou esta no miolo, nao nas pontas.
        Assert.DoesNotContain("Dia 1", depois);
        Assert.DoesNotContain("Dia 20", depois);
    }

    [Fact]
    public async Task Com_shift_a_roda_aproxima_o_eixo_vertical()
    {
        var grafico = Colunas();
        var antes = MarcasDoEixo(grafico);

        await grafico.InvokeAsync(() => grafico.Instance.RodarNoGrafico(-100, 300, 150, shift: true));

        var depois = MarcasDoEixo(grafico);
        // A faixa de valores mostrada encolheu: a distancia entre a primeira e a ultima marca e menor.
        Assert.True(depois.Max() - depois.Min() < antes.Max() - antes.Min());
        Assert.Equal(Categorias(grafico).Count, Categorias(Colunas()).Count);
    }

    [Fact]
    public async Task Duplo_clique_e_a_tecla_zero_voltam_ao_grafico_inteiro()
    {
        var grafico = Colunas();
        var inteiro = Categorias(grafico);

        await grafico.InvokeAsync(() => grafico.Instance.RodarNoGrafico(-100, 300, 150, shift: false));
        Assert.NotEqual(inteiro, Categorias(grafico));
        grafico.Find(".rvm-grafico-camada").DoubleClick();
        Assert.Equal(inteiro, Categorias(grafico));

        await grafico.InvokeAsync(() => grafico.Instance.RodarNoGrafico(-100, 300, 150, shift: false));
        grafico.Find(".rvm-grafico-camada").KeyDown(key: "0");
        Assert.Equal(inteiro, Categorias(grafico));
    }

    [Fact]
    public void Mais_menos_e_ctrl_com_setas_fazem_o_zoom_pelo_teclado()
    {
        var grafico = Colunas();
        var camada = grafico.Find(".rvm-grafico-camada");
        var inteiro = Categorias(grafico);

        camada.KeyDown(key: "+");
        var aproximado = Categorias(grafico);
        Assert.True(aproximado.Count < inteiro.Count);

        camada.KeyDown(new KeyboardEventArgs { Key = "ArrowRight", CtrlKey = true });
        Assert.NotEqual(aproximado, Categorias(grafico));
        // Ctrl nao mexe no ponto ativo: a leitura ponto a ponto continua nas setas sozinhas.
        Assert.Empty(grafico.FindAll(".rvm-grafico-dica-titulo"));

        camada.KeyDown(key: "-");
        camada.KeyDown(key: "-");
        Assert.Equal(inteiro, Categorias(grafico));
    }

    [Fact]
    public async Task O_leitor_de_tela_ouve_a_janela_e_a_instrucao()
    {
        var grafico = Colunas();
        Assert.Contains("zero volta ao grafico inteiro", grafico.Find("span[id$=instrucao]").TextContent);

        await grafico.InvokeAsync(() => grafico.Instance.RodarNoGrafico(-100, 300, 150, shift: false));

        var avisos = grafico.FindAll("span[aria-live=polite]").Select(s => s.TextContent).ToList();
        Assert.Contains(avisos, a => a.Contains("Mostrando de") && a.Contains("do eixo horizontal"));

        grafico.Find(".rvm-grafico-camada").KeyDown(key: "0");
        Assert.Contains(grafico.FindAll("span[aria-live=polite]").Select(s => s.TextContent),
            a => a.Contains("Grafico inteiro a vista"));
    }

    [Fact]
    public async Task Arrastar_desloca_a_janela_so_depois_de_aproximar()
    {
        var grafico = Colunas();
        var camada = grafico.Find(".rvm-grafico-camada");

        // Sem zoom, arrastar nao faz nada (e o grafico nao vira uma area arrastavel).
        Assert.DoesNotContain("rvm-grafico-arrastavel", camada.ClassName);
        camada.PointerDown(new() { OffsetX = 300, OffsetY = 150 });
        camada.PointerMove(new() { OffsetX = 200, OffsetY = 150 });
        Assert.Contains("Dia 1", Categorias(grafico));

        await grafico.InvokeAsync(() => grafico.Instance.RodarNoGrafico(-100, 300, 150, shift: false));
        await grafico.InvokeAsync(() => grafico.Instance.RodarNoGrafico(-100, 300, 150, shift: false));
        var aproximado = Categorias(grafico);
        Assert.Contains("rvm-grafico-arrastavel", grafico.Find(".rvm-grafico-camada").ClassName);

        camada = grafico.Find(".rvm-grafico-camada");
        camada.PointerDown(new() { OffsetX = 400, OffsetY = 150 });
        camada.PointerMove(new() { OffsetX = 200, OffsetY = 150 });
        // Arrastar para a esquerda leva a janela para frente no tempo.
        var depois = Categorias(grafico);
        Assert.NotEqual(aproximado, depois);
        Assert.True(Numero(depois[0]) > Numero(aproximado[0]));

        camada.PointerUp();
        var parado = Categorias(grafico);
        camada.PointerMove(new() { OffsetX = 500, OffsetY = 150 });
        Assert.Equal(parado, Categorias(grafico));
    }

    [Fact]
    public async Task Sem_Zoomable_a_roda_e_as_teclas_nao_mexem_no_grafico()
    {
        var grafico = Colunas(zoom: false);
        var inteiro = Categorias(grafico);

        await grafico.InvokeAsync(() => grafico.Instance.RodarNoGrafico(-100, 300, 150, shift: false));
        grafico.Find(".rvm-grafico-camada").KeyDown(key: "+");

        Assert.Equal(inteiro, Categorias(grafico));
        // Sem zoom, o "+" tambem nao rouba a instrucao do leitor de tela.
        Assert.DoesNotContain("zero volta", grafico.Find("span[id$=instrucao]").TextContent);
    }

    [Fact]
    public async Task Na_linha_o_ponto_sob_o_ponteiro_acompanha_o_zoom()
    {
        var linha = Render<RvmLineChart<Dia>>(p => p
            .Add(x => x.Items, Dados).Add(x => x.Label, d => d.Nome).Add(x => x.AriaLabel, "Movimento por dia")
            .Add(x => x.Zoomable, true)
            .AddChildContent<RvmChartSeries<Dia>>(s => s.Add(x => x.Name, "Valor").Add(x => x.Value, d => d.Valor)));

        linha.Find(".rvm-grafico-camada").PointerMove(new() { OffsetX = 300, OffsetY = 150 });
        var noMeio = linha.Find(".rvm-grafico-dica-titulo").TextContent;

        await linha.InvokeAsync(() => linha.Instance.RodarNoGrafico(-100, 300, 150, shift: false));
        linha.Find(".rvm-grafico-camada").PointerMove(new() { OffsetX = 300, OffsetY = 150 });

        // O ponteiro nao saiu do lugar, e o zoom foi centrado nele: continua o mesmo dia.
        Assert.Equal(noMeio, linha.Find(".rvm-grafico-dica-titulo").TextContent);
    }

    // --- Zoom por caixa desenhada (DSGN-013) ---

    private IRenderedComponent<RvmColumnChart<Dia>> ComCaixa()
        => Render<RvmColumnChart<Dia>>(p => p
            .Add(x => x.Items, Dados).Add(x => x.Label, d => d.Nome).Add(x => x.AriaLabel, "Movimento por dia")
            .Add(x => x.SelectionMode, RvmChartSelectionMode.ZoomBox)
            .AddChildContent<RvmChartSeries<Dia>>(s => s.Add(x => x.Name, "Valor").Add(x => x.Value, d => d.Valor)));

    [Fact]
    public void A_caixa_desenhada_aproxima_nos_dois_eixos()
    {
        var grafico = ComCaixa();
        var camada = grafico.Find(".rvm-grafico-camada");
        var inteiro = Categorias(grafico);
        var marcasInteiras = MarcasDoEixo(grafico);

        camada.PointerDown(new PointerEventArgs { OffsetX = 150, OffsetY = 80 });
        camada.PointerMove(new PointerEventArgs { OffsetX = 400, OffsetY = 200 });
        // Enquanto o botao esta apertado, a caixa aparece.
        Assert.Single(grafico.FindAll(".rvm-grafico-caixa-de-zoom"));
        camada.PointerUp(new PointerEventArgs { OffsetX = 400, OffsetY = 200 });

        Assert.Empty(grafico.FindAll(".rvm-grafico-caixa-de-zoom"));
        var depois = Categorias(grafico);
        var marcas = MarcasDoEixo(grafico);
        Assert.True(depois.Count < inteiro.Count, "a caixa aproxima o eixo horizontal");
        Assert.True(marcas.Max() - marcas.Min() < marcasInteiras.Max() - marcasInteiras.Min(), "e o vertical tambem");

        // E ha caminho de volta: o duplo clique e as teclas valem mesmo sem Zoomable.
        camada.DoubleClick();
        Assert.Equal(inteiro, Categorias(grafico));
    }

    [Fact]
    public void Caixa_pequena_demais_e_clique_e_nao_aproxima()
    {
        var grafico = ComCaixa();
        var camada = grafico.Find(".rvm-grafico-camada");
        var inteiro = Categorias(grafico);

        camada.PointerDown(new PointerEventArgs { OffsetX = 300, OffsetY = 150 });
        camada.PointerMove(new PointerEventArgs { OffsetX = 303, OffsetY = 152 });
        camada.PointerUp(new PointerEventArgs { OffsetX = 303, OffsetY = 152 });

        Assert.Equal(inteiro, Categorias(grafico));
    }

    [Fact]
    public void Com_shift_o_arrasto_desloca_em_vez_de_desenhar_a_caixa()
    {
        var grafico = ComCaixa();
        var camada = grafico.Find(".rvm-grafico-camada");

        // Primeiro aproxima pela caixa, senao nao ha o que deslocar.
        camada.PointerDown(new PointerEventArgs { OffsetX = 150, OffsetY = 80 });
        camada.PointerMove(new PointerEventArgs { OffsetX = 400, OffsetY = 220 });
        camada.PointerUp(new PointerEventArgs { OffsetX = 400, OffsetY = 220 });
        var aproximado = Categorias(grafico);

        camada = grafico.Find(".rvm-grafico-camada");
        camada.PointerDown(new PointerEventArgs { OffsetX = 400, OffsetY = 150, ShiftKey = true });
        camada.PointerMove(new PointerEventArgs { OffsetX = 200, OffsetY = 150, ShiftKey = true });
        camada.PointerUp(new PointerEventArgs { OffsetX = 200, OffsetY = 150, ShiftKey = true });

        Assert.Empty(grafico.FindAll(".rvm-grafico-caixa-de-zoom"));
        Assert.NotEqual(aproximado, Categorias(grafico));
    }

    private static int Numero(string categoria) => int.Parse(categoria["Dia ".Length..], CultureInfo.InvariantCulture);
}
