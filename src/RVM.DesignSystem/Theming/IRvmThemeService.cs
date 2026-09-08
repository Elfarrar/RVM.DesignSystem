namespace RVM.DesignSystem.Theming;

/// <summary>Como o modo de cor e escolhido.</summary>
public enum RvmThemePreference
{
    /// <summary>Segue o <c>prefers-color-scheme</c> do sistema.</summary>
    System,

    /// <summary>Claro, independente do sistema.</summary>
    Light,

    /// <summary>Escuro, independente do sistema.</summary>
    Dark,
}

/// <summary>
/// Tema corrente e a preferencia de modo do visitante.
/// </summary>
/// <remarks>
/// Uma das duas unicas excecoes a regra "componente sem estado global" (`03` § Estado e
/// servicos). Existe porque a preferencia de modo e do <b>usuario</b>, nao de uma tela: ela
/// atravessa a arvore inteira e sobrevive a navegacao.
/// </remarks>
public interface IRvmThemeService
{
    /// <summary>Tema corrente. Muda com <see cref="SetThemeAsync"/>.</summary>
    RvmTheme Theme { get; }

    /// <summary>Preferencia de modo corrente.</summary>
    RvmThemePreference Preference { get; }

    /// <summary>Disparado quando tema ou preferencia mudam, para o provider re-renderizar.</summary>
    event Action? Changed;

    /// <summary>Troca o tema em tempo de execucao (o seletor do site de documentacao usa isto).</summary>
    Task SetThemeAsync(RvmTheme theme);

    /// <summary>
    /// Troca a preferencia de modo e persiste no navegador.
    /// </summary>
    /// <remarks>
    /// A persistencia e a escrita do atributo acontecem no JS, porque dependem de
    /// <c>localStorage</c> e do elemento raiz. Em pre-render (Blazor Server antes do circuito,
    /// ou WASM antes do runtime) o JS ainda nao existe: a chamada e ignorada em silencio e o
    /// script anti-flash resolve no primeiro paint. Nenhum estado visual depende disto.
    /// </remarks>
    Task SetPreferenceAsync(RvmThemePreference preference);
}
