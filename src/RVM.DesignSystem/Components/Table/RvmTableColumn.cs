using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Table;

/// <summary>
/// Uma coluna da <see cref="RvmTable{TItem}"/> ou da <see cref="RvmDataGrid{TItem}"/>. Nao desenha
/// nada sozinha: so se apresenta a tabela em que foi declarada.
/// </summary>
public sealed class RvmTableColumn<TItem> : ComponentBase, IDisposable
{
    [CascadingParameter] private RvmTable<TItem>? Tabela { get; set; }

    /// <summary>Titulo no cabecalho.</summary>
    [Parameter, EditorRequired] public string Title { get; set; } = "";

    /// <summary>
    /// O valor da celula. E o que aparece (quando nao ha <see cref="ChildContent"/>), o que ordena e o
    /// que o filtro procura.
    /// </summary>
    [Parameter] public Func<TItem, object?>? Value { get; set; }

    /// <summary>Formato do valor, quando ele e formatavel (<c>"dd/MM/yyyy"</c>, <c>"N2"</c>).</summary>
    [Parameter] public string? Format { get; set; }

    /// <summary>Conteudo livre da celula (avatar, chip, botao). O item chega como <c>context</c>.</summary>
    [Parameter] public RenderFragment<TItem>? ChildContent { get; set; }

    /// <summary>Ordena pelo <see cref="Value"/> ao clicar no titulo.</summary>
    [Parameter] public bool Sortable { get; set; }

    /// <summary>Ordenacao aplicada ao abrir. Vale para uma coluna so.</summary>
    [Parameter] public RvmSortDirection? InitialSort { get; set; }

    /// <summary>Ganha campo de filtro na <see cref="RvmDataGrid{TItem}"/>. Padrao: sim, quando ha <see cref="Value"/>.</summary>
    [Parameter] public bool Filterable { get; set; } = true;

    /// <summary>Alinhamento. Padrao: <see cref="RvmTableAlign.Start"/>.</summary>
    [Parameter] public RvmTableAlign Align { get; set; } = RvmTableAlign.Start;

    /// <summary>Largura em CSS (<c>"120px"</c>, <c>"20%"</c>). Sem valor, a tabela distribui.</summary>
    [Parameter] public string? Width { get; set; }

    internal bool Ordenavel => Sortable && Value is not null;

    internal bool Filtravel => Filterable && Value is not null;

    /// <summary>O valor como texto, no formato pedido e na cultura corrente — o que a pessoa le.</summary>
    internal string TextoDe(TItem item)
        => Value?.Invoke(item) switch
        {
            null => "",
            IFormattable f when Format is not null => f.ToString(Format, CultureInfo.CurrentCulture),
            var v => Convert.ToString(v, CultureInfo.CurrentCulture) ?? ""
        };

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (Tabela is null)
        {
            throw new InvalidOperationException(
                $"{nameof(RvmTableColumn<TItem>)} precisa estar dentro de uma RvmTable ou RvmDataGrid.");
        }

        Tabela.AdicionarColuna(this);
    }

    /// <inheritdoc />
    public void Dispose() => Tabela?.RemoverColuna(this);
}
