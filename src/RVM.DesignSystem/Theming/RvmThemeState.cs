using Microsoft.JSInterop;

namespace RVM.DesignSystem.Theming;

/// <summary>
/// O tema do usuario nesta sessao (escopo do circuito no Server, da aba no WebAssembly). O
/// <see cref="RvmThemeProvider"/> com <c>UserTheme</c> le daqui e redesenha quando muda; o
/// <see cref="RvmThemePicker"/> escreve aqui. Registre com <c>AddRvmDesignSystem()</c> (ja chama
/// <see cref="RvmThemeServiceCollectionExtensions.AddRvmTheme(Microsoft.Extensions.DependencyInjection.IServiceCollection)"/>).
/// </summary>
public sealed class RvmThemeState(IRvmThemeStore store, IJSRuntime js)
{
    /// <summary>Nome do cookie e da chave do localStorage onde a escolha fica.</summary>
    public const string CookieName = "rvm.tema";

    private Task? _carga;

    /// <summary>A escolha em vigor.</summary>
    public RvmThemeSettings Current { get; private set; } = RvmThemeSettings.Default;

    /// <summary>Ja carregou (do store ou do estado da pre-renderizacao).</summary>
    public bool IsLoaded { get; private set; }

    /// <summary>Disparado quando <see cref="Current"/> muda.</summary>
    public event Action? Changed;

    /// <summary>Carrega do store uma vez so; chamadas repetidas esperam a mesma carga.</summary>
    public Task EnsureLoadedAsync(CancellationToken cancellationToken = default) =>
        IsLoaded ? Task.CompletedTask : (_carga ??= CarregarAsync(cancellationToken));

    private async Task CarregarAsync(CancellationToken cancellationToken)
    {
        var salvo = await store.LoadAsync(cancellationToken);
        if (!IsLoaded)
        {
            Current = salvo ?? RvmThemeSettings.Default;
            IsLoaded = true;
        }
    }

    /// <summary>Usa um valor ja conhecido (o que a pre-renderizacao entregou), sem ir ao store.</summary>
    public void Restore(RvmThemeSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        Current = settings;
        IsLoaded = true;
    }

    /// <summary>
    /// Troca o tema: aplica na hora (o provider redesenha), grava cookie e localStorage (para a proxima carga nao
    /// piscar) e entrega ao store do app.
    /// </summary>
    public async Task SetAsync(RvmThemeSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        Current = settings;
        IsLoaded = true;
        Changed?.Invoke();

        try
        {
            await js.InvokeVoidAsync("rvmTheme.save", cancellationToken, CookieName, settings.Serialize());
        }
        catch (Exception e) when (e is JSException or InvalidOperationException or JSDisconnectedException or TaskCanceledException)
        {
            // Sem navegador (pre-renderizacao, teste) ou circuito caindo: o store do app ainda grava.
        }

        await store.SaveAsync(settings, cancellationToken);
    }
}
