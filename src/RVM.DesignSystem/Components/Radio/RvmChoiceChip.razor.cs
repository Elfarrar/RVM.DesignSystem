using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Chip;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Radio;

/// <summary>
/// Opcao de escolha unica em forma de chip, dentro de um <see cref="RvmRadioGroup{TValue}"/>. O grupo da a
/// pergunta, o <c>name</c> e o <c>@bind-Value</c>; o chip e um radio nativo com cara de chip.
/// </summary>
/// <typeparam name="TValue">O mesmo tipo do grupo.</typeparam>
public partial class RvmChoiceChip<TValue> : ComponentBase
{
    /// <summary>O grupo em volta. Sem ele o componente nao tem como funcionar.</summary>
    [CascadingParameter] public RvmRadioGroup<TValue>? Grupo { get; set; }

    /// <summary>O valor que esta opcao representa.</summary>
    [Parameter, EditorRequired] public TValue? Value { get; set; }

    /// <summary>Texto da opcao, quando nao ha <see cref="ChildContent"/>. Tambem e o nome acessivel.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Conteudo livre da opcao. Vence o <see cref="Label"/>.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Estilo quando marcado. Padrao: <see cref="RvmChipVariant.Filled"/>. Desmarcado, o chip e sempre o
    /// contorno neutro.
    /// </summary>
    [Parameter] public RvmChipVariant Variant { get; set; } = RvmChipVariant.Filled;

    /// <summary>Papel de cor quando marcado. Sem valor, <see cref="RvmColor.Primary"/>.</summary>
    [Parameter] public RvmColor? Color { get; set; }

    /// <summary>24 px (<see cref="RvmSize.Small"/>) ou 32 px, como o RvmChip. <see cref="RvmSize.Large"/> sai igual ao medio.</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>Icone antes do rotulo.</summary>
    [Parameter] public RvmIconName? StartIcon { get; set; }

    /// <summary>Miniatura de foto antes do rotulo. Vence o icone.</summary>
    [Parameter] public string? AvatarSrc { get; set; }

    /// <summary>Texto alternativo da miniatura. Vazio quando o rotulo ja diz quem e.</summary>
    [Parameter] public string? AvatarAlt { get; set; }

    /// <summary>Chamado ao remover. Com alguem escutando, aparece o botao de remover ao lado do chip.</summary>
    [Parameter] public EventCallback OnRemove { get; set; }

    /// <summary>Nome acessivel do botao de remover. Padrao: "Remover".</summary>
    [Parameter] public string? RemoveLabel { get; set; }

    // Varios chips lado a lado: "Remover" sozinho nao diz qual (achado do review da onda 2a).
    internal string NomeDoRemover => RemoveLabel ?? (string.IsNullOrWhiteSpace(Label) ? "Remover" : $"Remover {Label}");

    /// <summary>So esta opcao indisponivel: desabilita o radio e o botao de remover. O <c>Disabled</c> do grupo desabilita todas.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos extras, repassados ao input nativo (menos <c>class</c> e <c>style</c>, que ficam na raiz).</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal bool Desabilitado => Disabled || Grupo?.Disabled == true;

    internal IReadOnlyDictionary<string, object>? AtributosDoInput
        => AtributosDoControle.Montar(AdditionalAttributes, null, null, null, false, false);

    internal string ClassesDaRaiz
    {
        get
        {
            var classes = string.Join(' ',
                "rvm-chip-escolha",
                Size == RvmSize.Small ? "rvm-pequeno" : "rvm-medio",
                Variant switch { RvmChipVariant.Outlined => "rvm-contorno", RvmChipVariant.Soft => "rvm-suave", _ => "rvm-preenchido" },
                PapelCss.Classe(Color ?? RvmColor.Primary));

            if (Desabilitado) classes += " rvm-desabilitado";
            if (OnRemove.HasDelegate) classes += " rvm-removivel";
            return ClassesCss.Juntar(classes, Class, AdditionalAttributes);
        }
    }

    private async Task Remover()
    {
        if (Desabilitado)
        {
            return;
        }

        await OnRemove.InvokeAsync();
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        // Falha cedo e em portugues, em vez do erro generico do InputRadio sobre "InputRadioGroup".
        if (Grupo is null)
        {
            throw new InvalidOperationException(
                $"O RvmChoiceChip<{typeof(TValue).Name}> precisa estar dentro de um RvmRadioGroup do MESMO tipo. "
                + "Se o grupo esta ligado a um valor anulavel, informe TValue nas opcoes (TValue=\"Opcao?\").");
        }
    }
}
