namespace RVM.DesignSystem.Components.Chart;

/// <summary>Uma linha da legenda: o nome da serie ou da fatia e, opcionalmente, o valor (contrato com o RVM.UI, DSGN-017).</summary>
/// <param name="Name">O nome, na mesma ordem em que o grafico desenha.</param>
/// <param name="Value">
/// O valor ja formatado ("R$ 26.500", "35%"), ou nulo quando so o nome importa. Ja formatado de proposito: a legenda
/// nao sabe a cultura nem a moeda de quem chama.
/// </param>
public sealed record RvmChartLegendEntry(string Name, string? Value = null);
