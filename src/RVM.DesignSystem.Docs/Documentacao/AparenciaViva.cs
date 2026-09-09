using RVM.DesignSystem.Theming;

namespace RVM.DesignSystem.Docs.Documentacao;

/// <summary>
/// Tinge as superficies de um tema com a matiz da marca.
/// </summary>
/// <remarks>
/// Parte do preview temporario de aparencia (DSGN-025/026) — <b>sai junto com ele</b> quando a
/// direcao for escolhida. Se a direcao vencer, isto vira feature do motor de tema, com teste
/// unitario proprio; nao pacote ainda.
///
/// <para>
/// ⚠️ <b>Isto existe porque tingir superficie NAO pode ser feito em CSS.</b> Os presets "sobrio"
/// e "marcante" mexem em sombra, tipografia e raio, que sao inocuos. Superficie e outra coisa:
/// ela e o fundo contra o qual TODA cor da paleta foi verificada. Um cinza tingido escrito a mao
/// numa folha de estilo furaria a garantia em silencio — foi assim que 14 paginas reprovaram no
/// axe em 08/09/2026, com o teste unitario aprovando. Aqui a troca passa por
/// <see cref="RvmPalette.EnsureContrast"/>, que re-deriva os papeis de frente contra as
/// superficies novas.
/// </para>
///
/// <para>
/// ⚠️ <b>O <c>EnsureContrast</c> nao re-deriva <c>on-surface</c> nem <c>on-background</c></b> —
/// ele ajusta os papeis de frente coloridos. Por isso o tingimento mexe muito na CROMA e pouco
/// na LUMINOSIDADE: e a luminosidade que carrega o contraste do texto. Tingir e diferente de
/// escurecer, e so o primeiro e seguro sem mexer no texto junto.
/// </para>
/// </remarks>
public static class AparenciaViva
{
    /// <summary>Devolve o tema com as superficies tingidas pela matiz da primaria.</summary>
    public static RvmTheme Tingir(RvmTheme tema)
    {
        ArgumentNullException.ThrowIfNull(tema);

        return tema with
        {
            Light = Tingida(tema.Light, claro: true),
            Dark = Tingida(tema.Dark, claro: false),
        };
    }

    private static RvmPalette Tingida(RvmPalette p, bool claro)
    {
        // A matiz vem da PRIMARIA, nao de um valor fixo: assim a superficie acompanha a marca de
        // quem usa, inclusive uma paleta montada em /fundamentos/paleta.
        var matiz = RvmColor.Parse(p.Primary).ToOklch().H;

        static string Cor(double l, double c, double h) => RvmColor.FromOklch(l, c, h).ToHex();

        var tingida = claro
            ? p with
            {
                Background = Cor(0.955, 0.020, matiz),
                Surface = Cor(0.988, 0.008, matiz),
                SurfaceRaised = Cor(1.000, 0.000, matiz),
                SurfaceSunken = Cor(0.918, 0.032, matiz),
                Border = Cor(0.880, 0.026, matiz),
                BorderStrong = Cor(0.780, 0.038, matiz),
            }
            : p with
            {
                Background = Cor(0.175, 0.022, matiz),
                Surface = Cor(0.215, 0.026, matiz),
                SurfaceRaised = Cor(0.258, 0.030, matiz),
                SurfaceSunken = Cor(0.140, 0.020, matiz),
                Border = Cor(0.320, 0.028, matiz),
                BorderStrong = Cor(0.430, 0.036, matiz),
            };

        // OBRIGATORIO depois de trocar superficie — a mesma linha que o RvmThemes.Rvm executa
        // pelo mesmo motivo. Sem ela a paleta continua compilando e o texto fica ilegivel.
        return tingida.EnsureContrast();
    }
}
