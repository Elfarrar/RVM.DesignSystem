namespace RVM.DesignSystem.Components.Typography;

/// <summary>
/// Papel do texto na hierarquia de leitura. Sao os tokens de texto, nao as cores de marca:
/// para escrever com a cor de um papel semantico, use a classe do token <c>-text</c>.
/// </summary>
public enum RvmTextColor
{
    /// <summary>Herda de quem esta em volta — o padrao.</summary>
    Inherit,

    /// <summary>O texto que o usuario veio ler.</summary>
    Primary,

    /// <summary>Apoio: rotulo, legenda, explicacao.</summary>
    Secondary,

    /// <summary>Indisponivel. Nao vale para texto que precisa ser lido.</summary>
    Disabled
}
