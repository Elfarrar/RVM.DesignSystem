using System.Globalization;

namespace RVM.DesignSystem.Components.Chart;

/// <summary>
/// Um eixo numerico: dominio com limites "redondos" e as marcas da grade. O mesmo criterio do d3 (passo
/// de 1, 2, 2,5, 5 ou 10 vezes uma potencia de 10), para os eixos lerem 0, 20 mil, 40 mil.
/// </summary>
internal readonly record struct Escala(double Min, double Max, double Passo)
{
    public static Escala Arredondada(double min, double max, int marcas = 5, bool inteiro = false)
    {
        if (double.IsNaN(min) || double.IsNaN(max) || double.IsInfinity(min) || double.IsInfinity(max))
        {
            (min, max) = (0, 1);
        }

        if (min > max)
        {
            (min, max) = (max, min);
        }

        if (max - min < 1e-12)
        {
            // Todos iguais (ou um so valor): abre uma faixa em volta para a barra ter altura.
            var folga = Math.Abs(max) < 1e-12 ? 1 : Math.Abs(max) * 0.5;
            max += folga;
            if (min < 0) min -= folga;
        }

        var bruto = (max - min) / Math.Max(1, marcas);
        var potencia = Math.Pow(10, Math.Floor(Math.Log10(bruto)));
        var normalizado = bruto / potencia;
        var passo = normalizado switch
        {
            <= 1 => 1,
            <= 2 => 2,
            <= 2.5 => 2.5,
            <= 5 => 5,
            _ => 10
        } * potencia;

        if (inteiro)
        {
            passo = Math.Max(1, Math.Ceiling(passo));
        }

        var inicio = Math.Floor(min / passo + 1e-9) * passo;
        var fim = Math.Ceiling(max / passo - 1e-9) * passo;
        return new Escala(inicio, fim, passo);
    }

    /// <summary>
    /// A parte da escala entre duas fracoes (0 = Min, 1 = Max), com o passo recalculado para a faixa nova.
    /// E o que o zoom mostra: os limites sao exatos (nao arredondam de volta para fora da janela) e so as
    /// marcas seguem numeros redondos.
    /// </summary>
    public Escala Recortada(double inicio, double fim, int marcas = 5)
    {
        if (fim - inicio >= 1 - 1e-9)
        {
            return this;
        }

        var faixa = Max - Min;
        var min = Min + faixa * inicio;
        var max = Min + faixa * fim;
        return new Escala(min, max, Arredondada(min, max, marcas).Passo);
    }

    public IEnumerable<double> Marcas
    {
        get
        {
            // Multiplos do passo dentro da faixa: com a escala inteira o Min ja e um deles; recortada pelo
            // zoom, a primeira marca e a proxima "redonda" depois do inicio da janela.
            var primeira = Math.Ceiling(Min / Passo - 1e-9) * Passo;
            // Por indice, e nao somando o passo a cada volta: somar acumula erro quando ha muitas marcas.
            var total = (int)Math.Floor((Max - primeira) / Passo + 1e-9);
            for (var i = 0; i <= total; i++)
            {
                yield return Math.Round(primeira + i * Passo, 10);
            }
        }
    }

    /// <summary>Posicao do valor entre dois pixels (inicio para Min, fim para Max).</summary>
    public double Posicao(double valor, double inicio, double fim)
        => Max - Min < 1e-12 ? inicio : inicio + (valor - Min) / (Max - Min) * (fim - inicio);

    /// <summary>Numero para atributo SVG: sempre com ponto, nunca a virgula do pt-BR.</summary>
    public static string N(double valor) => Math.Round(valor, 2).ToString("0.##", CultureInfo.InvariantCulture);
}
