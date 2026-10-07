namespace RVM.DesignSystem.Theming;

/// <summary>
/// Onde a escolha do usuario fica guardada. A biblioteca nao conhece o banco do app: quem sabe quem e o usuario
/// e onde salvar o perfil dele e o app. Implemente e registre com
/// <see cref="RvmThemeServiceCollectionExtensions.AddRvmTheme{TStore}"/>. Contrato com o RVM.UI (DSGN-017).
/// <para>
/// ⚠️ Para a pagina nao piscar no tema padrao ao carregar, o <see cref="LoadAsync"/> precisa responder JA NA
/// PRE-RENDERIZACAO (no servidor): leia do perfil do usuario ou do cookie <see cref="RvmThemeState.CookieName"/>,
/// que a biblioteca grava a cada troca.
/// </para>
/// </summary>
public interface IRvmThemeStore
{
    /// <summary>A escolha salva, ou <c>null</c> se nao houver (vale o padrao).</summary>
    ValueTask<RvmThemeSettings?> LoadAsync(CancellationToken cancellationToken = default);

    /// <summary>Guarda a escolha (no perfil do usuario, por exemplo).</summary>
    ValueTask SaveAsync(RvmThemeSettings settings, CancellationToken cancellationToken = default);
}
