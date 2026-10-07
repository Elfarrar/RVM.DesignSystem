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
    Disabled,

    /// <summary>Na cor do papel primario (o token <c>-text</c>, legivel nos dois temas).</summary>
    Accent,

    /// <summary>Confirmacao.</summary>
    Success,

    /// <summary>Erro.</summary>
    Danger,

    /// <summary>Atencao.</summary>
    Warning,

    /// <summary>Texto claro sobre fundo escuro ou colorido.</summary>
    Inverse,

    // Aliases do contrato com o RVM.UI (DSGN-017): o mesmo valor, o nome que o RVM.UI usa.

    /// <summary>O mesmo que <see cref="Primary"/>.</summary>
    Default = Primary
}
