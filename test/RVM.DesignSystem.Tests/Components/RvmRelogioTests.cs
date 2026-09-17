using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using RVM.DesignSystem.Components.TimePicker;

namespace RVM.DesignSystem.Tests.Components;

public class RvmTimeClockTests : BunitContext
{
    public RvmTimeClockTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<RvmTimeClock> Relogio(Action<ComponentParameterCollectionBuilder<RvmTimeClock>>? extra = null)
        => Render<RvmTimeClock>(p => extra?.Invoke(p));

    private static IEnumerable<string> Textos(IRenderedComponent<RvmTimeClock> cortado)
        => cortado.FindAll("[role=option]").Select(o => o.TextContent.Trim());

    [Fact]
    public void Horas_em_24h_tem_dois_aneis_e_a_opcao_ativa_no_leitor_de_tela()
    {
        var cortado = Relogio(p => p.Add(x => x.Value, new TimeOnly(15, 1)));

        var mostrador = cortado.Find("[role=listbox]");
        Assert.Equal("Horas", mostrador.GetAttribute("aria-label"));
        Assert.Equal("0", mostrador.GetAttribute("tabindex"));
        Assert.Equal(24, cortado.FindAll("[role=option]").Count);
        Assert.Equal(12, cortado.FindAll(".rvm-dentro").Count);
        Assert.Equal(["13", "14", "15"], cortado.FindAll(".rvm-dentro").Skip(1).Take(3).Select(o => o.TextContent.Trim()));

        var escolhida = cortado.Find("[aria-selected=true]");
        Assert.Equal("15", escolhida.TextContent.Trim());
        Assert.Equal("15 horas", escolhida.GetAttribute("aria-label"));
        Assert.Equal(escolhida.Id, mostrador.GetAttribute("aria-activedescendant"));
        Assert.Equal("Horas: 15", cortado.FindAll(".rvm-segmento")[0].GetAttribute("aria-label"));
        Assert.Equal("true", cortado.FindAll(".rvm-segmento")[0].GetAttribute("aria-pressed"));
        Assert.Empty(cortado.FindAll(".rvm-periodo"));
    }

    [Fact]
    public void Posicoes_saem_em_px_invariantes_mesmo_em_pt_BR()
    {
        var antes = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("pt-BR");
        try
        {
            var cortado = Relogio(p => p.Add(x => x.Value, new TimeOnly(3, 0)).Add(x => x.Use24Hours, false));

            // 3 horas: 108 px a direita do centro (130), menos metade dos 40 px da opcao.
            var tres = cortado.FindAll("[role=option]").Single(o => o.TextContent.Trim() == "3");
            Assert.Equal("left: 218px; top: 110px", tres.GetAttribute("style"));
            Assert.Equal("height: 108px; transform: rotate(90deg)", cortado.Find(".rvm-ponteiro").GetAttribute("style"));
        }
        finally
        {
            CultureInfo.CurrentCulture = antes;
        }
    }

    [Fact]
    public void Doze_horas_mostra_1_a_12_e_troca_o_periodo()
    {
        TimeOnly? valor = new TimeOnly(3, 1);
        var cortado = Relogio(p => p
            .Add(x => x.Value, valor)
            .Add(x => x.Use24Hours, false)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => valor = v)));

        Assert.Equal(["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"], Textos(cortado));
        Assert.Equal("03", cortado.FindAll(".rvm-segmento")[0].TextContent);
        Assert.Equal("true", cortado.FindAll(".rvm-periodo")[0].GetAttribute("aria-pressed"));

        cortado.FindAll(".rvm-periodo")[1].Click();

        Assert.Equal(new TimeOnly(15, 1), valor);
        Assert.Equal("true", cortado.FindAll(".rvm-periodo")[1].GetAttribute("aria-pressed"));
        Assert.Equal("3 horas", cortado.Find("[aria-selected=true]").GetAttribute("aria-label"));
    }

    [Fact]
    public void Sem_valor_mostra_tracos_e_a_primeira_seta_acende_o_meio_dia()
    {
        TimeOnly? valor = null;
        var cortado = Relogio(p => p.Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => valor = v)));

        Assert.Equal("--", cortado.FindAll(".rvm-segmento")[0].TextContent);
        Assert.Equal("12", cortado.Find("[aria-selected=true]").TextContent.Trim());

        cortado.Find("[role=listbox]").KeyDown(key: "ArrowUp");
        Assert.Equal(new TimeOnly(12, 0), valor);
    }

    [Fact]
    public void Teclado_anda_nas_horas_passa_aos_minutos_e_confirma()
    {
        TimeOnly? valor = new TimeOnly(23, 58);
        var visao = RvmTimeClockView.Hours;
        var confirmou = 0;
        var cortado = Relogio(p => p
            .Add(x => x.Value, valor)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => valor = v))
            .Add(x => x.ViewChanged, EventCallback.Factory.Create<RvmTimeClockView>(this, v => visao = v))
            .Add(x => x.OnConfirm, EventCallback.Factory.Create(this, () => confirmou++)));

        var mostrador = cortado.Find("[role=listbox]");
        mostrador.KeyDown(key: "ArrowUp");
        Assert.Equal(new TimeOnly(0, 58), valor);
        cortado.Find("[role=listbox]").KeyDown(key: "ArrowLeft");
        Assert.Equal(new TimeOnly(23, 58), valor);
        cortado.Find("[role=listbox]").KeyDown(key: "Home");
        Assert.Equal(new TimeOnly(0, 58), valor);
        cortado.Find("[role=listbox]").KeyDown(key: "End");
        Assert.Equal(new TimeOnly(23, 58), valor);
        cortado.Find("[role=listbox]").KeyDown(key: "x");
        Assert.Equal(new TimeOnly(23, 58), valor);

        cortado.Find("[role=listbox]").KeyDown(key: "Enter");
        Assert.Equal(RvmTimeClockView.Minutes, visao);
        Assert.Equal("Minutos", cortado.Find("[role=listbox]").GetAttribute("aria-label"));
        Assert.Equal(60, cortado.FindAll("[role=option]").Count);
        Assert.Contains("rvm-sem-numero", cortado.FindAll("[role=option]")[7].ClassName);
        Assert.DoesNotContain("rvm-sem-numero", cortado.FindAll("[role=option]")[58].ClassName);

        cortado.Find("[role=listbox]").KeyDown(key: "ArrowRight");
        cortado.Find("[role=listbox]").KeyDown(key: "ArrowRight");
        Assert.Equal(new TimeOnly(23, 0), valor);
        cortado.Find("[role=listbox]").KeyDown(key: "ArrowDown");
        Assert.Equal(new TimeOnly(23, 59), valor);

        cortado.Find("[role=listbox]").KeyDown(key: " ");
        Assert.Equal(1, confirmou);
    }

    [Fact]
    public void Segmentos_do_topo_trocam_o_mostrador_e_render_do_pai_nao_desfaz()
    {
        var cortado = Relogio(p => p.Add(x => x.Value, new TimeOnly(8, 20)).Add(x => x.Step, 5));

        cortado.FindAll(".rvm-segmento")[1].Click();
        Assert.Equal("Minutos", cortado.Find("[role=listbox]").GetAttribute("aria-label"));
        Assert.Equal(12, cortado.FindAll("[role=option]").Count);

        cortado.Render(p => p.Add(x => x.Title, "Outro titulo"));
        Assert.Equal("Minutos", cortado.Find("[role=listbox]").GetAttribute("aria-label"));

        cortado.FindAll(".rvm-segmento")[0].Click();
        Assert.Equal("Horas", cortado.Find("[role=listbox]").GetAttribute("aria-label"));

        cortado.Render(p => p.Add(x => x.View, RvmTimeClockView.Minutes));
        Assert.Equal("Minutos", cortado.Find("[role=listbox]").GetAttribute("aria-label"));
    }

    [Fact]
    public void Clicar_e_arrastar_escolhe_pelo_angulo_e_o_anel()
    {
        TimeOnly? valor = new TimeOnly(9, 10);
        var cortado = Relogio(p => p
            .Add(x => x.Value, valor)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => valor = v)));

        // Anel de fora, a direita do centro: 3 horas.
        cortado.Find("[role=listbox]").PointerDown(new PointerEventArgs { OffsetX = 238, OffsetY = 130, Buttons = 1 });
        Assert.Equal(new TimeOnly(3, 10), valor);

        // Arrastando para o anel de dentro, embaixo: 18 horas.
        cortado.Find("[role=listbox]").PointerMove(new PointerEventArgs { OffsetX = 130, OffsetY = 202, Buttons = 1 });
        Assert.Equal(new TimeOnly(18, 10), valor);

        // Mover sem botao apertado nao muda nada.
        cortado.Find("[role=listbox]").PointerMove(new PointerEventArgs { OffsetX = 130, OffsetY = 22, Buttons = 0 });
        Assert.Equal(new TimeOnly(18, 10), valor);

        // Soltar grava e leva aos minutos.
        cortado.Find("[role=listbox]").PointerUp(new PointerEventArgs { OffsetX = 130, OffsetY = 58 });
        Assert.Equal(new TimeOnly(0, 10), valor);
        Assert.Equal("Minutos", cortado.Find("[role=listbox]").GetAttribute("aria-label"));

        // Minutos: embaixo e 30; soltar nos minutos continua neles.
        cortado.Find("[role=listbox]").PointerDown(new PointerEventArgs { OffsetX = 130, OffsetY = 238, Buttons = 1 });
        cortado.Find("[role=listbox]").PointerUp(new PointerEventArgs { OffsetX = 130, OffsetY = 238 });
        Assert.Equal(new TimeOnly(0, 30), valor);
        Assert.Equal("Minutos", cortado.Find("[role=listbox]").GetAttribute("aria-label"));

        // Soltar sem ter apertado dentro (arrastou de fora) e sair do mostrador sao ignorados.
        cortado.Find("[role=listbox]").PointerLeave();
        cortado.Find("[role=listbox]").PointerUp(new PointerEventArgs { OffsetX = 238, OffsetY = 130 });
        Assert.Equal(new TimeOnly(0, 30), valor);
    }

    [Fact]
    public void Doze_horas_pelo_ponteiro_respeita_o_periodo()
    {
        TimeOnly? valor = new TimeOnly(14, 0);
        var cortado = Relogio(p => p
            .Add(x => x.Value, valor)
            .Add(x => x.Use24Hours, false)
            .Add(x => x.Step, 15)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => valor = v)));

        cortado.Find("[role=listbox]").PointerDown(new PointerEventArgs { OffsetX = 22, OffsetY = 130, Buttons = 1 });
        Assert.Equal(new TimeOnly(21, 0), valor);

        cortado.FindAll(".rvm-segmento")[1].Click();
        // 20 minutos arredonda para o passo de 15.
        cortado.Find("[role=listbox]").PointerDown(new PointerEventArgs { OffsetX = 224, OffsetY = 184, Buttons = 1 });
        Assert.Equal(new TimeOnly(21, 15), valor);
    }

    [Fact]
    public void Limites_desabilitam_horas_minutos_e_periodo()
    {
        TimeOnly? valor = new TimeOnly(9, 30);
        var cortado = Relogio(p => p
            .Add(x => x.Value, valor)
            .Add(x => x.Use24Hours, false)
            .Add(x => x.Min, new TimeOnly(8, 15))
            .Add(x => x.Max, new TimeOnly(11, 45))
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => valor = v)));

        Assert.True(cortado.FindAll(".rvm-periodo")[1].HasAttribute("disabled"));
        var desabilitadas = cortado.FindAll("[role=option][aria-disabled=true]").Select(o => o.TextContent.Trim());
        Assert.Equal(["1", "2", "3", "4", "5", "6", "7", "12"], desabilitadas);

        cortado.FindAll(".rvm-periodo")[1].Click();
        Assert.Equal(new TimeOnly(9, 30), valor);

        // Clicar numa hora desabilitada (6, embaixo) nao muda nada.
        cortado.Find("[role=listbox]").PointerDown(new PointerEventArgs { OffsetX = 130, OffsetY = 238, Buttons = 1 });
        Assert.Equal(new TimeOnly(9, 30), valor);

        // End vai para a ultima hora valida; o minuto 30 nao cabe as 11:45? cabe, e fica.
        cortado.Find("[role=listbox]").KeyDown(key: "End");
        Assert.Equal(new TimeOnly(11, 30), valor);

        // Home vai para 8 e o minuto vai para o escolhivel mais perto (30 cabe).
        cortado.Find("[role=listbox]").KeyDown(key: "Home");
        Assert.Equal(new TimeOnly(8, 30), valor);

        cortado.FindAll(".rvm-segmento")[1].Click();
        Assert.Equal("true", cortado.FindAll("[role=option]")[10].GetAttribute("aria-disabled"));
        cortado.Find("[role=listbox]").KeyDown(key: "Home");
        Assert.Equal(new TimeOnly(8, 15), valor);
    }

    [Fact]
    public void Hora_nova_ajusta_o_minuto_que_nao_cabe()
    {
        TimeOnly? valor = new TimeOnly(9, 50);
        var cortado = Relogio(p => p
            .Add(x => x.Value, valor)
            .Add(x => x.Max, new TimeOnly(10, 20))
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => valor = v)));

        cortado.Find("[role=listbox]").KeyDown(key: "ArrowUp");

        Assert.Equal(new TimeOnly(10, 20), valor);
    }

    [Fact]
    public void Sem_valor_com_minimo_comeca_no_primeiro_horario_valido()
    {
        var cortado = Relogio(p => p.Add(x => x.Min, new TimeOnly(13, 7)).Add(x => x.Step, 5));

        Assert.Equal("13", cortado.Find("[aria-selected=true]").TextContent.Trim());
        cortado.FindAll(".rvm-segmento")[1].Click();
        Assert.Equal("10", cortado.Find("[aria-selected=true]").TextContent.Trim());
    }

    [Fact]
    public void Janela_que_cruza_a_meia_noite_libera_a_noite_e_a_madrugada()
    {
        TimeOnly? valor = null;
        var cortado = Relogio(p => p
            .Add(x => x.Min, new TimeOnly(22, 0))
            .Add(x => x.Max, new TimeOnly(6, 0))
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => valor = v)));

        var liberadas = cortado.FindAll("[role=option]:not([aria-disabled])").Select(o => o.TextContent.Trim()).Order().ToArray();
        Assert.Equal(["00", "1", "2", "22", "23", "3", "4", "5", "6"], liberadas);
        Assert.Equal("22", cortado.Find("[aria-selected=true]").TextContent.Trim());

        cortado.Find("[role=listbox]").KeyDown(key: "ArrowUp");
        cortado.Find("[role=listbox]").KeyDown(key: "ArrowUp");
        cortado.Find("[role=listbox]").KeyDown(key: "ArrowUp");
        Assert.Equal(new TimeOnly(0, 0), valor);
        cortado.Find("[role=listbox]").KeyDown(key: "End");
        Assert.Equal(new TimeOnly(23, 0), valor);
    }

    [Fact]
    public void Valor_fora_do_passo_continua_ativo_e_as_setas_voltam_ao_passo()
    {
        TimeOnly? valor = new TimeOnly(9, 47);
        var cortado = Relogio(p => p
            .Add(x => x.Value, valor)
            .Add(x => x.Step, 15)
            .Add(x => x.View, RvmTimeClockView.Minutes)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => valor = v)));

        Assert.Equal(["00", "15", "30", "45", "47"], Textos(cortado));
        var ativa = cortado.Find("[aria-selected=true]");
        Assert.Equal("47", ativa.TextContent.Trim());
        Assert.Equal(ativa.Id, cortado.Find("[role=listbox]").GetAttribute("aria-activedescendant"));

        cortado.Find("[role=listbox]").KeyDown(key: "ArrowDown");
        Assert.Equal(new TimeOnly(9, 45), valor);
        Assert.Equal(["00", "15", "30", "45"], Textos(cortado));
    }

    [Fact]
    public void Acoes_e_atributos_extras()
    {
        var cortado = Relogio(p => p
            .Add(x => x.Actions, (RenderFragment)(b => b.AddMarkupContent(0, "<button>OK</button>")))
            .AddUnmatched("class", "minha")
            .AddUnmatched("data-teste", "x"));

        Assert.Equal("rvm-relogio minha", cortado.Find("[data-teste=x]").ClassName);
        Assert.Equal("OK", cortado.Find(".rvm-acoes-relogio button").TextContent);
    }
}

public class RvmTimePickerRelogioTests : BunitContext
{
    private sealed class Agenda
    {
        public TimeOnly? Visita { get; set; }
    }

    public RvmTimePickerRelogioTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<RvmTimePicker> Campo(Agenda agenda, Action<ComponentParameterCollectionBuilder<RvmTimePicker>>? extra = null)
        => Render<RvmTimePicker>(p =>
        {
            p.Add(x => x.Mode, RvmTimePickerMode.Clock)
             .Add(x => x.Label, "Visita")
             .Add(x => x.Value, agenda.Visita)
             .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => agenda.Visita = v))
             .Add(x => x.ValueExpression, () => agenda.Visita);
            extra?.Invoke(p);
        });

    [Fact]
    public void Campo_e_um_botao_que_abre_o_relogio_num_dialogo()
    {
        var cortado = Campo(new Agenda(), p => p.AddUnmatched("data-teste", "x"));

        var botao = cortado.Find("button.rvm-entrada");
        Assert.Equal("dialog", botao.GetAttribute("aria-haspopup"));
        Assert.Equal("x", botao.GetAttribute("data-teste"));
        Assert.Equal("hh:mm", cortado.Find(".rvm-valor").TextContent);
        Assert.Empty(cortado.FindAll("[role=combobox]"));

        botao.Click();

        var dialogo = cortado.Find("[role=dialog]");
        Assert.Equal("Escolher Visita", dialogo.GetAttribute("aria-label"));
        Assert.NotNull(dialogo.QuerySelector("[role=listbox]"));
        Assert.Equal(["Cancelar", "OK"], dialogo.QuerySelectorAll(".rvm-acoes-relogio button").Select(b => b.TextContent.Trim()));
    }

    [Fact]
    public void Mexer_no_relogio_nao_grava_e_cancelar_descarta()
    {
        var agenda = new Agenda { Visita = new TimeOnly(8, 0) };
        var cortado = Campo(agenda);

        cortado.Find("button.rvm-entrada").Click();
        cortado.Find("[role=listbox]").KeyDown(key: "ArrowUp");
        Assert.Equal("09", cortado.FindAll(".rvm-segmento")[0].TextContent);
        Assert.Equal(new TimeOnly(8, 0), agenda.Visita);

        cortado.FindAll(".rvm-acoes-relogio button")[0].Click();

        Assert.Empty(cortado.FindAll("[role=dialog]"));
        Assert.Equal(new TimeOnly(8, 0), agenda.Visita);
        Assert.Equal("08:00", cortado.Find(".rvm-valor").TextContent);

        // Reabrir parte do valor gravado, nao do rascunho descartado.
        cortado.Find("button.rvm-entrada").Click();
        Assert.Equal("08", cortado.FindAll(".rvm-segmento")[0].TextContent);
    }

    [Fact]
    public void Ok_grava_e_fecha_e_enter_nos_minutos_tambem()
    {
        var agenda = new Agenda();
        var cortado = Campo(agenda, p => p.Add(x => x.Name, "visita").Add(x => x.Use24Hours, false));

        cortado.Find("button.rvm-entrada").Click();
        cortado.Find("[role=listbox]").KeyDown(key: "ArrowUp");
        cortado.Find("[role=listbox]").KeyDown(key: "ArrowUp");
        cortado.FindAll(".rvm-acoes-relogio button")[1].Click();

        Assert.Equal(new TimeOnly(13, 0), agenda.Visita);
        Assert.Empty(cortado.FindAll("[role=dialog]"));
        Assert.Equal("1:00 PM", cortado.Find(".rvm-valor").TextContent);
        Assert.Equal("13:00", cortado.Find("input[type=hidden][name=visita]").GetAttribute("value"));

        cortado.Find("button.rvm-entrada").Click();
        cortado.Find("[role=listbox]").KeyDown(key: "Enter");
        cortado.Find("[role=listbox]").KeyDown(key: "ArrowUp");
        cortado.Find("[role=listbox]").KeyDown(key: "Enter");

        Assert.Equal(new TimeOnly(13, 1), agenda.Visita);
        Assert.Empty(cortado.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Ok_sem_mexer_grava_o_que_o_relogio_mostra()
    {
        var agenda = new Agenda();
        var cortado = Campo(agenda, p => p.Add(x => x.Min, new TimeOnly(14, 0)).Add(x => x.Step, 10));

        cortado.Find("button.rvm-entrada").Click();
        cortado.FindAll(".rvm-acoes-relogio button")[1].Click();

        Assert.Equal(new TimeOnly(14, 0), agenda.Visita);
    }

    [Fact]
    public void Ok_com_o_mesmo_valor_so_fecha()
    {
        var avisos = 0;
        var cortado = Render<RvmTimePicker>(p => p
            .Add(x => x.Mode, RvmTimePickerMode.Clock)
            .Add(x => x.Label, "Visita")
            .Add(x => x.Value, new TimeOnly(7, 0))
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, _ => avisos++)));

        cortado.Find("button.rvm-entrada").Click();
        cortado.FindAll(".rvm-acoes-relogio button")[1].Click();

        Assert.Equal(0, avisos);
        Assert.Empty(cortado.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Dentro_do_editform_mostra_a_validacao_e_esc_fecha()
    {
        var agenda = new Agenda();
        var contexto = new EditContext(agenda);
        var mensagens = new ValidationMessageStore(contexto);
        var cortado = Campo(agenda, p => p.AddCascadingValue(contexto).Add(x => x.Required, true));

        mensagens.Add(contexto.Field(nameof(Agenda.Visita)), "Escolha o horario da visita.");
        cortado.InvokeAsync(contexto.NotifyValidationStateChanged);

        Assert.Equal("true", cortado.Find("button.rvm-entrada").GetAttribute("aria-invalid"));
        Assert.Equal("Escolha o horario da visita.", cortado.Find(".rvm-apoio").TextContent);

        cortado.Find("button.rvm-entrada").Click();
        cortado.Find("[role=dialog]").KeyDown(key: "Escape");
        Assert.Empty(cortado.FindAll("[role=dialog]"));
    }
}
