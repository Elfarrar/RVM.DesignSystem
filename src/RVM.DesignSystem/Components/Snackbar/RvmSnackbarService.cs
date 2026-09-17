namespace RVM.DesignSystem.Components.Snackbar;

/// <summary>
/// Mostra mensagens curtas no canto da tela, em fila. Registrado por
/// <c>services.AddRvmDesignSystem()</c>; as mensagens aparecem onde houver um
/// <see cref="RvmSnackbarHost"/> (normalmente no layout).
/// </summary>
public sealed class RvmSnackbarService
{
    private readonly object _trava = new();
    private readonly List<RvmSnackbarMessage> _fila = [];

    /// <summary>Quantas mensagens ficam na tela ao mesmo tempo; as outras esperam. Padrao: 3.</summary>
    public int MaxVisible { get; set; } = 3;

    /// <summary>Disparado quando a fila muda.</summary>
    internal event Action? Mudou;

    /// <summary>As mensagens na tela agora, da mais antiga para a mais nova.</summary>
    internal IReadOnlyList<RvmSnackbarMessage> Visiveis
    {
        get
        {
            lock (_trava)
            {
                return _fila.Take(Math.Max(MaxVisible, 1)).ToList();
            }
        }
    }

    /// <summary>Quantas estao esperando a vez.</summary>
    internal int EmEspera
    {
        get
        {
            lock (_trava)
            {
                return Math.Max(_fila.Count - Math.Max(MaxVisible, 1), 0);
            }
        }
    }

    /// <summary>Poe uma mensagem na fila.</summary>
    /// <param name="text">O texto, em uma frase: o que aconteceu e, se preciso, o proximo passo.</param>
    /// <param name="options">Cor, acao, duracao. Sem valor, neutra por 6 segundos.</param>
    public RvmSnackbarMessage Show(string text, RvmSnackbarOptions? options = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        var mensagem = new RvmSnackbarMessage(text, options ?? new RvmSnackbarOptions());
        lock (_trava)
        {
            _fila.Add(mensagem);
        }

        Mudou?.Invoke();
        return mensagem;
    }

    /// <summary>Fecha uma mensagem (na tela ou na fila). A proxima da fila toma o lugar.</summary>
    public void Close(RvmSnackbarMessage message)
    {
        bool removida;
        lock (_trava)
        {
            removida = _fila.Remove(message);
        }

        if (removida)
        {
            Mudou?.Invoke();
        }
    }

    /// <summary>Fecha todas, inclusive as da fila.</summary>
    public void CloseAll()
    {
        lock (_trava)
        {
            if (_fila.Count == 0)
            {
                return;
            }

            _fila.Clear();
        }

        Mudou?.Invoke();
    }
}
