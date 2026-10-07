namespace RVM.DesignSystem.Components.Upload;

/// <summary>
/// Um arquivo na lista do <see cref="RvmFileUpload"/> ou do <see cref="RvmMediaUpload"/>. A biblioteca nao envia nada:
/// o app recebe os arquivos pelo <c>OnFilesSelected</c>, envia e atualiza <see cref="Status"/> e <see cref="Progress"/>.
/// </summary>
public sealed class RvmUploadItem
{
    /// <summary>Nome do arquivo, com a extensao ("contrato.pdf"). A extensao escolhe o icone.</summary>
    public required string Name { get; init; }

    /// <summary>Tamanho em bytes, mostrado em PT-BR ("1,2 MB").</summary>
    public long Size { get; init; }

    /// <summary>Em que ponto do envio o arquivo esta.</summary>
    public RvmUploadStatus Status { get; set; }

    /// <summary>Porcentagem enviada, de 0 a 100 (fora disso, a exibicao limita). Vale no <see cref="RvmUploadStatus.Uploading"/>.</summary>
    public int Progress { get; set; }

    /// <summary>Endereco da miniatura (uma URL ou um <c>data:</c>), usada pelo <see cref="RvmMediaUpload"/>.</summary>
    public string? PreviewUrl { get; set; }
}
