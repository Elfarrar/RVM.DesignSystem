namespace RVM.DesignSystem.Components.EventCalendar;

/// <summary>
/// O que o aplicativo quer mostrar num dia do <see cref="RvmMiniCalendar"/>. O calendario nao busca nada: quem sabe o
/// que acontece no dia e o aplicativo, que devolve este registro (ou <c>null</c> para dia vazio).
/// </summary>
/// <param name="Count">Quantas marcacoes o dia tem (tarefas, eventos). Aparecem como pontos, ate tres.</param>
/// <param name="Color">Cor dos pontos, por papel como em toda a biblioteca.</param>
/// <param name="Badge">Simbolo curto no canto do dia (ex.: fase da lua). Decorativo: o texto vai em <paramref name="Description"/>.</param>
/// <param name="Description">O que o leitor de tela ouve depois da data (ex.: "2 aplicacoes").</param>
/// <param name="Tooltip">Balao ao passar o mouse ou focar o dia. Sem ele, o dia nao tem balao.</param>
public sealed record RvmMiniCalendarDay(
    int Count = 0,
    RvmColor Color = RvmColor.Accent,
    string? Badge = null,
    string? Description = null,
    string? Tooltip = null);
