using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Lists;

/// <summary>
/// Um item de <see cref="RvmList"/>: texto, texto secundario, icone ou avatar no inicio, acao no fim.
/// Com <see cref="Href"/> vira link; com <see cref="OnClick"/>, botao; com
/// <see cref="NestedContent"/>, abre uma sublista.
/// </summary>
public partial class RvmListItem : ComponentBase
{
    private static int _proximoId;
    private readonly string _idGerado = $"rvm-item-{Interlocked.Increment(ref _proximoId)}";
    private bool _aberto;
    private bool? _expandedRecebido;

    [CascadingParameter] private RvmList? Lista { get; set; }

    /// <summary>O texto principal.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>A linha de baixo, menor e no tom secundario.</summary>
    [Parameter] public string? SecondaryText { get; set; }

    /// <summary>Icone no inicio.</summary>
    [Parameter] public RvmIconName? Icon { get; set; }

    /// <summary>
    /// Conteudo livre no inicio (um avatar). Vence o icone. Num item clicavel ele fica DENTRO do link ou
    /// botao: nao ponha controle interativo aqui — para caixa de marcar, use o item sem clique.
    /// </summary>
    [Parameter] public RenderFragment? StartContent { get; set; }

    /// <summary>Acao na direita (um botao de favoritar). Fica fora da area clicavel do item.</summary>
    [Parameter] public RenderFragment? EndContent { get; set; }

    /// <summary>Faz do item um link.</summary>
    [Parameter] public string? Href { get; set; }

    /// <summary>Faz do item um botao. Com <see cref="Href"/> junto, o link vence e este e ignorado.</summary>
    [Parameter] public EventCallback OnClick { get; set; }

    /// <summary>Item escolhido: <c>aria-current</c> no link, <c>aria-pressed</c> no botao.</summary>
    [Parameter] public bool Selected { get; set; }

    /// <summary>Indisponivel.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Sublista que abre e fecha pelo item (a variante "Dropdown" do kit).</summary>
    [Parameter] public RenderFragment? NestedContent { get; set; }

    /// <summary>Sublista aberta. Aceita <c>@bind-Expanded</c>.</summary>
    [Parameter] public bool Expanded { get; set; }

    /// <summary>Disparado quando a sublista abre ou fecha.</summary>
    [Parameter] public EventCallback<bool> ExpandedChanged { get; set; }

    /// <summary>Atributos extras, repassados ao <c>li</c>.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal bool Aninhado => NestedContent is not null && string.IsNullOrWhiteSpace(Href);

    internal string IdAninhada => $"{_idGerado}-sublista";

    internal string ClassesDoItem
    {
        get
        {
            var proprias = "rvm-item";
            if (Lista?.Dense == true) proprias += " rvm-denso";
            if (!string.IsNullOrWhiteSpace(SecondaryText)) proprias += " rvm-duas-linhas";
            if (Selected) proprias += " rvm-selecionado";
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
    protected override void OnParametersSet()
    {
        // So segue o parametro quando ELE mudou: senao um re-render do pai fecharia a sublista que a
        // pessoa acabou de abrir sem @bind.
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
    }

    private async Task AoClicarAsync()
    {
        if (!Disabled)
        {
            await OnClick.InvokeAsync();
        }
    }
}
