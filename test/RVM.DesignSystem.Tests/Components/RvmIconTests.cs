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
            .Where(nome => Render<RvmIcon>(p => p.Add(x => x.Name, nome)).Find("svg").ChildElementCount == 0)
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

    [Fact]
    public void Nome_do_contrato_desenha_o_Tabler_do_mapa()
    {
        // AltArrowDown (Solar, do RVM.UI) e desenhado com o chevron-down do Tabler — o mesmo traco do ChevronDown.
        var solar = Render<RvmIcon>(p => p.Add(x => x.Name, RvmIconName.AltArrowDown)).Find("svg").InnerHtml;
        var tabler = Render<RvmIcon>(p => p.Add(x => x.Name, RvmIconName.ChevronDown)).Find("svg").InnerHtml;

        Assert.Equal(tabler, solar);
    }

    [Theory]
    [InlineData(RvmIconStyle.Bold)]
    [InlineData(RvmIconStyle.BoldDuotone)]
    public void Estilo_cheio_usa_o_desenho_cheio_do_Tabler(RvmIconStyle estilo)
    {
        var cortado = Render<RvmIcon>(p => p.Add(x => x.Name, RvmIconName.Heart).Add(x => x.Style, estilo));

        Assert.Equal("currentColor", cortado.Find("svg > g").GetAttribute("fill"));
    }

    [Fact]
    public void Sem_versao_cheia_o_estilo_cheio_cai_no_traco()
    {
        // O x do Tabler tem versao cheia; o minus, nao.
        var cheio = Render<RvmIcon>(p => p.Add(x => x.Name, RvmIconName.Minus).Add(x => x.Style, RvmIconStyle.Bold));
        var traco = Render<RvmIcon>(p => p.Add(x => x.Name, RvmIconName.Minus));

        Assert.Equal(traco.Find("svg").InnerHtml, cheio.Find("svg").InnerHtml);
    }

    [Fact]
    public void SizePx_vence_o_tamanho()
    {
        var cortado = Render<RvmIcon>(p => p.Add(x => x.Name, RvmIconName.Bell).Add(x => x.Size, RvmSize.Small).Add(x => x.SizePx, 32));

        Assert.Equal("32", cortado.Find("svg").GetAttribute("width"));
    }

    [Fact]
    public void Class_do_contrato_chega_ao_svg()
    {
        var cortado = Render<RvmIcon>(p => p.Add(x => x.Name, RvmIconName.Check).Add(x => x.Class, "minha"));

        Assert.Equal("rvm-icone minha", cortado.Find("svg").GetAttribute("class"));
    }
}
