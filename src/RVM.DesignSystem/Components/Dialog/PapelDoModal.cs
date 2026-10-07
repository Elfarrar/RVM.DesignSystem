namespace RVM.DesignSystem.Components.Dialog;

/// <summary>
/// O que um componente montado sobre o <see cref="RvmModal"/> troca na caixa dele: o papel ARIA, quem
/// nomeia, quem descreve e a largura. Chega por cascata interna, sem parametro publico novo (o contrato
/// com o RVM.UI fixa a API). So o primeiro RvmModal abaixo de quem cascateou fica com o papel: um modal
/// declarado dentro do corpo nao herda o <c>alertdialog</c> de fora.
/// </summary>
internal sealed class PapelDoModal
{
    /// <summary><c>alertdialog</c> em vez de <c>dialog</c>.</summary>
    public bool Alerta { get; init; }

    /// <summary>Id do elemento que nomeia a caixa.</summary>
    public string? NomeadoPor { get; init; }

    /// <summary>Id do elemento que descreve a caixa.</summary>
    public string? DescritoPor { get; set; }

    /// <summary>Caixa estreita (400 px), a da confirmacao.</summary>
    public bool Estreito { get; init; }

    /// <summary>O RvmModal que ficou com o papel.</summary>
    public RvmModal? Dono { get; set; }
}
