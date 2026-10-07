namespace RVM.DesignSystem.Theming;

/// <summary>Catalogo das paletas que o DS ja traz. Contrato com o RVM.UI (DSGN-017).</summary>
public static class RvmPalettes
{
    /// <summary>Cobalto do kit NEATLAB. A paleta padrao do DS.</summary>
    public static readonly RvmPalette Blue = new("blue", "Azul", "O cobalto do kit, destaque padrao.");

    /// <summary>Violeta da identidade RVM.</summary>
    public static readonly RvmPalette Purple = new("purple", "Roxo", "O violeta da marca RVM.");

    /// <summary>Neutro escuro. No tema escuro o destaque inverte e fica claro.</summary>
    public static readonly RvmPalette Black = new("black", "Preto", "Sobrio, sem cor de destaque.");

    /// <summary>Roxo profundo com Hanken Grotesk (paleta de produto, veio do ERPAgro).</summary>
    public static readonly RvmPalette Profissional = new("profissional", "Profissional", "Roxo profundo, letra limpa e objetiva.");

    /// <summary>Terra com Lora, serifada (paleta de produto, veio do ERPAgro).</summary>
    public static readonly RvmPalette Acolhedor = new("acolhedor", "Acolhedor", "Tons de terra e letra com serifa.");

    /// <summary>Verde campo com Geist Mono, de largura fixa (paleta de produto, veio do ERPAgro).</summary>
    public static readonly RvmPalette Utilitario = new("utilitario", "Utilitario", "Verde campo e letra de largura fixa, mais densa.");

    /// <summary>As tres de destaque puro: azul, roxo e preto.</summary>
    public static IReadOnlyList<RvmPalette> Kit { get; } = [Blue, Purple, Black];

    /// <summary>As tres de produto, com cor e fonte.</summary>
    public static IReadOnlyList<RvmPalette> Produto { get; } = [Profissional, Acolhedor, Utilitario];

    /// <summary>Todas. E o padrao do <see cref="RvmThemePicker.Palettes"/>.</summary>
    public static IReadOnlyList<RvmPalette> All { get; } = [.. Kit, .. Produto];
}
