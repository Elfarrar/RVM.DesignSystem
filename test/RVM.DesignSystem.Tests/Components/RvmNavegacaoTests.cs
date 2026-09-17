using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using RVM.DesignSystem;
using RVM.DesignSystem.Components.Menu;
using RVM.DesignSystem.Components.Select;
using RVM.DesignSystem.Components.Tabs;
using RVM.DesignSystem.Components.TextField;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

public class RvmTabsTests : BunitContext
{
    public RvmTabsTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private static RenderFragment TresAbas(bool segundaDesabilitada = false) => b =>
    {
        void Aba(int seq, string titulo, string conteudo, bool desabilitada = false, RvmIconName? icone = null)
        {
            b.OpenComponent<RvmTab>(seq);
            b.AddAttribute(seq + 1, nameof(RvmTab.Title), titulo);
            b.AddAttribute(seq + 2, nameof(RvmTab.Disabled), desabilitada);
            if (icone is not null) b.AddAttribute(seq + 3, nameof(RvmTab.Icon), icone);
            b.AddAttribute(seq + 4, nameof(RvmTab.ChildContent), (RenderFragment)(c => c.AddContent(0, conteudo)));
            b.CloseComponent();
        }

        Aba(0, "Conta", "Painel da conta", icone: RvmIconName.User);
        Aba(10, "Seguranca", "Painel de seguranca", segundaDesabilitada);
        Aba(20, "Avisos", "Painel de avisos");
    };

    private IRenderedComponent<RvmTabs> Abas(int ativa = 0, Action<int>? aoMudar = null, bool segundaDesabilitada = false,
        Action<ComponentParameterCollectionBuilder<RvmTabs>>? extra = null)
        => Render<RvmTabs>(p =>
        {
            p.Add(x => x.ActiveIndex, ativa)
             .Add(x => x.ActiveIndexChanged, EventCallback.Factory.Create<int>(this, v => aoMudar?.Invoke(v)))
             .Add(x => x.AriaLabel, "Configuracoes")
             .Add(x => x.ChildContent, TresAbas(segundaDesabilitada));
            extra?.Invoke(p);
        });

    [Fact]
    public void Lista_de_abas_com_papeis_e_so_a_ativa_na_tabulacao()
    {
        var cortado = Abas();

        var lista = cortado.Find("[role=tablist]");
        Assert.Equal("Configuracoes", lista.GetAttribute("aria-label"));
        Assert.Equal("horizontal", lista.GetAttribute("aria-orientation"));
        var abas = cortado.FindAll("[role=tab]");
        Assert.Equal(3, abas.Count);
        Assert.Equal(["true", "false", "false"], abas.Select(a => a.GetAttribute("aria-selected")));
        Assert.Equal(["0", "-1", "-1"], abas.Select(a => a.GetAttribute("tabindex")));
    }

    [Fact]
    public void So_o_painel_ativo_e_renderizado_e_ligado_a_aba()
    {
        var cortado = Abas(ativa: 2);

        var painel = cortado.Find("[role=tabpanel]");
        Assert.Equal("Painel de avisos", painel.TextContent);
        var aba = cortado.FindAll("[role=tab]")[2];
        Assert.Equal(painel.Id, aba.GetAttribute("aria-controls"));
        Assert.Equal(aba.Id, painel.GetAttribute("aria-labelledby"));
        Assert.Single(cortado.FindAll("[role=tabpanel]"));
    }

    [Fact]
    public void Clicar_troca_de_aba()
    {
        var escolhida = -1;
        var cortado = Abas(aoMudar: v => escolhida = v);

        cortado.FindAll("[role=tab]")[2].Click();

        Assert.Equal(2, escolhida);
        Assert.Equal("Painel de avisos", cortado.Find("[role=tabpanel]").TextContent);
    }

    [Theory]
    [InlineData("ArrowRight", 0, 2)]
    [InlineData("ArrowLeft", 0, 2)]
    [InlineData("ArrowRight", 2, 0)]
    [InlineData("End", 0, 2)]
    [InlineData("Home", 2, 0)]
    public void Setas_home_e_end_pulam_a_desabilitada_e_dao_a_volta(string tecla, int de, int para)
    {
        var escolhida = -1;
        var cortado = Abas(ativa: de, aoMudar: v => escolhida = v, segundaDesabilitada: true);

        cortado.FindAll("[role=tab]")[de].KeyDown(key: tecla);

        Assert.Equal(para, escolhida);
    }

    [Fact]
    public void Setas_da_outra_orientacao_e_teclas_soltas_nao_fazem_nada()
    {
        var escolhida = -1;
        var cortado = Abas(aoMudar: v => escolhida = v);

        cortado.FindAll("[role=tab]")[0].KeyDown(key: "ArrowDown");
        cortado.FindAll("[role=tab]")[0].KeyDown(key: "a");

        Assert.Equal(-1, escolhida);
    }

    [Fact]
    public void Vertical_usa_setas_para_cima_e_para_baixo()
    {
        var escolhida = -1;
        var cortado = Abas(aoMudar: v => escolhida = v, extra: p => p.Add(x => x.Orientation, RvmOrientation.Vertical));

        Assert.Equal("vertical", cortado.Find("[role=tablist]").GetAttribute("aria-orientation"));
        cortado.FindAll("[role=tab]")[0].KeyDown(key: "ArrowDown");

        Assert.Equal(1, escolhida);
    }

    [Fact]
    public void Ativa_desabilitada_ou_fora_da_faixa_cai_numa_habilitada()
    {
        var desabilitada = Abas(ativa: 1, segundaDesabilitada: true);
        Assert.Equal("true", desabilitada.FindAll("[role=tab]")[0].GetAttribute("aria-selected"));

        var fora = Abas(ativa: 9);
        Assert.Equal("true", fora.FindAll("[role=tab]")[2].GetAttribute("aria-selected"));
    }

    [Fact]
    public void Clicar_na_desabilitada_nao_troca()
    {
        var escolhida = -1;
        var cortado = Abas(aoMudar: v => escolhida = v, segundaDesabilitada: true);

        Assert.True(cortado.FindAll("[role=tab]")[1].HasAttribute("disabled"));
        cortado.FindAll("[role=tab]")[1].Click();

        Assert.Equal(-1, escolhida);
    }

    [Theory]
    [InlineData(RvmTabsVariant.Standard, RvmOrientation.Horizontal, false, "abas sublinhadas horizontal")]
    [InlineData(RvmTabsVariant.Contained, RvmOrientation.Horizontal, true, "abas preenchidas horizontal largura-total")]
    [InlineData(RvmTabsVariant.Standard, RvmOrientation.Vertical, true, "abas sublinhadas vertical")]
    public void Estilo_orientacao_e_largura_viram_classes(RvmTabsVariant variante, RvmOrientation orientacao, bool larguraTotal, string esperado)
    {
        var cortado = Abas(extra: p => p.Add(x => x.Variant, variante).Add(x => x.Orientation, orientacao).Add(x => x.FullWidth, larguraTotal));

        Assert.Equal(esperado, cortado.Find("div.abas").GetAttribute("class"));
    }

    [Fact]
    public void Atributos_se_dividem_entre_raiz_e_lista_e_icone_aparece()
    {
        var cortado = Abas(extra: p => p.AddUnmatched("class", "minha").AddUnmatched("style", "margin: 0").AddUnmatched("data-teste", "x"));

        var raiz = cortado.Find("div.abas");
        Assert.EndsWith("minha", raiz.GetAttribute("class"));
        Assert.Equal("margin: 0", raiz.GetAttribute("style"));
        Assert.Equal("x", cortado.Find("[role=tablist]").GetAttribute("data-teste"));
        Assert.NotNull(cortado.FindAll("[role=tab]")[0].QuerySelector("svg"));
    }

    [Fact]
    public void Aba_fora_de_um_rvmtabs_explica_o_erro()
    {
        var erro = Assert.Throws<InvalidOperationException>(() => Render<RvmTab>(p => p.Add(x => x.Title, "Solta")));

        Assert.Contains("RvmTabs", erro.Message);
    }

    [Fact]
    public void Mudar_o_titulo_atualiza_a_lista()
    {
        var titulo = "Antes";
        var cortado = Render<RvmTabs>(p => p.Add(x => x.ChildContent, (RenderFragment)(b =>
        {
            b.OpenComponent<RvmTab>(0);
            b.AddAttribute(1, nameof(RvmTab.Title), titulo);
            b.CloseComponent();
        })));

        titulo = "Depois";
        cortado.Render(p => p.Add(x => x.ChildContent, (RenderFragment)(b =>
        {
            b.OpenComponent<RvmTab>(0);
            b.AddAttribute(1, nameof(RvmTab.Title), titulo);
            b.CloseComponent();
        })));

        Assert.Equal("Depois", cortado.Find("[role=tab]").TextContent.Trim());
    }
}

public class RvmMenuTests : BunitContext
{
    public RvmMenuTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private readonly List<string> _acoes = [];

    private IRenderedComponent<RvmMenu> Menu(Action<ComponentParameterCollectionBuilder<RvmMenu>>? extra = null)
        => Render<RvmMenu>(p =>
        {
            p.Add(x => x.Label, "Acoes")
             .Add(x => x.ChildContent, (RenderFragment)(b =>
             {
                 void Item(int seq, string texto, bool desabilitado = false)
                 {
                     b.OpenComponent<RvmMenuItem>(seq);
                     b.AddAttribute(seq + 1, nameof(RvmMenuItem.OnClick), EventCallback.Factory.Create(this, () => _acoes.Add(texto)));
                     b.AddAttribute(seq + 2, nameof(RvmMenuItem.Disabled), desabilitado);
                     b.AddAttribute(seq + 3, nameof(RvmMenuItem.Icon), RvmIconName.Pencil);
                     b.AddAttribute(seq + 4, nameof(RvmMenuItem.ChildContent), (RenderFragment)(c => c.AddContent(0, texto)));
                     b.CloseComponent();
                 }

                 Item(0, "Editar");
                 b.OpenComponent<RvmMenuDivider>(10);
                 b.CloseComponent();
                 Item(20, "Duplicar", desabilitado: true);
                 Item(30, "Excluir");
             }));
            extra?.Invoke(p);
        });

    [Fact]
    public void Fechado_o_botao_anuncia_o_menu_e_a_lista_nao_existe()
    {
        var cortado = Menu();

        var botao = cortado.Find("button");
        Assert.Equal("menu", botao.GetAttribute("aria-haspopup"));
        Assert.Equal("false", botao.GetAttribute("aria-expanded"));
        Assert.Null(botao.GetAttribute("aria-controls"));
        Assert.Empty(cortado.FindAll("[role=menu]"));
    }

    [Fact]
    public void Clicar_abre_com_itens_e_divisor_e_clicar_de_novo_fecha()
    {
        var cortado = Menu();

        cortado.Find("button").Click();
        var menu = cortado.Find("[role=menu]");
        var botao = cortado.Find("button.botao");
        Assert.Equal("true", botao.GetAttribute("aria-expanded"));
        Assert.Equal(menu.Id, botao.GetAttribute("aria-controls"));
        Assert.Equal(botao.Id, menu.GetAttribute("aria-labelledby"));
        Assert.Equal(3, cortado.FindAll("[role=menuitem]").Count);
        Assert.Single(cortado.FindAll("[role=separator]"));

        cortado.Find("button.botao").Click();
        Assert.Empty(cortado.FindAll("[role=menu]"));
    }

    [Fact]
    public void Escolher_um_item_executa_e_fecha()
    {
        var cortado = Menu();
        cortado.Find("button").Click();

        cortado.FindAll("[role=menuitem]")[2].Click();

        Assert.Equal(["Excluir"], _acoes);
        Assert.Empty(cortado.FindAll("[role=menu]"));
    }

    [Fact]
    public void Item_desabilitado_nao_executa()
    {
        var cortado = Menu();
        cortado.Find("button").Click();

        var duplicar = cortado.FindAll("[role=menuitem]")[1];
        Assert.True(duplicar.HasAttribute("disabled"));
        duplicar.Click();

        Assert.Empty(_acoes);
    }

    [Theory]
    [InlineData("ArrowDown")]
    [InlineData("ArrowUp")]
    public void Setas_no_botao_abrem_o_menu(string tecla)
    {
        var cortado = Menu();

        cortado.Find("button").KeyDown(key: tecla);

        Assert.NotEmpty(cortado.FindAll("[role=menu]"));
    }

    [Fact]
    public void Outras_teclas_no_botao_nao_abrem()
    {
        var cortado = Menu();

        cortado.Find("button").KeyDown(key: "a");

        Assert.Empty(cortado.FindAll("[role=menu]"));
    }

    [Theory]
    [InlineData("Escape")]
    [InlineData("Tab")]
    public void Esc_e_tab_fecham(string tecla)
    {
        var cortado = Menu();
        cortado.Find("button").Click();

        cortado.Find("[role=menu]").KeyDown(key: tecla);

        Assert.Empty(cortado.FindAll("[role=menu]"));
    }

    [Fact]
    public void Setas_home_e_end_no_menu_mantem_aberto()
    {
        var cortado = Menu();
        cortado.Find("button").Click();

        foreach (var tecla in new[] { "ArrowDown", "ArrowUp", "Home", "End", "x" })
        {
            cortado.Find("[role=menu]").KeyDown(key: tecla);
        }

        Assert.NotEmpty(cortado.FindAll("[role=menu]"));
    }

    [Fact]
    public void Clicar_fora_fecha()
    {
        var cortado = Menu();
        cortado.Find("button").Click();

        cortado.Find(".fundo").Click();

        Assert.Empty(cortado.FindAll("[role=menu]"));
    }

    [Fact]
    public void Botao_desabilitado_nao_abre_e_atributos_se_dividem()
    {
        var cortado = Menu(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.AlignEnd, true)
            .AddUnmatched("class", "minha")
            .AddUnmatched("style", "margin: 0")
            .AddUnmatched("data-teste", "x"));

        var raiz = cortado.Find("div.menu");
        Assert.Equal("menu minha", raiz.GetAttribute("class"));
        Assert.Equal("margin: 0", raiz.GetAttribute("style"));
        var botao = cortado.Find("button");
        Assert.Equal("x", botao.GetAttribute("data-teste"));
        Assert.True(botao.HasAttribute("disabled"));
        botao.KeyDown(key: "ArrowDown");
        Assert.Empty(cortado.FindAll("[role=menu]"));
    }

    [Fact]
    public void Alinhado_ao_fim_ganha_classe()
    {
        var cortado = Menu(p => p.Add(x => x.AlignEnd, true));
        cortado.Find("button").Click();

        Assert.Contains("fim", cortado.Find("[role=menu]").GetAttribute("class"));
    }

    [Fact]
    public void Item_fora_de_um_rvmmenu_explica_o_erro()
    {
        var erro = Assert.Throws<InvalidOperationException>(() => Render<RvmMenuItem>());

        Assert.Contains("RvmMenu", erro.Message);
    }
}

public class RvmSelectTests : BunitContext
{
    public RvmSelectTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private enum Cultura { Soja, Milho, Cafe, Algodao }

    private sealed class Plantio
    {
        public Cultura? Principal { get; set; }

        public IReadOnlyList<string> Talhoes { get; set; } = [];
    }

    private static readonly Cultura?[] Culturas = [Cultura.Soja, Cultura.Milho, Cultura.Cafe, Cultura.Algodao];

    private static string Nome(Cultura? c) => c switch
    {
        Cultura.Soja => "Soja",
        Cultura.Milho => "Milho",
        Cultura.Cafe => "Café",
        Cultura.Algodao => "Algodão",
        _ => string.Empty
    };

    private IRenderedComponent<RvmSelect<Cultura?>> Select(Plantio modelo,
        Action<ComponentParameterCollectionBuilder<RvmSelect<Cultura?>>>? extra = null,
        EditContext? contexto = null)
        => Render<RvmSelect<Cultura?>>(p =>
        {
            if (contexto is not null) p.AddCascadingValue(contexto);
            p.Add(x => x.Items, Culturas)
             .Add(x => x.ItemText, Nome)
             .Add(x => x.Label, "Cultura")
             .Add(x => x.Value, modelo.Principal)
             .Add(x => x.ValueChanged, EventCallback.Factory.Create<Cultura?>(this, v => modelo.Principal = v))
             .Add(x => x.ValueExpression, () => modelo.Principal);
            extra?.Invoke(p);
        });

    [Fact]
    public void Fechado_e_um_combobox_com_rotulo_e_sem_lista()
    {
        var cortado = Select(new Plantio());

        var combo = cortado.Find("[role=combobox]");
        Assert.Equal("listbox", combo.GetAttribute("aria-haspopup"));
        Assert.Equal("false", combo.GetAttribute("aria-expanded"));
        Assert.Equal("0", combo.GetAttribute("tabindex"));
        var rotulo = cortado.Find(".rotulo");
        Assert.Equal(rotulo.Id, combo.GetAttribute("aria-labelledby"));
        Assert.Empty(cortado.FindAll("[role=listbox]"));
        Assert.DoesNotContain("rotulo-fixo", cortado.Find("div.campo").GetAttribute("class"));
    }

    [Fact]
    public void Clicar_abre_a_lista_e_escolher_fecha_e_liga_o_valor()
    {
        var modelo = new Plantio();
        var cortado = Select(modelo);

        cortado.Find("[role=combobox]").Click();
        var lista = cortado.Find("[role=listbox]");
        Assert.Equal(lista.Id, cortado.Find("[role=combobox]").GetAttribute("aria-controls"));
        Assert.Equal(4, cortado.FindAll("[role=option]").Count);
        Assert.Equal(cortado.FindAll("[role=option]")[0].Id, cortado.Find("[role=combobox]").GetAttribute("aria-activedescendant"));

        cortado.FindAll("[role=option]")[2].Click();

        Assert.Equal(Cultura.Cafe, modelo.Principal);
        Assert.Empty(cortado.FindAll("[role=listbox]"));
        Assert.Equal("Café", cortado.Find(".valor").TextContent);
        Assert.Contains("rotulo-fixo", cortado.Find("div.campo").GetAttribute("class"));
    }

    [Fact]
    public void Teclado_abre_move_e_escolhe_pulando_desabilitada()
    {
        var modelo = new Plantio();
        var cortado = Select(modelo, p => p.Add(x => x.ItemDisabled, c => c == Cultura.Milho));
        var combo = cortado.Find("[role=combobox]");

        combo.KeyDown(key: "ArrowDown");
        Assert.NotEmpty(cortado.FindAll("[role=listbox]"));
        Assert.Equal("true", cortado.FindAll("[role=option]")[1].GetAttribute("aria-disabled"));

        cortado.Find("[role=combobox]").KeyDown(key: "ArrowDown");
        Assert.Contains("ativa", cortado.FindAll("[role=option]")[2].GetAttribute("class"));

        cortado.Find("[role=combobox]").KeyDown(key: "End");
        cortado.Find("[role=combobox]").KeyDown(key: "ArrowUp");
        cortado.Find("[role=combobox]").KeyDown(key: "Home");
        cortado.Find("[role=combobox]").KeyDown(key: "ArrowDown");
        cortado.Find("[role=combobox]").KeyDown(key: " ");

        Assert.Equal(Cultura.Cafe, modelo.Principal);
    }

    [Theory]
    [InlineData("Enter", 0)]
    [InlineData(" ", 0)]
    [InlineData("Home", 0)]
    [InlineData("ArrowUp", 3)]
    [InlineData("End", 3)]
    public void Teclas_que_abrem_e_onde_a_ativa_comeca(string tecla, int ativa)
    {
        var cortado = Select(new Plantio());

        cortado.Find("[role=combobox]").KeyDown(key: tecla);

        Assert.Contains("ativa", cortado.FindAll("[role=option]")[ativa].GetAttribute("class"));
    }

    [Fact]
    public void Abrir_com_valor_ativa_a_escolhida()
    {
        var cortado = Select(new Plantio { Principal = Cultura.Cafe });

        cortado.Find("[role=combobox]").Click();

        var opcao = cortado.FindAll("[role=option]")[2];
        Assert.Contains("ativa", opcao.GetAttribute("class"));
        Assert.Equal("true", opcao.GetAttribute("aria-selected"));
    }

    [Theory]
    [InlineData("Escape")]
    [InlineData("Tab")]
    public void Esc_e_tab_fecham_sem_escolher(string tecla)
    {
        var modelo = new Plantio();
        var cortado = Select(modelo);
        cortado.Find("[role=combobox]").Click();

        cortado.Find("[role=combobox]").KeyDown(key: tecla);

        Assert.Empty(cortado.FindAll("[role=listbox]"));
        Assert.Null(modelo.Principal);
    }

    [Fact]
    public void Clicar_fora_ou_no_gatilho_fecha()
    {
        var cortado = Select(new Plantio());
        cortado.Find("[role=combobox]").Click();
        cortado.Find(".fundo").Click();
        Assert.Empty(cortado.FindAll("[role=listbox]"));

        cortado.Find("[role=combobox]").Click();
        cortado.Find("[role=combobox]").Click();
        Assert.Empty(cortado.FindAll("[role=listbox]"));
    }

    [Fact]
    public void Tecla_solta_fechado_nao_abre_e_desabilitado_nao_abre_nada()
    {
        var cortado = Select(new Plantio(), p => p.Add(x => x.Disabled, true));
        var combo = cortado.Find("[role=combobox]");
        Assert.Equal("-1", combo.GetAttribute("tabindex"));
        Assert.Equal("true", combo.GetAttribute("aria-disabled"));

        combo.Click();
        cortado.Find("[role=combobox]").KeyDown(key: "ArrowDown");
        Assert.Empty(cortado.FindAll("[role=listbox]"));

        var outro = Select(new Plantio());
        outro.Find("[role=combobox]").KeyDown(key: "x");
        Assert.Empty(outro.FindAll("[role=listbox]"));
    }

    [Fact]
    public void Busca_filtra_sem_acento_e_enter_escolhe_a_primeira()
    {
        var modelo = new Plantio();
        var cortado = Select(modelo, p => p.Add(x => x.Searchable, true));
        cortado.Find("[role=combobox]").Click();

        var busca = cortado.Find("input.busca");
        Assert.Equal("Buscar", busca.GetAttribute("aria-label"));
        Assert.Null(cortado.Find("[role=combobox]").GetAttribute("aria-activedescendant"));

        busca.Input("cafe");
        Assert.Single(cortado.FindAll("[role=option]"));
        Assert.Equal(cortado.Find("[role=option]").Id, cortado.Find("input.busca").GetAttribute("aria-activedescendant"));

        cortado.Find("input.busca").KeyDown(key: "Enter");
        Assert.Equal(Cultura.Cafe, modelo.Principal);
    }

    [Fact]
    public void Busca_sem_resultado_avisa_e_setas_nao_quebram()
    {
        var cortado = Select(new Plantio(), p => p.Add(x => x.Searchable, true).Add(x => x.NoResultsText, "Nada aqui."));
        cortado.Find("[role=combobox]").Click();

        cortado.Find("input.busca").Input("trigo");
        cortado.Find("input.busca").KeyDown(key: "ArrowDown");
        cortado.Find("input.busca").KeyDown(key: "ArrowUp");
        cortado.Find("input.busca").KeyDown(key: "Enter");

        Assert.Equal("Nada aqui.", cortado.Find(".vazio").TextContent);
        Assert.Empty(cortado.FindAll("[role=option]"));

        cortado.Find("input.busca").KeyDown(key: "Escape");
        Assert.Empty(cortado.FindAll("[role=listbox]"));
    }

    [Fact]
    public void Valor_fora_das_opcoes_nao_conta_como_escolhido()
    {
        var cortado = Render<RvmSelect<int>>(p => p.Add(x => x.Items, new[] { 1, 2, 3 }).Add(x => x.Value, 0).Add(x => x.Placeholder, "Escolha"));

        Assert.Equal("Escolha", cortado.Find(".valor").TextContent);
        Assert.Contains("rotulo-fixo", cortado.Find("div.campo").GetAttribute("class"));
    }

    [Fact]
    public void Sem_itemtext_usa_tostring_e_escolher_a_mesma_nao_dispara()
    {
        var disparos = 0;
        var cortado = Render<RvmSelect<int>>(p => p
            .Add(x => x.Items, new[] { 1, 2 })
            .Add(x => x.Value, 2)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int>(this, _ => disparos++)));

        Assert.Equal("2", cortado.Find(".valor").TextContent);
        cortado.Find("[role=combobox]").Click();
        cortado.FindAll("[role=option]")[1].Click();

        Assert.Equal(0, disparos);
    }

    [Fact]
    public void Name_gera_input_hidden_com_o_valor()
    {
        var cortado = Select(new Plantio { Principal = Cultura.Milho }, p => p.Add(x => x.Name, "cultura"));

        var escondido = cortado.Find("input[type=hidden]");
        Assert.Equal("cultura", escondido.GetAttribute("name"));
        Assert.Equal("Milho", escondido.GetAttribute("value"));
    }

    [Fact]
    public void Dentro_do_editform_mostra_a_validacao_e_marca_modificado()
    {
        var modelo = new Plantio();
        var contexto = new EditContext(modelo);
        var mensagens = new ValidationMessageStore(contexto);
        var cortado = Select(modelo, p => p.Add(x => x.Required, true).Add(x => x.HelperText, "A principal do talhao"), contexto);

        Assert.Equal("A principal do talhao", cortado.Find(".apoio").TextContent);
        Assert.Equal("true", cortado.Find("[role=combobox]").GetAttribute("aria-required"));

        mensagens.Add(contexto.Field(nameof(Plantio.Principal)), "Escolha a cultura.");
        cortado.InvokeAsync(contexto.NotifyValidationStateChanged);

        var combo = cortado.Find("[role=combobox]");
        Assert.Equal("true", combo.GetAttribute("aria-invalid"));
        Assert.Equal(cortado.Find(".apoio").Id, combo.GetAttribute("aria-describedby"));
        Assert.Equal("Escolha a cultura.", cortado.Find(".apoio").TextContent);
        Assert.Contains("erro", cortado.Find("div.campo").GetAttribute("class"));

        cortado.Find("[role=combobox]").Click();
        cortado.FindAll("[role=option]")[0].Click();
        Assert.True(contexto.IsModified(contexto.Field(nameof(Plantio.Principal))));
    }

    [Fact]
    public void Erro_informado_por_fora_vence_o_apoio()
    {
        var cortado = Select(new Plantio(), p => p.Add(x => x.HelperText, "apoio").Add(x => x.ErrorText, "Servico fora do ar."));

        Assert.Equal("Servico fora do ar.", cortado.Find(".apoio").TextContent);
    }

    [Fact]
    public void Expressao_que_nao_e_propriedade_explica_o_erro()
    {
        var contexto = new EditContext(new Plantio());

        var erro = Assert.Throws<ArgumentException>(() => Render<RvmSelect<int>>(p => p
            .AddCascadingValue(contexto)
            .Add(x => x.Items, new[] { 1 })
            .Add(x => x.ValueExpression, () => 1)));

        Assert.Contains("propriedade", erro.Message);
    }

    [Theory]
    [InlineData(RvmTextFieldVariant.Outlined, RvmSize.Medium, "campo contorno medio")]
    [InlineData(RvmTextFieldVariant.Filled, RvmSize.Small, "campo preenchido pequeno")]
    [InlineData(RvmTextFieldVariant.Standard, RvmSize.Medium, "campo padrao medio")]
    public void Estilo_e_tamanho_viram_classes(RvmTextFieldVariant variante, RvmSize tamanho, string esperado)
    {
        var cortado = Select(new Plantio(), p => p.Add(x => x.Variant, variante).Add(x => x.Size, tamanho));

        Assert.StartsWith(esperado, cortado.Find("div.campo").GetAttribute("class"));
    }

    [Fact]
    public void Atributos_se_dividem_entre_raiz_e_combobox()
    {
        var cortado = Select(new Plantio(), p => p
            .AddUnmatched("class", "minha")
            .AddUnmatched("style", "width: 200px")
            .AddUnmatched("id", "cultura")
            .AddUnmatched("data-teste", "x"));

        var raiz = cortado.Find("div.campo");
        Assert.EndsWith("minha", raiz.GetAttribute("class"));
        Assert.Equal("width: 200px", raiz.GetAttribute("style"));
        var combo = cortado.Find("[role=combobox]");
        Assert.Equal("cultura", combo.Id);
        Assert.Equal("x", combo.GetAttribute("data-teste"));
    }

    [Fact]
    public void Multiplo_marca_varias_na_ordem_da_lista_e_fica_aberto()
    {
        var modelo = new Plantio();
        string[] talhoes = ["T1", "T2", "T3"];
        var cortado = Render<RvmMultiSelect<string>>(p => p
            .Add(x => x.Items, talhoes)
            .Add(x => x.Label, "Talhoes")
            .Add(x => x.Name, "talhoes")
            .Add(x => x.Values, modelo.Talhoes)
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => modelo.Talhoes = v))
            .Add(x => x.ValuesExpression, () => modelo.Talhoes));

        cortado.Find("[role=combobox]").Click();
        Assert.Equal("true", cortado.Find("[role=listbox]").GetAttribute("aria-multiselectable"));

        cortado.FindAll("[role=option]")[2].Click();
        cortado.FindAll("[role=option]")[0].Click();

        Assert.Equal(["T1", "T3"], modelo.Talhoes);
        Assert.NotEmpty(cortado.FindAll("[role=listbox]"));
        Assert.Equal(2, cortado.FindAll(".marca svg").Count);
        Assert.Equal("T1, T3", cortado.Find(".valor").TextContent);
        Assert.Equal(["T1", "T3"], cortado.FindAll("input[type=hidden]").Select(i => i.GetAttribute("value")));

        cortado.FindAll("[role=option]")[0].Click();
        Assert.Equal(["T3"], modelo.Talhoes);
    }
}
