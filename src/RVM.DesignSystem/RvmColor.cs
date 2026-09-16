namespace RVM.DesignSystem;

/// <summary>
/// Papel semantico de cor. O componente escolhe o PAPEL; que cor sai dele e problema da camada de
/// tokens, e muda com o tema. Componente nunca conhece cor (`05-api-dos-componentes.md`).
/// </summary>
public enum RvmColor
{
    /// <summary>Acao principal da tela.</summary>
    Primary,

    /// <summary>Acao de apoio, menos peso que a principal.</summary>
    Secondary,

    /// <summary>Informacao neutra.</summary>
    Info,

    /// <summary>Confirmacao de que algo deu certo.</summary>
    Success,

    /// <summary>Aviso: da para seguir, mas com atencao.</summary>
    Warning,

    /// <summary>Erro: alguma coisa falhou ou impede seguir.</summary>
    Error
}
