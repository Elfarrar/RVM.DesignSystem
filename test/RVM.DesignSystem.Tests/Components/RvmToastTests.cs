using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using RVM.DesignSystem.Components.Icon;
using RVM.DesignSystem.Components.Toast;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>IRvmToast, RvmToastService e RvmToastProvider, do contrato com o RVM.UI (DSGN-017).</summary>
public class RvmToastTests : BunitContext
{
    // Relogio falso: o tempo so anda quando o teste manda. Sincroniza por sinal, nunca por espera.
    private readonly FakeTimeProvider _relogio = new();
    private readonly IRvmToast _toast;

    public RvmToastTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<TimeProvider>(_relogio);
        Services.AddRvmDesignSystem();
        _toast = Services.GetRequiredService<IRvmToast>();
    }

    // --- Servico ---

    [Fact]
    public void AddRvmDesignSystem_registra_o_toast_com_escopo_e_AddRvmToast_nao_duplica()
    {
        var servicos = new ServiceCollection();
        servicos.AddRvmDesignSystem();
        servicos.AddRvmToast();

        var registro = Assert.Single(servicos, s => s.ServiceType == typeof(IRvmToast));
        Assert.Equal(ServiceLifetime.Scoped, registro.Lifetime);
        Assert.Equal(typeof(RvmToastService), registro.ImplementationType);
        Assert.IsType<RvmToastService>(_toast);
    }

    [Fact]
    public void Show_poe_na_fila_em_ordem_com_duracao_padrao_por_gravidade()
    {
        var mudancas = 0;
        _toast.Changed += () => mudancas++;

        var info = _toast.Show("Cotacao atualizada.");
        _toast.Success("Talhao salvo.", "Talhao 12");
        _toast.Warning("Chuva forte amanha.");
        _toast.Error("Nao conseguimos salvar.");
        _toast.Info("Nova versao.");

        Assert.Equal(5, mudancas);
        var mensagens = _toast.Messages;
        Assert.Equal(info, mensagens[0].Id);
        Assert.Equal(
            [RvmToastSeverity.Info, RvmToastSeverity.Success, RvmToastSeverity.Warning, RvmToastSeverity.Error, RvmToastSeverity.Info],
            mensagens.Select(m => m.Severity));
        Assert.Equal("Talhao 12", mensagens[1].Title);
        Assert.Null(mensagens[0].Title);
        Assert.Equal(TimeSpan.FromSeconds(5), mensagens[0].Duration);
        Assert.Equal(TimeSpan.FromSeconds(5), mensagens[1].Duration);
        Assert.Equal(TimeSpan.FromSeconds(8), mensagens[2].Duration);
        Assert.Equal(TimeSpan.FromSeconds(8), mensagens[3].Duration);
        Assert.Null(mensagens[0].Action);
    }

    [Fact]
    public void Duracoes_padrao_e_limite_sao_os_do_RVM_UI()
    {
        Assert.Equal(5, RvmToastService.MaxVisible);
        Assert.Equal(TimeSpan.FromSeconds(10), RvmToastService.ActionDuration);
        Assert.Equal(TimeSpan.FromSeconds(5), RvmToastService.DefaultDuration(RvmToastSeverity.Info));
        Assert.Equal(TimeSpan.FromSeconds(5), RvmToastService.DefaultDuration(RvmToastSeverity.Success));
        Assert.Equal(TimeSpan.FromSeconds(8), RvmToastService.DefaultDuration(RvmToastSeverity.Warning));
        Assert.Equal(TimeSpan.FromSeconds(8), RvmToastService.DefaultDuration(RvmToastSeverity.Error));
    }

    [Fact]
    public void Acima_do_MaxVisible_o_mais_antigo_sai()
    {
        var ids = Enumerable.Range(1, 7).Select(i => _toast.Show($"Aviso {i}.", duration: TimeSpan.Zero)).ToList();

        Assert.Equal(RvmToastService.MaxVisible, _toast.Messages.Count);
        Assert.Equal(ids.Skip(2), _toast.Messages.Select(m => m.Id));
    }

    [Fact]
    public void Some_sozinho_depois_da_duracao_e_duracao_zero_fica()
    {
        var rapido = _toast.Show("Rapido.", duration: TimeSpan.FromSeconds(3));
        var fixo = _toast.Show("Fixo.", duration: TimeSpan.Zero);

        _relogio.Advance(TimeSpan.FromSeconds(2));
        Assert.Contains(_toast.Messages, m => m.Id == rapido);

        _relogio.Advance(TimeSpan.FromSeconds(1));
        Assert.DoesNotContain(_toast.Messages, m => m.Id == rapido);

        _relogio.Advance(TimeSpan.FromHours(1));
        Assert.Equal(fixo, Assert.Single(_toast.Messages).Id);
    }

    [Fact]
    public void Pause_segura_e_Resume_recomeca_com_a_duracao_cheia()
    {
        var id = _toast.Show("Leia com calma.");

        _relogio.Advance(TimeSpan.FromSeconds(4));
        _toast.Pause(id);
        _relogio.Advance(TimeSpan.FromMinutes(1));
        Assert.Single(_toast.Messages);

        _toast.Resume(id);
        _toast.Resume(id); // Duas vezes nao arma dois relogios.
        _relogio.Advance(TimeSpan.FromSeconds(4));
        Assert.Single(_toast.Messages);
        _relogio.Advance(TimeSpan.FromSeconds(1));
        Assert.Empty(_toast.Messages);

        // Pausar ou retomar o que ja saiu nao quebra.
        _toast.Pause(id);
        _toast.Resume(id);
        Assert.Empty(_toast.Messages);
    }

    [Fact]
    public void Dismiss_e_Clear_avisam_so_quando_mudam()
    {
        var a = _toast.Show("A.");
        _toast.Show("B.");
        var mudancas = 0;
        _toast.Changed += () => mudancas++;

        _toast.Dismiss(a);
        _toast.Dismiss(a);
        _toast.Dismiss(Guid.NewGuid());
        Assert.Equal(1, mudancas);
        Assert.Single(_toast.Messages);

        _toast.Clear();
        Assert.Empty(_toast.Messages);
        Assert.Equal(2, mudancas);

        // O relogio do que foi limpo nao dispara depois.
        _relogio.Advance(TimeSpan.FromMinutes(1));
        Assert.Equal(2, mudancas);
    }

    [Fact]
    public void Show_com_acao_dura_mais_e_guarda_a_acao()
    {
        var acao = new RvmToastAction("Desfazer", () => Task.CompletedTask);

        _toast.Show("Talhao excluido.", acao, RvmToastSeverity.Success, "Talhao 12");
        _toast.Show("Lote exportado.", acao, duration: TimeSpan.FromSeconds(2));

        var mensagens = _toast.Messages;
        Assert.Same(acao, mensagens[0].Action);
        Assert.Equal(RvmToastService.ActionDuration, mensagens[0].Duration);
        Assert.Equal(RvmToastSeverity.Success, mensagens[0].Severity);
        Assert.Equal(TimeSpan.FromSeconds(2), mensagens[1].Duration);
        Assert.Equal(RvmToastSeverity.Info, mensagens[1].Severity);
    }

    [Fact]
    public void Validacoes_recusam_texto_vazio_duracao_negativa_e_acao_incompleta()
    {
        Assert.Throws<ArgumentException>(() => _toast.Show("  "));
        Assert.Throws<ArgumentOutOfRangeException>(() => _toast.Show("Oi.", duration: TimeSpan.FromSeconds(-1)));
        Assert.Throws<ArgumentNullException>(() => _toast.Show("Oi.", (RvmToastAction)null!));
        Assert.Throws<ArgumentException>(() => _toast.Show("Oi.", new RvmToastAction(" ", () => Task.CompletedTask)));
        Assert.Throws<ArgumentNullException>(() => _toast.Show("Oi.", new RvmToastAction("Abrir", null!)));
        Assert.Throws<ArgumentException>(() => _toast.Show("", new RvmToastAction("Abrir", () => Task.CompletedTask)));
        Assert.Throws<ArgumentNullException>(() => new RvmToastService(null!));
        Assert.Empty(_toast.Messages);
    }

    [Fact]
    public void Implementacao_sem_a_sobrecarga_de_acao_cai_no_Show_simples()
    {
        IRvmToast falso = new ToastSemAcao();

        falso.Show("Oi.", new RvmToastAction("Abrir", () => Task.CompletedTask), RvmToastSeverity.Warning);

        Assert.Equal(("Oi.", RvmToastSeverity.Warning), ((ToastSemAcao)falso).Ultimo);
    }

    [Fact]
    public void Dispose_para_os_relogios_e_o_construtor_padrao_usa_o_relogio_do_sistema()
    {
        var servico = new RvmToastService(_relogio);
        servico.Show("Oi.", duration: TimeSpan.FromSeconds(1));
        servico.Dispose();
        _relogio.Advance(TimeSpan.FromMinutes(1));
        Assert.Single(servico.Messages);

        using var doSistema = new RvmToastService();
        doSistema.Show("Oi.", duration: TimeSpan.Zero);
        Assert.Single(doSistema.Messages);
    }

    // --- Provider ---

    [Fact]
    public void Regiao_tem_nome_e_as_duas_regioes_vivas_existem_vazias()
    {
        var cortado = Render<RvmToastProvider>(p => p
            .Add(x => x.RegionLabel, "Notificacoes")
            .Add(x => x.Class, "minha"));

        var regiao = cortado.Find("section");
        Assert.Equal("Notificacoes", regiao.GetAttribute("aria-label"));
        Assert.Equal("rvm-toasts minha", regiao.GetAttribute("class"));
        Assert.NotNull(cortado.Find("[role=alert]"));
        Assert.NotNull(cortado.Find("[role=status]"));
        Assert.Empty(cortado.FindAll(".rvm-toast"));
    }

    [Fact]
    public void Padroes_dos_textos_sao_os_do_RVM_UI()
    {
        var cortado = Render<RvmToastProvider>();
        cortado.InvokeAsync(() => _toast.Show("Oi."));

        Assert.Equal("Avisos", cortado.Find("section").GetAttribute("aria-label"));
        Assert.Equal("Fechar aviso", cortado.WaitForElement("button.rvm-fechar").GetAttribute("aria-label"));
    }

    [Fact]
    public void Erro_e_atencao_vao_para_alert_e_info_e_sucesso_para_status()
    {
        var cortado = Render<RvmToastProvider>();

        cortado.InvokeAsync(() =>
        {
            _toast.Info("Cotacao atualizada.");
            _toast.Success("Talhao salvo.");
            _toast.Warning("Chuva forte amanha.");
            _toast.Error("Nao conseguimos salvar.", "Falha no envio");
        });

        cortado.WaitForAssertion(() => Assert.Equal(4, cortado.FindAll(".rvm-toast").Count));
        Assert.Equal(["Chuva forte amanha.", "Nao conseguimos salvar."], cortado.FindAll("[role=alert] .rvm-texto").Select(t => t.TextContent));
        Assert.Equal(["Cotacao atualizada.", "Talhao salvo."], cortado.FindAll("[role=status] .rvm-texto").Select(t => t.TextContent));
        Assert.Equal(
            ["rvm-toast rvm-warning", "rvm-toast rvm-error", "rvm-toast rvm-info", "rvm-toast rvm-success"],
            cortado.FindAll(".rvm-toast").Select(t => t.GetAttribute("class")));
        Assert.Equal("Falha no envio", cortado.Find(".rvm-error .rvm-titulo").TextContent);
        Assert.Empty(cortado.FindAll(".rvm-info .rvm-titulo"));
        Assert.Equal(4, cortado.FindAll(".rvm-toast > .rvm-icone[aria-hidden=true] svg").Count);
        Assert.Empty(cortado.FindAll("button.rvm-acao"));
    }

    [Theory]
    [InlineData(RvmToastSeverity.Info, RvmIconName.InfoCircle)]
    [InlineData(RvmToastSeverity.Success, RvmIconName.CircleCheck)]
    [InlineData(RvmToastSeverity.Warning, RvmIconName.AlertTriangle)]
    [InlineData(RvmToastSeverity.Error, RvmIconName.AlertCircle)]
    public void Cada_gravidade_tem_o_seu_icone(RvmToastSeverity gravidade, RvmIconName icone)
    {
        var cortado = Render<RvmToastProvider>();
        cortado.InvokeAsync(() => _toast.Show("Oi.", gravidade));
        var esperado = Render<RvmIcon>(p => p.Add(x => x.Name, icone)).Markup;

        cortado.WaitForAssertion(() => cortado.Find(".rvm-toast > .rvm-icone").InnerHtml.MarkupMatches(esperado));
    }

    [Fact]
    public void Fechar_tem_nome_e_tira_o_aviso()
    {
        var cortado = Render<RvmToastProvider>(p => p.Add(x => x.CloseLabel, "Dispensar"));
        cortado.InvokeAsync(() => _toast.Show("Oi.", duration: TimeSpan.Zero));

        var fechar = cortado.WaitForElement("button.rvm-fechar");
        Assert.Equal("Dispensar", fechar.GetAttribute("aria-label"));
        fechar.Click();

        cortado.WaitForAssertion(() => Assert.Empty(cortado.FindAll(".rvm-toast")));
        Assert.Empty(_toast.Messages);
    }

    [Fact]
    public void Acao_roda_e_fecha_o_aviso()
    {
        var desfeito = false;
        var cortado = Render<RvmToastProvider>();
        cortado.InvokeAsync(() => _toast.Show("Talhao excluido.",
            new RvmToastAction("Desfazer", () => { desfeito = true; return Task.CompletedTask; })));

        var botao = cortado.WaitForElement("button.rvm-acao");
        Assert.Equal("Desfazer", botao.TextContent);
        botao.Click();

        Assert.True(desfeito);
        cortado.WaitForAssertion(() => Assert.Empty(cortado.FindAll(".rvm-toast")));
    }

    [Fact]
    public async Task Acao_que_falha_fecha_o_aviso_mesmo_assim()
    {
        var cortado = Render<RvmToastProvider>();
        await cortado.InvokeAsync(() => _toast.Show("Exportar.",
            new RvmToastAction("Abrir", () => throw new InvalidOperationException("falhou"))));

        await Assert.ThrowsAsync<InvalidOperationException>(() => cortado.Find("button.rvm-acao").ClickAsync(new MouseEventArgs()));

        Assert.Empty(_toast.Messages);
    }

    [Fact]
    public void Relogio_fora_do_circuito_tira_o_aviso_da_tela()
    {
        var cortado = Render<RvmToastProvider>();
        cortado.InvokeAsync(() => _toast.Show("Rapido.", duration: TimeSpan.FromSeconds(5)));
        cortado.WaitForAssertion(() => Assert.Single(cortado.FindAll(".rvm-toast")));

        _relogio.Advance(TimeSpan.FromSeconds(5));

        cortado.WaitForAssertion(() => Assert.Empty(cortado.FindAll(".rvm-toast")));
    }

    [Fact]
    public async Task Mouse_ou_foco_seguram_o_aviso_e_so_soltar_os_dois_recomeca_o_tempo()
    {
        var cortado = Render<RvmToastProvider>();
        await cortado.InvokeAsync(() => _toast.Show("Leia com calma.", duration: TimeSpan.FromSeconds(5)));

        // Eventos AGUARDADOS: a versao sincrona volta antes do handler rodar.
        await cortado.WaitForElement(".rvm-toast").MouseEnterAsync(new MouseEventArgs());
        await cortado.Find(".rvm-toast").FocusInAsync(new FocusEventArgs());
        await cortado.Find(".rvm-toast").MouseLeaveAsync(new MouseEventArgs());
        _relogio.Advance(TimeSpan.FromMinutes(1));
        Assert.Single(_toast.Messages); // O foco ainda esta dentro.

        await cortado.Find(".rvm-toast").FocusOutAsync(new FocusEventArgs());
        _relogio.Advance(TimeSpan.FromSeconds(4));
        Assert.Single(_toast.Messages);
        _relogio.Advance(TimeSpan.FromSeconds(1));
        cortado.WaitForAssertion(() => Assert.Empty(cortado.FindAll(".rvm-toast")));
    }

    [Fact]
    public void Dispose_desinscreve_do_Changed()
    {
        var cortado = Render<RvmToastProvider>();
        var antes = cortado.RenderCount;

        cortado.Instance.Dispose();
        cortado.InvokeAsync(() => _toast.Show("Depois."));

        Assert.Equal(antes, cortado.RenderCount);
    }

    private sealed class ToastSemAcao : IRvmToast
    {
        public (string Texto, RvmToastSeverity Gravidade) Ultimo { get; private set; }

        public IReadOnlyList<RvmToastMessage> Messages => [];

        public event Action? Changed { add { } remove { } }

        public Guid Show(string text, RvmToastSeverity severity = RvmToastSeverity.Info, string? title = null, TimeSpan? duration = null)
        {
            Ultimo = (text, severity);
            return Guid.NewGuid();
        }

        public void Dismiss(Guid id) { }

        public void Clear() { }

        public void Pause(Guid id) { }

        public void Resume(Guid id) { }
    }

    [Fact]
    public void Duracao_acima_do_relogio_e_recusada_antes_de_entrar_na_fila()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _toast.Show("Safra fechada", duration: TimeSpan.FromDays(60)));

        Assert.Empty(_toast.Messages);
    }

    [Fact]
    public void Regioes_vivas_anunciam_so_o_aviso_que_entra()
    {
        var cortado = Render<RvmToastProvider>();

        Assert.All(cortado.FindAll(".rvm-pilha"), pilha =>
        {
            Assert.Equal("false", pilha.GetAttribute("aria-atomic"));
            Assert.Equal("additions", pilha.GetAttribute("aria-relevant"));
        });
    }
}
