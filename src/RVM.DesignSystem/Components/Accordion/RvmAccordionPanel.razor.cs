using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Accordion;

/// <summary>Um painel de <see cref="RvmAccordion"/>: um titulo que abre e fecha o conteudo.</summary>
public partial class RvmAccordionPanel : ComponentBase, IDisposable
{
    private static int _proximoId;
    private readonly string _idBase = $"rvm-painel-{Interlocked.Increment(ref _proximoId)}";
    private bool _aberto;
    private bool? _expandedRecebido;

    [CascadingParameter] private RvmAccordion? Acordeao { get; set; }

    /// <summary>O titulo do painel.</summary>
    [Parameter, EditorRequired] public string Title { get; set; } = string.Empty;

    /// <summary>Um complemento ao lado do titulo, no tom secundario ("Secondary heading" no kit).</summary>
    [Parameter] public string? SecondaryTitle { get; set; }

    /// <summary>Icone antes do titulo.</summary>
    [Parameter] public RvmIconName? Icon { get; set; }

    /// <summary>Aberto. Aceita <c>@bind-Expanded</c>.</summary>
    [Parameter] public bool Expanded { get; set; }

    /// <summary>Disparado quando abre ou fecha.</summary>
    [Parameter] public EventCallback<bool> ExpandedChanged { get; set; }

    /// <summary>Indisponivel: nao abre nem fecha.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>O conteudo.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Atributos extras, repassados a raiz do painel.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal bool Aberto => _aberto;

    internal string IdBotao => $"{_idBase}-titulo";

    internal string IdRegiao => $"{_idBase}-conteudo";

    internal int NivelDoCabecalho => Math.Clamp(Acordeao?.HeadingLevel ?? 3, 2, 6);

    internal RvmIconName IconeDeEstado
        => Acordeao?.Variant == RvmAccordionVariant.Filled
            ? (_aberto ? RvmIconName.Minus : RvmIconName.Plus)
            : (_aberto ? RvmIconName.ChevronUp : RvmIconName.ChevronDown);

    internal string ClassesDoPainel
    {
        get
        {
            var proprias = _aberto ? "rvm-painel rvm-aberto" : "rvm-painel";
            if (Disabled) proprias += " rvm-desabilitado";

            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }

    /// <inheritdoc />
    protected override void OnInitialized() => Acordeao?.Registrar(this);

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        // So segue o parametro quando ELE mudou: um re-render do pai nao pode fechar o que a pessoa
        // abriu sem @bind.
        if (_expandedRecebido != Expanded)
        {
            _aberto = Expanded;
            _expandedRecebido = Expanded;
        }
    }

    private async Task AlternarAsync()
    {
        if (Disabled)
        {
            return;
        }

        _aberto = !_aberto;
        await ExpandedChanged.InvokeAsync(_aberto);
        if (_aberto && Acordeao is not null)
        {
            await Acordeao.AoAbrirAsync(this);
        }
    }

    internal async Task FecharAsync()
    {
        _aberto = false;
        await ExpandedChanged.InvokeAsync(false);
        StateHasChanged();
    }

    /// <inheritdoc />
    public void Dispose() => Acordeao?.Remover(this);
}
