using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace RVM.DesignSystem.Components.Dialog;

/// <summary>
/// Casca de dialogo modal: fundo escurecido, caixa com titulo opcional e conteudo livre. Esc e clique no
/// fundo fecham; o foco entra, fica preso la dentro e volta para quem abriu.
/// </summary>
/// <remarks>
/// Renderiza onde foi declarado (sem portal), como o <see cref="RvmDialog"/>: declare-o fora de containers
/// com <c>transform</c>, <c>filter</c>, <c>perspective</c> ou <c>contain</c>.
/// <para>
/// Deixe o modal sempre no render, com <see cref="Open"/> ligado ao estado, e nao dentro de um <c>@if</c>:
/// destruido ao fechar, ele nao chega a devolver o foco ao <see cref="ReturnFocusTo"/>.
/// </para>
/// </remarks>
public partial class RvmModal : ComponentBase, IAsyncDisposable
{
    private readonly string _idBase = GeradorDeIds.Novo("rvm-modal");
    private ElementReference _caixa;
    private Sobreposicao? _sobreposicao;
    private bool _focoPreso;

    // Estado proprio: segue o parametro Open so quando ELE muda. Fechar por Esc ou pelo fundo mexe aqui,
    // nunca no [Parameter] — escrever no proprio parametro e sobrescrito no proximo render do pai.
    private bool _aberto;
    private bool _ultimoOpen;

    [Inject] private IJSRuntime JS { get; set; } = default!;

    [CascadingParameter] private PapelDoModal? Papel { get; set; }

    /// <summary>Aberto. Aceita <c>@bind-Open</c>.</summary>
    [Parameter] public bool Open { get; set; }

    /// <summary>Disparado quando o modal se fecha sozinho (Esc, fundo).</summary>
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }

    /// <summary>Titulo no topo da caixa. Tambem e o nome acessivel do modal.</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>Nome acessivel quando nao ha <see cref="Title"/> (o topo e desenhado no conteudo).</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>O conteudo da caixa, inclusive os botoes.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Clicar no fundo fecha. Padrao: sim.</summary>
    [Parameter] public bool CloseOnBackdrop { get; set; } = true;

    /// <summary>Esc fecha. Padrao: sim.</summary>
    [Parameter] public bool CloseOnEscape { get; set; } = true;

    /// <summary>
    /// Quem recebe o foco quando o modal fecha — normalmente o botao que o abriu. Sem ele, o foco volta
    /// ao elemento que o tinha quando o modal abriu.
    /// </summary>
    [Parameter] public ElementReference? ReturnFocusTo { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos extras, repassados ao elemento raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string IdTitulo => $"{_idBase}-titulo";

    private PapelDoModal? MeuPapel => Papel?.Dono == this ? Papel : null;

    private string? NomeadoPor => !string.IsNullOrWhiteSpace(Title) ? IdTitulo : MeuPapel?.NomeadoPor;

    internal string ClassesDaRaiz =>
        ClassesCss.Juntar(MeuPapel?.Estreito == true ? "rvm-modal rvm-estreito" : "rvm-modal", Class, AdditionalAttributes);

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (Papel is { Dono: null })
        {
            Papel.Dono = this;
        }
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (Open != _ultimoOpen)
        {
            _ultimoOpen = Open;
            _aberto = Open;
        }
    }

    /// <summary>Fecha o modal e avisa quem estiver ligado por <c>@bind-Open</c>.</summary>
    public async Task CloseAsync()
    {
        if (!_aberto)
        {
            return;
        }

        _aberto = false;
        StateHasChanged();
        await OpenChanged.InvokeAsync(false);
    }

    private Task AoClicarNoFundoAsync() => CloseOnBackdrop ? CloseAsync() : Task.CompletedTask;

    private Task AoTeclarAsync(KeyboardEventArgs e) =>
        e.Key == "Escape" && CloseOnEscape ? CloseAsync() : Task.CompletedTask;

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_aberto && !_focoPreso)
        {
            _focoPreso = true;
            _sobreposicao ??= new Sobreposicao(JS);
            await _sobreposicao.AbrirAsync(_caixa);
        }
        else if (!_aberto && _focoPreso)
        {
            _focoPreso = false;
            if (_sobreposicao is not null)
            {
                // Devolve o foco a quem o tinha ao abrir; o ReturnFocusTo, se veio, vence logo depois.
                await _sobreposicao.FecharAsync();
            }

            await DevolverFocoAsync();
        }
    }

    private async Task DevolverFocoAsync()
    {
        if (ReturnFocusTo is not { } destino)
        {
            return;
        }

        // O caso mais comum e confirmar uma exclusao, e a acao costuma tirar do DOM o proprio botao que
        // abriu o modal. Focar elemento desconectado lanca — e, no OnAfterRender, derrubaria o circuito.
        try
        {
            await destino.FocusAsync();
        }
        catch (Exception e) when (e is JSException or JSDisconnectedException or InvalidOperationException or TaskCanceledException)
        {
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_sobreposicao is not null)
        {
            await _sobreposicao.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }
}
