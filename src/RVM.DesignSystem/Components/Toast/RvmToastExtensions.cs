namespace RVM.DesignSystem.Components.Toast;

/// <summary>Atalhos por gravidade para o <see cref="IRvmToast"/>.</summary>
public static class RvmToastExtensions
{
    /// <summary>Aviso de sucesso.</summary>
    /// <param name="toast">O servico de avisos.</param>
    /// <param name="text">O texto do aviso.</param>
    /// <param name="title">Titulo opcional.</param>
    /// <returns>O identificador do aviso.</returns>
    public static Guid Success(this IRvmToast toast, string text, string? title = null)
        => toast.Show(text, RvmToastSeverity.Success, title);

    /// <summary>Aviso informativo.</summary>
    /// <param name="toast">O servico de avisos.</param>
    /// <param name="text">O texto do aviso.</param>
    /// <param name="title">Titulo opcional.</param>
    /// <returns>O identificador do aviso.</returns>
    public static Guid Info(this IRvmToast toast, string text, string? title = null)
        => toast.Show(text, RvmToastSeverity.Info, title);

    /// <summary>Aviso de atencao.</summary>
    /// <param name="toast">O servico de avisos.</param>
    /// <param name="text">O texto do aviso.</param>
    /// <param name="title">Titulo opcional.</param>
    /// <returns>O identificador do aviso.</returns>
    public static Guid Warning(this IRvmToast toast, string text, string? title = null)
        => toast.Show(text, RvmToastSeverity.Warning, title);

    /// <summary>Aviso de erro.</summary>
    /// <param name="toast">O servico de avisos.</param>
    /// <param name="text">O texto do aviso.</param>
    /// <param name="title">Titulo opcional.</param>
    /// <returns>O identificador do aviso.</returns>
    public static Guid Error(this IRvmToast toast, string text, string? title = null)
        => toast.Show(text, RvmToastSeverity.Error, title);
}
