namespace RVM.DesignSystem.Components.FileCard;

/// <summary>Forma do <see cref="RvmFileCard"/> (contrato com o RVM.UI, DSGN-017).</summary>
public enum RvmFileCardLayout
{
    /// <summary>Linha: icone, nome e menu lado a lado. Para lista de arquivos (o padrao).</summary>
    Horizontal,

    /// <summary>Coluna: icone grande em cima, nome embaixo; selecao e menu no topo. Para grade de arquivos.</summary>
    Vertical
}
