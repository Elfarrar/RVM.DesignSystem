using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Filter;
using RVM.DesignSystem.Components.Lists;
using RVM.DesignSystem.Components.Menu;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>RvmFilter, RvmFilterGroup e RvmListGroup, do contrato com o RVM.UI (DSGN-017).</summary>
public class RvmFiltrosTests : BunitContext
{
    public RvmFiltrosTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    // --- RvmFilter ---

    [Fact]
    public void Fechado_o_painel_nao_existe_e_o_botao_diz_que_esta_fechado()
    {
        var cortado = Render<RvmFilter>(p => p.Add(x => x.Id, "filtros"));

        var botao = cortado.Find("button#filtros");
        Assert.Equal("false", botao.GetAttribute("aria-expanded"));
        Assert.Null(botao.GetAttribute("aria-controls"));
        Assert.Contains("Filtros", botao.TextContent);
        Assert.Null(botao.GetAttribute("aria-label"));
        Assert.Empty(cortado.FindAll("[role=dialog]"));
    }

    [Theory]
    [InlineData(1, "Filtros, 1 ativo")]
    [InlineData(2, "Filtros, 2 ativos")]
    public void Contador_aparece_e_e_anunciado_no_nome_do_botao(int quantos, string nome)
    {
        var cortado = Render<RvmFilter>(p => p.Add(x => x.Count, quantos));

        var botao = cortado.Find(".rvm-filtro > button");
        Assert.Equal(nome, botao.GetAttribute("aria-label"));
        var contador = cortado.Find(".rvm-filtro-contador");
        Assert.Equal("true", contador.GetAttribute("aria-hidden"));
        Assert.Equal(quantos.ToString(System.Globalization.CultureInfo.InvariantCulture), contador.TextContent.Trim());
    }

    [Fact]
    public void Clicar_abre_o_painel_com_titulo_conteudo_e_botoes_e_avisa_OpenChanged()
    {
        var avisos = new List<bool>();
        var cortado = Render<RvmFilter>(p => p
            .Add(x => x.Id, "filtros")
            .Add(x => x.Title, "Filtrar talhoes")
            .Add(x => x.ClearText, "Zerar")
            .Add(x => x.ApplyText, "Ver resultado")
            .Add(x => x.OpenChanged, (bool v) => avisos.Add(v))
            .AddChildContent("<p class='grupos'>Cultura</p>"));

        cortado.Find("button#filtros").Click();

        var botao = cortado.Find("button#filtros");
        Assert.Equal("true", botao.GetAttribute("aria-expanded"));
        var painel = cortado.Find("[role=dialog]");
        Assert.Equal(botao.GetAttribute("aria-controls"), painel.Id);
        Assert.Equal("false", painel.GetAttribute("aria-modal"));
        Assert.Equal("Filtrar talhoes", cortado.Find($"#{painel.GetAttribute("aria-labelledby")}").TextContent);
        Assert.NotNull(cortado.Find(".grupos"));
        var textos = cortado.FindAll(".rvm-filtro-rodape button").Select(b => b.TextContent.Trim()).ToList();
        Assert.Equal(["Zerar", "Ver resultado"], textos);
        Assert.Equal([true], avisos);
        JSInterop.VerifyFocusAsyncInvoke();
    }

    [Fact]
    public void Esc_fecha_e_devolve_o_foco_ao_botao()
    {
        var avisos = new List<bool>();
        var cortado = Render<RvmFilter>(p => p.Add(x => x.OpenChanged, (bool v) => avisos.Add(v)));
        cortado.Find(".rvm-filtro > button").Click();

        cortado.Find("[role=dialog]").KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "Escape" });

        Assert.Empty(cortado.FindAll("[role=dialog]"));
        Assert.Equal([true, false], avisos);
        // Um foco ao abrir (no painel), outro ao fechar (no botao).
        JSInterop.VerifyFocusAsyncInvoke(calledTimes: 2);
    }

    [Fact]
    public void Outra_tecla_no_painel_nao_fecha()
    {
        var cortado = Render<RvmFilter>();
        cortado.Find(".rvm-filtro > button").Click();

        cortado.Find("[role=dialog]").KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "a" });

        Assert.NotEmpty(cortado.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Clicar_fora_fecha_sem_puxar_o_foco()
    {
        var cortado = Render<RvmFilter>();
        cortado.Find(".rvm-filtro > button").Click();

        cortado.Find(".rvm-fundo").Click();

        Assert.Empty(cortado.FindAll("[role=dialog]"));
        Assert.Empty(cortado.FindAll(".rvm-fundo"));
        JSInterop.VerifyFocusAsyncInvoke(calledTimes: 1);
    }

    [Fact]
    public void Clicar_de_novo_no_botao_fecha()
    {
        var cortado = Render<RvmFilter>();
        cortado.Find(".rvm-filtro > button").Click();
        cortado.Find(".rvm-filtro > button").Click();

        Assert.Empty(cortado.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Fechar_pelo_x_fecha()
    {
        var cortado = Render<RvmFilter>();
        cortado.Find(".rvm-filtro > button").Click();

        cortado.Find("button[aria-label='Fechar os filtros']").Click();

        Assert.Empty(cortado.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Aplicar_avisa_e_fecha()
    {
        var aplicou = false;
        var cortado = Render<RvmFilter>(p => p.Add(x => x.OnApply, () => aplicou = true));
        cortado.Find(".rvm-filtro > button").Click();

        cortado.FindAll(".rvm-filtro-rodape button")[1].Click();

        Assert.True(aplicou);
        Assert.Empty(cortado.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Limpar_avisa_e_deixa_o_painel_aberto()
    {
        var limpou = false;
        var cortado = Render<RvmFilter>(p => p.Add(x => x.OnClear, () => limpou = true));
        cortado.Find(".rvm-filtro > button").Click();

        cortado.FindAll(".rvm-filtro-rodape button")[0].Click();

        Assert.True(limpou);
        Assert.NotEmpty(cortado.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Estado_interno_so_segue_Open_quando_ele_muda()
    {
        var cortado = Render<RvmFilter>(p => p.Add(x => x.Open, true));
        Assert.NotEmpty(cortado.FindAll("[role=dialog]"));

        // A pessoa fecha; o pai nao muda Open (continua true) e re-renderiza por outro motivo.
        cortado.Find(".rvm-fundo").Click();
        cortado.Render(p => p.Add(x => x.Count, 3));
        Assert.Empty(cortado.FindAll("[role=dialog]"));

        // Quando o pai muda Open, o painel segue.
        cortado.Render(p => p.Add(x => x.Open, false));
        cortado.Render(p => p.Add(x => x.Open, true));
        Assert.NotEmpty(cortado.FindAll("[role=dialog]"));
    }

    [Theory]
    [InlineData(RvmMenuPlacement.BottomEnd, "rvm-fim")]
    [InlineData(RvmMenuPlacement.BottomCenter, "rvm-centro")]
    [InlineData(RvmMenuPlacement.TopStart, "rvm-acima")]
    [InlineData(RvmMenuPlacement.TopEnd, "rvm-acima")]
    public void Placement_vira_classe_do_painel(RvmMenuPlacement onde, string classe)
    {
        var cortado = Render<RvmFilter>(p => p.Add(x => x.Placement, onde).Add(x => x.Open, true));

        Assert.Contains(classe, cortado.Find("[role=dialog]").ClassList);
    }

    [Fact]
    public void BottomStart_nao_desloca_o_painel()
    {
        var cortado = Render<RvmFilter>(p => p.Add(x => x.Placement, RvmMenuPlacement.BottomStart).Add(x => x.Open, true));

        Assert.Equal("rvm-filtro-painel", cortado.Find("[role=dialog]").ClassName);
    }

    [Fact]
    public void Desabilitado_desabilita_o_botao_e_atributos_chegam_a_raiz()
    {
        var cortado = Render<RvmFilter>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.Class, "barra")
            .AddUnmatched("data-testid", "filtro-talhoes"));

        var raiz = cortado.Find(".rvm-filtro");
        Assert.Contains("barra", raiz.ClassList);
        Assert.Equal("filtro-talhoes", raiz.GetAttribute("data-testid"));
        Assert.True(cortado.Find(".rvm-filtro > button").HasAttribute("disabled"));
    }

    [Fact]
    public void Textos_padrao_sao_os_do_RVM_UI()
    {
        var filtro = new RvmFilter();

        Assert.Equal("Filtros", filtro.Label);
        Assert.Equal("Filtros", filtro.Title);
        Assert.Equal("Limpar", filtro.ClearText);
        Assert.Equal("Aplicar", filtro.ApplyText);
        Assert.Equal(RvmMenuPlacement.BottomEnd, filtro.Placement);
    }

    // --- RvmFilterGroup ---

    [Fact]
    public void Grupo_fechado_esconde_o_corpo_e_o_botao_aponta_para_ele()
    {
        var cortado = Render<RvmFilterGroup>(p => p
            .Add(x => x.Title, "Cultura")
            .Add(x => x.Id, "cultura")
            .AddChildContent("<span class='opcao'>Soja</span>"));

        var botao = cortado.Find("h3 > button#cultura");
        Assert.Equal("Cultura", botao.TextContent.Trim());
        Assert.Equal("false", botao.GetAttribute("aria-expanded"));
        var corpo = cortado.Find($"#{botao.GetAttribute("aria-controls")}");
        Assert.True(corpo.HasAttribute("hidden"));
        Assert.Equal("group", corpo.GetAttribute("role"));
        Assert.Equal("cultura", corpo.GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void Clicar_no_grupo_abre_e_avisa_OpenChanged()
    {
        var avisos = new List<bool>();
        var cortado = Render<RvmFilterGroup>(p => p
            .Add(x => x.Title, "Safra")
            .Add(x => x.OpenChanged, (bool v) => avisos.Add(v)));

        cortado.Find("button").Click();

        Assert.Equal("true", cortado.Find("button").GetAttribute("aria-expanded"));
        Assert.False(cortado.Find("[role=group]").HasAttribute("hidden"));
        Assert.Contains("rvm-aberto", cortado.Find(".rvm-grupo-filtro").ClassList);

        cortado.Find("button").Click();
        Assert.Equal([true, false], avisos);
    }

    [Fact]
    public void Grupo_segue_Open_so_quando_ele_muda()
    {
        var cortado = Render<RvmFilterGroup>(p => p.Add(x => x.Title, "Status").Add(x => x.Open, true));
        Assert.Equal("true", cortado.Find("button").GetAttribute("aria-expanded"));

        cortado.Find("button").Click();
        cortado.Render(p => p.Add(x => x.Scroll, false));
        Assert.Equal("false", cortado.Find("button").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void Scroll_limita_a_altura_e_entra_na_tabulacao()
    {
        var cortado = Render<RvmFilterGroup>(p => p
            .Add(x => x.Title, "Fazenda")
            .Add(x => x.Scroll, true)
            .Add(x => x.Class, "extra")
            .AddUnmatched("data-testid", "grupo-fazenda"));

        var corpo = cortado.Find("[role=group]");
        Assert.Contains("rvm-rolagem", corpo.ClassList);
        Assert.Equal("0", corpo.GetAttribute("tabindex"));
        var raiz = cortado.Find(".rvm-grupo-filtro");
        Assert.Contains("extra", raiz.ClassList);
        Assert.Equal("grupo-fazenda", raiz.GetAttribute("data-testid"));
    }

    [Fact]
    public void Sem_Scroll_o_corpo_nao_entra_na_tabulacao()
    {
        var cortado = Render<RvmFilterGroup>(p => p.Add(x => x.Title, "Fazenda"));

        Assert.False(cortado.Find("[role=group]").HasAttribute("tabindex"));
    }

    // --- RvmListGroup ---

    [Fact]
    public void ListGroup_e_secao_nomeada_pelo_titulo_com_subtitulo_e_selo()
    {
        var cortado = Render<RvmListGroup>(p => p
            .Add(x => x.Title, "Visitas da semana")
            .Add(x => x.Subtitle, "Agronomos em campo")
            .Add(x => x.BadgeText, "6 novas")
            .Add(x => x.BadgeColor, RvmColor.Success)
            .Add(x => x.Class, "painel")
            .AddUnmatched("data-testid", "visitas")
            .AddChildContent("<ul class='itens'><li>Talhao 1</li></ul>"));

        var secao = cortado.Find("section");
        Assert.Equal("Visitas da semana", secao.GetAttribute("aria-label"));
        Assert.Contains("painel", secao.ClassList);
        Assert.Equal("visitas", secao.GetAttribute("data-testid"));
        Assert.Contains("Visitas da semana", cortado.Find(".rvm-grupo-lista-linha").TextContent);
        Assert.Contains("Agronomos em campo", cortado.Markup);
        Assert.Contains("rvm-success", cortado.Find(".rvm-grupo-lista-linha > span").ClassList);
        Assert.Contains("6 novas", cortado.Find(".rvm-grupo-lista-linha > span").TextContent);
        Assert.NotNull(cortado.Find(".rvm-grupo-lista-corpo .itens"));
        Assert.Empty(cortado.FindAll(".rvm-grupo-lista-abas"));
        Assert.Empty(cortado.FindAll("[aria-haspopup=menu]"));
    }

    [Fact]
    public void ListGroup_fragmento_Badge_vence_BadgeText()
    {
        var cortado = Render<RvmListGroup>(p => p
            .Add(x => x.Title, "Pedidos")
            .Add(x => x.BadgeText, "Selo padrao")
            .Add(x => x.Badge, (RenderFragment)(b => b.AddMarkupContent(0, "<b class='meu-selo'>Novo</b>"))));

        Assert.NotNull(cortado.Find(".meu-selo"));
        Assert.DoesNotContain("Selo padrao", cortado.Markup);
    }

    [Fact]
    public void ListGroup_com_menu_e_abas()
    {
        var cortado = Render<RvmListGroup>(p => p
            .Add(x => x.Title, "Pedidos")
            .Add(x => x.MenuLabel, "Acoes dos pedidos")
            .Add<RvmMenuItem>(x => x.Menu, i => i.Add(m => m.Text, "Exportar"))
            .Add(x => x.Tabs, (RenderFragment)(b => b.AddMarkupContent(0, "<div class='minhas-abas'>abas</div>"))));

        var gatilho = cortado.Find("[aria-haspopup=menu]");
        Assert.Equal("Acoes dos pedidos", gatilho.GetAttribute("aria-label"));
        Assert.NotNull(cortado.Find(".rvm-grupo-lista-abas .minhas-abas"));
    }

    [Fact]
    public void ListGroup_padroes_do_RVM_UI()
    {
        var grupo = new RvmListGroup();

        Assert.Equal(RvmColor.Accent, grupo.BadgeColor);
        Assert.Equal("Acoes da lista", grupo.MenuLabel);
    }
}
