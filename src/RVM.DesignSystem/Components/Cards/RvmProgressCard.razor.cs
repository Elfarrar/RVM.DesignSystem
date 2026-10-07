using System.Globalization;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.IconBadge;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Cards;

/// <summary>
/// Card de progresso de painel: selo, titulo, descricao e uma barra com o valor e a porcentagem — meta de vendas,
/// orcamento de safra (contrato com o RVM.UI, DSGN-017).
/// </summary>
public partial class RvmProgressCard : ComponentBase
{
    /// <summary>Icone do selo.</summary>
    [Parameter, EditorRequired] public RvmIconName Icon { get; set; }

    /// <summary>Fundo do selo: cinza neutro (padrao, o "Grey BG" do kit), suave ou cheio.</summary>
    [Parameter] public RvmIconBadgeVariant IconVariant { get; set; } = RvmIconBadgeVariant.Neutral;

    /// <summary>Papel de cor do selo e da barra. Padrao: Accent.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Accent;

    /// <summary>Titulo do indicador. Sai como cabecalho <c>h3</c>.</summary>
    [Parameter, EditorRequired] public string Title { get; set; } = "";

    /// <summary>Descricao opcional abaixo do titulo.</summary>
    [Parameter] public string? Description { get; set; }

    /// <summary>Valor a esquerda, acima da barra, ja formatado por quem chama ("R$ 40.000").</summary>
    [Parameter, EditorRequired] public string Value { get; set; } = "";

    /// <summary>Progresso de 0 a 100. Fora disso e limitado.</summary>
    [Parameter] public double Percent { get; set; }

    /// <summary>Itens do menu de tres pontos (<c>RvmMenuItem</c>). Sem eles, sem menu.</summary>
    [Parameter] public RenderFragment? MenuItems { get; set; }

    /// <summary>Nome acessivel do menu. Padrao: "Mais acoes".</summary>
    [Parameter] public string MenuLabel { get; set; } = "Mais acoes";

    /// <summary>Conteudo livre abaixo da barra.</summary>
    [Parameter] public RenderFragment? Footer { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos repassados ao elemento raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    // O mesmo arredondamento do RvmProgressBar, para o texto visivel bater com o que o leitor de tela ouve.
    internal string Porcentagem
        => $"{Math.Round(double.IsNaN(Percent) ? 0 : Math.Clamp(Percent, 0, 100)).ToString(CultureInfo.InvariantCulture)}%";

    internal string NomeDaBarra => string.IsNullOrWhiteSpace(Value) ? Title : $"{Title}: {Value}";

    internal string CssClass => ClassesCss.Juntar("rvm-progress-card", Class, AdditionalAttributes);
}
