using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RVM.DesignSystem.Components.Dialog;

namespace RVM.DesignSystem.Components.AppShell;

/// <summary>
/// A moldura da aplicacao: menu lateral, barra do topo, conteudo e rodape. Em telas largas o menu fica
/// fixo e pode ser recolhido aos icones; em telas estreitas (ate 840 px de largura da moldura) vira uma
/// gaveta que abre por cima do conteudo.
/// </summary>
/// <remarks>
/// A largura que decide e a da PROPRIA moldura (container query), nao a da janela: ocupando a tela, da
/// no mesmo; dentro de um quadro de exemplo, ela se comporta pelo tamanho do quadro. Isso faz a moldura
/// ser o bloco de referencia de elementos <c>position: fixed</c> dentro dela (dialogo, gaveta) — como
/// ela ocupa a tela, eles continuam cobrindo a tela. Altura padrao: a da janela (<c>100dvh</c>); para outra,
/// passe <c>style="height: ..."</c>.
/// </remarks>
public partial class RvmAppShell : ComponentBase, IAsyncDisposable
{
    private static int _proximoId;
    private readonly string _idBase = $"rvm-casca-{Interlocked.Increment(ref _proximoId)}";
    private ElementReference _lateral;
    private ElementReference _navegacao;
    private Sobreposicao? _sobreposicao;
    private bool _focoPreso;
    private IJSObjectReference? _teclado;
    private bool _rolarParaAtual = true;

    [Inject] private IJSRuntime JS { get; set; } = default!;

    [Inject] private NavigationManager Navegacao { get; set; } = default!;

    /// <summary>Marca no alto do menu (logo e nome). Recolhido, aparece so o inicio dela: ponha o logo primeiro.</summary>
    [Parameter] public RenderFragment? Brand { get; set; }

    /// <summary>O menu: <see cref="RvmNavItem"/>, <see cref="RvmNavGroup"/> e <see cref="RvmNavSection"/>.</summary>
    [Parameter] public RenderFragment? Navigation { get; set; }

    /// <summary>Conteudo da barra do topo, depois do botao do menu (busca, tema, notificacoes, perfil).</summary>
    [Parameter] public RenderFragment? TopBar { get; set; }

    /// <summary>O conteudo da pagina.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Rodape abaixo do conteudo.</summary>
    [Parameter] public RenderFragment? Footer { get; set; }

    /// <summary>Menu recolhido aos icones (so em tela larga). Aceita <c>@bind-Collapsed</c>.</summary>
    [Parameter] public bool Collapsed { get; set; }

    /// <summary>Disparado quando a pessoa recolhe ou expande o menu.</summary>
    [Parameter] public EventCallback<bool> CollapsedChanged { get; set; }

    /// <summary>Gaveta do menu aberta (so em tela estreita). Aceita <c>@bind-MenuOpen</c>.</summary>
    [Parameter] public bool MenuOpen { get; set; }

    /// <summary>Disparado quando a gaveta abre ou fecha.</summary>
    [Parameter] public EventCallback<bool> MenuOpenChanged { get; set; }

    /// <summary>Nome da navegacao para o leitor de tela. Padrao: "Menu principal".</summary>
    [Parameter] public string NavigationLabel { get; set; } = "Menu principal";

    /// <summary>Atributos extras, repassados a raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string IdLateral => $"{_idBase}-menu";

    internal string IdConteudo => $"{_idBase}-conteudo";

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = "rvm-casca";
            if (Collapsed) proprias += " rvm-recolhida";
            if (MenuOpen) proprias += " rvm-menu-aberto";

            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }

    /// <inheritdoc />
    protected override void OnInitialized() => Navegacao.LocationChanged += AoNavegar;

    // Escolher um item na gaveta leva a outra pagina: a gaveta fecha sozinha, como no MUI. E o menu
    // rola ate o item da pagina nova, que pode estar abaixo da dobra (chegou por link no conteudo).
    private void AoNavegar(object? sender, LocationChangedEventArgs e)
    {
        _rolarParaAtual = true;
        _ = InvokeAsync(async () =>
        {
            if (MenuOpen)
            {
                await DefinirMenuAbertoAsync(false);
            }
            else
            {
                StateHasChanged();
            }
        });
    }

    internal async Task DefinirMenuAbertoAsync(bool aberto)
    {
        if (MenuOpen == aberto)
        {
            return;
        }

        MenuOpen = aberto;
        await MenuOpenChanged.InvokeAsync(aberto);
        StateHasChanged();
    }

    internal async Task DefinirRecolhidoAsync(bool recolhido)
    {
        if (Collapsed == recolhido)
        {
            return;
        }

        Collapsed = recolhido;
        await CollapsedChanged.InvokeAsync(recolhido);
        StateHasChanged();
    }

    private async Task AoTeclarNaLateralAsync(KeyboardEventArgs e)
    {
        if (e.Key == "Escape" && MenuOpen)
        {
            await DefinirMenuAbertoAsync(false);
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Gaveta aberta e modal: foco preso nela e pagina travada, como a RvmDrawer temporaria.
        if (MenuOpen && !_focoPreso)
        {
            _focoPreso = true;
            _sobreposicao ??= new Sobreposicao(JS);
            await _sobreposicao.AbrirAsync(_lateral);
        }
        else if (!MenuOpen && _focoPreso)
        {
            _focoPreso = false;
            if (_sobreposicao is not null)
            {
                await _sobreposicao.FecharAsync();
            }
        }

        if (_rolarParaAtual)
        {
            _rolarParaAtual = false;
            try
            {
                _teclado ??= await JS.InvokeAsync<IJSObjectReference>("import", "./_content/RVM.DesignSystem/rvm-teclado.js");
                await _teclado.InvokeVoidAsync("rolarAtualParaVer", _navegacao);
            }
            catch (Exception e) when (e is JSException or JSDisconnectedException or InvalidOperationException or TaskCanceledException)
            {
                // Sem JS (pre-renderizacao): o menu so nao rola ate o item atual.
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        Navegacao.LocationChanged -= AoNavegar;
        if (_sobreposicao is not null)
        {
            await _sobreposicao.DisposeAsync();
        }

        if (_teclado is not null)
        {
            try
            {
                await _teclado.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Circuito ja caiu no Blazor Server: nao ha o que liberar do lado do navegador.
            }
        }

        GC.SuppressFinalize(this);
    }
}
