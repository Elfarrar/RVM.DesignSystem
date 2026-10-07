using Microsoft.JSInterop;

namespace RVM.DesignSystem.Theming;

/// <summary>
/// Guarda a escolha so no navegador (localStorage e cookie, gravados pelo <see cref="RvmThemeState"/>). Basta
/// sozinho em app WebAssembly: la a leitura e sincrona, antes da primeira renderizacao, e nada pisca. Em app com
/// servidor, e o padrao de quem ainda nao tem um store que leia o cookie ou o perfil na pre-renderizacao.
/// </summary>
public sealed class RvmBrowserThemeStore(IJSRuntime js) : IRvmThemeStore
{
    /// <inheritdoc />
    public ValueTask<RvmThemeSettings?> LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // WebAssembly: interop sincrono, entao a escolha entra antes do primeiro render.
            if (js is IJSInProcessRuntime local)
            {
                var texto = local.Invoke<string?>("rvmTheme.load", RvmThemeState.CookieName);
                return ValueTask.FromResult<RvmThemeSettings?>(texto is null ? null : RvmThemeSettings.Parse(texto));
            }
        }
        catch (JSException)
        {
            // Sem o rvm-theme.js na pagina ou sem localStorage (modo privado estrito): segue no padrao.
        }

        // No servidor a pre-renderizacao nao tem navegador: quem resolve e o store do app (cookie ou perfil).
        return ValueTask.FromResult<RvmThemeSettings?>(null);
    }

    /// <inheritdoc />
    public ValueTask SaveAsync(RvmThemeSettings settings, CancellationToken cancellationToken = default) =>
        ValueTask.CompletedTask; // o RvmThemeState ja grava cookie e localStorage a cada troca
}
