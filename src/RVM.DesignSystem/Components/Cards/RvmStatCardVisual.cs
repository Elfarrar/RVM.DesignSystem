namespace RVM.DesignSystem.Components.Cards;

/// <summary>
/// O que o <see cref="RvmStatCard"/> desenha abaixo do numero (contrato com o RVM.UI, DSGN-017). Para grafico, use o
/// encaixe <c>Chart</c>: ele vence este parametro.
/// </summary>
public enum RvmStatCardVisual
{
    /// <summary>Nada.</summary>
    None,

    /// <summary>Barra de progresso com <c>VisualValue</c> de 0 a 100.</summary>
    Progress,

    /// <summary>A imagem de <c>ImageUrl</c>, decorativa.</summary>
    Image
}
