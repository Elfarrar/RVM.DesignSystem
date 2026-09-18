namespace RVM.DesignSystem.Components.Chart;

/// <summary>Em que formato o grafico e exportado.</summary>
public enum RvmChartExportFormat
{
    /// <summary>Imagem PNG, em duas vezes a resolucao da tela.</summary>
    Png,

    /// <summary>O proprio SVG, com as cores embutidas (abre em qualquer editor vetorial).</summary>
    Svg,

    /// <summary>Os dados em CSV, separados por ponto e virgula, com BOM (abre no Excel em PT-BR).</summary>
    Csv,

    /// <summary>Planilha do Excel (<c>.xlsx</c>): os mesmos dados do CSV, mas numero entra como numero.</summary>
    Xlsx,

    /// <summary>PDF de uma pagina com a imagem do grafico.</summary>
    Pdf
}
