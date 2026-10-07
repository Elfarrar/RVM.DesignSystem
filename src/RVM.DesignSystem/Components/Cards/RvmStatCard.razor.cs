using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Progress;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Cards;

/// <summary>
/// Card de estatistica de painel: titulo, numero, variacao e, abaixo, progresso, imagem ou um grafico do consumidor
/// (contrato com o RVM.UI, DSGN-017). Com <see cref="Surface"/> = <see cref="RvmSurface.Dark"/>, o card inteiro fica
/// na cor do papel, com o texto no token de contraste dele.
/// </summary>
public partial class RvmStatCard : ComponentBase
{
    /// <summary>Tamanho do card. Padrao: <see cref="RvmStatCardSize.Small"/>.</summary>
    [Parameter] public RvmStatCardSize Size { get; set; } = RvmStatCardSize.Small;

    /// <summary>Papel de cor: do enfeite e do progresso no fundo claro, do card inteiro no escuro. Padrao: Accent.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Accent;

    /// <summary>
    /// Fundo do card: claro (papel do tema, padrao) ou cheio na cor de <see cref="Color"/>, com o texto no token de
    /// contraste do papel (branco no primary, grafite nos outros — o par que passa AA nos dois temas).
    /// </summary>
    [Parameter] public RvmSurface Surface { get; set; } = RvmSurface.Light;

    /// <summary>Enfeite: nenhum (padrao), icone ao lado do titulo ou selo circular ao lado do numero.</summary>
    [Parameter] public RvmStatCardAdornment Adornment { get; set; } = RvmStatCardAdornment.None;

    /// <summary>Icone do enfeite. Obrigatorio quando <see cref="Adornment"/> nao e None.</summary>
    [Parameter] public RvmIconName? AdornmentIcon { get; set; }

    /// <summary>Titulo do indicador. Sai como cabecalho <c>h3</c>.</summary>
    [Parameter, EditorRequired] public string Title { get; set; } = "";

    /// <summary>Linha de apoio abaixo do titulo (o periodo: "Safra 25/26").</summary>
    [Parameter] public string? Subtitle { get; set; }

    /// <summary>Numero principal, ja formatado por quem chama (o card nao formata moeda).</summary>
    [Parameter, EditorRequired] public string Value { get; set; } = "";

    /// <summary>Sentido da variacao. Padrao: sem variacao.</summary>
    [Parameter] public RvmStatCardTrend Trend { get; set; } = RvmStatCardTrend.None;

    /// <summary>A variacao ("12%"). Obrigatoria quando <see cref="Trend"/> nao e None.</summary>
    [Parameter] public string? TrendValue { get; set; }

    /// <summary>Detalhe ao lado da variacao ("frente a safra passada").</summary>
    [Parameter] public string? TrendDetail { get; set; }

    /// <summary>O que vai abaixo do numero: nada (padrao), progresso ou imagem. Ignorado com <see cref="Chart"/>.</summary>
    [Parameter] public RvmStatCardVisual Visual { get; set; } = RvmStatCardVisual.None;

    /// <summary>Progresso de 0 a 100, para <see cref="RvmStatCardVisual.Progress"/> (obrigatorio nele).</summary>
    [Parameter] public double? VisualValue { get; set; }

    /// <summary>Nome acessivel da barra de progresso. Sem ele, o <see cref="Title"/>.</summary>
    [Parameter] public string? VisualLabel { get; set; }

    /// <summary>Endereco da imagem, para <see cref="RvmStatCardVisual.Image"/> (obrigatorio nele). Decorativa.</summary>
    [Parameter] public string? ImageUrl { get; set; }

    /// <summary>Encaixe livre abaixo do numero — um grafico pequeno, sem eixo. Preenchido, vence <see cref="Visual"/>.</summary>
    [Parameter] public RenderFragment? Chart { get; set; }

    /// <summary>Acao no canto: nenhuma (padrao), botao de icone ou menu de tres pontos.</summary>
    [Parameter] public RvmCardAction Action { get; set; } = RvmCardAction.None;

    /// <summary>Icone do botao de acao. Padrao: <see cref="RvmIconName.Add"/>.</summary>
    [Parameter] public RvmIconName ActionIcon { get; set; } = RvmIconName.Add;

    /// <summary>Nome acessivel do botao ou do menu. Obrigatorio quando <see cref="Action"/> nao e None.</summary>
    [Parameter] public string? ActionLabel { get; set; }

    /// <summary>Clique do botao de acao (<see cref="RvmCardAction.Button"/>).</summary>
    [Parameter] public EventCallback OnActionClick { get; set; }

    /// <summary>Itens do menu (<c>RvmMenuItem</c>), para <see cref="RvmCardAction.Menu"/> (obrigatorio nele).</summary>
    [Parameter] public RenderFragment? ActionMenu { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos repassados ao elemento raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private bool Cheio => Surface == RvmSurface.Dark;

    // No card cheio, as pecas do DS vao no papel Inverse: o CSS do card redefine os tokens dele para a tinta do papel.
    internal RvmColor CorDasPecas => Cheio ? RvmColor.Inverse : Color;

    internal RvmColor CorDoMenu => Cheio ? RvmColor.Inverse : RvmColor.Secondary;

    internal string ClasseDoValor => "rvm-valor " + (Size == RvmStatCardSize.Small ? "rvm-text-h5" : "rvm-text-h4");

    internal string CssClass => ClassesCss.Juntar(
        string.Join(' ',
            "rvm-stat-card",
            Size switch
            {
                RvmStatCardSize.Large => "rvm-grande",
                RvmStatCardSize.XtraLarge => "rvm-extra",
                _ => "rvm-pequeno"
            },
            Cheio ? "rvm-cheio" : "rvm-claro",
            PapelCss.Classe(Color)),
        Class,
        AdditionalAttributes);

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (Adornment != RvmStatCardAdornment.None && AdornmentIcon is null)
        {
            throw new ArgumentException("RvmStatCard precisa de AdornmentIcon quando Adornment nao e None.", nameof(AdornmentIcon));
        }
        if (Trend != RvmStatCardTrend.None && string.IsNullOrWhiteSpace(TrendValue))
        {
            throw new ArgumentException("RvmStatCard precisa de TrendValue quando Trend nao e None.", nameof(TrendValue));
        }
        if (Action != RvmCardAction.None && string.IsNullOrWhiteSpace(ActionLabel))
        {
            throw new ArgumentException("RvmStatCard precisa de ActionLabel quando Action nao e None: e o nome acessivel.", nameof(ActionLabel));
        }
        if (Action == RvmCardAction.Menu && ActionMenu is null)
        {
            throw new ArgumentException("RvmStatCard precisa de ActionMenu quando Action e Menu.", nameof(ActionMenu));
        }
        // Com Chart preenchido e ele quem desenha: as guardas de Visual so valem quando o proprio Visual vai aparecer.
        if (Chart is null && Visual == RvmStatCardVisual.Image && string.IsNullOrWhiteSpace(ImageUrl))
        {
            throw new ArgumentException("RvmStatCard precisa de ImageUrl quando Visual e Image.", nameof(ImageUrl));
        }
        if (Chart is null && Visual == RvmStatCardVisual.Progress && VisualValue is null)
        {
            throw new ArgumentException("RvmStatCard precisa de VisualValue quando Visual e Progress.", nameof(VisualValue));
        }
    }
}
