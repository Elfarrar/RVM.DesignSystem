namespace RVM.DesignSystem.Components.EventCalendar;

/// <summary>O periodo que o <see cref="RvmEventCalendar"/> mostra.</summary>
public enum RvmCalendarView
{
    /// <summary>O mes inteiro, em semanas de domingo a sabado.</summary>
    Month,

    /// <summary>Uma semana, de domingo a sabado, com a faixa de horas.</summary>
    Week,

    /// <summary>Um dia, com a faixa de horas.</summary>
    Day
}
