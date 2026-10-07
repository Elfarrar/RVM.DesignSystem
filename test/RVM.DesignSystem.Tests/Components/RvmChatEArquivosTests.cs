using Bunit;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Chat;
using RVM.DesignSystem.Components.FileCard;
using RVM.DesignSystem.Components.FileIcon;
using RVM.DesignSystem.Components.Menu;
using RVM.DesignSystem.Components.Upload;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>Chat (RvmChat e as pecas), RvmFileCard e RvmFileTypeCard (contrato com o RVM.UI, DSGN-017).</summary>
public class RvmChatEArquivosTests : BunitContext
{
    public RvmChatEArquivosTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    // --- RvmChat ---

    [Fact]
    public void Chat_nomeia_a_secao_e_tem_o_log_das_mensagens()
    {
        var cortado = Render<RvmChat>(p => p
            .Add(x => x.Title, "Ana Souza")
            .Add(x => x.Subtitle, "online")
            .Add(x => x.Class, "extra")
            .Add(x => x.Actions, (RenderFragment)(b => b.AddMarkupContent(0, "<button>Ligar</button>")))
            .Add(x => x.Composer, (RenderFragment)(b => b.AddMarkupContent(0, "<input aria-label=\"Mensagem\" />")))
            .AddChildContent("<p>oi</p>"));

        var secao = cortado.Find("section");
        Assert.Equal("Conversa com Ana Souza", secao.GetAttribute("aria-label"));
        Assert.Contains("rvm-chat", secao.ClassList);
        Assert.Contains("extra", secao.ClassList);
        Assert.Contains("Ana Souza", cortado.Find(".rvm-titulo").TextContent);
        Assert.Equal("online", cortado.Find(".rvm-subtitulo").TextContent);

        var log = cortado.Find("[role=log]");
        Assert.Equal("Mensagens", log.GetAttribute("aria-label"));
        Assert.Equal("polite", log.GetAttribute("aria-live"));
        Assert.Equal("0", log.GetAttribute("tabindex"));
        Assert.Equal("oi", log.QuerySelector("p")!.TextContent);
        Assert.NotNull(cortado.Find("footer input"));
        Assert.NotNull(cortado.Find(".rvm-acoes button"));
    }

    [Fact]
    public void Chat_com_Label_e_sem_compositor()
    {
        var cortado = Render<RvmChat>(p => p.Add(x => x.Title, "Equipe").Add(x => x.Label, "Conversa da equipe da safra"));

        Assert.Equal("Conversa da equipe da safra", cortado.Find("section").GetAttribute("aria-label"));
        Assert.Empty(cortado.FindAll("footer"));
        Assert.Empty(cortado.FindAll(".rvm-subtitulo"));
        Assert.Empty(cortado.FindAll(".rvm-acoes"));
    }

    // --- RvmChatContact ---

    [Fact]
    public void Contato_e_botao_com_aria_current_online_e_nao_lidas_em_texto()
    {
        var cliques = 0;
        var cortado = Render<RvmChatContact>(p => p
            .Add(x => x.Name, "Bruno Lima")
            .Add(x => x.Preview, "Mandei o laudo do talhao 3")
            .Add(x => x.TimeLabel, "14:32")
            .Add(x => x.Online, true)
            .Add(x => x.Selected, true)
            .Add(x => x.Unread, 3)
            .Add(x => x.OnClick, () => cliques++));

        var botao = cortado.Find("button.rvm-chat-contato");
        Assert.Equal("button", botao.GetAttribute("type"));
        Assert.Equal("true", botao.GetAttribute("aria-current"));
        Assert.Contains("rvm-selecionado", botao.ClassList);
        Assert.Contains("(online)", botao.TextContent);
        Assert.Contains("3 mensagens nao lidas", botao.TextContent);
        Assert.Equal("true", cortado.Find(".rvm-retrato").GetAttribute("aria-hidden"));
        // Com nao lidas, o selo toma o lugar do horario.
        Assert.Empty(cortado.FindAll(".rvm-horario"));

        botao.Click();
        Assert.Equal(1, cliques);
    }

    [Fact]
    public void Contato_sem_nao_lidas_mostra_o_horario_e_nao_marca_current()
    {
        var cortado = Render<RvmChatContact>(p => p.Add(x => x.Name, "Carla").Add(x => x.TimeLabel, "10 abr").Add(x => x.Unread, 0));

        Assert.Null(cortado.Find("button").GetAttribute("aria-current"));
        Assert.Equal("10 abr", cortado.Find(".rvm-horario").TextContent);
        Assert.Empty(cortado.FindAll(".rvm-online"));
        Assert.DoesNotContain("(online)", cortado.Markup);
        Assert.DoesNotContain("nao lida", cortado.Markup);
    }

    [Fact]
    public void Contato_com_uma_nao_lida_fala_no_singular()
    {
        var cortado = Render<RvmChatContact>(p => p.Add(x => x.Name, "Carla").Add(x => x.Unread, 1));

        Assert.Contains("1 mensagem nao lida", cortado.Markup);
    }

    // --- RvmChatGroup e baloes ---

    [Fact]
    public void Grupo_tem_avatar_horario_e_lado()
    {
        var cortado = Render<RvmChatGroup>(p => p
            .Add(x => x.Author, "Ana Souza")
            .Add(x => x.TimeLabel, "13:15")
            .Add(x => x.Side, RvmChatSide.Right)
            .AddChildContent<RvmChatMessage>(m => m.Add(x => x.Side, RvmChatSide.Right).AddChildContent("Pode mandar")));

        var raiz = cortado.Find(".rvm-chat-grupo");
        Assert.Contains("rvm-direita", raiz.ClassList);
        Assert.Equal("13:15", cortado.Find(".rvm-horario").TextContent);
        Assert.Equal("Ana Souza", cortado.Find(".rvm-avatar").GetAttribute("aria-label"));
        Assert.Equal("AS", cortado.Find(".rvm-iniciais").TextContent);
        Assert.Equal("Pode mandar", cortado.Find(".rvm-balao").TextContent);
    }

    [Fact]
    public void Grupo_padrao_e_a_esquerda_sem_horario()
    {
        var cortado = Render<RvmChatGroup>(p => p.Add(x => x.Author, "Bruno"));

        Assert.Contains("rvm-esquerda", cortado.Find(".rvm-chat-grupo").ClassList);
        Assert.Empty(cortado.FindAll(".rvm-horario"));
    }

    [Fact]
    public void Mensagem_pinta_pelo_lado_e_repassa_atributos()
    {
        var esquerda = Render<RvmChatMessage>(p => p.AddChildContent("Oi").AddUnmatched("data-id", "m1"));
        var direita = Render<RvmChatMessage>(p => p.Add(x => x.Side, RvmChatSide.Right).Add(x => x.Class, "x").AddChildContent("Oi"));

        Assert.Contains("rvm-esquerda", esquerda.Find("div").ClassList);
        Assert.Equal("m1", esquerda.Find("div").GetAttribute("data-id"));
        Assert.Contains("rvm-direita", direita.Find("div").ClassList);
        Assert.Contains("x", direita.Find("div").ClassList);
    }

    [Fact]
    public void Arquivo_no_chat_mostra_icone_decorativo_nome_e_tamanho()
    {
        var cortado = Render<RvmChatFile>(p => p
            .Add(x => x.Name, "laudo-solo.pdf")
            .Add(x => x.Size, "300 KB")
            .Add(x => x.Type, RvmFileIconName.Pdf)
            .Add(x => x.Side, RvmChatSide.Right));

        Assert.Contains("rvm-direita", cortado.Find(".rvm-balao").ClassList);
        Assert.Equal("true", cortado.Find("svg").GetAttribute("aria-hidden"));
        Assert.Contains("PDF", cortado.Find("svg").TextContent);
        Assert.Equal("laudo-solo.pdf", cortado.Find(".rvm-nome").TextContent);
        Assert.Equal("300 KB", cortado.Find(".rvm-tamanho").TextContent);
    }

    [Fact]
    public void Arquivo_no_chat_sem_tamanho()
    {
        var cortado = Render<RvmChatFile>(p => p.Add(x => x.Name, "a.txt"));

        Assert.Empty(cortado.FindAll(".rvm-tamanho"));
        Assert.Contains("rvm-esquerda", cortado.Find(".rvm-balao").ClassList);
    }

    [Fact]
    public void Audio_tem_botao_nomeado_que_avisa_e_barra_acessivel()
    {
        var alternou = 0;
        var cortado = Render<RvmChatVoice>(p => p
            .Add(x => x.Duration, "0:42")
            .Add(x => x.Progress, 0.25)
            .Add(x => x.OnToggle, () => alternou++));

        var botao = cortado.Find("button");
        Assert.Equal("Ouvir o audio", botao.GetAttribute("aria-label"));
        botao.Click();
        Assert.Equal(1, alternou);

        var barra = cortado.Find("[role=progressbar]");
        Assert.Equal("25", barra.GetAttribute("aria-valuenow"));
        Assert.Equal("Quanto do audio de 0:42 ja foi ouvido", barra.GetAttribute("aria-label"));
        Assert.Equal("0:42", cortado.Find(".rvm-duracao").TextContent);
    }

    [Fact]
    public void Audio_tocando_mostra_pausa_e_limita_o_progresso()
    {
        var cortado = Render<RvmChatVoice>(p => p
            .Add(x => x.Playing, true)
            .Add(x => x.Progress, 3)
            .Add(x => x.PauseLabel, "Pausar recado")
            .Add(x => x.Side, RvmChatSide.Right));

        Assert.Equal("Pausar recado", cortado.Find("button").GetAttribute("aria-label"));
        Assert.Equal("100", cortado.Find("[role=progressbar]").GetAttribute("aria-valuenow"));
        Assert.Equal("Quanto do audio ja foi ouvido", cortado.Find("[role=progressbar]").GetAttribute("aria-label"));
        Assert.Empty(cortado.FindAll(".rvm-duracao"));
        Assert.Contains("rvm-direita", cortado.Find(".rvm-balao").ClassList);
    }

    [Fact]
    public void Audio_com_progresso_invalido_vira_zero()
    {
        var cortado = Render<RvmChatVoice>(p => p.Add(x => x.Progress, double.NaN));

        Assert.Equal("0", cortado.Find("[role=progressbar]").GetAttribute("aria-valuenow"));
    }

    // --- RvmFileCard ---

    [Fact]
    public void Cartao_de_arquivo_horizontal_com_selecao_e_menu()
    {
        bool? marcado = null;
        var cortado = Render<RvmFileCard>(p => p
            .Add(x => x.Name, "colheita-2026.xlsx")
            .Add(x => x.Meta, "1,2 MB")
            .Add(x => x.Selectable, true)
            .Add(x => x.SelectedChanged, (bool v) => marcado = v)
            .Add(x => x.MenuItems, (RenderFragment)(b =>
            {
                b.OpenComponent<RvmMenuItem>(0);
                b.AddAttribute(1, nameof(RvmMenuItem.Text), "Baixar");
                b.CloseComponent();
            })));

        var raiz = cortado.Find(".rvm-file-card");
        Assert.Contains("rvm-horizontal", raiz.ClassList);
        Assert.DoesNotContain("rvm-selecionado", raiz.ClassList);
        Assert.Contains("XLS", cortado.Find("svg.rvm-icone-arquivo").TextContent);
        Assert.Equal("colheita-2026.xlsx", cortado.Find(".rvm-nome").TextContent);
        Assert.Equal("1,2 MB", cortado.Find(".rvm-meta").TextContent);

        var caixa = cortado.Find("input[type=checkbox]");
        Assert.Equal("Selecionar colheita-2026.xlsx", caixa.GetAttribute("aria-label"));
        caixa.Change(true);
        Assert.True(marcado);

        Assert.NotNull(cortado.Find("[aria-label='Mais acoes de colheita-2026.xlsx']"));
    }

    [Fact]
    public void Cartao_vertical_selecionado_de_pasta_com_MenuLabel()
    {
        var cortado = Render<RvmFileCard>(p => p
            .Add(x => x.Name, "Safra 2026")
            .Add(x => x.Category, RvmFileCardCategory.Folder)
            .Add(x => x.Layout, RvmFileCardLayout.Vertical)
            .Add(x => x.Selected, true)
            .Add(x => x.MenuLabel, "Opcoes da pasta")
            .Add(x => x.MenuItems, (RenderFragment)(_ => { })));

        var raiz = cortado.Find(".rvm-file-card");
        Assert.Contains("rvm-vertical", raiz.ClassList);
        Assert.Contains("rvm-com-topo", raiz.ClassList);
        Assert.Contains("rvm-selecionado", raiz.ClassList);
        Assert.NotNull(cortado.Find(".rvm-icone-arquivo .rvm-pasta-frente"));
        Assert.NotNull(cortado.Find("[aria-label='Opcoes da pasta']"));
        Assert.Empty(cortado.FindAll("input[type=checkbox]"));
        Assert.Empty(cortado.FindAll(".rvm-meta"));
    }

    [Fact]
    public void Cartao_de_imagem_e_padroes()
    {
        var imagem = Render<RvmFileCard>(p => p
            .Add(x => x.Name, "talhao.jpg")
            .Add(x => x.Category, RvmFileCardCategory.Image)
            .Add(x => x.Layout, RvmFileCardLayout.Vertical));
        var semNada = Render<RvmFileCard>(p => p.Add(x => x.Name, "sem-extensao"));

        Assert.NotNull(imagem.Find(".rvm-imagem[aria-hidden=true]"));
        Assert.DoesNotContain("rvm-com-topo", imagem.Find(".rvm-file-card").ClassList);
        Assert.Empty(imagem.FindAll(".rvm-topo"));
        // Extensao desconhecida vira texto, como no RVM.UI.
        Assert.Contains("TXT", semNada.Find("svg").TextContent);
        Assert.Empty(semNada.FindAll(".rvm-menu"));
    }

    // --- RvmFileTypeCard ---

    [Theory]
    [InlineData(RvmUploadStatus.None, null, "rvm-file-type-card")]
    [InlineData(RvmUploadStatus.Uploading, "Enviando", "rvm-enviando")]
    [InlineData(RvmUploadStatus.Uploaded, "Enviado", "rvm-enviado")]
    [InlineData(RvmUploadStatus.Failed, "Falha no envio", "rvm-falhou")]
    public void Cartao_de_tipo_nos_quatro_estados(RvmUploadStatus estado, string? texto, string classe)
    {
        var cortado = Render<RvmFileTypeCard>(p => p
            .Add(x => x.Name, "contrato.docx")
            .Add(x => x.Size, 1_258_291)
            .Add(x => x.Status, estado));

        Assert.Contains(classe, cortado.Find(".rvm-file-type-card").ClassList);
        Assert.Contains("1,2 MB", cortado.Find(".rvm-detalhes").TextContent);
        Assert.Contains("DOC", cortado.Find("svg").TextContent);
        if (texto is null)
        {
            Assert.Empty(cortado.FindAll("[role=status]"));
        }
        else
        {
            Assert.Equal(texto, cortado.Find("[role=status]").TextContent);
        }
    }

    [Fact]
    public void Cartao_de_tipo_enviando_tem_barra_e_porcentagem_limitada()
    {
        var cortado = Render<RvmFileTypeCard>(p => p
            .Add(x => x.Name, "mapa.png")
            .Add(x => x.Size, 512)
            .Add(x => x.Status, RvmUploadStatus.Uploading)
            .Add(x => x.Progress, 140));

        var barra = cortado.Find("[role=progressbar]");
        Assert.Equal("100", barra.GetAttribute("aria-valuenow"));
        Assert.Equal("Enviando mapa.png", barra.GetAttribute("aria-label"));
        Assert.Contains("100%", cortado.Find(".rvm-detalhes").TextContent);
        Assert.Contains("512 B", cortado.Find(".rvm-detalhes").TextContent);
    }

    [Fact]
    public void Cartao_de_tipo_enviando_sem_progresso_nao_tem_barra()
    {
        var cortado = Render<RvmFileTypeCard>(p => p.Add(x => x.Name, "a.pdf").Add(x => x.Status, RvmUploadStatus.Uploading));

        Assert.Empty(cortado.FindAll("[role=progressbar]"));
    }

    [Fact]
    public void Cartao_de_tipo_remove_pelo_X_nomeado_e_Icon_vence_a_extensao()
    {
        var removeu = 0;
        var cortado = Render<RvmFileTypeCard>(p => p
            .Add(x => x.Name, "planilha.pdf")
            .Add(x => x.Icon, RvmFileIconName.Xls)
            .Add(x => x.OnRemove, () => removeu++));

        Assert.Contains("XLS", cortado.Find("svg").TextContent);
        var x = cortado.Find("button[aria-label='Remover planilha.pdf']");
        x.Click();
        Assert.Equal(1, removeu);
    }

    [Fact]
    public void Cartao_de_tipo_com_Actions_troca_o_X()
    {
        var cortado = Render<RvmFileTypeCard>(p => p
            .Add(x => x.Name, "a.pdf")
            .Add(x => x.OnRemove, () => { })
            .Add(x => x.Actions, (RenderFragment)(b => b.AddMarkupContent(0, "<button>Baixar</button>"))));

        Assert.Equal("Baixar", cortado.Find(".rvm-acoes button").TextContent);
        Assert.Empty(cortado.FindAll("[aria-label='Remover a.pdf']"));
    }

    [Fact]
    public void Cartao_de_tipo_sem_remover_nem_acoes()
    {
        var cortado = Render<RvmFileTypeCard>(p => p.Add(x => x.Name, "a.pdf").Add(x => x.Class, "extra"));

        Assert.Empty(cortado.FindAll("button"));
        Assert.Contains("extra", cortado.Find(".rvm-file-type-card").ClassList);
    }
}
