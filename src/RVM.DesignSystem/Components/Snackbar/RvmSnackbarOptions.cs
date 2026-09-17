namespace RVM.DesignSystem.Components.Snackbar;

/// <summary>Como uma mensagem do <see cref="RvmSnackbarService"/> aparece e se comporta.</summary>
public sealed class RvmSnackbarOptions
{
    /// <summary>
    /// Papel de cor. Sem valor, e o snackbar neutro (a pagina Snackbar do kit); com valor, ganha o icone
    /// do papel no fundo do papel (a pagina Toast). <see cref="RvmColor.Error"/> e anunciado com
    /// urgencia (<c>role="alert"</c>); os demais, com educacao (<c>role="status"</c>).
    /// </summary>
    public RvmColor? Color { get; init; }

    /// <summary>Texto do botao de acao ("Desfazer"). Aparece so junto com <see cref="OnAction"/>.</summary>
    public string? ActionText { get; init; }

    /// <summary>O que o botao de acao faz. A mensagem fecha depois.</summary>
    public Func<Task>? OnAction { get; init; }

    /// <summary>
    /// Quanto tempo fica na tela. Padrao: 6 segundos. <c>null</c> deixa a mensagem ate alguem fechar —
    /// use para erro que a pessoa precisa ler com calma. O tempo para enquanto o mouse ou o foco estao
    /// na mensagem.
    /// </summary>
    public TimeSpan? Duration { get; init; } = TimeSpan.FromSeconds(6);

    /// <summary>Botao de fechar (x). Padrao: sim.</summary>
    public bool ShowCloseButton { get; init; } = true;

    /// <summary>Indicador girando no lugar do icone ("Enviando...").</summary>
    public bool Loading { get; init; }
}
