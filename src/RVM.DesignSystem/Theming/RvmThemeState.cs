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

    /// <summary>
    /// A escolha em vigor veio de algum lugar (store, pre-renderizacao, navegador ou troca do usuario), e nao e so
    /// o padrao por falta de informacao. So um valor conhecido pode ser passado da pre-renderizacao ao cliente:
    /// passar o padrao "por falta" apagaria a escolha que o cliente ainda ia ler do localStorage.
    /// </summary>
    internal bool IsKnown { get; private set; }

    /// <summary>Um provider ja registrou a passagem do valor da pre-renderizacao ao interativo (so pode um).</summary>
    internal bool PersistenceRegistered { get; set; }

    private bool _navegadorConsultado;

    private async Task CarregarAsync(CancellationToken cancellationToken)
    {
        var salvo = await store.LoadAsync(cancellationToken);
        if (!IsLoaded)
        {
            Current = salvo ?? RvmThemeSettings.Default;
            IsKnown = salvo is not null;
            IsLoaded = true;
        }
    }

    /// <summary>Usa um valor ja conhecido (o que a pre-renderizacao entregou), sem ir ao store.</summary>
    public void Restore(RvmThemeSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        Current = settings;
        IsLoaded = true;
        IsKnown = true;
    }

    /// <summary>
    /// Ultimo recurso para app com servidor sem store que leia o cookie: depois do primeiro render, le o
    /// localStorage pelo JS e aplica (avisando quem desenha). Pisca uma vez no padrao, mas a escolha volta. Uma
    /// consulta por sessao; nao faz nada quando o valor ja e conhecido.
    /// </summary>
    internal async Task LoadFromBrowserAsync()
    {
        if (IsKnown || _navegadorConsultado)
        {
            return;
        }

        _navegadorConsultado = true;
        string? texto;
        try
        {
            texto = await js.InvokeAsync<string?>("rvmTheme.load", CookieName);
        }
        catch (Exception e) when (e is JSException or InvalidOperationException or JSDisconnectedException or TaskCanceledException)
        {
            return; // sem JS (pre-renderizacao, rvm-theme.js ausente): fica no padrao
        }

        if (texto is not null && !IsKnown)
        {
            Restore(RvmThemeSettings.Parse(texto));
            Changed?.Invoke();
        }
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
        IsKnown = true;
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
