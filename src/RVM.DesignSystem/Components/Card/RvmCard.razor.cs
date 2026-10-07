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

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Cabecalho livre, no lugar do titulo e do subtitulo (a acao do cabecalho continua).</summary>
    [Parameter] public RenderFragment? Header { get; set; }

    /// <summary>Rodape (acoes finais, totais), abaixo das acoes e separado por um divisor.</summary>
    [Parameter] public RenderFragment? Footer { get; set; }

    /// <summary>Respiro interno. Padrao: 20 px.</summary>
    [Parameter] public RvmCardPadding Padding { get; set; } = RvmCardPadding.Medium;

    /// <summary>Aparencia da superficie. Padrao: com sombra.</summary>
    [Parameter] public RvmCardVariant Variant { get; set; } = RvmCardVariant.Elevated;

    /// <inheritdoc cref="ComponentBase" />
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal bool TemCabecalho
        => Header is not null || !string.IsNullOrWhiteSpace(Title) || !string.IsNullOrWhiteSpace(Subheader) || HeaderAction is not null;

    internal string CssClass
    {
        get
        {
            var proprias = "rvm-cartao" + Variant switch
            {
                RvmCardVariant.Outlined => " rvm-contornado",
                RvmCardVariant.Flat => " rvm-plano",
                _ => ""
            } + Padding switch
            {
                RvmCardPadding.None => " rvm-respiro-nenhum",
                RvmCardPadding.Small => " rvm-respiro-pequeno",
                RvmCardPadding.Large => " rvm-respiro-grande",
                _ => ""
            };
            return ClassesCss.Juntar(proprias, Class, AdditionalAttributes);
        }
    }
}
