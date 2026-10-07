using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.FileIcon;

/// <summary>
/// Icone de tipo de arquivo: uma folha com a etiqueta da extensao na cor do tipo, ou a pasta. Colorido de proposito —
/// a cor ajuda a achar o PDF numa lista, mas a extensao escrita e quem diz o tipo.
/// </summary>
public partial class RvmFileIcon : ComponentBase
{
    /// <summary>Tipo de arquivo.</summary>
    [Parameter, EditorRequired] public RvmFileIconName Name { get; set; }

    /// <summary>Lado em pixels. Padrao: 48.</summary>
    [Parameter] public int Size { get; set; } = 48;

    /// <summary>Nome acessivel ("Arquivo PDF"). Sem ele o icone e decorativo.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos repassados ao elemento raiz (o <c>&lt;svg&gt;</c>).</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string Extensao => Name.ToString().ToUpperInvariant();

    internal string ClassesDaRaiz
        => ClassesCss.Juntar($"rvm-icone-arquivo {PapelCss.Classe(PapelDoTipo(Name))}", Class, AdditionalAttributes);

    /// <summary>
    /// A cor de cada tipo, por familia: documento pela cor que o app dele costuma ter (PDF vermelho, planilha verde,
    /// texto azul, apresentacao laranja), imagem em info, video em erro, audio em sucesso, compactado em neutro.
    /// </summary>
    internal static RvmColor PapelDoTipo(RvmFileIconName tipo) => tipo switch
    {
        RvmFileIconName.Pdf or RvmFileIconName.Avi or RvmFileIconName.Mkv or RvmFileIconName.Mov or RvmFileIconName.Mp4
            => RvmColor.Error,
        RvmFileIconName.Xls or RvmFileIconName.Mp3 or RvmFileIconName.Wav
            => RvmColor.Success,
        RvmFileIconName.Ppt or RvmFileIconName.Ai or RvmFileIconName.Eps or RvmFileIconName.FolderYellow
            => RvmColor.Warning,
        RvmFileIconName.Jpg or RvmFileIconName.Png or RvmFileIconName.Gif or RvmFileIconName.Svg or RvmFileIconName.Psd
            => RvmColor.Info,
        RvmFileIconName.Txt or RvmFileIconName.Zip or RvmFileIconName.Rar
            => RvmColor.Secondary,
        _ => RvmColor.Primary
    };

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (Size <= 0) throw new ArgumentOutOfRangeException(nameof(Size), Size, "O tamanho do icone precisa ser maior que zero.");
    }
}
