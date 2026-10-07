using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RVM.DesignSystem.Components;

namespace RVM.DesignSystem.Theming;

/// <summary>
/// Envolve a arvore que usa o design system e define qual tema vale nela. Componente filho nenhum
/// precisa saber do tema: os tokens chegam por CSS a partir dos atributos deste elemento.
/// <para>
/// Tres jeitos de decidir o tema, do mais simples ao mais completo: <see cref="Theme"/> (claro ou escuro, com
/// <see cref="Persist"/> no navegador), <see cref="Settings"/> (um tema completo fixo) e <see cref="UserTheme"/> (o
/// que o usuario escolheu no <see cref="RvmThemePicker"/>, lido do <see cref="RvmThemeState"/>).
/// </para>
/// </summary>
public partial class RvmThemeProvider : ComponentBase, IDisposable
{
    internal const string StorageKey = "rvm-theme";
    private const string ChaveDoEstado = "rvm.tema";

    [Inject] private IJSRuntime JS { get; set; } = default!;

    [Inject] private IServiceProvider Servicos { get; set; } = default!;

    /// <summary>Provider de fora, quando este esta aninhado. So o de fora pinta o &lt;html&gt; com o tema do usuario.</summary>
    [CascadingParameter] private RvmThemeProvider? Pai { get; set; }

    /// <summary>
    /// Tema em vigor. Aceita ligacao de duas vias (<c>@bind-Theme</c>). Vale quando nao ha <see cref="Settings"/>
    /// nem <see cref="UserTheme"/>: com eles, manda o <see cref="RvmThemeSettings.Mode"/>.
    /// </summary>
    [Parameter] public RvmTheme Theme { get; set; } = RvmTheme.Light;

    /// <summary>
    /// Disparado quando o tema muda, inclusive pela preferencia restaurada do navegador. Com <see cref="UserTheme"/>,
    /// avisa quando o usuario passa para claro ou escuro (acompanhar o sistema nao avisa: so o navegador sabe qual e).
    /// </summary>
    [Parameter] public EventCallback<RvmTheme> ThemeChanged { get; set; }

    /// <summary>
    /// Guarda a escolha no navegador de quem esta vendo e a restaura na proxima visita. Desligue
    /// quando o tema vier de outro lugar (preferencia do usuario no banco, por exemplo). Ignorado com
    /// <see cref="Settings"/> ou <see cref="UserTheme"/>, que ja tem o proprio estado.
    /// </summary>
    [Parameter] public bool Persist { get; set; } = true;

    /// <summary>
    /// Cor de destaque (DSGN-017). Padrao <see cref="RvmAccent.Blue"/>, o cobalto do kit: sem informar, nada muda.
    /// Ignorada quando <see cref="Settings"/> ou <see cref="UserTheme"/> decidem a paleta.
    /// </summary>
    [Parameter] public RvmAccent Accent { get; set; } = RvmAccent.Blue;

    /// <summary>
    /// Esquema de cor (DSGN-017). <see cref="RvmColorScheme.Accessible"/> (padrao) usa os degraus que passam 4.5:1;
    /// <see cref="RvmColorScheme.Original"/> usa os degraus exatos do kit, que reprovam AA como texto. Vale para a
    /// subarvore; um provider aninhado pode trocar.
    /// </summary>
    [Parameter] public RvmColorScheme ColorScheme { get; set; } = RvmColorScheme.Accessible;

    /// <summary>
    /// Tema completo fixo, sem estado (previa, teste, tema vindo do perfil pelo proprio app). Tem precedencia
    /// sobre <see cref="UserTheme"/>, <see cref="Theme"/> e <see cref="Accent"/>.
    /// </summary>
    [Parameter] public RvmThemeSettings? Settings { get; set; }

    /// <summary>
    /// Aplica o tema escolhido pelo usuario, lido do <see cref="RvmThemeState"/> (registrado pelo
    /// <c>AddRvmDesignSystem()</c>), e redesenha quando ele troca no <see cref="RvmThemePicker"/>. Use no provider
    /// da raiz do app.
    /// </summary>
    [Parameter] public bool UserTheme { get; set; }

    /// <summary>Classe CSS extra no elemento raiz (contrato com o RVM.UI, DSGN-017).</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Conteudo tematizado.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc cref="ComponentBase" />
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private string? _ultimoSincronizado;
    private RvmThemeState? _estado;
    private PersistentComponentState? _persistencia;
    private PersistingComponentStateSubscription? _inscricao;
    private RvmThemeSettings? _tema;
    private RvmTheme? _ultimoAvisado;

    /// <summary>Valor do <c>data-theme</c>: <c>light</c>, <c>dark</c> ou, com o tema do usuario, <c>system</c>.</summary>
    internal string ThemeAttribute => _tema?.ModeAttribute ?? (Theme == RvmTheme.Dark ? "dark" : "light");

    /// <summary>Valor do <c>data-rvm-accent</c>.</summary>
    internal string AccentAttribute => _tema?.PaletteAttribute ?? Accent switch
    {
        RvmAccent.Purple => "purple",
        RvmAccent.Black => "black",
        _ => "blue"
    };

    internal string CssClass => ClassesCss.Juntar("rvm-root", Class, AdditionalAttributes);

    /// <summary>Troca entre claro e escuro. E o que um botao de tema chama.</summary>
    public async Task ToggleAsync()
    {
        // No automatico, alterna a partir do que esta NA TELA: com o sistema escuro, o primeiro clique vai ao claro.
        var escuro = ThemeAttribute switch
        {
            "dark" => true,
            "system" => await SistemaEscuroAsync(),
            _ => false
        };
        await SetThemeAsync(escuro ? RvmTheme.Light : RvmTheme.Dark);
    }

    private async Task<bool> SistemaEscuroAsync()
    {
        try
        {
            return await JS.InvokeAsync<bool>("rvmTheme.prefersDark");
        }
        catch (Exception e) when (e is JSException or JSDisconnectedException or InvalidOperationException or TaskCanceledException)
        {
            return false;
        }
    }

    /// <summary>
    /// Valor do <c>data-rvm-font-scale</c>. So o provider da RAIZ emite: a escala muda o tamanho base do documento
    /// (o &lt;html&gt;), e uma previa aninhada com fonte grande nao pode redimensionar a pagina inteira.
    /// </summary>
    internal string? FontScaleAttribute => Pai is null ? _tema?.FontScaleAttribute : null;

    /// <summary>
    /// Define o tema. Ignora a chamada quando o tema pedido ja e o atual. Com <see cref="UserTheme"/>, grava o modo
    /// na escolha do usuario.
    /// </summary>
    public async Task SetThemeAsync(RvmTheme theme)
    {
        if (_estado is not null && Settings is null)
        {
            var modo = theme == RvmTheme.Dark ? RvmThemeMode.Dark : RvmThemeMode.Light;
            if (_estado.Current.Mode != modo)
            {
                await _estado.SetAsync(_estado.Current with { Mode = modo });
            }

            return;
        }

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
    protected override async Task OnInitializedAsync()
    {
        if (!UserTheme)
        {
            return;
        }

        _estado = Servicos.GetService(typeof(RvmThemeState)) as RvmThemeState
            ?? throw new InvalidOperationException(
                "UserTheme precisa do RvmThemeState: chame services.AddRvmDesignSystem() (ou AddRvmTheme()) no Program.cs.");
        _persistencia = Servicos.GetService(typeof(PersistentComponentState)) as PersistentComponentState;

        // Na passagem da pre-renderizacao para o interativo, o valor pode vir pronto. Mas o store vem PRIMEIRO: no
        // WebAssembly ele le o localStorage de forma sincrona, e a pre-renderizacao no servidor (sem navegador) so
        // sabia o padrao — se o valor dela vencesse, a escolha do usuario nunca voltaria.
        string? daPreRenderizacao = null;
        if (!_estado.IsLoaded && _persistencia is not null)
        {
            _persistencia.TryTakeFromJson(ChaveDoEstado, out daPreRenderizacao);
        }

        await _estado.EnsureLoadedAsync();
        if (!_estado.IsKnown && daPreRenderizacao is not null)
        {
            _estado.Restore(RvmThemeSettings.Parse(daPreRenderizacao));
        }

        // Um registro por estado (dois providers UserTheme gravariam a mesma chave e o Blazor lanca), e so com valor
        // CONHECIDO: o padrao "por falta" nao viaja ao cliente.
        if (_persistencia is not null && !_estado.PersistenceRegistered)
        {
            _estado.PersistenceRegistered = true;
            var estado = _estado;
            _inscricao = _persistencia.RegisterOnPersisting(() =>
            {
                if (estado.IsKnown)
                {
                    _persistencia.PersistAsJson(ChaveDoEstado, estado.Current.Serialize());
                }

                return Task.CompletedTask;
            });
        }
        _estado.Changed += AoMudar;
        _ultimoAvisado = Theme;
        await AvisarModoAsync(_estado.Current.Mode);
    }

    /// <inheritdoc />
    protected override void OnParametersSet() => _tema = Settings ?? _estado?.Current;

    /// <summary>
    /// Avisa os descendentes (o <see cref="RvmThemePicker"/>) que o <c>data-theme</c> mudou. Necessario porque o
    /// provider se passa em cascata fixa: quem le o modo dele nao e redesenhado sozinho.
    /// </summary>
    internal event Action? ModoMudou;

    private string? _modoAvisado;

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_modoAvisado != ThemeAttribute)
        {
            var primeiro = _modoAvisado is null;
            _modoAvisado = ThemeAttribute;
            if (!primeiro)
            {
                ModoMudou?.Invoke();
            }
        }

        if (_tema is not null)
        {
            // Blazor Server sem store que leia o cookie: o store do navegador nao alcanca o localStorage na
            // pre-renderizacao. Le agora pelo JS; pisca uma vez no padrao, mas a escolha volta (e o AoMudar redesenha).
            if (firstRender && _estado is not null && Settings is null)
            {
                await _estado.LoadFromBrowserAsync();
            }

            // Tema completo: so o provider de fora pinta o <html> (area fora do provider, barra de rolagem). Sem
            // persistir na chave do Theme: a escolha inteira ja foi gravada pelo RvmThemeState.
            if (Pai is null && _ultimoSincronizado != ThemeAttribute)
            {
                _ultimoSincronizado = ThemeAttribute;
                await SincronizarComODocumentoAsync(false);
            }

            return;
        }

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
        if (_ultimoSincronizado != ThemeAttribute)
        {
            _ultimoSincronizado = ThemeAttribute;
            await SincronizarComODocumentoAsync(Persist);
        }
    }

    private void AoMudar() => _ = InvokeAsync(async () =>
    {
        _tema = Settings ?? _estado?.Current;
        StateHasChanged();
        if (_estado is not null)
        {
            await AvisarModoAsync(_estado.Current.Mode);
        }
    });

    private async Task AvisarModoAsync(RvmThemeMode modo)
    {
        RvmTheme? tema = modo switch
        {
            RvmThemeMode.Dark => RvmTheme.Dark,
            RvmThemeMode.Light => RvmTheme.Light,
            _ => null
        };
        if (tema is null || tema == _ultimoAvisado)
        {
            return;
        }

        _ultimoAvisado = tema;
        await ThemeChanged.InvokeAsync(tema.Value);
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
        catch (Exception e) when (e is JSException or JSDisconnectedException or InvalidOperationException or TaskCanceledException)
        {
            // Sem JS disponivel (pre-renderizacao, circuito caindo) o componente segue correto:
            // o tema ja saiu no data-theme deste elemento. Persistir e melhoria, nao requisito.
            return null;
        }
    }

    private async Task SincronizarComODocumentoAsync(bool persistir)
    {
        try
        {
            await JS.InvokeVoidAsync("rvmTheme.apply", ThemeAttribute, persistir);
        }
        catch (Exception e) when (e is JSException or JSDisconnectedException or InvalidOperationException or TaskCanceledException)
        {
            // Idem: pinta o <html> quando der: e o que evita a faixa clara fora do provider.
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_estado is not null)
        {
            _estado.Changed -= AoMudar;
        }

        _inscricao?.Dispose();
        GC.SuppressFinalize(this);
    }
}
