using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace RVM.DesignSystem.Theming;

/// <summary>
/// Envolve a arvore que usa o design system e define qual tema vale nela. Componente filho nenhum
/// precisa saber do tema: os tokens chegam por CSS a partir do <c>data-theme</c> deste elemento.
/// </summary>
public partial class RvmThemeProvider : ComponentBase
{
    internal const string StorageKey = "rvm-theme";

    [Inject] private IJSRuntime JS { get; set; } = default!;

    /// <summary>Tema em vigor. Aceita ligacao de duas vias (<c>@bind-Theme</c>).</summary>
    [Parameter] public RvmTheme Theme { get; set; } = RvmTheme.Light;

    /// <summary>Disparado quando o tema muda, inclusive pela preferencia restaurada do navegador.</summary>
    [Parameter] public EventCallback<RvmTheme> ThemeChanged { get; set; }

    /// <summary>
    /// Guarda a escolha no navegador de quem esta vendo e a restaura na proxima visita. Desligue
    /// quando o tema vier de outro lugar (preferencia do usuario no banco, por exemplo).
    /// </summary>
    [Parameter] public bool Persist { get; set; } = true;

    /// <summary>Conteudo tematizado.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc cref="ComponentBase" />
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private RvmTheme? _ultimoSincronizado;

    internal string ThemeAttribute => Theme == RvmTheme.Dark ? "dark" : "light";

    internal string CssClass =>
        AdditionalAttributes is not null
        && AdditionalAttributes.TryGetValue("class", out var informada)
        && informada is string texto
        && !string.IsNullOrWhiteSpace(texto)
            ? $"rvm-root {texto}"
            : "rvm-root";

    /// <summary>Troca entre claro e escuro. E o que um botao de tema chama.</summary>
    public Task ToggleAsync() => SetThemeAsync(Theme == RvmTheme.Dark ? RvmTheme.Light : RvmTheme.Dark);

    /// <summary>Define o tema. Ignora a chamada quando o tema pedido ja e o atual.</summary>
    public async Task SetThemeAsync(RvmTheme theme)
    {
        if (theme == Theme)
        {
            return;
        }

        Theme = theme;
        // InvokeAsync e nao StateHasChanged direto: este metodo e publico, entao pode ser chamado
        // de fora do dispatcher do renderizador (um timer, um evento de outro servico) — e ai o
        // StateHasChanged cru joga "The current thread is not associated with the Dispatcher".
        await InvokeAsync(StateHasChanged);
        await ThemeChanged.InvokeAsync(theme);
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Persist)
        {
            var guardado = await LerPreferenciaAsync();
            if (guardado is not null && guardado != Theme)
            {
                await SetThemeAsync(guardado.Value);
                return;
            }
        }

        // Todo render, e nao so o primeiro: o tema tambem muda por fora, quando o consumidor usa
        // `@bind-Theme`. Sincronizar so dentro do SetThemeAsync deixaria o <html> para tras.
        if (_ultimoSincronizado != Theme)
        {
            _ultimoSincronizado = Theme;
            await SincronizarComODocumentoAsync();
        }
    }

    private async Task<RvmTheme?> LerPreferenciaAsync()
    {
        try
        {
            var valor = await JS.InvokeAsync<string?>("rvmTheme.read");
            return valor switch
            {
                "dark" => RvmTheme.Dark,
                "light" => RvmTheme.Light,
                _ => null
            };
        }
        catch (Exception e) when (e is JSException or InvalidOperationException or TaskCanceledException)
        {
            // Sem JS disponivel (pre-renderizacao, circuito caindo) o componente segue correto:
            // o tema ja saiu no data-theme deste elemento. Persistir e melhoria, nao requisito.
            return null;
        }
    }

    private async Task SincronizarComODocumentoAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("rvmTheme.apply", ThemeAttribute, Persist);
        }
        catch (Exception e) when (e is JSException or InvalidOperationException or TaskCanceledException)
        {
            // Idem: pinta o <html> quando der: e o que evita a faixa clara fora do provider.
        }
    }
}
