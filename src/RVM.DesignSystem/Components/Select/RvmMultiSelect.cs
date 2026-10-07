using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Select;

/// <summary>
/// Escolha de VARIAS opcoes numa lista, com busca opcional. A lista fica aberta enquanto se marca;
/// Esc, Tab ou clicar fora fecham.
/// </summary>
/// <typeparam name="TValue">Tipo de cada opcao.</typeparam>
public sealed class RvmMultiSelect<TValue> : RvmSelectBase<TValue>
{
    /// <summary>
    /// As opcoes escolhidas, na ordem da lista. Aceita <c>@bind-Values</c>. E o mesmo valor que <see cref="Value"/>:
    /// use um par so. Se vierem os dois, vale este.
    /// </summary>
    [Parameter] public IReadOnlyList<TValue>? Values { get; set; }

    /// <summary>Disparado quando a selecao muda (junto com <see cref="ValueChanged"/>).</summary>
    [Parameter] public EventCallback<IReadOnlyList<TValue>> ValuesChanged { get; set; }

    /// <summary>Expressao do valor ligado. O <c>@bind-Values</c> preenche sozinho.</summary>
    [Parameter] public Expression<Func<IReadOnlyList<TValue>>>? ValuesExpression { get; set; }

    /// <summary>
    /// As opcoes escolhidas, com o nome do RVM.UI (contrato, DSGN-017). Aceita <c>@bind-Value</c>. E o mesmo valor
    /// que <see cref="Values"/>.
    /// </summary>
    [Parameter] public IReadOnlyList<TValue>? Value { get; set; }

    /// <summary>Disparado quando a selecao muda (junto com <see cref="ValuesChanged"/>).</summary>
    [Parameter] public EventCallback<IReadOnlyList<TValue>> ValueChanged { get; set; }

    /// <summary>Expressao do valor ligado. O <c>@bind-Value</c> preenche sozinho.</summary>
    [Parameter] public Expression<Func<IReadOnlyList<TValue>>>? ValueExpression { get; set; }

    internal override bool Multiplo => true;

    internal override LambdaExpression? ExpressaoDoCampo => ValuesExpression ?? ValueExpression;

    internal override IEnumerable<TValue> Selecionados => Values ?? Value ?? [];

    internal override bool EstaSelecionado(TValue item) => Selecionados.Contains(item);

    internal override async Task<bool> AplicarEscolhaAsync(TValue item)
    {
        var marcados = new HashSet<TValue>(Selecionados);
        if (!marcados.Remove(item))
        {
            marcados.Add(item);
        }

        // Na ordem das opcoes, e nao na ordem dos cliques: o texto exibido fica estavel.
        var novos = Items.Where(marcados.Contains).ToList();
        if (Values is not null || Value is null) Values = novos;
        if (Value is not null) Value = novos;
        await ValuesChanged.InvokeAsync(novos);
        await ValueChanged.InvokeAsync(novos);
        return false;
    }
}
