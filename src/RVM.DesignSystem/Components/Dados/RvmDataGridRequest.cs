namespace RVM.DesignSystem.Components;

/// <summary>
/// O que o grid esta pedindo, no modo remoto.
/// </summary>
/// <param name="Page">Pagina, comecando em 1.</param>
/// <param name="PageSize">Quantos itens por pagina.</param>
/// <param name="SortColumn">
/// Titulo da coluna ordenada, ou <c>null</c>. E o <b>titulo</b>, e nao a expressao: quem responde
/// traduz para o campo do banco, porque so ele sabe o nome da coluna la.
/// </param>
/// <param name="SortDirection">Direcao da ordenacao.</param>
public sealed record RvmDataGridRequest(
    int Page,
    int PageSize,
    string? SortColumn,
    RvmSortDirection SortDirection);

/// <summary>
/// A resposta do modo remoto.
/// </summary>
/// <param name="Items">Os itens DESTA pagina, ja ordenados por quem respondeu.</param>
/// <param name="TotalItems">
/// Quantos itens existem no total, e nao nesta pagina. E ele que o rodape usa para saber quantas
/// paginas ha — devolver a contagem da pagina faria a paginacao dizer que so existe uma.
/// </param>
public sealed record RvmDataGridResult<TItem>(
    IReadOnlyList<TItem> Items,
    int TotalItems);
