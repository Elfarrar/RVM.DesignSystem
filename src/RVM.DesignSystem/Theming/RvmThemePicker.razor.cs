using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components;

namespace RVM.DesignSystem.Theming;

/// <summary>
/// Preferencias de aparencia do usuario: claro, escuro ou automatico, a paleta e, com
/// <see cref="ShowAccessibility"/>, tamanho da fonte, contraste alto e menos movimento. Escreve no
/// <see cref="RvmThemeState"/>; quem aplica e o <see cref="RvmThemeProvider"/> com <c>UserTheme</c>. Contrato com o
/// RVM.UI (DSGN-017).
/// </summary>
public partial class RvmThemePicker : ComponentBase, IDisposable
{
    [Inject] private RvmThemeState Estado { get; set; } = default!;

    /// <summary>Provider em volta: o modo dele pinta as amostras das paletas.</summary>
    [CascadingParameter] private RvmThemeProvider? Provider { get; set; }

    /// <summary>Paletas oferecidas. Padrao: todas (<see cref="RvmPalettes.All"/>).</summary>
    [Parameter] public IReadOnlyList<RvmPalette> Palettes { get; set; } = RvmPalettes.All;

    /// <summary>Mostra tamanho da fonte, contraste alto e menos movimento. Padrao: sim.</summary>
    [Parameter] public bool ShowAccessibility { get; set; } = true;

    /// <summary>Chamado depois de cada troca, com a escolha nova.</summary>
    [Parameter] public EventCallback<RvmThemeSettings> OnChange { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos HTML extras, repassados ao elemento raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private RvmThemeSettings _atual = RvmThemeSettings.Default;

    // Um name por grupo e por picker: dois grupos com o mesmo name viram UM grupo nativo, e marcar a paleta
    // desmarcaria a aparencia.
    private readonly string _prefixo = GeradorDeIds.Novo("rvm-tema");

    /// <summary>O tema das amostras: o do provider em volta, ou o escolhido aqui quando nao ha provider.</summary>
    private string ModoDasAmostras => Provider?.ThemeAttribute ?? _atual.ModeAttribute;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await Estado.EnsureLoadedAsync();
        _atual = Estado.Current;
        Estado.Changed += AoMudar;
        if (Provider is not null)
        {
            Provider.ModoMudou += AoMudar;
        }
    }

    private async Task TrocarAsync(RvmThemeSettings novo)
    {
        _atual = novo;
        await Estado.SetAsync(novo);
        await OnChange.InvokeAsync(novo);
    }

    private void AoMudar() => _ = InvokeAsync(() =>
    {
        _atual = Estado.Current;
        StateHasChanged();
    });

    /// <inheritdoc />
    public void Dispose()
    {
        Estado.Changed -= AoMudar;
        if (Provider is not null)
        {
            Provider.ModoMudou -= AoMudar;
        }

        GC.SuppressFinalize(this);
    }
}
