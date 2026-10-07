using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Cards;

/// <summary>
/// Cartao de pagamento (debito ou credito) na cor do papel: saldo, numero mascarado e validade, com uma faixa de
/// acoes opcional (contrato com o RVM.UI, DSGN-017). O texto vai no token de contraste do papel, que passa AA nos
/// dois temas.
/// </summary>
public partial class RvmPaymentCard : ComponentBase
{
    /// <summary>Papel de cor do cartao. Padrao: Accent.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Accent;

    /// <summary>Rotulo do saldo. Padrao: "Saldo".</summary>
    [Parameter] public string BalanceLabel { get; set; } = "Saldo";

    /// <summary>Saldo, ja formatado por quem chama ("R$ 12.430,00").</summary>
    [Parameter, EditorRequired] public string Balance { get; set; } = "";

    /// <summary>
    /// Numero ja mascarado ("**** **** **** 9090"). Na tela sai como veio; o leitor de tela ouve so os ultimos digitos
    /// ("Cartao final 9090").
    /// </summary>
    [Parameter, EditorRequired] public string CardNumber { get; set; } = "";

    /// <summary>Validade ("07/28"). O leitor de tela ouve "Validade" antes.</summary>
    [Parameter, EditorRequired] public string ExpiresAt { get; set; } = "";

    /// <summary>Marca da bandeira, decorativa. Sem ela, sem marca.</summary>
    [Parameter] public RenderFragment? NetworkMark { get; set; }

    /// <summary>Botoes na faixa abaixo do cartao. Sem eles, so o cartao.</summary>
    [Parameter] public RenderFragment? Actions { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos repassados ao elemento raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>"Cartao final 9090": o ultimo grupo de digitos do numero. Sem digito nenhum, o numero como veio.</summary>
    internal string NumeroParaOLeitor
    {
        get
        {
            var final = Regex.Match(CardNumber ?? string.Empty, @"(\d+)\D*$");
            return final.Success ? $"Cartao final {final.Groups[1].Value}" : $"Cartao {CardNumber}";
        }
    }

    internal string ClasseDaFace => $"rvm-face rvm-cheio {PapelCss.Classe(Color)}";

    internal string CssClass => ClassesCss.Juntar(
        Actions is null ? "rvm-payment-card" : "rvm-payment-card rvm-com-acoes", Class, AdditionalAttributes);
}
