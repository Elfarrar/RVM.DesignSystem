using System.Text.RegularExpressions;

namespace RVM.DesignSystem.Theming;

/// <summary>
/// O que o usuario escolheu no <see cref="RvmThemePicker"/>. Imutavel: troque com <c>with</c> e entregue ao
/// <see cref="RvmThemeState"/>, ou passe direto ao <see cref="RvmThemeProvider.Settings"/>. Contrato com o RVM.UI (DSGN-017).
/// </summary>
/// <param name="Palette">Id da paleta (<see cref="RvmPalette.Id"/>). Padrao: <c>"blue"</c>, o cobalto do kit.</param>
/// <param name="Mode">Claro, escuro ou acompanhando o sistema.</param>
/// <param name="FontScale">Tamanho da fonte.</param>
/// <param name="HighContrast">Texto de apoio vira texto principal e as linhas ganham 3:1.</param>
/// <param name="ReducedMotion">Desliga animacoes e transicoes.</param>
public sealed partial record RvmThemeSettings(
    string Palette = "blue",
    RvmThemeMode Mode = RvmThemeMode.Light,
    RvmFontScale FontScale = RvmFontScale.Default,
    bool HighContrast = false,
    bool ReducedMotion = false)
{
    private const string Versao = "v1";

    /// <summary>O kit como ele e: azul, claro, tamanho padrao.</summary>
    public static RvmThemeSettings Default { get; } = new();

    [GeneratedRegex("^[a-z0-9][a-z0-9-]{0,39}$")]
    private static partial Regex IdValido();

    /// <summary>Texto curto para cookie, localStorage ou coluna do banco: <c>v1|blue|dark|large|0|1</c>.</summary>
    public string Serialize() =>
        string.Join('|', Versao, Palette, Mode.ToString().ToLowerInvariant(), FontScale.ToString().ToLowerInvariant(),
            HighContrast ? "1" : "0", ReducedMotion ? "1" : "0");

    /// <summary>
    /// Le o que <see cref="Serialize"/> gravou. Nunca lanca: campo invalido ou ausente volta ao padrao, porque um
    /// cookie mexido a mao nao pode derrubar a pagina.
    /// </summary>
    public static RvmThemeSettings Parse(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return Default;
        }

        // O cookie e gravado com encodeURIComponent: o "|" pode chegar como %7C.
        if (texto.Contains('%', StringComparison.Ordinal))
        {
            try
            {
                texto = Uri.UnescapeDataString(texto);
            }
            catch (UriFormatException)
            {
                return Default;
            }
        }

        var partes = texto.Trim().Split('|');
        if (partes.Length < 2 || partes[0] != Versao)
        {
            return Default;
        }

        string Parte(int i) => i < partes.Length ? partes[i] : string.Empty;
        var paleta = IdValido().IsMatch(Parte(1)) ? Parte(1) : Default.Palette;
        var modo = Enum.TryParse<RvmThemeMode>(Parte(2), ignoreCase: true, out var m) && Enum.IsDefined(m) ? m : Default.Mode;
        var escala = Enum.TryParse<RvmFontScale>(Parte(3), ignoreCase: true, out var e) && Enum.IsDefined(e) ? e : Default.FontScale;
        return new RvmThemeSettings(paleta, modo, escala, Parte(4) == "1", Parte(5) == "1");
    }

    /// <summary>Id valido de paleta (o que vai no atributo). Invalido vira o padrao.</summary>
    internal string PaletteAttribute => Palette is not null && IdValido().IsMatch(Palette) ? Palette : Default.Palette;

    /// <summary>Valor do <c>data-theme</c>: <c>light</c>, <c>dark</c> ou <c>system</c>.</summary>
    internal string ModeAttribute => Mode switch
    {
        RvmThemeMode.Dark => "dark",
        RvmThemeMode.System => "system",
        _ => "light"
    };

    /// <summary>Valor do <c>data-rvm-font-scale</c>; nulo no tamanho padrao (o atributo some).</summary>
    internal string? FontScaleAttribute => FontScale switch
    {
        RvmFontScale.Small => "small",
        RvmFontScale.Large => "large",
        RvmFontScale.ExtraLarge => "xlarge",
        _ => null
    };
}
