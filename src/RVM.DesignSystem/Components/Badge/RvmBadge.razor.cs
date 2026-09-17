using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Badge;

/// <summary>
/// Contador ou ponto de aviso. Sozinho, ou por cima de um icone ou avatar passado como conteudo.
/// </summary>
public partial class RvmBadge : ComponentBase
{
    /// <summary>O que aparece dentro da pilula ("3", "Novo"). Ignorado quando <see cref="Dot"/>.</summary>
    [Parameter] public string? Content { get; set; }

    /// <summary>Contador numerico. Vence <see cref="Content"/> e respeita <see cref="Max"/>.</summary>
    [Parameter] public int? Count { get; set; }

    /// <summary>Acima deste valor, o contador mostra "<c>Max</c>+". Padrao: 99.</summary>
    [Parameter] public int Max { get; set; } = 99;

    /// <summary>So o ponto de 8 px, sem texto — "tem novidade", sem dizer quanto.</summary>
    [Parameter] public bool Dot { get; set; }

    /// <summary>Papel de cor. Padrao: <see cref="RvmColor.Primary"/>.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Primary;

    /// <summary>
    /// O que o badge significa, para o leitor de tela ("3 mensagens nao lidas"). Obrigatorio na
    /// pratica quando <see cref="Dot"/>: um ponto sem rotulo nao diz nada a quem nao o ve.
    /// </summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>O conteudo sobre o qual o badge fica (um icone, um avatar).</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Atributos extras, repassados a raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private bool TemRotulo => !string.IsNullOrWhiteSpace(Label);

    internal string? TextoVisivel
        => Dot ? null
            : Count is { } n ? (n > Max ? $"{Max}+" : n.ToString(System.Globalization.CultureInfo.InvariantCulture))
            : Content;

    private string ClasseDaCor => Color switch
    {
        RvmColor.Secondary => "secondary",
        RvmColor.Info => "info",
        RvmColor.Success => "success",
        RvmColor.Warning => "warning",
        RvmColor.Error => "error",
        _ => "primary"
    };

    internal string ClassesDaMarca
        => string.Join(' ', "badge", Dot ? "ponto" : "pilula", ClasseDaCor, ChildContent is null ? "solto" : "sobreposto");

    internal string ClassesDaRaiz => ComClasseDoConsumidor("badge-raiz");

    internal string ClassesDaAncora => ComClasseDoConsumidor("badge-ancora");

    private string ComClasseDoConsumidor(string proprias)
        => AdditionalAttributes is not null
           && AdditionalAttributes.TryGetValue("class", out var informada)
           && informada is string texto
           && !string.IsNullOrWhiteSpace(texto)
            ? $"{proprias} {texto}"
            : proprias;
}
