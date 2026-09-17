namespace RVM.DesignSystem.Components.Chart;

/// <summary>Como o arrasto no grafico se comporta.</summary>
public enum RvmChartSelectionMode
{
    /// <summary>Nao seleciona nada; com <c>Zoomable</c>, o arrasto desloca o que esta a vista. Padrao.</summary>
    None,

    /// <summary>
    /// O arrasto marca uma faixa de categorias, para filtrar o resto da tela. Com <c>Zoomable</c>, quem
    /// desloca passa a ser Shift com o arrasto.
    /// </summary>
    Range
}

/// <summary>
/// A faixa marcada no grafico, em indices dos itens (os dois extremos entram). Quem recebe filtra os
/// proprios dados: <c>Items.Take(range.End + 1).Skip(range.Start)</c>.
/// </summary>
/// <param name="Start">O primeiro item da faixa.</param>
/// <param name="End">O ultimo item da faixa.</param>
public sealed record RvmChartRange(int Start, int End)
{
    /// <summary>O primeiro item da faixa.</summary>
    public int Start { get; } = Start <= End
        ? Start
        : throw new ArgumentOutOfRangeException(nameof(Start), Start, "O inicio da faixa vem depois do fim.");

    /// <summary>Quantos itens a faixa cobre.</summary>
    public int Count => End - Start + 1;

    /// <summary>O indice esta dentro da faixa.</summary>
    public bool Contains(int indice) => indice >= Start && indice <= End;
}
