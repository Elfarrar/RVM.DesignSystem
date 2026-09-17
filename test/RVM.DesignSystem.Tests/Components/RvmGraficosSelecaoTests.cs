using Bunit;
using Microsoft.AspNetCore.Components.Web;
using RVM.DesignSystem.Components.Chart;

namespace RVM.DesignSystem.Tests.Components;

// Selecao de faixa (DSGN-011, fatia 4): o grafico marca um intervalo de categorias e quem ouve filtra o
// resto da tela. A faixa e do consumidor — o grafico so avisa qual e.
public class RvmGraficosSelecaoTests : BunitContext
{
    public RvmGraficosSelecaoTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private record Mes(string Nome, double Valor);

    private static readonly Mes[] Dados =
        [.. new[] { "Jan", "Fev", "Mar", "Abr", "Mai", "Jun" }.Select((m, i) => new Mes(m, 100 + i * 20))];

    private RvmChartRange? _ultima;
    private int _avisos;

    private IRenderedComponent<RvmColumnChart<Mes>> Colunas(
        RvmChartSelectionMode modo = RvmChartSelectionMode.Range, RvmChartRange? inicial = null, bool zoom = false)
        => Render<RvmColumnChart<Mes>>(p => p
            .Add(x => x.Items, Dados).Add(x => x.Label, m => m.Nome).Add(x => x.AriaLabel, "Movimento por mes")
            .Add(x => x.SelectionMode, modo)
            .Add(x => x.Selection, inicial)
            .Add(x => x.Zoomable, zoom)
            .Add(x => x.SelectionChanged, f => { _ultima = f; _avisos++; })
            .AddChildContent<RvmChartSeries<Mes>>(s => s.Add(x => x.Name, "Valor").Add(x => x.Value, m => m.Valor)));

    // A largura padrao do desenho e 600 px e sao seis meses: cada faixa tem ~95 px depois dos rotulos.
    private static double XDoMes(int indice) => 40 + 95 * (indice + 0.5);

    private static void Arrastar(IRenderedComponent<RvmColumnChart<Mes>> g, int de, int ate)
    {
        var camada = g.Find(".rvm-grafico-camada");
        camada.PointerDown(new PointerEventArgs { OffsetX = XDoMes(de), OffsetY = 150 });
        camada.PointerMove(new PointerEventArgs { OffsetX = XDoMes(ate), OffsetY = 150 });
        camada.PointerUp(new PointerEventArgs { OffsetX = XDoMes(ate), OffsetY = 150 });
    }

    [Fact]
    public void Arrastar_marca_a_faixa_e_avisa_quem_ouve()
    {
        var grafico = Colunas();

        Arrastar(grafico, 1, 3);

        Assert.Equal(new RvmChartRange(1, 3), _ultima);
        Assert.Equal(3, _ultima!.Count);
        Assert.True(_ultima.Contains(2));
        var faixa = grafico.Find(".rvm-grafico-faixa-marcada").GetAttribute("style")!;
        Assert.Matches(@"left: 2[0-9](,|\.)?\d*%", faixa);
        Assert.Matches(@"width: 4[0-9](,|\.)?\d*%", faixa);
    }

    [Fact]
    public void Arrastar_de_tras_para_frente_da_na_mesma_faixa()
    {
        var grafico = Colunas();

        Arrastar(grafico, 4, 2);

        Assert.Equal(new RvmChartRange(2, 4), _ultima);
    }

    [Fact]
    public void Clicar_sem_arrastar_limpa_a_marcacao()
    {
        var grafico = Colunas();
        Arrastar(grafico, 1, 3);

        // Clicar sem arrastar limpa (para marcar um mes so, ha o Enter no teclado).
        Arrastar(grafico, 5, 5);
        Assert.Null(_ultima);
        Assert.Empty(grafico.FindAll(".rvm-grafico-faixa-marcada"));
    }

    [Fact]
    public void Shift_com_as_setas_marca_pelo_teclado_e_Esc_limpa()
    {
        var grafico = Colunas();
        var camada = grafico.Find(".rvm-grafico-camada");

        camada.KeyDown(key: "Home");
        camada.KeyDown(new KeyboardEventArgs { Key = "ArrowRight", ShiftKey = true });
        camada.KeyDown(new KeyboardEventArgs { Key = "ArrowRight", ShiftKey = true });
        Assert.Equal(new RvmChartRange(0, 2), _ultima);

        camada.KeyDown(new KeyboardEventArgs { Key = "ArrowLeft", ShiftKey = true });
        Assert.Equal(new RvmChartRange(0, 1), _ultima);

        camada.KeyDown(key: "Escape");
        Assert.Null(_ultima);

        // Enter marca so o ponto que esta sendo lido.
        camada.KeyDown(key: "End");
        camada.KeyDown(key: "Enter");
        Assert.Equal(new RvmChartRange(5, 5), _ultima);
    }

    [Fact]
    public void O_leitor_de_tela_ouve_a_faixa_e_a_instrucao()
    {
        var grafico = Colunas();
        Assert.Contains("Shift com as setas marca uma faixa", grafico.Find("span[id$=instrucao]").TextContent);

        Arrastar(grafico, 1, 3);
        Assert.Contains(grafico.FindAll("span[aria-live=polite]").Select(s => s.TextContent),
            a => a == "Selecionado de Fev a Abr: 3 de 6.");

        grafico.Find(".rvm-grafico-camada").KeyDown(key: "Escape");
        Assert.Contains(grafico.FindAll("span[aria-live=polite]").Select(s => s.TextContent), a => a == "Selecao limpa.");
    }

    [Fact]
    public void A_faixa_de_fora_e_adotada_e_o_render_do_pai_nao_apaga_a_marcada()
    {
        var grafico = Colunas(inicial: new RvmChartRange(0, 1));
        Assert.Single(grafico.FindAll(".rvm-grafico-faixa-marcada"));

        Arrastar(grafico, 3, 4);
        Assert.Equal(new RvmChartRange(3, 4), _ultima);

        // O pai renderiza de novo com a MESMA instancia de Selection: a marcacao nova continua de pe
        // (a armadilha que ja tinha mordido a selecao da RvmTable).
        grafico.Render();
        Assert.Equal(new RvmChartRange(3, 4), _ultima);
        Assert.Single(grafico.FindAll(".rvm-grafico-faixa-marcada"));

        grafico.Render(p => p.Add(x => x.Selection, new RvmChartRange(5, 5)));
        Assert.Contains(grafico.FindAll("span[aria-live=polite]").Select(s => s.TextContent), a => a.Contains("Jun"));
    }

    [Fact]
    public void Sem_SelectionMode_o_arrasto_nao_marca_nada()
    {
        var grafico = Colunas(RvmChartSelectionMode.None);

        Arrastar(grafico, 1, 3);

        Assert.Null(_ultima);
        Assert.Equal(0, _avisos);
        Assert.Empty(grafico.FindAll(".rvm-grafico-faixa-marcada"));
        Assert.DoesNotContain("Shift com as setas", grafico.Find("span[id$=instrucao]").TextContent);
    }

    [Fact]
    public async Task Com_zoom_e_selecao_juntos_o_arrasto_marca_e_shift_arrasta_desloca()
    {
        var grafico = Colunas(zoom: true);
        await grafico.InvokeAsync(() => grafico.Instance.RodarNoGrafico(-100, 300, 150, false));
        var antes = grafico.FindAll("text").Select(t => t.TextContent).ToList();

        var camada = grafico.Find(".rvm-grafico-camada");
        camada.PointerDown(new PointerEventArgs { OffsetX = 300, OffsetY = 150, ShiftKey = true });
        camada.PointerMove(new PointerEventArgs { OffsetX = 200, OffsetY = 150, ShiftKey = true });
        camada.PointerUp(new PointerEventArgs { OffsetX = 200, OffsetY = 150, ShiftKey = true });

        // Shift+arrastar deslocou em vez de marcar.
        Assert.Null(_ultima);
        Assert.NotEqual(antes, grafico.FindAll("text").Select(t => t.TextContent).ToList());
    }
}
