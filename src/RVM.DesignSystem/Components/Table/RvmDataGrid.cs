using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Table;

/// <summary>
/// A <see cref="RvmTable{TItem}"/> com paginacao, filtro por coluna e o cabecalho escuro com
/// separadores do kit. Os dados ficam todos na memoria: filtra, ordena e pagina no proprio componente.
/// </summary>
[CascadingTypeParameter(nameof(TItem))]
public class RvmDataGrid<TItem> : RvmTable<TItem>
{
    private int _pagina = 1;
    private int _tamanho = 10;
    private int? _paginaRecebida;
    private int? _tamanhoRecebido;

    /// <summary>A pagina mostrada, comecando em 1. Aceita <c>@bind-Page</c>.</summary>
    [Parameter] public int Page { get; set; } = 1;

    /// <summary>Disparado quando a pagina muda.</summary>
    [Parameter] public EventCallback<int> PageChanged { get; set; }

    /// <summary>Linhas por pagina. Aceita <c>@bind-PageSize</c>. Padrao: 10.</summary>
    [Parameter] public int PageSize { get; set; } = 10;

    /// <summary>Disparado quando a pessoa troca as linhas por pagina.</summary>
    [Parameter] public EventCallback<int> PageSizeChanged { get; set; }

    /// <summary>As escolhas de "Linhas por pagina". Padrao: 5, 10 e 25.</summary>
    [Parameter] public IReadOnlyList<int> PageSizeOptions { get; set; } = [5, 10, 25];

    /// <summary>Texto quando o filtro nao casa com nenhuma linha.</summary>
    [Parameter] public string NoResultsText { get; set; } = "Nenhum registro encontrado para esse filtro.";

    private protected override bool EhGrade => true;

    private protected override int? TamanhoDaPagina => _tamanho;

    // Pagina fora da faixa (os dados encolheram, o filtro cortou linhas, Page grande demais) mostra a
    // ultima que existe, em vez de uma tabela vazia com "11–10 de 10".
    private protected override int PaginaPara(int total)
        => Math.Clamp(_pagina, 1, Math.Max(1, (int)Math.Ceiling(total / (double)_tamanho)));

    private protected override IReadOnlyList<int> OpcoesDeTamanho
        => PageSizeOptions.Contains(_tamanho) ? PageSizeOptions : [.. PageSizeOptions.Append(_tamanho).Order()];

    private protected override string TextoSemLinhas
        => Filtros.Values.Any(f => !string.IsNullOrWhiteSpace(f)) && Items?.Any() == true ? NoResultsText : NoRecordsText;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        // So adota o valor de fora quando ele MUDOU: sem @bind, um novo render da pagina mandaria o
        // Page=1 de sempre e jogaria a pessoa de volta para a primeira pagina a cada clique.
        if (PageSize != _tamanhoRecebido)
        {
            _tamanhoRecebido = PageSize;
            _tamanho = Math.Max(1, PageSize);
        }

        if (Page != _paginaRecebida)
        {
            _paginaRecebida = Page;
            _pagina = Page;
        }
    }

    private protected override async Task IrParaPaginaAsync(int pagina)
    {
        _pagina = pagina;
        StateHasChanged();
        await PageChanged.InvokeAsync(pagina);
    }

    private protected override async Task MudarTamanhoAsync(string? valor)
    {
        if (!int.TryParse(valor, NumberStyles.Integer, CultureInfo.InvariantCulture, out var tamanho) || tamanho < 1)
        {
            return;
        }

        _tamanho = tamanho;
        await PageSizeChanged.InvokeAsync(tamanho);
        await IrParaPaginaAsync(1);
    }

    private protected override async Task FiltrarAsync(RvmTableColumn<TItem> coluna, string? texto)
    {
        Filtros[coluna] = texto ?? "";
        await AoMudarOsDadosAsync();
    }

    private protected override Task AoMudarOsDadosAsync() => _pagina == 1 ? Task.CompletedTask : IrParaPaginaAsync(1);
}
