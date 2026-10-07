using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Toast;

/// <summary>
/// Onde os avisos do <see cref="IRvmToast"/> aparecem: no canto superior direito, abaixo da barra do topo (o
/// <c>RvmSnackbarHost</c> fica no inferior esquerdo). Um por layout, fora de qualquer area que role. Ponteiro ou
/// foco em cima do aviso suspende o fechamento automatico (WCAG 2.2.1).
/// </summary>
public partial class RvmToastProvider : ComponentBase, IDisposable
{
    private readonly HashSet<Guid> _comMouse = [];
    private readonly HashSet<Guid> _comFoco = [];

    [Inject] private IRvmToast Toast { get; set; } = default!;

    /// <summary>Classe CSS extra na regiao.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Nome acessivel do botao de fechar de cada aviso. Padrao: "Fechar aviso".</summary>
    [Parameter] public string CloseLabel { get; set; } = "Fechar aviso";

    /// <summary>Nome da regiao para leitor de tela. Padrao: "Avisos".</summary>
    [Parameter] public string RegionLabel { get; set; } = "Avisos";

    private IReadOnlyList<RvmToastMessage> Avisos => Toast.Messages;

    internal string ClassesDaRaiz => ClassesCss.Juntar("rvm-toasts", Class, null);

    internal static bool EhUrgente(RvmToastSeverity gravidade)
        => gravidade is RvmToastSeverity.Error or RvmToastSeverity.Warning;

    internal static string ClassesDoAviso(RvmToastSeverity gravidade) => gravidade switch
    {
        RvmToastSeverity.Success => "rvm-toast rvm-success",
        RvmToastSeverity.Warning => "rvm-toast rvm-warning",
        RvmToastSeverity.Error => "rvm-toast rvm-error",
        _ => "rvm-toast rvm-info"
    };

    internal static RvmIconName IconeDe(RvmToastSeverity gravidade) => gravidade switch
    {
        RvmToastSeverity.Success => RvmIconName.CircleCheck,
        RvmToastSeverity.Warning => RvmIconName.AlertTriangle,
        RvmToastSeverity.Error => RvmIconName.AlertCircle,
        _ => RvmIconName.InfoCircle
    };

    /// <inheritdoc />
    protected override void OnInitialized() => Toast.Changed += AoMudar;

    // O relogio do servico dispara fora do contexto do renderer: sem InvokeAsync o Blazor Server recusa.
    private void AoMudar() => _ = InvokeAsync(StateHasChanged);

    // Mouse e foco contam separados: tirar o mouse com o foco ainda dentro nao retoma o tempo.
    private void Entrou(HashSet<Guid> conjunto, Guid id)
    {
        conjunto.Add(id);
        Toast.Pause(id);
    }

    private void Saiu(HashSet<Guid> conjunto, Guid id)
    {
        conjunto.Remove(id);
        if (!_comMouse.Contains(id) && !_comFoco.Contains(id))
        {
            Toast.Resume(id);
        }
    }

    // Roda o callback do app e fecha o aviso mesmo se ele falhar: o erro sobe para o tratamento do app, mas o
    // botao nao fica na tela convidando a clicar de novo.
    private async Task ExecutarAsync(Guid id, RvmToastAction acao)
    {
        try
        {
            await acao.OnClick();
        }
        finally
        {
            _comMouse.Remove(id);
            _comFoco.Remove(id);
            Toast.Dismiss(id);
        }
    }

    /// <summary>Solta a assinatura do evento: o servico tem escopo do circuito e vive mais que o layout.</summary>
    public void Dispose()
    {
        Toast.Changed -= AoMudar;
        GC.SuppressFinalize(this);
    }
}
