namespace RVM.DesignSystem.Components.Toast;

/// <summary>
/// Avisos passageiros (toast). Injete, chame <see cref="Show(string, RvmToastSeverity, string?, TimeSpan?)"/>
/// (ou os atalhos de <see cref="RvmToastExtensions"/>) e ponha um <see cref="RvmToastProvider"/> no layout.
/// </summary>
public interface IRvmToast
{
    /// <summary>Avisos na tela agora, do mais antigo ao mais novo.</summary>
    IReadOnlyList<RvmToastMessage> Messages { get; }

    /// <summary>Disparado quando a fila muda. Pode vir de outra thread (o relogio); o provider trata.</summary>
    event Action? Changed;

    /// <summary>Mostra um aviso. Sem <paramref name="duration"/>, usa o padrao da gravidade.</summary>
    /// <param name="text">O texto do aviso.</param>
    /// <param name="severity">A gravidade. Padrao: informacao.</param>
    /// <param name="title">Titulo opcional.</param>
    /// <param name="duration">Tempo na tela. <see cref="TimeSpan.Zero"/>: fica ate a pessoa fechar.</param>
    /// <returns>O identificador do aviso.</returns>
    Guid Show(string text, RvmToastSeverity severity = RvmToastSeverity.Info, string? title = null, TimeSpan? duration = null);

    /// <summary>
    /// Mostra um aviso com um botao de acao. Sem <paramref name="duration"/>, fica
    /// <see cref="RvmToastService.ActionDuration"/> (mais que o padrao: a pessoa precisa ler e agir) e, como todo
    /// aviso, nao some enquanto o ponteiro ou o foco estiverem nele.
    /// </summary>
    /// <param name="text">O texto do aviso.</param>
    /// <param name="action">O botao de acao.</param>
    /// <param name="severity">A gravidade. Padrao: informacao.</param>
    /// <param name="title">Titulo opcional.</param>
    /// <param name="duration">Tempo na tela. <see cref="TimeSpan.Zero"/>: fica ate a pessoa fechar.</param>
    /// <returns>O identificador do aviso.</returns>
    /// <remarks>Implementacao padrao so para nao quebrar quem implementa a interface: descarta a acao.</remarks>
    Guid Show(string text, RvmToastAction action, RvmToastSeverity severity = RvmToastSeverity.Info, string? title = null, TimeSpan? duration = null)
        => Show(text, severity, title, duration);

    /// <summary>Fecha um aviso.</summary>
    /// <param name="id">O identificador devolvido pelo <c>Show</c>.</param>
    void Dismiss(Guid id);

    /// <summary>Fecha todos.</summary>
    void Clear();

    /// <summary>Suspende o fechamento automatico (ponteiro ou foco em cima do aviso, WCAG 2.2.1).</summary>
    /// <param name="id">O identificador do aviso.</param>
    void Pause(Guid id);

    /// <summary>Retoma o fechamento automatico, com a duracao cheia de novo.</summary>
    /// <param name="id">O identificador do aviso.</param>
    void Resume(Guid id);
}
