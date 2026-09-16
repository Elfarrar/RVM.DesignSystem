namespace RVM.DesignSystem.Components.Typography;

/// <summary>
/// Que elemento HTML o texto vira. Existe porque ESTILO e SEMANTICA sao coisas diferentes: um
/// titulo de secao pode precisar do tamanho de <c>H5</c> e continuar sendo um <c>h2</c> na ordem
/// do documento. Trocar um pelo outro quebra a navegacao por cabecalho no leitor de tela.
/// </summary>
public enum RvmTextElement
{
    /// <summary>Deduz do <c>Variant</c>: H1..H6 viram cabecalho, o resto vira paragrafo ou span.</summary>
    Auto,

    /// <summary>h1</summary>
    H1,

    /// <summary>h2</summary>
    H2,

    /// <summary>h3</summary>
    H3,

    /// <summary>h4</summary>
    H4,

    /// <summary>h5</summary>
    H5,

    /// <summary>h6</summary>
    H6,

    /// <summary>p</summary>
    P,

    /// <summary>span</summary>
    Span,

    /// <summary>div</summary>
    Div,

    /// <summary>label</summary>
    Label
}
