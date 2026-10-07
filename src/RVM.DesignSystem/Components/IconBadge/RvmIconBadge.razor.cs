using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.IconBadge;

/// <summary>
/// Um icone num quadrado de cantos arredondados com fundo do papel de cor — o dos cards de estatistica do kit.
/// Decorativo por padrao; com <see cref="Label"/>, vira imagem com nome acessivel.
/// </summary>
public partial class RvmIconBadge : ComponentBase
{
    /// <summary>Qual icone.</summary>
    [Parameter, EditorRequired] public RvmIconName Icon { get; set; }

    /// <summary>Desenho do icone. Padrao: <see cref="RvmIconStyle.BoldDuotone"/> (cheio, quando o Tabler tem).</summary>
    [Parameter] public RvmIconStyle IconStyle { get; set; } = RvmIconStyle.BoldDuotone;

    /// <summary>Suave (padrao), cheio ou fundo cinza neutro.</summary>
    [Parameter] public RvmIconBadgeVariant Variant { get; set; } = RvmIconBadgeVariant.Soft;

    /// <summary>Papel de cor. Padrao: <see cref="RvmColor.Accent"/>.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Accent;

    /// <summary>Lado: 32, 40, 48 (padrao) ou 56 px.</summary>
    [Parameter] public RvmIconBadgeSize Size { get; set; } = RvmIconBadgeSize.Large;

    /// <summary>
    /// O que o badge significa, para o leitor de tela. Preencha so quando o badge for a unica fonte da informacao;
    /// ao lado de um texto que ja diz o mesmo, deixe vazio e ele fica decorativo.
    /// </summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <inheritdoc cref="ComponentBase" />
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal bool TemNome => !string.IsNullOrWhiteSpace(Label);

    internal int LadoDoIcone => Size switch
    {
        RvmIconBadgeSize.Small => 16,
        RvmIconBadgeSize.Medium => 20,
        RvmIconBadgeSize.XLarge => 28,
        _ => 24
    };

    internal string CssClass => ClassesCss.Juntar(
        string.Join(' ',
            "rvm-icon-badge",
            Variant switch
            {
                RvmIconBadgeVariant.Solid => "rvm-cheio",
                RvmIconBadgeVariant.Neutral => "rvm-neutro",
                _ => "rvm-suave"
            },
            Size switch
            {
                RvmIconBadgeSize.Small => "rvm-pequeno",
                RvmIconBadgeSize.Medium => "rvm-medio",
                RvmIconBadgeSize.XLarge => "rvm-extra",
                _ => "rvm-grande"
            },
            PapelCss.Classe(Color)),
        Class,
        AdditionalAttributes);
}
