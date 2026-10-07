using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Menu;

/// <summary>Uma acao de <see cref="RvmMenu"/>. Escolher fecha o menu e devolve o foco ao botao.</summary>
public partial class RvmMenuItem : ComponentBase, IDisposable
{
    private ElementReference _elemento;

    [CascadingParameter] private RvmMenu? Menu { get; set; }

    /// <summary>O que fazer ao escolher o item.</summary>
    [Parameter] public EventCallback OnClick { get; set; }

    /// <summary>Icone antes do texto.</summary>
    [Parameter] public RvmIconName? Icon { get; set; }

    /// <summary>Visivel, mas nao escolhivel — o teclado tambem o pula.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>O texto do item.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Texto da acao, quando nao ha <see cref="ChildContent"/>.</summary>
    [Parameter] public string Text { get; set; } = "";

    /// <summary>Destino. Com ele o item e um link; sem ele, um botao.</summary>
    [Parameter] public string? Href { get; set; }

    /// <summary>Acao destrutiva ("Excluir"): o item sai na cor de erro.</summary>
    [Parameter] public bool Danger { get; set; }

    /// <summary>
    /// O item LEVA o foco consigo (abre um painel, uma aba ou um dialogo e poe o foco la): ao fechar, o menu NAO
    /// devolve o foco ao botao que o abriu. Padrao: devolve, como manda o padrao de menu do WAI-ARIA.
    /// </summary>
    [Parameter] public bool MovesFocus { get; set; }

    /// <summary>Atributos extras, repassados ao botao do item.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal bool TemFoco { get; private set; }

    internal string ClassesDoItem
        => ClassesCss.Juntar(Danger ? "rvm-item rvm-perigo" : "rvm-item", Class, AdditionalAttributes);

    internal ValueTask FocusAsync() => _elemento.FocusAsync();

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (Menu is null)
        {
            throw new InvalidOperationException("RvmMenuItem precisa estar dentro de um RvmMenu.");
        }

        Menu.Registrar(this);
    }

    private async Task AoClicarAsync()
    {
        if (Disabled)
        {
            return;
        }

        await OnClick.InvokeAsync();
        if (Menu is not null)
        {
            await Menu.FecharAsync(devolverFoco: !MovesFocus);
        }
    }

    /// <inheritdoc />
    public void Dispose() => Menu?.Remover(this);
}
