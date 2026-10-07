namespace RVM.DesignSystem.Components.Widget;

/// <summary>Forma do gatilho do <see cref="RvmWidget"/> (contrato com o RVM.UI, DSGN-017).</summary>
public enum RvmWidgetVariant
{
    /// <summary>Botao redondo com icone ou imagem, como o sino e a bandeira do topo do kit — o padrao.</summary>
    Icon,

    /// <summary>Cartao com imagem, titulo, subtitulo e seta (perfil, equipe).</summary>
    Card
}
