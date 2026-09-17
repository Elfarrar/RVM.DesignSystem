using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Snackbar;

/// <summary>
/// Onde as mensagens do <see cref="RvmSnackbarService"/> aparecem. Um por layout, fora de qualquer
/// area que role.
/// </summary>
public partial class RvmSnackbarHost : ComponentBase, IDisposable
{
    private readonly Dictionary<int, CancellationTokenSource> _relogios = [];

    [Inject] private RvmSnackbarService Servico { get; set; } = default!;

    /// <summary>Nome acessivel do botao de fechar de cada mensagem. Padrao: "Fechar mensagem".</summary>
    [Parameter] public string CloseLabel { get; set; } = "Fechar mensagem";

    /// <summary>Atributos extras, repassados a raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal IReadOnlyList<RvmSnackbarMessage> Visiveis => Servico.Visiveis;

    internal string ClassesDaRaiz
        => AdditionalAttributes is not null
           && AdditionalAttributes.TryGetValue("class", out var informada)
           && informada is string texto
           && !string.IsNullOrWhiteSpace(texto)
            ? $"avisos {texto}"
            : "avisos";

    internal static string ClassesDaMensagem(RvmSnackbarMessage mensagem)
        => mensagem.Options.Color switch
        {
            null => "mensagem neutra",
            RvmColor.Secondary => "mensagem colorida secondary",
            RvmColor.Info => "mensagem colorida info",
            RvmColor.Success => "mensagem colorida success",
            RvmColor.Warning => "mensagem colorida warning",
            RvmColor.Error => "mensagem colorida error",
            _ => "mensagem colorida primary"
        };

    internal static RvmIconName? IconeDe(RvmColor? cor) => cor switch
    {
        RvmColor.Success => RvmIconName.CircleCheck,
        RvmColor.Error => RvmIconName.AlertCircle,
        RvmColor.Warning => RvmIconName.AlertTriangle,
        RvmColor.Info or RvmColor.Primary or RvmColor.Secondary => RvmIconName.InfoCircle,
        _ => null
    };

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Servico.Mudou += AoMudar;
        ArmarRelogios();
    }

    private void AoMudar()
        => _ = InvokeAsync(() =>
        {
            ArmarRelogios();
            StateHasChanged();
        });

    /// <summary>Cada mensagem visivel com duracao ganha um relogio; as que sairam perdem o delas.</summary>
    private void ArmarRelogios()
    {
        var visiveis = Servico.Visiveis;
        foreach (var id in _relogios.Keys.Where(id => visiveis.All(m => m.Id != id)).ToList())
        {
            _relogios[id].Cancel();
            _relogios[id].Dispose();
            _relogios.Remove(id);
        }

        foreach (var mensagem in visiveis.Where(m => !_relogios.ContainsKey(m.Id)))
        {
            Armar(mensagem);
        }
    }

    private void Armar(RvmSnackbarMessage mensagem)
    {
        if (mensagem.Options.Duration is not { } duracao || duracao <= TimeSpan.Zero)
        {
            // Sem duracao: fica ate alguem fechar. O registro vazio evita rearmar a cada mudanca.
            _relogios[mensagem.Id] = new CancellationTokenSource();
            return;
        }

        var relogio = new CancellationTokenSource();
        _relogios[mensagem.Id] = relogio;
        _ = EsperarEFecharAsync(mensagem, duracao, relogio.Token);
    }

    private async Task EsperarEFecharAsync(RvmSnackbarMessage mensagem, TimeSpan duracao, CancellationToken token)
    {
        try
        {
            await Task.Delay(duracao, token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        await InvokeAsync(() => Servico.Close(mensagem));
    }

    /// <summary>
    /// Mouse ou foco na mensagem: o tempo para (WCAG 2.2.1). Ao sair, recomeca inteiro — quem estava
    /// lendo nao perde a mensagem no meio da frase.
    /// </summary>
    internal void Pausar(RvmSnackbarMessage mensagem)
    {
        if (_relogios.TryGetValue(mensagem.Id, out var relogio))
        {
            relogio.Cancel();
        }
    }

    internal void Retomar(RvmSnackbarMessage mensagem)
    {
        if (!Servico.Visiveis.Contains(mensagem))
        {
            return;
        }

        if (_relogios.Remove(mensagem.Id, out var antigo))
        {
            antigo.Dispose();
        }

        Armar(mensagem);
    }

    private async Task AgirAsync(RvmSnackbarMessage mensagem)
    {
        if (mensagem.Options.OnAction is { } acao)
        {
            await acao();
        }

        Servico.Close(mensagem);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Servico.Mudou -= AoMudar;
        foreach (var relogio in _relogios.Values)
        {
            relogio.Cancel();
            relogio.Dispose();
        }

        _relogios.Clear();
        GC.SuppressFinalize(this);
    }
}
