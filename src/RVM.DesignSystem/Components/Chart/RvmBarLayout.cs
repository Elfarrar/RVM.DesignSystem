namespace RVM.DesignSystem.Components.Chart;

/// <summary>Como o <c>RvmBarChart</c> arruma as barras (contrato com o RVM.UI, DSGN-017).</summary>
public enum RvmBarLayout
{
    /// <summary>Colunas que sobem da base, as categorias embaixo (o mesmo desenho do <c>RvmColumnChart</c>).</summary>
    Vertical,

    /// <summary>Barras deitadas, as categorias a esquerda. E o padrao do <c>RvmBarChart</c>.</summary>
    Horizontal,

    /// <summary>
    /// Barras deitadas a partir de um zero no meio, com o eixo simetrico. Com exatamente duas series, a primeira
    /// vai para a direita e a segunda para a esquerda (entrada e saida, ganho e perda: o lado vem da serie, e o
    /// eixo mostra o tamanho dos dois lados); com outra quantidade, o lado vem do sinal e o negativo vai para a
    /// esquerda.
    /// </summary>
    Diverging
}
