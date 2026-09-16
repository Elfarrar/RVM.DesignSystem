namespace RVM.DesignSystem.Components.Typography;

/// <summary>
/// Os 24 estilos da escala tipografica do kit (`06-tokens-e-tematizacao.md`). Cada um vira a classe
/// `rvm-text-*` correspondente, que le tamanho, entrelinha e peso dos tokens.
/// </summary>
public enum RvmTypographyVariant
{
    /// <summary>96/112/300.</summary>
    H1,

    /// <summary>60/72/300.</summary>
    H2,

    /// <summary>48/56/400.</summary>
    H3,

    /// <summary>34/42/400.</summary>
    H4,

    /// <summary>24/32/400.</summary>
    H5,

    /// <summary>20/32/500.</summary>
    H6,

    /// <summary>16/28/400.</summary>
    Subtitle1,

    /// <summary>14/22/500.</summary>
    Subtitle2,

    /// <summary>14/24/400 — o texto corrido padrao.</summary>
    Body1,

    /// <summary>14/20/400 — texto corrido mais apertado.</summary>
    Body2,

    /// <summary>12/20/400 — legenda.</summary>
    Caption,

    /// <summary>12/32/400, caixa alta — rotulo acima de um bloco.</summary>
    Overline,

    /// <summary>15/26/500.</summary>
    ButtonLarge,

    /// <summary>14/24/500.</summary>
    ButtonMedium,

    /// <summary>13/22/500.</summary>
    ButtonSmall,

    /// <summary>12/12/400 — rotulo de campo.</summary>
    InputLabel,

    /// <summary>12/20/400 — texto de apoio abaixo do campo.</summary>
    Helper,

    /// <summary>16/24/400 — o que o usuario digita.</summary>
    Input,

    /// <summary>20/20/400 — iniciais dentro do avatar.</summary>
    AvatarInitials,

    /// <summary>13/18/400.</summary>
    Chip,

    /// <summary>10/14/500.</summary>
    Tooltip,

    /// <summary>16/24/500.</summary>
    AlertTitle,

    /// <summary>14/24/500 — cabecalho de tabela.</summary>
    TableHeader,

    /// <summary>12/20/500 — rotulo de badge.</summary>
    BadgeLabel
}
