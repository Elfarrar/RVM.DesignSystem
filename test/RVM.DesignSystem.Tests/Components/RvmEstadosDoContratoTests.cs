using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Chart;
using RVM.DesignSystem.Components.EmptyState;
using RVM.DesignSystem.Components.Lists;
using RVM.DesignSystem.Components.Mascot;
using RVM.DesignSystem.Components.Table;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>Mascote e os estados carregando, erro e vazio do contrato com o RVM.UI (DSGN-017).</summary>
public class RvmEstadosDoContratoTests : BunitContext
{
    public RvmEstadosDoContratoTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Mascote_e_imagem_com_texto_alternativo_padrao()
    {
        var raiz = Render<RvmMascot>(p => p.Add(x => x.Name, RvmMascotName.RobotError)).Find("span");

        Assert.Equal("img", raiz.GetAttribute("role"));
        Assert.Equal("Ilustracao: algo deu errado", raiz.GetAttribute("aria-label"));
        Assert.Contains("rvm-error", raiz.ClassList);
        Assert.Contains("--rvm-mascote-lado: 160px", raiz.GetAttribute("style"));
    }

    [Fact]
    public void Mascote_decorativo_some_do_leitor_de_tela_e_Alt_troca_o_texto()
    {
        var decorativo = Render<RvmMascot>(p => p.Add(x => x.Name, RvmMascotName.DogEmptyState).Add(x => x.Decorative, true)).Find("span");
        var comAlt = Render<RvmMascot>(p => p.Add(x => x.Name, RvmMascotName.DogEmptyState).Add(x => x.Alt, "Nenhum pedido neste mes")).Find("span");

        Assert.Equal("true", decorativo.GetAttribute("aria-hidden"));
        Assert.Null(decorativo.GetAttribute("role"));
        Assert.Equal("Nenhum pedido neste mes", comAlt.GetAttribute("aria-label"));
    }

    [Fact]
    public void Todo_mascote_tem_desenho()
    {
        foreach (var nome in Enum.GetValues<RvmMascotName>())
        {
            Assert.NotEmpty(Render<RvmMascot>(p => p.Add(x => x.Name, nome)).FindAll("svg path, svg circle, svg g"));
        }
    }

    [Fact]
    public void Diametro_invalido_e_erro_de_programacao()
        => Assert.Throws<ArgumentOutOfRangeException>(() => Render<RvmMascot>(p => p.Add(x => x.Name, RvmMascotName.RobotError).Add(x => x.Width, 0)));

    [Fact]
    public void Vazio_com_mascote_cor_e_status()
    {
        var cortado = Render<RvmEmptyState>(p => p
            .Add(x => x.Title, "Nada")
            .Add(x => x.Mascot, RvmMascotName.DogSearch)
            .Add(x => x.Color, RvmColor.Error)
            .Add(x => x.Status, true));

        var raiz = cortado.Find("section");
        Assert.Equal("status", raiz.GetAttribute("role"));
        Assert.Contains("rvm-error", raiz.ClassList);
        Assert.Equal("true", cortado.Find(".rvm-mascote").GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Lista_mostra_erro_antes_de_carregando_e_vazio()
    {
        var cortado = Render<RvmList>(p => p.Add(x => x.Error, true).Add(x => x.Loading, true).Add(x => x.IsEmpty, true));

        Assert.Empty(cortado.FindAll("ul"));
        Assert.Contains("Nao deu para carregar", cortado.Find("section").TextContent);
        Assert.Equal("status", cortado.Find("section").GetAttribute("role"));
    }

    [Fact]
    public void Lista_carregando_anuncia_o_texto()
    {
        var cortado = Render<RvmList>(p => p.Add(x => x.Loading, true).Add(x => x.LoadingText, "Buscando contas"));

        var aviso = cortado.Find("[role=status]");
        Assert.Contains("Buscando contas", aviso.TextContent);
        Assert.NotNull(cortado.Find("[role=progressbar]"));
    }

    [Fact]
    public void Lista_vazia_usa_o_Empty_do_consumidor()
    {
        var cortado = Render<RvmList>(p => p.Add(x => x.IsEmpty, true).Add(x => x.Empty, (RenderFragment)(b => b.AddMarkupContent(0, "<p class=\"meu-vazio\">Cadastre a primeira</p>"))));

        Assert.NotNull(cortado.Find(".meu-vazio"));
    }

    [Fact]
    public void Lista_com_altura_maxima_rola_e_alcanca_pelo_teclado()
    {
        var ul = Render<RvmList>(p => p.Add(x => x.MaxHeight, "320px")).Find("ul");

        Assert.Contains("max-height: 320px", ul.GetAttribute("style"));
        Assert.Equal("0", ul.GetAttribute("tabindex"));
    }

    private IRenderedComponent<RvmTable<string>> Tabela(Action<ComponentParameterCollectionBuilder<RvmTable<string>>> extra, IEnumerable<string>? itens = null)
        => Render<RvmTable<string>>(p =>
        {
            p.Add(x => x.Items, itens ?? ["Alfa"]).Add(x => x.Caption, "Nomes")
             .AddChildContent<RvmTableColumn<string>>(c => c.Add(x => x.Title, "Nome").Add(x => x.Value, s => s));
            extra(p);
        });

    [Fact]
    public void Tabela_carregando_troca_as_linhas_pelo_aviso_e_esconde_a_paginacao()
    {
        var cortado = Tabela(p => p.Add(x => x.Loading, true).Add(x => x.LoadingMascot, RvmMascotName.RobotLoading));

        Assert.Empty(cortado.FindAll("tr.rvm-linha"));
        Assert.NotNull(cortado.Find("tr.rvm-linha-estado .rvm-mascote"));
        Assert.Contains("Carregando...", cortado.Find("tr.rvm-linha-estado").TextContent);
    }

    [Fact]
    public void Tabela_com_erro_usa_o_conteudo_do_consumidor()
    {
        var cortado = Tabela(p => p.Add(x => x.Error, true).Add(x => x.ErrorContent, (RenderFragment)(b => b.AddMarkupContent(0, "<p class=\"meu-erro\">Falhou</p>"))));

        Assert.NotNull(cortado.Find("tr.rvm-linha-estado .meu-erro"));
    }

    [Fact]
    public void Tabela_sem_itens_usa_o_Empty_e_sem_ele_a_linha_de_sempre()
    {
        var comEmpty = Tabela(p => p.Add(x => x.Empty, (RenderFragment)(b => b.AddMarkupContent(0, "<p class=\"meu-vazio\">Nada</p>"))), []);
        var semEmpty = Tabela(_ => { }, []);

        Assert.NotNull(comEmpty.Find(".meu-vazio"));
        Assert.Contains("Nenhum registro para mostrar.", semEmpty.Find("tr.rvm-linha-vazia").TextContent);
    }

    [Fact]
    public void Tabela_com_titulo_e_acoes_acima()
    {
        var cortado = Tabela(p => p
            .Add(x => x.Header, (RenderFragment)(b => b.AddMarkupContent(0, "<h2>Contas</h2>")))
            .Add(x => x.Toolbar, (RenderFragment)(b => b.AddMarkupContent(0, "<button>Exportar</button>"))));

        Assert.Equal("Contas", cortado.Find(".rvm-titulo-tabela h2").TextContent);
        Assert.Equal("Exportar", cortado.Find(".rvm-acoes-tabela button").TextContent);
    }

    [Fact]
    public void Coluna_formata_na_cultura_dela()
    {
        var cortado = Render<RvmTable<decimal>>(p => p
            .Add(x => x.Items, [1234.5m]).Add(x => x.Caption, "Valores")
            .AddChildContent<RvmTableColumn<decimal>>(c => c.Add(x => x.Title, "Valor").Add(x => x.Value, v => v).Add(x => x.Format, "N2").Add(x => x.Culture, new CultureInfo("pt-BR"))));

        Assert.Equal("1.234,50", cortado.Find("tr.rvm-linha td").TextContent.Trim());
    }

    private IRenderedComponent<RvmLineChart<Venda>> Linha(Action<ComponentParameterCollectionBuilder<RvmLineChart<Venda>>> extra)
        => Render<RvmLineChart<Venda>>(p =>
        {
            p.Add(x => x.Items, [new Venda("Jan", 1000, 0), new Venda("Fev", 3000, 0)]).Add(x => x.Label, v => v.Mes).Add(x => x.AriaLabel, "Vendas")
             .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Receita").Add(x => x.Value, v => v.Receita));
            extra(p);
        });

    [Fact]
    public void Grafico_carregando_troca_o_desenho_pelo_aviso_na_mesma_altura()
    {
        var cortado = Linha(p => p.Add(x => x.Loading, true).Add(x => x.Height, 240));

        Assert.Empty(cortado.FindAll("svg.rvm-grafico-svg"));
        Assert.Contains("min-height: 240px", cortado.Find(".rvm-grafico-estado").GetAttribute("style"));
        Assert.Contains("Carregando o grafico...", cortado.Find("[role=status]").TextContent);
    }

    [Fact]
    public void Grafico_vazio_e_com_erro()
    {
        Assert.Contains("Ainda nao ha dado para este grafico.", Linha(p => p.Add(x => x.IsEmpty, true)).Find("figure").TextContent);
        Assert.Contains("Nao deu para carregar o grafico", Linha(p => p.Add(x => x.Error, true).Add(x => x.ErrorMascot, RvmMascotName.RobotError)).Find("figure").TextContent);
    }

    [Fact]
    public void Formato_do_eixo_vale_so_no_eixo()
    {
        var cortado = Linha(p => p.Add(x => x.FormatAxisValue, v => $"{v / 1000}k").Add(x => x.ValueFormat, v => $"R$ {v}"));

        var textos = cortado.FindAll("svg text").Select(t => t.TextContent).ToList();
        Assert.Contains("3k", textos);
        Assert.DoesNotContain(textos, t => t.StartsWith("R$", StringComparison.Ordinal));
    }

    [Fact]
    public void Sem_eixo_nao_ha_valores_no_eixo()
    {
        var textos = Linha(p => p.Add(x => x.ShowAxis, false).Add(x => x.FormatAxisValue, v => $"{v}#")).FindAll("svg text").Select(t => t.TextContent);

        Assert.DoesNotContain(textos, t => t.EndsWith('#'));
    }

    [Theory]
    [InlineData(RvmChartSize.Small, "rvm-grafico-pequeno")]
    [InlineData(RvmChartSize.Medium, "rvm-grafico-medio")]
    public void Tamanho_vira_classe(RvmChartSize tamanho, string classe)
        => Assert.Contains(classe, Linha(p => p.Add(x => x.Size, tamanho)).Find("figure").ClassList);
}
