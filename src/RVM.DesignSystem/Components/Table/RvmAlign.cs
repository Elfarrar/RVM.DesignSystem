namespace RVM.DesignSystem.Components.Table;

/// <summary>
/// Alinhamento do conteudo de uma celula ou coluna da tabela de dados (contrato com o RVM.UI, DSGN-017). Os mesmos tres
/// do <see cref="RvmTableAlign"/>, que a <see cref="RvmTable{TItem}"/> usa.
/// </summary>
public enum RvmAlign
{
    /// <summary>No inicio da linha (esquerda). O padrao para texto.</summary>
    Start,

    /// <summary>No centro.</summary>
    Center,

    /// <summary>No fim da linha (direita). Para numeros e valores, que assim alinham pela unidade.</summary>
    End
}
