using System.ComponentModel;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RVM.DesignSystem.Components.Table;

namespace RVM.DesignSystem.Components.Chart;

/// <summary>
/// O que todo grafico tem: dados, series, a figura com nome, a dica, a legenda, o teclado e a tabela de
/// dados para leitor de tela. Cada tipo so desenha o SVG.
/// </summary>
/// <remarks>
/// O SVG sai pronto no primeiro render, sem JS. O JS so mede a largura real do grafico: antes disso o
/// desenho assume 600 px e escala para caber.
/// </remarks>
[CascadingTypeParameter(nameof(TItem))]
public abstract partial class RvmChartBase<TItem> : ComponentBase, IAsyncDisposable
{
    internal const double LarguraPadrao = 600;
    internal const double MargemTopo = 16;
    internal const double MargemDireitaPadrao = 16;
    internal const double MargemBaixo = 32;

    private static readonly RvmColor[] Paleta =
        [RvmColor.Primary, RvmColor.Success, RvmColor.Warning, RvmColor.Info, RvmColor.Error, RvmColor.Secondary];

    private readonly List<RvmChartSeries<TItem>> _series = [];
    private ElementReference _area;
    private ElementReference _svg;
    private ElementReference _camada;
    private IJSObjectReference? _modulo;
    private IJSObjectReference? _observador;
    private IJSObjectReference? _teclado;
    private DotNetObjectReference<RvmChartBase<TItem>>? _referencia;

    [Inject] private IJSRuntime JS { get; set; } = default!;

    /// <summary>Base dos ids gerados (gradientes, instrucoes).</summary>
    internal string IdBase { get; } = GeradorDeIds.Novo("rvm-grafico");

    /// <summary>Os dados.</summary>
    [Parameter, EditorRequired] public IEnumerable<TItem>? Items { get; set; }

    /// <summary>O nome de cada item: a categoria no eixo, a fatia na legenda.</summary>
    [Parameter] public Func<TItem, string>? Label { get; set; }

    /// <summary>As series: <see cref="RvmChartSeries{TItem}"/>.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Nome do grafico para o leitor de tela ("Vendas por mes"). Obrigatorio.</summary>
    [Parameter, EditorRequired] public string AriaLabel { get; set; } = "";

    /// <summary>Altura em px. Padrao: 300.</summary>
    [Parameter] public int Height { get; set; } = 300;

    /// <summary>Legenda abaixo do grafico. Padrao: sim (aparece quando ha mais de uma serie).</summary>
    [Parameter] public bool ShowLegend { get; set; } = true;

    /// <summary>Linhas de grade. Padrao: sim.</summary>
    [Parameter] public bool ShowGrid { get; set; } = true;

    /// <summary>Formato dos valores no eixo e na dica. Padrao: <see cref="RvmChartFormat.Compact"/>.</summary>
    [Parameter] public Func<double, string>? ValueFormat { get; set; }

    /// <summary>
    /// Formato dos valores do eixo secundario. Sem valor, o compacto padrao — nao o
    /// <see cref="ValueFormat"/>, que descreve a unidade do eixo primario.
    /// </summary>
    [Parameter] public Func<double, string>? SecondaryValueFormat { get; set; }

    /// <summary>
    /// Anima o desenho ao aparecer e ao mudar de valor. Padrao: sim — e desligada sozinha para quem pediu
    /// "reduzir movimento" no sistema.
    /// </summary>
    [Parameter] public bool Animated { get; set; } = true;

    /// <summary>Nome do arquivo exportado, sem extensao. Padrao: o <see cref="AriaLabel"/> em minusculas com hifens.</summary>
    [Parameter] public string? ExportFileName { get; set; }

    /// <summary>Atributos extras, repassados a figura.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal IReadOnlyList<RvmChartSeries<TItem>> Series => _series;

    internal double Largura { get; private set; } = LarguraPadrao;

    internal int? Ativo { get; private set; }

    internal IReadOnlyList<TItem> Dados => Memo("Dados", () => Items as IReadOnlyList<TItem> ?? [.. Items ?? []]);

    private readonly Dictionary<string, (int Versao, object? Valor)> _cache = [];
    private int _versaoDoLayout;

    /// <summary>
    /// Guarda um calculo de layout ate os parametros ou a largura mudarem. Sem isso, colunas, pontos e
    /// fatias eram recalculados varias vezes por render — e mover o mouse renderiza a cada evento
    /// (achado do review dos graficos).
    /// </summary>
    internal T Memo<T>(string chave, Func<T> calcular)
    {
        if (_cache.TryGetValue(chave, out var guardado) && guardado.Versao == _versaoDoLayout)
        {
            return (T)guardado.Valor!;
        }

        var valor = calcular();
        _cache[chave] = (_versaoDoLayout, valor);
        return valor;
    }

    /// <inheritdoc />
    protected override void OnParametersSet() => _versaoDoLayout++;

    internal string Formatar(double valor) => (ValueFormat ?? RvmChartFormat.Compact)(valor);

    /// <summary>O valor no formato do eixo em que a serie e lida.</summary>
    internal string Formatar(double valor, RvmChartAxis eixo)
        => eixo == RvmChartAxis.Secondary ? (SecondaryValueFormat ?? RvmChartFormat.Compact)(valor) : Formatar(valor);

    /// <summary>
    /// Ha duas escalas de valor a desenhar: alguma serie pediu o eixo da direita e alguma ficou na
    /// esquerda. Se TODAS pedirem o secundario, o grafico segue com um eixo so — dois eixos iguais
    /// ocupariam as duas bordas para dizer a mesma coisa.
    /// </summary>
    internal bool TemEixoSecundario
        => _series.Any(s => s.Axis == RvmChartAxis.Secondary) && _series.Any(s => s.Axis == RvmChartAxis.Primary);

    /// <summary>As series lidas naquele eixo (todas, quando o grafico tem um eixo so).</summary>
    internal IReadOnlyList<RvmChartSeries<TItem>> SeriesDo(RvmChartAxis eixo)
        => !TemEixoSecundario
            ? eixo == RvmChartAxis.Primary ? _series : []
            : [.. _series.Where(s => s.Axis == eixo)];

    /// <summary>Em qual eixo a serie e lida de fato (o primario, se o grafico so tem um).</summary>
    internal RvmChartAxis EixoDa(RvmChartSeries<TItem> serie)
        => TemEixoSecundario ? serie.Axis : RvmChartAxis.Primary;

    /// <summary>O nome da serie na tabela de dados, dizendo o eixo quando ha dois.</summary>
    internal string NomeNaTabela(RvmChartSeries<TItem> serie)
        => TemEixoSecundario && serie.Axis == RvmChartAxis.Secondary ? $"{serie.Name} (eixo direito)" : serie.Name;

    /// <summary>Rotulo de uma marca de eixo: o formato do consumidor, ou o compacto na precisao do passo.</summary>
    internal string FormatarMarca(double marca, double passo)
        => ValueFormat is { } formato ? formato(marca) : RvmChartFormat.CompactForAxis(marca, passo);

    /// <summary>Rotulo de uma marca, no formato do eixo a que ela pertence.</summary>
    internal string FormatarMarca(double marca, double passo, RvmChartAxis eixo)
        => eixo == RvmChartAxis.Secondary
            ? SecondaryValueFormat is { } formato ? formato(marca) : RvmChartFormat.CompactForAxis(marca, passo)
            : FormatarMarca(marca, passo);

    internal string NomeDe(TItem item, int indice) => Label?.Invoke(item) ?? (indice + 1).ToString(CultureInfo.InvariantCulture);

    internal RvmColor CorDa(RvmChartSeries<TItem> serie) => serie.Color ?? Paleta[_series.IndexOf(serie) % Paleta.Length];

    internal static RvmColor CorDaPaleta(int indice) => Paleta[indice % Paleta.Length];

    internal static string ClasseDaCor(RvmColor cor) => "rvm-cor-" + cor.ToString().ToLowerInvariant();

    internal static string N(double valor) => Escala.N(valor);

    /// <summary>
    /// Um &lt;text&gt; do SVG. Em .razor a tag &lt;text&gt; e reservada do Razor (vira "so texto" e nao aceita
    /// atributo); por isso o texto do grafico sai daqui, com o conteudo codificado.
    /// </summary>
    internal static MarkupString Texto(double x, double y, string conteudo, string ancora = "middle", string? baseline = null, string classe = "rvm-grafico-rotulo")
        => new($"<text class=\"{classe}\" x=\"{N(x)}\" y=\"{N(y)}\" text-anchor=\"{ancora}\"{(baseline is null ? "" : $" dominant-baseline=\"{baseline}\"")}>{System.Net.WebUtility.HtmlEncode(conteudo)}</text>");

    internal void AdicionarSerie(RvmChartSeries<TItem> serie) => _series.Add(serie);

    internal void RemoverSerie(RvmChartSeries<TItem> serie) => _series.Remove(serie);

    // --- O que cada tipo de grafico diz ---

    /// <summary>O conteudo do SVG (sem a tag svg).</summary>
    internal abstract RenderFragment Desenho { get; }

    /// <summary>Quantos pontos o teclado percorre (categorias, fatias, pontos).</summary>
    internal abstract int QuantidadeDePontos { get; }

    /// <summary>Titulo da dica do ponto (a categoria).</summary>
    internal abstract string TituloDoPonto(int indice);

    /// <summary>As linhas da dica do ponto: serie, valor e cor.</summary>
    internal abstract IReadOnlyList<LinhaDaDica> LinhasDoPonto(int indice);

    /// <summary>Onde a dica do ponto aponta, em coordenadas do desenho.</summary>
    internal abstract (double X, double Y) AncoraDoPonto(int indice);

    /// <summary>O ponto sob o ponteiro, em coordenadas do desenho.</summary>
    internal abstract int? PontoEm(double x, double y);

    /// <summary>Itens da legenda. Com dois eixos, cada serie diz em qual delas e lida.</summary>
    internal virtual IReadOnlyList<ItemDaLegenda> Legenda
        => _series.Count > 1
            ? [.. _series.Select(s => new ItemDaLegenda(s.Name, CorDa(s),
                TemEixoSecundario ? (s.Axis == RvmChartAxis.Secondary ? "eixo direito" : "eixo esquerdo") : null))]
            : [];

    /// <summary>A tabela de dados, com os valores passados pelo formatador informado.</summary>
    internal abstract TabelaDeDados MontarTabela(Func<double, string> formatar);

    /// <summary>A tabela de dados para leitor de tela, no formato da dica e do eixo.</summary>
    internal TabelaDeDados Tabela => MontarTabela(Formatar);

    /// <summary>Conteudo sobre o centro do desenho (o total da rosca).</summary>
    internal virtual RenderFragment? Centro => null;

    internal sealed record LinhaDaDica(string Nome, string Valor, RvmColor Cor);

    internal sealed record ItemDaLegenda(string Nome, RvmColor Cor, string? Detalhe);

    internal sealed record TabelaDeDados(IReadOnlyList<string> Cabecalho, IReadOnlyList<IReadOnlyList<string>> Linhas);

    // --- Estado da dica e do teclado ---

    internal string? TextoDoAtivo
        => Ativo is { } i && i < QuantidadeDePontos
            ? $"{TituloDoPonto(i)}: {string.Join("; ", LinhasDoPonto(i).Select(l => string.IsNullOrEmpty(l.Nome) ? l.Valor : $"{l.Nome} {l.Valor}"))}"
            : null;

    internal string EstiloDaDica
    {
        get
        {
            if (Ativo is not { } i || i >= QuantidadeDePontos)
            {
                return "display: none";
            }

            var (x, y) = AncoraDoPonto(i);
            return string.Create(CultureInfo.InvariantCulture,
                $"left: {Math.Clamp(x / Largura * 100, 0, 100):0.##}%; top: {Math.Clamp(y / Height * 100, 0, 100):0.##}%");
        }
    }

    internal string ClassesDaDica
    {
        get
        {
            if (Ativo is not { } i || i >= QuantidadeDePontos)
            {
                return "rvm-grafico-dica";
            }

            var (x, y) = AncoraDoPonto(i);
            var classes = "rvm-grafico-dica";
            if (x < Largura * 0.2) classes += " rvm-dica-a-direita";
            else if (x > Largura * 0.8) classes += " rvm-dica-a-esquerda";
            if (y < Height * 0.3) classes += " rvm-dica-abaixo";
            return classes;
        }
    }

    internal void AoMoverPonteiro(PointerEventArgs e) => Ativo = PontoEm(e.OffsetX, e.OffsetY);

    internal void AoSairPonteiro() => Ativo = null;

    internal void AoTeclar(KeyboardEventArgs e)
    {
        var total = QuantidadeDePontos;
        if (total == 0)
        {
            return;
        }

        Ativo = e.Key switch
        {
            "ArrowRight" or "ArrowDown" => Ativo is { } i ? (i + 1) % total : 0,
            "ArrowLeft" or "ArrowUp" => Ativo is { } i ? (i - 1 + total) % total : total - 1,
            "Home" => 0,
            "End" => total - 1,
            "Escape" => null,
            _ => Ativo
        };
    }

    private string NomeDoArquivo
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(ExportFileName))
            {
                return ExportFileName.Trim();
            }

            var semAcento = AriaLabel.Normalize(System.Text.NormalizationForm.FormD)
                .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark);
            var limpo = new string([.. semAcento.Select(c => char.IsLetterOrDigit(c) ? char.ToLowerInvariant(c) : '-')]);
            var nome = string.Join('-', limpo.Split('-', StringSplitOptions.RemoveEmptyEntries));
            return string.IsNullOrEmpty(nome) ? "grafico" : nome;
        }
    }

    /// <summary>
    /// Baixa o grafico: imagem PNG, o proprio SVG com as cores embutidas, os dados em CSV ou um PDF de uma
    /// pagina com a imagem. Devolve <c>false</c> quando o navegador nao deixou (sem JS, aba fechando).
    /// </summary>
    public async Task<bool> ExportAsync(RvmChartExportFormat format)
    {
        try
        {
            _modulo ??= await JS.InvokeAsync<IJSObjectReference>("import", "./_content/RVM.DesignSystem/rvm-grafico.js");
            switch (format)
            {
                case RvmChartExportFormat.Csv:
                    // Numero inteiro, nao o compacto do eixo: "45.000" o Excel soma, "45 mil" e texto.
                    var tabela = MontarTabela(RvmChartFormat.Number);
                    await _modulo.InvokeVoidAsync("baixar", $"{NomeDoArquivo}.csv", "text/csv;charset=utf-8",
                        RvmChartCsv.Gerar(tabela.Cabecalho, tabela.Linhas), false);
                    return true;

                case RvmChartExportFormat.Svg:
                    var svg = await _modulo.InvokeAsync<string>("serializar", _svg);
                    await _modulo.InvokeVoidAsync("baixar", $"{NomeDoArquivo}.svg", "image/svg+xml;charset=utf-8", svg, false);
                    return true;

                case RvmChartExportFormat.Png:
                    var png = await _modulo.InvokeAsync<string>("paraImagem", _svg, "image/png", 2);
                    await _modulo.InvokeVoidAsync("baixar", $"{NomeDoArquivo}.png", "image/png", png, true);
                    return true;

                default:
                    // O PDF aceita JPEG direto (DCTDecode); o PNG teria de ser descomprimido e reescrito.
                    var jpeg = await _modulo.InvokeAsync<string>("paraImagem", _svg, "image/jpeg", 2);
                    // [largura, altura] em vez de um objeto: menos um tipo publico so para o interop.
                    var tamanho = await _modulo.InvokeAsync<int[]>("tamanho", _svg, 2);
                    var pdf = RvmChartPdf.Criar(Convert.FromBase64String(jpeg), tamanho[0], tamanho[1]);
                    await _modulo.InvokeVoidAsync("baixar", $"{NomeDoArquivo}.pdf", "application/pdf", Convert.ToBase64String(pdf), true);
                    return true;
            }
        }
        catch (Exception e) when (e is JSException or JSDisconnectedException or InvalidOperationException or TaskCanceledException)
        {
            return false;
        }
    }

    internal string ClassesDaRaiz
        => AdditionalAttributes is not null
           && AdditionalAttributes.TryGetValue("class", out var informada)
           && informada is string texto
           && !string.IsNullOrWhiteSpace(texto)
            ? $"{ClassesProprias} {texto}"
            : ClassesProprias;

    private string ClassesProprias => Animated ? "rvm-grafico rvm-animado" : "rvm-grafico";

    // --- Layout cartesiano, comum a colunas, barras, linha, area, histograma e dispersao ---

    /// <summary>
    /// Espaco a direita do desenho: a margem de sempre, ou o que os rotulos do eixo secundario precisam.
    /// </summary>
    internal double MargemDireita
        => Memo("MargemDireita", () => RotulosDoEixoSecundario is { } rotulos
            ? LarguraDosRotulos(rotulos, MargemDireitaPadrao)
            : MargemDireitaPadrao);

    /// <summary>
    /// Os rotulos do eixo secundario, nos graficos que o desenham; <c>null</c> quando nao ha segundo eixo.
    /// </summary>
    internal virtual IEnumerable<string>? RotulosDoEixoSecundario => null;

    /// <summary>Largura reservada aos rotulos do eixo de valores, pelo maior texto.</summary>
    internal static double LarguraDosRotulos(IEnumerable<string> textos, double minimo = 32)
        => Math.Max(minimo, textos.Select(t => t.Length).DefaultIfEmpty(0).Max() * 7 + 12);

    /// <summary>De quantas em quantas categorias escrever o rotulo, para nao encavalar.</summary>
    internal static int SaltoDeRotulos(int quantidade, double espaco, double larguraMinima = 44)
        => Math.Max(1, (int)Math.Ceiling(quantidade / Math.Max(1, espaco / larguraMinima)));

    // O RvmAdiado e interno, e o Razor so enxerga componente publico na marcacao: entra pelo builder.
    // Desenhar DEPOIS das series e o que garante que o grafico ve o parametro novo delas no mesmo render.
    private static RenderFragment DepoisDasSeries(RenderFragment conteudo) => builder =>
    {
        builder.OpenComponent<RvmAdiado>(0);
        builder.AddComponentParameter(1, nameof(RvmAdiado.ChildContent), conteudo);
        builder.CloseComponent();
    };

    /// <summary>Chamado pelo JS quando a largura real do grafico muda.</summary>
    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task DefinirLargura(int largura)
    {
        if (largura > 0 && Math.Abs(largura - Largura) >= 1)
        {
            Largura = largura;
            _versaoDoLayout++;
            StateHasChanged();
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        try
        {
            _referencia = DotNetObjectReference.Create(this);
            _modulo = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/RVM.DesignSystem/rvm-grafico.js");
            _observador = await _modulo.InvokeAsync<IJSObjectReference?>("observar", _area, _referencia);
            _teclado = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/RVM.DesignSystem/rvm-teclado.js");
            await _teclado.InvokeVoidAsync("prenderTeclas", _camada, new[] { "ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight", "Home", "End" });
        }
        catch (Exception e) when (e is JSException or JSDisconnectedException or InvalidOperationException or TaskCanceledException)
        {
            // Sem JS (pre-renderizacao, bUnit): o grafico fica na largura padrao, escalado para caber.
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_observador is not null)
            {
                await _observador.InvokeVoidAsync("parar");
                await _observador.DisposeAsync();
            }

            if (_modulo is not null) await _modulo.DisposeAsync();
            if (_teclado is not null) await _teclado.DisposeAsync();
        }
        catch (Exception e) when (e is JSDisconnectedException or JSException or TaskCanceledException)
        {
            // Circuito ja caiu ou a pagina saiu: nada a liberar do lado do navegador.
        }

        _referencia?.Dispose();
        GC.SuppressFinalize(this);
    }
}
