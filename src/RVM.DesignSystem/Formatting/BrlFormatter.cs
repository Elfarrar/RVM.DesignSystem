using System.Globalization;

namespace RVM.DesignSystem.Formatting;

/// <summary>
/// Formata valores monetarios em real brasileiro.
/// </summary>
/// <remarks>
/// Existe para que nenhum componente formate moeda por interpolacao manual
/// (regra do CLAUDE.md § Convencoes): a cultura e explicita e nao depende da
/// cultura do processo, que varia entre Blazor Server e WebAssembly.
/// </remarks>
public static class BrlFormatter
{
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    /// <summary>Formata o valor como moeda, com simbolo: <c>R$ 1.234,56</c>.</summary>
    public static string Format(decimal value) => value.ToString("C2", PtBr);

    /// <summary>Formata o valor sem o simbolo da moeda: <c>1.234,56</c>.</summary>
    public static string FormatWithoutSymbol(decimal value) => value.ToString("N2", PtBr);

    /// <summary>
    /// Formata um valor opcional, devolvendo <paramref name="emptyText"/> quando nulo.
    /// </summary>
    public static string Format(decimal? value, string emptyText = "—")
        => value is null ? emptyText : Format(value.Value);
}
