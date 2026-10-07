namespace RVM.DesignSystem.Components.Chart;

/// <summary>Rampa de cores das fatias e series (contrato com o RVM.UI, DSGN-017).</summary>
public enum RvmChartPalette
{
    /// <summary>Degraus do primary: a primeira fatia leva a cor cheia e as demais clareiam. E o padrao.</summary>
    Monochromatic,

    /// <summary>As cores dos papeis do tema. A cor nao quer dizer nada aqui: para estado, use o texto, nao a fatia.</summary>
    FullColor
}
