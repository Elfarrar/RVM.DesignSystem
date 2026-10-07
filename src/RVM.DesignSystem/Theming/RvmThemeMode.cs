namespace RVM.DesignSystem.Theming;

/// <summary>Aparencia escolhida pelo usuario no <see cref="RvmThemePicker"/>. Contrato com o RVM.UI (DSGN-017).</summary>
public enum RvmThemeMode
{
    /// <summary>Claro. Padrao.</summary>
    Light,

    /// <summary>Escuro.</summary>
    Dark,

    /// <summary>Acompanha o sistema operacional (<c>prefers-color-scheme</c>), resolvido no CSS, sem JavaScript.</summary>
    System
}
