using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Button;
using RVM.DesignSystem.Components.PickerField;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.TimePicker;

/// <summary>
/// O <see cref="RvmTimePicker"/> no modo relogio: o campo abre o <see cref="RvmTimeClock"/> num dialogo.
/// A pessoa mexe num RASCUNHO; so OK (ou Enter nos minutos) grava, CANCELAR e Esc descartam — como o
/// "Time Picker (Mobile)" do kit.
/// </summary>
internal sealed class RvmCampoDeRelogio : RvmPickerFieldBase
{
    private TimeOnly? _rascunho;

    [Parameter] public TimeOnly? Value { get; set; }

    [Parameter] public EventCallback<TimeOnly?> ValueChanged { get; set; }

    [Parameter] public Expression<Func<TimeOnly?>>? ValueExpression { get; set; }

    [Parameter] public bool Use24Hours { get; set; } = true;

    [Parameter] public int Step { get; set; } = 1;

    [Parameter] public TimeOnly Min { get; set; } = TimeOnly.MinValue;

    [Parameter] public TimeOnly Max { get; set; } = new(23, 59);

    internal RvmTimeClock? RelogioAberto { get; set; }

    internal override bool TemValor => Value is not null;

    internal override string TextoExibido => Value is { } h ? RvmTimePicker.Formatar(h, Use24Hours) : string.Empty;

    internal override IEnumerable<(string Sufixo, string Valor)> ValoresParaEnvio
        => Value is { } h ? [(string.Empty, $"{h.Hour:00}:{h.Minute:00}")] : [];

    internal override LambdaExpression? ExpressaoDoCampo => ValueExpression;

    internal override string DialogLabel => $"Escolher {Label}";

    internal override string PlaceholderPadrao => "hh:mm";

    internal override RvmIconName IconeDoCampo => RvmIconName.Clock;

    internal override void AoAbrir() => _rascunho = Value;

    internal override void AoFechar() => RelogioAberto = null;

    internal override async Task<bool> FocarConteudoAsync()
    {
        if (RelogioAberto is null)
        {
            return false;
        }

        await RelogioAberto.FocusAsync();
        return true;
    }

    internal override RenderFragment ConteudoDoDialogo => builder =>
    {
        builder.OpenComponent<RvmTimeClock>(0);
        builder.AddComponentParameter(1, nameof(RvmTimeClock.Value), _rascunho);
        builder.AddComponentParameter(2, nameof(RvmTimeClock.ValueChanged), EventCallback.Factory.Create<TimeOnly?>(this, v => _rascunho = v));
        builder.AddComponentParameter(3, nameof(RvmTimeClock.Use24Hours), Use24Hours);
        builder.AddComponentParameter(4, nameof(RvmTimeClock.Step), Step);
        builder.AddComponentParameter(5, nameof(RvmTimeClock.Min), Min);
        builder.AddComponentParameter(6, nameof(RvmTimeClock.Max), Max);
        builder.AddComponentParameter(7, nameof(RvmTimeClock.OnConfirm), EventCallback.Factory.Create(this, ConfirmarAsync));
        builder.AddComponentParameter(8, nameof(RvmTimeClock.Actions), (RenderFragment)(acoes =>
        {
            acoes.OpenComponent<RvmButton>(0);
            acoes.AddComponentParameter(1, nameof(RvmButton.Variant), RvmButtonVariant.Text);
            acoes.AddComponentParameter(2, nameof(RvmButton.OnClick), EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, () => FecharAsync()));
            acoes.AddComponentParameter(3, nameof(RvmButton.ChildContent), (RenderFragment)(t => t.AddContent(0, "Cancelar")));
            acoes.CloseComponent();
            acoes.OpenComponent<RvmButton>(4);
            acoes.AddComponentParameter(5, nameof(RvmButton.Variant), RvmButtonVariant.Text);
            acoes.AddComponentParameter(6, nameof(RvmButton.OnClick), EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, ConfirmarAsync));
            acoes.AddComponentParameter(7, nameof(RvmButton.ChildContent), (RenderFragment)(t => t.AddContent(0, "OK")));
            acoes.CloseComponent();
        }));
        builder.AddComponentReferenceCapture(9, c => RelogioAberto = (RvmTimeClock)c);
        builder.CloseComponent();
    };

    // OK sem ter mexido grava o horario que o relogio mostra: a pessoa viu o ponteiro ali e confirmou.
    private async Task ConfirmarAsync()
    {
        var escolhido = _rascunho ?? RelogioAberto?.Atual;
        if (escolhido != Value)
        {
            Value = escolhido;
            await ValueChanged.InvokeAsync(escolhido);
            AvisarFormulario();
        }

        await FecharAsync();
    }
}
