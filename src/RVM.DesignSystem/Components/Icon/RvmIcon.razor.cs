using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Icon;

/// <summary>
/// Um icone do conjunto Tabler, desenhado inline. A cor vem por heranca (<c>currentColor</c>), ou
/// do papel semantico quando <see cref="Color"/> e informado.
/// </summary>
public partial class RvmIcon : ComponentBase
{
    /// <summary>Qual icone. Enum: nome errado nao compila.</summary>
    [Parameter, EditorRequired] public RvmIconName Name { get; set; }

    /// <summary>Tamanho do lado do icone. Padrao: <see cref="RvmSize.Medium"/> (20 px).</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>
    /// Papel semantico da cor. Sem valor, o icone herda a cor do texto em volta — que e o que se
    /// quer dentro de botao, chip e alerta, onde o contraste ja foi resolvido pelo container.
    /// </summary>
    [Parameter] public RvmColor? Color { get; set; }

    /// <summary>
    /// O que o icone significa, para quem nao o enxerga. **Preencha so quando o icone for a UNICA
    /// fonte daquela informacao** — botao de icone sem rotulo, por exemplo. Ao lado de um texto que
    /// ja diz a mesma coisa, deixe vazio: o leitor de tela repetiria a informacao.
    /// </summary>
    [Parameter] public string? Title { get; set; }

    /// <inheritdoc cref="ComponentBase" />
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Lado do icone em pixels, para o <c>width</c> e o <c>height</c> do SVG.</summary>
    internal int Lado => Size switch
    {
        RvmSize.Small => 16,
        RvmSize.Large => 24,
        _ => 20
    };

    internal MarkupString Desenho => new(RvmIconCatalogo.Desenhos[Name]);

    internal string CssClass
    {
        get
        {
            const string propria = "rvm-icone";
            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{propria} {texto}"
                : propria;
        }
    }

    internal string? Estilo
    {
        get
        {
            var cor = Color switch
            {
                RvmColor.Primary => "var(--rvm-color-primary-text)",
                RvmColor.Secondary => "var(--rvm-color-secondary-text)",
                RvmColor.Info => "var(--rvm-color-info-text)",
                RvmColor.Success => "var(--rvm-color-success-text)",
                RvmColor.Warning => "var(--rvm-color-warning-text)",
                RvmColor.Error => "var(--rvm-color-error-text)",
                _ => null
            };

            var informado = AdditionalAttributes is not null
                            && AdditionalAttributes.TryGetValue("style", out var valor)
                            && valor is string texto
                            && !string.IsNullOrWhiteSpace(texto)
                ? texto
                : null;

            return (cor, informado) switch
            {
                (null, null) => null,
                (null, _) => informado,
                (_, null) => $"color: {cor};",
                _ => $"color: {cor}; {informado}"
            };
        }
    }
}
