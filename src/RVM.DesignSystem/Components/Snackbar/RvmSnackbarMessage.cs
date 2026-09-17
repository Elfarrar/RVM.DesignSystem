namespace RVM.DesignSystem.Components.Snackbar;

/// <summary>Uma mensagem na fila do <see cref="RvmSnackbarService"/>. Serve para fecha-la depois.</summary>
public sealed class RvmSnackbarMessage
{
    private static int _proximoId;

    internal RvmSnackbarMessage(string text, RvmSnackbarOptions options)
    {
        Text = text;
        Options = options;
    }

    internal int Id { get; } = Interlocked.Increment(ref _proximoId);

    /// <summary>O texto da mensagem.</summary>
    public string Text { get; }

    /// <summary>As opcoes com que foi mostrada.</summary>
    public RvmSnackbarOptions Options { get; }
}
