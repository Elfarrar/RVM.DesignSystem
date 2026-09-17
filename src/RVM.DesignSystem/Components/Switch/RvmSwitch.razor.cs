using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Switch;

/// <summary>
/// Liga e desliga algo que vale na hora (notificacoes, modo compacto). Para uma escolha que so vale
/// quando o formulario e enviado, o checkbox costuma ser o controle mais honesto.
/// </summary>
public partial class RvmSwitch : ComponentBase
{
    /// <summary>Ligado ou desligado. Aceita <c>@bind-Value</c>.</summary>
    [Parameter] public bool Value { get; set; }

    /// <summary>Disparado quando muda.</summary>
    [Parameter] public EventCallback<bool> ValueChanged { get; set; }

    /// <summary>Expressao do valor ligado. O <c>@bind-Value</c> preenche sozinho.</summary>
    [Parameter] public Expression<Func<bool>>? ValueExpression { get; set; }

    private Expression<Func<bool>>? _expressaoPadrao;

    /// <summary>
    /// A expressao que vai para o InputCheckbox. Ele EXIGE uma, e so o <c>@bind-Value</c> a fornece:
    /// sem ela, usos comuns como <c>Value="true" Disabled="true"</c> (so exibir) ou
    /// <c>Value</c> + <c>ValueChanged</c> faziam o controle estourar e sumir da tela — com o
    /// <c>#blazor-error-ui</c> no ar. Pego no navegador; o bUnit sempre passava a expressao.
    /// </summary>
    internal Expression<Func<bool>> ExpressaoEfetiva => ValueExpression ?? (_expressaoPadrao ??= () => Value);

    /// <summary>Texto ao lado. Tambem e o nome acessivel.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Rotulo livre, quando <see cref="Label"/> esta vazio.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Papel de cor do ligado. Padrao: <see cref="RvmColor.Primary"/>.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Primary;

    /// <summary>
    /// Trilho de 34 x 14 px (padrao) ou 26 x 10 px (<see cref="RvmSize.Small"/>), medidos no kit.
    /// O kit nao define um switch grande, entao <see cref="RvmSize.Large"/> sai igual ao medio.
    /// </summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>Indisponivel.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Atributos extras: <c>class</c> e <c>style</c> na raiz; o resto no input nativo.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = string.Join(' ',
                "controle switch",
                Size == RvmSize.Small ? "pequeno" : "medio",
                Color switch
                {
                    RvmColor.Secondary => "secondary",
                    RvmColor.Info => "info",
                    RvmColor.Success => "success",
                    RvmColor.Warning => "warning",
                    RvmColor.Error => "error",
                    _ => "primary"
                });

            if (Disabled) proprias += " desabilitado";

            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }

    internal string? EstiloDoConsumidor
        => AdditionalAttributes is not null
           && AdditionalAttributes.TryGetValue("style", out var valor)
           && valor is string texto
           && !string.IsNullOrWhiteSpace(texto)
            ? texto
            : null;

    internal IReadOnlyDictionary<string, object>? AtributosDoInput
        => AdditionalAttributes?
            .Where(a => !string.Equals(a.Key, "class", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(a.Key, "style", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(a => a.Key, a => a.Value);
}
