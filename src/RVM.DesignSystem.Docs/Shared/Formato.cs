using System.Globalization;

namespace RVM.DesignSystem.Docs.Shared;

/// <summary>
/// Formatos das telas de exemplo em PT-BR fixo. No WebAssembly a cultura segue o idioma do navegador e
/// so os dados dele sao carregados: "C0" sairia "$184,500" num navegador em ingles.
/// </summary>
public static class Formato
{
    public static string Moeda(decimal valor)
        => "R$ " + Numero(valor);

    public static string Numero(decimal valor)
        => valor.ToString("#,0", CultureInfo.InvariantCulture).Replace(',', '.');

    public static string Data(DateOnly data)
        => $"{data.Day:00}/{data.Month:00}/{data.Year:0000}";

    public static string DiaMes(DateOnly data)
        => $"{data.Day:00}/{data.Month:00}";

    public static string Hora(TimeOnly hora)
        => $"{hora.Hour:00}:{hora.Minute:00}";
}
