namespace RVM.DesignSystem.Icons;

/// <summary>
/// Estilo do desenho do icone (contrato com o RVM.UI, DSGN-017). O Tabler tem traco e cheio: <see cref="Bold"/> e
/// <see cref="BoldDuotone"/> saem cheios onde o Tabler desenha a versao cheia; o resto sai no traco.
/// </summary>
public enum RvmIconStyle
{
    /// <summary>Cheio, quando o Tabler tem a versao cheia do icone; senao, traco.</summary>
    Bold,

    /// <summary>O mesmo que <see cref="Bold"/>: o Tabler nao tem duotone.</summary>
    BoldDuotone,

    /// <summary>Traco. O Tabler nao tem o traco interrompido do Solar.</summary>
    Broken,

    /// <summary>Traco — o padrao.</summary>
    Linear
}
