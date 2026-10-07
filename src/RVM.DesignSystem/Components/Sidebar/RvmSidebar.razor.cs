using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.AppShell;

namespace RVM.DesignSystem.Components.Sidebar;

/// <summary>
/// O menu lateral: marca, area de topo, navegacao (<c>RvmNavSection</c>, <c>RvmNavItem</c>, <c>RvmNavGroup</c>) e
/// rodape. O <c>RvmAppShell</c> monta um destes na coluna dele; use sozinho quando a casca for outra.
/// </summary>
public partial class RvmSidebar : ComponentBase
{
    private ElementReference _navegacao;

    /// <summary>As secoes e os itens do menu.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>A marca no alto do menu aberto.</summary>
    [Parameter] public RenderFragment? Logo { get; set; }

    /// <summary>A marca do menu recolhido (so o simbolo). Sem ela, a de <see cref="Logo"/>.</summary>
    [Parameter] public RenderFragment? LogoCollapsed { get; set; }

    /// <summary>Area abaixo da marca (troca de equipe, busca).</summary>
    [Parameter] public RenderFragment? Header { get; set; }

    /// <summary>Rodape do menu (perfil, avisos).</summary>
    [Parameter] public RenderFragment? Footer { get; set; }

    /// <summary>Fundo do corpo (padrao) ou na cor primaria.</summary>
    [Parameter] public RvmSidebarVariant Variant { get; set; } = RvmSidebarVariant.White;

    /// <summary>Recolhido aos icones (68 px). Aceita <c>@bind-Collapsed</c>.</summary>
    [Parameter] public bool Collapsed { get; set; }

    /// <summary>Avisa quando o menu recolhe ou abre.</summary>
    [Parameter] public EventCallback<bool> CollapsedChanged { get; set; }

    /// <summary>Mostra o botao de recolher e abrir no pe do menu.</summary>
    [Parameter] public bool Collapsible { get; set; }

    /// <summary>Barra de 4 px na borda do item ativo, alem do fundo.</summary>
    [Parameter] public bool ActiveLine { get; set; }

    /// <summary>Nome da navegacao para o leitor de tela.</summary>
    [Parameter] public string Label { get; set; } = "Menu principal";

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos repassados ao elemento raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    // Dentro de um RvmAppShell, o RvmSidebar que o consumidor passa em Sidebar segue o recolhido da casca mesmo sem
    // @bind-Collapsed: a coluna encolhe para 68 px e os itens precisam saber (achado do review da onda 1).
    [CascadingParameter] private RvmAppShell? Casca { get; set; }

    private bool _recolhido;
    private bool? _collapsedRecebido;

    /// <summary>O estado que os itens leem: o proprio, ou o da casca em volta.</summary>
    internal bool Recolhido => _recolhido || Casca?.Collapsed == true;

    // So segue o parametro quando ELE mudou: um re-render do pai nao pode desfazer o que a pessoa fez sem @bind
    // (achado do review da onda 1, DSGN-017 — o mesmo do RvmAccordionPanel).
    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (_collapsedRecebido != Collapsed)
        {
            _recolhido = Collapsed;
            _collapsedRecebido = Collapsed;
        }
    }

    /// <summary>A lista que rola: o <c>RvmAppShell</c> traz o item atual para a vista dentro dela.</summary>
    internal ElementReference Navegacao => _navegacao;

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = "rvm-barra-lateral";
            if (Variant == RvmSidebarVariant.Colored) proprias += " rvm-colorida";
            if (Recolhido) proprias += " rvm-recolhida";
            if (ActiveLine) proprias += " rvm-linha-ativa";
            return ClassesCss.Juntar(proprias, Class, AdditionalAttributes);
        }
    }

    internal async Task DefinirRecolhidoAsync(bool recolhido)
    {
        if (_recolhido == recolhido)
        {
            return;
        }

        _recolhido = recolhido;
        await CollapsedChanged.InvokeAsync(recolhido);
    }
}
