using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Stepper;

/// <summary>
/// As etapas de um processo e em qual a pessoa esta: cadastro em partes, assistente de configuracao.
/// </summary>
public partial class RvmStepper : ComponentBase
{
    /// <summary>As etapas, em ordem. Sem elas, o stepper vira o indicador de bolinhas de <see cref="Count"/>.</summary>
    [Parameter] public IReadOnlyList<RvmStep> Steps { get; set; } = [];

    /// <summary>Quantos passos existem, no indicador de bolinhas (sem <see cref="Steps"/>).</summary>
    [Parameter] public int Count { get; set; } = 1;

    /// <summary>Passo atual, contando de 1, no indicador de bolinhas.</summary>
    [Parameter] public int Current { get; set; } = 1;

    /// <summary>Nome do indicador de bolinhas para o leitor de tela. Nas etapas, o nome e o <see cref="AriaLabel"/>.</summary>
    [Parameter] public string Label { get; set; } = "Progresso";

    internal bool SoBolinhas => Steps.Count == 0;

    internal int AtualNasBolinhas => Math.Clamp(Current, 1, Math.Max(1, Count));

    /// <summary>Indice da etapa atual, a partir de 0. As anteriores contam como concluidas.</summary>
    [Parameter] public int ActiveStep { get; set; }

    /// <summary>Etapas em linha (padrao) ou empilhadas.</summary>
    [Parameter] public RvmOrientation Orientation { get; set; } = RvmOrientation.Horizontal;

    /// <summary>Texto ao lado com o numero grande (padrao) ou embaixo, centralizado.</summary>
    [Parameter] public RvmStepperLabelPlacement Placement { get; set; } = RvmStepperLabelPlacement.End;

    /// <summary>Nome da lista para o leitor de tela. Padrao: "Etapas".</summary>
    [Parameter] public string AriaLabel { get; set; } = "Etapas";

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

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

            return ClassesCss.Juntar(proprias, Class, AdditionalAttributes);
        }
    }
}
