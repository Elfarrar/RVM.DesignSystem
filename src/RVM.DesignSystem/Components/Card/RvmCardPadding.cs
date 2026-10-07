namespace RVM.DesignSystem.Components.Card;

/// <summary>Respiro interno do <see cref="RvmCard"/> (contrato com o RVM.UI, DSGN-017).</summary>
public enum RvmCardPadding
{
    /// <summary>Sem respiro: o conteudo encosta na borda (tabela, imagem).</summary>
    None,

    /// <summary>16 px.</summary>
    Small,

    /// <summary>20 px, o medido no kit — o padrao.</summary>
    Medium,

    /// <summary>32 px.</summary>
    Large
}
