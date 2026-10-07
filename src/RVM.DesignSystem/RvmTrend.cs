namespace RVM.DesignSystem;

/// <summary>Para onde um numero andou (contrato com o RVM.UI, DSGN-017). A cor vem daqui.</summary>
public enum RvmTrend
{
    /// <summary>So o numero.</summary>
    None,

    /// <summary>Subiu: seta para cima, na cor de sucesso.</summary>
    Up,

    /// <summary>Caiu: seta para baixo, na cor de erro.</summary>
    Down
}
