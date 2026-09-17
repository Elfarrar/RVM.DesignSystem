using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace RVM.DesignSystem.Components.Tabs;

/// <summary>
/// Abas: uma lista de titulos e o painel da aba escolhida. As abas sao <see cref="RvmTab"/> filhas.
/// </summary>
public partial class RvmTabs : ComponentBase, IAsyncDisposable
{
    private static int _proximoId;
    private readonly string _idBase = $"rvm-abas-{Interlocked.Increment(ref _proximoId)}";
    private readonly List<RvmTab> _abas = [];
    private ElementReference _lista;
    private IJSObjectReference? _modulo;

    [Inject] private IJSRuntime JS { get; set; } = default!;

    /// <summary>Indice da aba ativa, a partir de 0. Aceita <c>@bind-ActiveIndex</c>.</summary>
    [Parameter] public int ActiveIndex { get; set; }

    /// <summary>Disparado quando a aba ativa muda.</summary>
    [Parameter] public EventCallback<int> ActiveIndexChanged { get; set; }

    /// <summary>Sublinhada (padrao) ou preenchida.</summary>
    [Parameter] public RvmTabsVariant Variant { get; set; } = RvmTabsVariant.Standard;

    /// <summary>Abas em linha (padrao) ou empilhadas a esquerda do painel.</summary>
    [Parameter] public RvmOrientation Orientation { get; set; } = RvmOrientation.Horizontal;

    /// <summary>As abas dividem a largura toda ("Full Width" no kit). So na horizontal.</summary>
    [Parameter] public bool FullWidth { get; set; }

    /// <summary>Nome da lista de abas para o leitor de tela ("Configuracoes da conta").</summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>As <see cref="RvmTab"/>.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Atributos extras: <c>class</c> e <c>style</c> na raiz; o resto na lista de abas.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// A aba realmente ativa: <see cref="ActiveIndex"/> trazido para dentro da lista e, se cair numa
    /// aba desabilitada, a primeira habilitada. Sem isso, nenhuma aba ficaria na ordem de tabulacao.
    /// </summary>
    internal int IndiceAtivo
    {
        get
        {
            if (_abas.Count == 0)
            {
                return -1;
            }

            var indice = Math.Clamp(ActiveIndex, 0, _abas.Count - 1);
            return _abas[indice].Disabled ? _abas.FindIndex(a => !a.Disabled) : indice;
        }
    }

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = string.Join(' ',
                "rvm-abas",
                Variant == RvmTabsVariant.Contained ? "rvm-preenchidas" : "rvm-sublinhadas",
                Orientation == RvmOrientation.Vertical ? "rvm-vertical" : "rvm-horizontal");

            if (FullWidth && Orientation == RvmOrientation.Horizontal)
            {
                proprias += " rvm-largura-total";
            }

            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }

    internal string? EstiloDoConsumidor
        => AdditionalAttributes is not null
           && AdditionalAttributes.TryGetValue("style", out var valor)
           && valor is string texto
           && !string.IsNullOrWhiteSpace(texto)
            ? texto
            : null;

    internal IReadOnlyDictionary<string, object>? AtributosDaLista
        => AdditionalAttributes?
            .Where(a => !string.Equals(a.Key, "class", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(a.Key, "style", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(a => a.Key, a => a.Value);

    internal string IdDaAba(RvmTab aba) => $"{_idBase}-aba-{_abas.IndexOf(aba)}";

    internal string IdDoPainel(RvmTab aba) => $"{_idBase}-painel-{_abas.IndexOf(aba)}";

    internal bool EstaAtiva(RvmTab aba) => _abas.IndexOf(aba) == IndiceAtivo;

    internal void Registrar(RvmTab aba)
    {
        _abas.Add(aba);
        StateHasChanged();
    }

    internal void Remover(RvmTab aba)
    {
        if (_abas.Remove(aba))
        {
            StateHasChanged();
        }
    }

    internal void AoMudarAba() => StateHasChanged();

    private async Task AtivarAsync(int indice, bool focar)
    {
        if (indice < 0 || indice >= _abas.Count || _abas[indice].Disabled)
        {
            return;
        }

        if (indice != IndiceAtivo)
        {
            ActiveIndex = indice;
            await ActiveIndexChanged.InvokeAsync(indice);
        }

        if (focar)
        {
            try
            {
                await _abas[indice].Botao.FocusAsync();
            }
            catch (Exception e) when (e is JSException or JSDisconnectedException or InvalidOperationException or TaskCanceledException)
            {
                // Sem JS disponivel (pre-renderizacao): a aba ja foi trocada, so o foco nao andou.
            }
        }
    }

    private async Task AoTeclarAsync(KeyboardEventArgs e)
    {
        var vertical = Orientation == RvmOrientation.Vertical;
        var destino = e.Key switch
        {
            "ArrowRight" when !vertical => Proxima(IndiceAtivo, +1),
            "ArrowLeft" when !vertical => Proxima(IndiceAtivo, -1),
            "ArrowDown" when vertical => Proxima(IndiceAtivo, +1),
            "ArrowUp" when vertical => Proxima(IndiceAtivo, -1),
            "Home" => Proxima(-1, +1),
            "End" => Proxima(_abas.Count, -1),
            _ => -1
        };

        if (destino >= 0)
        {
            await AtivarAsync(destino, focar: true);
        }
    }

    /// <summary>A proxima aba habilitada na direcao, dando a volta nas pontas.</summary>
    internal int Proxima(int de, int passo)
    {
        for (var i = 1; i <= _abas.Count; i++)
        {
            var candidata = ((de + passo * i) % _abas.Count + _abas.Count) % _abas.Count;
            if (!_abas[candidata].Disabled)
            {
                return candidata;
            }
        }

        return -1;
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        try
        {
            _modulo = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/RVM.DesignSystem/rvm-teclado.js");
            await _modulo.InvokeVoidAsync("prenderTeclas", _lista, new[] { "ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight", "Home", "End" });
        }
        catch (Exception e) when (e is JSException or JSDisconnectedException or InvalidOperationException or TaskCanceledException)
        {
            // Sem JS: as setas ainda trocam de aba; so a pagina rola junto.
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
