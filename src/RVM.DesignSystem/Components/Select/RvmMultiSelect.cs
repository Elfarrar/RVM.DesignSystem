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
    /// <summary>As opcoes escolhidas, na ordem da lista. Aceita <c>@bind-Values</c>.</summary>
    [Parameter] public IReadOnlyList<TValue> Values { get; set; } = [];

    /// <summary>Disparado quando a selecao muda.</summary>
    [Parameter] public EventCallback<IReadOnlyList<TValue>> ValuesChanged { get; set; }

    /// <summary>Expressao do valor ligado. O <c>@bind-Values</c> preenche sozinho.</summary>
    [Parameter] public Expression<Func<IReadOnlyList<TValue>>>? ValuesExpression { get; set; }

    internal override bool Multiplo => true;

    internal override LambdaExpression? ExpressaoDoCampo => ValuesExpression;

    internal override IEnumerable<TValue> Selecionados => Values ?? [];

    internal override bool EstaSelecionado(TValue item) => Values?.Contains(item) ?? false;

    internal override async Task<bool> AplicarEscolhaAsync(TValue item)
    {
        var marcados = new HashSet<TValue>(Selecionados);
        if (!marcados.Remove(item))
        {
            marcados.Add(item);
        }

        // Na ordem das opcoes, e nao na ordem dos cliques: o texto exibido fica estavel.
        var novos = Items.Where(marcados.Contains).ToList();
        Values = novos;
        await ValuesChanged.InvokeAsync(novos);
        return false;
    }
}
