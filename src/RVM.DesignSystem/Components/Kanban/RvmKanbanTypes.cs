namespace RVM.DesignSystem.Components.Kanban;

/// <summary>
/// Uma coluna do <see cref="RvmKanbanBoard{TItem}"/> (contrato com o RVM.UI, DSGN-017).
/// </summary>
/// <typeparam name="TItem">O tipo do cartao.</typeparam>
/// <param name="Id">Identificador da coluna. E o que volta no <see cref="RvmKanbanMove{TItem}"/>.</param>
/// <param name="Title">Titulo visivel. A contagem sai do tamanho de <paramref name="Items"/>.</param>
/// <param name="Items">Os cartoes, na ordem em que aparecem.</param>
public sealed record RvmKanbanColumn<TItem>(string Id, string Title, IReadOnlyList<TItem> Items);

/// <summary>
/// Um cartao mudando de coluna. O quadro NAO altera as listas: ele avisa, e quem manda no dado e o aplicativo.
/// </summary>
/// <typeparam name="TItem">O tipo do cartao.</typeparam>
/// <param name="Item">O cartao movido.</param>
/// <param name="FromColumnId">De onde saiu.</param>
/// <param name="ToColumnId">Para onde vai.</param>
/// <param name="ToIndex">Em que posicao entra na coluna de destino (o fim dela).</param>
public sealed record RvmKanbanMove<TItem>(TItem Item, string FromColumnId, string ToColumnId, int ToIndex);
