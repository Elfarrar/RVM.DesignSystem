using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Stepper;

/// <summary>
/// As etapas de um processo e em qual a pessoa esta: cadastro em partes, assistente de configuracao.
/// </summary>
public partial class RvmStepper : ComponentBase
{
    /// <summary>As etapas, em ordem.</summary>
    [Parameter, EditorRequired] public IReadOnlyList<RvmStep> Steps { get; set; } = [];

    /// <summary>Indice da etapa atual, a partir de 0. As anteriores contam como concluidas.</summary>
    [Parameter] public int ActiveStep { get; set; }

    /// <summary>Etapas em linha (padrao) ou empilhadas.</summary>
    [Parameter] public RvmOrientation Orientation { get; set; } = RvmOrientation.Horizontal;

    /// <summary>Texto ao lado com o numero grande (padrao) ou embaixo, centralizado.</summary>
    [Parameter] public RvmStepperLabelPlacement Placement { get; set; } = RvmStepperLabelPlacement.End;

    /// <summary>Nome da lista para o leitor de tela. Padrao: "Etapas".</summary>
    [Parameter] public string AriaLabel { get; set; } = "Etapas";

    /// <summary>Atributos extras, repassados ao <c>ol</c>.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string EstadoDe(int indice, RvmStep etapa)
        => etapa.HasError ? "erro" : indice < ActiveStep ? "concluida" : indice == ActiveStep ? "atual" : "pendente";

    internal static string TextoDoEstado(string estado) => estado switch
    {
        "concluida" => "concluida",
        "atual" => "etapa atual",
        "erro" => "com erro, precisa de revisao",
        _ => "pendente"
    };

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = string.Join(' ',
                "rvm-etapas",
                Orientation == RvmOrientation.Vertical ? "rvm-vertical" : "rvm-horizontal",
                Placement == RvmStepperLabelPlacement.Bottom ? "rvm-texto-embaixo" : "rvm-texto-ao-lado");

            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }
}
