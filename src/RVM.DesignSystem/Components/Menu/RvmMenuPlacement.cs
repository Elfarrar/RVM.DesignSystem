namespace RVM.DesignSystem.Components.Menu;

/// <summary>Onde o menu abre em relacao ao gatilho (contrato com o RVM.UI, DSGN-017).</summary>
public enum RvmMenuPlacement
{
    /// <summary>Abaixo, alinhado pela direita.</summary>
    BottomEnd,

    /// <summary>Abaixo, alinhado pela esquerda — o padrao.</summary>
    BottomStart,

    /// <summary>Abaixo, centralizado.</summary>
    BottomCenter,

    /// <summary>Acima, alinhado pela direita.</summary>
    TopEnd,

    /// <summary>Acima, alinhado pela esquerda.</summary>
    TopStart
}
