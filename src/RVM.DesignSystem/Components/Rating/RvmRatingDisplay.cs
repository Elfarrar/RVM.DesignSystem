namespace RVM.DesignSystem.Components.Rating;

/// <summary>Forma da nota so mostrada (contrato com o RVM.UI, DSGN-017). Editavel, e sempre a de estrelas.</summary>
public enum RvmRatingDisplay
{
    /// <summary>Uma estrela e a nota escrita ("4,5"): cabe numa celula de tabela.</summary>
    Compact,

    /// <summary>Uma estrela por ponto da nota — o padrao.</summary>
    Stars
}
