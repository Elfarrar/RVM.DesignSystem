using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Calendar;

namespace RVM.DesignSystem.Components.DatePicker;

/// <summary>Campo de data com calendario. Dentro de um EditForm, participa da validacao.</summary>
public sealed class RvmDatePicker : RvmDatePickerBase
{
    /// <summary>A data escolhida. Aceita <c>@bind-Value</c>.</summary>
    [Parameter] public DateOnly? Value { get; set; }

    /// <summary>Disparado quando a data muda.</summary>
    [Parameter] public EventCallback<DateOnly?> ValueChanged { get; set; }

    /// <summary>Expressao do valor ligado. O <c>@bind-Value</c> preenche sozinho.</summary>
    [Parameter] public Expression<Func<DateOnly?>>? ValueExpression { get; set; }

    internal override bool TemValor => Value is not null;

    internal override string TextoExibido => Value is { } d ? Formatar(d) : string.Empty;

    internal override IEnumerable<(string Sufixo, string Valor)> ValoresParaEnvio
        => Value is { } d ? [(string.Empty, Invariante(d))] : [];

    internal override LambdaExpression? ExpressaoDoCampo => ValueExpression;

    internal override string DialogLabel => $"Escolher {Label}";

    internal override RenderFragment ConteudoDoDialogo => builder =>
    {
        builder.OpenComponent<RvmCalendar>(0);
        builder.AddComponentParameter(1, nameof(RvmCalendar.Value), Value);
        builder.AddComponentParameter(2, nameof(RvmCalendar.ValueChanged), EventCallback.Factory.Create<DateOnly?>(this, EscolherAsync));
        builder.AddComponentParameter(3, nameof(RvmCalendar.Min), Min);
        builder.AddComponentParameter(4, nameof(RvmCalendar.Max), Max);
        builder.AddComponentParameter(5, nameof(RvmCalendar.DateDisabled), DateDisabled);
        builder.AddComponentParameter(6, nameof(RvmCalendar.Today), Today);
        builder.AddComponentReferenceCapture(7, c => CalendarioAberto = (RvmCalendar)c);
        builder.CloseComponent();
    };

    private async Task EscolherAsync(DateOnly? data)
    {
        Value = data;
        await ValueChanged.InvokeAsync(data);
        AvisarFormulario();
        await FecharAsync();
    }
}
