using Bunit;
using RVM.DesignSystem;
using RVM.DesignSystem.Components.Divider;

namespace RVM.DesignSystem.Tests.Components;

public class RvmDividerTests : BunitContext
{
    [Fact]
    public void Sem_rotulo_sai_um_hr_deitado()
    {
        var cortado = Render<RvmDivider>();

        var hr = cortado.Find("hr");
        Assert.Contains("horizontal", hr.GetAttribute("class"));
        Assert.Equal("horizontal", hr.GetAttribute("aria-orientation"));
    }

    [Fact]
    public void Em_pe_muda_a_classe_e_o_aria_orientation()
    {
        var cortado = Render<RvmDivider>(p => p.Add(x => x.Orientation, RvmOrientation.Vertical));

        var hr = cortado.Find("hr");
        Assert.Contains("vertical", hr.GetAttribute("class"));
        Assert.Equal("vertical", hr.GetAttribute("aria-orientation"));
    }

    [Fact]
    public void Com_rotulo_o_hr_da_lugar_a_um_separador_com_papel_explicito()
    {
        // <hr> e elemento vazio: nao aceita conteudo. Com rotulo, o papel de separador precisa ir
        // no atributo, senao o leitor de tela perde a informacao de que ali separa dois blocos.
        var cortado = Render<RvmDivider>(p => p.AddChildContent("ou"));

        Assert.Empty(cortado.FindAll("hr"));
        var separador = cortado.Find("div");
        Assert.Equal("separator", separador.GetAttribute("role"));
        Assert.Equal("horizontal", separador.GetAttribute("aria-orientation"));
        Assert.Equal("ou", cortado.Find("span").TextContent);
    }

    [Fact]
    public void Classe_do_consumidor_soma_com_as_do_componente()
    {
        var cortado = Render<RvmDivider>(p => p.AddUnmatched("class", "minha"));

        var classe = cortado.Find("hr").GetAttribute("class");
        Assert.Contains("divisor", classe);
        Assert.Contains("horizontal", classe);
        Assert.Contains("minha", classe);
    }

    [Fact]
    public void Atributos_extras_chegam_ao_elemento_raiz()
    {
        var cortado = Render<RvmDivider>(p => p.AddUnmatched("data-teste", "1"));

        Assert.Equal("1", cortado.Find("hr").GetAttribute("data-teste"));
    }
}
