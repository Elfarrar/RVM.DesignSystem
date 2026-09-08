namespace RVM.DesignSystem.Theming;

/// <summary>
/// As cores que um produto escolhe. Tudo o mais e derivado delas.
/// </summary>
/// <remarks>
/// <b>Duas cores sao obrigatorias; quatro sao opcionais.</b> Essa proporcao e a tese do motor de
/// tema: um produto novo escreve duas linhas em vez das ~200 que cada app do ecossistema
/// duplicou.
///
/// <para>
/// As quatro opcionais existem porque <b>convencao cultural nao e lei</b>. Verde-sucesso,
/// vermelho-erro e a leitura mais comum no Brasil, e por isso e o padrao — mas um produto de
/// sinalizacao ferroviaria, um de saude ou um cuja marca ja e vermelha tem motivo real para
/// discordar. Quem discorda informa a cor; quem nao informa fica com o padrao.
/// </para>
///
/// <para>
/// ⚠️ <b>Informar uma cor de estado nao dispensa o contraste.</b> O que a biblioteca aceita de
/// voce e a <b>matiz e o croma</b>; a luminosidade continua sendo calculada contra a superficie
/// mais exigente da paleta, ate atender AA. Uma cor bonita e ilegivel entra como a versao
/// legivel dela mesma — ver <see cref="RvmContrast.Ensure"/>.
/// </para>
/// </remarks>
public sealed record RvmSeed
{
    /// <summary>
    /// Cor principal da marca, em hexadecimal.
    /// </summary>
    /// <remarks>
    /// E ela que aparece no botao de acao, no link e no item de menu ativo — os <b>10%</b> da
    /// regra de proporcao. Ela tambem matiza os neutros em ~4% do croma dela, que e o detalhe
    /// barato que faz a interface parecer desenhada em vez de montada.
    /// </remarks>
    public required string Primary { get; init; }

    /// <summary>Cor de apoio, em hexadecimal. Complementa a primaria em variacoes de componente.</summary>
    public required string Secondary { get; init; }

    /// <summary>Cor de sucesso. Nulo usa o verde padrao.</summary>
    public string? Success { get; init; }

    /// <summary>Cor de aviso. Nulo usa o ambar padrao.</summary>
    public string? Warning { get; init; }

    /// <summary>Cor de erro. Nulo usa o vermelho padrao.</summary>
    public string? Danger { get; init; }

    /// <summary>Cor de informacao. Nulo usa o azul padrao.</summary>
    public string? Info { get; init; }
}
