namespace RVM.DesignSystem.Components.Card;

/// <summary>Aparencia da superficie do <see cref="RvmCard"/> (contrato com o RVM.UI, DSGN-017).</summary>
public enum RvmCardVariant
{
    /// <summary>Papel com a sombra do kit — o padrao.</summary>
    Elevated,

    /// <summary>Papel com borda no divisor, sem sombra.</summary>
    Outlined,

    /// <summary>Fundo neutro claro, sem borda nem sombra: bloco dentro de outro cartao.</summary>
    Flat
}
