using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.EventCalendar;

/// <summary>
/// Agenda de compromissos em mes, semana ou dia (contrato com o RVM.UI, DSGN-017). A agenda nao busca nada: os
/// compromissos chegam por <see cref="Events"/>.
/// </summary>
public partial class RvmEventCalendar : ComponentBase
{
    private DateOnly _foco;
    private DateOnly? _dateVisto;
    private bool _iniciado;

    /// <summary>Compromissos a mostrar.</summary>
    [Parameter] public IReadOnlyList<RvmCalendarEvent> Events { get; set; } = [];

    /// <summary>Mes, semana ou dia. Padrao: <see cref="RvmCalendarView.Month"/>.</summary>
    [Parameter] public RvmCalendarView View { get; set; } = RvmCalendarView.Month;

    /// <summary>
    /// Data em foco (no mes, qualquer dia dele). Sem ela, hoje. A navegacao pelas setas e pelo botao de hoje mexe no
    /// estado interno e NAO e desfeita por um render do pai: a agenda so volta a seguir este parametro quando ele muda.
    /// Quem quer guardar a data escuta <see cref="DateChanged"/>.
    /// </summary>
    [Parameter] public DateOnly? Date { get; set; }

    /// <summary>Avisa quando as setas ou o botao de hoje levam a agenda para outra data.</summary>
    [Parameter] public EventCallback<DateOnly> DateChanged { get; set; }

    /// <summary>O instante tratado como agora: marca hoje e desenha a linha do agora. Sem ele, o relogio local.</summary>
    [Parameter] public DateTime? Now { get; set; }

    /// <summary>Primeira hora da grade na semana e no dia. Padrao: 6.</summary>
    [Parameter] public int FirstHour { get; set; } = 6;

    /// <summary>Ultima hora da grade, inclusive. Padrao: 20.</summary>
    [Parameter] public int LastHour { get; set; } = 20;

    /// <summary>Quantos compromissos aparecem no dia (ou na hora) antes do "+N". Padrao: 3.</summary>
    [Parameter] public int MaxEventsPerDay { get; set; } = 3;

    /// <summary>Clique num compromisso. Sem ele as pilulas nao sao botao.</summary>
    [Parameter] public EventCallback<RvmCalendarEvent> OnEventClick { get; set; }

    /// <summary>Clique no "+N" de um dia. Sem ele o "+N" so informa.</summary>
    [Parameter] public EventCallback<DateOnly> OnOverflowClick { get; set; }

    /// <summary>Canto direito do cabecalho: troca de visao, filtro, botao de novo compromisso.</summary>
    [Parameter] public RenderFragment? Actions { get; set; }

    /// <summary>Texto do botao que volta para hoje. Padrao: "Hoje".</summary>
    [Parameter] public string TodayText { get; set; } = "Hoje";

    /// <summary>Nome acessivel da agenda. Sem ele sai "Agenda de {periodo}".</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos repassados ao elemento raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// O botao da ultima pilula clicada, para quem abre um dialogo a partir dela devolver o foco ao fechar. Ja esta
    /// preenchido quando o <see cref="OnEventClick"/> dispara.
    /// </summary>
    public ElementReference? LastClickedEventElement { get; private set; }

    private DateTime Agora => Now ?? DateTime.Now;

    private DateOnly Hoje => DateOnly.FromDateTime(Agora);

    private string ClassesDaRaiz => ClassesCss.Juntar("rvm-agenda", Class, AdditionalAttributes);

    private string ClasseDaVisao => View switch
    {
        RvmCalendarView.Week => "rvm-agenda-semana",
        RvmCalendarView.Day => "rvm-agenda-dia",
        _ => "rvm-agenda-mes"
    };

    // Escritos por extenso, e nao montados: "proximo" concorda com o genero do periodo.
    private string Anterior => View switch
    {
        RvmCalendarView.Week => "Semana anterior",
        RvmCalendarView.Day => "Dia anterior",
        _ => "Mes anterior"
    };

    private string Proximo => View switch
    {
        RvmCalendarView.Week => "Proxima semana",
        RvmCalendarView.Day => "Proximo dia",
        _ => "Proximo mes"
    };

    private string Titulo => View switch
    {
        RvmCalendarView.Week => DatasPorExtenso.Intervalo(InicioDaSemana(), InicioDaSemana().AddDays(6)),
        RvmCalendarView.Day => DatasPorExtenso.Maiuscula(DatasPorExtenso.Completa(_foco)),
        _ => DatasPorExtenso.TituloDoMes(_foco)
    };

    private string NomeAcessivel => string.IsNullOrWhiteSpace(Label) ? $"Agenda de {Titulo}" : Label;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (FirstHour is < 0 or > 23 || LastHour is < 0 or > 23 || LastHour < FirstHour)
        {
            throw new ArgumentOutOfRangeException(
                nameof(FirstHour),
                $"A grade de horas vai de {FirstHour} a {LastHour}: as duas ficam entre 0 e 23, e a ultima nao pode vir antes da primeira.");
        }

        // O Blazor chama isto a cada render do pai, mesmo sem parametro novo: sem a guarda, a navegacao do usuario
        // seria desfeita em silencio. So segue o Date quando ele muda por fora.
        if (!_iniciado || Date != _dateVisto)
        {
            _foco = Date ?? Hoje;
            _dateVisto = Date;
            _iniciado = true;
        }
    }

    private IEnumerable<DateOnly> DiasDaVisao() => View == RvmCalendarView.Day
        ? [_foco]
        : Enumerable.Range(0, 7).Select(i => InicioDaSemana().AddDays(i));

    private DateOnly InicioDaSemana() => _foco.AddDays(-(int)_foco.DayOfWeek);

    private IEnumerable<int> Horas() => Enumerable.Range(FirstHour, LastHour - FirstHour + 1);

    /// <summary>As semanas do quadro do mes, de domingo a sabado, com os dias vizinhos nas pontas.</summary>
    private IEnumerable<DateOnly[]> Semanas()
    {
        var primeiro = new DateOnly(_foco.Year, _foco.Month, 1);
        var ultimo = primeiro.AddMonths(1).AddDays(-1);
        for (var d = primeiro.AddDays(-(int)primeiro.DayOfWeek); d <= ultimo; d = d.AddDays(7))
        {
            var inicio = d;
            yield return [.. Enumerable.Range(0, 7).Select(i => inicio.AddDays(i))];
        }
    }

    private string ClasseDoDia(DateOnly dia)
    {
        var classes = "rvm-agenda-dia-celula";
        if (View == RvmCalendarView.Month && (dia.Month != _foco.Month || dia.Year != _foco.Year))
        {
            classes += " rvm-agenda-fora";
        }

        if (dia == Hoje)
        {
            classes += " rvm-agenda-hoje";
        }

        return classes;
    }

    private bool EhAgora(DateOnly dia, int hora) => dia == Hoje && hora == Agora.Hour;

    // O minuto dentro da linha da hora: 10h30 fica na metade dela.
    private string PosicaoDoAgora =>
        string.Create(CultureInfo.InvariantCulture, $"--rvm-agenda-agora: {Agora.Minute / 60d * 100:0.##}%");

    private List<RvmCalendarEvent> NoDia(DateOnly dia) =>
        [.. Events.Where(e => DateOnly.FromDateTime(e.Start) == dia).OrderBy(e => e.Start)];

    private List<RvmCalendarEvent> NaHora(DateOnly dia, int hora) =>
        [.. Events.Where(e => DateOnly.FromDateTime(e.Start) == dia && e.Start.Hour == hora).OrderBy(e => e.Start)];

    private static string Horario(RvmCalendarEvent e) => $"{e.Start:HH:mm} as {e.EndOrDefault:HH:mm}";

    private EventCallback CliqueNoEvento(RvmCalendarEvent e) => OnEventClick.HasDelegate
        ? EventCallback.Factory.Create(this, () => OnEventClick.InvokeAsync(e))
        : default;

    // Roda antes do clique (a pilula garante a ordem): o tratador do clique ja encontra o valor.
    private void Registrar(ElementReference pilula) => LastClickedEventElement = pilula;

    private Task AndarAsync(int passos) => IrAsync(View switch
    {
        RvmCalendarView.Week => _foco.AddDays(7 * passos),
        RvmCalendarView.Day => _foco.AddDays(passos),
        _ => _foco.AddMonths(passos)
    });

    private Task IrParaHojeAsync() => IrAsync(Hoje);

    private Task IrAsync(DateOnly destino)
    {
        _foco = destino;
        return DateChanged.InvokeAsync(destino);
    }
}
