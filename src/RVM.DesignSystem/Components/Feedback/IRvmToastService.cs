namespace RVM.DesignSystem.Components;

/// <summary>
/// Uma notificacao na fila.
/// </summary>
/// <param name="Id">Identificador, gerado pelo servico.</param>
/// <param name="Text">A mensagem. PT-BR explicativo, sem codigo interno.</param>
/// <param name="Severity">Severidade.</param>
/// <param name="Title">Titulo curto opcional.</param>
/// <param name="Duration">Quanto tempo fica na tela.</param>
/// <param name="ActionLabel">Rotulo da acao — tipicamente "Desfazer".</param>
/// <param name="OnAction">O que a acao faz. Fechar o toast fica por conta do host.</param>
public sealed record RvmToastMessage(
    Guid Id,
    string Text,
    RvmSeverity Severity,
    string? Title,
    TimeSpan Duration,
    string? ActionLabel,
    Func<Task>? OnAction);

/// <summary>
/// Fila de notificacoes, chamavel de qualquer lugar (<c>RF-10</c>).
/// </summary>
/// <remarks>
/// Uma das duas excecoes a regra "componente sem estado global" (`03` § Estado e servicos). A
/// outra e o <see cref="Theming.IRvmThemeService"/>.
///
/// <para>
/// Existe porque quem dispara a notificacao quase nunca e quem a exibe: o metodo que salvou o
/// pedido esta tres componentes abaixo do layout onde o toast aparece. Passar um
/// <c>EventCallback</c> por essa cadeia inteira e o codigo que este servico elimina.
/// </para>
///
/// <para>
/// <c>Scoped</c> no DI, ou seja, por circuito em Blazor Server: um toast disparado numa aba nao
/// pode aparecer na aba de outra pessoa.
/// </para>
/// </remarks>
public interface IRvmToastService
{
    /// <summary>Os toasts vivos, do mais antigo para o mais novo.</summary>
    IReadOnlyList<RvmToastMessage> Current { get; }

    /// <summary>Disparado quando a fila muda, para o host re-renderizar.</summary>
    event Action? Changed;

    /// <summary>
    /// Enfileira uma notificacao.
    /// </summary>
    /// <param name="text">A mensagem, em PT-BR explicativo.</param>
    /// <param name="severity">Severidade.</param>
    /// <param name="title">Titulo curto opcional.</param>
    /// <param name="duration">
    /// Tempo na tela. <b>Valores abaixo de 5 segundos sao elevados para 5</b> — ver
    /// <see cref="MinimoNaTela"/>.
    /// </param>
    /// <param name="actionLabel">Rotulo da acao, tipicamente "Desfazer".</param>
    /// <param name="onAction">O que a acao faz.</param>
    /// <returns>O id do toast, para dispensa-lo antes da hora.</returns>
    Guid Show(
        string text,
        RvmSeverity severity = RvmSeverity.Info,
        string? title = null,
        TimeSpan? duration = null,
        string? actionLabel = null,
        Func<Task>? onAction = null);

    /// <summary>Atalho para <see cref="RvmSeverity.Success"/>.</summary>
    /// <param name="text">A mensagem.</param>
    /// <param name="title">Titulo curto opcional.</param>
    /// <returns>O id do toast.</returns>
    Guid Success(string text, string? title = null);

    /// <summary>Atalho para <see cref="RvmSeverity.Danger"/>.</summary>
    /// <param name="text">A mensagem.</param>
    /// <param name="title">Titulo curto opcional.</param>
    /// <returns>O id do toast.</returns>
    Guid Error(string text, string? title = null);

    /// <summary>Remove um toast da fila.</summary>
    /// <param name="id">O id devolvido por <see cref="Show"/>.</param>
    void Dismiss(Guid id);

    /// <summary>Esvazia a fila. Util ao trocar de tela.</summary>
    void Clear();

    /// <summary>
    /// O minimo que um toast fica na tela.
    /// </summary>
    /// <remarks>
    /// Cinco segundos e o piso do `11` § Onda 3, e ele nao e estetico: e o tempo que alguem
    /// precisa para <b>notar</b> que apareceu algo, mover os olhos ate la e ler. Quem le devagar,
    /// quem usa ampliacao ou quem depende de leitor de tela precisa de mais — e por isso o
    /// numero e um PISO que o servico impoe, e nao um padrao que quem chama pode reduzir.
    /// </remarks>
    static TimeSpan MinimoNaTela => TimeSpan.FromSeconds(5);
}
