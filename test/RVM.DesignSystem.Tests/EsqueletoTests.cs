using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Tests;

/// <summary>
/// Prova que o arreio de teste (bUnit + xUnit + cultura fixa) renderiza de verdade. Enquanto a
/// biblioteca nao tem componente — os nove primeiros chegam na <c>DSGN-002</c> — e este teste que
/// segura o pipeline de pe: se o bUnit parar de funcionar, o CI avisa aqui, e nao no meio da onda 1.
/// </summary>
public class EsqueletoTests : BunitContext
{
    [Fact]
    public void BUnit_renderiza_marcacao()
    {
        var cortado = Render(builder =>
        {
            builder.OpenElement(0, "p");
            builder.AddContent(1, "RVM Design System");
            builder.CloseElement();
        });

        cortado.MarkupMatches("<p>RVM Design System</p>");
    }

    [Fact]
    public void Cultura_dos_testes_e_invariavel_entre_maquinas()
    {
        Assert.Equal("en-US", System.Globalization.CultureInfo.CurrentCulture.Name);
        Assert.Equal("1.5", 1.5.ToString());
    }
}
