using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RVM.DesignSystem.Components.Snackbar;
using RVM.DesignSystem.Components.Toast;

namespace RVM.DesignSystem;

/// <summary>Registro dos servicos da biblioteca.</summary>
public static class RvmServiceCollectionExtensions
{
    /// <summary>
    /// Registra o que a biblioteca precisa injetar: o <see cref="RvmSnackbarService"/> e o
    /// <see cref="IRvmToast"/>. Escopo por circuito (Blazor Server) ou por aba (WebAssembly) — a fila de uma pessoa
    /// nao aparece para outra.
    /// </summary>
    public static IServiceCollection AddRvmDesignSystem(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddScoped<RvmSnackbarService>();
        services.AddRvmToast();
        // O relogio do snackbar e do toast vem do DI para o teste poder avancar o tempo sem esperar de verdade.
        services.TryAddSingleton(TimeProvider.System);
        return services;
    }
}
