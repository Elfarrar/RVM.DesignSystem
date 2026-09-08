using System.Globalization;
using System.Text;

namespace RVM.DesignSystem.Theming;

/// <summary>Modo de cor corrente.</summary>
public enum RvmThemeMode
{
    /// <summary>Modo claro.</summary>
    Light,

    /// <summary>Modo escuro.</summary>
    Dark,
}

/// <summary>
/// Um tema completo: nome, paleta clara e paleta escura.
/// </summary>
/// <remarks>
/// O C# e a fonte da verdade do tema; o CSS emitido por <see cref="ToCss"/> e a superficie que
/// os componentes consomem (`04-modelo-de-dados.md` § O espelho tipado).
/// </remarks>
public sealed record RvmTheme
{
    /// <summary>Nome do tema, exibido no seletor do site de documentacao.</summary>
    public required string Name { get; init; }

    /// <summary>Paleta do modo claro.</summary>
    public required RvmPalette Light { get; init; }

    /// <summary>Paleta do modo escuro.</summary>
    public required RvmPalette Dark { get; init; }

    /// <summary>
    /// Deriva um tema completo — os dois modos, todos os papeis — a partir de duas cores.
    /// </summary>
    /// <param name="name">Nome do tema, usado no seletor do site de documentacao.</param>
    /// <param name="primary">Cor principal da marca, em hexadecimal.</param>
    /// <param name="secondary">Cor de apoio, em hexadecimal.</param>
    /// <remarks>
    /// <b>Derivar por padrao, sobrescrever por excecao</b> (`06` § Como o tema e aplicado): um
    /// produto novo escreve duas linhas em vez das ~200 que cada app do ecossistema duplicou.
    ///
    /// Toda cor derivada passa por <see cref="RvmContrast.Ensure"/> contra o fundo em que vai
    /// ser usada. E o que faz a paleta <b>nascer</b> aprovada, em vez de ser derivada e depois
    /// torcer para passar no teste. O teste continua existindo — para pegar paleta escrita a mao
    /// e regressao desta funcao.
    /// </remarks>
    /// <returns>Tema com as duas paletas derivadas e ja verificadas contra os limiares AA.</returns>
    public static RvmTheme FromSeed(string name, string primary, string secondary)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var seed = RvmColor.Parse(primary);
        var accent = RvmColor.Parse(secondary);

        return new RvmTheme
        {
            Name = name,
            Light = BuildPalette(seed, accent, RvmThemeMode.Light),
            Dark = BuildPalette(seed, accent, RvmThemeMode.Dark),
        };
    }

    private static RvmPalette BuildPalette(RvmColor seed, RvmColor accent, RvmThemeMode mode)
    {
        var dark = mode == RvmThemeMode.Dark;
        var (_, seedChroma, seedHue) = seed.ToOklch();

        // Neutros levemente matizados pela primaria (~4% do croma dela): e o detalhe barato que
        // faz o cinza do Fiscal puxar para o teal e o do ObraEmDia para o verde, e faz a interface
        // parecer desenhada em vez de montada (`06` § Escala neutra).
        var neutralChroma = Math.Min(seedChroma * 0.04, 0.016);
        RvmColor Neutral(double l) => RvmColor.FromOklch(l, neutralChroma, seedHue);

        var background = dark ? Neutral(0.16) : Neutral(0.97);
        var surface = dark ? Neutral(0.19) : Neutral(0.99);
        var surfaceRaised = dark ? Neutral(0.24) : Neutral(1.00);
        var surfaceSunken = dark ? Neutral(0.13) : Neutral(0.95);

        var onSurface = RvmContrast.Ensure(Neutral(dark ? 0.94 : 0.22), surface, RvmContrast.NormalText);
        var onBackground = RvmContrast.Ensure(onSurface, background, RvmContrast.NormalText);

        // Texto secundario: um neutro entre o texto principal e a borda forte, medido contra a
        // superficie mais exigente logo abaixo. Nao e `onSurface` com opacidade — ver o
        // comentario em RvmPalette.OnSurfaceVariant.
        var onSurfaceVariantTone = Neutral(dark ? 0.72 : 0.44);

        // ⚠️ O contraste e garantido contra a superficie MAIS EXIGENTE da paleta, nao contra a
        // `surface`. Papel de marca e de estado tambem aparecem como TEXTO sobre o `background`
        // da pagina e sobre o `surface-sunken` de um campo — e no modo claro esses dois sao mais
        // escuros que a `surface`, entao passar contra ela nao garante nada.
        //
        // Medir so contra `surface` deixava `success` em 4.45 sobre o background: reprova AA, e o
        // teste unitario aprovava. Achado pelo axe no site em 08/09/2026.
        var piorFundo = new[] { surface, surfaceRaised, surfaceSunken, background }
            .OrderBy(c => Math.Abs(c.RelativeLuminance - 0.5))
            .First();

        // A cor da marca no escuro nao pode ser a mesma do claro: um roxo de L=38% sobre fundo
        // L=19% e ilegivel. Clareia-se ate passar, mantendo matiz e croma.
        var primaryTone = dark ? seed.WithLightness(0.74) : seed;
        var primary = RvmContrast.Ensure(primaryTone, piorFundo, RvmContrast.NormalText);
        var onPrimary = RvmContrast.Ensure(RvmContrast.BestForegroundOn(primary), primary, RvmContrast.NormalText);

        var secondaryTone = dark ? accent.WithLightness(0.74) : accent;
        var secondary = RvmContrast.Ensure(secondaryTone, piorFundo, RvmContrast.NormalText);
        var onSecondary = RvmContrast.Ensure(RvmContrast.BestForegroundOn(secondary), secondary, RvmContrast.NormalText);

        // Container: a marca diluida na superficie, para chip, badge e destaque suave.
        var primaryContainer = dark
            ? seed.WithLightness(0.30).ScaleChroma(0.75)
            : seed.WithLightness(0.93).ScaleChroma(0.55);
        var onPrimaryContainer = RvmContrast.Ensure(
            dark ? seed.WithLightness(0.90) : seed.WithLightness(0.30),
            primaryContainer,
            RvmContrast.NormalText);

        // Estados: matiz fixo (verde, ambar, vermelho, azul) — sao convencao cultural, nao marca.
        // O que varia por tema e a luminosidade, ajustada ao fundo do modo.
        RvmColor Status(double hue, double chroma)
        {
            var baseTone = RvmColor.FromOklch(dark ? 0.72 : 0.52, chroma, hue);
            return RvmContrast.Ensure(baseTone, piorFundo, RvmContrast.NormalText);
        }

        var success = Status(148, 0.14);
        var warning = Status(75, 0.14);
        var danger = Status(25, 0.19);
        var info = Status(245, 0.16);

        // Borda decorativa nao precisa de 3:1 (nao comunica estado); a de controle precisa.
        var border = dark ? Neutral(0.30) : Neutral(0.89);
        var borderStrong = RvmContrast.Ensure(Neutral(dark ? 0.49 : 0.63), surface, RvmContrast.LargeTextOrUi);

        // Desabilitado a 3:1 e deliberado — ver RvmPalette.PairsToVerify.
        var disabled = dark ? Neutral(0.26) : Neutral(0.92);
        var onDisabled = RvmContrast.Ensure(Neutral(0.55), disabled, RvmContrast.LargeTextOrUi);

        var onSurfaceVariant = RvmContrast.Ensure(onSurfaceVariantTone, piorFundo, RvmContrast.NormalText);

        var focusRing = RvmContrast.Ensure(primary, surface, RvmContrast.LargeTextOrUi);

        return new RvmPalette
        {
            Primary = primary.ToHex(),
            OnPrimary = onPrimary.ToHex(),
            PrimaryContainer = primaryContainer.ToHex(),
            OnPrimaryContainer = onPrimaryContainer.ToHex(),
            Secondary = secondary.ToHex(),
            OnSecondary = onSecondary.ToHex(),
            Success = success.ToHex(),
            OnSuccess = RvmContrast.Ensure(RvmContrast.BestForegroundOn(success), success, RvmContrast.NormalText).ToHex(),
            Warning = warning.ToHex(),
            OnWarning = RvmContrast.Ensure(RvmContrast.BestForegroundOn(warning), warning, RvmContrast.NormalText).ToHex(),
            Danger = danger.ToHex(),
            OnDanger = RvmContrast.Ensure(RvmContrast.BestForegroundOn(danger), danger, RvmContrast.NormalText).ToHex(),
            Info = info.ToHex(),
            OnInfo = RvmContrast.Ensure(RvmContrast.BestForegroundOn(info), info, RvmContrast.NormalText).ToHex(),
            Surface = surface.ToHex(),
            OnSurface = onSurface.ToHex(),
            OnSurfaceVariant = onSurfaceVariant.ToHex(),
            SurfaceRaised = surfaceRaised.ToHex(),
            SurfaceSunken = surfaceSunken.ToHex(),
            Background = background.ToHex(),
            OnBackground = onBackground.ToHex(),
            Border = border.ToHex(),
            BorderStrong = borderStrong.ToHex(),
            Disabled = disabled.ToHex(),
            OnDisabled = onDisabled.ToHex(),
            FocusRing = focusRing.ToHex(),
            // Preto com alfa, e nao um hex derivado: veu precisa de transparencia, e e o
            // unico papel da paleta que nao e uma cor solida.
            Scrim = dark ? "rgb(0 0 0 / 0.65)" : "rgb(0 0 0 / 0.5)",
        };
    }

    /// <summary>
    /// Emite o bloco CSS com as custom properties dos dois modos.
    /// </summary>
    /// <remarks>
    /// Metodo puro, e nao componente, de proposito: assim o que o navegador recebe pode ser
    /// verificado em teste unitario, sem renderizar nada. O provider so escreve esta string
    /// dentro de uma tag <c>style</c>.
    ///
    /// O modo escuro sai sob <c>[data-rvm-theme="dark"]</c> — trocar de modo e uma escrita de
    /// atributo no elemento raiz, sem re-render do Blazor e sem flash (`06` § 2).
    /// </remarks>
    /// <returns>Blocos <c>:root</c> prontos para ir dentro de uma tag <c>style</c>.</returns>
    public string ToCss()
    {
        var sb = new StringBuilder();

        sb.Append(":root{");
        AppendVariables(sb, Light);
        sb.Append("}\n");

        sb.Append(":root[data-rvm-theme=\"dark\"]{");
        AppendVariables(sb, Dark);
        sb.Append("}\n");

        // Quem nao escolheu segue o sistema. O :not() garante que a escolha explicita do usuario
        // (data-rvm-theme="light") vence a preferencia do SO — sem ele, quem usa o sistema no
        // escuro nao consegue forcar o site no claro.
        sb.Append("@media (prefers-color-scheme:dark){:root:not([data-rvm-theme=\"light\"]){");
        AppendVariables(sb, Dark);
        sb.Append("}}\n");

        return sb.ToString();
    }

    private static void AppendVariables(StringBuilder sb, RvmPalette p)
    {
        static void Var(StringBuilder sb, string name, string value) =>
            sb.Append(CultureInfo.InvariantCulture, $"--rvm-color-{name}:{value};");

        Var(sb, "primary", p.Primary);
        Var(sb, "on-primary", p.OnPrimary);
        Var(sb, "primary-container", p.PrimaryContainer);
        Var(sb, "on-primary-container", p.OnPrimaryContainer);
        Var(sb, "secondary", p.Secondary);
        Var(sb, "on-secondary", p.OnSecondary);
        Var(sb, "success", p.Success);
        Var(sb, "on-success", p.OnSuccess);
        Var(sb, "warning", p.Warning);
        Var(sb, "on-warning", p.OnWarning);
        Var(sb, "danger", p.Danger);
        Var(sb, "on-danger", p.OnDanger);
        Var(sb, "info", p.Info);
        Var(sb, "on-info", p.OnInfo);
        Var(sb, "surface", p.Surface);
        Var(sb, "on-surface", p.OnSurface);
        Var(sb, "on-surface-variant", p.OnSurfaceVariant);
        Var(sb, "surface-raised", p.SurfaceRaised);
        Var(sb, "surface-sunken", p.SurfaceSunken);
        Var(sb, "background", p.Background);
        Var(sb, "on-background", p.OnBackground);
        Var(sb, "border", p.Border);
        Var(sb, "border-strong", p.BorderStrong);
        Var(sb, "disabled", p.Disabled);
        Var(sb, "on-disabled", p.OnDisabled);
        Var(sb, "focus-ring", p.FocusRing);
        Var(sb, "scrim", p.Scrim);
    }
}
