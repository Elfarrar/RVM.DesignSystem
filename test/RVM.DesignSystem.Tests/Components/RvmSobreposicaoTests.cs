using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Microsoft.JSInterop;
using RVM.DesignSystem;
using RVM.DesignSystem.Components.Dialog;
using RVM.DesignSystem.Components.Drawer;
using RVM.DesignSystem.Components.Snackbar;

namespace RVM.DesignSystem.Tests.Components;

public class RvmDialogTests : BunitContext
{
    public RvmDialogTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<RvmDialog> Dialogo(bool aberto = true, Action<bool>? aoMudar = null,
        Action<ComponentParameterCollectionBuilder<RvmDialog>>? extra = null)
        => Render<RvmDialog>(p =>
        {
            p.Add(x => x.Open, aberto)
             .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => aoMudar?.Invoke(v)))
             .Add(x => x.Title, "Excluir talhao?")
             .AddChildContent("O historico de aplicacoes tambem sera apagado.")
             .Add(x => x.Actions, (RenderFragment)(b => b.AddMarkupContent(0, "<button>Excluir</button>")));
            extra?.Invoke(p);
        });

    [Fact]
    public void Fechado_nao_existe_no_dom()
    {
        var cortado = Dialogo(aberto: false);

        Assert.Empty(cortado.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Aberto_e_um_dialogo_modal_nomeado_pelo_titulo()
    {
        var cortado = Dialogo();

        var caixa = cortado.Find("[role=dialog]");
        Assert.Equal("true", caixa.GetAttribute("aria-modal"));
        Assert.Equal("-1", caixa.GetAttribute("tabindex"));
        var titulo = cortado.Find("h2.rvm-titulo");
        Assert.Equal("Excluir talhao?", titulo.TextContent);
        Assert.Equal(titulo.Id, caixa.GetAttribute("aria-labelledby"));
        Assert.Null(caixa.GetAttribute("aria-describedby"));
        Assert.Equal("Excluir", cortado.Find(".rvm-acoes button").TextContent);
        Assert.Equal("rvm-dialogo rvm-medio", cortado.Find("div.rvm-dialogo").GetAttribute("class"));
        Assert.Equal("true", cortado.Find(".rvm-fundo").GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Esc_e_fundo_fecham_e_avisam()
    {
        var estados = new List<bool>();
        var cortado = Dialogo(aoMudar: estados.Add);

        cortado.Find("[role=dialog]").KeyDown(key: "a");
        Assert.NotEmpty(cortado.FindAll("[role=dialog]"));

        cortado.Find("[role=dialog]").KeyDown(key: "Escape");
        Assert.Empty(cortado.FindAll("[role=dialog]"));

        var outro = Dialogo(aoMudar: estados.Add);
        outro.Find(".rvm-fundo").Click();
        Assert.Empty(outro.FindAll("[role=dialog]"));
        Assert.Equal([false, false], estados);
    }

    [Fact]
    public void Alerta_e_alertdialog_le_o_corpo_e_o_fundo_nao_fecha()
    {
        var cortado = Dialogo(extra: p => p.Add(x => x.Alert, true));

        var caixa = cortado.Find("[role=alertdialog]");
        Assert.Equal(cortado.Find(".rvm-conteudo").Id, caixa.GetAttribute("aria-describedby"));
        cortado.Find(".rvm-fundo").Click();
        Assert.NotEmpty(cortado.FindAll("[role=alertdialog]"));
    }

    [Fact]
    public void Esc_e_fundo_desligados_nao_fecham()
    {
        var cortado = Dialogo(extra: p => p.Add(x => x.CloseOnEscape, false).Add(x => x.CloseOnBackdropClick, false));

        cortado.Find("[role=dialog]").KeyDown(key: "Escape");
        cortado.Find(".rvm-fundo").Click();

        Assert.NotEmpty(cortado.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Botao_de_fechar_tem_nome_e_fecha()
    {
        var cortado = Dialogo(extra: p => p.Add(x => x.ShowCloseButton, true).Add(x => x.CloseLabel, "Fechar aviso"));

        var fechar = cortado.Find("button.rvm-fechar");
        Assert.Equal("Fechar aviso", fechar.GetAttribute("aria-label"));
        fechar.Click();

        Assert.Empty(cortado.FindAll("[role=dialog]"));
    }

    [Fact]
    public async Task Fechar_por_codigo_quando_ja_fechado_nao_avisa()
    {
        var avisos = 0;
        var cortado = Dialogo(aberto: false, aoMudar: _ => avisos++);

        await cortado.InvokeAsync(cortado.Instance.FecharAsync);

        Assert.Equal(0, avisos);
    }

    [Theory]
    [InlineData(RvmSize.Small, false, "rvm-dialogo rvm-pequeno")]
    [InlineData(RvmSize.Large, false, "rvm-dialogo rvm-grande")]
    [InlineData(RvmSize.Medium, true, "rvm-dialogo rvm-tela-cheia")]
    public void Tamanho_e_tela_cheia_viram_classes(RvmSize tamanho, bool telaCheia, string esperado)
    {
        var cortado = Dialogo(extra: p => p.Add(x => x.Size, tamanho).Add(x => x.FullScreen, telaCheia).AddUnmatched("class", "minha"));

        Assert.Equal($"{esperado} minha", cortado.Find("div.rvm-dialogo").GetAttribute("class"));
    }

    [Fact]
    public void Sem_titulo_nao_tem_topo_nem_aria_labelledby()
    {
        var cortado = Render<RvmDialog>(p => p.Add(x => x.Open, true).AddChildContent("Salvo.").AddUnmatched("aria-label", "Aviso"));

        var caixa = cortado.Find("[role=dialog]");
        Assert.Null(caixa.GetAttribute("aria-labelledby"));
        Assert.Equal("Aviso", caixa.GetAttribute("aria-label"));
        Assert.Empty(cortado.FindAll(".rvm-topo"));
    }

    [Fact]
    public void Esc_no_dialogo_de_dentro_nao_fecha_o_de_fora()
    {
        var fechados = new List<string>();
        var cortado = Render<RvmDialog>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Title, "Editar talhao")
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, _ => fechados.Add("fora")))
            .Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<RvmDialog>(0);
                b.AddAttribute(1, nameof(RvmDialog.Open), true);
                b.AddAttribute(2, nameof(RvmDialog.Title), "Excluir?");
                b.AddAttribute(3, nameof(RvmDialog.OpenChanged), EventCallback.Factory.Create<bool>(this, _ => fechados.Add("dentro")));
                b.CloseComponent();
            })));

        cortado.Find("[role=dialog] [role=dialog]").KeyDown(key: "Escape");

        Assert.Equal(["dentro"], fechados);
    }

    [Fact]
    public async Task Fechar_antes_de_o_abrir_voltar_do_js_ainda_fecha()
    {
        // Review da onda 3: abrir e fechar rapido deixava a rolagem da pagina travada para sempre.
        var js = new JsControlado();
        Services.AddSingleton<IJSRuntime>(js);
        var cortado = Render<RvmDialog>(p => p.Add(x => x.Open, true).Add(x => x.Title, "Rapido"));

        cortado.WaitForAssertion(() => Assert.Equal(1, js.PedidosDeAbrir));
        cortado.Render(p => p.Add(x => x.Open, false));

        // So agora o "abrir" volta do JS — depois de o fechar ja ter sido pedido.
        js.LiberarAbrir();

        cortado.WaitForAssertion(() => Assert.Equal(1, js.Fechamentos));
        await Task.CompletedTask;
    }

    /// <summary>IJSRuntime em que o teste decide quando o "abrir" termina.</summary>
    private sealed class JsControlado : IJSRuntime
    {
        private readonly TaskCompletionSource _abrir = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private int _pedidosDeAbrir;
        private int _fechamentos;

        public int PedidosDeAbrir => _pedidosDeAbrir;

        public int Fechamentos => _fechamentos;

        public void LiberarAbrir() => _abrir.TrySetResult();

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            => new((TValue)(object)new Modulo(this));

        private sealed class Modulo(JsControlado dono) : IJSObjectReference
        {
            public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
                => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

            public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            {
                if (identifier == "abrir")
                {
                    Interlocked.Increment(ref dono._pedidosDeAbrir);
                    await dono._abrir.Task;
                    return (TValue)(object)new Modulo(dono);
                }

                if (identifier == "fechar")
                {
                    Interlocked.Increment(ref dono._fechamentos);
                }

                return default!;
            }

            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        }
    }

    [Fact]
    public void Abrir_e_fechar_chama_o_modulo_de_foco()
    {
        var modulo = JSInterop.SetupModule("./_content/RVM.DesignSystem/rvm-sobreposicao.js");
        var aberta = modulo.SetupModule("abrir", _ => true);
        aberta.SetupVoid("fechar").SetVoidResult();

        var cortado = Dialogo();
        Assert.Single(modulo.Invocations, i => i.Identifier == "abrir");

        cortado.Find("[role=dialog]").KeyDown(key: "Escape");
        Assert.Single(aberta.Invocations, i => i.Identifier == "fechar");
    }
}

public class RvmDrawerTests : BunitContext
{
    public RvmDrawerTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Permanente_e_um_aside_nomeado_sempre_visivel()
    {
        var cortado = Render<RvmDrawer>(p => p
            .Add(x => x.Variant, RvmDrawerVariant.Permanent)
            .Add(x => x.AriaLabel, "Menu principal")
            .Add(x => x.Size, "280px")
            .AddChildContent("<nav>itens</nav>"));

        var aside = cortado.Find("aside");
        Assert.Equal("Menu principal", aside.GetAttribute("aria-label"));
        Assert.Equal("rvm-gaveta rvm-permanente rvm-esquerda", aside.GetAttribute("class"));
        Assert.Equal("width: 280px", aside.GetAttribute("style"));
        Assert.Empty(cortado.FindAll("[role=dialog], .rvm-fundo"));
    }

    [Fact]
    public void Temporaria_fechada_nao_existe_e_aberta_e_dialogo_modal()
    {
        var fechada = Render<RvmDrawer>(p => p.Add(x => x.AriaLabel, "Filtros"));
        Assert.Empty(fechada.FindAll("[role=dialog]"));

        var aberta = Render<RvmDrawer>(p => p.Add(x => x.AriaLabel, "Filtros").Add(x => x.Open, true).Add(x => x.Anchor, RvmDrawerAnchor.Right));
        var caixa = aberta.Find("[role=dialog]");
        Assert.Equal("true", caixa.GetAttribute("aria-modal"));
        Assert.Equal("Filtros", caixa.GetAttribute("aria-label"));
        Assert.Equal("rvm-gaveta rvm-temporaria rvm-direita", caixa.GetAttribute("class"));
        Assert.Equal("width: 320px", caixa.GetAttribute("style"));
    }

    [Theory]
    [InlineData(RvmDrawerAnchor.Top, "rvm-topo", "height: 320px")]
    [InlineData(RvmDrawerAnchor.Bottom, "rvm-base", "height: 320px")]
    [InlineData(RvmDrawerAnchor.Left, "rvm-esquerda", "width: 320px")]
    public void Lado_define_classe_e_medida(RvmDrawerAnchor lado, string classe, string estilo)
    {
        var cortado = Render<RvmDrawer>(p => p.Add(x => x.AriaLabel, "Gaveta").Add(x => x.Open, true).Add(x => x.Anchor, lado).AddUnmatched("class", "minha"));

        var caixa = cortado.Find("[role=dialog]");
        Assert.Equal($"rvm-gaveta rvm-temporaria {classe} minha", caixa.GetAttribute("class"));
        Assert.Equal(estilo, caixa.GetAttribute("style"));
    }

    [Fact]
    public void Esc_e_fundo_fecham_e_avisam()
    {
        var estados = new List<bool>();
        IRenderedComponent<RvmDrawer> Gaveta() => Render<RvmDrawer>(p => p
            .Add(x => x.AriaLabel, "Filtros")
            .Add(x => x.Open, true)
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, estados.Add)));

        var porTecla = Gaveta();
        porTecla.Find("[role=dialog]").KeyDown(key: "Enter");
        Assert.NotEmpty(porTecla.FindAll("[role=dialog]"));
        porTecla.Find("[role=dialog]").KeyDown(key: "Escape");
        Assert.Empty(porTecla.FindAll("[role=dialog]"));

        var porFundo = Gaveta();
        porFundo.Find(".rvm-fundo").Click();
        Assert.Empty(porFundo.FindAll("[role=dialog]"));

        Assert.Equal([false, false], estados);
    }

    [Fact]
    public async Task Fechar_permanente_por_codigo_nao_faz_nada()
    {
        var avisos = 0;
        var cortado = Render<RvmDrawer>(p => p
            .Add(x => x.AriaLabel, "Menu")
            .Add(x => x.Variant, RvmDrawerVariant.Permanent)
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, _ => avisos++)));

        await cortado.InvokeAsync(cortado.Instance.FecharAsync);

        Assert.Equal(0, avisos);
        Assert.NotNull(cortado.Find("aside"));
    }
}

public class RvmSnackbarTests : BunitContext
{
    private readonly RvmSnackbarService _servico;

    // Relogio falso: o tempo so anda quando o teste manda. Sincroniza por sinal, nunca por espera.
    private readonly FakeTimeProvider _relogio = new();

    public RvmSnackbarTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<TimeProvider>(_relogio);
        Services.AddRvmDesignSystem();
        _servico = Services.GetRequiredService<RvmSnackbarService>();
    }

    [Fact]
    public void As_duas_regioes_vivas_existem_vazias_desde_o_inicio()
    {
        var cortado = Render<RvmSnackbarHost>();

        Assert.NotNull(cortado.Find("[role=status]"));
        Assert.NotNull(cortado.Find("[role=alert]"));
        Assert.Empty(cortado.FindAll(".rvm-mensagem"));
        Assert.Equal("rvm-avisos", cortado.Find("div.rvm-avisos").GetAttribute("class"));
    }

    [Fact]
    public void Neutra_vai_para_a_regiao_educada_e_erro_para_a_urgente()
    {
        var cortado = Render<RvmSnackbarHost>();

        cortado.InvokeAsync(() => _servico.Show("Talhao salvo."));
        cortado.InvokeAsync(() => _servico.Show("Nao conseguimos salvar.", new RvmSnackbarOptions { Color = RvmColor.Error }));

        cortado.WaitForAssertion(() =>
        {
            Assert.Equal("Talhao salvo.", cortado.Find("[role=status] .rvm-mensagem .rvm-texto").TextContent);
            Assert.Equal("Nao conseguimos salvar.", cortado.Find("[role=alert] .rvm-mensagem .rvm-texto").TextContent);
        });
        Assert.Equal("rvm-mensagem rvm-neutra", cortado.Find("[role=status] .rvm-mensagem").GetAttribute("class"));
        Assert.Equal("rvm-mensagem rvm-colorida rvm-error", cortado.Find("[role=alert] .rvm-mensagem").GetAttribute("class"));
        Assert.Empty(cortado.FindAll("[role=status] .rvm-mensagem > .rvm-icone"));
        Assert.NotNull(cortado.Find("[role=alert] .rvm-mensagem > .rvm-icone svg"));
    }

    [Fact]
    public void Acao_executa_e_fecha()
    {
        var desfeito = false;
        var cortado = Render<RvmSnackbarHost>();
        cortado.InvokeAsync(() => _servico.Show("Talhao excluido.", new RvmSnackbarOptions
        {
            ActionText = "Desfazer",
            OnAction = () => { desfeito = true; return Task.CompletedTask; },
            Duration = null
        }));

        cortado.WaitForElement("button.rvm-acao").Click();

        Assert.True(desfeito);
        cortado.WaitForAssertion(() => Assert.Empty(cortado.FindAll(".rvm-mensagem")));
    }

    [Fact]
    public void Botao_de_fechar_tem_nome_e_fecha()
    {
        var cortado = Render<RvmSnackbarHost>(p => p.Add(x => x.CloseLabel, "Dispensar"));
        cortado.InvokeAsync(() => _servico.Show("Oi.", new RvmSnackbarOptions { Duration = null }));

        var fechar = cortado.WaitForElement("button.rvm-fechar");
        Assert.Equal("Dispensar", fechar.GetAttribute("aria-label"));
        fechar.Click();

        cortado.WaitForAssertion(() => Assert.Empty(cortado.FindAll(".rvm-mensagem")));
    }

    [Fact]
    public void Some_sozinha_depois_da_duracao()
    {
        var cortado = Render<RvmSnackbarHost>();
        cortado.InvokeAsync(() => _servico.Show("Rapida.", new RvmSnackbarOptions { Duration = TimeSpan.FromSeconds(5), ShowCloseButton = false }));

        cortado.WaitForAssertion(() => Assert.NotEmpty(cortado.FindAll(".rvm-mensagem")));
        Assert.Empty(cortado.FindAll("button.rvm-fechar"));

        _relogio.Advance(TimeSpan.FromSeconds(4));
        Assert.NotEmpty(cortado.FindAll(".rvm-mensagem"));

        _relogio.Advance(TimeSpan.FromSeconds(1));
        cortado.WaitForAssertion(() => Assert.Empty(cortado.FindAll(".rvm-mensagem")));
    }

    [Fact]
    public async Task Mouse_em_cima_segura_a_mensagem_e_sair_recomeca_o_tempo()
    {
        var cortado = Render<RvmSnackbarHost>();
        await cortado.InvokeAsync(() => _servico.Show("Leia com calma.", new RvmSnackbarOptions { Duration = TimeSpan.FromSeconds(5) }));

        // Eventos AGUARDADOS: a versao sincrona volta antes do handler rodar, e o relogio falso andava
        // antes de o tempo ser rearmado — o teste falhava uma vez a cada tres.
        await cortado.WaitForElement(".rvm-mensagem").MouseEnterAsync(new MouseEventArgs());
        _relogio.Advance(TimeSpan.FromMinutes(1));
        Assert.NotEmpty(cortado.FindAll(".rvm-mensagem"));

        // Foco entrou e saiu: o tempo recomeca INTEIRO — 4 s depois ainda esta la, aos 5 s some.
        await cortado.Find(".rvm-mensagem").FocusInAsync(new FocusEventArgs());
        await cortado.Find(".rvm-mensagem").FocusOutAsync(new FocusEventArgs());
        _relogio.Advance(TimeSpan.FromSeconds(4));
        Assert.NotEmpty(cortado.FindAll(".rvm-mensagem"));
        _relogio.Advance(TimeSpan.FromSeconds(1));
        cortado.WaitForAssertion(() => Assert.Empty(cortado.FindAll(".rvm-mensagem")), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Fila_mostra_no_maximo_o_limite_e_a_proxima_toma_o_lugar()
    {
        _servico.MaxVisible = 2;
        var cortado = Render<RvmSnackbarHost>();
        RvmSnackbarMessage? primeira = null;
        cortado.InvokeAsync(() =>
        {
            primeira = _servico.Show("Um.", new RvmSnackbarOptions { Duration = null });
            _servico.Show("Dois.", new RvmSnackbarOptions { Duration = null });
            _servico.Show("Tres.", new RvmSnackbarOptions { Duration = null, Loading = true, Color = RvmColor.Info });
        });

        cortado.WaitForAssertion(() => Assert.Equal(2, cortado.FindAll(".rvm-mensagem").Count));
        cortado.InvokeAsync(() => _servico.Close(primeira!));

        cortado.WaitForAssertion(() => Assert.Equal(["Dois.", "Tres."], cortado.FindAll(".rvm-texto").Select(t => t.TextContent)));
        Assert.NotNull(cortado.Find("[role=progressbar]"));
    }

    [Fact]
    public void Fechar_todas_e_fechar_a_que_ja_saiu_nao_quebram()
    {
        var cortado = Render<RvmSnackbarHost>();
        RvmSnackbarMessage? mensagem = null;
        cortado.InvokeAsync(() =>
        {
            mensagem = _servico.Show("Um.", new RvmSnackbarOptions { Duration = null });
            _servico.Show("Dois.", new RvmSnackbarOptions { Color = RvmColor.Success });
        });
        cortado.WaitForAssertion(() => Assert.Equal(2, cortado.FindAll(".rvm-mensagem").Count));

        cortado.InvokeAsync(_servico.CloseAll);
        cortado.InvokeAsync(_servico.CloseAll);
        cortado.InvokeAsync(() => _servico.Close(mensagem!));

        cortado.WaitForAssertion(() => Assert.Empty(cortado.FindAll(".rvm-mensagem")));
    }

    [Fact]
    public void Texto_vazio_e_recusado()
    {
        Assert.Throws<ArgumentException>(() => _servico.Show("  "));
    }

    [Theory]
    [InlineData(RvmColor.Primary, "rvm-primary")]
    [InlineData(RvmColor.Secondary, "rvm-secondary")]
    [InlineData(RvmColor.Info, "rvm-info")]
    [InlineData(RvmColor.Success, "rvm-success")]
    [InlineData(RvmColor.Warning, "rvm-warning")]
    public void Cada_papel_tem_classe_e_icone(RvmColor cor, string classe)
    {
        var cortado = Render<RvmSnackbarHost>(p => p.AddUnmatched("class", "minha"));
        cortado.InvokeAsync(() => _servico.Show("Aviso.", new RvmSnackbarOptions { Color = cor }));

        cortado.WaitForAssertion(() => Assert.Equal($"rvm-mensagem rvm-colorida {classe}", cortado.Find(".rvm-mensagem").GetAttribute("class")));
        Assert.NotNull(cortado.Find(".rvm-mensagem > .rvm-icone svg"));
        Assert.Equal("rvm-avisos minha", cortado.Find("div.rvm-avisos").GetAttribute("class"));
    }

    [Fact]
    public async Task Descartar_o_host_para_de_ouvir()
    {
        var cortado = Render<RvmSnackbarHost>();
        await cortado.InvokeAsync(() => _servico.Show("Com relogio.", new RvmSnackbarOptions { Duration = TimeSpan.FromMinutes(1) }));
        cortado.WaitForAssertion(() => Assert.NotEmpty(cortado.FindAll(".rvm-mensagem")));

        await DisposeComponentsAsync();

        _servico.Show("Depois.");
    }
}
