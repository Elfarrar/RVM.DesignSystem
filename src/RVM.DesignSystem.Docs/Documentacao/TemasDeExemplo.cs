using RVM.DesignSystem.Theming;

namespace RVM.DesignSystem.Docs.Documentacao;

/// <summary>
/// Temas de fora do ecossistema, para o site de documentacao demonstrar o motor de tema.
/// </summary>
/// <remarks>
/// ⚠️ <b>Vivem no SITE, nao na biblioteca.</b> O <see cref="RvmThemes.Samples"/> existe para as
/// identidades dos apps RVM; uma paleta de terceiro nao e identidade nossa e nao deve virar API
/// publica do pacote — sairia caro depois: nome publico e um compromisso, e a paleta do Bootstrap
/// pode mudar na versao seguinte deles.
/// </remarks>
public static class TemasDeExemplo
{
    /// <summary>As seis cores do Bootstrap 5, como o proprio Bootstrap as declara.</summary>
    /// <remarks>
    /// Sao os valores de <c>$primary</c>, <c>$secondary</c>, <c>$success</c>, <c>$info</c>,
    /// <c>$warning</c> e <c>$danger</c> do Bootstrap 5.3. Guardados aqui em separado do tema
    /// porque a pagina de comparacao precisa mostrar o ORIGINAL ao lado do derivado — e sem eles
    /// nao haveria como medir a diferenca.
    /// </remarks>
    public static IReadOnlyList<(string Papel, string Hex)> SementesDoBootstrap { get; } =
    [
        ("primary", "#0D6EFD"),
        ("secondary", "#6C757D"),
        ("success", "#198754"),
        ("info", "#0DCAF0"),
        ("warning", "#FFC107"),
        ("danger", "#DC3545"),
    ];

    /// <summary>
    /// O tema derivado das cores do Bootstrap 5.
    /// </summary>
    /// <remarks>
    /// ⚠️ <b>Nem toda cor sai igual a que entrou, e isso e o ponto do exemplo.</b> Do que se
    /// informa, o motor aproveita a <b>matiz e o croma</b>; a luminosidade e recalculada contra a
    /// superficie mais exigente ate atender AA.
    ///
    /// <para>
    /// Duas cores do Bootstrap precisam disso: o <c>warning</c> <c>#FFC107</c> da ~1.6:1 sobre
    /// branco e o <c>info</c> <c>#0DCAF0</c> da ~1.9:1 — contra os 4.5:1 exigidos para texto
    /// normal. Usa-las como vieram deixaria texto ilegivel; o Bootstrap contorna isso pintando
    /// texto escuro sobre elas em vez de usa-las como cor de texto, o que e uma solucao valida
    /// para o Bootstrap e nao para uma paleta de papeis como esta.
    /// </para>
    /// </remarks>
    public static RvmTheme Bootstrap { get; } = RvmTheme.FromSeed("Bootstrap", new RvmSeed
    {
        Primary = "#0D6EFD",
        Secondary = "#6C757D",
        Success = "#198754",
        Info = "#0DCAF0",
        Warning = "#FFC107",
        Danger = "#DC3545",
    });

    /// <summary>Os temas de exemplo que o seletor do site oferece, alem dos da biblioteca.</summary>
    public static IReadOnlyList<RvmTheme> Todos { get; } = [Bootstrap];
}
