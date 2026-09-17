using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.TextField;

namespace RVM.DesignSystem.Components.TimePicker;

/// <summary>
/// Campo de horario em 24 h (padrao) ou 12 h: uma lista de horarios em intervalos fixos, ou um relogio
/// circular num dialogo (<see cref="Mode"/>).
/// </summary>
public partial class RvmTimePicker : ComponentBase
{
    /// <summary>O horario escolhido. Aceita <c>@bind-Value</c>.</summary>
    [Parameter] public TimeOnly? Value { get; set; }

    /// <summary>Disparado quando o horario muda.</summary>
    [Parameter] public EventCallback<TimeOnly?> ValueChanged { get; set; }

    /// <summary>Expressao do valor ligado. O <c>@bind-Value</c> preenche sozinho.</summary>
    [Parameter] public Expression<Func<TimeOnly?>>? ValueExpression { get; set; }

    /// <summary>Lista ou relogio. Padrao: <see cref="RvmTimePickerMode.List"/>.</summary>
    [Parameter] public RvmTimePickerMode Mode { get; set; } = RvmTimePickerMode.List;

    /// <summary>Minutos entre um horario e o proximo. Padrao: 30 na lista, 1 no relogio.</summary>
    [Parameter] public int? Step { get; set; }

    /// <summary>Primeiro horario da lista. Padrao: 00:00.</summary>
    [Parameter] public TimeOnly Min { get; set; } = TimeOnly.MinValue;

    /// <summary>Ultimo horario da lista (inclusive). Padrao: 23:59.</summary>
    [Parameter] public TimeOnly Max { get; set; } = new(23, 59);

    /// <summary>Relogio de 24 h ("18:30", padrao) ou de 12 h ("6:30 PM").</summary>
    [Parameter] public bool Use24Hours { get; set; } = true;

    /// <summary>Rotulo do campo.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Texto de apoio abaixo.</summary>
    [Parameter] public string? HelperText { get; set; }

    /// <summary>Erro informado por fora.</summary>
    [Parameter] public string? ErrorText { get; set; }

    /// <summary>Texto no campo vazio.</summary>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>Estilo do campo. Padrao: contorno. O relogio usa sempre o contorno.</summary>
    [Parameter] public RvmTextFieldVariant Variant { get; set; } = RvmTextFieldVariant.Outlined;

    /// <summary>56 px (padrao) ou 40 px.</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>Busca no topo da lista: digitar "18" acha 18:00 e 18:30. Padrao: sim. So na lista.</summary>
    [Parameter] public bool Searchable { get; set; } = true;

    /// <summary>Asterisco e aria-required.</summary>
    [Parameter] public bool Required { get; set; }

    /// <summary>Indisponivel.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary><c>name</c> para envio de formulario (valor em <c>HH:mm</c>).</summary>
    [Parameter] public string? Name { get; set; }

    /// <summary>Atributos extras, repassados ao campo.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Os horarios da lista, de <see cref="Min"/> a <see cref="Max"/>, de <see cref="Step"/> em <see cref="Step"/>.</summary>
    internal IReadOnlyList<TimeOnly?> Horarios
    {
        get
        {
            var passo = Math.Clamp(Step ?? 30, 1, 720);
            var lista = new List<TimeOnly?>();
            for (var minutos = Min.Hour * 60 + Min.Minute; minutos <= Max.Hour * 60 + Max.Minute; minutos += passo)
            {
                lista.Add(new TimeOnly(minutos / 60, minutos % 60));
            }

            return lista;
        }
    }

    internal string Texto(TimeOnly? horario) => horario is { } h ? Formatar(h, Use24Hours) : string.Empty;

    /// <summary>"18:30" em 24 h; "6:30 PM" em 12 h.</summary>
    internal static string Formatar(TimeOnly h, bool use24Hours)
        => use24Hours
            ? $"{h.Hour:00}:{h.Minute:00}"
            : $"{RvmTimeClock.Hora12(h.Hour)}:{h.Minute:00} {(h.Hour < 12 ? "AM" : "PM")}";

    // O RvmCampoDeRelogio e interno, e o Razor so enxerga componente publico na marcacao.
    internal RenderFragment CampoDeRelogio => builder =>
    {
        builder.OpenComponent<RvmCampoDeRelogio>(0);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddComponentParameter(2, nameof(RvmCampoDeRelogio.Value), Value);
        builder.AddComponentParameter(3, nameof(RvmCampoDeRelogio.ValueChanged), ValueChanged);
        builder.AddComponentParameter(4, nameof(RvmCampoDeRelogio.ValueExpression), ValueExpression);
        builder.AddComponentParameter(5, nameof(RvmCampoDeRelogio.Use24Hours), Use24Hours);
        builder.AddComponentParameter(6, nameof(RvmCampoDeRelogio.Step), Step ?? 1);
        builder.AddComponentParameter(7, nameof(RvmCampoDeRelogio.Min), Min);
        builder.AddComponentParameter(8, nameof(RvmCampoDeRelogio.Max), Max);
        builder.AddComponentParameter(9, nameof(RvmCampoDeRelogio.Label), Label ?? string.Empty);
        builder.AddComponentParameter(10, nameof(RvmCampoDeRelogio.HelperText), HelperText);
        builder.AddComponentParameter(11, nameof(RvmCampoDeRelogio.ErrorText), ErrorText);
        builder.AddComponentParameter(12, nameof(RvmCampoDeRelogio.Placeholder), Placeholder);
        builder.AddComponentParameter(13, nameof(RvmCampoDeRelogio.Size), Size);
        builder.AddComponentParameter(14, nameof(RvmCampoDeRelogio.Required), Required);
        builder.AddComponentParameter(15, nameof(RvmCampoDeRelogio.Disabled), Disabled);
        builder.AddComponentParameter(16, nameof(RvmCampoDeRelogio.Name), Name);
        builder.CloseComponent();
    };
}
