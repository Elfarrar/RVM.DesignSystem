namespace RVM.DesignSystem.Components.Tooltip;

/// <summary>
/// Lado em que um elemento flutuante (o balao do <c>RvmMiniCalendar</c>) aparece. Do contrato com o RVM.UI
/// (DSGN-017); o <see cref="RvmTooltip"/> continua com o proprio <see cref="RvmTooltipPlacement"/>.
/// </summary>
public enum RvmPlacement
{
    /// <summary>Acima.</summary>
    Top,

    /// <summary>Abaixo.</summary>
    Bottom,

    /// <summary>A esquerda.</summary>
    Left,

    /// <summary>A direita.</summary>
    Right
}
