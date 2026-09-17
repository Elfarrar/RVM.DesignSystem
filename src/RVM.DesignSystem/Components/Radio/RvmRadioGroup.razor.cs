using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Radio;

/// <summary>
/// Grupo de opcoes em que so uma vale. Os filhos sao <see cref="RvmRadio{TValue}"/>, que herdam a
/// cor, o tamanho e o estado desabilitado daqui.
/// </summary>
/// <typeparam name="TValue">Tipo do valor escolhido — um enum costuma ser o mais claro.</typeparam>
public partial class RvmRadioGroup<TValue> : ComponentBase
{
    /// <summary>O valor escolhido. Aceita <c>@bind-Value</c>.</summary>
    [Parameter] public TValue? Value { get; set; }

    /// <summary>Disparado quando a escolha muda.</summary>
    [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }

    /// <summary>Expressao do valor ligado. O <c>@bind-Value</c> preenche sozinho.</summary>
    [Parameter] public Expression<Func<TValue?>>? ValueExpression { get; set; }

    private Expression<Func<TValue?>>? _expressaoPadrao;

    /// <summary>
    /// O InputRadioGroup EXIGE uma expressao, e so o <c>@bind-Value</c> a fornece. Sem esta
    /// alternativa, <c>Value</c> + <c>ValueChanged</c> fazia o grupo estourar em tempo de execucao.
    /// </summary>
    internal Expression<Func<TValue?>> ExpressaoEfetiva => ValueExpression ?? (_expressaoPadrao ??= () => Value);

    /// <summary>A pergunta do grupo ("Forma de pagamento"). Vira a legenda do fieldset.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>
    /// <c>name</c> dos radios. Sem ele, o Blazor gera um a partir do <c>@bind-Value</c> — que e o que
    /// o formulario em SSR estatico precisa.
    /// </summary>
    [Parameter] public string? Name { get; set; }

    /// <summary>Opcoes em linha (<see cref="RvmOrientation.Horizontal"/>) ou empilhadas (padrao).</summary>
    [Parameter] public RvmOrientation Orientation { get; set; } = RvmOrientation.Vertical;

    /// <summary>Papel de cor da opcao escolhida. Padrao: <see cref="RvmColor.Primary"/>.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Primary;

    /// <summary>18, 20 (padrao) ou 22 px de circulo — medidos no kit.</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>Desabilita o grupo inteiro.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>As opcoes.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Atributos extras, repassados ao fieldset.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = Orientation == RvmOrientation.Horizontal ? "rvm-grupo rvm-horizontal" : "rvm-grupo rvm-vertical";
            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }
}
