using System.Globalization;
using RVM.DesignSystem.Theming;

namespace RVM.DesignSystem.Components;

/// <summary>
/// A cor e as iniciais que um nome produz.
/// </summary>
/// <remarks>
/// Isolado do componente para poder ser <b>medido em teste</b>: a cor de fundo do avatar nao sai
/// da paleta do tema, ela e calculada a partir do nome - e cor calculada e exatamente o tipo de
/// cor que escapa do portao de contraste sem ninguem perceber. Aqui ela nao escapa: o teste
/// varre milhares de nomes e reprova se algum par cair abaixo de 4.5:1.
/// </remarks>
public static class RvmAvatarCor
{
    private static readonly char[] Separador = [' '];

    /// <summary>
    /// Deriva o par de cores (fundo, texto) de um nome.
    /// </summary>
    /// <param name="nome">O nome completo da pessoa ou entidade.</param>
    /// <returns>Fundo e texto em hexadecimal, com contraste AA garantido entre os dois.</returns>
    /// <remarks>
    /// A matiz vem de um hash <b>FNV-1a</b>, e nao de <c>string.GetHashCode</c>: desde o .NET
    /// Core o hash de string e aleatorizado por processo, entao o mesmo nome mudaria de cor a
    /// cada reinicio do servidor - e num avatar a cor e o que a pessoa usa para se reconhecer
    /// numa lista. Precisa ser estavel entre processos e entre maquinas.
    ///
    /// <para>
    /// Luminosidade e croma sao <b>fixos</b>. Deixa-los variar com o hash daria paletas mais
    /// bonitas e contraste imprevisivel - e o contraste e o que nao e negociavel.
    /// </para>
    /// </remarks>
    public static (string Fundo, string Texto) DoNome(string? nome)
    {
        var matiz = MatizDe(nome);

        // L=0.55 e C=0.11: escuro o bastante para receber texto claro em toda a volta do
        // circulo de matizes, e colorido o bastante para os avatares se distinguirem.
        var fundo = RvmColor.FromOklch(0.55, 0.11, matiz);
        var texto = RvmContrast.Ensure(
            RvmContrast.BestForegroundOn(fundo), fundo, RvmContrast.NormalText);

        return (fundo.ToHex(), texto.ToHex());
    }

    /// <summary>Matiz (0-360) estavel para um nome.</summary>
    private static double MatizDe(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            return 0;
        }

        // FNV-1a de 32 bits. Estavel entre processos, plataformas e versoes do runtime.
        const uint offset = 2166136261;
        const uint prime = 16777619;

        var hash = offset;

        foreach (var c in nome.Trim().ToUpperInvariant())
        {
            hash ^= c;
            hash *= prime;
        }

        return hash % 360;
    }

    /// <summary>
    /// As iniciais exibidas quando nao ha imagem.
    /// </summary>
    /// <param name="nome">O nome completo.</param>
    /// <returns>Uma ou duas letras maiusculas; vazio quando o nome nao tem letra alguma.</returns>
    /// <remarks>
    /// Primeira e ultima palavra, e nao as duas primeiras: "Maria Aparecida Silva" vira "MS", que
    /// e como a pessoa e identificada, e nao "MA".
    /// </remarks>
    public static string Iniciais(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            return string.Empty;
        }

        var palavras = nome
            .Split(Separador, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(p => char.IsLetter(p[0]))
            .ToArray();

        if (palavras.Length == 0)
        {
            return string.Empty;
        }

        var primeira = char.ToUpper(palavras[0][0], CultureInfo.InvariantCulture);

        return palavras.Length == 1
            ? primeira.ToString()
            : string.Concat(primeira, char.ToUpper(palavras[^1][0], CultureInfo.InvariantCulture));
    }
}
