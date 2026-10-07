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

    /// <summary>As pessoas do grupo, como lista. Ignorado quando ha <see cref="ChildContent"/>.</summary>
    [Parameter] public IReadOnlyList<RvmAvatarItem> Items { get; set; } = [];

    /// <summary>Quantas pessoas de <see cref="Items"/> aparecem antes do "+N". Padrao: 4.</summary>
    [Parameter] public int Max { get; set; } = 4;

    internal bool PorLista => ChildContent is null && Items.Count > 0;

    internal IEnumerable<RvmAvatarItem> Visiveis => Items.Take(Math.Max(1, Max));

    internal int Sobra => PorLista ? Math.Max(0, Items.Count - Math.Max(1, Max)) + Surplus : Surplus;

    internal static string IniciaisDe(string nome)
        => string.Concat(nome.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(p => char.ToUpperInvariant(p[0])));

    /// <summary>Tamanho do avatar "+N". Padrao: <see cref="RvmSize.Medium"/>.</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>O que o grupo representa, para o leitor de tela ("Participantes da reuniao").</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <inheritdoc cref="ComponentBase" />
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string NomeDoGrupo => string.IsNullOrWhiteSpace(Label) ? "Grupo de pessoas" : Label;

    internal string CssClass
    {
        get
        {
            const string propria = "rvm-grupo";
            return ClassesCss.Juntar(propria, Class, AdditionalAttributes);
        }
    }
}
