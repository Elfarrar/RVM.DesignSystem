namespace RVM.DesignSystem.Components.Chart;

/// <summary>
/// Qual grafico a <see cref="RvmChartLegend"/> acompanha (contrato com o RVM.UI, DSGN-017). Cada grafico tem a
/// sua rampa de cores por posicao; escolher o errado faz a legenda apontar para uma cor que o desenho nao usou.
/// </summary>
public enum RvmLegendRamp
{
    /// <summary>Pizza e rosca: segue a <see cref="RvmChartPalette"/>. E o padrao.</summary>
    Radial,

    /// <summary>Colunas e barras: as cores de serie, na ordem (a paleta nao muda nada, como no grafico).</summary>
    Bar,

    /// <summary>Linha e area: as cores de serie, na ordem (a paleta nao muda nada, como no grafico).</summary>
    Line,

    /// <summary>Medidor (a barra repartida): degraus proprios no monocromatico.</summary>
    Meter,

    /// <summary>
    /// Barras divergentes (<see cref="RvmBarLayout.Diverging"/>): a primeira serie no primary, a segunda na cor de
    /// erro.
    /// </summary>
    Diverging
}
