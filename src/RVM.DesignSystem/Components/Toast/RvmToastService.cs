namespace RVM.DesignSystem.Components.Toast;

/// <summary>Fila de avisos com fechamento automatico. Registrado por <c>AddRvmToast()</c>.</summary>
public sealed class RvmToastService : IRvmToast, IDisposable
{
    /// <summary>Mais do que isso empilhado vira ruido: o mais antigo sai.</summary>
    public const int MaxVisible = 5;

    /// <summary>Duracao padrao do aviso com botao de acao: ler e decidir leva mais tempo (WCAG 2.2.1).</summary>
    public static readonly TimeSpan ActionDuration = TimeSpan.FromSeconds(10);

    private readonly object _trava = new();
    private readonly List<RvmToastMessage> _fila = [];
    private readonly Dictionary<Guid, ITimer> _relogios = [];
    private readonly TimeProvider _tempo;

    /// <summary>Fila com o relogio do sistema.</summary>
    public RvmToastService()
        : this(TimeProvider.System)
    {
    }

    /// <summary>Fila com um relogio proprio (o teste avanca o tempo sem esperar de verdade).</summary>
    /// <param name="timeProvider">O relogio.</param>
    public RvmToastService(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        _tempo = timeProvider;
    }

    /// <inheritdoc />
    public event Action? Changed;

    /// <inheritdoc />
    public IReadOnlyList<RvmToastMessage> Messages
    {
        get
        {
            lock (_trava)
            {
                return _fila.ToArray();
            }
        }
    }

    /// <summary>Duracao padrao por gravidade: erro e atencao ficam mais, porque pedem leitura e acao.</summary>
    /// <param name="severity">A gravidade.</param>
    /// <returns>8 segundos para erro e atencao; 5 para o resto.</returns>
    public static TimeSpan DefaultDuration(RvmToastSeverity severity) => severity switch
    {
        RvmToastSeverity.Error or RvmToastSeverity.Warning => TimeSpan.FromSeconds(8),
        _ => TimeSpan.FromSeconds(5)
    };

    /// <inheritdoc />
    public Guid Show(string text, RvmToastSeverity severity = RvmToastSeverity.Info, string? title = null, TimeSpan? duration = null)
        => Adicionar(text, severity, title, duration ?? DefaultDuration(severity), action: null);

    /// <inheritdoc />
    public Guid Show(string text, RvmToastAction action, RvmToastSeverity severity = RvmToastSeverity.Info, string? title = null, TimeSpan? duration = null)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (string.IsNullOrWhiteSpace(action.Label))
        {
            throw new ArgumentException("A acao precisa de rotulo.", nameof(action));
        }

        ArgumentNullException.ThrowIfNull(action.OnClick, nameof(action));
        return Adicionar(text, severity, title, duration ?? ActionDuration, action);
    }

    private static readonly TimeSpan DuracaoMaxima = TimeSpan.FromMilliseconds(uint.MaxValue - 1);

    private Guid Adicionar(string text, RvmToastSeverity severity, string? title, TimeSpan tempo, RvmToastAction? action)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("O aviso precisa de texto.", nameof(text));
        }

        if (tempo < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException("duration", tempo, "A duracao nao pode ser negativa.");
        }

        // O relogio aceita ate ~49 dias; acima, o aviso entraria na fila e o timer lancaria depois, deixando-o preso.
        if (tempo > DuracaoMaxima)
        {
            throw new ArgumentOutOfRangeException("duration", tempo, "A duracao passa do maximo do relogio (49 dias). Use zero para o aviso ficar ate ser fechado.");
        }

        var aviso = new RvmToastMessage(Guid.NewGuid(), text, severity, title, tempo) { Action = action };
        lock (_trava)
        {
            _fila.Add(aviso);
            while (_fila.Count > MaxVisible)
            {
                PararRelogio(_fila[0].Id);
                _fila.RemoveAt(0);
            }

            Agendar(aviso);
        }

        Changed?.Invoke();
        return aviso.Id;
    }

    /// <inheritdoc />
    public void Dismiss(Guid id)
    {
        bool tirou;
        lock (_trava)
        {
            PararRelogio(id);
            tirou = _fila.RemoveAll(a => a.Id == id) > 0;
        }

        if (tirou)
        {
            Changed?.Invoke();
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        lock (_trava)
        {
            PararTodos();
            _fila.Clear();
        }

        Changed?.Invoke();
    }

    /// <inheritdoc />
    public void Pause(Guid id)
    {
        lock (_trava)
        {
            PararRelogio(id);
        }
    }

    /// <inheritdoc />
    public void Resume(Guid id)
    {
        lock (_trava)
        {
            var aviso = _fila.Find(a => a.Id == id);
            if (aviso is not null && !_relogios.ContainsKey(id))
            {
                Agendar(aviso);
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        lock (_trava)
        {
            PararTodos();
        }
    }

    // Chamado com a trava. Duracao zero: aviso fixo, sem relogio.
    private void Agendar(RvmToastMessage aviso)
    {
        if (aviso.Duration == TimeSpan.Zero)
        {
            return;
        }

        _relogios[aviso.Id] = _tempo.CreateTimer(_ => Dismiss(aviso.Id), null, aviso.Duration, Timeout.InfiniteTimeSpan);
    }

    private void PararRelogio(Guid id)
    {
        if (_relogios.Remove(id, out var relogio))
        {
            relogio.Dispose();
        }
    }

    private void PararTodos()
    {
        foreach (var relogio in _relogios.Values)
        {
            relogio.Dispose();
        }

        _relogios.Clear();
    }
}
