using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RVM.DesignSystem.Theming;

/// <summary>Registro do tema do usuario. Contrato com o RVM.UI (DSGN-017).</summary>
public static class RvmThemeServiceCollectionExtensions
{
    /// <summary>
    /// Tema do usuario guardado so no navegador (cookie e localStorage). Basta em WebAssembly; em app com servidor,
    /// prefira <see cref="AddRvmTheme{TStore}"/> com um store que leia o perfil ou o cookie. O
    /// <c>AddRvmDesignSystem()</c> ja chama este.
    /// </summary>
    public static IServiceCollection AddRvmTheme(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddScoped<IRvmThemeStore, RvmBrowserThemeStore>();
        services.TryAddScoped<RvmThemeState>();
        return services;
    }

    /// <summary>Tema do usuario com o store do app (perfil do usuario, cookie lido no servidor...).</summary>
    public static IServiceCollection AddRvmTheme<TStore>(this IServiceCollection services)
        where TStore : class, IRvmThemeStore
    {
        ArgumentNullException.ThrowIfNull(services);
        services.RemoveAll<IRvmThemeStore>();
        services.AddScoped<IRvmThemeStore, TStore>();
        services.TryAddScoped<RvmThemeState>();
        return services;
    }
}
