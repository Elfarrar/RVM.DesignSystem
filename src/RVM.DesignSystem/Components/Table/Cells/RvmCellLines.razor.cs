using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Table;

/// <summary>Titulo e segunda linha de uma celula da tabela — a peca de texto que as <c>RvmCell*</c> usam por baixo.</summary>
public partial class RvmCellLines : ComponentBase
{
    /// <summary>O titulo.</summary>
    [Parameter] public string? Text { get; set; }

    /// <summary>Conteudo livre no lugar do titulo.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>A segunda linha, menor e mais clara.</summary>
    [Parameter] public string? Supporting { get; set; }

    /// <summary>Destaque: o titulo mais pesado.</summary>
    [Parameter] public bool Highlight { get; set; }
}
