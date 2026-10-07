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
    [Parameter] public string Title { get; set; } = string.Empty;

    /// <summary>Icone acima do titulo.</summary>
    [Parameter] public RvmIconName? Icon { get; set; }

    /// <summary>Visivel, mas nao selecionavel — o teclado tambem a pula.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>O painel da aba.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Texto da aba. Vence o <see cref="Title"/> quando os dois vem.</summary>
    [Parameter] public string Text { get; set; } = "";

    /// <summary>Contador a direita do texto ("Pendentes 3").</summary>
    [Parameter] public int? Count { get; set; }

    /// <summary>Valor que identifica a aba (o <c>Value</c> do <see cref="RvmTabs"/>). Sem ele, vale a posicao.</summary>
    [Parameter] public string Value { get; set; } = "";

    /// <summary>Id do botao da aba. Sem ele, um id unico e gerado.</summary>
    [Parameter] public string? Id { get; set; }

    /// <summary>
    /// Id do painel que a aba controla, quando o conteudo fica FORA do <see cref="RvmTabs"/>. Com ele e sem
    /// <see cref="ChildContent"/>, a aba nao desenha painel proprio.
    /// </summary>
    [Parameter] public string? PanelId { get; set; }

    internal string TituloEfetivo => string.IsNullOrWhiteSpace(Text) ? Title : Text;

    /// <summary>Classe CSS extra no botao da aba.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos repassados ao botao da aba.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

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

    private (string Titulo, RvmIconName? Icone, bool Desabilitada, string? Classe, int? Contador, string Valor)? _ultimoAviso;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        // So avisa o pai quando o que aparece NA LISTA mudou. Avisar sempre fecharia um laco: o pai
        // re-renderiza, repassa o ChildContent (que o Blazor sempre considera novo), a aba recebe
        // parametros de novo e avisa de novo.
        var atual = (TituloEfetivo, Icon, Disabled, Class, Count, Value);
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
        if (Abas is null || !Abas.EstaAtiva(this) || (PanelId is not null && ChildContent is null))
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
