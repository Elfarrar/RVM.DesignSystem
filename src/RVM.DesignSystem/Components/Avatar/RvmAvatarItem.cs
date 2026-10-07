namespace RVM.DesignSystem.Components.Avatar;

/// <summary>Pessoa de um <see cref="RvmAvatarGroup"/> montado por <c>Items</c> (contrato com o RVM.UI, DSGN-017).</summary>
/// <param name="Name">Nome: texto alternativo do avatar e, sem foto, a fonte das iniciais.</param>
/// <param name="Src">Foto. Sem ela, as iniciais do nome.</param>
public sealed record RvmAvatarItem(string Name, string? Src = null);
