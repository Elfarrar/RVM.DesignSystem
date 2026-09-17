using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Typography;

namespace RVM.DesignSystem.Components.Card;

/// <summary>
/// Superficie que agrupa um assunto: midia no topo, cabecalho, conteudo e acoes — todos opcionais.
/// </summary>
public partial class RvmCard : ComponentBase
{
    /// <summary>Titulo do cabecalho.</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>Linha de apoio abaixo do titulo.</summary>
    [Parameter] public string? Subheader { get; set; }

    /// <summary>
    /// Elemento do titulo. Padrao: <c>h3</c>. Ajuste a ordem de cabecalhos da pagina — um card
    /// solto numa pagina cujo titulo e h1 costuma querer h2.
    /// </summary>
    [Parameter] public RvmTextElement TitleElement { get; set; } = RvmTextElement.H3;

    /// <summary>Algo no canto direito do cabecalho (menu, botao de icone).</summary>
    [Parameter] public RenderFragment? HeaderAction { get; set; }

    /// <summary>Imagem no topo do card.</summary>
    [Parameter] public string? MediaSrc { get; set; }

    /// <summary>Texto alternativo da imagem. Vazio quando ela e so decorativa.</summary>
    [Parameter] public string? MediaAlt { get; set; }

    /// <summary>Altura da imagem em pixels. Padrao: 200.</summary>
    [Parameter] public int MediaHeight { get; set; } = 200;

    /// <summary>O conteudo.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Acoes no rodape do card.</summary>
    [Parameter] public RenderFragment? Actions { get; set; }

    /// <inheritdoc cref="ComponentBase" />
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal bool TemCabecalho
        => !string.IsNullOrWhiteSpace(Title) || !string.IsNullOrWhiteSpace(Subheader) || HeaderAction is not null;

    internal string CssClass
    {
        get
        {
            const string propria = "rvm-cartao";
            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{propria} {texto}"
                : propria;
        }
    }
}
