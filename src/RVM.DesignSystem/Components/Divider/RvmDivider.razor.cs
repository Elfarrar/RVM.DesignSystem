using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Divider;

/// <summary>
/// Linha que separa blocos de conteudo. Com <see cref="ChildContent"/>, a linha abre espaco para um
/// rotulo no meio ("ou", "ontem", "mais opcoes").
/// </summary>
public partial class RvmDivider : ComponentBase
{
    /// <summary>Deitado (padrao) ou em pe. O rotulo so vale no deitado.</summary>
    [Parameter] public RvmOrientation Orientation { get; set; } = RvmOrientation.Horizontal;

    /// <summary>Rotulo no meio da linha. Sem ele, sai um <c>&lt;hr&gt;</c> simples.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc cref="ComponentBase" />
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string AriaOrientation
        => Orientation == RvmOrientation.Vertical ? "vertical" : "horizontal";

    internal string CssClass
    {
        get
        {
            var proprias = ChildContent is not null
                ? "divisor com-texto"
                : Orientation == RvmOrientation.Vertical
                    ? "divisor vertical"
                    : "divisor horizontal";

            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }
}
