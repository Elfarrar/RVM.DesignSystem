namespace RVM.DesignSystem.Components.Menu;

/// <summary>Forma do gatilho do <see cref="RvmMenu"/> (contrato com o RVM.UI, DSGN-017).</summary>
public enum RvmMenuTrigger
{
    /// <summary>Botao com o rotulo e a seta, no estilo de <c>ButtonVariant</c>.</summary>
    Button,

    /// <summary>So o icone (<c>Icon</c>, ou os tres pontos); o <c>Label</c> vira o nome acessivel.</summary>
    Icon,

    /// <summary>Botao com o <c>Text</c> (ou o <c>Label</c>) e a seta — o padrao.</summary>
    Text
}
