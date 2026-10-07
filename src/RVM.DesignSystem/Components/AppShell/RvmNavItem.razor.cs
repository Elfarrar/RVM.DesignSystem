using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Sidebar;
using Microsoft.AspNetCore.Components.Routing;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.AppShell;

/// <summary>
/// Um link do menu lateral. Marca a pagina atual com <c>aria-current="page"</c> e o destaque do kit.
/// </summary>
public partial class RvmNavItem : ComponentBase, IDisposable
{
    [Inject] private NavigationManager Navegacao { get; set; } = default!;

    [CascadingParameter] private RvmSidebar? Casca { get; set; }

    [CascadingParameter(Name = RvmNavGroup.NomeDaCascata)] private RvmNavGroup? Grupo { get; set; }

    /// <summary>Endereco do link (<c>""</c> e a raiz do app). Sem ele (<c>null</c>), o item e um botao que avisa por <see cref="OnClick"/>.</summary>
    [Parameter] public string? Href { get; set; }

    /// <summary>Clique do item sem <see cref="Href"/>.</summary>
    [Parameter] public EventCallback OnClick { get; set; }

    /// <summary>Ativo, no item sem <see cref="Href"/> (o link descobre sozinho pela rota).</summary>
    [Parameter] public bool Active { get; set; }

    /// <summary>Imagem redonda de 20 px no lugar do icone (decorativa: o texto ja nomeia o item).</summary>
    [Parameter] public string? ImageUrl { get; set; }

    /// <summary>Texto do item. Com o menu recolhido, continua sendo o nome do link para o leitor de tela.</summary>
    [Parameter, EditorRequired] public string Text { get; set; } = "";

    /// <summary>Icone. Sem icone, o item leva o circulo pequeno dos subitens do kit.</summary>
    [Parameter] public RvmIconName? Icon { get; set; }

    /// <summary>Selo ao lado do texto ("Novo", "3").</summary>
    [Parameter] public string? Badge { get; set; }

    /// <summary>Como decidir que e a pagina atual. Padrao: <see cref="RvmNavMatch.Prefix"/>.</summary>
    [Parameter] public RvmNavMatch Match { get; set; } = RvmNavMatch.Prefix;

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos extras, repassados ao link.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal bool Recolhido => Casca?.Recolhido == true && Grupo is null;

    internal bool Ativo
    {
        get
        {
            // Href vazio e link de verdade (a raiz do app); so sem Href o item e botao.
            if (Href is null)
            {
                return Active;
            }

            var destino = Sem(Navegacao.ToAbsoluteUri(Href).AbsoluteUri);
            var atual = Sem(Navegacao.Uri);
            if (Match == RvmNavMatch.All)
            {
                return string.Equals(destino, atual, StringComparison.OrdinalIgnoreCase);
            }

            // Mesmo criterio do NavLink: /talhoes vale em /talhoes e /talhoes/12, mas nao em /talhoes-velhos.
            return atual.StartsWith(destino, StringComparison.OrdinalIgnoreCase)
                   && (atual.Length == destino.Length || destino.EndsWith('/') || atual[destino.Length] == '/');
        }
    }

    // Sem consulta, sem ancora e sem a barra final: "/talhoes/?x=1#y" e "/talhoes" sao a mesma pagina.
    private static string Sem(string endereco)
    {
        var corte = endereco.IndexOfAny(['?', '#']);
        var limpo = corte < 0 ? endereco : endereco[..corte];
        return limpo.Length > 1 && limpo.EndsWith('/') && !limpo.EndsWith("://", StringComparison.Ordinal) ? limpo[..^1] : limpo;
    }

    internal string ClassesDoLink
    {
        get
        {
            var proprias = "rvm-nav-link";
            if (Ativo) proprias += " rvm-ativo";
            if (Recolhido) proprias += " rvm-recolhido";
            if (Icon is null) proprias += " rvm-subitem";

            return ClassesCss.Juntar(proprias, Class, AdditionalAttributes);
        }
    }

    /// <inheritdoc />
    protected override void OnInitialized() => Navegacao.LocationChanged += AoNavegar;

    private void AoNavegar(object? sender, LocationChangedEventArgs e) => _ = InvokeAsync(StateHasChanged);

    /// <inheritdoc />
    public void Dispose()
    {
        Navegacao.LocationChanged -= AoNavegar;
        GC.SuppressFinalize(this);
    }
}
