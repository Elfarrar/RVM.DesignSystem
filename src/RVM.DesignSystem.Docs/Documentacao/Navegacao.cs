namespace RVM.DesignSystem.Docs.Documentacao;

/// <summary>Um item de menu.</summary>
/// <param name="Texto">O rótulo exibido.</param>
/// <param name="Rota">A rota, sem barra inicial.</param>
public sealed record ItemDeMenu(string Texto, string Rota);

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
/// arquivos. Segue o desenho do `07` § Estrutura de navegação; os grupos que ainda não têm
/// página (Padrões, ondas 2 a 4) entram quando os componentes deles existirem.
/// </remarks>
public static class Navegacao
{
    /// <summary>Os grupos exibidos na navegação lateral.</summary>
    public static IReadOnlyList<GrupoDeMenu> Grupos { get; } =
    [
        new("Começar",
        [
            new("Início", ""),
            new("Instalação", "instalacao"),
        ]),

        new("Fundamentos",
        [
            new("Cor", "fundamentos/cor"),
            new("Ícones", "fundamentos/icones"),
            new("Espaçamento", "fundamentos/espacamento"),
        ]),

        new("Componentes",
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

        new("Sobre",
        [
            new("Acessibilidade", "acessibilidade"),
            new("Changelog", "changelog"),
        ]),
    ];
}
