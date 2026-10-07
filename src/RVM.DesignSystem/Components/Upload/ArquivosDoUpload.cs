using System.Globalization;
using Microsoft.AspNetCore.Components.Forms;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Upload;

/// <summary>O que os tres componentes de envio dividem: ler a escolha, formatar tamanho, escolher o icone e os ids.</summary>
internal static class ArquivosDoUpload
{
    private static readonly string[] Unidades = ["KB", "MB", "GB", "TB"];

    /// <summary>
    /// Tamanho em PT-BR ("1,2 MB"), base 1024. Formatado na cultura invariante e com a virgula trocada a mao: o WASM
    /// roda sem ICU, e a cultura pt-BR nao existe la.
    /// </summary>
    public static string Tamanho(long bytes)
    {
        if (bytes < 1024) return $"{Math.Max(0, bytes).ToString(CultureInfo.InvariantCulture)} B";
        double valor = bytes;
        var unidade = -1;
        while (valor >= 1024 && unidade < Unidades.Length - 1)
        {
            valor /= 1024;
            unidade++;
        }
        // 1.048.500 bytes dariam 1023,9 KB, arredondado para "1024 KB": sobe para a proxima unidade.
        if (Math.Round(valor, 1) >= 1024 && unidade < Unidades.Length - 1)
        {
            valor /= 1024;
            unidade++;
        }
        return $"{valor.ToString("0.#", CultureInfo.InvariantCulture).Replace('.', ',')} {Unidades[unidade]}";
    }

    /// <summary>O icone pelo tipo do arquivo, lido da extensao.</summary>
    public static RvmIconName Icone(string nome) => Path.GetExtension(nome).ToLowerInvariant() switch
    {
        ".pdf" or ".doc" or ".docx" or ".odt" or ".txt" or ".rtf" or ".md" => RvmIconName.FileText,
        ".xls" or ".xlsx" or ".ods" or ".csv" => RvmIconName.Table,
        ".png" or ".jpg" or ".jpeg" or ".gif" or ".webp" or ".svg" or ".bmp" or ".heic" => RvmIconName.Gallery,
        ".mp4" or ".mov" or ".avi" or ".mkv" or ".webm" => RvmIconName.VideoFrame,
        ".mp3" or ".wav" or ".ogg" or ".m4a" or ".flac" => RvmIconName.MusicNote,
        ".zip" or ".rar" or ".7z" or ".tar" or ".gz" => RvmIconName.ZipFile,
        _ => RvmIconName.File
    };

    /// <summary>"1 arquivo escolhido" ou "3 arquivos escolhidos", para a regiao <c>aria-live</c>.</summary>
    public static string Escolhidos(int quantidade)
        => quantidade == 1 ? "1 arquivo escolhido" : $"{quantidade.ToString(CultureInfo.InvariantCulture)} arquivos escolhidos";

    /// <summary>
    /// Le a escolha respeitando o limite. Acima dele devolve a mensagem de erro em vez de lancar — o
    /// <c>GetMultipleFiles</c> lanca quando ha mais arquivos que o maximo pedido.
    /// </summary>
    public static string? Ler(InputFileChangeEventArgs e, int maximo, out IReadOnlyList<IBrowserFile> arquivos)
    {
        var limite = Math.Max(1, maximo);
        if (e.FileCount > limite)
        {
            arquivos = [];
            return limite == 1
                ? $"Voce escolheu {e.FileCount} arquivos, mas aqui cabe so 1 por vez. Escolha um arquivo e tente de novo."
                : $"Voce escolheu {e.FileCount} arquivos, mas o limite e {limite} por vez. Escolha menos arquivos e tente de novo.";
        }
        arquivos = e.FileCount == 0 ? [] : e.GetMultipleFiles(limite);
        return null;
    }

    /// <summary>Junta ids para <c>aria-describedby</c>/<c>aria-labelledby</c>; nulo quando nao sobra nenhum.</summary>
    public static string? Ids(params string?[] ids)
    {
        var juntos = string.Join(' ', ids.Where(i => !string.IsNullOrWhiteSpace(i)));
        return juntos.Length == 0 ? null : juntos;
    }
}
