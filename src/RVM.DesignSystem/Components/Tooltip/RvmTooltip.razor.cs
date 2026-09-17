using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace RVM.DesignSystem.Components.Tooltip;

/// <summary>
/// Dica curta que aparece ao passar o mouse ou focar o conteudo. Para texto que a pessoa PRECISA
/// ler para seguir, a dica e o lugar errado — use texto de apoio visivel.
/// </summary>
public partial class RvmTooltip : ComponentBase
{
    private readonly string _idGerado = $"rvm-dica-{Guid.NewGuid():N}";
    private bool _dispensada;

    /// <summary>O texto da dica.</summary>
    [Parameter, EditorRequired] public string Text { get; set; } = string.Empty;

    /// <summary>Lado em que a dica aparece. Padrao: <see cref="RvmTooltipPlacement.Top"/>.</summary>
    [Parameter] public RvmTooltipPlacement Placement { get; set; } = RvmTooltipPlacement.Top;

    /// <summary>Com a seta apontando para o conteudo (padrao) ou sem ("None" no kit).</summary>
    [Parameter] public bool Arrow { get; set; } = true;

    /// <summary>Id da dica. Sem valor, e gerado.</summary>
    [Parameter] public string? Id { get; set; }

    /// <summary>
    /// O conteudo que recebe a dica. O contexto e o id da dica: repasse-o como
    /// <c>aria-describedby="@context"</c> ao elemento focavel, para o leitor de tela ler a dica.
    /// </summary>
    [Parameter] public RenderFragment<string>? ChildContent { get; set; }

    /// <summary>Atributos extras, repassados a raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string IdDaDica => string.IsNullOrWhiteSpace(Id) ? _idGerado : Id;

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = _dispensada ? "com-dica dispensada" : "com-dica";
            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }

    internal string ClassesDaDica
        => string.Join(' ',
            "dica",
            Placement switch
            {
                RvmTooltipPlacement.Bottom => "abaixo",
                RvmTooltipPlacement.Left => "esquerda",
                RvmTooltipPlacement.Right => "direita",
                _ => "acima"
            },
            Arrow ? "com-seta" : "sem-seta");

    private void AoTeclar(KeyboardEventArgs e)
    {
        if (e.Key == "Escape")
        {
            _dispensada = true;
        }
    }

    // Saiu o mouse ou o foco: a proxima passagem mostra a dica de novo.
    private void Rearmar() => _dispensada = false;
}
