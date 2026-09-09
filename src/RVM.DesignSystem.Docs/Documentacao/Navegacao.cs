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
/// <b>Todo item tem ícone desde a DSGN-026.</b> Antes só "Início" tinha, e o menu era uma
/// coluna de texto — a maior superfície da tela sem nenhuma forma para o olho ancorar. Ícone
/// aqui não é enfeite: é o que deixa achar "DataGrid" na lista pelo formato, sem ler os 45
/// rótulos. Alguns são metáfora frouxa (não existe desenho óbvio para "Skeleton"); mesmo
/// assim, forma distinta ajuda a varrer, e é por isso que nenhum item ficou sem.
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
            new("Instalação", "instalacao", "download-simple"),
        ]),

        new("Fundamentos",
        [
            new("Cor", "fundamentos/cor", "palette"),
            new("Criar sua paleta", "fundamentos/paleta", "sliders-horizontal"),
            new("Ícones", "fundamentos/icones", "dots-nine"),
            new("Cores do Bootstrap", "fundamentos/bootstrap", "browser"),
            new("Espaçamento", "fundamentos/espacamento", "arrows-out-line-horizontal"),
            new("Densidade", "fundamentos/densidade", "rows"),
        ]),

        new("Básicos",
        [
            new("Button", "componentes/button", "cursor-click"),
            new("IconButton", "componentes/icon-button", "selection"),
            new("TextField", "componentes/text-field", "textbox"),
            new("TextArea", "componentes/text-area", "text-align-left"),
            new("Select", "componentes/select", "caret-down"),
            new("Checkbox", "componentes/checkbox", "check-square"),
            new("RadioGroup", "componentes/radio-group", "radio-button"),
            new("Switch", "componentes/switch", "toggle-right"),
            new("FormField", "componentes/form-field", "note"),
        ]),

        new("Layout",
        [
            new("AppShell", "componentes/app-shell", "browser"),
            new("Sidebar", "componentes/sidebar", "sidebar-simple"),
            new("Topbar", "componentes/topbar", "rectangle"),
            new("NavItem", "componentes/nav-item", "signpost"),
            new("Tabs", "componentes/tabs", "tabs"),
            new("Breadcrumb", "componentes/breadcrumb", "flow-arrow"),
            new("Card", "componentes/card", "cards"),
            new("Stack", "componentes/stack", "stack"),
            new("Grid", "componentes/grid", "squares-four"),
            new("Divider", "componentes/divider", "minus"),
            new("Chip", "componentes/chip", "tag"),
            new("Avatar", "componentes/avatar", "user-circle"),
        ]),

        new("Feedback",
        [
            new("Dialog", "componentes/dialog", "frame-corners"),
            new("Toast", "componentes/toast", "bell"),
            new("Alert", "componentes/alert", "warning"),
            new("EmptyState", "componentes/empty-state", "tray"),
            new("Spinner", "componentes/spinner", "spinner-gap"),
            new("Progress", "componentes/progress", "hourglass"),
            new("Skeleton", "componentes/skeleton", "placeholder"),
            new("Tooltip", "componentes/tooltip", "info"),
        ]),

        new("Dados",
        [
            new("DataGrid", "componentes/data-grid", "table"),
            new("Pagination", "componentes/pagination", "caret-double-right"),
            new("FilterBar", "componentes/filter-bar", "funnel"),
            new("DatePicker", "componentes/date-picker", "calendar-blank"),
            new("Autocomplete", "componentes/autocomplete", "magnifying-glass"),
        ]),

        new("Padrões",
        [
            new("Padrões", "padroes", "book-open"),
            new("Dashboard", "padroes/dashboard", "chart-bar"),
            new("Listagem", "padroes/listagem", "list-checks"),
        ]),

        new("Sobre",
        [
            new("Acessibilidade", "acessibilidade", "person-arms-spread"),
            new("Changelog", "changelog", "clock-counter-clockwise"),
        ]),
    ];
}
