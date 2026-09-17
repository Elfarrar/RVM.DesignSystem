namespace RVM.DesignSystem.Components.AppShell;

/// <summary>Como o item de menu decide que e a pagina atual.</summary>
public enum RvmNavMatch
{
    /// <summary>O endereco atual comeca pelo do item (<c>/talhoes</c> vale em <c>/talhoes/12</c>).</summary>
    Prefix,

    /// <summary>O endereco atual e exatamente o do item. Use no item da pagina inicial.</summary>
    All
}
