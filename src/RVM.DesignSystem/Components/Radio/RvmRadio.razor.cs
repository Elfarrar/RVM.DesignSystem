using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Radio;

/// <summary>Uma opcao de um <see cref="RvmRadioGroup{TValue}"/>.</summary>
/// <typeparam name="TValue">O mesmo tipo do grupo.</typeparam>
public partial class RvmRadio<TValue> : ComponentBase
{
    /// <summary>O grupo em volta. Sem ele o componente nao tem como funcionar.</summary>
    [CascadingParameter] public RvmRadioGroup<TValue>? Grupo { get; set; }

    /// <summary>O valor que esta opcao representa.</summary>
    [Parameter, EditorRequired] public TValue? Value { get; set; }

    /// <summary>Texto da opcao. Tambem e o nome acessivel.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Rotulo livre, quando <see cref="Label"/> esta vazio.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Atributos extras, repassados ao input nativo.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string ClassesDaRaiz
    {
        get
        {
            var tamanho = Grupo?.Size ?? RvmSize.Medium;
            var cor = Grupo?.Color ?? RvmColor.Primary;
            var classes = string.Join(' ',
                "rvm-controle rvm-radio",
                tamanho switch { RvmSize.Small => "rvm-pequeno", RvmSize.Large => "rvm-grande", _ => "rvm-medio" },
                cor switch
                {
                    RvmColor.Secondary => "rvm-secondary",
                    RvmColor.Info => "rvm-info",
                    RvmColor.Success => "rvm-success",
                    RvmColor.Warning => "rvm-warning",
                    RvmColor.Error => "rvm-error",
                    _ => "rvm-primary"
                });

            return Grupo?.Disabled == true ? classes + " rvm-desabilitado" : classes;
        }
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        // Falha cedo e em portugues, em vez do erro generico do InputRadio sobre "InputRadioGroup".
        if (Grupo is null)
        {
            // Armadilha comum: grupo ligado a um valor anulavel (`Opcao?`) com opcoes nao anulaveis
            // (`Opcao.A`) — os tipos genericos nao casam e a cascata nao encontra o grupo.
            throw new InvalidOperationException(
                $"O RvmRadio<{typeof(TValue).Name}> precisa estar dentro de um RvmRadioGroup do MESMO tipo. "
                + "Se o grupo esta ligado a um valor anulavel, informe TValue nas opcoes (TValue=\"Opcao?\").");
        }
    }
}
