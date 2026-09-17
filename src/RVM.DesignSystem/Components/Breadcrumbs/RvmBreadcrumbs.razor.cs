using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Breadcrumbs;

/// <summary>
/// Trilha de navegacao: onde a pessoa esta dentro da hierarquia, com atalho para cada nivel acima.
/// O ultimo passo e a pagina atual.
/// </summary>
public partial class RvmBreadcrumbs : ComponentBase
{
    /// <summary>Os passos, do mais alto ao atual.</summary>
    [Parameter, EditorRequired] public IReadOnlyList<RvmBreadcrumbItem> Items { get; set; } = [];

    /// <summary>Barra ou seta entre os passos. Padrao: <see cref="RvmBreadcrumbSeparator.Slash"/>.</summary>
    [Parameter] public RvmBreadcrumbSeparator Separator { get; set; } = RvmBreadcrumbSeparator.Slash;

    /// <summary>Nome da regiao para o leitor de tela. Padrao: "Trilha de navegacao".</summary>
    [Parameter] public string AriaLabel { get; set; } = "Trilha de navegacao";

    /// <summary>Atributos extras, repassados ao <c>nav</c>.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string ClassesDaRaiz
        => AdditionalAttributes is not null
           && AdditionalAttributes.TryGetValue("class", out var informada)
           && informada is string texto
           && !string.IsNullOrWhiteSpace(texto)
            ? $"trilha {texto}"
            : "trilha";
}
