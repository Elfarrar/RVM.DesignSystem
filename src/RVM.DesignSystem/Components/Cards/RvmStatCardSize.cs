namespace RVM.DesignSystem.Components.Cards;

/// <summary>Tamanho do <see cref="RvmStatCard"/> (contrato com o RVM.UI, DSGN-017).</summary>
public enum RvmStatCardSize
{
    /// <summary>Compacto: o numero em tamanho de titulo pequeno. O padrao.</summary>
    Small,

    /// <summary>Numero maior e mais respiro, para o destaque de uma linha do painel.</summary>
    Large,

    /// <summary>O maior, com altura minima para receber um grafico abaixo do numero.</summary>
    XtraLarge
}
