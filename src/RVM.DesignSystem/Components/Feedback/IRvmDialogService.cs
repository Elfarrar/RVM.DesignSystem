namespace RVM.DesignSystem.Components;

/// <summary>
/// O texto e a aparencia de uma confirmacao.
/// </summary>
/// <param name="Title">Titulo — vira o nome acessivel do dialogo.</param>
/// <param name="Message">A pergunta, em PT-BR explicativo.</param>
/// <param name="ConfirmLabel">
/// Rotulo do botao que confirma. <b>Diga o verbo</b>: "Excluir" e "Publicar" informam o que vai
/// acontecer; "OK" e "Sim" obrigam a reler a pergunta para saber o que se esta confirmando.
/// </param>
/// <param name="CancelLabel">Rotulo do botao que cancela.</param>
/// <param name="Severity">
/// Severidade. <see cref="RvmSeverity.Danger"/> pinta a acao de confirmar como destrutiva.
/// </param>
/// <param name="Size">Tamanho da caixa.</param>
public sealed record RvmConfirmOptions(
    string Title,
    string Message,
    string ConfirmLabel = "Confirmar",
    string CancelLabel = "Cancelar",
    RvmSeverity Severity = RvmSeverity.Info,
    RvmDialogSize Size = RvmDialogSize.Small);

/// <summary>
/// Confirmacao imperativa, chamavel de qualquer lugar.
/// </summary>
/// <remarks>
/// <b>O que este servico faz e o que ele NAO faz.</b> Ele resolve o caso que se repete em toda
/// tela — "tem certeza?" — sem obrigar cada uma a carregar um `bool` de estado, um
/// <c>RvmDialog</c> na marcacao e dois handlers.
///
/// <para>
/// Ele <b>nao</b> abre um dialogo com conteudo arbitrario. Para isso existe o
/// <see cref="RvmDialog"/> declarativo com <c>@bind-Open</c>: o resultado tipado ali e o proprio
/// campo de quem usa, com o tipo que ele quiser, sem passar por uma API generica que teria de
/// renderizar componente dinamico e devolver <c>object</c>. Um formulario dentro de um dialogo e
/// marcacao, e marcacao se escreve na tela — nao se monta por chamada de metodo.
/// </para>
///
/// <para>
/// <c>Scoped</c> no DI, como o de toast e o de tema.
/// </para>
/// </remarks>
public interface IRvmDialogService
{
    /// <summary>A confirmacao que esta na tela agora, ou <c>null</c>.</summary>
    RvmConfirmOptions? Current { get; }

    /// <summary>Disparado quando a confirmacao corrente muda, para o host re-renderizar.</summary>
    event Action? Changed;

    /// <summary>
    /// Pergunta e espera a resposta.
    /// </summary>
    /// <param name="options">Texto e aparencia.</param>
    /// <returns>
    /// <c>true</c> se confirmou; <c>false</c> se cancelou, fechou no "x" ou apertou
    /// <kbd>ESC</kbd>. <b>Toda forma de sair sem decidir devolve <c>false</c></b> — quem chama
    /// nunca fica esperando para sempre, e o caminho seguro e o padrao.
    /// </returns>
    Task<bool> ConfirmAsync(RvmConfirmOptions options);

    /// <summary>Atalho para a confirmacao mais comum.</summary>
    /// <param name="title">Titulo.</param>
    /// <param name="message">A pergunta.</param>
    /// <param name="confirmLabel">Rotulo do botao que confirma. Diga o verbo.</param>
    /// <returns>O mesmo contrato do <see cref="ConfirmAsync(RvmConfirmOptions)"/>.</returns>
    Task<bool> ConfirmAsync(string title, string message, string confirmLabel = "Confirmar");

    /// <summary>
    /// Fecha a confirmacao corrente respondendo <paramref name="resultado"/>.
    /// </summary>
    /// <param name="resultado">O que devolver a quem chamou.</param>
    /// <remarks>Chamado pelo <see cref="RvmDialogHost"/>; raramente por quem consome.</remarks>
    void Responder(bool resultado);
}
