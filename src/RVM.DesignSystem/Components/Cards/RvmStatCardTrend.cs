namespace RVM.DesignSystem.Components.Cards;

/// <summary>Sentido da variacao no <see cref="RvmStatCard"/> (contrato com o RVM.UI, DSGN-017).</summary>
public enum RvmStatCardTrend
{
    /// <summary>Sem variacao.</summary>
    None,

    /// <summary>Alta: seta para cima, na cor de sucesso, e "Alta de" para o leitor de tela.</summary>
    Up,

    /// <summary>Queda: seta para baixo, na cor de erro, e "Queda de" para o leitor de tela.</summary>
    Down
}
