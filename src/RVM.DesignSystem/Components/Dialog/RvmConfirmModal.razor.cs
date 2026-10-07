using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Dialog;

/// <summary>
/// Pergunta de confirmacao antes de uma acao: icone, titulo, explicacao e os botoes Cancelar e Confirmar.
/// Para acao destrutiva, use <see cref="RvmColor.Error"/>: pinta o icone e o botao de confirmar.
/// </summary>
/// <remarks>
/// E um <c>alertdialog</c>: o leitor de tela le a pergunta e a explicacao juntas. O foco abre no
/// Cancelar, a resposta segura. Desistir pelo botao, pelo Esc ou pelo fundo dispara o mesmo
/// <see cref="OnCancel"/>.
/// </remarks>
public partial class RvmConfirmModal : ComponentBase
{
    private readonly string _idBase = GeradorDeIds.Novo("rvm-confirmacao");
    private readonly PapelDoModal _papel;
    private bool _aberto;
    private bool _ultimoOpen;
    private bool _confirmando;

    /// <summary>Cria a confirmacao com o papel que a caixa do modal vai assumir.</summary>
    public RvmConfirmModal()
    {
        _papel = new PapelDoModal { Alerta = true, NomeadoPor = IdTitulo, Estreito = true };
    }

    /// <summary>Aberto. Aceita <c>@bind-Open</c>.</summary>
    [Parameter] public bool Open { get; set; }

    /// <summary>Disparado quando a confirmacao se fecha (confirmou ou desistiu).</summary>
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }

    /// <summary>A pergunta. E o nome acessivel do dialogo — sem ela o dialogo nao tem nome.</summary>
    [Parameter, EditorRequired] public string Title { get; set; } = string.Empty;

    /// <summary>Explicacao abaixo da pergunta. Diga o que a acao faz, nao repita o titulo.</summary>
    [Parameter] public string? Description { get; set; }

    /// <summary>Corpo livre, no lugar da <see cref="Description"/>.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Icone do circulo. Padrao: triangulo de alerta.</summary>
    [Parameter] public RvmIconName Icon { get; set; } = RvmIconName.DangerTriangle;

    /// <summary>Papel de cor do icone e do botao de confirmar. <see cref="RvmColor.Error"/> para acao destrutiva.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Accent;

    /// <summary>Texto centralizado (padrao) ou alinhado a esquerda.</summary>
    [Parameter] public RvmDialogAlign Align { get; set; } = RvmDialogAlign.Center;

    /// <summary>Texto do botao que confirma. Diga a acao ("Excluir"), nao "OK". Padrao: "Confirmar".</summary>
    [Parameter] public string ConfirmText { get; set; } = "Confirmar";

    /// <summary>Texto do botao que desiste. Padrao: "Cancelar".</summary>
    [Parameter] public string CancelText { get; set; } = "Cancelar";

    /// <summary>Confirmou. A confirmacao fecha depois de a acao terminar; enquanto isso, os botoes travam.</summary>
    [Parameter] public EventCallback OnConfirm { get; set; }

    /// <summary>Desistiu, pelo botao, pelo Esc ou pelo fundo.</summary>
    [Parameter] public EventCallback OnCancel { get; set; }

    /// <summary>Clicar no fundo desiste. Padrao: sim.</summary>
    [Parameter] public bool CloseOnBackdrop { get; set; } = true;

    /// <summary>
    /// Quem recebe o foco quando a confirmacao fecha — o botao que a abriu. Sem ele, o foco volta ao
    /// elemento que o tinha ao abrir.
    /// </summary>
    [Parameter] public ElementReference? ReturnFocusTo { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos extras, repassados ao elemento raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string IdTitulo => $"{_idBase}-titulo";

    internal string IdDescricao => $"{_idBase}-descricao";

    private string ClassesDaRaiz => ClassesCss.Juntar("rvm-confirmacao", Class, AdditionalAttributes);

    private string ClassesDoCorpo => Align == RvmDialogAlign.Left ? "rvm-corpo rvm-esquerda" : "rvm-corpo rvm-centro";

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (Open != _ultimoOpen)
        {
            _ultimoOpen = Open;
            _aberto = Open;
        }

        _papel.DescritoPor = ChildContent is not null || !string.IsNullOrWhiteSpace(Description) ? IdDescricao : null;
    }

    // Enquanto a acao do app esta em voo a confirmacao continua na tela e os botoes travam: um segundo
    // clique (ou Enter e clique juntos) executaria a acao duas vezes.
    private async Task ConfirmarAsync()
    {
        if (_confirmando || !_aberto)
        {
            return;
        }

        _confirmando = true;
        try
        {
            await OnConfirm.InvokeAsync();
            _aberto = false;
            await OpenChanged.InvokeAsync(false);
        }
        finally
        {
            _confirmando = false;
        }
    }

    // Botao, Esc e fundo passam pelo mesmo caminho: quem so escuta OnCancel nao perde a desistencia por Esc.
    private async Task CancelarAsync()
    {
        if (_confirmando || !_aberto)
        {
            return;
        }

        _aberto = false;
        await OnCancel.InvokeAsync();
        await OpenChanged.InvokeAsync(false);
    }

    private Task AoMudarAberturaAsync(bool aberto) => aberto ? Task.CompletedTask : CancelarAsync();
}
