using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Calendar;

namespace RVM.DesignSystem.Components.DatePicker;

/// <summary>
/// Campo de intervalo de datas: o primeiro clique no calendario marca o inicio, o segundo o fim, e o
/// dialogo fecha.
/// </summary>
public sealed class RvmDateRangePicker : RvmDatePickerBase
{
    /// <summary>O intervalo escolhido. Aceita <c>@bind-Value</c>.</summary>
    [Parameter] public RvmDateRange? Value { get; set; }

    /// <summary>Disparado quando o intervalo muda (inclusive no primeiro clique, com o fim vazio).</summary>
    [Parameter] public EventCallback<RvmDateRange?> ValueChanged { get; set; }

    /// <summary>Expressao do valor ligado. O <c>@bind-Value</c> preenche sozinho.</summary>
    [Parameter] public Expression<Func<RvmDateRange?>>? ValueExpression { get; set; }

    internal override string PlaceholderPadrao => "dd/mm/aaaa – dd/mm/aaaa";

    internal override bool TemValor => Value is not null;

    internal override string TextoExibido
        => Value is { } r ? $"{Formatar(r.Start)} – {(r.End is { } fim ? Formatar(fim) : "…")}" : string.Empty;

    internal override IEnumerable<(string Sufixo, string Valor)> ValoresParaEnvio
    {
        get
        {
            if (Value is not { } r)
            {
                yield break;
            }

            yield return ("Inicio", Invariante(r.Start));
            if (r.End is { } fim)
            {
                yield return ("Fim", Invariante(fim));
            }
        }
    }

    internal override LambdaExpression? ExpressaoDoCampo => ValueExpression;

    internal override string DialogLabel => $"Escolher {Label}: primeiro o inicio, depois o fim";

    internal override RenderFragment ConteudoDoDialogo => builder =>
    {
        builder.OpenComponent<RvmCalendar>(0);
        builder.AddComponentParameter(1, nameof(RvmCalendar.IsRange), true);
        builder.AddComponentParameter(2, nameof(RvmCalendar.Range), Value);
        builder.AddComponentParameter(3, nameof(RvmCalendar.RangeChanged), EventCallback.Factory.Create<RvmDateRange?>(this, EscolherAsync));
        builder.AddComponentParameter(4, nameof(RvmCalendar.Min), Min);
        builder.AddComponentParameter(5, nameof(RvmCalendar.Max), Max);
        builder.AddComponentParameter(6, nameof(RvmCalendar.DateDisabled), DateDisabled);
        builder.AddComponentParameter(7, nameof(RvmCalendar.Today), Today);
        builder.AddComponentReferenceCapture(8, c => CalendarioAberto = (RvmCalendar)c);
        builder.CloseComponent();
    };

    private async Task EscolherAsync(RvmDateRange? intervalo)
    {
        Value = intervalo;
        await ValueChanged.InvokeAsync(intervalo);
        if (intervalo is { End: not null })
        {
            AvisarFormulario();
            await FecharAsync();
        }
    }
}
