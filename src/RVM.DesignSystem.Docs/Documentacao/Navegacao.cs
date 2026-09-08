namespace RVM.DesignSystem.Docs.Documentacao;

/// <summary>Um item de menu.</summary>
/// <param name="Texto">O rótulo exibido.</param>
/// <param name="Rota">A rota, sem barra inicial.</param>
/// <param name="Icone">Ícone opcional, do conjunto Phosphor.</param>
public sealed record ItemDeMenu(string Texto, string Rota, string? Icone = null);

/// <summary>Um grupo de itens.</summary>
/// <param name="Titulo">O título da seção.</param>
/// <param name="Itens">Os itens do grupo.</param>
public sealed record GrupoDeMenu(string Titulo, IReadOnlyList<ItemDeMenu> Itens);

/// <summary>
/// A estrutura de navegação do site.
/// </summary>
/// <remarks>
/// Declarada num lugar só, e não espalhada em <c>NavLink</c> pelo layout: assim a página nova
/// aparece no menu por acrescentar uma linha aqui, e não por alguém lembrar de editar dois
/// arquivos. Segue o desenho do `07` § Estrutura de navegação.
///
/// <para>
/// Os componentes estão em <b>dois grupos</b> desde a onda 2. Vinte e um itens numa lista só
/// obrigam a rolar a barra lateral para achar qualquer coisa — e a separação por onda é a que
/// as pessoas já usam para falar da biblioteca.
/// </para>
/// </remarks>
public static class Navegacao
{
    /// <summary>Os grupos exibidos na navegação lateral.</summary>
    public static IReadOnlyList<GrupoDeMenu> Grupos { get; } =
    [
        new("Começar",
        [
            new("Início", "", "house"),
            new("Instalação", "instalacao"),
        ]),

        new("Fundamentos",
        [
            new("Cor", "fundamentos/cor"),
            new("Ícones", "fundamentos/icones"),
            new("Espaçamento", "fundamentos/espacamento"),
            new("Densidade", "fundamentos/densidade"),
        ]),

        new("Básicos",
        [
            new("Button", "componentes/button"),
            new("IconButton", "componentes/icon-button"),
            new("TextField", "componentes/text-field"),
            new("TextArea", "componentes/text-area"),
            new("Select", "componentes/select"),
            new("Checkbox", "componentes/checkbox"),
            new("RadioGroup", "componentes/radio-group"),
            new("Switch", "componentes/switch"),
            new("FormField", "componentes/form-field"),
        ]),

        new("Layout",
        [
            new("AppShell", "componentes/app-shell"),
            new("Sidebar", "componentes/sidebar"),
            new("Topbar", "componentes/topbar"),
            new("NavItem", "componentes/nav-item"),
            new("Tabs", "componentes/tabs"),
            new("Breadcrumb", "componentes/breadcrumb"),
            new("Card", "componentes/card"),
            new("Stack", "componentes/stack"),
            new("Grid", "componentes/grid"),
            new("Divider", "componentes/divider"),
            new("Chip", "componentes/chip"),
            new("Avatar", "componentes/avatar"),
        ]),

        new("Feedback",
        [
            new("Dialog", "componentes/dialog"),
            new("Toast", "componentes/toast"),
            new("Alert", "componentes/alert"),
            new("EmptyState", "componentes/empty-state"),
            new("Spinner", "componentes/spinner"),
            new("Progress", "componentes/progress"),
            new("Skeleton", "componentes/skeleton"),
            new("Tooltip", "componentes/tooltip"),
        ]),

        new("Padrões",
        [
            new("Padrões", "padroes"),
        ]),

        new("Sobre",
        [
            new("Acessibilidade", "acessibilidade"),
            new("Changelog", "changelog"),
        ]),
    ];
}
