namespace RVM.DesignSystem.Components.EventCalendar;

/// <summary>
/// Datas escritas em PT-BR por tabela propria, e nao pelo <c>CultureInfo</c>: no WebAssembly a cultura pt-BR pode nao
/// estar carregada (sem ICU), e o mes sairia em ingles. Mesma escolha do <c>RvmCalendar</c>.
/// </summary>
internal static class DatasPorExtenso
{
    private static readonly string[] Meses =
        ["janeiro", "fevereiro", "marco", "abril", "maio", "junho", "julho", "agosto", "setembro", "outubro", "novembro", "dezembro"];

    private static readonly string[] Dias =
        ["domingo", "segunda-feira", "terca-feira", "quarta-feira", "quinta-feira", "sexta-feira", "sabado"];

    private static readonly string[] DiasCurtos = ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sab"];

    /// <summary>Domingo a sabado, a ordem das colunas.</summary>
    public static readonly DayOfWeek[] Semana =
    [
        DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
        DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday,
    ];

    public static string Mes(int mes) => Meses[mes - 1];

    public static string Dia(DayOfWeek dia) => Dias[(int)dia];

    public static string DiaCurto(DayOfWeek dia) => DiasCurtos[(int)dia];

    /// <summary>"Janeiro de 2026".</summary>
    public static string TituloDoMes(DateOnly d) => Maiuscula($"{Mes(d.Month)} de {d.Year}");

    /// <summary>"12 de janeiro de 2026".</summary>
    public static string Data(DateOnly d) => $"{d.Day} de {Mes(d.Month)} de {d.Year}";

    /// <summary>"quarta-feira, 12 de janeiro de 2026".</summary>
    public static string Completa(DateOnly d) => $"{Dia(d.DayOfWeek)}, {Data(d)}";

    /// <summary>"9 a 15 de janeiro de 2026", sem repetir o que as duas pontas tem em comum.</summary>
    public static string Intervalo(DateOnly inicio, DateOnly fim)
    {
        if (inicio.Year != fim.Year)
        {
            return $"{Data(inicio)} a {Data(fim)}";
        }

        return inicio.Month == fim.Month
            ? $"{inicio.Day} a {Data(fim)}"
            : $"{inicio.Day} de {Mes(inicio.Month)} a {Data(fim)}";
    }

    public static string Maiuscula(string texto) => texto.Length == 0 ? texto : char.ToUpperInvariant(texto[0]) + texto[1..];
}
