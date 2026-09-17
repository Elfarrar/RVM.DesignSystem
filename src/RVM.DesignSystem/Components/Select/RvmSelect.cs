using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Select;

/// <summary>
/// Escolha de UMA opcao numa lista, com busca opcional. Dentro de um <c>EditForm</c> participa da
/// validacao pelo <c>@bind-Value</c>.
/// </summary>
/// <typeparam name="TValue">Tipo de cada opcao.</typeparam>
public sealed class RvmSelect<TValue> : RvmSelectBase<TValue>
{
    /// <summary>
    /// A opcao escolhida. Aceita <c>@bind-Value</c>. Para o campo poder comecar vazio com enum ou
    /// numero, use o tipo anulavel (<c>Status?</c>, <c>int?</c>): sem isso o <c>default</c> ja e uma escolha.
    /// </summary>
    [Parameter] public TValue? Value { get; set; }

    /// <summary>Disparado quando a escolha muda.</summary>
    [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }

    /// <summary>Expressao do valor ligado. O <c>@bind-Value</c> preenche sozinho.</summary>
    [Parameter] public Expression<Func<TValue?>>? ValueExpression { get; set; }

    internal override bool Multiplo => false;

    internal override LambdaExpression? ExpressaoDoCampo => ValueExpression;

    /// <summary>
    /// So conta como escolhido o valor que esta nas opcoes. ⚠️ Isso NAO resolve "nada escolhido" para
    /// tipo valor nao anulavel: num <c>RvmSelect&lt;Status&gt;</c> sem valor, <c>default</c> (0) e uma
    /// opcao legitima e aparece escolhida — nao ha como distinguir "escolheu 0" de "nao escolheu". Para
    /// comecar vazio, use o tipo anulavel (<c>RvmSelect&lt;Status?&gt;</c>), como no <c>InputSelect</c>.
    /// Achado do review independente da onda 2.
    /// </summary>
    internal override IEnumerable<TValue> Selecionados
        => Value is not null && Items.Contains(Value) ? [Value] : [];

    internal override bool EstaSelecionado(TValue item) => EqualityComparer<TValue>.Default.Equals(item, Value!);

    internal override async Task<bool> AplicarEscolhaAsync(TValue item)
    {
        if (!EstaSelecionado(item))
        {
            Value = item;
            await ValueChanged.InvokeAsync(item);
        }

        return true;
    }
}
