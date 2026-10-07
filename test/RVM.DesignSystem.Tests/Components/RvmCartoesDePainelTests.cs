using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Avatar;
using RVM.DesignSystem.Components.Cards;
using RVM.DesignSystem.Components.IconBadge;
using RVM.DesignSystem.Components.Menu;
using RVM.DesignSystem.Components.Progress;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>
/// Cards de painel do contrato com o RVM.UI (DSGN-017): RvmStatCard, RvmProgressCard, RvmProjectCard, RvmTaskCard,
/// RvmPaymentCard e RvmCurrencyConverter.
/// </summary>
public class RvmCartoesDePainelTests : BunitContext
{
    public RvmCartoesDePainelTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private static RenderFragment Itens(string texto) => b =>
    {
        b.OpenComponent<RvmMenuItem>(0);
        b.AddAttribute(1, "ChildContent", (RenderFragment)(c => c.AddContent(0, texto)));
        b.CloseComponent();
    };

    // --- RvmStatCard ---

    [Fact]
    public void Stat_card_tem_titulo_como_h3_numero_e_tendencia_com_texto_para_o_leitor()
    {
        var cortado = Render<RvmStatCard>(p => p
            .Add(x => x.Title, "Sacas colhidas")
            .Add(x => x.Subtitle, "Safra 25/26")
            .Add(x => x.Value, "48.320")
            .Add(x => x.Trend, RvmStatCardTrend.Up)
            .Add(x => x.TrendValue, "12%")
            .Add(x => x.TrendDetail, "sobre a safra passada")
            .Add(x => x.Class, "minha")
            .AddUnmatched("data-testid", "sacas"));

        var raiz = cortado.Find(".rvm-stat-card");
        Assert.Contains("minha", raiz.ClassList);
        Assert.Contains("rvm-claro", raiz.ClassList);
        Assert.Contains("rvm-pequeno", raiz.ClassList);
        Assert.Equal("sacas", raiz.GetAttribute("data-testid"));
        Assert.Equal("Sacas colhidas", cortado.Find("h3").TextContent);
        Assert.Equal("48.320", cortado.Find(".rvm-valor").TextContent);
        var tendencia = cortado.Find(".rvm-tendencia");
        Assert.Contains("rvm-sobe", tendencia.ClassList);
        Assert.Equal("Alta de 12%", tendencia.TextContent.Trim());
        Assert.Equal("sobre a safra passada", cortado.Find(".rvm-detalhe").TextContent);
    }

    [Fact]
    public void Stat_card_em_queda_diz_queda_e_tamanho_grande_aumenta_o_numero()
    {
        var cortado = Render<RvmStatCard>(p => p
            .Add(x => x.Title, "Receita")
            .Add(x => x.Value, "R$ 1,2 mi")
            .Add(x => x.Size, RvmStatCardSize.Large)
            .Add(x => x.Trend, RvmStatCardTrend.Down)
            .Add(x => x.TrendValue, "3%"));

        Assert.Equal("Queda de 3%", cortado.Find(".rvm-desce").TextContent.Trim());
        Assert.Contains("rvm-text-h4", cortado.Find(".rvm-valor").ClassList);
        Assert.Contains("rvm-grande", cortado.Find(".rvm-stat-card").ClassList);
    }

    [Fact]
    public void Stat_card_cheio_usa_o_papel_e_passa_inverse_as_pecas()
    {
        var cortado = Render<RvmStatCard>(p => p
            .Add(x => x.Title, "Silos")
            .Add(x => x.Value, "9 de 12")
            .Add(x => x.Surface, RvmSurface.Dark)
            .Add(x => x.Color, RvmColor.Warning)
            .Add(x => x.Size, RvmStatCardSize.XtraLarge)
            .Add(x => x.Adornment, RvmStatCardAdornment.Icon)
            .Add(x => x.AdornmentIcon, RvmIconName.Box)
            .Add(x => x.Visual, RvmStatCardVisual.Progress)
            .Add(x => x.VisualValue, 75));

        var raiz = cortado.Find(".rvm-stat-card");
        Assert.Contains("rvm-cheio", raiz.ClassList);
        Assert.Contains("rvm-warning", raiz.ClassList);
        Assert.Contains("rvm-extra", raiz.ClassList);
        Assert.Contains("rvm-inverse", cortado.Find(".rvm-icon-badge").ClassList);
        var barra = cortado.Find("[role=progressbar]");
        Assert.Equal("Silos", barra.GetAttribute("aria-label"));
        Assert.Equal("75", barra.GetAttribute("aria-valuenow"));
        Assert.Equal(RvmSurface.Dark, cortado.FindComponent<RvmProgressBar>().Instance.Surface);
    }

    [Fact]
    public void Stat_card_com_selo_imagem_e_rotulo_proprio_do_progresso()
    {
        var comSelo = Render<RvmStatCard>(p => p
            .Add(x => x.Title, "Custo")
            .Add(x => x.Value, "R$ 4.870")
            .Add(x => x.Adornment, RvmStatCardAdornment.Badge)
            .Add(x => x.AdornmentIcon, RvmIconName.Wallet)
            .Add(x => x.Color, RvmColor.Info)
            .Add(x => x.Visual, RvmStatCardVisual.Image)
            .Add(x => x.ImageUrl, "lavoura.svg"));

        Assert.Contains("rvm-info", comSelo.Find(".rvm-artistic-icon-badge").ClassList);
        var img = comSelo.Find("img.rvm-imagem");
        Assert.Equal("lavoura.svg", img.GetAttribute("src"));
        Assert.Equal("", img.GetAttribute("alt"));

        var comProgresso = Render<RvmStatCard>(p => p
            .Add(x => x.Title, "Meta")
            .Add(x => x.Value, "68%")
            .Add(x => x.Visual, RvmStatCardVisual.Progress)
            .Add(x => x.VisualValue, 68)
            .Add(x => x.VisualLabel, "Meta de vendas atingida"));
        Assert.Equal("Meta de vendas atingida", comProgresso.Find("[role=progressbar]").GetAttribute("aria-label"));
    }

    [Fact]
    public void Stat_card_com_chart_ignora_o_visual()
    {
        var cortado = Render<RvmStatCard>(p => p
            .Add(x => x.Title, "Receita")
            .Add(x => x.Value, "R$ 412 mil")
            .Add(x => x.Visual, RvmStatCardVisual.Progress)
            .Add(x => x.Chart, b => b.AddMarkupContent(0, "<svg class=\"meu-grafico\"></svg>")));

        Assert.Single(cortado.FindAll(".meu-grafico"));
        Assert.Empty(cortado.FindAll("[role=progressbar]"));
    }

    [Fact]
    public async Task Stat_card_botao_de_acao_tem_nome_e_avisa_o_clique()
    {
        var cliques = 0;
        var cortado = Render<RvmStatCard>(p => p
            .Add(x => x.Title, "Notas")
            .Add(x => x.Value, "14")
            .Add(x => x.Action, RvmCardAction.Button)
            .Add(x => x.ActionLabel, "Emitir nota")
            .Add(x => x.OnActionClick, () => cliques++));

        var botao = cortado.Find("button[aria-label='Emitir nota']");
        await botao.ClickAsync(new());
        Assert.Equal(1, cliques);
    }

    [Fact]
    public void Stat_card_menu_tem_nome_e_abre_os_itens()
    {
        var cortado = Render<RvmStatCard>(p => p
            .Add(x => x.Title, "Custo")
            .Add(x => x.Value, "R$ 4.870")
            .Add(x => x.Action, RvmCardAction.Menu)
            .Add(x => x.ActionLabel, "Opcoes do custo")
            .Add(x => x.ActionMenu, Itens("Exportar")));

        var gatilho = cortado.Find("button[aria-label='Opcoes do custo']");
        Assert.Equal("menu", gatilho.GetAttribute("aria-haspopup"));
        gatilho.Click();
        Assert.Equal("Exportar", cortado.Find("[role=menuitem]").TextContent.Trim());
    }

    [Theory]
    [InlineData(RvmStatCardAdornment.Icon, null, RvmStatCardTrend.None, null, RvmCardAction.None, null, nameof(RvmStatCard.AdornmentIcon))]
    [InlineData(RvmStatCardAdornment.None, null, RvmStatCardTrend.Up, null, RvmCardAction.None, null, nameof(RvmStatCard.TrendValue))]
    [InlineData(RvmStatCardAdornment.None, null, RvmStatCardTrend.None, null, RvmCardAction.Button, null, nameof(RvmStatCard.ActionLabel))]
    [InlineData(RvmStatCardAdornment.None, null, RvmStatCardTrend.None, null, RvmCardAction.Menu, "Menu", nameof(RvmStatCard.ActionMenu))]
    public void Stat_card_recusa_combinacao_incompleta(RvmStatCardAdornment enfeite, RvmIconName? icone, RvmStatCardTrend tendencia,
        string? valorDaTendencia, RvmCardAction acao, string? rotulo, string parametro)
    {
        var erro = Assert.Throws<ArgumentException>(() => Render<RvmStatCard>(p => p
            .Add(x => x.Title, "T")
            .Add(x => x.Value, "1")
            .Add(x => x.Adornment, enfeite)
            .Add(x => x.AdornmentIcon, icone)
            .Add(x => x.Trend, tendencia)
            .Add(x => x.TrendValue, valorDaTendencia)
            .Add(x => x.Action, acao)
            .Add(x => x.ActionLabel, rotulo)));
        Assert.Equal(parametro, erro.ParamName);
    }

    [Fact]
    public void Stat_card_recusa_imagem_sem_endereco_e_progresso_sem_valor()
    {
        Assert.Equal(nameof(RvmStatCard.ImageUrl), Assert.Throws<ArgumentException>(() => Render<RvmStatCard>(p => p
            .Add(x => x.Title, "T").Add(x => x.Value, "1").Add(x => x.Visual, RvmStatCardVisual.Image))).ParamName);
        Assert.Equal(nameof(RvmStatCard.VisualValue), Assert.Throws<ArgumentException>(() => Render<RvmStatCard>(p => p
            .Add(x => x.Title, "T").Add(x => x.Value, "1").Add(x => x.Visual, RvmStatCardVisual.Progress))).ParamName);
    }

    [Fact]
    public void Stat_card_padroes_do_RVM_UI()
    {
        var card = new RvmStatCard();
        Assert.Equal(RvmStatCardSize.Small, card.Size);
        Assert.Equal(RvmColor.Accent, card.Color);
        Assert.Equal(RvmSurface.Light, card.Surface);
        Assert.Equal(RvmIconName.Add, card.ActionIcon);
        Assert.Equal(RvmCardAction.None, card.Action);
        Assert.Equal(RvmStatCardVisual.None, card.Visual);
    }

    [Fact]
    public void Card_cheio_nao_redefine_token_do_tema_no_escopo_que_contem_o_menu()
    {
        // A lista do RvmMenu e descendente do card: token do tema redefinido na raiz (ou em qualquer ancestral dela)
        // vaza para o menu aberto — texto branco no papel branco, grafite no papel escuro (review da DSGN-017).
        // Redefinir token do tema so pode no elemento de uma peca do DS (::deep .peca), nunca na lista nem no menu.
        var pasta = Path.Combine(RaizDoRepositorio.Caminho, "src", "RVM.DesignSystem", "Components", "Cards");
        var violacoes = new List<string>();
        foreach (var arquivo in Directory.GetFiles(pasta, "*.razor.css"))
        {
            var css = System.Text.RegularExpressions.Regex.Replace(File.ReadAllText(arquivo), @"/\*.*?\*/", "",
                System.Text.RegularExpressions.RegexOptions.Singleline);
            foreach (System.Text.RegularExpressions.Match regra in System.Text.RegularExpressions.Regex.Matches(css, @"([^{}]+)\{([^{}]*)\}"))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(regra.Groups[2].Value, @"(^|;)\s*--rvm-(color|focus)-"))
                {
                    continue;
                }
                violacoes.AddRange(regra.Groups[1].Value.Split(',').Select(s => s.Trim())
                    .Where(s => !s.Contains("::deep", StringComparison.Ordinal) || s.Contains("rvm-lista", StringComparison.Ordinal)
                                || s.EndsWith(".rvm-menu", StringComparison.Ordinal))
                    .Select(s => $"{Path.GetFileName(arquivo)}: {s}"));
            }
        }

        Assert.Empty(violacoes);
    }

    // --- RvmProgressCard ---

    [Fact]
    public void Progress_card_mostra_valor_e_porcentagem_e_a_barra_tem_titulo_e_valor_no_nome()
    {
        var cortado = Render<RvmProgressCard>(p => p
            .Add(x => x.Icon, RvmIconName.Wallet)
            .Add(x => x.Title, "Orcamento de insumos")
            .Add(x => x.Description, "Safra 25/26")
            .Add(x => x.Value, "R$ 312.000")
            .Add(x => x.Percent, 78.4)
            .Add(x => x.Color, RvmColor.Success)
            .Add(x => x.Class, "minha"));

        Assert.Contains("minha", cortado.Find(".rvm-progress-card").ClassList);
        Assert.Equal("Orcamento de insumos", cortado.Find("h3").TextContent);
        Assert.Equal("Safra 25/26", cortado.Find(".rvm-descricao").TextContent);
        Assert.Equal("R$ 312.000", cortado.Find(".rvm-quantia").TextContent);
        Assert.Equal("78%", cortado.Find(".rvm-porcentagem").TextContent);
        Assert.Equal("true", cortado.Find(".rvm-linha").GetAttribute("aria-hidden"));
        var barra = cortado.Find("[role=progressbar]");
        Assert.Equal("Orcamento de insumos: R$ 312.000", barra.GetAttribute("aria-label"));
        Assert.Equal("78%", barra.GetAttribute("aria-valuetext"));
        var selo = cortado.Find(".rvm-icon-badge");
        Assert.Contains("rvm-neutro", selo.ClassList);
        Assert.Contains("rvm-success", selo.ClassList);
        Assert.Empty(cortado.FindAll(".rvm-menu"));
    }

    [Fact]
    public void Progress_card_com_menu_usa_o_nome_padrao_e_mostra_o_rodape()
    {
        var cortado = Render<RvmProgressCard>(p => p
            .Add(x => x.Icon, RvmIconName.Wallet)
            .Add(x => x.Title, "Meta")
            .Add(x => x.Value, "R$ 1,4 mi")
            .Add(x => x.Percent, 140)
            .Add(x => x.IconVariant, RvmIconBadgeVariant.Solid)
            .Add(x => x.MenuItems, Itens("Editar"))
            .Add(x => x.Footer, b => b.AddContent(0, "Faltam 23 dias")));

        Assert.NotNull(cortado.Find("button[aria-label='Mais acoes']"));
        Assert.Equal("100%", cortado.Find(".rvm-porcentagem").TextContent);
        Assert.Equal("Faltam 23 dias", cortado.Find(".rvm-rodape").TextContent);
        Assert.Contains("rvm-cheio", cortado.Find(".rvm-icon-badge").ClassList);
    }

    // --- RvmTaskCard e RvmProjectCard ---

    private static readonly IReadOnlyList<RvmAvatarItem> Equipe =
        [new("Ana Souza"), new("Bruno Lima"), new("Carla Dias"), new("Diego Rocha"), new("Elisa Prado")];

    [Fact]
    public void Task_card_tem_selo_titulo_progresso_equipe_e_prazo_legiveis()
    {
        var cortado = Render<RvmTaskCard>(p => p
            .Add(x => x.Label, "Bloqueada")
            .Add(x => x.Title, "Conciliar notas")
            .Add(x => x.Description, "Aguardando XML")
            .Add(x => x.ProgressText, "Bloqueada")
            .Add(x => x.ProgressPercent, 40)
            .Add(x => x.ProgressColor, RvmColor.Error)
            .Add(x => x.Members, Equipe)
            .Add(x => x.DueDate, "12 out 2026")
            .Add(x => x.Class, "minha"));

        var raiz = cortado.Find(".rvm-task-card");
        Assert.Contains("minha", raiz.ClassList);
        Assert.Empty(cortado.FindAll(".rvm-capa"));
        Assert.Contains("rvm-error", cortado.Find(".rvm-etiqueta").ClassList);
        Assert.Equal("Conciliar notas", cortado.Find("h3").TextContent);
        Assert.Contains("rvm-error", cortado.Find(".rvm-texto-do-progresso").ClassList);
        Assert.Equal("Progresso de Conciliar notas", cortado.Find("[role=progressbar]").GetAttribute("aria-label"));
        var grupo = cortado.Find("[role=group]");
        Assert.Equal("Responsaveis", grupo.GetAttribute("aria-label"));
        Assert.Contains("+2", grupo.TextContent);
        Assert.Equal("Prazo: 12 out 2026", cortado.Find(".rvm-prazo").TextContent.Trim());
    }

    [Fact]
    public void Task_card_sem_equipe_nem_prazo_nao_tem_rodape_e_menu_tem_nome()
    {
        var cortado = Render<RvmTaskCard>(p => p
            .Add(x => x.Title, "Calibrar")
            .Add(x => x.ProgressText, "3 de 5")
            .Add(x => x.MenuLabel, "Opcoes da tarefa")
            .Add(x => x.MenuItems, Itens("Editar")));

        Assert.Empty(cortado.FindAll(".rvm-rodape"));
        Assert.Empty(cortado.FindAll(".rvm-etiqueta"));
        Assert.NotNull(cortado.Find("button[aria-label='Opcoes da tarefa']"));
    }

    [Fact]
    public void Project_card_tem_capa_e_os_padroes_do_RVM_UI()
    {
        var comCapa = Render<RvmProjectCard>(p => p
            .Add(x => x.ImageUrl, "soja.png")
            .Add(x => x.Title, "Plantio")
            .Add(x => x.ProgressText, "72%")
            .Add(x => x.Members, Equipe));

        Assert.NotNull(comCapa.Find(".rvm-project-card"));
        var capa = comCapa.Find("img.rvm-capa");
        Assert.Equal("soja.png", capa.GetAttribute("src"));
        Assert.Equal("", capa.GetAttribute("alt"));
        Assert.Equal("Equipe do projeto", comCapa.Find("[role=group]").GetAttribute("aria-label"));
        Assert.Contains("+1", comCapa.Find("[role=group]").TextContent);

        var semCapa = Render<RvmProjectCard>(p => p.Add(x => x.Title, "Auditoria").Add(x => x.ProgressText, "Atrasado"));
        Assert.Empty(semCapa.FindAll("img.rvm-capa"));
        Assert.NotNull(semCapa.Find(".rvm-icon-badge"));

        Assert.Equal(4, new RvmProjectCard().MembersMax);
        Assert.Equal(3, new RvmTaskCard().MembersMax);
        Assert.Equal(RvmColor.Danger, new RvmTaskCard().LabelColor);
        Assert.Equal(RvmColor.Accent, new RvmTaskCard().ProgressColor);
        Assert.Equal("Mais acoes", new RvmProjectCard().MenuLabel);
    }

    // --- RvmPaymentCard ---

    [Fact]
    public void Payment_card_mascara_o_numero_para_o_leitor_e_mostra_saldo_e_validade()
    {
        var cortado = Render<RvmPaymentCard>(p => p
            .Add(x => x.Balance, "R$ 12.430,00")
            .Add(x => x.CardNumber, "**** **** **** 9090")
            .Add(x => x.ExpiresAt, "07/28")
            .Add(x => x.Color, RvmColor.Warning)
            .Add(x => x.Class, "minha")
            .Add(x => x.NetworkMark, b => b.AddMarkupContent(0, "<svg class=\"bandeira\"></svg>")));

        var raiz = cortado.Find(".rvm-payment-card");
        Assert.Contains("minha", raiz.ClassList);
        Assert.DoesNotContain("rvm-com-acoes", raiz.ClassList);
        var face = cortado.Find(".rvm-face");
        Assert.Contains("rvm-cheio", face.ClassList);
        Assert.Contains("rvm-warning", face.ClassList);
        Assert.Equal("Saldo", cortado.Find(".rvm-rotulo").TextContent);
        Assert.Equal("R$ 12.430,00", cortado.Find(".rvm-valor").TextContent);
        Assert.Equal("true", cortado.Find(".rvm-numero").GetAttribute("aria-hidden"));
        Assert.Contains(cortado.FindAll(".rvm-so-leitor"), s => s.TextContent == "Cartao final 9090");
        Assert.Equal("Validade 07/28", cortado.Find(".rvm-validade").TextContent);
        Assert.Equal("true", cortado.Find(".rvm-bandeira").GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Payment_card_com_acoes_e_numero_sem_digito()
    {
        var cortado = Render<RvmPaymentCard>(p => p
            .Add(x => x.Balance, "R$ 0,00")
            .Add(x => x.BalanceLabel, "Limite")
            .Add(x => x.CardNumber, "Virtual")
            .Add(x => x.ExpiresAt, "02/29")
            .Add(x => x.Actions, b => b.AddMarkupContent(0, "<button>Ver fatura</button>")));

        Assert.Contains("rvm-com-acoes", cortado.Find(".rvm-payment-card").ClassList);
        Assert.Equal("Ver fatura", cortado.Find(".rvm-acoes button").TextContent);
        Assert.Equal("Limite", cortado.Find(".rvm-rotulo").TextContent);
        Assert.Contains(cortado.FindAll(".rvm-so-leitor"), s => s.TextContent == "Cartao Virtual");
        Assert.Equal(RvmColor.Accent, new RvmPaymentCard().Color);
    }

    // --- RvmCurrencyConverter ---

    [Fact]
    public async Task Conversor_e_secao_com_nome_taxa_anunciada_e_botao_que_avisa()
    {
        var conversoes = 0;
        var cortado = Render<RvmCurrencyConverter>(p => p
            .Add(x => x.Title, "Conversor de moeda")
            .Add(x => x.Subtitle, "Cotacao comercial")
            .Add(x => x.Rate, "1 USD = 5,42 BRL")
            .Add(x => x.From, b => b.AddMarkupContent(0, "<input id=\"de\" />"))
            .Add(x => x.To, b => b.AddMarkupContent(0, "<input id=\"para\" />"))
            .Add(x => x.OnConvert, () => conversoes++)
            .Add(x => x.Class, "minha"));

        var secao = cortado.Find("section");
        Assert.Contains("minha", secao.ClassList);
        var titulo = cortado.Find("h3");
        Assert.Equal("Conversor de moeda", titulo.TextContent);
        Assert.Equal(titulo.Id, secao.GetAttribute("aria-labelledby"));
        Assert.NotNull(cortado.Find("#de"));
        Assert.NotNull(cortado.Find("#para"));
        var taxa = cortado.Find("[role=status]");
        Assert.Equal("1 USD = 5,42 BRL", taxa.TextContent.Trim());
        Assert.DoesNotContain("rvm-sem-taxa", taxa.ClassList);

        var botao = cortado.Find("button");
        Assert.Equal("Converter", botao.TextContent.Trim());
        await botao.ClickAsync(new());
        Assert.Equal(1, conversoes);
        Assert.Empty(cortado.FindAll(".rvm-menu"));
    }

    [Fact]
    public void Conversor_sem_taxa_mantem_a_regiao_e_desabilita_e_tem_menu_com_nome()
    {
        var cortado = Render<RvmCurrencyConverter>(p => p
            .Add(x => x.Title, "Conversor")
            .Add(x => x.Disabled, true)
            .Add(x => x.Menu, Itens("Atualizar cotacao")));

        Assert.Contains("rvm-sem-taxa", cortado.Find("[role=status]").ClassList);
        Assert.NotNull(cortado.Find("button[aria-label='Acoes do conversor']"));
        Assert.True(cortado.Find(".rvm-botao-converter").HasAttribute("disabled"));

        var padrao = new RvmCurrencyConverter();
        Assert.Equal("Converter", padrao.ActionText);
        Assert.Equal(RvmIconName.Login3, padrao.ActionIcon);
        Assert.Equal(RvmColor.Accent, padrao.Color);
    }
}
