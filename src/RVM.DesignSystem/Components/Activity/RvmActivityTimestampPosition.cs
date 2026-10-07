namespace RVM.DesignSystem.Components.Activity;

/// <summary>Onde o carimbo de data e hora do <see cref="RvmActivity"/> aparece (contrato com o RVM.UI, DSGN-017).</summary>
public enum RvmActivityTimestampPosition
{
    /// <summary>Abaixo do conteudo, sozinho na ultima linha — o padrao.</summary>
    Bottom,

    /// <summary>No canto superior direito, ao lado do titulo — como no "Activity Timeline" do kit.</summary>
    TopRight
}
