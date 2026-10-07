namespace RVM.DesignSystem.Components.Button;

/// <summary>Peso visual do botao. Uma tela tem UM contained em destaque; o resto apoia.</summary>
public enum RvmButtonVariant
{
    /// <summary>Preenchido com a cor do papel. E a acao principal.</summary>
    Contained,

    /// <summary>So contorno. Acao secundaria, com o mesmo peso de leitura.</summary>
    Outlined,

    /// <summary>So texto. Acao terciaria, dentro de um card ou de uma linha de lista.</summary>
    Text,

    /// <summary>Fundo suave da cor do papel, texto na cor. Acao de apoio com mais presenca que o contorno.</summary>
    Soft,

    // Aliases do contrato com o RVM.UI (DSGN-017): o mesmo valor, o nome que o RVM.UI usa.

    /// <summary>O mesmo que <see cref="Contained"/>.</summary>
    Filled = Contained
}
