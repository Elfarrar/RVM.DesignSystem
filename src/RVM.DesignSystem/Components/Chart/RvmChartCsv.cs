using System.Text;

namespace RVM.DesignSystem.Components.Chart;

/// <summary>
/// A tabela de dados do grafico em CSV. Separador <c>;</c> e BOM: o Excel em PT-BR abre CSV com virgula numa
/// coluna so, e sem BOM come os acentos.
/// </summary>
public static class RvmChartCsv
{
    /// <summary>O CSV de um cabecalho e suas linhas, pronto para baixar.</summary>
    public static string Gerar(IReadOnlyList<string> cabecalho, IReadOnlyList<IReadOnlyList<string>> linhas)
    {
        var texto = new StringBuilder("﻿");
        texto.Append(string.Join(';', cabecalho.Select(Campo))).Append("\r\n");
        foreach (var linha in linhas)
        {
            texto.Append(string.Join(';', linha.Select(Campo))).Append("\r\n");
        }

        return texto.ToString();
    }

    private static string Campo(string valor)
        => valor.AsSpan().IndexOfAny(';', '"', '\n') >= 0 || valor.Contains('\r')
            ? '"' + valor.Replace("\"", "\"\"") + '"'
            : valor;
}
