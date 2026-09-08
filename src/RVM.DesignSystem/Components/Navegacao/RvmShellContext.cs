namespace RVM.DesignSystem.Components;

/// <summary>
/// O estado que a casca de aplicacao compartilha entre topbar, sidebar e conteudo.
/// </summary>
/// <remarks>
/// <b>Por que existe um objeto em vez de parametros.</b> O botao de menu vive na
/// <c>RvmTopbar</c> e a gaveta que ele abre vive na <c>RvmSidebar</c> — dois componentes irmaos,
/// nenhum pai do outro. Sem um estado em comum, o consumidor teria que segurar um <c>bool</c> no
/// proprio layout e ligar os dois na mao, que e exatamente o codigo duplicado nos quatro
/// <c>MainLayout</c> do ecossistema e que esta onda existe para eliminar.
///
/// <para>
/// Nao e estado global: uma instancia por <see cref="RvmAppShell"/>, cascateada para baixo. Duas
/// cascas na mesma pagina (raro, mas possivel) tem estados independentes.
/// </para>
/// </remarks>
public sealed class RvmShellContext
{
    /// <summary>
    /// O <c>id</c> do elemento <c>main</c>, alvo do "pular para o conteudo".
    /// </summary>
    /// <remarks>
    /// Fixo e nao configuravel de proposito: o link de pulo e o <c>main</c> precisam concordar, e
    /// deixar isso por conta de quem usa e criar um jeito de a acessibilidade quebrar em
    /// silencio — o link continua funcionando visualmente e simplesmente nao leva a lugar nenhum.
    /// </remarks>
    public const string ContentId = "rvm-conteudo";

    /// <summary>A gaveta esta aberta sobre o conteudo (viewport estreita).</summary>
    public bool SidebarOpen { get; private set; }

    /// <summary>A barra lateral esta reduzida a icones (viewport larga).</summary>
    public bool SidebarCollapsed { get; private set; }

    /// <summary>Disparado quando qualquer um dos dois estados muda.</summary>
    public event Action? Changed;

    /// <summary>Abre ou fecha a gaveta. E o que o botao de menu da topbar chama.</summary>
    public void ToggleSidebar()
    {
        SidebarOpen = !SidebarOpen;
        Changed?.Invoke();
    }

    /// <summary>
    /// Fecha a gaveta.
    /// </summary>
    /// <remarks>
    /// Chamado ao navegar e ao clicar fora. Sem isso, no celular a gaveta continua cobrindo a
    /// tela que o usuario acabou de escolher — ele toca no item e parece que nada aconteceu.
    /// </remarks>
    public void CloseSidebar()
    {
        if (!SidebarOpen)
        {
            return;
        }

        SidebarOpen = false;
        Changed?.Invoke();
    }

    /// <summary>Alterna entre a barra completa e a so de icones.</summary>
    public void ToggleCollapsed()
    {
        SidebarCollapsed = !SidebarCollapsed;
        Changed?.Invoke();
    }
}
