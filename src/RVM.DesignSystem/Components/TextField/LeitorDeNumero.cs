using System.Globalization;
using System.Text;

namespace RVM.DesignSystem.Components.TextField;

/// <summary>
/// Leitura e escrita dos numeros do <see cref="RvmNumericField{TValue}"/> (contrato com o RVM.UI, DSGN-017).
/// O risco que isto evita e o erro de 1000x: <c>decimal.Parse</c> com <c>NumberStyles.Number</c> aceita separador de
/// milhar em qualquer lugar, e <c>"1.5"</c> em pt-BR vira 15 sem erro. Aqui o separador de milhar so vale em grupos de
/// tres digitos; fora disso o campo recusa com mensagem, nunca adivinha.
/// </summary>
internal static class LeitorDeNumero
{
    /// <summary>
    /// Cultura padrao do campo: a do ecossistema, nunca a do servidor. Sem os dados de cultura (InvariantGlobalization,
    /// container sem ICU), pedir "pt-BR" lanca — e no inicializador estatico derrubaria todo campo numerico. Ai sai uma
    /// copia da invariante com os separadores e a moeda do Brasil (achado do review da onda 2a, DSGN-017).
    /// </summary>
    public static readonly CultureInfo PtBr = CriarPtBr();

    private static CultureInfo CriarPtBr()
    {
        try
        {
            return CultureInfo.GetCultureInfo("pt-BR");
        }
        catch (CultureNotFoundException)
        {
            var cultura = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            var numeros = cultura.NumberFormat;
            numeros.NumberDecimalSeparator = numeros.CurrencyDecimalSeparator = numeros.PercentDecimalSeparator = ",";
            numeros.NumberGroupSeparator = numeros.CurrencyGroupSeparator = numeros.PercentGroupSeparator = ".";
            numeros.CurrencySymbol = "R$";
            // "R$ 1.234,56" e "-R$ 1.234,56", como o pt-BR do ICU (a invariante cola o simbolo no numero). O caso comum
            // e o WebAssembly, que so carrega o pacote de culturas EFIGS, sem portugues.
            numeros.CurrencyPositivePattern = 2;
            numeros.CurrencyNegativePattern = 9;
            return CultureInfo.ReadOnly(cultura);
        }
    }

    /// <summary>Le o texto digitado ou colado. Vazio devolve <c>true</c> com valor nulo: quem decide e o tipo.</summary>
    public static bool TryParse(string? texto, CultureInfo cultura, bool aceitaDecimais, int? casas, string? prefixo,
        string? sufixo, out decimal? valor, out string? erro)
    {
        valor = null;
        erro = null;
        var fmt = cultura.NumberFormat;
        var s = Limpar(texto, fmt, prefixo, sufixo);
        if (s.Length == 0) return true;

        var negativo = s[0] == '-';
        if (negativo) s = s[1..];

        var dec = fmt.NumberDecimalSeparator;
        var grupo = fmt.NumberGroupSeparator;
        var exemplo = 1234.56m.ToString("N2", cultura);
        var invalido = $"Digite um numero valido, como {exemplo}.";

        foreach (var c in s)
        {
            if (!char.IsAsciiDigit(c) && !dec.Contains(c) && !grupo.Contains(c))
            {
                erro = invalido;
                return false;
            }
        }

        var partes = s.Split(dec);
        if (partes.Length > 2)
        {
            erro = $"Use \"{dec}\" uma vez so, para as casas decimais, como {exemplo}.";
            return false;
        }

        var inteira = partes[0];
        var fracao = partes.Length == 2 ? partes[1] : "";

        if (grupo.Length > 0 && fracao.Contains(grupo, StringComparison.Ordinal))
        {
            erro = $"Depois de \"{dec}\" so vem as casas decimais, como {exemplo}.";
            return false;
        }

        if (grupo.Length > 0 && inteira.Contains(grupo, StringComparison.Ordinal))
        {
            var grupos = inteira.Split(grupo);
            if (grupos[0].Length is < 1 or > 3 || grupos.Skip(1).Any(g => g.Length != 3))
            {
                erro = $"Para casas decimais use \"{dec}\"; \"{grupo}\" so separa os milhares, como {exemplo}.";
                return false;
            }

            inteira = inteira.Replace(grupo, "", StringComparison.Ordinal);
        }

        if (inteira.Length == 0 && fracao.Length == 0)
        {
            erro = invalido;
            return false;
        }

        if (!aceitaDecimais && fracao.Length > 0)
        {
            erro = "Este campo aceita so numeros inteiros, sem casas decimais.";
            return false;
        }

        var invariante = (inteira.Length == 0 ? "0" : inteira) + (fracao.Length > 0 ? "." + fracao : "");
        if (!decimal.TryParse(invariante, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var lido))
        {
            erro = "Esse numero e grande demais para este campo.";
            return false;
        }

        if (negativo) lido = -lido;
        if (casas is { } n) lido = Math.Round(lido, n, MidpointRounding.AwayFromZero);

        valor = lido;
        return true;
    }

    /// <summary>Escreve o valor na mesma cultura em que ele e lido: <paramref name="formato"/> primeiro, depois as casas.</summary>
    public static string Escrever(decimal? valor, CultureInfo cultura, int? casas, string? formato)
    {
        if (valor is not { } v) return "";
        if (!string.IsNullOrWhiteSpace(formato)) return v.ToString(formato, cultura);
        return casas is { } n
            ? v.ToString("N" + n.ToString(CultureInfo.InvariantCulture), cultura)
            : v.ToString("#,##0.############################", cultura);
    }

    /// <summary>
    /// Tira o que pode vir colado junto do numero: prefixo e sufixo do campo, simbolos de moeda e de porcentagem da
    /// cultura e espacos (inclusive o nao separavel da formatacao pt-BR depois do "R$"). O menos tipografico vira hifen.
    /// </summary>
    private static string Limpar(string? texto, NumberFormatInfo fmt, string? prefixo, string? sufixo)
    {
        if (string.IsNullOrWhiteSpace(texto)) return "";

        var s = texto.Trim();
        foreach (var adorno in new[] { prefixo, sufixo, fmt.CurrencySymbol, fmt.PercentSymbol })
        {
            if (!string.IsNullOrWhiteSpace(adorno)) s = s.Replace(adorno.Trim(), "", StringComparison.OrdinalIgnoreCase);
        }

        var sb = new StringBuilder(s.Length);
        foreach (var c in s)
        {
            if (!char.IsWhiteSpace(c)) sb.Append(c == '−' ? '-' : c);
        }

        return sb.ToString();
    }
}
