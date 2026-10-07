namespace RVM.DesignSystem.Components.Toast;

/// <summary>Um aviso na fila do <see cref="IRvmToast"/>.</summary>
/// <param name="Id">Identificador, para fechar por codigo.</param>
/// <param name="Text">Texto do aviso: o que aconteceu e, se houver, o que fazer.</param>
/// <param name="Severity">Gravidade.</param>
/// <param name="Title">Titulo opcional, acima do texto.</param>
/// <param name="Duration">Tempo na tela. <see cref="TimeSpan.Zero"/> deixa o aviso ate a pessoa fechar.</param>
public sealed record RvmToastMessage(Guid Id, string Text, RvmToastSeverity Severity, string? Title, TimeSpan Duration)
{
    /// <summary>Botao de acao opcional dentro do aviso ("Desfazer"). <c>null</c>: aviso sem acao.</summary>
    public RvmToastAction? Action { get; init; }
}
