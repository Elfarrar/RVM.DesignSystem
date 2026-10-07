namespace RVM.DesignSystem.Components.Chart;

/// <summary>
/// Que classe de cor cada serie ou fatia recebe, por posicao. Mora num lugar so porque a legenda solta
/// (<see cref="RvmChartLegend"/>) precisa dar a MESMA cor que o grafico ao lado deu para a mesma posicao: se
/// cada um calculasse a sua, a legenda apontaria para uma cor que o desenho nao usou.
/// <para>
/// A rampa fica em C#, e nao em CSS, porque depende da quantidade de fatias (com duas o monocromatico pula do
/// cheio para o mais claro). O CSS so liga a classe a um token: <c>rvm-cor-primary-NNN</c> e o primary
/// misturado ao papel, que acompanha o tema.
/// </para>
/// </summary>
internal static class RampaDoGrafico
{
    /// <summary>A partir de quantas fatias o monocromatico desiste e volta as cores dos papeis.</summary>
    private const int DegrausDoMonocromatico = 5;

    /// <summary>
    /// Os degraus do primary nos radiais, por quantidade de fatias (a mesma tabela do RVM.UI): a primeira
    /// leva sempre o cheio e as demais clareiam.
    /// </summary>
    private static readonly int[][] RadialMono =
    [
        [500],
        [500, 100],
        [500, 300, 100],
        [500, 300, 200, 100],
        [500, 400, 300, 200, 100],
    ];

    /// <summary>O medidor tem a rampa propria do kit: com dois pedacos <c>500 + 300</c>, com tres <c>500, 100, 300</c>.</summary>
    private static readonly int[][] MedidorMono =
    [
        [500],
        [500, 300],
        [500, 100, 300],
    ];

    /// <summary>
    /// A cor de serie dos graficos cartesianos (colunas, barras, linha, area): as cores dos papeis, na ordem,
    /// voltando ao comeco depois da sexta. E tambem o <see cref="RvmChartPalette.FullColor"/> dos radiais.
    /// </summary>
    internal static string Serie(int indice) => RvmChartBase<object>.ClasseDaCor(RvmChartBase<object>.CorDaPaleta(indice));

    /// <summary>A cor de uma fatia de pizza ou rosca.</summary>
    internal static string Radial(RvmChartPalette paleta, int indice, int total) => Rampa(RadialMono, paleta, indice, total);

    /// <summary>A cor de um pedaco do medidor.</summary>
    internal static string Medidor(RvmChartPalette paleta, int indice, int total) => Rampa(MedidorMono, paleta, indice, total);

    /// <summary>
    /// As barras divergentes: a primeira serie (a que vai para a direita) no primary, a segunda no erro. Aqui a
    /// cor e semantica, o grafico compara opostos (entrada e saida, ganho e perda).
    /// </summary>
    internal static RvmColor Divergente(int indice) => indice == 0 ? RvmColor.Primary : RvmColor.Error;

    /// <summary>A cor que a legenda solta da a uma posicao, conforme o grafico que ela acompanha.</summary>
    internal static string DaLegenda(RvmLegendRamp rampa, RvmChartPalette paleta, int indice, int total) => rampa switch
    {
        // Os graficos cartesianos do DS ignoram a paleta: a legenda tambem.
        RvmLegendRamp.Bar or RvmLegendRamp.Line => Serie(indice),
        RvmLegendRamp.Meter => Medidor(paleta, indice, total),
        RvmLegendRamp.Diverging => RvmChartBase<object>.ClasseDaCor(Divergente(indice)),
        _ => Radial(paleta, indice, total),
    };

    /// <summary>
    /// A mesma cor do <see cref="Radial"/>/<see cref="Medidor"/>, mas como valor CSS e nao classe: os graficos radiais
    /// pintam por variavel inline. Os percentuais casam com as classes <c>rvm-cor-primary-NNN</c> do
    /// <c>RvmChartBase.razor.css</c> — mudou la, mude aqui (a legenda e o grafico tem que dar a mesma cor).
    /// </summary>
    internal static string CorCss(RvmChartPalette paleta, int indice, int total, bool medidor)
    {
        var classe = Rampa(medidor ? MedidorMono : RadialMono, paleta, indice, total);
        return classe switch
        {
            "rvm-cor-primary-400" => Mistura(80),
            "rvm-cor-primary-300" => Mistura(60),
            "rvm-cor-primary-200" => Mistura(42),
            "rvm-cor-primary-100" => Mistura(26),
            _ => $"var(--rvm-color-{classe["rvm-cor-".Length..]}-text)"
        };

        static string Mistura(int percentual)
            => $"color-mix(in srgb, var(--rvm-color-primary-text) {percentual}%, var(--rvm-color-background-paper))";
    }

    private static string Rampa(int[][] rampa, RvmChartPalette paleta, int indice, int total)
    {
        // Mais fatias do que degraus: a rampa nao tem como separar, e as cores dos papeis voltam.
        if (paleta is RvmChartPalette.FullColor || total > DegrausDoMonocromatico)
        {
            return Serie(indice);
        }

        var degraus = rampa[Math.Clamp(total, 1, rampa.Length) - 1];
        var degrau = degraus[Math.Clamp(indice, 0, degraus.Length - 1)];
        return degrau == 500 ? "rvm-cor-primary" : $"rvm-cor-primary-{degrau}";
    }
}
