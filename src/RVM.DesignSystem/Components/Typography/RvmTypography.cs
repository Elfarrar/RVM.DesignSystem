using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace RVM.DesignSystem.Components.Typography;

/// <summary>
/// Texto na escala tipografica do design system. Escolhe o ESTILO pelo <see cref="Variant"/> e a
/// SEMANTICA pelo <see cref="As"/> — as duas coisas sao separadas de proposito.
/// </summary>
/// <remarks>
/// Escrito em C# puro, e nao em <c>.razor</c>, porque a tag do elemento e dinamica e o Razor nao
/// tem sintaxe para isso (<c>DynamicComponent</c> monta componente, nao elemento HTML). Por
/// consequencia nao ha <c>.razor.css</c>: o estilo deste componente E a escala tipografica, que
/// mora na camada de tokens como as classes <c>rvm-text-*</c>.
/// </remarks>
public class RvmTypography : ComponentBase
{
    /// <summary>Estilo da escala. Padrao: <see cref="RvmTypographyVariant.Body1"/>.</summary>
    [Parameter] public RvmTypographyVariant Variant { get; set; } = RvmTypographyVariant.Body1;

    /// <summary>Elemento HTML gerado. Padrao: deduzido do <see cref="Variant"/>.</summary>
    [Parameter] public RvmTextElement As { get; set; } = RvmTextElement.Auto;

    /// <summary>Papel do texto na hierarquia de leitura. Padrao: herda de quem esta em volta.</summary>
    [Parameter] public RvmTextColor Color { get; set; } = RvmTextColor.Inherit;

    /// <summary>O texto.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc cref="ComponentBase" />
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Tag);

        // `AdditionalAttributes` primeiro: assim `class` e `style` calculados abaixo mesclam com o
        // que o consumidor mandou, em vez de serem sobrescritos por ele.
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "class", CssClass);

        var cor = TokenDeCor;
        if (cor is not null)
        {
            builder.AddAttribute(3, "style", Estilo(cor));
        }

        builder.AddContent(4, ChildContent);
        builder.CloseElement();
    }

    internal string Tag => As switch
    {
        RvmTextElement.Auto => Variant switch
        {
            RvmTypographyVariant.H1 => "h1",
            RvmTypographyVariant.H2 => "h2",
            RvmTypographyVariant.H3 => "h3",
            RvmTypographyVariant.H4 => "h4",
            RvmTypographyVariant.H5 => "h5",
            RvmTypographyVariant.H6 => "h6",
            RvmTypographyVariant.Body1 or RvmTypographyVariant.Body2
                or RvmTypographyVariant.Subtitle1 or RvmTypographyVariant.Subtitle2 => "p",
            _ => "span"
        },
        RvmTextElement.H1 => "h1",
        RvmTextElement.H2 => "h2",
        RvmTextElement.H3 => "h3",
        RvmTextElement.H4 => "h4",
        RvmTextElement.H5 => "h5",
        RvmTextElement.H6 => "h6",
        RvmTextElement.P => "p",
        RvmTextElement.Div => "div",
        RvmTextElement.Label => "label",
        _ => "span"
    };

    internal string CssClass
    {
        get
        {
            var classe = $"rvm-text-{Sufixo}";
            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{classe} {texto}"
                : classe;
        }
    }

    private string Sufixo => Variant switch
    {
        RvmTypographyVariant.H1 => "h1",
        RvmTypographyVariant.H2 => "h2",
        RvmTypographyVariant.H3 => "h3",
        RvmTypographyVariant.H4 => "h4",
        RvmTypographyVariant.H5 => "h5",
        RvmTypographyVariant.H6 => "h6",
        RvmTypographyVariant.Subtitle1 => "subtitle1",
        RvmTypographyVariant.Subtitle2 => "subtitle2",
        RvmTypographyVariant.Body1 => "body1",
        RvmTypographyVariant.Body2 => "body2",
        RvmTypographyVariant.Caption => "caption",
        RvmTypographyVariant.Overline => "overline",
        RvmTypographyVariant.ButtonLarge => "button-lg",
        RvmTypographyVariant.ButtonMedium => "button-md",
        RvmTypographyVariant.ButtonSmall => "button-sm",
        RvmTypographyVariant.InputLabel => "input-label",
        RvmTypographyVariant.Helper => "helper",
        RvmTypographyVariant.Input => "input",
        RvmTypographyVariant.AvatarInitials => "avatar",
        RvmTypographyVariant.Chip => "chip",
        RvmTypographyVariant.Tooltip => "tooltip",
        RvmTypographyVariant.AlertTitle => "alert-title",
        RvmTypographyVariant.TableHeader => "table-header",
        _ => "badge"
    };

    private string? TokenDeCor => Color switch
    {
        RvmTextColor.Primary => "var(--rvm-color-text-primary)",
        RvmTextColor.Secondary => "var(--rvm-color-text-secondary)",
        RvmTextColor.Disabled => "var(--rvm-color-text-disabled)",
        _ => null
    };

    private string Estilo(string cor)
        => AdditionalAttributes is not null
           && AdditionalAttributes.TryGetValue("style", out var informado)
           && informado is string texto
           && !string.IsNullOrWhiteSpace(texto)
            ? $"color: {cor}; {texto}"
            : $"color: {cor};";
}
