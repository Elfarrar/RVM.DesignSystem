using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem;
using RVM.DesignSystem.Components.Rating;
using RVM.DesignSystem.Components.Stepper;
using RVM.DesignSystem.Components.Timeline;

namespace RVM.DesignSystem.Tests.Components;

public class RvmRatingTests : BunitContext
{
    [Fact]
    public void Editavel_e_um_grupo_de_radios_nativos_com_nome()
    {
        var cortado = Render<RvmRating>(p => p.Add(x => x.Value, 3).Add(x => x.Label, "Nota do atendimento"));

        var grupo = cortado.Find("fieldset.rvm-avaliacao");
        Assert.Equal("Nota do atendimento", grupo.QuerySelector("legend")!.TextContent);
        var radios = cortado.FindAll("input[type=radio]");
        Assert.Equal(5, radios.Count);
        Assert.Single(radios, r => r.HasAttribute("checked"));
        Assert.Equal("3", cortado.Find("input[checked]").GetAttribute("value"));
        Assert.All(radios, r => Assert.Equal(radios[0].GetAttribute("name"), r.GetAttribute("name")));
        Assert.Equal(["1 estrela de 5", "2 estrelas de 5", "3 estrelas de 5", "4 estrelas de 5", "5 estrelas de 5"],
            cortado.FindAll(".rvm-alvo .rvm-so-leitor:not(input)").Select(s => s.TextContent));
    }

    [Fact]
    public void Escolher_muda_a_nota_e_avisa()
    {
        var escolhida = 0d;
        var cortado = Render<RvmRating>(p => p
            .Add(x => x.Value, 1)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, v => escolhida = v)));

        cortado.FindAll("input[type=radio]")[3].Change(true);

        Assert.Equal(4, escolhida);
        Assert.Equal("4", cortado.Find("input[checked]").GetAttribute("value"));
    }

    [Fact]
    public void Meia_estrela_dobra_os_radios_e_cobre_metade_da_estrela()
    {
        var cortado = Render<RvmRating>(p => p.Add(x => x.AllowHalf, true).Add(x => x.Max, 4).Add(x => x.Value, 2.5));

        var radios = cortado.FindAll("input[type=radio]");
        Assert.Equal(8, radios.Count);
        Assert.Equal("2.5", cortado.Find("input[checked]").GetAttribute("value"));
        Assert.Equal("left: 25%; width: 12.5%", cortado.FindAll(".rvm-alvo")[2].GetAttribute("style"));
        Assert.Equal("2,5 estrelas de 4", cortado.FindAll(".rvm-alvo span")[4].TextContent);
    }

    [Fact]
    public void Preenchimento_parcial_sai_em_porcentagem_invariante_mesmo_em_pt_br()
    {
        var anterior = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("pt-BR");
        try
        {
            var cortado = Render<RvmRating>(p => p.Add(x => x.Value, 2.25).Add(x => x.AllowHalf, true));

            var cheias = cortado.FindAll(".rvm-cheia");
            Assert.Equal("width: 100%", cheias[1].GetAttribute("style"));
            Assert.Equal("width: 25%", cheias[2].GetAttribute("style"));
            Assert.Equal("width: 0%", cheias[3].GetAttribute("style"));
            Assert.Equal("left: 0%; width: 10%", cortado.FindAll(".rvm-alvo")[0].GetAttribute("style"));
        }
        finally
        {
            CultureInfo.CurrentCulture = anterior;
        }
    }

    [Fact]
    public void Passar_o_mouse_mostra_a_previa_e_sair_volta()
    {
        var cortado = Render<RvmRating>(p => p.Add(x => x.Value, 1));

        cortado.FindAll(".rvm-alvo")[3].MouseEnter();
        Assert.Equal("width: 100%", cortado.FindAll(".rvm-cheia")[3].GetAttribute("style"));

        cortado.Find("fieldset").MouseLeave();
        Assert.Equal("width: 0%", cortado.FindAll(".rvm-cheia")[3].GetAttribute("style"));
    }

    [Fact]
    public void Somente_leitura_e_imagem_com_a_nota_por_extenso()
    {
        var cortado = Render<RvmRating>(p => p.Add(x => x.ReadOnly, true).Add(x => x.Value, 3.5).Add(x => x.Label, "Nota"));

        var imagem = cortado.Find("[role=img]");
        Assert.Equal("Nota: 3,5 de 5", imagem.GetAttribute("aria-label"));
        Assert.Contains("rvm-leitura", imagem.GetAttribute("class"));
        Assert.Empty(cortado.FindAll("input"));
    }

    [Fact]
    public void Desabilitado_nao_muda_nem_mostra_previa()
    {
        var disparos = 0;
        var cortado = Render<RvmRating>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.Name, "nota")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, _ => disparos++)));

        var grupo = cortado.Find("fieldset");
        Assert.True(grupo.HasAttribute("disabled"));
        Assert.Equal("nota", cortado.Find("input").GetAttribute("name"));
        cortado.FindAll(".rvm-alvo")[4].MouseEnter();
        Assert.Equal("width: 0%", cortado.FindAll(".rvm-cheia")[4].GetAttribute("style"));
        cortado.FindAll("input")[4].Change(true);
        Assert.Equal(0, disparos);
    }

    [Fact]
    public void Somente_leitura_ignora_escolha()
    {
        var cortado = Render<RvmRating>(p => p.Add(x => x.ReadOnly, true).Add(x => x.Value, 2));

        Assert.Equal("Avaliacao: 2 de 5", cortado.Find("[role=img]").GetAttribute("aria-label"));
    }

    [Theory]
    [InlineData(RvmSize.Small, "rvm-avaliacao rvm-pequeno minha")]
    [InlineData(RvmSize.Medium, "rvm-avaliacao rvm-medio minha")]
    [InlineData(RvmSize.Large, "rvm-avaliacao rvm-grande rvm-desabilitado minha")]
    public void Tamanho_e_estado_viram_classes(RvmSize tamanho, string esperado)
    {
        var cortado = Render<RvmRating>(p => p
            .Add(x => x.Size, tamanho)
            .Add(x => x.Disabled, tamanho == RvmSize.Large)
            .AddUnmatched("class", "minha"));

        Assert.Equal(esperado, cortado.Find("fieldset").GetAttribute("class"));
    }
}

public class RvmStepperTests : BunitContext
{
    private static readonly RvmStep[] Etapas =
    [
        new("Dados da conta", "E-mail e senha"),
        new("Dados pessoais", "Nome e telefone"),
        new("Redes sociais")
    ];

    [Fact]
    public void Lista_ordenada_com_a_atual_marcada_e_estado_em_texto()
    {
        var cortado = Render<RvmStepper>(p => p.Add(x => x.Steps, Etapas).Add(x => x.ActiveStep, 1));

        var lista = cortado.Find("ol");
        Assert.Equal("Etapas", lista.GetAttribute("aria-label"));
        var etapas = cortado.FindAll("li");
        Assert.Equal(["rvm-etapa rvm-concluida", "rvm-etapa rvm-atual", "rvm-etapa rvm-pendente"], etapas.Select(e => e.GetAttribute("class")));
        Assert.Equal("step", etapas[1].GetAttribute("aria-current"));
        Assert.Null(etapas[0].GetAttribute("aria-current"));
        Assert.Equal(["concluida", "etapa atual", "pendente"], cortado.FindAll(".rvm-so-leitor").Select(s => s.TextContent));
        Assert.NotNull(etapas[0].QuerySelector(".rvm-marcador svg"));
        Assert.Null(etapas[2].QuerySelector(".rvm-marcador svg"));
    }

    [Fact]
    public void Numero_de_duas_casas_so_com_texto_ao_lado_e_conector_entre_as_etapas()
    {
        var aoLado = Render<RvmStepper>(p => p.Add(x => x.Steps, Etapas));
        Assert.Equal(["01", "02", "03"], aoLado.FindAll(".rvm-numero").Select(n => n.TextContent));
        Assert.Equal(2, aoLado.FindAll(".rvm-conector").Count);
        Assert.Equal("E-mail e senha", aoLado.Find(".rvm-descricao").TextContent);

        var embaixo = Render<RvmStepper>(p => p
            .Add(x => x.Steps, Etapas)
            .Add(x => x.Placement, RvmStepperLabelPlacement.Bottom)
            .Add(x => x.Orientation, RvmOrientation.Vertical)
            .Add(x => x.AriaLabel, "Cadastro")
            .AddUnmatched("class", "minha"));
        Assert.Empty(embaixo.FindAll(".rvm-numero"));
        Assert.Equal("rvm-etapas rvm-vertical rvm-texto-embaixo minha", embaixo.Find("ol").GetAttribute("class"));
        Assert.Equal("Cadastro", embaixo.Find("ol").GetAttribute("aria-label"));
    }

    [Fact]
    public void Etapa_com_erro_avisa_em_texto_e_troca_o_icone()
    {
        var cortado = Render<RvmStepper>(p => p
            .Add(x => x.Steps, new RvmStep[] { new("Dados da conta", HasError: true), new("Revisao") })
            .Add(x => x.ActiveStep, 1));

        var erro = cortado.FindAll("li")[0];
        Assert.Contains("rvm-erro", erro.GetAttribute("class"));
        Assert.Equal("com erro, precisa de revisao", erro.QuerySelector(".rvm-so-leitor")!.TextContent);
        Assert.NotNull(erro.QuerySelector(".rvm-marcador svg"));
        Assert.Equal("rvm-etapas rvm-horizontal rvm-texto-ao-lado", cortado.Find("ol").GetAttribute("class"));
    }
}

public class RvmTimelineTests : BunitContext
{
    private static RenderFragment Itens => b =>
    {
        b.OpenComponent<RvmTimelineItem>(0);
        b.AddAttribute(1, nameof(RvmTimelineItem.Color), (RvmColor?)RvmColor.Success);
        b.AddAttribute(2, nameof(RvmTimelineItem.ChildContent), (RenderFragment)(c => c.AddContent(0, "Plantio concluido")));
        b.AddAttribute(3, nameof(RvmTimelineItem.OppositeContent), (RenderFragment)(c => c.AddContent(0, "12/10")));
        b.CloseComponent();
        b.OpenComponent<RvmTimelineItem>(10);
        b.AddAttribute(11, nameof(RvmTimelineItem.Outlined), true);
        b.AddAttribute(12, nameof(RvmTimelineItem.ChildContent), (RenderFragment)(c => c.AddContent(0, "Primeira aplicacao")));
        b.AddAttribute(13, "class", "minha");
        b.CloseComponent();
    };

    [Fact]
    public void Lista_ordenada_com_itens_e_desenho_escondido()
    {
        var cortado = Render<RvmTimeline>(p => p.Add(x => x.AriaLabel, "Safra 2026").Add(x => x.ChildContent, Itens));

        var lista = cortado.Find("ol");
        Assert.Equal("Safra 2026", lista.GetAttribute("aria-label"));
        Assert.Equal("rvm-linha-do-tempo rvm-conteudo-a-direita", lista.GetAttribute("class"));
        var itens = cortado.FindAll("li");
        Assert.Equal("rvm-item-tempo rvm-cheio rvm-success rvm-com-oposto", itens[0].GetAttribute("class"));
        Assert.Equal("rvm-item-tempo rvm-contorno rvm-neutro minha", itens[1].GetAttribute("class"));
        Assert.Equal("Plantio concluido", itens[0].QuerySelector(".rvm-principal")!.TextContent);
        Assert.Equal("12/10", itens[0].QuerySelector(".rvm-oposto")!.TextContent);
        Assert.All(cortado.FindAll(".rvm-separador"), s => Assert.Equal("true", s.GetAttribute("aria-hidden")));
    }

    [Theory]
    [InlineData(RvmTimelinePosition.Left, "rvm-linha-do-tempo rvm-conteudo-a-esquerda")]
    [InlineData(RvmTimelinePosition.Alternate, "rvm-linha-do-tempo rvm-alternada")]
    public void Posicao_vira_classe(RvmTimelinePosition posicao, string esperado)
    {
        var cortado = Render<RvmTimeline>(p => p.Add(x => x.Position, posicao).Add(x => x.ChildContent, Itens).AddUnmatched("class", "x"));

        Assert.Equal($"{esperado} x", cortado.Find("ol").GetAttribute("class"));
    }

    [Theory]
    [InlineData(RvmColor.Primary, "rvm-primary")]
    [InlineData(RvmColor.Secondary, "rvm-secondary")]
    [InlineData(RvmColor.Info, "rvm-info")]
    [InlineData(RvmColor.Warning, "rvm-warning")]
    [InlineData(RvmColor.Error, "rvm-error")]
    public void Cada_papel_de_cor_vira_classe(RvmColor cor, string classe)
    {
        var cortado = Render<RvmTimelineItem>(p => p.Add(x => x.Color, cor));

        Assert.Contains(classe, cortado.Find("li").GetAttribute("class"));
    }
}
