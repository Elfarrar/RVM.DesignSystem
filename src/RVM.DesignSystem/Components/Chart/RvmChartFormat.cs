using System.Globalization;

namespace RVM.DesignSystem.Components.Chart;

/// <summary>Formatos de numero dos graficos, em PT-BR fixo (nao dependem da cultura do navegador).</summary>
public static class RvmChartFormat
{
    /// <summary>"950", "1,5 mil", "60 mil", "1,2 mi", "3 bi". O padrao dos eixos e da dica.</summary>
    public static string Compact(double valor)
    {
        var abs = Math.Abs(valor);
        var (divisor, sufixo) = abs switch
        {
            >= 1e9 => (1e9, " bi"),
            >= 1e6 => (1e6, " mi"),
            >= 1e3 => (1e3, " mil"),
            _ => (1d, "")
        };

        return Decimal(valor / divisor, divisor == 1 ? "0.##" : "0.#") + sufixo;
    }

    /// <summary>
    /// O compacto para uma marca de eixo: casas decimais suficientes para o passo da escala. Sem isso, um
    /// eixo de 940 a 1.060 escrevia "1 mil" em tres marcas diferentes (achado ao testar o review).
    /// </summary>
    internal static string CompactForAxis(double valor, double passo)
    {
        var abs = Math.Max(Math.Abs(valor), Math.Abs(passo));
        var (divisor, sufixo) = abs switch
        {
            >= 1e9 => (1e9, " bi"),
            >= 1e6 => (1e6, " mi"),
            >= 1e3 => (1e3, " mil"),
            _ => (1d, "")
        };

        var passoNaUnidade = Math.Abs(passo) / divisor;
        var casas = passoNaUnidade <= 0 ? 0 : Math.Clamp((int)Math.Ceiling(-Math.Log10(passoNaUnidade) - 1e-9), 0, 3);
        var formato = casas == 0 ? "0" : "0." + new string('#', casas);
        return Decimal(Math.Round(valor / divisor, casas), formato) + sufixo;
    }

    /// <summary>Numero com separador de milhar e ate duas casas: "1.250,5".</summary>
    public static string Number(double valor)
        => valor.ToString("#,0.##", CultureInfo.InvariantCulture).Replace(',', '').Replace('.', ',').Replace('', '.');

    private static string Decimal(double valor, string formato)
        => valor.ToString(formato, CultureInfo.InvariantCulture).Replace('.', ',');
}
