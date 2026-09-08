namespace RVM.DesignSystem.Components;

/// <inheritdoc cref="IRvmDialogService" />
public sealed class RvmDialogService : IRvmDialogService
{
    // Fila, e nao um so: uma segunda confirmacao disparada enquanto a primeira esta aberta nao
    // pode nem sobrescrever a primeira (quem chamou ficaria esperando para sempre) nem abrir
    // duas caixas ao mesmo tempo (o `showModal` empilharia e a de baixo ficaria inerte, com o
    // usuario respondendo a pergunta errada).
    private readonly Queue<(RvmConfirmOptions Options, TaskCompletionSource<bool> Resposta)> _fila = new();

    private (RvmConfirmOptions Options, TaskCompletionSource<bool> Resposta)? _atual;

    /// <inheritdoc />
    public RvmConfirmOptions? Current => _atual?.Options;

    /// <inheritdoc />
    public event Action? Changed;

    /// <inheritdoc />
    public Task<bool> ConfirmAsync(RvmConfirmOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        // RunContinuationsAsynchronously: sem isso a continuacao de quem chamou roda dentro do
        // SetResult, ou seja, no meio do tratamento do clique — e um `await ConfirmAsync(...)`
        // seguido de navegacao executaria antes de o dialogo terminar de fechar.
        var resposta = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        _fila.Enqueue((options, resposta));
        AvancarSePreciso();

        return resposta.Task;
    }

    /// <inheritdoc />
    public Task<bool> ConfirmAsync(string title, string message, string confirmLabel = "Confirmar") =>
        ConfirmAsync(new RvmConfirmOptions(title, message, confirmLabel));

    /// <inheritdoc />
    public void Responder(bool resultado)
    {
        if (_atual is not { } atual)
        {
            return;
        }

        _atual = null;
        atual.Resposta.TrySetResult(resultado);

        AvancarSePreciso();
        Changed?.Invoke();
    }

    private void AvancarSePreciso()
    {
        if (_atual is not null || _fila.Count == 0)
        {
            return;
        }

        _atual = _fila.Dequeue();
        Changed?.Invoke();
    }
}
