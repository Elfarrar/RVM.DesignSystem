namespace RVM.DesignSystem.Components.Timeline;

/// <summary>De que lado da linha fica o conteudo principal.</summary>
public enum RvmTimelinePosition
{
    /// <summary>A direita da linha ("Standard" no kit). O padrao.</summary>
    Right,

    /// <summary>A esquerda da linha, alinhado a direita ("Right" no kit).</summary>
    Left,

    /// <summary>Alternando os lados a cada item ("Alternating" no kit).</summary>
    Alternate
}
