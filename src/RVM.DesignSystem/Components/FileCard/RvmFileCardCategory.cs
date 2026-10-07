namespace RVM.DesignSystem.Components.FileCard;

/// <summary>O que o <see cref="RvmFileCard"/> representa: decide o icone (contrato com o RVM.UI, DSGN-017).</summary>
public enum RvmFileCardCategory
{
    /// <summary>Pasta: o desenho de pasta do <c>RvmFileIcon</c>.</summary>
    Folder,

    /// <summary>Arquivo: o icone do tipo, lido da extensao do nome.</summary>
    File,

    /// <summary>Imagem: o icone de galeria.</summary>
    Image
}
