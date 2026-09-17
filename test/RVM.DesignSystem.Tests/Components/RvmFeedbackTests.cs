using Bunit;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem;
using RVM.DesignSystem.Components.EmptyState;
using RVM.DesignSystem.Components.Progress;
using RVM.DesignSystem.Components.Skeleton;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

public class RvmProgressTests : BunitContext
{
    [Fact]
    public void Sem_valor_e_uma_barra_indeterminada_sem_aria_valuenow()
    {
        var cortado = Render<RvmProgress>();

        var raiz = cortado.Find("[role=progressbar]");
        Assert.Equal("progresso linear indeterminado medio primary", raiz.GetAttribute("class"));
        Assert.Equal("Carregando", raiz.GetAttribute("aria-label"));
        Assert.Null(raiz.GetAttribute("aria-valuenow"));
        Assert.Null(raiz.GetAttribute("aria-valuemin"));
        Assert.Null(cortado.Find(".barra").GetAttribute("style"));
    }

    [Theory]
    [InlineData(42.4, "42", "width: 42.4%")]
    [InlineData(-5, "0", "width: 0%")]
    [InlineData(130, "100", "width: 100%")]
    public void Com_valor_anuncia_a_porcentagem_limitada_a_0_100(double valor, string anunciado, string estilo)
    {
        var cortado = Render<RvmProgress>(p => p.Add(x => x.Value, valor).Add(x => x.Label, "Enviando planilha"));

        var raiz = cortado.Find("[role=progressbar]");
        Assert.Contains("determinado", raiz.GetAttribute("class"));
        Assert.Equal("0", raiz.GetAttribute("aria-valuemin"));
        Assert.Equal("100", raiz.GetAttribute("aria-valuemax"));
        Assert.Equal(anunciado, raiz.GetAttribute("aria-valuenow"));
        Assert.Equal("Enviando planilha", raiz.GetAttribute("aria-label"));
        Assert.Equal(estilo, cortado.Find(".barra").GetAttribute("style"));
    }

    [Fact]
    public void Mostrar_valor_so_no_determinado_e_escondido_do_leitor()
    {
        var determinado = Render<RvmProgress>(p => p.Add(x => x.Value, 70).Add(x => x.ShowValue, true));
        var valor = determinado.Find(".valor");
        Assert.Equal("70%", valor.TextContent);
        Assert.Equal("true", valor.GetAttribute("aria-hidden"));

        var indeterminado = Render<RvmProgress>(p => p.Add(x => x.ShowValue, true));
        Assert.Empty(indeterminado.FindAll(".valor"));
    }

    [Fact]
    public void Anel_determinado_desenha_o_arco_pelo_deslocamento()
    {
        var cortado = Render<RvmProgress>(p => p.Add(x => x.Variant, RvmProgressVariant.Circular).Add(x => x.Value, 25));

        Assert.Contains("circular", cortado.Find("[role=progressbar]").GetAttribute("class"));
        Assert.Equal("true", cortado.Find("svg").GetAttribute("aria-hidden"));
        // Circunferencia de 2 * pi * 20.2 = 126.92; faltando 75%, o deslocamento e 95.19.
        Assert.Equal("stroke-dasharray: 126.92; stroke-dashoffset: 95.19", cortado.Find(".arco").GetAttribute("style"));
    }

    [Fact]
    public void Anel_sai_com_ponto_decimal_mesmo_em_pt_br()
    {
        // Os testes rodam em en-US; o site, em pt-BR. Com @Raio no markup saia r="20,2" e o anel sumia.
        var anterior = System.Globalization.CultureInfo.CurrentCulture;
        System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
        try
        {
            var cortado = Render<RvmProgress>(p => p.Add(x => x.Variant, RvmProgressVariant.Circular).Add(x => x.Value, 25));

            var arco = cortado.Find(".arco");
            Assert.Equal("20.2", arco.GetAttribute("r"));
            Assert.Equal("3.6", arco.GetAttribute("stroke-width"));
            Assert.Equal("20.2", cortado.Find(".trilho-anel").GetAttribute("r"));
            Assert.Equal("stroke-dasharray: 126.92; stroke-dashoffset: 95.19", arco.GetAttribute("style"));
        }
        finally
        {
            System.Globalization.CultureInfo.CurrentCulture = anterior;
        }
    }

    [Fact]
    public void Anel_indeterminado_nao_tem_estilo_de_arco()
    {
        var cortado = Render<RvmProgress>(p => p.Add(x => x.Variant, RvmProgressVariant.Circular));

        Assert.Null(cortado.Find(".arco").GetAttribute("style"));
    }

    [Theory]
    [InlineData(RvmSize.Small, RvmColor.Secondary, "pequeno secondary")]
    [InlineData(RvmSize.Large, RvmColor.Info, "grande info")]
    [InlineData(RvmSize.Medium, RvmColor.Success, "medio success")]
    [InlineData(RvmSize.Medium, RvmColor.Warning, "medio warning")]
    [InlineData(RvmSize.Medium, RvmColor.Error, "medio error")]
    public void Tamanho_e_cor_viram_classes(RvmSize tamanho, RvmColor cor, string esperado)
    {
        var cortado = Render<RvmProgress>(p => p.Add(x => x.Size, tamanho).Add(x => x.Color, cor).AddUnmatched("class", "minha"));

        Assert.EndsWith($"{esperado} minha", cortado.Find("[role=progressbar]").GetAttribute("class"));
    }
}

public class RvmSkeletonTests : BunitContext
{
    [Fact]
    public void Padrao_e_uma_linha_de_texto_animada_e_escondida_do_leitor()
    {
        var cortado = Render<RvmSkeleton>();

        var raiz = cortado.Find("span");
        Assert.Equal("esqueleto texto animado", raiz.GetAttribute("class"));
        Assert.Equal("true", raiz.GetAttribute("aria-hidden"));
        Assert.Equal(string.Empty, raiz.GetAttribute("style"));
    }

    [Fact]
    public void Retangulo_tem_altura_padrao_e_aceita_medidas()
    {
        var padrao = Render<RvmSkeleton>(p => p.Add(x => x.Variant, RvmSkeletonVariant.Rectangular));
        Assert.Equal("height: 120px", padrao.Find("span").GetAttribute("style"));

        var medido = Render<RvmSkeleton>(p => p
            .Add(x => x.Variant, RvmSkeletonVariant.Rectangular)
            .Add(x => x.Width, "50%").Add(x => x.Height, "3rem")
            .Add(x => x.Animated, false));
        Assert.Equal("width: 50%; height: 3rem", medido.Find("span").GetAttribute("style"));
        Assert.Equal("esqueleto retangulo", medido.Find("span").GetAttribute("class"));
    }

    [Fact]
    public void Circulo_usa_a_largura_como_altura()
    {
        var padrao = Render<RvmSkeleton>(p => p.Add(x => x.Variant, RvmSkeletonVariant.Circular));
        Assert.Equal("width: 40px; height: 40px", padrao.Find("span").GetAttribute("style"));

        var medido = Render<RvmSkeleton>(p => p.Add(x => x.Variant, RvmSkeletonVariant.Circular).Add(x => x.Width, "56px"));
        Assert.Equal("width: 56px; height: 56px", medido.Find("span").GetAttribute("style"));
    }

    [Fact]
    public void Classe_e_estilo_do_consumidor_se_somam()
    {
        var cortado = Render<RvmSkeleton>(p => p
            .Add(x => x.Width, "10rem")
            .AddUnmatched("class", "minha")
            .AddUnmatched("style", "margin-top: 8px;"));

        var raiz = cortado.Find("span");
        Assert.Equal("esqueleto texto animado minha", raiz.GetAttribute("class"));
        Assert.Equal("width: 10rem; margin-top: 8px", raiz.GetAttribute("style"));
    }
}

public class RvmEmptyStateTests : BunitContext
{
    [Fact]
    public void Secao_nomeada_pelo_titulo_com_descricao()
    {
        var cortado = Render<RvmEmptyState>(p => p
            .Add(x => x.Title, "Nenhum talhao cadastrado ainda")
            .Add(x => x.Description, "Cadastre o primeiro para acompanhar a safra."));

        var secao = cortado.Find("section.vazio");
        var titulo = cortado.Find("h2.titulo");
        Assert.Equal("Nenhum talhao cadastrado ainda", titulo.TextContent);
        Assert.Equal(titulo.Id, secao.GetAttribute("aria-labelledby"));
        Assert.Equal("Cadastre o primeiro para acompanhar a safra.", cortado.Find("p.descricao").TextContent);
        Assert.Empty(cortado.FindAll(".icone, .ilustracao, .acoes, .detalhe"));
    }

    [Theory]
    [InlineData(1, "h2")]
    [InlineData(3, "h3")]
    [InlineData(4, "h4")]
    [InlineData(5, "h5")]
    [InlineData(9, "h6")]
    public void Nivel_do_titulo_fica_entre_h2_e_h6(int nivel, string elemento)
    {
        var cortado = Render<RvmEmptyState>(p => p.Add(x => x.Title, "Vazio").Add(x => x.HeadingLevel, nivel));

        Assert.Equal("Vazio", cortado.Find($"{elemento}.titulo").TextContent);
    }

    [Fact]
    public void Titulo_recebe_o_escopo_do_css_isolado()
    {
        var cortado = Render<RvmEmptyState>(p => p.Add(x => x.Title, "Vazio").Add(x => x.HeadingLevel, 5));

        Assert.Contains(cortado.Find("h5.titulo").Attributes, a => a.Name.StartsWith("b-", StringComparison.Ordinal));
    }

    [Fact]
    public void Icone_decorativo_e_acoes()
    {
        var cortado = Render<RvmEmptyState>(p => p
            .Add(x => x.Title, "Sem resultados")
            .Add(x => x.Icon, RvmIconName.Search)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "<small>dica</small>")))
            .Add(x => x.Actions, (RenderFragment)(b => b.AddMarkupContent(0, "<button>Limpar filtros</button>")))
            .AddUnmatched("class", "minha"));

        Assert.Equal("true", cortado.Find(".icone").GetAttribute("aria-hidden"));
        Assert.Equal("Limpar filtros", cortado.Find(".acoes button").TextContent);
        Assert.Equal("dica", cortado.Find(".detalhe small").TextContent);
        Assert.Equal("vazio minha", cortado.Find("section").GetAttribute("class"));
    }

    [Fact]
    public void Ilustracao_vence_o_icone()
    {
        var cortado = Render<RvmEmptyState>(p => p
            .Add(x => x.Title, "Pagina nao encontrada")
            .Add(x => x.Icon, RvmIconName.Search)
            .Add(x => x.Illustration, (RenderFragment)(b => b.AddMarkupContent(0, "<img src=\"x.svg\" alt=\"\" />"))));

        Assert.Equal("true", cortado.Find(".ilustracao").GetAttribute("aria-hidden"));
        Assert.Empty(cortado.FindAll(".icone"));
    }
}
