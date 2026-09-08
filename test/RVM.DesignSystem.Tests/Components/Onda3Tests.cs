using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using RVM.DesignSystem.Components;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>
/// Comportamento e acessibilidade dos componentes de feedback (onda 3).
/// </summary>
/// <remarks>
/// O foco preso, o ESC e o retorno de foco do <see cref="RvmDialog"/> <b>nao</b> sao testados
/// aqui, e nao por esquecimento: quem os implementa e o <c>&lt;dialog&gt;</c> nativo com
/// <c>showModal()</c>, e o bUnit nao roda navegador — ele nao tem camada de topo, nem
/// inertizacao, nem foco de verdade. Testar isso aqui provaria que o mock funciona.
///
/// <para>
/// Esses tres estao no E2E, com navegador de verdade, que e onde o criterio de saida da onda
/// (`09-roadmap`) manda verificar.
/// </para>
/// </remarks>
public class Onda3Tests : BunitContext
{
    public Onda3Tests()
    {
        // O RvmDialog importa um modulo JS em OnAfterRenderAsync. Sem o modo solto, toda
        // renderizacao dele estouraria por falta de handler.
        JSInterop.Mode = JSRuntimeMode.Loose;

        Services.AddScoped<IRvmToastService, RvmToastService>();
        Services.AddScoped<IRvmDialogService, RvmDialogService>();
    }

    // ---------- RvmSpinner ----------

    [Fact]
    public void Spinner_poe_o_desenho_em_aria_hidden_e_o_TEXTO_no_role_status()
    {
        // Um SVG girando nao diz nada a quem nao ve a tela; a frase diz.
        var cut = Render<RvmSpinner>(p => p.Add(x => x.Label, "Carregando os pedidos…"));

        Assert.Equal("status", cut.Find("span.rvm-spinner").GetAttribute("role"));
        Assert.Equal("true", cut.Find("span.rvm-spinner__roda").GetAttribute("aria-hidden"));
        Assert.Contains("Carregando os pedidos…", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void Spinner_sem_rotulo_visivel_MANTEM_o_texto_para_o_leitor_de_tela()
    {
        // Um role="status" vazio e uma regiao que anuncia silencio.
        var cut = Render<RvmSpinner>();

        Assert.NotNull(cut.Find("span.rvm-sr-only"));
        Assert.Contains("Carregando", cut.Find("span.rvm-sr-only").TextContent, StringComparison.Ordinal);
    }

    // ---------- RvmProgress ----------

    [Fact]
    public void Progresso_determinado_informa_valuenow()
    {
        var cut = Render<RvmProgress>(p => p.Add(x => x.Value, 35));

        var barra = cut.Find("[role=progressbar]");
        Assert.Equal("35", barra.GetAttribute("aria-valuenow"));
        Assert.Equal("0", barra.GetAttribute("aria-valuemin"));
        Assert.Equal("100", barra.GetAttribute("aria-valuemax"));
        Assert.Equal("35%", barra.GetAttribute("aria-valuetext"));
    }

    [Fact]
    public void Progresso_indeterminado_OMITE_valuenow()
    {
        // A omissao E o significado: "nao sei quanto falta". Preencher com 0 faria o leitor
        // anunciar "zero por cento" para sempre.
        var cut = Render<RvmProgress>();

        var barra = cut.Find("[role=progressbar]");
        Assert.Null(barra.GetAttribute("aria-valuenow"));
        Assert.Null(barra.GetAttribute("aria-valuemin"));
    }

    [Fact]
    public void Progresso_limita_o_valor_em_vez_de_estourar()
    {
        var cut = Render<RvmProgress>(p => p.Add(x => x.Value, 250));

        Assert.Equal("100", cut.Find("[role=progressbar]").GetAttribute("aria-valuenow"));
    }

    // ---------- RvmSkeleton ----------

    [Fact]
    public void Esqueleto_e_SEMPRE_invisivel_ao_leitor_de_tela()
    {
        // Anunciar cinco caixas cinzas nao informa nada e atrasa a informacao real.
        var cut = Render<RvmSkeleton>();

        Assert.Equal("true", cut.Find("span.rvm-skeleton").GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Esqueleto_de_varias_linhas_deixa_a_ULTIMA_mais_curta()
    {
        var cut = Render<RvmSkeleton>(p => p.Add(x => x.Lines, 3));

        var linhas = cut.FindAll("span.rvm-skeleton");
        Assert.Equal(3, linhas.Count);
        Assert.DoesNotContain("rvm-skeleton--ultima", linhas[0].GetAttribute("class")!, StringComparison.Ordinal);
        Assert.Contains("rvm-skeleton--ultima", linhas[2].GetAttribute("class")!, StringComparison.Ordinal);
    }

    // ---------- RvmAlert ----------

    [Fact]
    public void Alerta_estatico_NAO_tem_role_alert()
    {
        // Um role="alert" interrompe o leitor de tela a cada render. Num aviso que ja estava na
        // tela quando ela abriu, isso e ruido que atropela o titulo da pagina.
        var cut = Render<RvmAlert>(p => p.AddChildContent("Atenção ao prazo."));

        Assert.Null(cut.Find("div.rvm-alert").GetAttribute("role"));
    }

    [Fact]
    public void Alerta_vivo_de_ERRO_interrompe_e_o_resto_espera()
    {
        var erro = Render<RvmAlert>(p => p
            .Add(x => x.Live, true)
            .Add(x => x.Severity, RvmSeverity.Danger)
            .AddChildContent("Não foi possível salvar."));

        Assert.Equal("alert", erro.Find("div.rvm-alert").GetAttribute("role"));

        var sucesso = Render<RvmAlert>(p => p
            .Add(x => x.Live, true)
            .Add(x => x.Severity, RvmSeverity.Success)
            .AddChildContent("Salvo."));

        // Anunciar "salvo com sucesso" por cima do que o leitor estava dizendo custa mais do
        // que informa.
        Assert.Equal("status", sucesso.Find("div.rvm-alert").GetAttribute("role"));
    }

    [Theory]
    [InlineData(RvmSeverity.Success, "check-circle")]
    [InlineData(RvmSeverity.Warning, "warning")]
    [InlineData(RvmSeverity.Danger, "x-circle")]
    [InlineData(RvmSeverity.Info, "info")]
    public void Cada_severidade_tem_uma_FORMA_propria(RvmSeverity severidade, string icone)
    {
        // As formas precisam ser distinguiveis SEM cor — e o que faz severidade nao depender
        // so dela. O par fica cravado para que trocar um icone seja decisao consciente.
        //
        // A comparacao e pelo DESENHO renderizado, e nao pelo nome passado adiante: assim o
        // teste prova o que chega na tela, e nao que uma tabela interna concorda consigo mesma.
        var esperado = Render<RvmIcon>(p => p
            .Add(x => x.Name, icone)
            .Add(x => x.Weight, RvmIconWeight.Fill));

        var caminho = esperado.Find("svg").InnerHtml;

        var alerta = Render<RvmAlert>(p => p
            .Add(x => x.Severity, severidade)
            .AddChildContent("mensagem"));

        Assert.Contains(caminho, alerta.Find("svg.rvm-alert__icone").InnerHtml, StringComparison.Ordinal);
    }

    // ---------- RvmEmptyState ----------

    [Theory]
    [InlineData(RvmEmptyStateVariant.Empty, "Nada por aqui ainda")]
    [InlineData(RvmEmptyStateVariant.NoResults, "Nenhum resultado para essa busca")]
    [InlineData(RvmEmptyStateVariant.Error, "Não foi possível carregar")]
    [InlineData(RvmEmptyStateVariant.Forbidden, "Você não tem acesso a esta área")]
    public void Cada_razao_de_estar_vazio_tem_texto_PROPRIO(RvmEmptyStateVariant variante, string titulo)
    {
        // Tratar as quatro como o mesmo estado vazio e o que produz aquele "Nenhum resultado
        // encontrado" que nao ajuda ninguem a sair dali.
        var cut = Render<RvmEmptyState>(p => p.Add(x => x.Variant, variante));

        Assert.Equal(titulo, cut.Find("h2.rvm-empty-state__titulo").TextContent);
    }

    [Fact]
    public void O_estado_de_ERRO_usa_o_fallback_em_PT_BR_sem_jargao()
    {
        var cut = Render<RvmEmptyState>(p => p.Add(x => x.Variant, RvmEmptyStateVariant.Error));

        Assert.Contains("Tivemos um problema técnico", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void O_titulo_do_estado_vazio_e_h2_e_nao_h1()
    {
        // O estado vazio ocupa uma REGIAO da tela, nao a tela. Disputar com o h1 da pagina
        // bagunca a lista de titulos do leitor de tela.
        var cut = Render<RvmEmptyState>();

        Assert.Empty(cut.FindAll("h1"));
        Assert.NotNull(cut.Find("h2"));
    }

    // ---------- RvmToastService ----------

    [Fact]
    public void O_servico_de_toast_IMPOE_o_piso_de_cinco_segundos()
    {
        // O piso nao e estetico: e o tempo de notar que apareceu algo, mover os olhos e ler.
        // Por isso ele e imposto no SERVICO, e nao no host — vale para qualquer host.
        var servico = new RvmToastService();
        servico.Show("Salvo.", duration: TimeSpan.FromMilliseconds(200));

        Assert.Equal(IRvmToastService.MinimoNaTela, servico.Current[0].Duration);
    }

    [Fact]
    public void O_servico_de_toast_respeita_duracao_MAIOR_que_o_piso()
    {
        var servico = new RvmToastService();
        servico.Show("Relatório pronto.", duration: TimeSpan.FromSeconds(20));

        Assert.Equal(TimeSpan.FromSeconds(20), servico.Current[0].Duration);
    }

    [Fact]
    public void O_servico_de_toast_avisa_quem_escuta()
    {
        var servico = new RvmToastService();
        var avisos = 0;
        servico.Changed += () => avisos++;

        var id = servico.Show("Um.");
        servico.Dismiss(id);

        Assert.Equal(2, avisos);
        Assert.Empty(servico.Current);
    }

    [Fact]
    public void Dispensar_um_id_que_nao_existe_nao_avisa_ninguem()
    {
        var servico = new RvmToastService();
        var avisos = 0;
        servico.Changed += () => avisos++;

        servico.Dismiss(Guid.NewGuid());

        Assert.Equal(0, avisos);
    }

    // ---------- RvmToastHost ----------

    [Fact]
    public void O_host_de_toast_cria_as_regioes_aria_live_MESMO_VAZIO()
    {
        // ⚠️ E o teste mais importante deste arquivo. Uma regiao aria-live so e observada a
        // partir do instante em que entra no DOM: se ela nascesse junto com o primeiro toast,
        // a mensagem nao seria anunciada. O defeito passa no teste manual com os olhos.
        var cut = Render<RvmToastHost>();

        Assert.Equal("polite", cut.Find("[role=status]").GetAttribute("aria-live"));
        Assert.Equal("assertive", cut.Find("[role=alert]").GetAttribute("aria-live"));
    }

    [Fact]
    public void Erro_vai_para_a_regiao_ASSERTIVA_e_o_resto_para_a_educada()
    {
        var servico = Services.GetRequiredService<IRvmToastService>();
        var cut = Render<RvmToastHost>();

        servico.Success("Pedido salvo.");
        servico.Error("Não foi possível salvar.");
        cut.Render();

        var educada = cut.Find("[role=status]");
        var assertiva = cut.Find("[role=alert]");

        Assert.Contains("Pedido salvo.", educada.InnerHtml, StringComparison.Ordinal);
        Assert.DoesNotContain("Não foi possível salvar.", educada.InnerHtml, StringComparison.Ordinal);
        Assert.Contains("Não foi possível salvar.", assertiva.InnerHtml, StringComparison.Ordinal);
    }

    [Fact]
    public void O_host_limita_quantos_toasts_aparecem_de_uma_vez()
    {
        // Sem limite, uma falha em laco enche a tela e cobre justamente a interface que a
        // pessoa usaria para sair do problema.
        var servico = Services.GetRequiredService<IRvmToastService>();
        var cut = Render<RvmToastHost>(p => p.Add(x => x.Max, 2));

        for (var i = 1; i <= 5; i++)
        {
            servico.Show($"Mensagem {i}.");
        }

        cut.Render();

        Assert.Equal(2, cut.FindAll("div.rvm-toast").Count);

        // Os mais RECENTES ganham: a ultima coisa que aconteceu e a mais relevante.
        Assert.Contains("Mensagem 5.", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("Mensagem 1.", cut.Markup, StringComparison.Ordinal);
    }

    // ---------- RvmDialogService ----------

    [Fact]
    public async Task Sair_SEM_decidir_devolve_false()
    {
        // ESC, "x" e clique fora sao todos "nao". O caminho destrutivo nunca pode ser o que
        // acontece por acidente.
        var servico = new RvmDialogService();
        var resposta = servico.ConfirmAsync("Excluir?", "Isto não pode ser desfeito.");

        servico.Responder(false);

        Assert.False(await resposta);
    }

    [Fact]
    public async Task Confirmar_devolve_true_para_quem_chamou()
    {
        var servico = new RvmDialogService();
        var resposta = servico.ConfirmAsync("Publicar?", "Fica visível para todo mundo.");

        servico.Responder(true);

        Assert.True(await resposta);
    }

    [Fact]
    public async Task Duas_confirmacoes_seguidas_ENFILEIRAM_em_vez_de_se_atropelar()
    {
        // Sobrescrever a primeira deixaria quem a chamou esperando para sempre; abrir as duas
        // faria a de baixo ficar inerte, com a pessoa respondendo a pergunta errada.
        var servico = new RvmDialogService();

        var primeira = servico.ConfirmAsync("Primeira", "?");
        var segunda = servico.ConfirmAsync("Segunda", "?");

        Assert.Equal("Primeira", servico.Current!.Title);
        Assert.False(primeira.IsCompleted);
        Assert.False(segunda.IsCompleted);

        servico.Responder(true);

        Assert.True(await primeira);
        Assert.Equal("Segunda", servico.Current!.Title);

        servico.Responder(false);
        Assert.False(await segunda);
        Assert.Null(servico.Current);
    }

    [Fact]
    public void Responder_sem_dialogo_aberto_nao_estoura()
    {
        var servico = new RvmDialogService();

        servico.Responder(true);

        Assert.Null(servico.Current);
    }

    // ---------- RvmDialogHost ----------

    [Fact]
    public void O_host_de_dialogo_poe_CANCELAR_antes_de_confirmar_no_DOM()
    {
        // Cancelar e o primeiro a receber foco depois do botao de fechar. Numa confirmacao
        // destrutiva, o caminho seguro e o que deve estar debaixo do dedo.
        var servico = Services.GetRequiredService<IRvmDialogService>();
        var cut = Render<RvmDialogHost>();

        _ = servico.ConfirmAsync(new RvmConfirmOptions(
            "Excluir o pedido?", "Isto não pode ser desfeito.",
            ConfirmLabel: "Excluir", CancelLabel: "Manter",
            Severity: RvmSeverity.Danger));

        cut.Render();

        var botoes = cut.FindAll("footer button");
        Assert.Equal(2, botoes.Count);
        Assert.Contains("Manter", botoes[0].TextContent, StringComparison.Ordinal);
        Assert.Contains("Excluir", botoes[1].TextContent, StringComparison.Ordinal);

        // E a acao destrutiva e pintada como destrutiva.
        Assert.Contains("rvm-button--danger", botoes[1].GetAttribute("class")!, StringComparison.Ordinal);
    }

    [Fact]
    public void O_dialogo_tem_NOME_acessivel_vindo_do_titulo()
    {
        // Sem nome, o leitor anuncia "diálogo" e nada mais — e quem nao ve a tela nao sabe o
        // que acabou de abrir na frente dela.
        var cut = Render<RvmDialog>(p => p
            .Add(x => x.Title, "Excluir o pedido?")
            .Add(x => x.Open, true));

        var dialogo = cut.Find("dialog");
        var idDoTitulo = dialogo.GetAttribute("aria-labelledby");

        Assert.False(string.IsNullOrWhiteSpace(idDoTitulo));
        Assert.Equal("Excluir o pedido?", cut.Find($"#{idDoTitulo}").TextContent);
    }

    [Fact]
    public void O_dialogo_NAO_repete_role_nem_aria_modal()
    {
        // Os dois sao implicitos no <dialog> aberto por showModal(). Atributo ARIA redundante
        // com a semantica nativa e uma das causas mais comuns de leitor anunciar errado.
        var cut = Render<RvmDialog>(p => p.Add(x => x.Title, "Título"));

        var dialogo = cut.Find("dialog");
        Assert.Null(dialogo.GetAttribute("role"));
        Assert.Null(dialogo.GetAttribute("aria-modal"));
    }

    // ---------- RvmTooltip ----------

    [Fact]
    public void A_dica_existe_SEMPRE_no_DOM()
    {
        // aria-describedby precisa apontar para um elemento que exista. Renderizar a dica so no
        // hover faria a referencia apontar para o vazio na maior parte do tempo — e leitores de
        // tela leem a descricao quando o controle recebe FOCO, nao quando o mouse passa.
        var cut = Render<RvmTooltip>(p => p
            .Add(x => x.Text, "Salva e volta à lista")
            .AddChildContent("<button>Salvar</button>"));

        var balao = cut.Find("[role=tooltip]");
        Assert.Contains("Salva e volta à lista", balao.TextContent, StringComparison.Ordinal);
        Assert.Null(balao.GetAttribute("data-visivel"));
    }

    [Fact]
    public void A_dica_aparece_no_FOCO_e_nao_so_no_ponteiro()
    {
        // A falha mais comum de tooltip na web: quem navega por teclado nunca a ve.
        var cut = Render<RvmTooltip>(p => p
            .Add(x => x.Text, "Dica")
            .AddChildContent("<button>Salvar</button>"));

        cut.Find("span.rvm-tooltip").FocusIn();
        Assert.Equal("true", cut.Find("[role=tooltip]").GetAttribute("data-visivel"));

        cut.Find("span.rvm-tooltip").FocusOut();
        Assert.Null(cut.Find("[role=tooltip]").GetAttribute("data-visivel"));
    }

    [Fact]
    public void ESC_esconde_a_dica()
    {
        // WCAG 1.4.13: sem isso, uma dica que cobre o campo seguinte prende quem depende de
        // ampliacao.
        var cut = Render<RvmTooltip>(p => p
            .Add(x => x.Text, "Dica")
            .AddChildContent("<button>Salvar</button>"));

        cut.Find("span.rvm-tooltip").MouseEnter();
        Assert.Equal("true", cut.Find("[role=tooltip]").GetAttribute("data-visivel"));

        cut.Find("span.rvm-tooltip").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        Assert.Null(cut.Find("[role=tooltip]").GetAttribute("data-visivel"));
    }
}
