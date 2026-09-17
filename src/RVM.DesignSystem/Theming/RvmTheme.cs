namespace RVM.DesignSystem.Theming;

/// <summary>
/// Os dois temas do design system. Enum, nunca string magica: o valor entra em atributo HTML e em
/// armazenamento local, e um typo em string so apareceria na tela do usuario.
/// </summary>
public enum RvmTheme
{
    /// <summary>Tema claro — corpo <c>#F4F5FA</c>, papel <c>#FFFFFF</c>.</summary>
    Light,

    /// <summary>Tema escuro — corpo <c>#28243D</c>, papel <c>#312D4B</c>.</summary>
    Dark
}
