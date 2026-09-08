namespace RVM.DesignSystem.Theming;

/// <summary>
/// Os papeis semanticos de cor de um modo (claro ou escuro) de um tema.
/// </summary>
/// <remarks>
/// ⚠️ <b>A lista e FECHADA de proposito</b> (`04-modelo-de-dados.md` § Papeis semanticos).
/// Cada papel que pinta um fundo tem o seu <c>On*</c>, e o par inteiro e medido pelo teste de
/// contraste. Acrescentar um papel de fundo sem o <c>On*</c> correspondente cria um par que
/// ninguem verifica — que e exatamente o buraco que este projeto existe para fechar.
///
/// Nome em ingles por decisao registrada (`04` § Pendencias, item 1): e o vocabulario universal
/// de design token. O PT-BR do ecossistema vale para texto de usuario final, nao para nome de API.
/// </remarks>
public sealed record RvmPalette
{
    /// <summary>Cor principal da marca: acao primaria, link, estado selecionado.</summary>
    public required string Primary { get; init; }

    /// <summary>Texto e icone sobre <see cref="Primary"/>.</summary>
    public required string OnPrimary { get; init; }

    /// <summary>Fundo suave derivado da marca: chip, badge, destaque discreto.</summary>
    public required string PrimaryContainer { get; init; }

    /// <summary>Texto e icone sobre <see cref="PrimaryContainer"/>.</summary>
    public required string OnPrimaryContainer { get; init; }

    /// <summary>Cor de apoio da marca: acao secundaria, enfase alternativa.</summary>
    public required string Secondary { get; init; }

    /// <summary>Texto e icone sobre <see cref="Secondary"/>.</summary>
    public required string OnSecondary { get; init; }

    /// <summary>Estado de sucesso: confirmacao, operacao concluida.</summary>
    public required string Success { get; init; }

    /// <summary>Texto e icone sobre <see cref="Success"/>.</summary>
    public required string OnSuccess { get; init; }

    /// <summary>Estado de atencao: risco reversivel, pendencia.</summary>
    public required string Warning { get; init; }

    /// <summary>Texto e icone sobre <see cref="Warning"/>.</summary>
    public required string OnWarning { get; init; }

    /// <summary>Estado de erro: falha, acao destrutiva.</summary>
    public required string Danger { get; init; }

    /// <summary>Texto e icone sobre <see cref="Danger"/>.</summary>
    public required string OnDanger { get; init; }

    /// <summary>Estado informativo: aviso neutro, dica.</summary>
    public required string Info { get; init; }

    /// <summary>Texto e icone sobre <see cref="Info"/>.</summary>
    public required string OnInfo { get; init; }

    /// <summary>Superficie de conteudo — o fundo padrao de painel e dialogo.</summary>
    public required string Surface { get; init; }

    /// <summary>
    /// Texto e icone sobre <see cref="Surface"/>, e tambem sobre
    /// <see cref="SurfaceRaised"/> e <see cref="SurfaceSunken"/>.
    /// </summary>
    public required string OnSurface { get; init; }

    /// <summary>Superficie elevada (card, menu flutuante). Compartilha o <see cref="OnSurface"/>.</summary>
    public required string SurfaceRaised { get; init; }

    /// <summary>Superficie rebaixada (poco, campo de entrada). Compartilha o <see cref="OnSurface"/>.</summary>
    public required string SurfaceSunken { get; init; }

    /// <summary>Fundo da pagina, atras das superficies.</summary>
    public required string Background { get; init; }

    /// <summary>Texto e icone sobre <see cref="Background"/>.</summary>
    public required string OnBackground { get; init; }

    /// <summary>
    /// Divisoria decorativa. Nao precisa de 3:1 — nao comunica estado nem delimita controle.
    /// </summary>
    public required string Border { get; init; }

    /// <summary>
    /// Limite de controle interativo (contorno de input, de checkbox). Precisa de 3:1
    /// contra a superficie adjacente (WCAG 2.1 SC 1.4.11).
    /// </summary>
    public required string BorderStrong { get; init; }

    /// <summary>Fundo de controle desabilitado.</summary>
    public required string Disabled { get; init; }

    /// <summary>Texto e icone sobre <see cref="Disabled"/>.</summary>
    public required string OnDisabled { get; init; }

    /// <summary>Anel de foco de teclado. Precisa de 3:1 contra a superficie adjacente.</summary>
    public required string FocusRing { get; init; }

    /// <summary>
    /// Os pares <c>x</c> / <c>on-x</c> com o contraste minimo que cada um deve atingir.
    /// </summary>
    /// <returns>
    /// Tuplas de nome do par, cor de frente, cor de fundo e razao minima exigida.
    /// </returns>
    /// <remarks>
    /// E a fonte unica que o teste de contraste percorre. Papel novo entra aqui, ou nao e medido.
    ///
    /// Repare que <c>Disabled</c> pede 3:1 e nao 4.5:1: a WCAG 2.1 isenta explicitamente
    /// componente desabilitado (SC 1.4.3, "Incidental"), e forcar 4.5 faria o desabilitado ficar
    /// tao legivel quanto o habilitado — ou seja, deixaria de comunicar que esta desabilitado.
    /// A regra existe para acessibilidade; aplica-la aqui trabalharia contra ela.
    /// </remarks>
    public IEnumerable<(string Nome, string Frente, string Fundo, double Minimo)> PairsToVerify()
    {
        yield return ("primary/on-primary", OnPrimary, Primary, RvmContrast.NormalText);
        yield return ("primary-container/on-primary-container", OnPrimaryContainer, PrimaryContainer, RvmContrast.NormalText);
        yield return ("secondary/on-secondary", OnSecondary, Secondary, RvmContrast.NormalText);
        yield return ("success/on-success", OnSuccess, Success, RvmContrast.NormalText);
        yield return ("warning/on-warning", OnWarning, Warning, RvmContrast.NormalText);
        yield return ("danger/on-danger", OnDanger, Danger, RvmContrast.NormalText);
        yield return ("info/on-info", OnInfo, Info, RvmContrast.NormalText);
        yield return ("surface/on-surface", OnSurface, Surface, RvmContrast.NormalText);
        yield return ("background/on-background", OnBackground, Background, RvmContrast.NormalText);

        // As superficies elevada e rebaixada nao tem On* proprio: emprestam o on-surface.
        // Por isso PRECISAM ser medidas contra ele — e onde um cinza mal escolhido some.
        yield return ("surface-raised/on-surface", OnSurface, SurfaceRaised, RvmContrast.NormalText);
        yield return ("surface-sunken/on-surface", OnSurface, SurfaceSunken, RvmContrast.NormalText);

        // Limite de controle e anel de foco: 3:1 contra a superficie adjacente.
        yield return ("border-strong/surface", BorderStrong, Surface, RvmContrast.LargeTextOrUi);
        yield return ("focus-ring/surface", FocusRing, Surface, RvmContrast.LargeTextOrUi);

        // Desabilitado: 3:1. Ver o comentario acima — nao e afrouxamento, e a regra correta.
        yield return ("disabled/on-disabled", OnDisabled, Disabled, RvmContrast.LargeTextOrUi);
    }
}
