using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Table;

/// <summary>
/// O <c>&lt;td&gt;</c> comum das celulas da tabela: alinhamento, espacamento e a fileira do conteudo. E a peca que as
/// <c>RvmCell*</c> usam por baixo; para montar uma celula, prefira uma delas.
/// </summary>
public partial class RvmCellFrame : ComponentBase
{
    /// <summary>Alinhamento do conteudo. Nulo na coluna de escolha, que tem so a largura da caixa.</summary>
    [Parameter] public RvmAlign? Align { get; set; }

    /// <summary>Classe da celula que usa a moldura (<c>rvm-celula-numero</c>, <c>rvm-celula-escolha</c>...).</summary>
    [Parameter] public string? FrameClass { get; set; }

    /// <summary>O conteudo da celula.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Classe CSS extra no <c>&lt;td&gt;</c>.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos extras no <c>&lt;td&gt;</c>.</summary>
    [Parameter] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string ClassesDaRaiz
    {
        get
        {
            var alinhamento = Align switch
            {
                RvmAlign.Center => "rvm-centro",
                RvmAlign.End => "rvm-fim",
                RvmAlign.Start => "rvm-inicio",
                _ => null
            };
            var proprias = string.Join(' ', new[] { "rvm-celula", alinhamento, FrameClass }.Where(c => c is not null));
            return ClassesCss.Juntar(proprias, Class, AdditionalAttributes);
        }
    }
}
