using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RVM.DesignSystem.Components.Button;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Menu;

/// <summary>
/// Botao que abre uma lista de acoes. Os itens sao <see cref="RvmMenuItem"/> e
/// <see cref="RvmMenuDivider"/> filhos.
/// </summary>
public partial class RvmMenu : ComponentBase, IAsyncDisposable
{
    private static int _proximoId;
    private readonly string _idBase = $"rvm-menu-{Interlocked.Increment(ref _proximoId)}";
    private readonly List<RvmMenuItem> _itens = [];
    private RvmButton? _botao;
    private ElementReference _lista;
    private IJSObjectReference? _modulo;
    private bool _aberto;

    /// <summary>Qual item focar depois que o menu aparecer: primeiro, ultimo ou nenhum.</summary>
    private int? _focarAoAbrir;

    [Inject] private IJSRuntime JS { get; set; } = default!;

    /// <summary>O texto do botao.</summary>
    [Parameter, EditorRequired] public string Label { get; set; } = string.Empty;

    /// <summary>Icone antes do texto do botao.</summary>
    [Parameter] public RvmIconName? Icon { get; set; }

    /// <summary>Estilo do botao. Padrao: <see cref="RvmButtonVariant.Outlined"/>.</summary>
    [Parameter] public RvmButtonVariant ButtonVariant { get; set; } = RvmButtonVariant.Outlined;

    /// <summary>Cor do botao. Padrao: <see cref="RvmColor.Primary"/>.</summary>
    [Parameter] public RvmColor ButtonColor { get; set; } = RvmColor.Primary;

    /// <summary>Tamanho do botao.</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>Alinha o menu pela direita do botao — para botoes encostados na borda direita.</summary>
    [Parameter] public bool AlignEnd { get; set; }

    /// <summary>Botao indisponivel.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Os itens.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Atributos extras: <c>class</c> e <c>style</c> na raiz; o resto no botao.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal bool Aberto => _aberto;

    internal string IdDoBotao => $"{_idBase}-botao";

    internal string IdDoMenu => $"{_idBase}-lista";

    internal string ClassesDaRaiz
        => AdditionalAttributes is not null
           && AdditionalAttributes.TryGetValue("class", out var informada)
           && informada is string texto
           && !string.IsNullOrWhiteSpace(texto)
            ? $"rvm-menu {texto}"
            : "rvm-menu";

    internal string? EstiloDoConsumidor
        => AdditionalAttributes is not null
           && AdditionalAttributes.TryGetValue("style", out var valor)
           && valor is string texto
           && !string.IsNullOrWhiteSpace(texto)
            ? texto
            : null;

    internal IReadOnlyDictionary<string, object>? AtributosDoBotao
        => AdditionalAttributes?
            .Where(a => !string.Equals(a.Key, "class", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(a.Key, "style", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(a => a.Key, a => a.Value);

    internal void Registrar(RvmMenuItem item) => _itens.Add(item);

    internal void Remover(RvmMenuItem item) => _itens.Remove(item);

    private Task AoClicarNoBotaoAsync()
        => _aberto ? FecharAsync(devolverFoco: false) : AbrirAsync(focar: 0);

    private async Task AoTeclarNoBotaoAsync(KeyboardEventArgs e)
    {
        if (Disabled)
        {
            return;
        }

        switch (e.Key)
        {
            case "ArrowDown":
                await AbrirAsync(focar: 0);
                break;
            case "ArrowUp":
                await AbrirAsync(focar: -1);
                break;
        }
    }

    private async Task AoTeclarNoMenuAsync(KeyboardEventArgs e)
    {
        var atual = _itens.FindIndex(i => i.TemFoco);
        switch (e.Key)
        {
            case "ArrowDown":
                await FocarAsync(Proximo(atual, +1));
                break;
            case "ArrowUp":
                await FocarAsync(Proximo(atual < 0 ? 0 : atual, -1));
                break;
            case "Home":
                await FocarAsync(Proximo(-1, +1));
                break;
            case "End":
                await FocarAsync(Proximo(_itens.Count, -1));
                break;
            case "Escape":
                await FecharAsync(devolverFoco: true);
                break;
            case "Tab":
                // O navegador ja leva o foco adiante; so o menu precisa sumir.
                await FecharAsync(devolverFoco: false);
                break;
        }
    }

    /// <summary>O proximo item habilitado na direcao, dando a volta nas pontas.</summary>
    internal int Proximo(int de, int passo)
    {
        for (var i = 1; i <= _itens.Count; i++)
        {
            var candidato = ((de + passo * i) % _itens.Count + _itens.Count) % _itens.Count;
            if (!_itens[candidato].Disabled)
            {
                return candidato;
            }
        }

        return -1;
    }

    private Task AbrirAsync(int focar)
    {
        _aberto = true;
        _focarAoAbrir = focar;
        StateHasChanged();
        return Task.CompletedTask;
    }

    internal async Task FecharAsync(bool devolverFoco)
    {
        _aberto = false;
        _focarAoAbrir = null;
        StateHasChanged();

        if (devolverFoco && _botao is not null)
        {
            await Tentar(() => _botao.FocusAsync());
        }
    }

    private async Task FocarAsync(int indice)
    {
        if (indice >= 0 && indice < _itens.Count)
        {
            await Tentar(() => _itens[indice].FocusAsync());
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!_aberto || _focarAoAbrir is not { } focar)
        {
            return;
        }

        _focarAoAbrir = null;
        await Tentar(async () =>
        {
            _modulo ??= await JS.InvokeAsync<IJSObjectReference>("import", "./_content/RVM.DesignSystem/rvm-teclado.js");
            await _modulo.InvokeVoidAsync("prenderTeclas", _lista, new[] { "ArrowUp", "ArrowDown", "Home", "End" });
        });
        await FocarAsync(focar < 0 ? Proximo(_itens.Count, -1) : Proximo(-1, +1));
    }

    private static async Task Tentar(Func<ValueTask> acao)
    {
        try
        {
            await acao();
        }
        catch (Exception e) when (e is JSException or JSDisconnectedException or InvalidOperationException or TaskCanceledException)
        {
            // Sem JS (pre-renderizacao, circuito caindo): o menu abre e fecha; so o foco nao anda.
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_modulo is not null)
        {
            try
            {
                await _modulo.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Circuito ja caiu no Blazor Server: nao ha o que liberar do lado do navegador.
            }
        }

        GC.SuppressFinalize(this);
    }
}
