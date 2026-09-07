using RVM.DesignSystem.Formatting;

namespace RVM.DesignSystem.Tests.Formatting;

public class BrlFormatterTests
{
    [Theory]
    [InlineData(1234.56, "1.234,56")]
    [InlineData(0, "0,00")]
    [InlineData(-9.9, "9,90")]
    [InlineData(1000000, "1.000.000,00")]
    public void Format_usa_separadores_ptBR_independente_da_cultura_do_processo(decimal value, string esperado)
    {
        // A cultura do processo esta fixada em en-US pelo TestCulture: se o formatter
        // dependesse dela, estes casos falhariam com ponto no lugar da virgula.
        var resultado = BrlFormatter.Format(value);

        Assert.Contains(esperado, resultado);
        Assert.Contains("R$", resultado);
    }

    [Fact]
    public void FormatWithoutSymbol_omite_o_simbolo()
    {
        Assert.Equal("1.234,56", BrlFormatter.FormatWithoutSymbol(1234.56m));
    }

    [Fact]
    public void Format_nulo_devolve_o_texto_vazio_padrao()
    {
        Assert.Equal("—", BrlFormatter.Format((decimal?)null));
    }

    [Fact]
    public void Format_nulo_aceita_texto_vazio_customizado()
    {
        Assert.Equal("sem valor", BrlFormatter.Format(null, "sem valor"));
    }

    [Fact]
    public void Format_opcional_com_valor_formata_normalmente()
    {
        Assert.Equal(BrlFormatter.Format(10m), BrlFormatter.Format((decimal?)10m));
    }
}
