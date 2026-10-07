namespace RVM.DesignSystem.Components.Table;

/// <summary>
/// O que a <see cref="RvmColumn{TItem}"/> precisa saber para se desenhar: se esta no cabecalho ou numa linha, e qual
/// item da linha (contrato com o RVM.UI, DSGN-017).
/// </summary>
/// <remarks>
/// A alternativa comum — cada coluna se registrar na tabela e a tabela desenhar depois, como a
/// <see cref="RvmTable{TItem}"/> faz — precisa de duas passadas de render e nao funciona em SSR estatico, onde so ha
/// uma. Aqui o mesmo fragmento de colunas e renderizado uma vez no cabecalho e uma vez por linha, e cada coluna
/// decide o que emitir. Sem lista de colunas, nao ha ordem de registro para dar errado nem render de atraso.
/// </remarks>
internal sealed class RvmColumnContext<TItem>
{
    /// <summary>A coluna esta desenhando o cabecalho.</summary>
    public bool Header { get; init; }

    /// <summary>O item da linha. So faz sentido quando <see cref="Header"/> e falso.</summary>
    public TItem? Item { get; init; }

    /// <summary>Titulo da coluna pela qual a tabela esta ordenada.</summary>
    public string? SortedBy { get; init; }

    /// <summary>Direcao da ordenacao atual.</summary>
    public RvmSortDirection Direction { get; init; } = RvmSortDirection.None;

    /// <summary>Avisa que pediram para ordenar por uma coluna (titulo e direcao nova).</summary>
    public Func<string, RvmSortDirection, Task>? OnSort { get; init; }
}
