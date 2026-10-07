using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.IconBadge;

/// <summary>
/// A versao de destaque do <see cref="RvmIconBadge"/>: circulo de 56 px com degrade do papel ou brilho na cor dele,
/// para abrir um card ou uma secao. Decorativo por padrao; com <see cref="Label"/>, vira imagem com nome acessivel.
/// </summary>
public partial class RvmArtisticIconBadge : ComponentBase
{
    /// <summary>Qual icone. Sai cheio (quando o Tabler tem) e com 28 px.</summary>
    [Parameter, EditorRequired] public RvmIconName Icon { get; set; }

    /// <summary>Degrade (padrao) ou brilho.</summary>
    [Parameter] public RvmArtisticVariant Variant { get; set; } = RvmArtisticVariant.Gradient;

    /// <summary>Papel de cor. Padrao: <see cref="RvmColor.Accent"/>.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Accent;

    /// <summary>
    /// O que o badge significa, para o leitor de tela. Sem ele o badge e decorativo (<c>aria-hidden</c>).
    /// </summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <inheritdoc cref="ComponentBase" />
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal bool TemNome => !string.IsNullOrWhiteSpace(Label);

    internal string CssClass => ClassesCss.Juntar(
        string.Join(' ',
            "rvm-artistic-icon-badge",
            Variant == RvmArtisticVariant.Glow ? "rvm-brilho" : "rvm-degrade",
            PapelCss.Classe(Color)),
        Class,
        AdditionalAttributes);
}
