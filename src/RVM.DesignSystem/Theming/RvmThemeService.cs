using Microsoft.JSInterop;

namespace RVM.DesignSystem.Theming;

/// <inheritdoc cref="IRvmThemeService" />
public sealed class RvmThemeService : IRvmThemeService, IAsyncDisposable
{
    private const string ModulePath = "./_content/RVM.DesignSystem/js/rvm-theme.js";

    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    /// <summary>Cria o servico com o tema inicial.</summary>
    /// <param name="js">Runtime de interop, usado so para persistir a preferencia.</param>
    /// <param name="initial">Tema inicial; quando nulo, usa <see cref="RvmThemes.Rvm"/>.</param>
    public RvmThemeService(IJSRuntime js, RvmTheme? initial = null)
    {
        _js = js;
        Theme = initial ?? RvmThemes.Rvm;
    }

    /// <inheritdoc />
    public RvmTheme Theme { get; private set; }

    /// <inheritdoc />
    public RvmThemePreference Preference { get; private set; } = RvmThemePreference.System;

    /// <inheritdoc />
    public event Action? Changed;

    /// <inheritdoc />
    public Task SetThemeAsync(RvmTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        Theme = theme;
        Changed?.Invoke();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SetPreferenceAsync(RvmThemePreference preference)
    {
        Preference = preference;
        Changed?.Invoke();

        try
        {
            var module = await GetModuleAsync();
            await module.InvokeVoidAsync("setPreference", preference.ToString().ToLowerInvariant());
        }
        catch (Exception e) when (e is JSException or InvalidOperationException or TaskCanceledException or ObjectDisposedException)
        {
            // Pre-render, circuito caindo ou runtime ainda nao pronto: o JS nao esta disponivel.
            // Engolir e correto — a preferencia em memoria ja mudou e a tela ja reagiu; o que se
            // perde e so a PERSISTENCIA, que o proximo clique refaz. Deixar propagar quebraria a
            // troca de tema por um detalhe de ciclo de vida.
        }
    }

    /// <summary>
    /// Le a preferencia gravada no navegador. Chamar de <c>OnAfterRenderAsync</c>, nunca antes.
    /// </summary>
    /// <returns>A preferencia lida, ou <see cref="RvmThemePreference.System"/> se nao houver.</returns>
    public async Task<RvmThemePreference> LoadPreferenceAsync()
    {
        try
        {
            var module = await GetModuleAsync();
            var stored = await module.InvokeAsync<string?>("getPreference");

            if (Enum.TryParse<RvmThemePreference>(stored, ignoreCase: true, out var parsed))
            {
                Preference = parsed;
                Changed?.Invoke();
            }
        }
        catch (Exception e) when (e is JSException or InvalidOperationException or TaskCanceledException or ObjectDisposedException)
        {
            // Mesmo motivo do SetPreferenceAsync: sem JS, fica o padrao (System), que e o que o
            // script anti-flash ja aplicou.
        }

        return Preference;
    }

    private async ValueTask<IJSObjectReference> GetModuleAsync() =>
        _module ??= await _js.InvokeAsync<IJSObjectReference>("import", ModulePath);

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_module is null)
        {
            return;
        }

        try
        {
            await _module.DisposeAsync();
        }
        catch (Exception e) when (e is JSException or InvalidOperationException or TaskCanceledException)
        {
            // Circuito ja caiu — nao ha modulo para liberar e reclamar disso no dispose so
            // polui o log de quem consome.
        }
    }
}
