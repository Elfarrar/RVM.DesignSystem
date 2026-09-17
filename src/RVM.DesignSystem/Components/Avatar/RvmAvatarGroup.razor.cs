using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Avatar;

/// <summary>
/// Avatares sobrepostos, como na linha "Grouped Avatars" do kit. O "+N" do fim diz quantos ficaram
/// de fora.
/// </summary>
public partial class RvmAvatarGroup : ComponentBase
{
    /// <summary>Os avatares visiveis. Use o mesmo <see cref="Size"/> neles.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Quantos ficaram de fora. Acima de zero, aparece um avatar "+N" no fim.</summary>
    [Parameter] public int Surplus { get; set; }

    /// <summary>Tamanho do avatar "+N". Padrao: <see cref="RvmSize.Medium"/>.</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>O que o grupo representa, para o leitor de tela ("Participantes da reuniao").</summary>
    [Parameter] public string? Label { get; set; }

    /// <inheritdoc cref="ComponentBase" />
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string NomeDoGrupo => string.IsNullOrWhiteSpace(Label) ? "Grupo de pessoas" : Label;

    internal string CssClass
    {
        get
        {
            const string propria = "rvm-grupo";
            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{propria} {texto}"
                : propria;
        }
    }
}
