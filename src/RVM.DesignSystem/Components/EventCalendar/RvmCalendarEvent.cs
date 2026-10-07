namespace RVM.DesignSystem.Components.EventCalendar;

/// <summary>Um compromisso na agenda do <see cref="RvmEventCalendar"/> (contrato com o RVM.UI, DSGN-017).</summary>
/// <remarks>
/// A <see cref="Color"/> e a CATEGORIA do compromisso ("plantio", "visita tecnica"), nao um estado. Entra por papel,
/// como em toda a biblioteca, e o tema decide a cor.
/// </remarks>
public sealed record RvmCalendarEvent
{
    /// <summary>Titulo do compromisso. E o que o leitor de tela anuncia.</summary>
    public required string Title { get; init; }

    /// <summary>Inicio, com hora. Na visao de mes so a data e usada.</summary>
    public required DateTime Start { get; init; }

    /// <summary>Fim. Sem ele o compromisso dura uma hora.</summary>
    public DateTime? End { get; init; }

    /// <summary>Categoria. Vira a cor da pilula. Padrao: <see cref="RvmColor.Accent"/>.</summary>
    public RvmColor Color { get; init; } = RvmColor.Accent;

    /// <summary>Foto de quem participa, a esquerda do titulo. Decorativa: o titulo ja descreve.</summary>
    public string? ImageUrl { get; init; }

    /// <summary>Fim efetivo: uma hora depois do inicio quando <see cref="End"/> nao vem.</summary>
    public DateTime EndOrDefault => End ?? Start.AddHours(1);
}
