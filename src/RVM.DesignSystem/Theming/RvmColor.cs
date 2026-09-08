using System.Globalization;

namespace RVM.DesignSystem.Theming;

/// <summary>
/// Uma cor sRGB, com as conversoes e o calculo de contraste que o motor de tema precisa.
/// </summary>
/// <remarks>
/// E publico de proposito: a mesma funcao que reprova o build tambem alimenta a pagina de cor
/// do site de documentacao (RF-24). Uma implementacao, dois usos — se fossem duas, uma delas
/// ficaria errada sem ninguem perceber.
///
/// A manipulacao acontece em OKLCH, nao em HSL: em HSL, dois tons com o mesmo "L" tem brilho
/// percebido muito diferente conforme o matiz (um amarelo 50% parece muito mais claro que um
/// azul 50%), e uma paleta derivada assim fica visivelmente desequilibrada.
/// </remarks>
public readonly record struct RvmColor
{
    /// <summary>Componente vermelho, 0-255.</summary>
    public byte R { get; }

    /// <summary>Componente verde, 0-255.</summary>
    public byte G { get; }

    /// <summary>Componente azul, 0-255.</summary>
    public byte B { get; }

    /// <summary>Cria a cor a partir dos tres componentes sRGB.</summary>
    /// <param name="r">Vermelho, 0-255.</param>
    /// <param name="g">Verde, 0-255.</param>
    /// <param name="b">Azul, 0-255.</param>
    public RvmColor(byte r, byte g, byte b) => (R, G, B) = (r, g, b);

    /// <summary>Le uma cor em <c>#RGB</c> ou <c>#RRGGBB</c>.</summary>
    /// <exception cref="FormatException">Quando o texto nao e um hexadecimal de cor valido.</exception>
    public static RvmColor Parse(string hex)
    {
        ArgumentNullException.ThrowIfNull(hex);
        var s = hex.Trim().TrimStart('#');

        if (s.Length == 3)
        {
            s = string.Concat(s[0], s[0], s[1], s[1], s[2], s[2]);
        }

        if (s.Length != 6 || !int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
        {
            throw new FormatException($"'{hex}' nao e uma cor hexadecimal valida. Use #RGB ou #RRGGBB.");
        }

        return new RvmColor(
            byte.Parse(s.AsSpan(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            byte.Parse(s.AsSpan(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            byte.Parse(s.AsSpan(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
    }

    /// <summary>Devolve a cor como <c>#RRGGBB</c>, sempre em maiuscula.</summary>
    public string ToHex() => string.Create(CultureInfo.InvariantCulture, $"#{R:X2}{G:X2}{B:X2}");

    /// <summary>Luminancia relativa segundo a WCAG 2.1.</summary>
    public double RelativeLuminance
    {
        get
        {
            static double Channel(byte c)
            {
                var v = c / 255.0;
                return v <= 0.04045 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
            }

            return 0.2126 * Channel(R) + 0.7152 * Channel(G) + 0.0722 * Channel(B);
        }
    }

    /// <summary>
    /// Razao de contraste entre duas cores, de 1 (identicas) a 21 (preto contra branco).
    /// </summary>
    public static double Contrast(RvmColor a, RvmColor b)
    {
        var (la, lb) = (a.RelativeLuminance, b.RelativeLuminance);
        var (hi, lo) = la > lb ? (la, lb) : (lb, la);
        return (hi + 0.05) / (lo + 0.05);
    }

    /// <summary>Converte para OKLCH: luminosidade 0-1, croma e matiz em graus.</summary>
    public (double L, double C, double H) ToOklch()
    {
        static double Linear(byte c)
        {
            var v = c / 255.0;
            return v <= 0.04045 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
        }

        double r = Linear(R), g = Linear(G), b = Linear(B);

        var l = Math.Cbrt(0.4122214708 * r + 0.5363325363 * g + 0.0514459929 * b);
        var m = Math.Cbrt(0.2119034982 * r + 0.6806995451 * g + 0.1073969566 * b);
        var s = Math.Cbrt(0.0883024619 * r + 0.2817188376 * g + 0.6299787005 * b);

        var okL = 0.2104542553 * l + 0.7936177850 * m - 0.0040720468 * s;
        var okA = 1.9779984951 * l - 2.4285922050 * m + 0.4505937099 * s;
        var okB = 0.0259040371 * l + 0.7827717662 * m - 0.8086757660 * s;

        var chroma = Math.Sqrt(okA * okA + okB * okB);
        var hue = Math.Atan2(okB, okA) * 180.0 / Math.PI;
        if (hue < 0) hue += 360;

        return (okL, chroma, hue);
    }

    /// <summary>Constroi a cor a partir de OKLCH, recortando no gamut sRGB.</summary>
    public static RvmColor FromOklch(double l, double c, double hueDegrees)
    {
        var h = hueDegrees * Math.PI / 180.0;
        double a = c * Math.Cos(h), bb = c * Math.Sin(h);

        var l_ = l + 0.3963377774 * a + 0.2158037573 * bb;
        var m_ = l - 0.1055613458 * a - 0.0638541728 * bb;
        var s_ = l - 0.0894841775 * a - 1.2914855480 * bb;

        double lc = l_ * l_ * l_, mc = m_ * m_ * m_, sc = s_ * s_ * s_;

        var r = 4.0767416621 * lc - 3.3077115913 * mc + 0.2309699292 * sc;
        var g = -1.2684380046 * lc + 2.6097574011 * mc - 0.3413193965 * sc;
        var b = -0.0041960863 * lc - 0.7034186147 * mc + 1.7076147010 * sc;

        static byte Encode(double x)
        {
            x = Math.Clamp(x, 0.0, 1.0);
            var v = x <= 0.0031308 ? 12.92 * x : 1.055 * Math.Pow(x, 1.0 / 2.4) - 0.055;
            return (byte)Math.Round(Math.Clamp(v, 0.0, 1.0) * 255.0);
        }

        return new RvmColor(Encode(r), Encode(g), Encode(b));
    }

    /// <summary>Mesma cor com outra luminosidade OKLCH (0-1), preservando matiz e croma.</summary>
    public RvmColor WithLightness(double lightness)
    {
        var (_, c, h) = ToOklch();
        return FromOklch(Math.Clamp(lightness, 0.0, 1.0), c, h);
    }

    /// <summary>Mesma cor com o croma multiplicado por <paramref name="factor"/>.</summary>
    public RvmColor ScaleChroma(double factor)
    {
        var (l, c, h) = ToOklch();
        return FromOklch(l, Math.Max(0, c * factor), h);
    }

    /// <summary>Mistura linear em OKLab. <paramref name="amount"/> 0 devolve esta cor, 1 devolve a outra.</summary>
    public RvmColor MixWith(RvmColor other, double amount)
    {
        amount = Math.Clamp(amount, 0.0, 1.0);
        var (l1, c1, h1) = ToOklch();
        var (l2, c2, h2) = other.ToOklch();

        // Interpola o matiz pelo caminho curto — sem isso, misturar 350° com 10° passa
        // pelo verde em vez de cruzar o vermelho.
        var delta = h2 - h1;
        if (delta > 180) delta -= 360;
        if (delta < -180) delta += 360;

        return FromOklch(
            l1 + (l2 - l1) * amount,
            c1 + (c2 - c1) * amount,
            h1 + delta * amount);
    }
}
