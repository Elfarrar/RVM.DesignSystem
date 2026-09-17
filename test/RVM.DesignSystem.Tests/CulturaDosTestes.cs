using System.Globalization;
using System.Runtime.CompilerServices;

namespace RVM.DesignSystem.Tests;

/// <summary>
/// Fixa a cultura dos testes em <c>en-US</c>. Sem isto o resultado muda conforme a maquina que roda
/// (ponto decimal, formato de data), e um teste passa aqui e falha no runner sem ninguem entender.
/// </summary>
internal static class CulturaDosTestes
{
    [ModuleInitializer]
    internal static void Inicializar()
    {
        var cultura = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = cultura;
        CultureInfo.DefaultThreadCurrentUICulture = cultura;
    }
}
