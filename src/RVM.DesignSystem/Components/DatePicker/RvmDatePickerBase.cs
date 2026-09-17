using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Calendar;
using RVM.DesignSystem.Components.PickerField;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.DatePicker;

/// <summary>
/// O que <see cref="RvmDatePicker"/> e <see cref="RvmDateRangePicker"/> tem em comum: o campo com o
/// calendario no dialogo e os limites de data.
/// </summary>
public abstract class RvmDatePickerBase : RvmPickerFieldBase
{
    /// <summary>Primeira data escolhivel.</summary>
    [Parameter] public DateOnly? Min { get; set; }

    /// <summary>Ultima data escolhivel.</summary>
    [Parameter] public DateOnly? Max { get; set; }

    /// <summary>Datas que nao podem ser escolhidas.</summary>
    [Parameter] public Func<DateOnly, bool>? DateDisabled { get; set; }

    /// <summary>"Hoje" para marcar o dia atual no calendario. Padrao: a data do sistema.</summary>
    [Parameter] public DateOnly? Today { get; set; }

    internal RvmCalendar? CalendarioAberto { get; set; }

    internal override string PlaceholderPadrao => "dd/mm/aaaa";

    internal override RvmIconName IconeDoCampo => RvmIconName.Calendar;

    internal override async Task<bool> FocarConteudoAsync()
    {
        if (CalendarioAberto is null)
        {
            return false;
        }

        await CalendarioAberto.FocusAsync();
        return true;
    }

    internal override void AoFechar() => CalendarioAberto = null;

    internal static string Formatar(DateOnly d) => $"{d.Day:00}/{d.Month:00}/{d.Year:0000}";

    internal static string Invariante(DateOnly d) => $"{d.Year:0000}-{d.Month:00}-{d.Day:00}";
}
