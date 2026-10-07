namespace RVM.DesignSystem.Components.Chat;

/// <summary>
/// De que lado da conversa a mensagem aparece (contrato com o RVM.UI, DSGN-017). O lado decide tambem a cor do balao:
/// por isso os baloes nao tem parametro de cor.
/// </summary>
public enum RvmChatSide
{
    /// <summary>De quem escreveu para voce: balao na superficie do papel, a esquerda.</summary>
    Left,

    /// <summary>Suas mensagens: balao no primario do tema, a direita.</summary>
    Right
}
