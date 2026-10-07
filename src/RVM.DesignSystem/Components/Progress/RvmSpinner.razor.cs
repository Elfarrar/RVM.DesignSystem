using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Progress;

/// <summary>
/// "Esta carregando", sem previsao: o anel indeterminado com um texto para o leitor de tela
/// (<c>role="status"</c>). Do contrato com o RVM.UI (DSGN-017).
/// </summary>
public partial class RvmSpinner : ComponentBase
{
    /// <summary>Anel de 24, 40 (padrao) ou 56 px — os tamanhos do <see cref="RvmProgress"/> circular.</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>O que esta carregando ("Carregando talhoes"). Vai para o leitor de tela sempre, e para a tela com <see cref="ShowLabel"/>.</summary>
    [Parameter] public string Label { get; set; } = "Carregando";

    /// <summary>Mostra o texto ao lado do anel. Sem ele, o texto so vai para o leitor de tela.</summary>
    [Parameter] public bool ShowLabel { get; set; }

    /// <summary>Ocupa a linha, centralizado e com respiro em cima e embaixo — o lugar de um carregamento de pagina.</summary>
    [Parameter] public bool Centered { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos extras, repassados a raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string ClassesDaRaiz
        => ClassesCss.Juntar(Centered ? "rvm-spinner rvm-centralizado" : "rvm-spinner", Class, AdditionalAttributes);
}
