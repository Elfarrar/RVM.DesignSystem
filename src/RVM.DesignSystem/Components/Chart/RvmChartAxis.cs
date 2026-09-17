namespace RVM.DesignSystem.Components.Chart;

/// <summary>Em qual eixo de valores a serie e lida.</summary>
public enum RvmChartAxis
{
    /// <summary>O eixo da esquerda (no grafico de colunas, de linha e de area). Padrao.</summary>
    Primary,

    /// <summary>
    /// O eixo da direita, com escala propria: para uma serie de outra unidade ou de outra ordem de
    /// grandeza (receita em reais e margem em porcento no mesmo grafico).
    /// </summary>
    Secondary
}
