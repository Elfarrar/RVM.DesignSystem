using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Chart;

/// <summary>
/// Uma serie de um grafico: o nome na legenda, o valor de cada item e a cor. Nao desenha nada sozinha;
/// so se apresenta ao grafico em que foi declarada.
/// </summary>
public sealed class RvmChartSeries<TItem> : ComponentBase, IDisposable
{
    [CascadingParameter] private RvmChartBase<TItem>? Grafico { get; set; }

    /// <summary>Nome na legenda, na dica e na tabela de dados.</summary>
    [Parameter, EditorRequired] public string Name { get; set; } = "";

    /// <summary>O valor de cada item (no grafico de dispersao, o Y).</summary>
    [Parameter, EditorRequired] public Func<TItem, double>? Value { get; set; }

    /// <summary>So no grafico de dispersao: o X de cada item.</summary>
    [Parameter] public Func<TItem, double>? X { get; set; }

    /// <summary>Papel de cor. Sem valor, segue a ordem: primary, success, warning, info, error, secondary.</summary>
    [Parameter] public RvmColor? Color { get; set; }

    internal double ValorDe(TItem item) => Value?.Invoke(item) ?? 0;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (Grafico is null)
        {
            throw new InvalidOperationException(
                $"{nameof(RvmChartSeries<TItem>)} precisa estar dentro de um grafico (RvmColumnChart, RvmLineChart...).");
        }

        Grafico.AdicionarSerie(this);
    }

    /// <inheritdoc />
    public void Dispose() => Grafico?.RemoverSerie(this);
}
