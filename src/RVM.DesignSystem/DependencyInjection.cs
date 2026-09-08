using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using RVM.DesignSystem.Components;
using RVM.DesignSystem.Theming;

namespace RVM.DesignSystem;

/// <summary>Opcoes de registro da biblioteca.</summary>
public sealed class RvmDesignSystemOptions
{
    /// <summary>Tema inicial. Quando nulo, usa <see cref="RvmThemes.Rvm"/>.</summary>
    public RvmTheme? Theme { get; set; }
}

/// <summary>Registro da biblioteca no contêiner de DI.</summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra tudo que a biblioteca precisa. Uma chamada, nao uma por componente.
    /// </summary>
    /// <param name="services">O contêiner do app consumidor.</param>
    /// <param name="configure">Ajustes opcionais — hoje, o tema inicial.</param>
    /// <returns>O mesmo contêiner, para encadear.</returns>
    /// <remarks>
    /// <c>Scoped</c> e nao <c>Singleton</c>: em Blazor Server, <c>Scoped</c> equivale a "por
    /// circuito", ou seja, por aba aberta. Com <c>Singleton</c>, trocar o tema numa aba trocaria
    /// na de todo mundo — inclusive de outros usuarios do mesmo servidor.
    /// </remarks>
    public static IServiceCollection AddRvmDesignSystem(
        this IServiceCollection services,
        Action<RvmDesignSystemOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new RvmDesignSystemOptions();
        configure?.Invoke(options);

        services.AddScoped<IRvmThemeService>(sp =>
            new RvmThemeService(sp.GetRequiredService<IJSRuntime>(), options.Theme));

        // As outras duas excecoes a "componente sem estado global" (`03` § Estado e servicos).
        // Ambas Scoped pelo mesmo motivo do tema: em Blazor Server, Scoped e "por circuito".
        // Com Singleton, um toast disparado por uma pessoa apareceria na tela de outra.
        services.AddScoped<IRvmToastService, RvmToastService>();
        services.AddScoped<IRvmDialogService, RvmDialogService>();

        return services;
    }
}
