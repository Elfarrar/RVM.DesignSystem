using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace RVM.DesignSystem.Components.Calendar;

/// <summary>
/// Calendario de um mes para escolher uma data ou um intervalo. Aparece solto na pagina ou dentro do
/// <c>RvmDatePicker</c>.
/// </summary>
public partial class RvmCalendar : ComponentBase, IAsyncDisposable
{
    private static readonly string[] Meses =
        ["janeiro", "fevereiro", "marco", "abril", "maio", "junho", "julho", "agosto", "setembro", "outubro", "novembro", "dezembro"];

    private static readonly (string Curto, string Completo)[] Semana =
        [("D", "domingo"), ("S", "segunda-feira"), ("T", "terca-feira"), ("Q", "quarta-feira"), ("Q", "quinta-feira"), ("S", "sexta-feira"), ("S", "sabado")];

    private static int _proximoId;
    private readonly string _idBase = $"rvm-calendario-{Interlocked.Increment(ref _proximoId)}";
    private readonly ElementReference[] _dias = new ElementReference[31];
    private ElementReference _grade;
    private IJSObjectReference? _modulo;
    private DateOnly _foco;
    private bool _focoIniciado;
    private bool _focarAposRender;

    [Inject] private IJSRuntime JS { get; set; } = default!;

    /// <summary>A data escolhida. Aceita <c>@bind-Value</c>. Ignorada com <see cref="IsRange"/>.</summary>
    [Parameter] public DateOnly? Value { get; set; }

    /// <summary>Disparado quando a pessoa escolhe uma data.</summary>
    [Parameter] public EventCallback<DateOnly?> ValueChanged { get; set; }

    /// <summary>Escolhe um intervalo (dois cliques: inicio e fim) em vez de uma data.</summary>
    [Parameter] public bool IsRange { get; set; }

    /// <summary>O intervalo escolhido. Aceita <c>@bind-Range</c>.</summary>
    [Parameter] public RvmDateRange? Range { get; set; }

    /// <summary>Disparado a cada clique do intervalo (o primeiro deixa o fim vazio).</summary>
    [Parameter] public EventCallback<RvmDateRange?> RangeChanged { get; set; }

    /// <summary>Primeira data escolhivel.</summary>
    [Parameter] public DateOnly? Min { get; set; }

    /// <summary>Ultima data escolhivel.</summary>
    [Parameter] public DateOnly? Max { get; set; }

    /// <summary>Datas que nao podem ser escolhidas (domingos, feriados). Continuam focaveis.</summary>
    [Parameter] public Func<DateOnly, bool>? DateDisabled { get; set; }

    /// <summary>"Hoje" para marcar o dia atual. Padrao: a data do sistema. Existe para teste e fuso.</summary>
    [Parameter] public DateOnly? Today { get; set; }

    /// <summary>Atributos extras, repassados a raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal DateOnly Hoje => Today ?? DateOnly.FromDateTime(DateTime.Today);

    internal DateOnly Foco => _foco;

    internal string IdTitulo => $"{_idBase}-mes";

    internal string TituloDoMes => $"{Meses[_foco.Month - 1]} de {_foco.Year}";

    internal static IReadOnlyList<(string Curto, string Completo)> DiasDaSemana => Semana;

    internal static string NomeCompleto(DateOnly d)
        => $"{Semana[(int)d.DayOfWeek].Completo}, {d.Day} de {Meses[d.Month - 1]} de {d.Year}";

    internal string ClassesDaRaiz
        => AdditionalAttributes is not null
           && AdditionalAttributes.TryGetValue("class", out var informada)
           && informada is string texto
           && !string.IsNullOrWhiteSpace(texto)
            ? $"rvm-calendario {texto}"
            : "rvm-calendario";

    /// <summary>As semanas do mes em foco, com <c>null</c> nos dias de outros meses.</summary>
    internal IEnumerable<DateOnly?[]> Semanas
    {
        get
        {
            var primeiro = new DateOnly(_foco.Year, _foco.Month, 1);
            var dias = DateTime.DaysInMonth(_foco.Year, _foco.Month);
            var celulas = new List<DateOnly?>();
            celulas.AddRange(Enumerable.Repeat<DateOnly?>(null, (int)primeiro.DayOfWeek));
            celulas.AddRange(Enumerable.Range(0, dias).Select(i => (DateOnly?)primeiro.AddDays(i)));
            while (celulas.Count % 7 != 0) celulas.Add(null);
            return celulas.Chunk(7);
        }
    }

    internal static int IndiceDe(DateOnly d) => d.Day - 1;

    internal bool EstaBloqueado(DateOnly d)
        => (Min is { } min && d < min) || (Max is { } max && d > max) || (DateDisabled?.Invoke(d) ?? false);

    internal bool EstaEscolhido(DateOnly d)
        => IsRange
            ? Range is { } r && (d == r.Start || d == r.End)
            : Value == d;

    internal string ClassesDoDia(DateOnly d)
    {
        var classes = "rvm-celula";
        if (EstaEscolhido(d)) classes += " rvm-escolhido";
        if (IsRange && Range is { End: not null } r && r.Contains(d)) classes += " rvm-no-intervalo";
        if (d == Hoje) classes += " rvm-hoje";
        if (EstaBloqueado(d)) classes += " rvm-bloqueado";
        return classes;
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (!_focoIniciado)
        {
            _foco = Limitar((IsRange ? Range?.Start : Value) ?? Hoje);
            _focoIniciado = true;
        }
    }

    /// <summary>Leva o foco ao dia em foco (o escolhido, ou hoje). Usado ao abrir o seletor.</summary>
    public async Task FocusAsync()
    {
        await Tentar(() => _dias[IndiceDe(_foco)].FocusAsync());
    }

    private DateOnly Limitar(DateOnly d)
    {
        if (Min is { } min && d < min) return min;
        if (Max is { } max && d > max) return max;
        return d;
    }

    internal bool PodeIrPara(int meses)
    {
        var alvo = new DateOnly(_foco.Year, _foco.Month, 1).AddMonths(meses);
        var fimDoAlvo = alvo.AddMonths(1).AddDays(-1);
        return !(Min is { } min && fimDoAlvo < min) && !(Max is { } max && alvo > max);
    }

    private async Task MudarMesAsync(int meses, bool focar)
    {
        if (!PodeIrPara(meses))
        {
            return;
        }

        await MoverFocoAsync(_foco.AddMonths(meses), focar);
    }

    private async Task MoverFocoAsync(DateOnly destino, bool focar)
    {
        _foco = Limitar(destino);
        _focarAposRender = focar;
        StateHasChanged();
        await Task.CompletedTask;
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

        if (destino is { } d)
        {
            await MoverFocoAsync(d, focar: true);
        }
    }

    private async Task EscolherAsync(DateOnly d)
    {
        _foco = d;
        if (EstaBloqueado(d))
        {
            return;
        }

        if (!IsRange)
        {
            Value = d;
            await ValueChanged.InvokeAsync(d);
            return;
        }

        // Primeiro clique (ou intervalo ja completo): comeca de novo. Segundo: fecha, em qualquer ordem.
        RvmDateRange novo = Range is { End: null } aberto
            ? (d < aberto.Start ? new RvmDateRange(d, aberto.Start) : new RvmDateRange(aberto.Start, d))
            : new RvmDateRange(d, null);
        Range = novo;
        await RangeChanged.InvokeAsync(novo);
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Tentar(async () =>
            {
                _modulo = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/RVM.DesignSystem/rvm-teclado.js");
                await _modulo.InvokeVoidAsync("prenderTeclas", _grade,
                    new[] { "ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight", "Home", "End", "PageUp", "PageDown" });
            });
        }

        if (_focarAposRender)
        {
            _focarAposRender = false;
            await FocusAsync();
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
            // Sem JS: a grade funciona pelo clique; so o foco e a rolagem nao sao controlados.
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
