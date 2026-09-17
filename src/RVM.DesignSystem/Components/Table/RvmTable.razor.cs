using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Table;

/// <summary>
/// Tabela de dados: colunas declaradas com <see cref="RvmTableColumn{TItem}"/>, ordenacao pelo titulo,
/// selecao por caixa de marcar e versao densa. Para paginar e filtrar, <see cref="RvmDataGrid{TItem}"/>.
/// </summary>
[CascadingTypeParameter(nameof(TItem))]
public partial class RvmTable<TItem> : ComponentBase
{
    private static int _proximoId;
    private readonly List<RvmTableColumn<TItem>> _colunas = [];
    private IReadOnlyCollection<TItem>? _selecaoRecebida;
    private HashSet<TItem> _selecionados = [];

    /// <summary>Base dos ids gerados (o select de linhas por pagina precisa de um).</summary>
    private string IdBase { get; } = $"rvm-tabela-{Interlocked.Increment(ref _proximoId)}";

    /// <summary>As linhas.</summary>
    [Parameter, EditorRequired] public IEnumerable<TItem>? Items { get; set; }

    /// <summary>As colunas: <see cref="RvmTableColumn{TItem}"/>.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Nome da tabela para o leitor de tela (vai num <c>caption</c> escondido). Tambem nomeia a area
    /// de rolagem quando a tabela nao cabe na largura.
    /// </summary>
    [Parameter, EditorRequired] public string Caption { get; set; } = "";

    /// <summary>Linhas de 36 px em vez de 52 — medidos no kit.</summary>
    [Parameter] public bool Dense { get; set; }

    /// <summary>Uma caixa de marcar por linha e outra no cabecalho para todas.</summary>
    [Parameter] public bool Selectable { get; set; }

    /// <summary>As linhas marcadas. Aceita <c>@bind-SelectedItems</c>.</summary>
    [Parameter] public IReadOnlyCollection<TItem>? SelectedItems { get; set; }

    /// <summary>Disparado quando a marcacao muda.</summary>
    [Parameter] public EventCallback<IReadOnlyCollection<TItem>> SelectedItemsChanged { get; set; }

    /// <summary>
    /// Nome da caixa de marcar de cada linha. Padrao: "Selecionar" + o texto da primeira coluna — o
    /// leitor de tela precisa saber QUAL linha marca, e "Selecionar linha 3" nao diz.
    /// </summary>
    [Parameter] public Func<TItem, string>? RowLabel { get; set; }

    /// <summary>Texto quando nao ha linhas.</summary>
    [Parameter] public string NoRecordsText { get; set; } = "Nenhum registro para mostrar.";

    /// <summary>Atributos extras, repassados ao elemento raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal IReadOnlyList<RvmTableColumn<TItem>> Colunas => _colunas;

    internal RvmTableColumn<TItem>? ColunaOrdenada { get; private set; }

    internal RvmSortDirection Sentido { get; private set; }

    /// <summary>Visual e recursos da grade (cabecalho escuro, separadores, filtro, paginacao).</summary>
    private protected virtual bool EhGrade => false;

    /// <summary>Texto de cada filtro por coluna (so a grade preenche).</summary>
    private protected Dictionary<RvmTableColumn<TItem>, string> Filtros { get; } = [];

    /// <summary>Tamanho da pagina, ou <c>null</c> para mostrar tudo.</summary>
    private protected virtual int? TamanhoDaPagina => null;

    /// <summary>A pagina mostrada para um total de linhas, comecando em 1.</summary>
    private protected virtual int PaginaPara(int total) => 1;

    internal void AdicionarColuna(RvmTableColumn<TItem> coluna)
    {
        _colunas.Add(coluna);
        if (ColunaOrdenada is null && coluna.InitialSort is { } sentido && coluna.Ordenavel)
        {
            ColunaOrdenada = coluna;
            Sentido = sentido;
        }
    }

    internal void RemoverColuna(RvmTableColumn<TItem> coluna)
    {
        _colunas.Remove(coluna);
        Filtros.Remove(coluna);
        if (ColunaOrdenada == coluna)
        {
            ColunaOrdenada = null;
        }
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        // So recria a marcacao quando chega uma colecao nova: a mesma referencia de volta pelo @bind
        // nao apaga o que a pessoa acabou de marcar.
        if (!ReferenceEquals(SelectedItems, _selecaoRecebida))
        {
            _selecaoRecebida = SelectedItems;
            _selecionados = SelectedItems is null ? [] : [.. SelectedItems];
        }
    }

    /// <summary>As linhas depois do filtro e da ordenacao, antes da paginacao.</summary>
    internal IReadOnlyList<TItem> Filtradas
    {
        get
        {
            IEnumerable<TItem> linhas = Items ?? [];
            foreach (var (coluna, texto) in Filtros)
            {
                if (!string.IsNullOrWhiteSpace(texto))
                {
                    var procurado = texto.Trim();
                    linhas = linhas.Where(i => coluna.TextoDe(i).Contains(procurado, StringComparison.CurrentCultureIgnoreCase));
                }
            }

            if (ColunaOrdenada is { Value: { } valor })
            {
                linhas = Sentido == RvmSortDirection.Ascending
                    ? linhas.OrderBy(valor, Comparer<object?>.Default)
                    : linhas.OrderByDescending(valor, Comparer<object?>.Default);
            }

            return [.. linhas];
        }
    }

    /// <summary>As linhas desenhadas agora.</summary>
    internal IReadOnlyList<TItem> Visiveis(IReadOnlyList<TItem> filtradas)
        => TamanhoDaPagina is { } tamanho
            ? [.. filtradas.Skip((PaginaPara(filtradas.Count) - 1) * tamanho).Take(tamanho)]
            : filtradas;

    internal async Task OrdenarAsync(RvmTableColumn<TItem> coluna)
    {
        // Crescente, decrescente, sem ordem — o ciclo do MUI, que o kit segue.
        if (ColunaOrdenada != coluna)
        {
            ColunaOrdenada = coluna;
            Sentido = RvmSortDirection.Ascending;
        }
        else if (Sentido == RvmSortDirection.Ascending)
        {
            Sentido = RvmSortDirection.Descending;
        }
        else
        {
            ColunaOrdenada = null;
        }

        await AoMudarOsDadosAsync();
    }

    /// <summary>A grade volta para a primeira pagina quando a ordem ou o filtro mudam.</summary>
    private protected virtual Task AoMudarOsDadosAsync() => Task.CompletedTask;

    /// <summary>Texto da linha vazia: sem dados, ou nada casou com o filtro.</summary>
    private protected virtual string TextoSemLinhas => NoRecordsText;

    /// <summary>Opcoes do "Linhas por pagina".</summary>
    private protected virtual IReadOnlyList<int> OpcoesDeTamanho => [];

    private protected virtual Task MudarTamanhoAsync(string? valor) => Task.CompletedTask;

    private protected virtual Task IrParaPaginaAsync(int pagina) => Task.CompletedTask;

    private protected virtual Task FiltrarAsync(RvmTableColumn<TItem> coluna, string? texto) => Task.CompletedTask;

    internal static string? AriaSort(bool ordenada, RvmSortDirection sentido)
        => !ordenada ? null : sentido == RvmSortDirection.Ascending ? "ascending" : "descending";

    internal bool EstaSelecionado(TItem item) => _selecionados.Contains(item);

    internal async Task MarcarAsync(TItem item, bool marcado)
    {
        if (marcado ? _selecionados.Add(item) : _selecionados.Remove(item))
        {
            await AvisarSelecaoAsync();
        }
    }

    /// <summary>A caixa do cabecalho vale para as linhas visiveis — na grade, a pagina atual.</summary>
    internal async Task MarcarTodasAsync(IReadOnlyList<TItem> visiveis, bool marcado)
    {
        foreach (var item in visiveis)
        {
            if (marcado) _selecionados.Add(item); else _selecionados.Remove(item);
        }

        await AvisarSelecaoAsync();
    }

    private async Task AvisarSelecaoAsync()
    {
        IReadOnlyCollection<TItem> nova = [.. _selecionados];
        _selecaoRecebida = nova;
        await SelectedItemsChanged.InvokeAsync(nova);
    }

    internal string RotuloDaLinha(TItem item)
        => RowLabel?.Invoke(item)
           ?? (Colunas.FirstOrDefault(c => c.Value is not null) is { } primeira
               ? $"Selecionar {primeira.TextoDe(item)}"
               : "Selecionar linha");

    // O RvmAdiado e interno, e o Razor so enxerga componente publico na marcacao: entra pelo builder.
    private static RenderFragment DepoisDasColunas(RenderFragment conteudo) => builder =>
    {
        builder.OpenComponent<RvmAdiado>(0);
        builder.AddComponentParameter(1, nameof(RvmAdiado.ChildContent), conteudo);
        builder.CloseComponent();
    };

    internal static string ClasseDoAlinhamento(RvmTableAlign alinhamento)
        => alinhamento switch
        {
            RvmTableAlign.Center => "rvm-centro",
            RvmTableAlign.End => "rvm-fim",
            _ => "rvm-inicio"
        };

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = "rvm-tabela";
            if (EhGrade) proprias += " rvm-grade-dados";
            if (Dense) proprias += " rvm-densa";

            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }
}
