using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Tabs;

/// <summary>
/// Uma aba de <see cref="RvmTabs"/>. O titulo vira o botao da lista; o conteudo so e renderizado
/// enquanto a aba esta ativa.
/// </summary>
public sealed class RvmTab : ComponentBase, IDisposable
{
    [CascadingParameter] private RvmTabs? Abas { get; set; }

    /// <summary>O texto do botao da aba.</summary>
    [Parameter, EditorRequired] public string Title { get; set; } = string.Empty;

    /// <summary>Icone acima do titulo.</summary>
    [Parameter] public RvmIconName? Icon { get; set; }

    /// <summary>Visivel, mas nao selecionavel — o teclado tambem a pula.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>O painel da aba.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>O botao da aba, para o foco andar pelo teclado.</summary>
    internal ElementReference Botao { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (Abas is null)
        {
            throw new InvalidOperationException("RvmTab precisa estar dentro de um RvmTabs.");
        }

        Abas.Registrar(this);
    }

    private (string Titulo, RvmIconName? Icone, bool Desabilitada)? _ultimoAviso;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        // So avisa o pai quando o que aparece NA LISTA mudou. Avisar sempre fecharia um laco: o pai
        // re-renderiza, repassa o ChildContent (que o Blazor sempre considera novo), a aba recebe
        // parametros de novo e avisa de novo.
        var atual = (Title, Icon, Disabled);
        if (_ultimoAviso is { } anterior && anterior == atual)
        {
            return;
        }

        var primeiraVez = _ultimoAviso is null;
        _ultimoAviso = atual;
        if (!primeiraVez)
        {
            Abas?.AoMudarAba();
        }
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Abas is null || !Abas.EstaAtiva(this))
        {
            return;
        }

        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "role", "tabpanel");
        builder.AddAttribute(2, "id", Abas.IdDoPainel(this));
        builder.AddAttribute(3, "aria-labelledby", Abas.IdDaAba(this));
        builder.AddAttribute(4, "tabindex", "0");
        builder.AddAttribute(5, "class", "rvm-painel-aba");
        builder.AddContent(6, ChildContent);
        builder.CloseElement();
    }

    /// <inheritdoc />
    public void Dispose() => Abas?.Remover(this);
}
