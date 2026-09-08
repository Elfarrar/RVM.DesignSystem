namespace RVM.DesignSystem.Components;

/// <inheritdoc cref="IRvmToastService" />
public sealed class RvmToastService : IRvmToastService
{
    private readonly List<RvmToastMessage> _fila = [];

    /// <inheritdoc />
    public IReadOnlyList<RvmToastMessage> Current => _fila;

    /// <inheritdoc />
    public event Action? Changed;

    /// <inheritdoc />
    public Guid Show(
        string text,
        RvmSeverity severity = RvmSeverity.Info,
        string? title = null,
        TimeSpan? duration = null,
        string? actionLabel = null,
        Func<Task>? onAction = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        // O piso e imposto AQUI, e nao no host: assim vale para qualquer host, inclusive um
        // que alguem escreva depois. Um toast de 2 segundos nao e uma escolha de estilo, e um
        // aviso que a pessoa nao chega a ler.
        var tempo = duration is null || duration < IRvmToastService.MinimoNaTela
            ? IRvmToastService.MinimoNaTela
            : duration.Value;

        var mensagem = new RvmToastMessage(
            Guid.NewGuid(), text, severity, title, tempo, actionLabel, onAction);

        _fila.Add(mensagem);
        Changed?.Invoke();

        return mensagem.Id;
    }

    /// <inheritdoc />
    public Guid Success(string text, string? title = null) =>
        Show(text, RvmSeverity.Success, title);

    /// <inheritdoc />
    public Guid Error(string text, string? title = null) =>
        Show(text, RvmSeverity.Danger, title);

    /// <inheritdoc />
    public void Dismiss(Guid id)
    {
        if (_fila.RemoveAll(m => m.Id == id) > 0)
        {
            Changed?.Invoke();
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        if (_fila.Count == 0)
        {
            return;
        }

        _fila.Clear();
        Changed?.Invoke();
    }
}
