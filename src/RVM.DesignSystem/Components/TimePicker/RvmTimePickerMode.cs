namespace RVM.DesignSystem.Components.TimePicker;

/// <summary>Como o <see cref="RvmTimePicker"/> oferece os horarios.</summary>
public enum RvmTimePickerMode
{
    /// <summary>Lista de horarios com busca (a coluna "Time Picker" do kit). O padrao.</summary>
    List,

    /// <summary>Relogio circular num dialogo, com CANCELAR e OK (o "Time Picker (Mobile)" do kit).</summary>
    Clock
}
