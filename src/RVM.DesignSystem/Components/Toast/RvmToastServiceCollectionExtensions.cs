using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RVM.DesignSystem.Components.Toast;

/// <summary>Registro do <see cref="IRvmToast"/> no container.</summary>
public static class RvmToastServiceCollectionExtensions
{
    /// <summary>
    /// Registra o <see cref="IRvmToast"/> com escopo: no Blazor Server cada circuito (cada aba) tem a propria fila,
    /// e o aviso de uma pessoa nunca aparece na tela de outra. Ja vem dentro do <c>AddRvmDesignSystem()</c>; chamar
    /// os dois nao duplica o registro.
    /// </summary>
    /// <param name="services">O container.</param>
    /// <returns>O mesmo container, para encadear.</returns>
    public static IServiceCollection AddRvmToast(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddScoped<IRvmToast, RvmToastService>();
        return services;
    }
}
