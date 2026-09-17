using System.Text;
using Bunit;
using RVM.DesignSystem.Components.Chart;

namespace RVM.DesignSystem.Tests.Components;

public class RvmChartCsvTests
{
    [Fact]
    public void Csv_tem_BOM_ponto_e_virgula_e_campos_escapados()
    {
        var csv = RvmChartCsv.Gerar(
            ["Categoria", "Valor"],
            [["Jan", "10"], ["Soja; milho", "20"], ["Disse \"ok\"", "30"]]);

        Assert.StartsWith("﻿", csv);
        var linhas = csv.TrimStart('﻿').Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(["Categoria;Valor", "Jan;10", "\"Soja; milho\";20", "\"Disse \"\"ok\"\"\";30"], linhas);
    }
}

public class RvmChartPdfTests
{
    private static readonly byte[] Jpeg = [0xFF, 0xD8, 0xFF, 0xE0, 1, 2, 3, 0xFF, 0xD9];

    [Fact]
    public void Pdf_de_uma_pagina_com_a_imagem_dentro()
    {
        var pdf = RvmChartPdf.Criar(Jpeg, 600, 300);
        var texto = Encoding.Latin1.GetString(pdf);

        Assert.StartsWith("%PDF-1.4", texto);
        Assert.EndsWith("%%EOF\n", texto);
        Assert.Contains("/Type /Catalog", texto);
        Assert.Contains("/MediaBox [0 0 600 300]", texto);
        Assert.Contains("/Filter /DCTDecode", texto);
        Assert.Contains($"/Length {Jpeg.Length}", texto);
        Assert.Contains("q 600 0 0 300 0 0 cm /Im0 Do Q", texto);
        // O JPEG entra byte a byte, sem reescrever pixel.
        Assert.Contains(Encoding.Latin1.GetString(Jpeg), texto);
    }

    [Fact]
    public void Xref_aponta_para_o_inicio_de_cada_objeto()
    {
        var pdf = RvmChartPdf.Criar(Jpeg, 100, 50);
        var texto = Encoding.Latin1.GetString(pdf);

        var inicioDoXref = int.Parse(texto[(texto.LastIndexOf("startxref", StringComparison.Ordinal) + 10)..].Split('\n')[0]);
        Assert.StartsWith("xref", texto[inicioDoXref..]);

        var entradas = texto[(inicioDoXref + "xref\n0 6\n0000000000 65535 f \n".Length)..].Split('\n');
        for (var i = 0; i < 5; i++)
        {
            var posicao = int.Parse(entradas[i][..10]);
            Assert.StartsWith($"{i + 1} 0 obj", texto[posicao..]);
        }
    }

    [Fact]
    public void Imagem_vazia_ou_tamanho_invalido_e_erro_de_quem_chamou()
    {
        Assert.Throws<ArgumentException>(() => RvmChartPdf.Criar([], 100, 100));
        Assert.Throws<ArgumentException>(() => RvmChartPdf.Criar(Jpeg, 0, 100));
    }
}

public class RvmGraficoExportarTests : BunitContext
{
    public RvmGraficoExportarTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<RvmColumnChart<Venda>> Grafico(Action<ComponentParameterCollectionBuilder<RvmColumnChart<Venda>>>? extra = null)
        => Render<RvmColumnChart<Venda>>(p =>
        {
            p.Add(x => x.Items, [new Venda("Jan", 10_000, 0), new Venda("Fev", 20_000, 0)])
             .Add(x => x.Label, v => v.Mes)
             .Add(x => x.AriaLabel, "Receita por mes")
             .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Receita").Add(x => x.Value, v => v.Receita));
            extra?.Invoke(p);
        });

    private BunitJSModuleInterop Modulo() => JSInterop.SetupModule("./_content/RVM.DesignSystem/rvm-grafico.js");

    [Fact]
    public void Animado_por_padrao_e_desligavel()
    {
        Assert.Contains("rvm-animado", Grafico().Find("figure").ClassName);
        Assert.DoesNotContain("rvm-animado", Grafico(p => p.Add(x => x.Animated, false)).Find("figure").ClassName);
        Assert.Contains("pathLength", Render<RvmLineChart<Venda>>(p => p
            .Add(x => x.Items, [new Venda("Jan", 1, 0)]).Add(x => x.AriaLabel, "Linha")
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "R").Add(x => x.Value, v => v.Receita)))
            .Find("path.rvm-grafico-linha").OuterHtml);
    }

    [Fact]
    public async Task Exportar_csv_manda_os_dados_da_tabela_para_o_navegador()
    {
        var modulo = Modulo();
        var cortado = Grafico();

        Assert.True(await cortado.Instance.ExportAsync(RvmChartExportFormat.Csv));

        var chamada = modulo.Invocations["baixar"].Single();
        Assert.Equal("receita-por-mes.csv", chamada.Arguments[0]);
        Assert.Equal("text/csv;charset=utf-8", chamada.Arguments[1]);
        Assert.Contains("Categoria;Receita", (string)chamada.Arguments[2]!);
        // Numero cheio, nao o compacto do eixo.
        Assert.Contains("Fev;20.000", (string)chamada.Arguments[2]!);
        Assert.Equal(false, chamada.Arguments[3]);
    }

    [Fact]
    public async Task Exportar_svg_e_png_pedem_o_desenho_ao_navegador()
    {
        var modulo = Modulo();
        modulo.Setup<string>("serializar", _ => true).SetResult("<svg/>");
        modulo.Setup<string>("paraImagem", _ => true).SetResult("AAAA");
        var cortado = Grafico();

        Assert.True(await cortado.Instance.ExportAsync(RvmChartExportFormat.Svg));
        Assert.True(await cortado.Instance.ExportAsync(RvmChartExportFormat.Png));

        var chamadas = modulo.Invocations["baixar"].ToList();
        Assert.Equal(["receita-por-mes.svg", "receita-por-mes.png"], chamadas.Select(c => (string)c.Arguments[0]!));
        Assert.Equal(["image/svg+xml;charset=utf-8", "image/png"], chamadas.Select(c => (string)c.Arguments[1]!));
        Assert.Equal([false, true], chamadas.Select(c => (bool)c.Arguments[3]!));
        // PNG em duas vezes a resolucao da tela.
        Assert.Equal(2, modulo.Invocations["paraImagem"].Single().Arguments[2]);
    }

    [Fact]
    public async Task Exportar_pdf_usa_a_imagem_do_navegador_e_o_nome_escolhido()
    {
        var modulo = Modulo();
        modulo.Setup<string>("paraImagem", _ => true).SetResult(Convert.ToBase64String([0xFF, 0xD8, 0xFF, 0xD9]));
        modulo.Setup<int[]>("tamanho", _ => true).SetResult([1200, 600]);
        var cortado = Grafico(p => p.Add(x => x.ExportFileName, "painel-da-safra"));

        Assert.True(await cortado.Instance.ExportAsync(RvmChartExportFormat.Pdf));

        var chamada = modulo.Invocations["baixar"].Single();
        Assert.Equal("painel-da-safra.pdf", chamada.Arguments[0]);
        Assert.Equal("application/pdf", chamada.Arguments[1]);
        Assert.Equal("image/jpeg", modulo.Invocations["paraImagem"].Single().Arguments[1]);
        var pdf = Encoding.Latin1.GetString(Convert.FromBase64String((string)chamada.Arguments[2]!));
        Assert.StartsWith("%PDF-1.4", pdf);
        Assert.Contains("/MediaBox [0 0 1200 600]", pdf);
    }

    [Fact]
    public async Task Navegador_que_recusa_devolve_falso_em_vez_de_estourar()
    {
        var modulo = Modulo();
        modulo.Setup<string>("paraImagem", _ => true).SetException(new Microsoft.JSInterop.JSException("canvas bloqueado"));
        var cortado = Grafico();

        Assert.False(await cortado.Instance.ExportAsync(RvmChartExportFormat.Png));
    }
}
