using System.Globalization;
using System.IO.Compression;
using System.Text;

namespace RVM.DesignSystem.Components.Chart;

/// <summary>
/// A tabela de dados do grafico como planilha do Excel. Um <c>.xlsx</c> e um ZIP de arquivos XML, e e
/// isso que esta classe monta — sem biblioteca de terceiros (decisao do projeto). O que o CSV nao da:
/// numero continua numero, entao a soma da coluna funciona sem ninguem reescrever nada.
/// </summary>
public static class RvmChartXlsx
{
    /// <summary>A planilha de um cabecalho e suas linhas. Celula que e numero entra como numero.</summary>
    public static byte[] Gerar(IReadOnlyList<string> cabecalho, IReadOnlyList<IReadOnlyList<string>> linhas, string aba = "Dados")
    {
        var arquivo = new MemoryStream();
        using (var zip = new ZipArchive(arquivo, ZipArchiveMode.Create, leaveOpen: true))
        {
            Escrever(zip, "[Content_Types].xml",
                """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
                  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
                  <Default Extension="xml" ContentType="application/xml"/>
                  <Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>
                  <Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
                </Types>
                """);

            Escrever(zip, "_rels/.rels",
                """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
                  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/>
                </Relationships>
                """);

            Escrever(zip, "xl/workbook.xml",
                $"""
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
                  <sheets><sheet name="{Texto(NomeDaAba(aba))}" sheetId="1" r:id="rId1"/></sheets>
                </workbook>
                """);

            Escrever(zip, "xl/_rels/workbook.xml.rels",
                """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
                  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/>
                </Relationships>
                """);

            Escrever(zip, "xl/worksheets/sheet1.xml", Planilha(cabecalho, linhas));
        }

        return arquivo.ToArray();
    }

    private static string Planilha(IReadOnlyList<string> cabecalho, IReadOnlyList<IReadOnlyList<string>> linhas)
    {
        var xml = new StringBuilder()
            .Append("""<?xml version="1.0" encoding="UTF-8" standalone="yes"?>""")
            .Append("""<worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"><sheetData>""");

        Linha(xml, 1, cabecalho);
        for (var i = 0; i < linhas.Count; i++)
        {
            Linha(xml, i + 2, linhas[i]);
        }

        return xml.Append("</sheetData></worksheet>").ToString();
    }

    private static void Linha(StringBuilder xml, int numero, IReadOnlyList<string> celulas)
    {
        xml.Append(CultureInfo.InvariantCulture, $"<row r=\"{numero}\">");
        for (var c = 0; c < celulas.Count; c++)
        {
            var referencia = Coluna(c) + numero.ToString(CultureInfo.InvariantCulture);
            // O Excel so soma o que entrou como numero: texto que "parece" numero vira aspa verde na celula.
            if (double.TryParse(celulas[c], NumberStyles.Float, CultureInfo.InvariantCulture, out var valor))
            {
                xml.Append(CultureInfo.InvariantCulture, $"<c r=\"{referencia}\"><v>{valor.ToString("R", CultureInfo.InvariantCulture)}</v></c>");
            }
            else
            {
                // inlineStr dispensa a tabela de textos compartilhados, que seria mais uma parte no ZIP.
                xml.Append(CultureInfo.InvariantCulture, $"<c r=\"{referencia}\" t=\"inlineStr\"><is><t xml:space=\"preserve\">{Texto(celulas[c])}</t></is></c>");
            }
        }

        xml.Append("</row>");
    }

    /// <summary>A, B, ... Z, AA, AB...</summary>
    private static string Coluna(int indice)
    {
        var nome = "";
        for (var i = indice; i >= 0; i = i / 26 - 1)
        {
            nome = (char)('A' + i % 26) + nome;
        }

        return nome;
    }

    /// <summary>O Excel recusa a planilha se o nome da aba passar de 31 caracteres ou tiver <c>:\/?*[]</c>.</summary>
    private static string NomeDaAba(string nome)
    {
        var limpo = new string([.. nome.Where(c => !":\\/?*[]".Contains(c))]).Trim();
        return string.IsNullOrEmpty(limpo) ? "Dados" : limpo[..Math.Min(31, limpo.Length)];
    }

    private static string Texto(string valor)
        => valor.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");

    private static void Escrever(ZipArchive zip, string caminho, string conteudo)
    {
        using var saida = zip.CreateEntry(caminho, CompressionLevel.Optimal).Open();
        var bytes = Encoding.UTF8.GetBytes(conteudo);
        saida.Write(bytes, 0, bytes.Length);
    }
}
