using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.TextField;

namespace RVM.DesignSystem.Components.TimePicker;

/// <summary>
/// Campo de horario: uma lista de horarios em intervalos fixos, em 24 h (padrao) ou 12 h.
/// </summary>
public partial class RvmTimePicker : ComponentBase
{
    /// <summary>O horario escolhido. Aceita <c>@bind-Value</c>.</summary>
    [Parameter] public TimeOnly? Value { get; set; }

    /// <summary>Disparado quando o horario muda.</summary>
    [Parameter] public EventCallback<TimeOnly?> ValueChanged { get; set; }

    /// <summary>Expressao do valor ligado. O <c>@bind-Value</c> preenche sozinho.</summary>
    [Parameter] public Expression<Func<TimeOnly?>>? ValueExpression { get; set; }

    /// <summary>Minutos entre um horario e o proximo. Padrao: 30.</summary>
    [Parameter] public int Step { get; set; } = 30;

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

    /// <summary>Estilo do campo. Padrao: contorno.</summary>
    [Parameter] public RvmTextFieldVariant Variant { get; set; } = RvmTextFieldVariant.Outlined;

    /// <summary>56 px (padrao) ou 40 px.</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>Busca no topo da lista: digitar "18" acha 18:00 e 18:30. Padrao: sim.</summary>
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
            var passo = Math.Clamp(Step, 1, 720);
            var lista = new List<TimeOnly?>();
            for (var minutos = Min.Hour * 60 + Min.Minute; minutos <= Max.Hour * 60 + Max.Minute; minutos += passo)
            {
                lista.Add(new TimeOnly(minutos / 60, minutos % 60));
            }

            return lista;
        }
    }

    internal string Texto(TimeOnly? horario)
    {
        if (horario is not { } h)
        {
            return string.Empty;
        }

        if (Use24Hours)
        {
            return $"{h.Hour:00}:{h.Minute:00}";
        }

        var hora12 = h.Hour % 12 == 0 ? 12 : h.Hour % 12;
        return $"{hora12}:{h.Minute:00} {(h.Hour < 12 ? "AM" : "PM")}";
    }
}
