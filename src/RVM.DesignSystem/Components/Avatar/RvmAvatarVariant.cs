namespace RVM.DesignSystem.Components.Avatar;

/// <summary>
/// Preenchimento do avatar de iniciais ou icone. No kit: a linha "Colors" e o
/// <see cref="Filled"/>; a linha "Light" e o <see cref="Soft"/>.
/// </summary>
public enum RvmAvatarVariant
{
    /// <summary>Fundo na cor do papel, conteudo no token de contraste.</summary>
    Filled,

    /// <summary>Fundo suave do papel, conteudo na variante <c>-text</c> (medido: 4.71:1 no pior caso).</summary>
    Soft
}
