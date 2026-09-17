namespace RVM.DesignSystem.Components.Button;

/// <summary>Peso visual do botao. Uma tela tem UM contained em destaque; o resto apoia.</summary>
public enum RvmButtonVariant
{
    /// <summary>Preenchido com a cor do papel. E a acao principal.</summary>
    Contained,

    /// <summary>So contorno. Acao secundaria, com o mesmo peso de leitura.</summary>
    Outlined,

    /// <summary>So texto. Acao terciaria, dentro de um card ou de uma linha de lista.</summary>
    Text
}
