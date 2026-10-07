using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Label;

/// <summary>
/// Etiqueta de texto com fundo na cor de um papel — o status de uma linha ("Pago", "Em atraso",
/// "Colhido"). Nao e o <c>&lt;label&gt;</c> de formulario: para rotulo de campo, use o <c>Label</c> do
/// proprio campo. Do contrato com o RVM.UI (DSGN-017).
/// </summary>
public partial class RvmLabel : ComponentBase
{
    /// <summary>Suave (padrao) ou cheia.</summary>
    [Parameter] public RvmLabelVariant Variant { get; set; } = RvmLabelVariant.Soft;

    /// <summary>Papel de cor. Padrao: <see cref="RvmColor.Accent"/>.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Accent;

    /// <summary>Altura de 24, 28 (padrao) ou 32 px.</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>O texto da etiqueta.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos extras, repassados a raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = string.Join(' ',
                "rvm-etiqueta",
                Variant == RvmLabelVariant.Solid ? "rvm-cheia" : "rvm-suave",
                Size switch { RvmSize.Small => "rvm-pequena", RvmSize.Large => "rvm-grande", _ => "rvm-media" },
                PapelCss.Classe(Color));

            return ClassesCss.Juntar(proprias, Class, AdditionalAttributes);
        }
    }
}
