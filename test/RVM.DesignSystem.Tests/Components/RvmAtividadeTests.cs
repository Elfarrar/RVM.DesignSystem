using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RVM.DesignSystem.Components.Activity;
using RVM.DesignSystem.Components.MarkerButton;
using RVM.DesignSystem.Components.Menu;
using RVM.DesignSystem.Components.Widget;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>
/// RvmActivity, RvmHistory, RvmComment, RvmReview, RvmNotificationItem, RvmMarkerButton e RvmWidget, do contrato
/// com o RVM.UI (DSGN-017).
/// </summary>
public class RvmAtividadeTests : BunitContext
{
    public RvmAtividadeTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    // --- RvmActivity ---

    [Fact]
    public void Atividade_avulsa_e_div_com_ponto_e_carimbo_embaixo()
    {
        var cortado = Render<RvmActivity>(p => p
            .Add(x => x.Title, "Pulverizacao no talhao 12")
            .Add(x => x.Description, "Fungicida aplicado")
            .Add(x => x.Timestamp, "20 dez 2025, 08:00")
            .Add(x => x.Class, "extra")
            .AddUnmatched("data-testid", "atv"));

        var raiz = cortado.Find("[data-testid=atv]");
        Assert.Equal("DIV", raiz.TagName);
        Assert.Contains("rvm-atividade", raiz.ClassList);
        Assert.Contains("extra", raiz.ClassList);
        Assert.Single(cortado.FindAll(".rvm-atividade-ponto"));
        Assert.Equal("true", cortado.Find(".rvm-atividade-trilha").GetAttribute("aria-hidden"));
        Assert.Equal("Fungicida aplicado", cortado.Find(".rvm-atividade-descricao").TextContent);
        // Bottom e o padrao do RVM.UI: o carimbo e um paragrafo depois do conteudo, fora do cabecalho.
        Assert.Empty(cortado.FindAll(".rvm-atividade-cabecalho .rvm-atividade-carimbo"));
        Assert.Equal("P", cortado.Find(".rvm-atividade-carimbo").TagName);
    }

    [Fact]
    public void Carimbo_TopRight_vai_para_o_cabecalho_e_corpo_livre_aparece()
    {
        var cortado = Render<RvmActivity>(p => p
            .Add(x => x.Title, "Nota fiscal emitida")
            .Add(x => x.Timestamp, "Quarta")
            .Add(x => x.TimestampPosition, RvmActivityTimestampPosition.TopRight)
            .AddChildContent("<span class=\"nf\">nf-1234.pdf</span>"));

        Assert.Equal("Quarta", cortado.Find(".rvm-atividade-cabecalho .rvm-atividade-carimbo").TextContent);
        Assert.Equal("nf-1234.pdf", cortado.Find(".rvm-atividade-corpo .nf").TextContent);
    }

    [Fact]
    public void Marcador_de_icone_e_de_avatar()
    {
        var icone = Render<RvmActivity>(p => p
            .Add(x => x.Title, "Pagamento")
            .Add(x => x.Tracker, RvmActivityTracker.Icon)
            .Add(x => x.Icon, RvmIconName.Calendar));
        Assert.Single(icone.FindAll(".rvm-atividade-trilha .rvm-icon-badge"));
        Assert.Empty(icone.FindAll(".rvm-atividade-ponto"));

        var avatar = Render<RvmActivity>(p => p
            .Add(x => x.Title, "Comentou")
            .Add(x => x.Tracker, RvmActivityTracker.Avatar)
            .Add(x => x.AvatarName, "Ana Souza"));
        Assert.Contains("AS", avatar.Find(".rvm-atividade-trilha").TextContent);
    }

    [Fact]
    public void Marcador_de_icone_sem_Icon_e_erro_de_programacao()
    {
        Assert.Throws<ArgumentException>(() => Render<RvmActivity>(p => p
            .Add(x => x.Title, "x")
            .Add(x => x.Tracker, RvmActivityTracker.Icon)));
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("  ", "")]
    [InlineData("Ana", "A")]
    [InlineData("ana maria souza", "AS")]
    [InlineData("🌱", "🌱")] // par substituto: cortar no primeiro char deixava meio emoji
    [InlineData("🌱 Agro", "🌱")] // o RvmAvatar corta as iniciais em 2 chars: o emoji inteiro fica, a letra sai
    public void Iniciais_do_nome_no_avatar(string? nome, string esperado)
    {
        var cortado = Render<RvmActivity>(p => p
            .Add(x => x.Title, "x")
            .Add(x => x.Tracker, RvmActivityTracker.Avatar)
            .Add(x => x.AvatarName, nome));
        Assert.Equal(esperado, cortado.Find(".rvm-atividade-trilha").TextContent.Trim());
    }

    // --- RvmHistory ---

    private static RenderFragment DuasAtividades => b =>
    {
        b.OpenComponent<RvmActivity>(0);
        b.AddComponentParameter(1, nameof(RvmActivity.Title), "Plantio");
        b.CloseComponent();
        b.OpenComponent<RvmActivity>(2);
        b.AddComponentParameter(3, nameof(RvmActivity.Title), "Colheita");
        b.CloseComponent();
    };

    [Fact]
    public void Historico_e_cartao_com_titulo_etiqueta_e_lista_ordenada_de_li()
    {
        var cortado = Render<RvmHistory>(p => p
            .Add(x => x.Title, "Historico do talhao")
            .Add(x => x.Text, "Ultimos 30 dias")
            .Add(x => x.Label, "Safra 2026")
            .Add(x => x.Class, "extra")
            .AddUnmatched("data-testid", "hist")
            .Add(x => x.ChildContent, DuasAtividades));

        var raiz = cortado.Find("[data-testid=hist]");
        Assert.Contains("rvm-historico", raiz.ClassList);
        Assert.Contains("extra", raiz.ClassList);
        var titulo = cortado.Find("h3.rvm-historico-titulo");
        Assert.Equal("Historico do talhao", titulo.TextContent);
        Assert.Equal("Ultimos 30 dias", cortado.Find(".rvm-historico-texto").TextContent);
        Assert.Contains("Safra 2026", cortado.Find(".rvm-historico-linha-titulo").TextContent);

        var corpo = cortado.Find(".rvm-historico-corpo");
        Assert.Equal("0", corpo.GetAttribute("tabindex"));
        Assert.Equal("group", corpo.GetAttribute("role"));
        Assert.Equal(titulo.Id, corpo.GetAttribute("aria-labelledby"));
        var itens = cortado.FindAll("ol > li.rvm-atividade");
        Assert.Equal(2, itens.Count);
        Assert.Empty(cortado.FindAll("[role=menu], [aria-haspopup=menu]"));
    }

    [Fact]
    public void Historico_com_MenuItems_ganha_o_menu_de_tres_pontos()
    {
        var padrao = Render<RvmHistory>(p => p
            .Add(x => x.Title, "Historico")
            .Add(x => x.MenuItems, (RenderFragment)(b => b.AddContent(0, "item"))));
        Assert.Equal("Mais acoes", padrao.Find("[aria-haspopup=menu]").GetAttribute("aria-label"));

        var proprio = Render<RvmHistory>(p => p
            .Add(x => x.Title, "Historico")
            .Add(x => x.MenuLabel, "Opcoes do historico")
            .Add(x => x.MenuItems, (RenderFragment)(b => b.AddContent(0, "item"))));
        Assert.Equal("Opcoes do historico", proprio.Find("[aria-haspopup=menu]").GetAttribute("aria-label"));
    }

    [Fact]
    public void Historico_erro_vence_carregando_que_vence_vazio()
    {
        var cortado = Render<RvmHistory>(p => p
            .Add(x => x.Title, "Historico")
            .Add(x => x.Error, true)
            .Add(x => x.Loading, true)
            .Add(x => x.IsEmpty, true)
            .Add(x => x.ChildContent, DuasAtividades));
        Assert.Contains("Nao deu para carregar", cortado.Markup);
        Assert.Contains("Tivemos um problema tecnico", cortado.Markup);
        Assert.Empty(cortado.FindAll("ol"));
        Assert.Null(cortado.Find(".rvm-historico-corpo").GetAttribute("tabindex"));

        cortado.Render(p => p.Add(x => x.Error, false));
        Assert.Contains("Carregando...", cortado.Find("[role=status]").TextContent);

        cortado.Render(p => p.Add(x => x.Loading, false));
        Assert.Contains("Nenhuma atividade por aqui ainda.", cortado.Markup);

        cortado.Render(p => p.Add(x => x.Empty, (RenderFragment)(b => b.AddMarkupContent(0, "<p class=\"vazio\">Registre o primeiro manejo</p>"))));
        Assert.Equal("Registre o primeiro manejo", cortado.Find(".vazio").TextContent);

        cortado.Render(p => p.Add(x => x.IsEmpty, false));
        Assert.Equal(2, cortado.FindAll("li.rvm-atividade").Count);
    }

    [Fact]
    public void Historico_usa_os_textos_de_estado_informados()
    {
        var cortado = Render<RvmHistory>(p => p
            .Add(x => x.Title, "Historico")
            .Add(x => x.Error, true)
            .Add(x => x.ErrorTitle, "Falhou")
            .Add(x => x.ErrorText, "Tente de novo"));
        Assert.Contains("Falhou", cortado.Markup);
        Assert.Contains("Tente de novo", cortado.Markup);

        cortado.Render(p => p.Add(x => x.Error, false).Add(x => x.Loading, true).Add(x => x.LoadingText, "Buscando..."));
        Assert.Contains("Buscando...", cortado.Markup);

        cortado.Render(p => p.Add(x => x.Loading, false).Add(x => x.IsEmpty, true).Add(x => x.EmptyText, "Nada ainda"));
        Assert.Contains("Nada ainda", cortado.Markup);
    }

    // --- RvmComment ---

    [Fact]
    public void Comentario_mostra_autor_quando_texto_e_responde()
    {
        var respondeu = 0;
        var cortado = Render<RvmComment>(p => p
            .Add(x => x.AuthorName, "Joao Lima")
            .Add(x => x.Timestamp, "ha 2 horas")
            .Add(x => x.Text, "Chuva atrasou a colheita.")
            .Add(x => x.Class, "extra")
            .AddUnmatched("data-testid", "com")
            .Add(x => x.OnReply, () => respondeu++));

        var raiz = cortado.Find("[data-testid=com]");
        Assert.Equal("ARTICLE", raiz.TagName);
        Assert.Contains("extra", raiz.ClassList);
        Assert.Equal("Joao Lima", cortado.Find(".rvm-comentario-autor").TextContent);
        Assert.Equal("ha 2 horas", cortado.Find(".rvm-comentario-carimbo").TextContent);
        Assert.Equal("Chuva atrasou a colheita.", cortado.Find(".rvm-comentario-texto").TextContent);
        Assert.Equal("true", cortado.Find(".rvm-comentario-avatar").GetAttribute("aria-hidden"));

        var botao = cortado.Find("button");
        Assert.Equal("Responder a Joao Lima", botao.GetAttribute("aria-label"));
        Assert.Contains("Responder", botao.TextContent);
        botao.Click();
        Assert.Equal(1, respondeu);
    }

    [Fact]
    public void Comentario_sem_resposta_e_com_respostas_aninhadas()
    {
        var cortado = Render<RvmComment>(p => p
            .Add(x => x.AuthorName, "Ana")
            .Add(x => x.Timestamp, "ontem")
            .Add(x => x.Text, "Pergunta")
            .Add(x => x.ShowReply, false)
            .Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<RvmComment>(0);
                b.AddComponentParameter(1, nameof(RvmComment.AuthorName), "Bia");
                b.AddComponentParameter(2, nameof(RvmComment.Timestamp), "hoje");
                b.AddComponentParameter(3, nameof(RvmComment.Text), "Resposta");
                b.AddComponentParameter(4, nameof(RvmComment.ReplyLabel), "Comentar");
                b.CloseComponent();
            })));

        Assert.Single(cortado.FindAll(".rvm-comentario-respostas > article.rvm-comentario"));
        // So a resposta tem botao, com o rotulo proprio.
        var botao = Assert.Single(cortado.FindAll("button"));
        Assert.Equal("Comentar a Bia", botao.GetAttribute("aria-label"));
    }

    // --- RvmReview ---

    [Fact]
    public void Avaliacao_le_a_nota_por_extenso_e_lista_as_fotos()
    {
        var cortado = Render<RvmReview>(p => p
            .Add(x => x.AuthorName, "Carlos")
            .Add(x => x.AuthorDetail, "Fazenda Boa Vista")
            .Add(x => x.Timestamp, "12 mar 2026")
            .Add(x => x.Rating, 4)
            .Add(x => x.Text, "Semente com otima germinacao.")
            .Add(x => x.Images, new[] { "a.jpg", "b.jpg" })
            .Add(x => x.Class, "extra")
            .AddUnmatched("data-testid", "av"));

        Assert.Contains("extra", cortado.Find("[data-testid=av]").ClassList);
        Assert.Equal("Nota: 4 de 5", cortado.Find(".rvm-avaliacao-cabecalho [role=img]").GetAttribute("aria-label"));
        Assert.Equal("Carlos", cortado.Find(".rvm-avaliacao-nome").TextContent);
        Assert.Equal("- Fazenda Boa Vista", cortado.Find(".rvm-avaliacao-detalhe").TextContent);
        Assert.Equal("12 mar 2026", cortado.Find(".rvm-avaliacao-carimbo").TextContent);
        Assert.Equal("Fotos de Carlos", cortado.Find(".rvm-avaliacao-fotos").GetAttribute("aria-label"));
        var fotos = cortado.FindAll(".rvm-avaliacao-foto");
        Assert.Equal(["a.jpg", "b.jpg"], fotos.Select(f => f.GetAttribute("src")));
        Assert.Equal("Foto 2 de 2", fotos[1].GetAttribute("alt"));
    }

    [Fact]
    public void Avaliacao_sem_detalhe_nem_fotos()
    {
        var cortado = Render<RvmReview>(p => p
            .Add(x => x.AuthorName, "Carlos")
            .Add(x => x.Timestamp, "hoje")
            .Add(x => x.Text, "Bom"));

        Assert.Empty(cortado.FindAll(".rvm-avaliacao-detalhe"));
        Assert.Empty(cortado.FindAll(".rvm-avaliacao-fotos"));
    }

    // --- RvmNotificationItem ---

    [Fact]
    public void Aviso_nao_lido_tem_ponto_texto_para_leitor_e_acao()
    {
        var lidas = 0;
        var cortado = Render<RvmNotificationItem>(p => p
            .Add(x => x.Title, "Boleto vence amanha")
            .Add(x => x.Text, "Fornecedor de adubo")
            .Add(x => x.Tag, (RenderFragment)(b => b.AddContent(0, "Financeiro")))
            .Add(x => x.OnRead, () => lidas++)
            .Add(x => x.Class, "extra")
            .AddUnmatched("data-testid", "av"));

        var raiz = cortado.Find("[data-testid=av]");
        Assert.Equal("LI", raiz.TagName);
        Assert.Contains("rvm-aviso-novo", raiz.ClassList);
        Assert.Contains("extra", raiz.ClassList);
        Assert.Single(cortado.FindAll(".rvm-aviso-ponto"));
        Assert.Contains("Nao lida:", cortado.Find(".rvm-aviso-titulo").TextContent);
        Assert.Equal("Financeiro", cortado.Find(".rvm-aviso-etiqueta").TextContent);
        Assert.Equal("Fornecedor de adubo", cortado.Find(".rvm-aviso-texto").TextContent);

        var botao = cortado.Find("button");
        Assert.Contains("Marcar como lida", botao.TextContent);
        botao.Click();
        Assert.Equal(1, lidas);
        // O botao some quando o pai marca como lido: o foco vai para o proprio aviso.
        cortado.Render(p => p.Add(x => x.Read, true));
        Assert.Contains(JSInterop.Invocations, i => i.Identifier.Contains("focus", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Aviso_lido_ou_sem_OnRead_nao_tem_acao()
    {
        var lido = Render<RvmNotificationItem>(p => p
            .Add(x => x.Title, "Lido")
            .Add(x => x.Read, true)
            .Add(x => x.OnRead, () => { }));
        Assert.Contains("rvm-aviso-lido", lido.Find("li").ClassList);
        Assert.Empty(lido.FindAll("button"));
        Assert.Empty(lido.FindAll(".rvm-aviso-ponto"));
        Assert.DoesNotContain("Nao lida", lido.Markup);

        var semAcao = Render<RvmNotificationItem>(p => p.Add(x => x.Title, "Novo").Add(x => x.ReadText, "Ok"));
        Assert.Empty(semAcao.FindAll("button"));
        Assert.Empty(semAcao.FindAll(".rvm-aviso-texto"));
    }

    // --- RvmMarkerButton ---

    [Fact]
    public void Marcador_alterna_aria_pressed_rotulo_e_avisa()
    {
        var avisos = new List<bool>();
        var cortado = Render<RvmMarkerButton>(p => p
            .Add(x => x.MarkedChanged, (bool v) => avisos.Add(v))
            .Add(x => x.Class, "extra")
            .AddUnmatched("data-testid", "mk"));

        var botao = cortado.Find("button");
        Assert.Equal("button", botao.GetAttribute("type"));
        Assert.Equal("false", botao.GetAttribute("aria-pressed"));
        Assert.Equal("Marcar como favorito", botao.GetAttribute("aria-label"));
        Assert.Contains("rvm-marcador-pilula", botao.ClassList);
        Assert.Contains("extra", botao.ClassList);

        botao.Click();
        botao = cortado.Find("button");
        Assert.Equal("true", botao.GetAttribute("aria-pressed"));
        Assert.Equal("Tirar dos favoritos", botao.GetAttribute("title"));
        Assert.Contains("rvm-marcador-marcado", botao.ClassList);

        botao.Click();
        Assert.Equal([true, false], avisos);
    }

    [Fact]
    public void Marcador_de_pagina_rotulos_proprios_forma_e_desabilitado()
    {
        var cortado = Render<RvmMarkerButton>(p => p
            .Add(x => x.Type, RvmMarkerType.Bookmark)
            .Add(x => x.Shape, RvmFieldShape.Rounded));
        Assert.Equal("Salvar nos marcadores", cortado.Find("button").GetAttribute("aria-label"));
        Assert.DoesNotContain("rvm-marcador-pilula", cortado.Find("button").ClassList);

        cortado.Render(p => p.Add(x => x.Marked, true));
        Assert.Equal("Tirar dos marcadores", cortado.Find("button").GetAttribute("aria-label"));

        cortado.Render(p => p.Add(x => x.MarkedLabel, "Salvo").Add(x => x.Label, "Salvar").Add(x => x.Disabled, true));
        Assert.Equal("Salvo", cortado.Find("button").GetAttribute("aria-label"));
        Assert.True(cortado.Find("button").HasAttribute("disabled"));

        cortado.Render(p => p.Add(x => x.Marked, false));
        Assert.Equal("Salvar", cortado.Find("button").GetAttribute("aria-label"));
    }

    // --- RvmWidget ---

    [Fact]
    public void Widget_fechado_nao_tem_painel_e_abre_no_clique()
    {
        var avisos = new List<bool>();
        var cortado = Render<RvmWidget>(p => p
            .Add(x => x.Label, "Notificacoes")
            .Add(x => x.Icon, RvmIconName.Bell)
            .Add(x => x.Badge, 3)
            .Add(x => x.Id, "sino")
            .Add(x => x.Class, "extra")
            .AddUnmatched("data-testid", "wd")
            .Add(x => x.OpenChanged, (bool v) => avisos.Add(v))
            .AddChildContent("<p class=\"conteudo-teste\">oi</p>"));

        Assert.Contains("extra", cortado.Find("[data-testid=wd]").ClassList);
        var gatilho = cortado.Find("#sino");
        Assert.Equal("dialog", gatilho.GetAttribute("aria-haspopup"));
        Assert.Equal("false", gatilho.GetAttribute("aria-expanded"));
        Assert.Null(gatilho.GetAttribute("aria-controls"));
        Assert.Equal("Notificacoes, 3 novos", gatilho.GetAttribute("aria-label"));
        Assert.Equal("Notificacoes", gatilho.GetAttribute("title"));
        Assert.Empty(cortado.FindAll("[role=dialog]"));

        gatilho.Click();
        gatilho = cortado.Find("#sino");
        Assert.Equal("true", gatilho.GetAttribute("aria-expanded"));
        Assert.Equal("sino-painel", gatilho.GetAttribute("aria-controls"));
        var painel = cortado.Find("[role=dialog]");
        Assert.Equal("sino-painel", painel.Id);
        Assert.Equal("sino", painel.GetAttribute("aria-labelledby"));
        Assert.Contains("rvm-widget-lista", painel.ClassList);
        Assert.Equal("oi", cortado.Find(".conteudo-teste").TextContent);

        // Clicar de novo no gatilho fecha.
        gatilho.Click();
        Assert.Empty(cortado.FindAll("[role=dialog]"));
        Assert.Equal([true, false], avisos);
    }

    [Fact]
    public void Widget_Esc_fecha_e_clique_fora_fecha()
    {
        var cortado = Render<RvmWidget>(p => p.Add(x => x.Label, "Idioma").Add(x => x.Icon, RvmIconName.Bell));

        cortado.Find("button").Click();
        cortado.Find(".rvm-widget").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        Assert.Single(cortado.FindAll("[role=dialog]"));
        cortado.Find(".rvm-widget").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        Assert.Empty(cortado.FindAll("[role=dialog]"));

        cortado.Find("button").Click();
        cortado.Find(".rvm-widget-fundo").Click();
        Assert.Empty(cortado.FindAll("[role=dialog]"));
        Assert.Contains(JSInterop.Invocations, i => i.Identifier.Contains("focus", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Widget_so_segue_o_Open_quando_ele_muda()
    {
        var cortado = Render<RvmWidget>(p => p.Add(x => x.Label, "Avisos").Add(x => x.Open, true));
        Assert.Single(cortado.FindAll("[role=dialog]"));

        // A pessoa fecha; o pai re-renderiza com o MESMO Open=true (sem @bind): continua fechado.
        cortado.Find(".rvm-widget").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        cortado.Render(p => p.Add(x => x.Open, true));
        Assert.Empty(cortado.FindAll("[role=dialog]"));

        // Quando o pai muda o valor, o widget segue.
        cortado.Render(p => p.Add(x => x.Open, false));
        cortado.Render(p => p.Add(x => x.Open, true));
        Assert.Single(cortado.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Widget_folha_com_titulo_fechar_rodape_e_posicao()
    {
        var cortado = Render<RvmWidget>(p => p
            .Add(x => x.Label, "Notificacoes")
            .Add(x => x.Icon, RvmIconName.Bell)
            .Add(x => x.Size, RvmWidgetPanelSize.Sheet)
            .Add(x => x.Placement, RvmMenuPlacement.TopStart)
            .Add(x => x.PanelTitle, "Avisos")
            .Add(x => x.Footer, (RenderFragment)(b => b.AddContent(0, "Ver todos")))
            .Add(x => x.Open, true));

        var painel = cortado.Find("[role=dialog]");
        Assert.Contains("rvm-widget-folha", painel.ClassList);
        Assert.Contains("rvm-widget-acima", painel.ClassList);
        Assert.Contains("rvm-widget-inicio", painel.ClassList);
        var titulo = cortado.Find("h2.rvm-widget-titulo-do-painel");
        Assert.Equal(titulo.Id, painel.GetAttribute("aria-labelledby"));
        Assert.Equal("Ver todos", cortado.Find(".rvm-widget-rodape").TextContent);

        cortado.Find("[aria-label='Fechar Avisos']").Click();
        Assert.Empty(cortado.FindAll("[role=dialog]"));
    }

    [Theory]
    [InlineData(RvmMenuPlacement.BottomEnd, "")]
    [InlineData(RvmMenuPlacement.BottomStart, "rvm-widget-inicio")]
    [InlineData(RvmMenuPlacement.BottomCenter, "rvm-widget-centro")]
    [InlineData(RvmMenuPlacement.TopEnd, "rvm-widget-acima")]
    public void Widget_posicoes_do_painel(RvmMenuPlacement posicao, string classe)
    {
        var cortado = Render<RvmWidget>(p => p.Add(x => x.Label, "x").Add(x => x.Placement, posicao).Add(x => x.Open, true));
        var painel = cortado.Find("[role=dialog]");
        if (classe.Length == 0)
        {
            Assert.DoesNotContain(painel.ClassList, c => c is "rvm-widget-inicio" or "rvm-widget-centro" or "rvm-widget-acima");
        }
        else
        {
            Assert.Contains(classe, painel.ClassList);
        }
    }

    [Fact]
    public void Widget_cartao_tem_titulo_subtitulo_imagem_e_nome_que_comeca_pelo_titulo()
    {
        var cortado = Render<RvmWidget>(p => p
            .Add(x => x.Label, "Perfil")
            .Add(x => x.Variant, RvmWidgetVariant.Card)
            .Add(x => x.Title, "Ana Souza")
            .Add(x => x.Subtitle, "Agronoma")
            .Add(x => x.ImageUrl, "ana.jpg"));

        var gatilho = cortado.Find("button");
        Assert.Contains("rvm-widget-cartao", gatilho.ClassList);
        Assert.Equal("Ana Souza, Perfil", gatilho.GetAttribute("aria-label"));
        Assert.Null(gatilho.GetAttribute("title"));
        Assert.Equal("Agronoma", cortado.Find(".rvm-widget-subtitulo").TextContent);
        Assert.Equal("", cortado.Find("img").GetAttribute("alt"));
        Assert.Empty(cortado.FindAll(".rvm-widget-contador"));
    }

    [Theory]
    [InlineData(1, "Avisos, 1 novo")]
    [InlineData(2, "Avisos, 2 novos")]
    public void Widget_contador_no_singular_e_no_plural(int badge, string nome)
    {
        var cortado = Render<RvmWidget>(p => p.Add(x => x.Label, "Avisos").Add(x => x.Badge, badge));
        Assert.Equal(nome, cortado.Find("button").GetAttribute("aria-label"));
    }

    [Fact]
    public void Widget_aberto_segura_o_Esc_e_fechado_deixa_subir()
    {
        var teclasNoPai = 0;
        var cortado = Render(b =>
        {
            b.OpenElement(0, "div");
            b.AddAttribute(1, "class", "pai");
            b.AddAttribute(2, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, () => teclasNoPai++));
            b.OpenComponent<RvmWidget>(3);
            b.AddComponentParameter(4, nameof(RvmWidget.Label), "Avisos");
            b.CloseComponent();
            b.CloseElement();
        });

        // Fechado: o Esc no gatilho sobe (fecha o dialogo em volta, por exemplo).
        cortado.Find(".rvm-widget-gatilho").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        Assert.Equal(1, teclasNoPai);

        // Aberto: o Esc fecha so o painel.
        cortado.Find(".rvm-widget-gatilho").Click();
        cortado.Find("[role=dialog]").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        Assert.Empty(cortado.FindAll("[role=dialog]"));
        Assert.Equal(1, teclasNoPai);
    }

    [Fact]
    public void Widget_e_aviso_sobrevivem_ao_circuito_caido_ao_mover_o_foco()
    {
        JSInterop.Mode = JSRuntimeMode.Strict;
        JSInterop.SetupVoid(_ => true).SetException(new JSDisconnectedException("circuito caiu"));

        var widget = Render<RvmWidget>(p => p.Add(x => x.Label, "Avisos"));
        widget.Find("button").Click();
        Assert.Single(widget.FindAll("[role=dialog]"));
        widget.Find(".rvm-widget").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        Assert.Empty(widget.FindAll("[role=dialog]"));

        var aviso = Render<RvmNotificationItem>(p => p.Add(x => x.Title, "Boleto").Add(x => x.OnRead, () => { }));
        aviso.Find("button").Click();
        Assert.Contains("rvm-aviso", aviso.Find("li").ClassList);
    }

    [Fact]
    public void Widget_desabilitado_e_id_gerado()
    {
        var cortado = Render<RvmWidget>(p => p.Add(x => x.Label, "x").Add(x => x.Disabled, true).Add(x => x.Badge, 0));
        var gatilho = cortado.Find("button");
        Assert.True(gatilho.HasAttribute("disabled"));
        Assert.StartsWith("rvm-widget-", gatilho.Id);
        Assert.Equal("x", gatilho.GetAttribute("aria-label"));
    }
}
