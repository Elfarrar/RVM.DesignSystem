namespace RVM.DesignSystem.Components.Typography;

/// <summary>
/// Nivel de texto do contrato com o RVM.UI (DSGN-017). Cada nivel aponta para um estilo da escala do
/// design system (<see cref="RvmTypographyVariant"/>); a tabela de correspondencia esta na pagina do RvmText.
/// </summary>
public enum RvmTextVariant
{
    /// <summary>O maior titulo. Escala do DS: <c>rvm-text-h3</c> (48/56).</summary>
    DisplayXl,

    /// <summary>Titulo grande. Escala do DS: <c>rvm-text-h4</c> (34/42).</summary>
    DisplayL,

    /// <summary>Titulo medio. Escala do DS: <c>rvm-text-h5</c> (24/32).</summary>
    DisplayM,

    /// <summary>Titulo pequeno. Escala do DS: <c>rvm-text-h6</c> (20/32).</summary>
    DisplayS,

    /// <summary>Texto de destaque. Escala do DS: <c>rvm-text-subtitle1</c> (16/28).</summary>
    TextXl,

    /// <summary>Texto grande. Escala do DS: <c>rvm-text-input</c> (16/24).</summary>
    TextL,

    /// <summary>Texto padrao. Escala do DS: <c>rvm-text-body2</c> (14/20).</summary>
    TextM,

    /// <summary>Texto pequeno. Escala do DS: <c>rvm-text-caption</c> (12/20).</summary>
    TextS,

    /// <summary>So rotulo auxiliar, abaixo do legivel para texto corrido. Escala do DS: <c>rvm-text-tooltip</c> (10/14).</summary>
    TextXs
}
