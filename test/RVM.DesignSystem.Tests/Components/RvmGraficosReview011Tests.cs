using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using RVM.DesignSystem.Components.Chart;

namespace RVM.DesignSystem.Tests.Components;

// Achados do review independente da DSGN-011 (17/09/2026).
public class RvmGraficosReview011Tests : BunitContext
{
    public RvmGraficosReview011Tests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private record Mes(string Nome, double Valor);

    private static readonly Mes[] Dados =
        [.. new[] { "Jan", "Fev", "Mar", "Abr", "Mai", "Jun" }.Select((m, i) => new Mes(m, 100 + i * 20))];

    private RvmChartRange? _faixa;

    private IRenderedComponent<RvmColumnChart<Mes>> Colunas()
        => Render<RvmColumnChart<Mes>>(p => p
            .Add(x => x.Items, Dados).Add(x => x.Label, m => m.Nome).Add(x => x.AriaLabel, "Movimento por mes")
            .Add(x => x.SelectionMode, RvmChartSelectionMode.Range)
            .Add(x => x.SelectionChanged, f => _faixa = f)
            .AddChildContent<RvmChartSeries<Mes>>(s => s.Add(x => x.Name, "Valor").Add(x => x.Value, m => m.Valor)));

    [Fact]
    public void Arrastar_ate_passar_da_borda_estende_a_faixa_em_vez_de_colapsar()
    {
        var grafico = Colunas();
        var camada = grafico.Find(".rvm-grafico-camada");

        // A camada de eventos cobre a figura inteira: soltar alguns pixels depois do desenho (a margem
        // direita) devolvia "nenhum ponto" e a faixa voltava para o mes em que o arrasto comecou.
        camada.PointerDown(new PointerEventArgs { OffsetX = 135, OffsetY = 150 });
        camada.PointerMove(new PointerEventArgs { OffsetX = 598, OffsetY = 150 });
        camada.PointerUp(new PointerEventArgs { OffsetX = 598, OffsetY = 150 });

        Assert.Equal(new RvmChartRange(1, 5), _faixa);
    }

    [Fact]
    public async Task Pdf_de_um_grafico_sem_altura_devolve_falso_em_vez_de_estourar()
    {
        var modulo = JSInterop.SetupModule("./_content/RVM.DesignSystem/rvm-grafico.js");
        modulo.Setup<string>("paraImagem", _ => true).SetResult(Convert.ToBase64String([0xFF, 0xD8, 0xFF, 0xD9]));
        // O navegador devolve altura zero enquanto o grafico nao tem layout (ou com Height="0").
        modulo.Setup<int[]>("tamanho", _ => true).SetResult([1200, 0]);
        var grafico = Colunas();

        Assert.False(await grafico.Instance.ExportAsync(RvmChartExportFormat.Pdf));
    }

    [Fact]
    public void Grafico_sem_eixo_cartesiano_nao_promete_teclas_de_zoom()
    {
        // A rosca nao tem area de desenho para recortar: pedir Zoomable nela nao pode virar instrucao de
        // tecla que nao faz nada para quem usa leitor de tela.
        var rosca = Render<RvmPieChart<Mes>>(p => p
            .Add(x => x.Items, Dados).Add(x => x.Label, m => m.Nome).Add(x => x.Value, m => m.Valor)
            .Add(x => x.AriaLabel, "Movimento por mes")
            .Add(x => x.Zoomable, true)
            .Add(x => x.SelectionMode, RvmChartSelectionMode.Range));

        var instrucao = rosca.Find("span[id$=instrucao]").TextContent;
        Assert.DoesNotContain("Mais e menos aproximam", instrucao);
        Assert.DoesNotContain("Shift com as setas", instrucao);
        Assert.Single(rosca.FindAll("span[aria-live=polite]"));
    }

    [Fact]
    public async Task Zoom_num_grafico_de_um_ponto_so_nao_manda_o_ponto_para_fora()
    {
        var linha = Render<RvmLineChart<Mes>>(p => p
            .Add(x => x.Items, Dados.Take(1)).Add(x => x.Label, m => m.Nome).Add(x => x.AriaLabel, "Um mes so")
            .Add(x => x.Zoomable, true).Add(x => x.ShowPoints, true)
            .AddChildContent<RvmChartSeries<Mes>>(s => s.Add(x => x.Name, "Valor").Add(x => x.Value, m => m.Valor)));

        // Roda perto da borda, varias vezes: a janela do eixo X sai do meio.
        for (var i = 0; i < 5; i++)
        {
            await linha.InvokeAsync(() => linha.Instance.RodarNoGrafico(-100, 560, 150, false));
        }

        var x = double.Parse(linha.Find("circle.rvm-grafico-ponto").GetAttribute("cx")!, CultureInfo.InvariantCulture);
        Assert.InRange(x, 40, 584);
    }

    [Fact]
    public void Faixa_com_o_fim_antes_do_inicio_e_erro_de_quem_chamou()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new RvmChartRange(4, 2));

    [Fact]
    public async Task Ligar_o_zoom_depois_do_primeiro_render_liga_a_roda()
    {
        // Em modo relaxado o bUnit atende e registra as chamadas do modulo; o que interessa e quando a
        // roda foi ligada.
        var modulo = JSInterop.SetupModule("./_content/RVM.DesignSystem/rvm-grafico.js");

        var grafico = Render<RvmLineChart<Mes>>(p => p
            .Add(x => x.Items, Dados).Add(x => x.Label, m => m.Nome).Add(x => x.AriaLabel, "Movimento por mes")
            .Add(x => x.Zoomable, false)
            .AddChildContent<RvmChartSeries<Mes>>(s => s.Add(x => x.Name, "Valor").Add(x => x.Value, m => m.Valor)));
        Assert.Empty(modulo.Invocations["observarRoda"]);

        grafico.Render(p => p.Add(x => x.Zoomable, true));

        Assert.Single(modulo.Invocations["observarRoda"]);
    }
}
