using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RVM.DesignSystem.Components.Dialog;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>RvmModal e RvmConfirmModal, do contrato com o RVM.UI (DSGN-017).</summary>
public class RvmModaisTests : BunitContext
{
    public RvmModaisTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    // --- RvmModal ---

    private IRenderedComponent<RvmModal> Modal(bool aberto = true, Action<bool>? aoMudar = null,
        Action<ComponentParameterCollectionBuilder<RvmModal>>? extra = null)
        => Render<RvmModal>(p =>
        {
            p.Add(x => x.Open, aberto)
             .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => aoMudar?.Invoke(v)))
             .Add(x => x.Title, "Detalhes da safra")
             .AddChildContent("Soja em 120 ha.");
            extra?.Invoke(p);
        });

    [Fact]
    public void Modal_fechado_nao_existe_no_dom()
    {
        Assert.Empty(Modal(aberto: false).FindAll("[role=dialog]"));
    }

    [Fact]
    public void Modal_aberto_e_dialogo_modal_nomeado_pelo_titulo()
    {
        var cortado = Modal();

        var caixa = cortado.Find("[role=dialog]");
        Assert.Equal("true", caixa.GetAttribute("aria-modal"));
        Assert.Equal("-1", caixa.GetAttribute("tabindex"));
        var titulo = cortado.Find("h2.rvm-titulo");
        Assert.Equal("Detalhes da safra", titulo.TextContent);
        Assert.Equal(titulo.Id, caixa.GetAttribute("aria-labelledby"));
        Assert.Null(caixa.GetAttribute("aria-label"));
        Assert.Contains("Soja em 120 ha.", cortado.Find(".rvm-conteudo").TextContent);
    }

    [Fact]
    public void Modal_sem_titulo_e_nomeado_pelo_Label()
    {
        var cortado = Render<RvmModal>(p => p.Add(x => x.Open, true).Add(x => x.Label, "Foto do talhao").AddChildContent("..."));

        var caixa = cortado.Find("[role=dialog]");
        Assert.Equal("Foto do talhao", caixa.GetAttribute("aria-label"));
        Assert.Null(caixa.GetAttribute("aria-labelledby"));
        Assert.Empty(cortado.FindAll("h2"));
    }

    [Fact]
    public void Esc_fecha_o_modal_e_avisa()
    {
        bool? recebido = null;
        var cortado = Modal(aoMudar: v => recebido = v);

        cortado.Find("[role=dialog]").KeyDown(key: "Escape");

        Assert.False(recebido);
        Assert.Empty(cortado.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Esc_nao_fecha_com_CloseOnEscape_desligado()
    {
        bool? recebido = null;
        var cortado = Modal(aoMudar: v => recebido = v, extra: p => p.Add(x => x.CloseOnEscape, false));

        cortado.Find("[role=dialog]").KeyDown(key: "Escape");
        cortado.Find("[role=dialog]").KeyDown(key: "Enter");

        Assert.Null(recebido);
        Assert.NotEmpty(cortado.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Clique_no_fundo_fecha_salvo_com_CloseOnBackdrop_desligado()
    {
        bool? recebido = null;
        var fecha = Modal(aoMudar: v => recebido = v);
        fecha.Find(".rvm-fundo").Click();
        Assert.False(recebido);

        recebido = null;
        var naoFecha = Modal(aoMudar: v => recebido = v, extra: p => p.Add(x => x.CloseOnBackdrop, false));
        naoFecha.Find(".rvm-fundo").Click();
        Assert.Null(recebido);
        Assert.NotEmpty(naoFecha.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Estado_interno_so_segue_o_Open_quando_ele_muda()
    {
        var cortado = Modal();

        cortado.Find("[role=dialog]").KeyDown(key: "Escape");
        // O pai nao ligou o bind: re-renderiza com o mesmo Open=true, e o modal continua fechado.
        cortado.Render(p => p.Add(x => x.Open, true));
        Assert.Empty(cortado.FindAll("[role=dialog]"));

        cortado.Render(p => p.Add(x => x.Open, false));
        cortado.Render(p => p.Add(x => x.Open, true));
        Assert.NotEmpty(cortado.FindAll("[role=dialog]"));
        Assert.True(cortado.Instance.Open);
    }

    [Fact]
    public async Task CloseAsync_fecha_pelo_codigo()
    {
        bool? recebido = null;
        var cortado = Modal(aoMudar: v => recebido = v);

        await cortado.InvokeAsync(cortado.Instance.CloseAsync);

        Assert.False(recebido);
        Assert.Empty(cortado.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Ao_fechar_o_foco_vai_para_o_ReturnFocusTo()
    {
        var destino = new ElementReference("botao-abrir", new WebElementReferenceContext(JSInterop.JSRuntime));
        var cortado = Modal(extra: p => p.Add(x => x.ReturnFocusTo, destino));

        cortado.Render(p => p.Add(x => x.Open, false));

        Assert.Equal(destino.Id, Assert.IsType<ElementReference>(JSInterop.VerifyFocusAsyncInvoke().Arguments[0]).Id);
    }

    [Fact]
    public void Ao_fechar_sem_ReturnFocusTo_nao_foca_pelo_Blazor()
    {
        var cortado = Modal();

        cortado.Render(p => p.Add(x => x.Open, false));

        // Sem destino, quem devolve o foco e o modulo da sobreposicao (ao elemento que o tinha ao abrir).
        Assert.Empty(JSInterop.Invocations["Blazor._internal.domWrapper.focus"]);
    }

    [Fact]
    public void Class_e_atributos_vao_para_a_raiz_do_modal()
    {
        var cortado = Modal(extra: p => p.Add(x => x.Class, "minha").AddUnmatched("data-teste", "modal"));

        var raiz = cortado.Find(".rvm-modal");
        Assert.Contains("minha", raiz.ClassList);
        Assert.Equal("modal", raiz.GetAttribute("data-teste"));
        Assert.DoesNotContain("rvm-estreito", raiz.ClassList);
    }

    // --- RvmConfirmModal ---

    private IRenderedComponent<RvmConfirmModal> Confirmacao(List<string>? eventos = null,
        Action<ComponentParameterCollectionBuilder<RvmConfirmModal>>? extra = null)
        => Render<RvmConfirmModal>(p =>
        {
            p.Add(x => x.Open, true)
             .Add(x => x.Title, "Excluir o Talhao 12?")
             .Add(x => x.Description, "O historico de aplicacoes tambem sera apagado.")
             .Add(x => x.OnConfirm, () => eventos?.Add("confirmou"))
             .Add(x => x.OnCancel, () => eventos?.Add("desistiu"))
             .Add(x => x.OpenChanged, (bool v) => eventos?.Add($"open={v}"));
            extra?.Invoke(p);
        });

    [Fact]
    public void Confirmacao_e_alertdialog_nomeado_pela_pergunta_e_descrito_pela_explicacao()
    {
        var cortado = Confirmacao();

        var caixa = cortado.Find("[role=alertdialog]");
        Assert.Equal("true", caixa.GetAttribute("aria-modal"));
        var pergunta = cortado.Find("h2.rvm-pergunta");
        Assert.Equal("Excluir o Talhao 12?", pergunta.TextContent);
        Assert.Equal(pergunta.Id, caixa.GetAttribute("aria-labelledby"));
        var descricao = cortado.Find(".rvm-descricao");
        Assert.Equal("O historico de aplicacoes tambem sera apagado.", descricao.TextContent);
        Assert.Equal(descricao.Id, caixa.GetAttribute("aria-describedby"));
        Assert.Contains("rvm-estreito", cortado.Find(".rvm-modal").ClassList);
        Assert.Contains("rvm-confirmacao", cortado.Find(".rvm-modal").ClassList);
    }

    [Fact]
    public void Confirmacao_tem_textos_padrao_e_o_foco_abre_no_Cancelar()
    {
        var cortado = Render<RvmConfirmModal>(p => p.Add(x => x.Open, true).Add(x => x.Title, "Arquivar a safra?"));

        var botoes = cortado.FindAll(".rvm-acoes button");
        Assert.Equal(["Cancelar", "Confirmar"], botoes.Select(b => b.TextContent.Trim()));
        Assert.Equal("true", botoes[0].GetAttribute("data-rvm-foco-inicial"));
        Assert.Null(botoes[1].GetAttribute("data-rvm-foco-inicial"));
        Assert.Null(cortado.Find("[role=alertdialog]").GetAttribute("aria-describedby"));
        Assert.Contains("rvm-centro", cortado.Find(".rvm-corpo").ClassList);
        Assert.Contains("rvm-primary", cortado.Find(".rvm-circulo").ClassList);
    }

    [Fact]
    public void Color_pinta_o_icone_e_o_botao_de_confirmar()
    {
        var cortado = Confirmacao(extra: p => p
            .Add(x => x.Color, RvmColor.Danger)
            .Add(x => x.Icon, RvmIconName.Trash)
            .Add(x => x.ConfirmText, "Excluir"));

        Assert.Contains("rvm-error", cortado.Find(".rvm-circulo").ClassList);
        Assert.Equal("true", cortado.Find(".rvm-circulo").GetAttribute("aria-hidden"));
        var confirmar = cortado.FindAll(".rvm-acoes button")[1];
        Assert.Equal("Excluir", confirmar.TextContent.Trim());
        Assert.Contains("rvm-error", confirmar.ClassList);
        Assert.Contains("rvm-preenchido", confirmar.ClassList);
        Assert.DoesNotContain("rvm-error", cortado.FindAll(".rvm-acoes button")[0].ClassList);
    }

    [Fact]
    public void Confirmar_roda_a_acao_e_depois_fecha()
    {
        var eventos = new List<string>();
        var cortado = Confirmacao(eventos);

        cortado.FindAll(".rvm-acoes button")[1].Click();

        Assert.Equal(["confirmou", "open=False"], eventos);
        Assert.Empty(cortado.FindAll("[role=alertdialog]"));
    }

    [Fact]
    public void Cancelar_Esc_e_fundo_sao_a_mesma_desistencia()
    {
        var pelos = new[] { "botao", "esc", "fundo" };
        foreach (var caminho in pelos)
        {
            var eventos = new List<string>();
            var cortado = Confirmacao(eventos);

            switch (caminho)
            {
                case "botao": cortado.FindAll(".rvm-acoes button")[0].Click(); break;
                case "esc": cortado.Find("[role=alertdialog]").KeyDown(key: "Escape"); break;
                default: cortado.Find(".rvm-fundo").Click(); break;
            }

            Assert.Equal(["desistiu", "open=False"], eventos);
            Assert.Empty(cortado.FindAll("[role=alertdialog]"));
        }
    }

    [Fact]
    public void Fundo_nao_desiste_com_CloseOnBackdrop_desligado()
    {
        var eventos = new List<string>();
        var cortado = Confirmacao(eventos, p => p.Add(x => x.CloseOnBackdrop, false));

        cortado.Find(".rvm-fundo").Click();

        Assert.Empty(eventos);
        Assert.NotEmpty(cortado.FindAll("[role=alertdialog]"));
    }

    [Fact]
    public void Enquanto_confirma_os_botoes_travam_e_o_segundo_clique_nao_repete_a_acao()
    {
        var acao = new TaskCompletionSource();
        var confirmacoes = 0;
        var cortado = Render<RvmConfirmModal>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Title, "Excluir?")
            .Add(x => x.OnConfirm, async () => { confirmacoes++; await acao.Task; }));

        cortado.FindAll(".rvm-acoes button")[1].Click();
        var botoes = cortado.FindAll(".rvm-acoes button");
        Assert.True(botoes[0].HasAttribute("disabled"));
        Assert.True(botoes[1].HasAttribute("disabled"));
        cortado.Find("[role=alertdialog]").KeyDown(key: "Escape");
        cortado.Find(".rvm-fundo").Click();
        Assert.NotEmpty(cortado.FindAll("[role=alertdialog]"));

        cortado.InvokeAsync(acao.SetResult);

        cortado.WaitForAssertion(() => Assert.Empty(cortado.FindAll("[role=alertdialog]")));
        Assert.Equal(1, confirmacoes);
    }

    [Fact]
    public void Align_Left_e_corpo_livre_no_lugar_da_descricao()
    {
        var cortado = Confirmacao(extra: p => p
            .Add(x => x.Align, RvmDialogAlign.Left)
            .AddChildContent("<strong>3 aplicacoes</strong> serao apagadas."));

        Assert.Contains("rvm-esquerda", cortado.Find(".rvm-corpo").ClassList);
        var descricao = cortado.Find(".rvm-descricao");
        Assert.Equal("DIV", descricao.TagName);
        Assert.Contains("3 aplicacoes", descricao.TextContent);
        Assert.DoesNotContain("historico", cortado.Markup);
        Assert.Equal(descricao.Id, cortado.Find("[role=alertdialog]").GetAttribute("aria-describedby"));
    }

    [Fact]
    public void Modal_dentro_da_confirmacao_nao_herda_o_alertdialog()
    {
        var cortado = Confirmacao(extra: p => p.Add(x => x.ChildContent, (RenderFragment)(b =>
        {
            b.OpenComponent<RvmModal>(0);
            b.AddAttribute(1, nameof(RvmModal.Open), true);
            b.AddAttribute(2, nameof(RvmModal.Title), "Detalhes");
            b.CloseComponent();
        })));

        Assert.Single(cortado.FindAll("[role=alertdialog]"));
        var interno = cortado.Find("[role=alertdialog] [role=dialog]");
        Assert.Null(interno.GetAttribute("aria-describedby"));
        Assert.Equal(2, cortado.FindAll(".rvm-modal").Count);
        Assert.Single(cortado.FindAll(".rvm-estreito"));
    }

    [Fact]
    public void Class_e_atributos_da_confirmacao_vao_para_a_raiz()
    {
        var cortado = Confirmacao(extra: p => p.Add(x => x.Class, "minha").AddUnmatched("data-teste", "confirmar"));

        var raiz = cortado.Find(".rvm-modal");
        Assert.Contains("minha", raiz.ClassList);
        Assert.Equal("confirmar", raiz.GetAttribute("data-teste"));
    }

    [Fact]
    public void Confirmacao_ReturnFocusTo_chega_ao_modal()
    {
        var destino = new ElementReference("botao-excluir", new WebElementReferenceContext(JSInterop.JSRuntime));
        var cortado = Confirmacao(extra: p => p.Add(x => x.ReturnFocusTo, destino));

        cortado.FindAll(".rvm-acoes button")[0].Click();

        Assert.Equal(destino.Id, Assert.IsType<ElementReference>(JSInterop.VerifyFocusAsyncInvoke().Arguments[0]).Id);
    }
}
