using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RVM.DesignSystem.Components.Dialog;

namespace RVM.DesignSystem.Components.Drawer;

/// <summary>
/// Painel que sai de um lado da tela: menu de navegacao, filtros, detalhe de um item.
/// </summary>
/// <remarks>
/// A temporaria renderiza onde foi declarada (sem portal): fora de ancestral com <c>transform</c>,
/// <c>filter</c>, <c>perspective</c> ou <c>contain</c>, senao ela nao cobre a tela.
/// </remarks>
public partial class RvmDrawer : ComponentBase, IAsyncDisposable
{
    private ElementReference _caixa;
    private Sobreposicao? _sobreposicao;
    private bool _focoPreso;

    [Inject] private IJSRuntime JS { get; set; } = default!;

    /// <summary>Aberta (so na temporaria). Aceita <c>@bind-Open</c>.</summary>
    [Parameter] public bool Open { get; set; }

    /// <summary>Disparado quando a gaveta se fecha sozinha (Esc, fundo).</summary>
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }

    /// <summary>Lado da tela. Padrao: <see cref="RvmDrawerAnchor.Left"/>.</summary>
    [Parameter] public RvmDrawerAnchor Anchor { get; set; } = RvmDrawerAnchor.Left;

    /// <summary>Temporaria (padrao) ou permanente.</summary>
    [Parameter] public RvmDrawerVariant Variant { get; set; } = RvmDrawerVariant.Temporary;

    /// <summary>
    /// Largura (esquerda e direita) ou altura (topo e base), em CSS. Padrao: 320px, a do kit.
    /// </summary>
    [Parameter] public string Size { get; set; } = "320px";

    /// <summary>Nome da gaveta para o leitor de tela ("Filtros", "Menu principal").</summary>
    [Parameter, EditorRequired] public string AriaLabel { get; set; } = string.Empty;

    /// <summary>O conteudo.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Atributos extras, repassados a gaveta.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private bool Horizontal => Anchor is RvmDrawerAnchor.Left or RvmDrawerAnchor.Right;

    internal string ClassesDaGaveta
    {
        get
        {
            var proprias = string.Join(' ',
                "rvm-gaveta",
                Variant == RvmDrawerVariant.Permanent ? "rvm-permanente" : "rvm-temporaria",
                Anchor switch
                {
                    RvmDrawerAnchor.Right => "rvm-direita",
                    RvmDrawerAnchor.Top => "rvm-topo",
                    RvmDrawerAnchor.Bottom => "rvm-base",
                    _ => "rvm-esquerda"
                });

            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }

    internal string ClassesDaRaizTemporaria => "rvm-camada";

    internal string EstiloDaGaveta => Horizontal ? $"width: {Size}" : $"height: {Size}";

    /// <summary>Fecha a gaveta temporaria e avisa quem estiver ligado por <c>@bind-Open</c>.</summary>
    public async Task FecharAsync()
    {
        if (!Open)
        {
            return;
        }

        Open = false;
        await OpenChanged.InvokeAsync(false);
        StateHasChanged();
    }

    private async Task AoTeclarAsync(KeyboardEventArgs e)
    {
        if (e.Key == "Escape")
        {
            await FecharAsync();
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var aberta = Open && Variant == RvmDrawerVariant.Temporary;
        if (aberta && !_focoPreso)
        {
            _focoPreso = true;
            _sobreposicao ??= new Sobreposicao(JS);
            await _sobreposicao.AbrirAsync(_caixa);
        }
        else if (!aberta && _focoPreso)
        {
            _focoPreso = false;
            if (_sobreposicao is not null)
            {
                await _sobreposicao.FecharAsync();
            }
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
