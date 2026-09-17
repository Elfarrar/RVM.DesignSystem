using System.Globalization;
using System.Text;

namespace RVM.DesignSystem.Components.Chart;

/// <summary>
/// Um PDF de uma pagina com a imagem do grafico dentro, montado byte a byte — sem biblioteca de terceiros
/// (decisao do projeto) e sem compressao propria: o JPEG que vem do <c>canvas</c> ja e comprimido e entra
/// como imagem <c>DCTDecode</c>, o unico filtro que o PDF aceita sem reescrever os pixels.
/// </summary>
public static class RvmChartPdf
{
    /// <summary>O PDF com o JPEG ocupando a pagina inteira. Largura e altura em pontos (1 px = 1 pt).</summary>
    public static byte[] Criar(byte[] jpeg, int largura, int altura)
    {
        if (jpeg.Length == 0 || largura <= 0 || altura <= 0)
        {
            throw new ArgumentException("A imagem do grafico veio vazia.", nameof(jpeg));
        }

        var conteudo = Texto($"q {N(largura)} 0 0 {N(altura)} 0 0 cm /Im0 Do Q\n");
        var objetos = new List<byte[]>
        {
            Texto("<< /Type /Catalog /Pages 2 0 R >>"),
            Texto("<< /Type /Pages /Kids [3 0 R] /Count 1 >>"),
            Texto($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {N(largura)} {N(altura)}] "
                  + "/Resources << /XObject << /Im0 4 0 R >> >> /Contents 5 0 R >>"),
            Fluxo($"<< /Type /XObject /Subtype /Image /Width {N(largura)} /Height {N(altura)} "
                  + $"/ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode /Length {jpeg.Length} >>", jpeg),
            Fluxo($"<< /Length {conteudo.Length} >>", conteudo)
        };

        var pdf = new MemoryStream();
        Escrever(pdf, "%PDF-1.4\n%âãÏÓ\n");
        var posicoes = new List<long>();
        for (var i = 0; i < objetos.Count; i++)
        {
            posicoes.Add(pdf.Position);
            Escrever(pdf, $"{i + 1} 0 obj\n");
            pdf.Write(objetos[i]);
            Escrever(pdf, "\nendobj\n");
        }

        var inicioDoXref = pdf.Position;
        Escrever(pdf, $"xref\n0 {objetos.Count + 1}\n0000000000 65535 f \n");
        foreach (var posicao in posicoes)
        {
            Escrever(pdf, $"{posicao.ToString("0000000000", CultureInfo.InvariantCulture)} 00000 n \n");
        }

        Escrever(pdf, $"trailer\n<< /Size {objetos.Count + 1} /Root 1 0 R >>\nstartxref\n{inicioDoXref}\n%%EOF\n");
        return pdf.ToArray();
    }

    private static byte[] Fluxo(string dicionario, byte[] dados)
    {
        var fluxo = new MemoryStream();
        Escrever(fluxo, dicionario + "\nstream\n");
        fluxo.Write(dados);
        Escrever(fluxo, "\nendstream");
        return fluxo.ToArray();
    }

    private static byte[] Texto(string texto) => Encoding.Latin1.GetBytes(texto);

    private static void Escrever(Stream destino, string texto) => destino.Write(Texto(texto));

    private static string N(int valor) => valor.ToString(CultureInfo.InvariantCulture);
}
