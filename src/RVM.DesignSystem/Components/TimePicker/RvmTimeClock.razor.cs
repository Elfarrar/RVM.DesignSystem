using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace RVM.DesignSystem.Components.TimePicker;

/// <summary>
/// Relogio circular para escolher um horario: primeiro a hora, depois os minutos. Aparece solto na
/// pagina ou dentro do <see cref="RvmTimePicker"/> no modo <see cref="RvmTimePickerMode.Clock"/>.
/// </summary>
public partial class RvmTimeClock : ComponentBase, IAsyncDisposable
{
    /// <summary>Lado do mostrador, em px. MEDIDO no kit.</summary>
    internal const int Lado = 260;

    /// <summary>Raio dos numeros de fora (e das 12 horas em 12 h) e dos de dentro (13 a 00 em 24 h).</summary>
    internal const int RaioDeFora = 108;

    internal const int RaioDeDentro = 72;

    private static int _proximoId;
    private readonly string _idBase = $"rvm-relogio-{Interlocked.Increment(ref _proximoId)}";
    private ElementReference _mostrador;
    private IJSObjectReference? _modulo;
    private RvmTimeClockView? _visaoInterna;

    [Inject] private IJSRuntime JS { get; set; } = default!;

    /// <summary>O horario escolhido. Aceita <c>@bind-Value</c>.</summary>
    [Parameter] public TimeOnly? Value { get; set; }

    /// <summary>Disparado a cada mudanca de hora, minuto ou periodo.</summary>
    [Parameter] public EventCallback<TimeOnly?> ValueChanged { get; set; }

    /// <summary>Horas ou minutos no mostrador. Aceita <c>@bind-View</c>. Padrao: horas.</summary>
    [Parameter] public RvmTimeClockView View { get; set; } = RvmTimeClockView.Hours;

    /// <summary>Disparado quando o mostrador troca entre horas e minutos.</summary>
    [Parameter] public EventCallback<RvmTimeClockView> ViewChanged { get; set; }

    /// <summary>24 h (padrao, com 13 a 00 no anel de dentro) ou 12 h com AM e PM.</summary>
    [Parameter] public bool Use24Hours { get; set; } = true;

    /// <summary>Minutos entre uma opcao e a proxima, de 1 a 30. Padrao: 1.</summary>
    [Parameter] public int Step { get; set; } = 1;

    /// <summary>
    /// Primeiro horario escolhivel. Padrao: 00:00. Maior que <see cref="Max"/>, a janela cruza a meia-noite
    /// (22:00 a 06:00, um plantao noturno).
    /// </summary>
    [Parameter] public TimeOnly Min { get; set; } = TimeOnly.MinValue;

    /// <summary>Ultimo horario escolhivel (inclusive). Padrao: 23:59.</summary>
    [Parameter] public TimeOnly Max { get; set; } = new(23, 59);

    /// <summary>Titulo pequeno no topo. Padrao: "Selecione o horario".</summary>
    [Parameter] public string Title { get; set; } = "Selecione o horario";

    /// <summary>Botoes abaixo do mostrador (o CANCELAR e OK do dialogo).</summary>
    [Parameter] public RenderFragment? Actions { get; set; }

    /// <summary>Enter nos minutos: quem esta num dialogo confirma a escolha aqui.</summary>
    [Parameter] public EventCallback OnConfirm { get; set; }

    /// <summary>Atributos extras, repassados a raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>A visao mostrada: a de fora, ou a que a pessoa escolheu sem <c>@bind-View</c>.</summary>
    internal RvmTimeClockView Visao => _visaoInterna ?? View;

    internal int Passo => Math.Clamp(Step, 1, 30);

    /// <summary>
    /// O horario desenhado. Sem valor, o primeiro escolhivel a partir do meio-dia: o mostrador precisa de
    /// um ponteiro e de uma opcao ativa para o teclado. Nada e avisado ate a pessoa mexer.
    /// </summary>
    internal TimeOnly Atual => Value ?? Limitar(new TimeOnly(12, 0));

    internal bool Pm => Atual.Hour >= 12;

    internal string IdMostrador => $"{_idBase}-mostrador";

    internal string IdOpcao(int valor) => $"{_idBase}-{(Visao == RvmTimeClockView.Hours ? "h" : "m")}-{valor}";

    internal string TextoHoras => Value is { } v ? (Use24Hours ? v.Hour : Hora12(v.Hour)).ToString("00", CultureInfo.InvariantCulture) : "--";

    internal string TextoMinutos => Value is { } v ? v.Minute.ToString("00", CultureInfo.InvariantCulture) : "--";

    internal static int Hora12(int hora24) => hora24 % 12 == 0 ? 12 : hora24 % 12;

    // Min > Max e janela que cruza a meia-noite: fora e so o que fica entre o fim e o comeco. Tratar como
    // janela linear desabilitava o relogio inteiro (achado do review).
    internal bool Fora(TimeOnly t) => Min <= Max ? t < Min || t > Max : t < Min && t > Max;

    /// <summary>Os minutos escolhiveis numa hora, de <see cref="Passo"/> em <see cref="Passo"/>.</summary>
    internal IEnumerable<int> MinutosValidos(int hora)
        => Enumerable.Range(0, 60).Where(m => m % Passo == 0 && !Fora(new TimeOnly(hora, m)));

    internal bool HoraDesabilitada(int hora24) => !MinutosValidos(hora24).Any();

    internal bool MinutoDesabilitado(int minuto) => Fora(new TimeOnly(Atual.Hour, minuto));

    private TimeOnly Limitar(TimeOnly t)
    {
        if (Min <= Max)
        {
            if (t < Min) t = Min;
            if (t > Max) t = Max;
        }
        else if (Fora(t))
        {
            t = Min;
        }

        var validos = MinutosValidos(t.Hour).ToList();
        if (validos.Count == 0 || validos.Contains(t.Minute))
        {
            return t;
        }

        var proximo = validos.Where(m => m >= t.Minute).DefaultIfEmpty(validos[^1]).First();
        return new TimeOnly(t.Hour, proximo);
    }

    /// <summary>Uma opcao do mostrador: valor, texto, posicao e estado.</summary>
    internal sealed record Opcao(int Valor, string Texto, string Rotulo, int X, int Y, bool Dentro, bool Visivel, bool Escolhida, bool Desabilitada);

    internal IReadOnlyList<Opcao> Opcoes => Visao == RvmTimeClockView.Hours ? OpcoesDeHora() : OpcoesDeMinuto();

    private List<Opcao> OpcoesDeHora()
    {
        var lista = new List<Opcao>();
        if (Use24Hours)
        {
            // Fora: 1 a 12 (12 no alto). Dentro: 00 no alto e 13 a 23 — como o relogio de 24 h do MUI.
            for (var h = 1; h <= 12; h++)
            {
                lista.Add(NovaHora(h, h.ToString(CultureInfo.InvariantCulture), h % 12, dentro: false));
            }

            for (var h = 12; h <= 23; h++)
            {
                var valor = h == 12 ? 0 : h;
                lista.Add(NovaHora(valor, valor.ToString("00", CultureInfo.InvariantCulture), h % 12, dentro: true));
            }
        }
        else
        {
            for (var h = 1; h <= 12; h++)
            {
                var valor = (h % 12) + (Pm ? 12 : 0);
                lista.Add(NovaHora(valor, h.ToString(CultureInfo.InvariantCulture), h % 12, dentro: false));
            }
        }

        return lista;
    }

    private Opcao NovaHora(int hora24, string texto, int posicao, bool dentro)
    {
        var (x, y) = Posicao(posicao * 30, dentro ? RaioDeDentro : RaioDeFora);
        var rotulo = Use24Hours ? $"{hora24} horas" : $"{texto} horas";
        return new Opcao(hora24, texto, rotulo, x, y, dentro, Visivel: true, Escolhida: Atual.Hour == hora24, HoraDesabilitada(hora24));
    }

    private List<Opcao> OpcoesDeMinuto()
    {
        var lista = new List<Opcao>();
        // Valor gravado fora do passo (09:47 com Step 15) ganha a opcao dele: sem ela nao havia opcao ativa
        // para o leitor de tela, e o ponteiro apontava para o vazio (achado do review). As setas seguem o passo.
        var minutos = Enumerable.Range(0, 60).Where(m => m % Passo == 0 || m == Atual.Minute);
        foreach (var m in minutos)
        {
            var (x, y) = Posicao(m * 6, RaioDeFora);
            var texto = m.ToString("00", CultureInfo.InvariantCulture);
            lista.Add(new Opcao(m, texto, $"{texto} minutos", x, y, Dentro: false, Visivel: m % 5 == 0,
                Escolhida: Atual.Minute == m, MinutoDesabilitado(m)));
        }

        return lista;
    }

    /// <summary>Canto de cima-esquerda da opcao de 40 px, com o angulo em graus a partir do alto, no sentido horario.</summary>
    private static (int X, int Y) Posicao(double graus, int raio)
    {
        var rad = graus * Math.PI / 180;
        return ((int)Math.Round(Lado / 2d + raio * Math.Sin(rad) - 20), (int)Math.Round(Lado / 2d - raio * Math.Cos(rad) - 20));
    }

    /// <summary>Angulo e comprimento do ponteiro, ja em texto invariante para o atributo style.</summary>
    internal string EstiloDoPonteiro
    {
        get
        {
            double graus;
            int raio;
            if (Visao == RvmTimeClockView.Hours)
            {
                graus = Atual.Hour % 12 * 30;
                raio = Use24Hours && (Atual.Hour == 0 || Atual.Hour > 12) ? RaioDeDentro : RaioDeFora;
            }
            else
            {
                graus = Atual.Minute * 6;
                raio = RaioDeFora;
            }

            return string.Create(CultureInfo.InvariantCulture, $"height: {raio}px; transform: rotate({graus}deg)");
        }
    }

    internal static string EstiloDaOpcao(Opcao o) => string.Create(CultureInfo.InvariantCulture, $"left: {o.X}px; top: {o.Y}px");

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = "rvm-relogio";
            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }

    internal async Task MudarVisaoAsync(RvmTimeClockView visao)
    {
        if (Visao == visao)
        {
            return;
        }

        _visaoInterna = visao;
        await ViewChanged.InvokeAsync(visao);
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        // A visao de fora vale quando ela MUDA; sem @bind-View, o render do pai nao desfaz a troca.
        if (_visaoRecebida != View)
        {
            _visaoRecebida = View;
            _visaoInterna = null;
        }
    }

    private RvmTimeClockView? _visaoRecebida;

    private async Task DefinirAsync(TimeOnly t)
    {
        if (Fora(t) || Value == t)
        {
            return;
        }

        Value = t;
        await ValueChanged.InvokeAsync(t);
    }

    internal Task EscolherHoraAsync(int hora24)
    {
        if (HoraDesabilitada(hora24))
        {
            return Task.CompletedTask;
        }

        // Mantem o minuto quando da; senao, o escolhivel mais perto naquela hora.
        var validos = MinutosValidos(hora24).ToList();
        var minuto = validos.Contains(Atual.Minute) ? Atual.Minute : validos.OrderBy(m => Math.Abs(m - Atual.Minute)).First();
        return DefinirAsync(new TimeOnly(hora24, minuto));
    }

    internal Task EscolherMinutoAsync(int minuto)
        => MinutoDesabilitado(minuto) ? Task.CompletedTask : DefinirAsync(new TimeOnly(Atual.Hour, minuto));

    internal Task EscolherPeriodoAsync(bool pm)
    {
        if (Pm == pm)
        {
            return Task.CompletedTask;
        }

        var hora = (Atual.Hour + 12) % 24;
        return PeriodoDesabilitado(pm) ? Task.CompletedTask : EscolherHoraAsync(hora);
    }

    internal bool PeriodoDesabilitado(bool pm) => Enumerable.Range(pm ? 12 : 0, 12).All(HoraDesabilitada);

    internal async Task EscolherOpcaoAsync(int valor)
    {
        if (Visao == RvmTimeClockView.Hours)
        {
            await EscolherHoraAsync(valor);
        }
        else
        {
            await EscolherMinutoAsync(valor);
        }
    }

    // --- Ponteiro do mouse / toque ---
    // As opcoes nao recebem o ponteiro (pointer-events: none): o alvo e sempre o mostrador, e OffsetX/Y
    // vem em relacao a ele. Assim da para clicar em qualquer ponto e arrastar, sem JS.

    private bool _arrastando;

    internal async Task AoApertarAsync(PointerEventArgs e)
    {
        _arrastando = true;
        await EscolherPeloPontoAsync(e.OffsetX, e.OffsetY);
    }

    internal async Task AoMoverAsync(PointerEventArgs e)
    {
        if (_arrastando && (e.Buttons & 1) == 1)
        {
            await EscolherPeloPontoAsync(e.OffsetX, e.OffsetY);
        }
    }

    internal async Task AoSoltarAsync(PointerEventArgs e)
    {
        if (!_arrastando)
        {
            return;
        }

        _arrastando = false;
        await EscolherPeloPontoAsync(e.OffsetX, e.OffsetY);
        if (Visao == RvmTimeClockView.Hours)
        {
            // Soltar a hora leva aos minutos, como no MUI.
            await MudarVisaoAsync(RvmTimeClockView.Minutes);
        }
    }

    internal void AoSairDoMostrador() => _arrastando = false;

    internal Task EscolherPeloPontoAsync(double x, double y)
    {
        var dx = x - Lado / 2d;
        var dy = Lado / 2d - y;
        var graus = (Math.Atan2(dx, dy) * 180 / Math.PI + 360) % 360;
        var distancia = Math.Sqrt(dx * dx + dy * dy);

        if (Visao == RvmTimeClockView.Minutes)
        {
            var minuto = (int)Math.Round(graus / 6) % 60;
            minuto = (int)Math.Round(minuto / (double)Passo) * Passo % 60;
            return EscolherMinutoAsync(minuto);
        }

        var posicao = (int)Math.Round(graus / 30) % 12;
        int hora;
        if (Use24Hours)
        {
            var dentro = distancia < (RaioDeFora + RaioDeDentro) / 2d;
            hora = dentro ? (posicao == 0 ? 0 : posicao + 12) : (posicao == 0 ? 12 : posicao);
        }
        else
        {
            hora = posicao + (Pm ? 12 : 0);
        }

        return EscolherHoraAsync(hora);
    }

    // --- Teclado ---

    internal async Task AoTeclarAsync(KeyboardEventArgs e)
    {
        switch (e.Key)
        {
            case "ArrowUp" or "ArrowRight":
                await AndarAsync(+1);
                break;
            case "ArrowDown" or "ArrowLeft":
                await AndarAsync(-1);
                break;
            case "Home":
                await IrParaPontaAsync(primeira: true);
                break;
            case "End":
                await IrParaPontaAsync(primeira: false);
                break;
            case "Enter" or " ":
                if (Value is null)
                {
                    await DefinirAsync(Atual);
                }

                if (Visao == RvmTimeClockView.Hours)
                {
                    await MudarVisaoAsync(RvmTimeClockView.Minutes);
                }
                else
                {
                    await OnConfirm.InvokeAsync();
                }

                break;
        }
    }

    /// <summary>Os valores na ordem do teclado: horas do periodo (ou do dia) e minutos, sem os desabilitados.</summary>
    private List<int> Sequencia()
        => Visao == RvmTimeClockView.Hours
            ? [.. (Use24Hours ? Enumerable.Range(0, 24) : Enumerable.Range(Pm ? 12 : 0, 12)).Where(h => !HoraDesabilitada(h))]
            : [.. Enumerable.Range(0, 60).Where(m => m % Passo == 0 && !MinutoDesabilitado(m))];

    private Task AndarAsync(int direcao)
    {
        var sequencia = Sequencia();
        if (sequencia.Count == 0)
        {
            return Task.CompletedTask;
        }

        var atual = Visao == RvmTimeClockView.Hours ? Atual.Hour : Atual.Minute;
        int alvo;
        if (Value is null)
        {
            // Primeira tecla so "acende" o valor mostrado.
            alvo = sequencia.Contains(atual) ? atual : sequencia[0];
        }
        else if (direcao > 0)
        {
            alvo = sequencia.FirstOrDefault(v => v > atual, sequencia[0]);
        }
        else
        {
            alvo = sequencia.LastOrDefault(v => v < atual, sequencia[^1]);
        }

        return EscolherOpcaoAsync(alvo);
    }

    private Task IrParaPontaAsync(bool primeira)
    {
        var sequencia = Sequencia();
        return sequencia.Count == 0 ? Task.CompletedTask : EscolherOpcaoAsync(primeira ? sequencia[0] : sequencia[^1]);
    }

    /// <summary>Leva o foco ao mostrador.</summary>
    public async Task FocusAsync()
    {
        try
        {
            await _mostrador.FocusAsync();
        }
        catch (Exception e) when (e is JSException or InvalidOperationException or TaskCanceledException)
        {
            // Sem JS (pre-renderizacao): o foco fica onde esta.
        }
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
            // Enter tambem: ele confirma e fecha o dialogo AINDA no keydown, o foco volta ao botao do campo,
            // e o keypress do mesmo Enter "clicava" nele e reabria o relogio (pego no E2E).
            await _modulo.InvokeVoidAsync("prenderTeclas", _mostrador, new[] { "ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight", "Home", "End", " ", "Enter" });
        }
        catch (Exception e) when (e is JSException or InvalidOperationException or TaskCanceledException)
        {
            // Sem JS: as setas funcionam, so a pagina rola junto.
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
