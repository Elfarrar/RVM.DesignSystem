using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RVM.DesignSystem.Components.Tooltip;

namespace RVM.DesignSystem.Components.EventCalendar;

/// <summary>
/// Calendario mensal compacto de agenda lateral (contrato com o RVM.UI, DSGN-017): mostra o mes, marca cada dia com o
/// que o aplicativo informa em <see cref="DayInfo"/> e deixa escolher (ou filtrar por) um dia.
/// </summary>
public partial class RvmMiniCalendar : ComponentBase, IAsyncDisposable
{
    private static readonly string[] TeclasDaGrade =
        ["ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight", "Home", "End", "PageUp", "PageDown"];

    private static int _proximoId;
    private readonly string _idBase = $"rvm-mini-calendario-{Interlocked.Increment(ref _proximoId)}";
    private readonly ElementReference[] _dias = new ElementReference[42];
    private ElementReference _grade;
    private IJSObjectReference? _modulo;
    private DateOnly _mes;
    private DateOnly? _mesVisto;
    private DateOnly _foco;
    private bool _iniciado;
    private bool _focarDia;
    private DateOnly? _mesPreso;

    [Inject] private IJSRuntime JS { get; set; } = default!;

    /// <summary>Mes mostrado (qualquer dia dele). Sem ele, o mes do dia escolhido ou o de hoje.</summary>
    [Parameter] public DateOnly Month { get; set; }

    /// <summary>Troca de mes pelo usuario (setas, teclado). Entrega o primeiro dia do mes novo.</summary>
    [Parameter] public EventCallback<DateOnly> MonthChanged { get; set; }

    /// <summary>Dia escolhido. Aceita <c>@bind-Value</c>.</summary>
    [Parameter] public DateOnly? Value { get; set; }

    /// <summary>Aviso do dia escolhido (<c>null</c> quando <see cref="AllowDeselect"/> desfaz a escolha).</summary>
    [Parameter] public EventCallback<DateOnly?> ValueChanged { get; set; }

    /// <summary>Clicar de novo no dia escolhido desfaz a escolha (filtro liga/desliga).</summary>
    [Parameter] public bool AllowDeselect { get; set; }

    /// <summary>Hoje. Sem ele, a data local do aparelho — passe o hoje do fuso do negocio quando o servidor roda em UTC.</summary>
    [Parameter] public DateOnly? Today { get; set; }

    /// <summary>Marcacoes de cada dia. Chamado para os 42 dias da grade a cada render: deve ser barato (dicionario, nao consulta).</summary>
    [Parameter] public Func<DateOnly, RvmMiniCalendarDay?>? DayInfo { get; set; }

    /// <summary>Lado do balao do dia. Padrao: <see cref="RvmPlacement.Top"/>.</summary>
    [Parameter] public RvmPlacement TooltipPlacement { get; set; } = RvmPlacement.Top;

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos repassados ao elemento raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private DateOnly Hoje => Today ?? DateOnly.FromDateTime(DateTime.Today);

    private string IdTitulo => $"{_idBase}-titulo";

    private string Titulo => DatasPorExtenso.TituloDoMes(_mes);

    private string ClassesDaRaiz => ClassesCss.Juntar("rvm-mini-calendario", Class, AdditionalAttributes);

    /// <summary>Primeiro domingo da grade (pode ser do mes anterior).</summary>
    private DateOnly Inicio => _mes.AddDays(-(int)_mes.DayOfWeek);

    private RvmTooltipPlacement LadoDoBalao => TooltipPlacement switch
    {
        RvmPlacement.Bottom => RvmTooltipPlacement.Bottom,
        RvmPlacement.Left => RvmTooltipPlacement.Left,
        RvmPlacement.Right => RvmTooltipPlacement.Right,
        _ => RvmTooltipPlacement.Top
    };

    private static DateOnly Primeiro(DateOnly dia) => new(dia.Year, dia.Month, 1);

    private static int Pontos(RvmMiniCalendarDay? info) => Math.Clamp(info?.Count ?? 0, 0, 3);

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        DateOnly? mes = Month == default ? null : Primeiro(Month);
        if (!_iniciado)
        {
            _iniciado = true;
            _mesVisto = mes;
            _mes = mes ?? Primeiro(Value ?? Hoje);
            _foco = FocoInicial();
            return;
        }

        // So segue o parametro quando ele MUDA: um pai que re-renderiza com o mesmo Month (sem @bind) nao desfaz o
        // mes que o usuario escolheu pelas setas.
        if (mes != _mesVisto)
        {
            _mesVisto = mes;
            if (mes is { } novo && novo != _mes)
            {
                _mes = novo;
                _foco = FocoInicial();
            }
        }
    }

    // O dia que entra no Tab: o escolhido, se estiver neste mes; senao hoje; senao o dia 1.
    private DateOnly FocoInicial()
    {
        if (Value is { } v && Primeiro(v) == _mes)
        {
            return v;
        }

        return Primeiro(Hoje) == _mes ? Hoje : _mes;
    }

    private string ClassesDoDia(DateOnly dia)
    {
        var classes = "rvm-mini-dia";
        if (Primeiro(dia) != _mes) classes += " rvm-mini-fora";
        if (dia == Hoje) classes += " rvm-mini-hoje";
        if (dia == Value) classes += " rvm-mini-escolhido";
        return classes;
    }

    private string Rotulo(DateOnly dia, RvmMiniCalendarDay? info)
    {
        var rotulo = DatasPorExtenso.Completa(dia);
        if (dia == Hoje) rotulo += ", hoje";
        if (!string.IsNullOrWhiteSpace(info?.Description)) rotulo += $", {info.Description}";
        return rotulo;
    }

    private async Task TrocarMesAsync(int meses)
    {
        _mes = _mes.AddMonths(meses);
        _foco = FocoInicial();
        await MonthChanged.InvokeAsync(_mes);
    }

    private async Task EscolherAsync(DateOnly dia)
    {
        _foco = dia;
        await ValueChanged.InvokeAsync(AllowDeselect && dia == Value ? null : dia);
    }

    private async Task AoTeclarAsync(KeyboardEventArgs e)
    {
        DateOnly? destino = e.Key switch
        {
            "ArrowLeft" => _foco.AddDays(-1),
            "ArrowRight" => _foco.AddDays(1),
            "ArrowUp" => _foco.AddDays(-7),
            "ArrowDown" => _foco.AddDays(7),
            "Home" => _foco.AddDays(-(int)_foco.DayOfWeek),
            "End" => _foco.AddDays(6 - (int)_foco.DayOfWeek),
            "PageUp" => e.ShiftKey ? _foco.AddYears(-1) : _foco.AddMonths(-1),
            "PageDown" => e.ShiftKey ? _foco.AddYears(1) : _foco.AddMonths(1),
            _ => null
        };

        if (destino is not { } novo)
        {
            return;
        }

        _foco = novo;
        _focarDia = true;
        if (Primeiro(novo) != _mes)
        {
            _mes = Primeiro(novo);
            await MonthChanged.InvokeAsync(_mes);
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // A tabela e recriada a cada mes (@key): o bloqueio de rolagem das setas e preso de novo na tabela nova.
        if (_mesPreso != _mes)
        {
            _mesPreso = _mes;
            await Tentar(async () =>
            {
                _modulo ??= await JS.InvokeAsync<IJSObjectReference>("import", "./_content/RVM.DesignSystem/rvm-teclado.js");
                await _modulo.InvokeVoidAsync("prenderTeclas", _grade, TeclasDaGrade);
            });
        }

        if (_focarDia)
        {
            _focarDia = false;
            await Tentar(() => _dias[_foco.DayNumber - Inicio.DayNumber].FocusAsync());
        }
    }

    private static async Task<bool> Tentar(Func<ValueTask> acao)
    {
        try
        {
            await acao();
            return true;
        }
        catch (Exception e) when (e is JSException or JSDisconnectedException or InvalidOperationException or TaskCanceledException)
        {
            // Sem JS: a grade funciona pelo clique e pelas teclas; so o foco e a rolagem nao sao controlados.
            return false;
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
