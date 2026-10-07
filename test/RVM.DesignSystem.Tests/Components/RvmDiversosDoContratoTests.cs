using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using RVM.DesignSystem.Components.Accordion;
using RVM.DesignSystem.Components.Alert;
using RVM.DesignSystem.Components.AppShell;
using RVM.DesignSystem.Components.Avatar;
using RVM.DesignSystem.Components.Chip;
using RVM.DesignSystem.Components.Menu;
using RVM.DesignSystem.Components.Pagination;
using RVM.DesignSystem.Components.Rating;
using RVM.DesignSystem.Components.Skeleton;
using RVM.DesignSystem.Components.Stepper;
using RVM.DesignSystem.Components.Timeline;
using RVM.DesignSystem.Components.Tooltip;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>Parametros avulsos do contrato com o RVM.UI (DSGN-017).</summary>
public class RvmDiversosDoContratoTests : BunitContext
{
    public RvmDiversosDoContratoTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Dense_vira_classe()
    {
        Assert.Contains("rvm-denso", Render<RvmAccordion>(p => p.Add(x => x.Dense, true)).Find(".rvm-acordeao").ClassList);
        Assert.Contains("rvm-denso", Render<RvmAlert>(p => p.Add(x => x.Dense, true).AddChildContent("Salvo")).Find(".rvm-alerta").ClassList);
        Assert.Contains("rvm-denso", Render<RvmTimeline>(p => p.Add(x => x.Dense, true)).Find(".rvm-linha-do-tempo").ClassList);
    }

    [Fact]
    public void Chip_aceita_o_rotulo_em_texto()
        => Assert.Equal("Soja", Render<RvmChip>(p => p.Add(x => x.Label, "Soja")).Find(".rvm-rotulo").TextContent);

    [Fact]
    public void Paginacao_com_rotulos_das_setas()
    {
        var cortado = Render<RvmPagination>(p => p.Add(x => x.Count, 3).Add(x => x.PreviousLabel, "Voltar").Add(x => x.NextLabel, "Avancar"));

        var setas = cortado.FindAll(".rvm-seta");
        Assert.Equal("Voltar", setas[0].GetAttribute("aria-label"));
        Assert.Equal("Avancar", setas[^1].GetAttribute("aria-label"));
    }

    [Fact]
    public void Esqueleto_de_varias_linhas_encurta_a_ultima()
    {
        var linhas = Render<RvmSkeleton>(p => p.Add(x => x.Lines, 3)).FindAll(".rvm-esqueleto-linhas > .rvm-esqueleto");

        Assert.Equal(3, linhas.Count);
        Assert.Equal("width: 60%", linhas[2].GetAttribute("style"));
    }

    [Fact]
    public void Nota_compacta_com_nome_proprio_e_lado_livre()
    {
        var raiz = Render<RvmRating>(p => p
            .Add(x => x.Value, 4.5).Add(x => x.ReadOnly, true).Add(x => x.Display, RvmRatingDisplay.Compact)
            .Add(x => x.AriaLabel, "Nota do fornecedor").Add(x => x.SizePx, 14)).Find(".rvm-avaliacao");

        Assert.Equal("Nota do fornecedor", raiz.GetAttribute("aria-label"));
        Assert.Single(raiz.QuerySelectorAll(".rvm-estrela"));
        Assert.Contains("4,5", raiz.QuerySelector(".rvm-nota-escrita")!.TextContent);
        Assert.Contains("--rvm-estrela-lado: 14px", raiz.GetAttribute("style"));
    }

    [Fact]
    public void Avatar_com_nome_e_anel()
    {
        var foto = Render<RvmAvatar>(p => p.Add(x => x.Src, "/a.png").Add(x => x.Name, "Rafael")).Find("img");
        var semFoto = Render<RvmAvatar>(p => p.Add(x => x.Name, "Rafael").Add(x => x.Ring, true)).Find(".rvm-anel");

        Assert.Equal("Rafael", foto.GetAttribute("alt"));
        Assert.Equal("Rafael", foto.GetAttribute("title"));
        Assert.Equal("Rafael", semFoto.GetAttribute("aria-label"));
    }

    [Fact]
    public void Dica_sem_conteudo_ganha_gatilho_pronto()
    {
        var cortado = Render<RvmTooltip>(p => p.Add(x => x.Text, "Area plantada").Add(x => x.Trigger, RvmTooltipTrigger.Question).Add(x => x.TriggerLabel, "Ajuda"));

        var gatilho = cortado.Find("button.rvm-gatilho-padrao");
        Assert.Equal("Ajuda", gatilho.GetAttribute("aria-label"));
        Assert.Equal(cortado.Find("[role=tooltip]").GetAttribute("id"), gatilho.GetAttribute("aria-describedby"));
    }

    [Fact]
    public void Item_de_menu_link_perigoso_e_que_leva_o_foco()
    {
        var cortado = Render<RvmMenu>(p => p.Add(x => x.Label, "Acoes").AddChildContent<RvmMenuItem>(i => i.Add(x => x.Text, "Excluir").Add(x => x.Href, "/excluir").Add(x => x.Danger, true)));
        cortado.Find("button").Click();

        var item = cortado.Find("[role=menuitem]");
        Assert.Equal("A", item.TagName);
        Assert.Equal("/excluir", item.GetAttribute("href"));
        Assert.Contains("rvm-perigo", item.ClassList);
        Assert.Equal("Excluir", item.TextContent.Trim());
    }

    [Fact]
    public void Item_de_navegacao_sem_link_e_botao_ativo_por_parametro()
    {
        var clicou = false;
        var cortado = Render<RvmAppShell>(p => p.Add(x => x.Navigation, (RenderFragment)(b =>
        {
            b.OpenComponent<RvmNavItem>(0);
            b.AddComponentParameter(1, nameof(RvmNavItem.Text), "Filtros");
            b.AddComponentParameter(2, nameof(RvmNavItem.Active), true);
            b.AddComponentParameter(3, nameof(RvmNavItem.ImageUrl), "/f.png");
            b.AddComponentParameter(4, nameof(RvmNavItem.OnClick), EventCallback.Factory.Create(this, () => clicou = true));
            b.CloseComponent();
        })));

        var botao = cortado.Find("button.rvm-nav-link");
        Assert.Equal("page", botao.GetAttribute("aria-current"));
        Assert.NotNull(botao.QuerySelector("img.rvm-nav-imagem"));
        botao.Click();
        Assert.True(clicou);
    }

    [Fact]
    public void Grupo_da_navegacao_abre_pela_rota()
    {
        Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>().NavigateTo("financeiro/contas");
        var cortado = Render<RvmAppShell>(p => p.Add(x => x.Navigation, (RenderFragment)(b =>
        {
            b.OpenComponent<RvmNavGroup>(0);
            b.AddComponentParameter(1, nameof(RvmNavGroup.Text), "Financeiro");
            b.AddComponentParameter(2, nameof(RvmNavGroup.ActivePrefix), "financeiro");
            b.CloseComponent();
        })));

        var botao = cortado.Find("button.rvm-nav-grupo");
        Assert.Equal("true", botao.GetAttribute("aria-expanded"));
        Assert.Equal("true", botao.GetAttribute("aria-current"));
    }

    [Fact]
    public void Stepper_sem_etapas_vira_bolinhas()
    {
        var barra = Render<RvmStepper>(p => p.Add(x => x.Count, 4).Add(x => x.Current, 2).Add(x => x.Label, "Cadastro")).Find("[role=progressbar]");

        Assert.Equal("Cadastro", barra.GetAttribute("aria-label"));
        Assert.Equal("Passo 2 de 4", barra.GetAttribute("aria-valuetext"));
        Assert.Equal(4, barra.QuerySelectorAll(".rvm-bolinha").Length);
        Assert.Contains("rvm-bolinha-atual", barra.QuerySelectorAll(".rvm-bolinha")[1].ClassList);
    }
}
