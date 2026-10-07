namespace RVM.DesignSystem.Components.Toast;

/// <summary>Gravidade do aviso. Decide o icone, a duracao padrao e se o leitor de tela interrompe.</summary>
public enum RvmToastSeverity
{
    /// <summary>Informacao. Anunciado sem interromper (<c>role="status"</c>).</summary>
    Info,

    /// <summary>Deu certo. Anunciado sem interromper (<c>role="status"</c>).</summary>
    Success,

    /// <summary>Atencao. Anunciado com prioridade (<c>role="alert"</c>).</summary>
    Warning,

    /// <summary>Erro. Anunciado com prioridade (<c>role="alert"</c>).</summary>
    Error
}
