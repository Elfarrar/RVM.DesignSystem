using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Accordion;

/// <summary>
/// Grupo de paineis que abrem e fecham. Os paineis sao <see cref="RvmAccordionPanel"/> filhos.
/// </summary>
public partial class RvmAccordion : ComponentBase
{
    private readonly List<RvmAccordionPanel> _paineis = [];

    /// <summary>So um painel aberto por vez: abrir um fecha os outros.</summary>
    [Parameter] public bool Exclusive { get; set; }

    /// <summary>Estilo. Padrao: <see cref="RvmAccordionVariant.Standard"/>.</summary>
    [Parameter] public RvmAccordionVariant Variant { get; set; } = RvmAccordionVariant.Standard;

    /// <summary>Nivel dos cabecalhos dos paineis (2 a 6). Padrao: 3.</summary>
    [Parameter] public int HeadingLevel { get; set; } = 3;

    /// <summary>Os paineis.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Atributos extras, repassados a raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = Variant == RvmAccordionVariant.Filled ? "acordeao preenchido" : "acordeao padrao";
            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }

    internal void Registrar(RvmAccordionPanel painel) => _paineis.Add(painel);

    internal void Remover(RvmAccordionPanel painel) => _paineis.Remove(painel);

    /// <summary>No modo exclusivo, fecha todos os paineis abertos menos o que acabou de abrir.</summary>
    internal async Task AoAbrirAsync(RvmAccordionPanel aberto)
    {
        if (!Exclusive)
        {
            return;
        }

        foreach (var painel in _paineis.Where(p => !ReferenceEquals(p, aberto) && p.Aberto).ToList())
        {
            await painel.FecharAsync();
        }
    }
}
