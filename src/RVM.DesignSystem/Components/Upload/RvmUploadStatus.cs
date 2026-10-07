namespace RVM.DesignSystem.Components.Upload;

/// <summary>Em que ponto do envio um arquivo esta. Quem envia e o app; o componente so mostra.</summary>
public enum RvmUploadStatus
{
    /// <summary>Escolhido, ainda sem envio: o cartao mostra so nome e tamanho.</summary>
    None,

    /// <summary>Enviando: barra de progresso com o <see cref="RvmUploadItem.Progress"/>.</summary>
    Uploading,

    /// <summary>Enviado: sinal de concluido.</summary>
    Uploaded,

    /// <summary>O envio falhou: aviso de erro (e, no <see cref="RvmMediaUpload"/>, "Tentar de novo").</summary>
    Failed
}
