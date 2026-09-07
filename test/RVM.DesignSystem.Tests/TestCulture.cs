using System.Globalization;
using System.Runtime.CompilerServices;

namespace RVM.DesignSystem.Tests;

/// <summary>
/// Fixa a cultura do processo de teste em en-US.
/// </summary>
/// <remarks>
/// Padrao do ecossistema: o runner self-hosted e a maquina do Rafael tem culturas
/// diferentes, e teste que passa numa e falha na outra custa mais caro do que essas
/// cinco linhas. Formatacao pt-BR e sempre explicita no codigo de producao, entao
/// fixar en-US aqui nao esconde bug — expoe.
/// </remarks>
internal static class TestCulture
{
    [ModuleInitializer]
    internal static void Initialize()
    {
        var enUs = CultureInfo.GetCultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = enUs;
        CultureInfo.DefaultThreadCurrentUICulture = enUs;
    }
}
