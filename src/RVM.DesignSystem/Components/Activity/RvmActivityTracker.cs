namespace RVM.DesignSystem.Components.Activity;

/// <summary>Marcador da coluna esquerda do <see cref="RvmActivity"/> (contrato com o RVM.UI, DSGN-017).</summary>
public enum RvmActivityTracker
{
    /// <summary>So um ponto na cor primaria — o padrao.</summary>
    Dot,

    /// <summary>Circulo suave com um icone (exige <c>Icon</c>).</summary>
    Icon,

    /// <summary>A foto de quem fez a acao (<c>RvmAvatar</c>).</summary>
    Avatar
}
