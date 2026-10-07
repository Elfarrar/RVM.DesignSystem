namespace RVM.DesignSystem.Theming;

/// <summary>
/// Tamanho da fonte escolhido pelo usuario. Muda o tamanho base do documento, entao todo valor em <c>rem</c>
/// (texto e espacamento) acompanha, como no ajuste de fonte do proprio navegador. Contrato com o RVM.UI (DSGN-017).
/// </summary>
public enum RvmFontScale
{
    /// <summary>87,5% (14 px sobre 16).</summary>
    Small,

    /// <summary>100%, o do kit. Padrao.</summary>
    Default,

    /// <summary>112,5% (18 px sobre 16).</summary>
    Large,

    /// <summary>125% (20 px sobre 16).</summary>
    ExtraLarge
}
