namespace RVM.DesignSystem.Theming;

/// <summary>
/// Os limiares da WCAG 2.1 nivel AA e o ajuste que garante que uma cor os atinja.
/// </summary>
/// <remarks>
/// Existe separado do <see cref="RvmColor"/> porque e usado por tres lugares com intencoes
/// diferentes: o <c>FromSeed</c> usa para DERIVAR cor que ja nasce aprovada, o teste unitario
/// usa para REPROVAR o build, e a pagina de cor do site usa para MOSTRAR o numero ao visitante.
/// </remarks>
public static class RvmContrast
{
    /// <summary>Texto normal: 4.5:1 (WCAG 2.1 SC 1.4.3).</summary>
    public const double NormalText = 4.5;

    /// <summary>Texto grande e limites de controle: 3:1 (SC 1.4.3 e 1.4.11).</summary>
    public const double LargeTextOrUi = 3.0;

    /// <summary>
    /// Devolve <paramref name="color"/> escurecida ou clareada o minimo necessario para atingir
    /// <paramref name="target"/> contra <paramref name="against"/>, preservando matiz e croma.
    /// </summary>
    /// <remarks>
    /// Anda a luminosidade OKLCH em passos pequenos a partir do valor atual, na direcao que
    /// afasta do fundo, e para no primeiro valor que atinge o alvo — por isso o resultado e a
    /// cor mais proxima da original que ainda passa, e nao um preto ou branco chapado.
    ///
    /// E o que faz o portao de contraste ser barato: em vez de derivar uma paleta e torcer para
    /// ela passar no teste, a paleta **nasce** aprovada. O teste continua existindo para pegar
    /// paleta escrita a mao e regressao de derivacao.
    /// </remarks>
    public static RvmColor Ensure(RvmColor color, RvmColor against, double target)
    {
        if (RvmColor.Contrast(color, against) >= target)
        {
            return color;
        }

        // Afasta do fundo: fundo claro pede cor mais escura, e vice-versa.
        var direction = against.RelativeLuminance > 0.5 ? -1 : 1;
        var (l, _, _) = color.ToOklch();

        for (var step = 1; step <= 200; step++)
        {
            var candidate = color.WithLightness(l + direction * step * 0.005);
            if (RvmColor.Contrast(candidate, against) >= target)
            {
                return candidate;
            }
        }

        // Saturado no extremo: preto ou branco, o que der mais contraste. Acontece quando o
        // fundo esta no meio da escala e nem preto nem branco chegam ao alvo — ai devolvemos
        // o melhor possivel e o TESTE reprova, que e o comportamento certo: falhar visivel.
        var black = new RvmColor(0, 0, 0);
        var white = new RvmColor(255, 255, 255);
        return RvmColor.Contrast(black, against) >= RvmColor.Contrast(white, against) ? black : white;
    }

    /// <summary>
    /// Escolhe entre preto e branco o texto mais legivel sobre <paramref name="background"/>.
    /// </summary>
    public static RvmColor BestForegroundOn(RvmColor background)
    {
        var black = new RvmColor(16, 16, 20);
        var white = new RvmColor(255, 255, 255);
        return RvmColor.Contrast(black, background) >= RvmColor.Contrast(white, background) ? black : white;
    }
}
