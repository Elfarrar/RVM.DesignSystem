using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Chip;

/// <summary>
/// Rotulo compacto: um filtro aplicado, uma etiqueta, uma pessoa num campo de destinatarios.
/// Removivel quando <see cref="OnRemove"/> tem quem escute.
/// </summary>
public partial class RvmChip : ComponentBase
{
    /// <summary>Estilo. Padrao: <see cref="RvmChipVariant.Filled"/>.</summary>
    [Parameter] public RvmChipVariant Variant { get; set; } = RvmChipVariant.Filled;

    /// <summary>Papel de cor. Sem valor, o chip e neutro — a coluna "Default" do kit.</summary>
    [Parameter] public RvmColor? Color { get; set; }

    /// <summary>
    /// 24 px (<see cref="RvmSize.Small"/>) ou 32 px — medidos no kit. O kit nao define um chip
    /// grande, entao <see cref="RvmSize.Large"/> sai igual ao medio.
    /// </summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>Icone antes do rotulo.</summary>
    [Parameter] public RvmIconName? StartIcon { get; set; }

    /// <summary>Miniatura de foto antes do rotulo (a variacao "Thumbnail" do kit). Vence o icone.</summary>
    [Parameter] public string? AvatarSrc { get; set; }

    /// <summary>Texto alternativo da miniatura. Vazio quando o rotulo ja diz quem e.</summary>
    [Parameter] public string? AvatarAlt { get; set; }

    /// <summary>Chamado ao remover. Com alguem escutando, aparece o botao de remover.</summary>
    [Parameter] public EventCallback OnRemove { get; set; }

    /// <summary>
    /// Nome acessivel do botao de remover. Padrao: "Remover". Vale personalizar com o rotulo
    /// ("Remover filtro de status") quando houver varios chips lado a lado.
    /// </summary>
    [Parameter] public string RemoveLabel { get; set; } = "Remover";

    /// <summary>Indisponivel: esmaecido e sem remover.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>O rotulo.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc cref="ComponentBase" />
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string CssClass
    {
        get
        {
            var proprias = string.Join(' ',
                "rvm-chip",
                Size == RvmSize.Small ? "rvm-pequeno" : "rvm-medio",
                Variant switch { RvmChipVariant.Outlined => "rvm-contorno", RvmChipVariant.Soft => "rvm-suave", _ => "rvm-preenchido" },
                Color switch
                {
                    RvmColor.Primary => "rvm-primary",
                    RvmColor.Secondary => "rvm-secondary",
                    RvmColor.Info => "rvm-info",
                    RvmColor.Success => "rvm-success",
                    RvmColor.Warning => "rvm-warning",
                    RvmColor.Error => "rvm-error",
                    _ => "rvm-neutro"
                });

            if (Disabled)
            {
                proprias += " rvm-desabilitado";
            }

            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }

    private async Task Remover()
    {
        if (Disabled)
        {
            return;
        }

        await OnRemove.InvokeAsync();
    }
}
