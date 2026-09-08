namespace RVM.DesignSystem.Theming;

/// <summary>
/// Os temas que a biblioteca traz prontos.
/// </summary>
public static class RvmThemes
{
    /// <summary>
    /// O tema padrao: a identidade do ecossistema RVM.
    /// </summary>
    /// <remarks>
    /// Roxo do Visual Studio como principal, azul do VS Code como apoio — a mesma identidade do
    /// <c>rvmtech.com.br</c> (RVM.Curriculo). Decisao do Rafael, detalhada em
    /// `06-tokens-e-tematizacao.md` § A identidade da marca.
    ///
    /// ⚠️ <b>Sao os valores RENDERIZADOS do site, nao os nominais.</b> O `CLAUDE.md` do
    /// RVM.Curriculo cita <c>#68217A</c> e <c>#007ACC</c>, mas o CSS de la declara em OKLCH e
    /// renderiza mais escuro. O azul <b>nominal</b> <c>#007ACC</c> da 3.90 de contraste contra o
    /// fundo claro e <b>reprovaria</b> no teste; o renderizado <c>#006DBD</c> da 4.62.
    /// <b>Nao "corrigir" para os nominais.</b>
    /// </remarks>
    public static RvmTheme Rvm { get; } = BuildRvm();

    private static RvmTheme BuildRvm()
    {
        var derivado = RvmTheme.FromSeed("RVM", "#641974", "#006DBD");

        // "Derivar por padrao, sobrescrever por excecao" (`06` § Como o tema e aplicado).
        // Esta e a excecao: o cinza do rvmtech.com.br e parte da identidade, e a derivacao
        // padrao produz superficies quase brancas. Sem esta sobrescrita, a biblioteca e o site
        // de documentacao discordariam da propria marca — num design system, isso e o defeito
        // mais caro que existe.
        //
        // So os neutros do modo CLARO sao sobrescritos; o escuro segue derivado, porque o site
        // nao tem modo escuro para copiar. Todo valor aqui passa pelo mesmo teste de contraste
        // que os derivados — sobrescrever nao isenta ninguem do portao.
        var claroComOsCinzasDoSite = derivado.Light with
        {
            Background = "#EEEEF1",      // fundo da pagina, o cinza do site
            Surface = "#FCFCFE",         // superficie elevada do site (bg-elev)
            SurfaceRaised = "#FFFFFF",
            SurfaceSunken = "#E4E4EA",
            OnSurface = "#1E1E26",       // o texto do site
            OnBackground = "#1E1E26",
            Border = "#DEDEE4",
        };

        // ⚠️ OBRIGATORIO depois de sobrescrever superficie. O FromSeed derivou as cores contra
        // as superficies que ELE calculou; trocar os cinzas por baixo quebra a garantia EM
        // SILENCIO — a paleta continua compilando e o texto fica ilegivel.
        //
        // Sem esta chamada, quatro papeis ficam abaixo de AA sobre o surface-sunken deste tema
        // (secondary 4.23, success 4.18, warning 4.45, info 4.23). Foi assim que o axe reprovou
        // 14 paginas do site em 08/09/2026, com o teste unitario aprovando.
        return derivado with { Light = claroComOsCinzasDoSite.EnsureContrast() };
    }

    /// <summary>
    /// Os quatro apps do ecossistema, como temas de exemplo.
    /// </summary>
    /// <remarks>
    /// Existem para tres coisas (`06` § Migrando as paletas que ja existem): provar que
    /// <c>FromSeed</c> aguenta cores reais e nao so as bonitas do exemplo; alimentar o seletor de
    /// tema do site, onde o visitante ve o mesmo componente nas quatro identidades; e submeter as
    /// cores atuais ao teste de contraste.
    ///
    /// Nao sao migracao: os apps seguem no MudBlazor, sem prazo (`01` § Escopo).
    /// </remarks>
    public static class Samples
    {
        /// <summary>ERPAgro — verde de lavoura.</summary>
        public static RvmTheme ErpAgro { get; } = RvmTheme.FromSeed("ERPAgro", "#2E7D32", "#F9A825");

        /// <summary>ObraEmDia — verde-terra e ocre.</summary>
        public static RvmTheme ObraEmDia { get; } = RvmTheme.FromSeed("ObraEmDia", "#2A6E49", "#B07D2A");

        /// <summary>Fiscal — teal sobrio.</summary>
        public static RvmTheme Fiscal { get; } = RvmTheme.FromSeed("Fiscal", "#1B5E5A", "#2A6F97");

        /// <summary>Propostinha — indigo.</summary>
        public static RvmTheme Propostinha { get; } = RvmTheme.FromSeed("Propostinha", "#3F51B5", "#00897B");

        /// <summary>Todos os temas de exemplo, para o seletor do site e para o teste.</summary>
        public static IReadOnlyList<RvmTheme> All { get; } =
            [ErpAgro, ObraEmDia, Fiscal, Propostinha];
    }

    /// <summary>
    /// Todo tema registrado na biblioteca — o que o teste de contraste percorre.
    /// </summary>
    /// <remarks>
    /// Tema novo entra aqui, ou nasce sem ser medido. E a mesma logica da lista fechada de papeis
    /// em <see cref="RvmPalette.PairsToVerify"/>: o que nao esta na lista nao e verificado.
    /// </remarks>
    public static IReadOnlyList<RvmTheme> All { get; } =
        [Rvm, .. Samples.All];
}
