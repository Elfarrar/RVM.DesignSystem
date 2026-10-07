using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace RVM.DesignSystem.Components.Typography;

/// <summary>
/// Texto com os niveis do contrato com o RVM.UI (DSGN-017). Cada <see cref="RvmTextVariant"/> aponta
/// para um estilo da escala do design system (as classes <c>rvm-text-*</c>); o <see cref="Weight"/>
/// troca so o peso. Para a escala completa, use o <see cref="RvmTypography"/>.
/// </summary>
/// <remarks>
/// C# puro pelo mesmo motivo do <see cref="RvmTypography"/>: a tag e dinamica e o Razor nao tem sintaxe
/// para elemento dinamico. Sem <c>.razor.css</c>: tamanho e entrelinha vem das classes da escala; cor e
/// peso vao no <c>style</c>, por token.
/// </remarks>
public class RvmText : ComponentBase
{
    /// <summary>Nivel de texto. Padrao: <see cref="RvmTextVariant.TextM"/>.</summary>
    [Parameter] public RvmTextVariant Variant { get; set; } = RvmTextVariant.TextM;

    /// <summary>Peso. Sem valor, vale o peso do estilo da escala.</summary>
    [Parameter] public RvmFontWeight? Weight { get; set; }

    /// <summary>Papel do texto na hierarquia de leitura. Padrao: <see cref="RvmTextColor.Default"/>.</summary>
    [Parameter] public RvmTextColor Color { get; set; } = RvmTextColor.Default;

    /// <summary>Elemento HTML. <see cref="RvmTextElement.Auto"/> (padrao): Display vira h1 a h4, o resto vira p.</summary>
    [Parameter] public RvmTextElement Element { get; set; } = RvmTextElement.Auto;

    /// <summary>O texto.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos extras, repassados a raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Tag);

        // AdditionalAttributes primeiro: class e style calculados abaixo mesclam com os do consumidor.
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "class", ClassesCss.Juntar($"rvm-text-{Escala}", Class, AdditionalAttributes));

        var estilo = Estilo;
        if (estilo is not null)
        {
            builder.AddAttribute(3, "style", estilo);
        }

        builder.AddContent(4, ChildContent);
        builder.CloseElement();
    }

    internal string Tag => Element switch
    {
        RvmTextElement.Auto => Variant switch
        {
            RvmTextVariant.DisplayXl => "h1",
            RvmTextVariant.DisplayL => "h2",
            RvmTextVariant.DisplayM => "h3",
            RvmTextVariant.DisplayS => "h4",
            _ => "p"
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

    /// <summary>O sufixo da classe <c>rvm-text-*</c> de cada nivel — a tabela da pagina do RvmText.</summary>
    internal string Escala => Variant switch
    {
        RvmTextVariant.DisplayXl => "h3",
        RvmTextVariant.DisplayL => "h4",
        RvmTextVariant.DisplayM => "h5",
        RvmTextVariant.DisplayS => "h6",
        RvmTextVariant.TextXl => "subtitle1",
        RvmTextVariant.TextL => "input",
        RvmTextVariant.TextS => "caption",
        RvmTextVariant.TextXs => "tooltip",
        _ => "body2"
    };

    // A Inter do pacote vai de 300 a 500, que sao os tokens. 600 e 700 nao tem token: o numero e o do
    // proprio enum, e o navegador sintetiza o negrito.
    private string? Peso => Weight switch
    {
        null => null,
        RvmFontWeight.Regular => "var(--rvm-font-weight-regular)",
        RvmFontWeight.Medium => "var(--rvm-font-weight-medium)",
        { } peso => ((int)peso).ToString(System.Globalization.CultureInfo.InvariantCulture)
    };

    private string? Estilo
    {
        get
        {
            var cor = RvmTypography.CorDoTexto(Color);
            var peso = Peso;
            var informado = AdditionalAttributes is not null
                            && AdditionalAttributes.TryGetValue("style", out var valor)
                            && valor is string texto
                            && !string.IsNullOrWhiteSpace(texto)
                ? texto
                : null;

            var partes = new[]
            {
                cor is null ? null : $"color: {cor};",
                peso is null ? null : $"font-weight: {peso};",
                informado
            }.Where(p => p is not null);

            var estilo = string.Join(' ', partes);
            return estilo.Length == 0 ? null : estilo;
        }
    }
}
