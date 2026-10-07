using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RVM.DesignSystem.Components.Button;
using RVM.DesignSystem.Components.Menu;

namespace RVM.DesignSystem.Components.Filter;

/// <summary>
/// Botao "Filtros" com o contador de filtros ativos, que abre um painel com os grupos
/// (<see cref="RvmFilterGroup"/>) e os botoes Limpar e Aplicar (contrato com o RVM.UI, DSGN-017).
/// </summary>
public partial class RvmFilter : ComponentBase
{
    private static int _proximoId;
    private readonly string _idBase = $"rvm-filtro-{Interlocked.Increment(ref _proximoId)}";
    private RvmButton? _botao;
    private ElementReference _painel;
    private bool _aberto;
    private bool _ultimoDoPai;
    private bool _focarPainel;

    /// <summary>Texto do botao. Padrao: "Filtros".</summary>
    [Parameter] public string Label { get; set; } = "Filtros";

    /// <summary>Titulo do painel. Padrao: "Filtros".</summary>
    [Parameter] public string Title { get; set; } = "Filtros";

    /// <summary>Quantos filtros estao ativos. Acima de zero, o contador aparece no botao e e anunciado no nome dele.</summary>
    [Parameter] public int Count { get; set; }

    /// <summary>Os grupos (<see cref="RvmFilterGroup"/>) e campos do painel.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Texto do botao que limpa. Padrao: "Limpar".</summary>
    [Parameter] public string ClearText { get; set; } = "Limpar";

    /// <summary>Texto do botao que aplica. Padrao: "Aplicar".</summary>
    [Parameter] public string ApplyText { get; set; } = "Aplicar";

    /// <summary>Painel aberto. O estado interno so segue este valor quando ele muda.</summary>
    [Parameter] public bool Open { get; set; }

    /// <summary>Avisa a abertura e o fechamento do painel.</summary>
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }

    /// <summary>"Aplicar": avisa e depois fecha o painel, devolvendo o foco ao botao.</summary>
    [Parameter] public EventCallback OnApply { get; set; }

    /// <summary>"Limpar": avisa e deixa o painel aberto, para a pessoa ver o que sobrou.</summary>
    [Parameter] public EventCallback OnClear { get; set; }

    /// <summary>Onde o painel abre. Padrao: abaixo, alinhado pela direita (o filtro costuma ficar na direita da barra).</summary>
    [Parameter] public RvmMenuPlacement Placement { get; set; } = RvmMenuPlacement.BottomEnd;

    /// <summary>Botao indisponivel.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Id do botao. Sem ele, um id unico e gerado.</summary>
    [Parameter] public string? Id { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos extras, repassados ao elemento raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal bool Aberto => _aberto;

    internal string IdDoBotao => string.IsNullOrWhiteSpace(Id) ? $"{_idBase}-botao" : Id;

    internal string IdDoPainel => $"{IdDoBotao}-painel";

    internal string IdDoTitulo => $"{IdDoBotao}-titulo";

    /// <summary>"Filtros, 2 ativos": o numero do contador e desenho; quem ouve a tela recebe o nome inteiro.</summary>
    internal string? RotuloDoBotao => Count switch
    {
        <= 0 => null,
        1 => $"{Label}, 1 ativo",
        _ => $"{Label}, {Count} ativos"
    };

    internal string ClassesDaRaiz => ClassesCss.Juntar("rvm-filtro", Class, AdditionalAttributes);

    internal string ClassesDoPainel => "rvm-filtro-painel" + Placement switch
    {
        RvmMenuPlacement.BottomStart => "",
        RvmMenuPlacement.BottomCenter => " rvm-centro",
        RvmMenuPlacement.TopEnd => " rvm-acima rvm-fim",
        RvmMenuPlacement.TopStart => " rvm-acima",
        _ => " rvm-fim"
    };

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (Open == _ultimoDoPai)
        {
            return;
        }

        _ultimoDoPai = Open;
        _aberto = Open;
        _focarPainel = Open;
    }

    private async Task AlternarAsync()
    {
        if (_aberto)
        {
            await FecharAsync(devolverFoco: false);
            return;
        }

        _aberto = true;
        _focarPainel = true;
        await OpenChanged.InvokeAsync(true);
    }

    internal async Task FecharAsync(bool devolverFoco)
    {
        if (!_aberto)
        {
            return;
        }

        _aberto = false;
        _focarPainel = false;
        await OpenChanged.InvokeAsync(false);
        StateHasChanged();

        if (devolverFoco && _botao is not null)
        {
            await Tentar(() => _botao.FocusAsync());
        }
    }

    private async Task AplicarAsync()
    {
        await OnApply.InvokeAsync();
        await FecharAsync(devolverFoco: true);
    }

    private Task LimparAsync() => OnClear.InvokeAsync();

    private async Task AoTeclarAsync(KeyboardEventArgs e)
    {
        if (e.Key == "Escape")
        {
            await FecharAsync(devolverFoco: true);
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_focarPainel && _aberto)
        {
            _focarPainel = false;
            await Tentar(() => _painel.FocusAsync());
        }
    }

    private static async Task Tentar(Func<ValueTask> acao)
    {
        try
        {
            await acao();
        }
        catch (Exception e) when (e is JSException or JSDisconnectedException or InvalidOperationException or TaskCanceledException)
        {
            // Sem JS (pre-renderizacao, circuito caindo): o painel abre e fecha; so o foco nao anda.
        }
    }
}
