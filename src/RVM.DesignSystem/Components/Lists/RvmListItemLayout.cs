namespace RVM.DesignSystem.Components.Lists;

/// <summary>Forma do <see cref="RvmListItem"/> (contrato com o RVM.UI, DSGN-017). Muda a hierarquia de cor do texto.</summary>
public enum RvmListItemLayout
{
    /// <summary>
    /// O titulo identifica o item (tinta principal) e o subtitulo apoia; a direita ficam o valor e a variacao. E o
    /// item das listas de valores — o padrao.
    /// </summary>
    TwoColumn,

    /// <summary>O titulo e um rotulo (cor de apoio) e o subtitulo abaixo e o dado (tinta principal). E o item das fichas.</summary>
    OneColumn
}
