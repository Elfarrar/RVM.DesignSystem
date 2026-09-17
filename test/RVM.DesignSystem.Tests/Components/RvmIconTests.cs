using Bunit;
using RVM.DesignSystem;
using RVM.DesignSystem.Components.Icon;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

public class RvmIconTests : BunitContext
{
    [Fact]
    public void Desenha_um_svg_que_herda_a_cor_de_quem_o_usa()
    {
        var cortado = Render<RvmIcon>(p => p.Add(x => x.Name, RvmIconName.Check));

        var svg = cortado.Find("svg");
        Assert.Equal("0 0 24 24", svg.GetAttribute("viewBox"));
        Assert.Equal("currentColor", svg.GetAttribute("stroke"));
        Assert.NotEmpty(cortado.FindAll("path"));
    }

    [Theory]
    [InlineData(RvmSize.Small, "16")]
    [InlineData(RvmSize.Medium, "20")]
    [InlineData(RvmSize.Large, "24")]
    public void O_tamanho_vira_lado_em_pixels(RvmSize tamanho, string lado)
    {
        var cortado = Render<RvmIcon>(p => p
            .Add(x => x.Name, RvmIconName.Check)
            .Add(x => x.Size, tamanho));

        var svg = cortado.Find("svg");
        Assert.Equal(lado, svg.GetAttribute("width"));
        Assert.Equal(lado, svg.GetAttribute("height"));
    }

    [Fact]
    public void Sem_titulo_o_icone_e_escondido_do_leitor_de_tela()
    {
        // O caso comum e o icone ao lado de um texto que diz a mesma coisa. Anunciar os dois faz o
        // leitor de tela repetir a informacao.
        var cortado = Render<RvmIcon>(p => p.Add(x => x.Name, RvmIconName.Check));

        var svg = cortado.Find("svg");
        Assert.Equal("true", svg.GetAttribute("aria-hidden"));
        Assert.False(svg.HasAttribute("role"));
        Assert.Empty(cortado.FindAll("title"));
    }

    [Fact]
    public void Com_titulo_o_icone_vira_imagem_com_nome_acessivel()
    {
        var cortado = Render<RvmIcon>(p => p
            .Add(x => x.Name, RvmIconName.Trash)
            .Add(x => x.Title, "Excluir"));

        var svg = cortado.Find("svg");
        Assert.Equal("img", svg.GetAttribute("role"));
        Assert.False(svg.HasAttribute("aria-hidden"));
        Assert.Equal("Excluir", cortado.Find("title").TextContent);
    }

    [Fact]
    public void Papel_de_cor_vira_token()
    {
        var cortado = Render<RvmIcon>(p => p
            .Add(x => x.Name, RvmIconName.AlertTriangle)
            .Add(x => x.Color, RvmColor.Warning));

        Assert.Contains("var(--rvm-color-warning-text)", cortado.Find("svg").GetAttribute("style"));
    }

    [Fact]
    public void Sem_papel_de_cor_nao_ha_style()
    {
        var cortado = Render<RvmIcon>(p => p.Add(x => x.Name, RvmIconName.Check));

        Assert.True(string.IsNullOrEmpty(cortado.Find("svg").GetAttribute("style")));
    }

    [Fact]
    public void Todo_nome_do_enum_tem_desenho_no_catalogo()
    {
        // Enum e catalogo sao gerados juntos a partir dos SVG do Tabler. Se alguem acrescentar um
        // nome a mao e esquecer o desenho, o componente estoura em tempo de execucao — este teste
        // troca isso por uma falha na hora de compilar a suite.
        var semDesenho = Enum.GetValues<RvmIconName>()
            .Where(nome => Render<RvmIcon>(p => p.Add(x => x.Name, nome)).FindAll("path").Count == 0)
            .ToArray();

        Assert.Empty(semDesenho);
    }

    [Fact]
    public void Atributos_extras_e_classe_chegam_ao_svg()
    {
        var cortado = Render<RvmIcon>(p => p
            .Add(x => x.Name, RvmIconName.Check)
            .AddUnmatched("class", "minha")
            .AddUnmatched("data-teste", "1"));

        var svg = cortado.Find("svg");
        Assert.Equal("rvm-icone minha", svg.GetAttribute("class"));
        Assert.Equal("1", svg.GetAttribute("data-teste"));
    }
}
