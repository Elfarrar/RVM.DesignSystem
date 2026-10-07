using RVM.DesignSystem.Components.FileIcon;

namespace RVM.DesignSystem.Components.FileCard;

/// <summary>O tipo do <see cref="RvmFileIcon"/> pela extensao do nome do arquivo.</summary>
internal static class TipoDoArquivo
{
    private static readonly Dictionary<string, RvmFileIconName> Tipos = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ai"] = RvmFileIconName.Ai, ["avi"] = RvmFileIconName.Avi, ["doc"] = RvmFileIconName.Doc, ["docx"] = RvmFileIconName.Doc,
        ["eps"] = RvmFileIconName.Eps, ["fig"] = RvmFileIconName.Fig, ["gif"] = RvmFileIconName.Gif, ["jpg"] = RvmFileIconName.Jpg,
        ["jpeg"] = RvmFileIconName.Jpg, ["mkv"] = RvmFileIconName.Mkv, ["mov"] = RvmFileIconName.Mov, ["mp3"] = RvmFileIconName.Mp3,
        ["mp4"] = RvmFileIconName.Mp4, ["pdf"] = RvmFileIconName.Pdf, ["png"] = RvmFileIconName.Png, ["ppt"] = RvmFileIconName.Ppt,
        ["pptx"] = RvmFileIconName.Ppt, ["psd"] = RvmFileIconName.Psd, ["rar"] = RvmFileIconName.Rar, ["svg"] = RvmFileIconName.Svg,
        ["txt"] = RvmFileIconName.Txt, ["wav"] = RvmFileIconName.Wav, ["xls"] = RvmFileIconName.Xls, ["xlsx"] = RvmFileIconName.Xls,
        ["csv"] = RvmFileIconName.Xls, ["zip"] = RvmFileIconName.Zip
    };

    /// <summary>O tipo pela extensao; extensao desconhecida (ou nenhuma) vira texto, como no RVM.UI.</summary>
    public static RvmFileIconName PeloNome(string? nome)
        => Tipos.TryGetValue(Path.GetExtension(nome ?? string.Empty).TrimStart('.'), out var tipo) ? tipo : RvmFileIconName.Txt;
}
