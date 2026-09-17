using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using RVM.DesignSystem.Components.Calendar;
using RVM.DesignSystem.Components.DatePicker;
using RVM.DesignSystem.Components.TimePicker;

namespace RVM.DesignSystem.Tests.Components;

public class RvmCalendarTests : BunitContext
{
    private static readonly DateOnly Hoje = new(2021, 12, 14);

    public RvmCalendarTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<RvmCalendar> Calendario(Action<ComponentParameterCollectionBuilder<RvmCalendar>>? extra = null)
        => Render<RvmCalendar>(p =>
        {
            p.Add(x => x.Today, Hoje);
            extra?.Invoke(p);
        });

    private static IReadOnlyList<AngleSharp.Dom.IElement> Dias(IRenderedComponent<RvmCalendar> c) => c.FindAll("button.rvm-dia");

    private static readonly string[] Meses =
        ["janeiro", "fevereiro", "marco", "abril", "maio", "junho", "julho", "agosto", "setembro", "outubro", "novembro", "dezembro"];

    /// <summary>O dia em foco, lido do DOM: o mes do titulo e o unico dia com tabindex=0.</summary>
    private static string FocoDe(IRenderedComponent<RvmCalendar> c)
        => $"{c.Find("button.rvm-dia[tabindex='0']").TextContent.Trim()} {c.Find("h2.rvm-mes").TextContent}";

    private static string Esperado(int ano, int mes, int dia) => $"{dia} {Meses[mes - 1]} de {ano}";

    [Fact]
    public void Grade_do_mes_com_semana_nomeada_e_so_um_dia_na_tabulacao()
    {
        var cortado = Calendario();

        var grade = cortado.Find("table[role=grid]");
        Assert.Equal(cortado.Find("h2.rvm-mes").Id, grade.GetAttribute("aria-labelledby"));
        Assert.Equal("dezembro de 2021", cortado.Find("h2.rvm-mes").TextContent);
        Assert.Equal(["D", "S", "T", "Q", "Q", "S", "S"], cortado.FindAll("th").Select(t => t.TextContent));
        Assert.Equal("domingo", cortado.FindAll("th")[0].GetAttribute("abbr"));
        Assert.Equal(31, Dias(cortado).Count);
        Assert.Single(Dias(cortado), d => d.GetAttribute("tabindex") == "0");
        // 1 de dezembro de 2021 caiu numa quarta: tres celulas vazias antes.
        Assert.Equal(3, cortado.FindAll("tbody tr:first-child td.rvm-vazio").Count);
    }

    [Fact]
    public void Hoje_e_marcado_e_tem_nome_completo()
    {
        var cortado = Calendario();

        var hoje = Dias(cortado)[13];
        Assert.Equal("date", hoje.GetAttribute("aria-current"));
        Assert.Equal("terca-feira, 14 de dezembro de 2021", hoje.GetAttribute("aria-label"));
        Assert.Equal("0", hoje.GetAttribute("tabindex"));
    }

    [Fact]
    public void Clicar_escolhe_e_marca_a_celula()
    {
        DateOnly? escolhido = null;
        var cortado = Calendario(p => p.Add(x => x.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, v => escolhido = v)));

        Dias(cortado)[19].Click();

        Assert.Equal(new DateOnly(2021, 12, 20), escolhido);
        var celula = cortado.FindAll("td[role=gridcell]")[19];
        Assert.Equal("true", celula.GetAttribute("aria-selected"));
        Assert.Contains("rvm-escolhido", celula.GetAttribute("class"));
    }

    [Theory]
    [InlineData("ArrowRight", false, 2021, 12, 15)]
    [InlineData("ArrowLeft", false, 2021, 12, 13)]
    [InlineData("ArrowDown", false, 2021, 12, 21)]
    [InlineData("ArrowUp", false, 2021, 12, 7)]
    [InlineData("Home", false, 2021, 12, 12)]
    [InlineData("End", false, 2021, 12, 18)]
    [InlineData("PageDown", false, 2022, 1, 14)]
    [InlineData("PageUp", false, 2021, 11, 14)]
    [InlineData("PageUp", true, 2020, 12, 14)]
    [InlineData("x", false, 2021, 12, 14)]
    public void Teclado_move_o_foco_pelo_padrao_da_grade(string tecla, bool shift, int ano, int mes, int dia)
    {
        var cortado = Calendario();

        cortado.Find("table").KeyDown(new KeyboardEventArgs { Key = tecla, ShiftKey = shift });

        Assert.Equal(Esperado(ano, mes, dia), FocoDe(cortado));
    }

    [Fact]
    public void Botoes_de_mes_navegam_e_respeitam_min_e_max()
    {
        var cortado = Calendario(p => p
            .Add(x => x.Min, new DateOnly(2021, 12, 5))
            .Add(x => x.Max, new DateOnly(2022, 1, 20)));

        var anterior = cortado.Find("button[aria-label='Mes anterior']");
        Assert.True(anterior.HasAttribute("disabled"));

        cortado.Find("button[aria-label='Proximo mes']").Click();
        Assert.Equal("janeiro de 2022", cortado.Find("h2").TextContent);
        Assert.True(cortado.Find("button[aria-label='Proximo mes']").HasAttribute("disabled"));

        // Teclado tambem nao passa do limite: Page Down iria a 14 de fevereiro, para no Max.
        Assert.Equal(Esperado(2022, 1, 14), FocoDe(cortado));
        cortado.Find("table").KeyDown(key: "PageDown");
        Assert.Equal(Esperado(2022, 1, 20), FocoDe(cortado));
        cortado.Find("table").KeyDown(key: "ArrowUp");
        Assert.Equal(Esperado(2022, 1, 13), FocoDe(cortado));
    }

    [Fact]
    public void Dia_bloqueado_nao_escolhe_mas_continua_focavel()
    {
        var disparos = 0;
        var cortado = Calendario(p => p
            .Add(x => x.DateDisabled, d => d.DayOfWeek == DayOfWeek.Sunday)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, _ => disparos++)));

        var domingo = Dias(cortado)[11];
        Assert.Equal("true", domingo.GetAttribute("aria-disabled"));
        domingo.Click();

        Assert.Equal(0, disparos);
        Assert.Equal(Esperado(2021, 12, 12), FocoDe(cortado));
    }

    [Fact]
    public void Intervalo_em_dois_cliques_em_qualquer_ordem()
    {
        RvmDateRange? intervalo = null;
        var cortado = Calendario(p => p
            .Add(x => x.IsRange, true)
            .Add(x => x.RangeChanged, EventCallback.Factory.Create<RvmDateRange?>(this, v => intervalo = v)));

        Dias(cortado)[22].Click();
        Assert.Equal(new RvmDateRange(new DateOnly(2021, 12, 23), null), intervalo);

        Dias(cortado)[4].Click();
        Assert.Equal(new RvmDateRange(new DateOnly(2021, 12, 5), new DateOnly(2021, 12, 23)), intervalo);
        Assert.Contains("rvm-no-intervalo", cortado.FindAll("td[role=gridcell]")[10].GetAttribute("class"));
        Assert.Equal(2, cortado.FindAll("td[aria-selected=true]").Count);

        Dias(cortado)[27].Click();
        Assert.Equal(new RvmDateRange(new DateOnly(2021, 12, 28), null), intervalo);
    }

    [Fact]
    public void Abre_no_mes_da_data_escolhida_e_aceita_classe()
    {
        var cortado = Calendario(p => p.Add(x => x.Value, new DateOnly(2022, 3, 11)).AddUnmatched("class", "minha"));

        Assert.Equal("marco de 2022", cortado.Find("h2").TextContent);
        Assert.Equal("rvm-calendario minha", cortado.Find("div.rvm-calendario").GetAttribute("class"));
    }

    [Fact]
    public void Intervalo_contem_as_pontas()
    {
        var intervalo = new RvmDateRange(new DateOnly(2021, 1, 1), new DateOnly(2021, 1, 3));

        Assert.True(intervalo.Contains(new DateOnly(2021, 1, 1)));
        Assert.True(intervalo.Contains(new DateOnly(2021, 1, 3)));
        Assert.False(intervalo.Contains(new DateOnly(2021, 1, 4)));
        Assert.True(new RvmDateRange(new DateOnly(2021, 1, 1), null).Contains(new DateOnly(2021, 1, 1)));
    }
}

public class RvmDatePickerTests : BunitContext
{
    private static readonly DateOnly Hoje = new(2021, 12, 14);

    private sealed class Pedido
    {
        public DateOnly? Entrega { get; set; }

        public RvmDateRange? Periodo { get; set; }
    }

    public RvmDatePickerTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Fechado_e_um_botao_que_anuncia_o_dialogo_com_rotulo_e_valor()
    {
        var cortado = Render<RvmDatePicker>(p => p.Add(x => x.Label, "Entrega").Add(x => x.Value, new DateOnly(2021, 12, 3)));

        var botao = cortado.Find("button.rvm-entrada");
        Assert.Equal("dialog", botao.GetAttribute("aria-haspopup"));
        Assert.Equal("false", botao.GetAttribute("aria-expanded"));
        Assert.Equal($"{cortado.Find(".rvm-rotulo").Id} {cortado.Find(".rvm-valor").Id}", botao.GetAttribute("aria-labelledby"));
        Assert.Equal("03/12/2021", cortado.Find(".rvm-valor").TextContent);
        Assert.Contains("rvm-rotulo-fixo", cortado.Find("div.rvm-campo-data").GetAttribute("class"));
        Assert.Empty(cortado.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Abrir_mostra_o_calendario_e_escolher_fecha_e_liga_o_valor()
    {
        var pedido = new Pedido();
        var cortado = Render<RvmDatePicker>(p => p
            .Add(x => x.Label, "Entrega")
            .Add(x => x.Today, Hoje)
            .Add(x => x.Value, pedido.Entrega)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, v => pedido.Entrega = v))
            .Add(x => x.ValueExpression, () => pedido.Entrega));

        cortado.Find("button.rvm-entrada").Click();
        var dialogo = cortado.Find("[role=dialog]");
        Assert.Equal("Escolher Entrega", dialogo.GetAttribute("aria-label"));
        Assert.Equal("true", dialogo.GetAttribute("aria-modal"));
        Assert.Equal("true", cortado.Find("button.rvm-entrada").GetAttribute("aria-expanded"));

        cortado.FindAll("button.rvm-dia")[19].Click();

        Assert.Equal(new DateOnly(2021, 12, 20), pedido.Entrega);
        Assert.Empty(cortado.FindAll("[role=dialog]"));
        Assert.Equal("20/12/2021", cortado.Find(".rvm-valor").TextContent);
    }

    [Fact]
    public void Esc_e_clicar_fora_fecham_sem_escolher()
    {
        var cortado = Render<RvmDatePicker>(p => p.Add(x => x.Label, "Entrega").Add(x => x.Today, Hoje));

        cortado.Find("button.rvm-entrada").Click();
        cortado.Find("[role=dialog]").KeyDown(key: "a");
        Assert.NotEmpty(cortado.FindAll("[role=dialog]"));
        cortado.Find("[role=dialog]").KeyDown(key: "Escape");
        Assert.Empty(cortado.FindAll("[role=dialog]"));

        cortado.Find("button.rvm-entrada").Click();
        cortado.Find(".rvm-fundo").Click();
        Assert.Empty(cortado.FindAll("[role=dialog]"));

        cortado.Find("button.rvm-entrada").Click();
        cortado.Find("button.rvm-entrada").Click();
        Assert.Empty(cortado.FindAll("[role=dialog]"));
        Assert.Equal("dd/mm/aaaa", cortado.Find(".rvm-valor").TextContent);
    }

    [Fact]
    public void Desabilitado_nao_abre()
    {
        var cortado = Render<RvmDatePicker>(p => p.Add(x => x.Label, "Entrega").Add(x => x.Disabled, true));

        Assert.True(cortado.Find("button.rvm-entrada").HasAttribute("disabled"));
        Assert.Contains("rvm-desabilitado", cortado.Find("div.rvm-campo-data").GetAttribute("class"));
    }

    [Fact]
    public void Name_gera_input_hidden_em_formato_iso()
    {
        var cortado = Render<RvmDatePicker>(p => p
            .Add(x => x.Label, "Entrega")
            .Add(x => x.Name, "entrega")
            .Add(x => x.Value, new DateOnly(2021, 12, 3)));

        var escondido = cortado.Find("input[type=hidden]");
        Assert.Equal("entrega", escondido.GetAttribute("name"));
        Assert.Equal("2021-12-03", escondido.GetAttribute("value"));
    }

    [Fact]
    public void Dentro_do_editform_mostra_a_validacao()
    {
        var pedido = new Pedido();
        var contexto = new EditContext(pedido);
        var mensagens = new ValidationMessageStore(contexto);
        var cortado = Render<RvmDatePicker>(p => p
            .AddCascadingValue(contexto)
            .Add(x => x.Label, "Entrega")
            .Add(x => x.Required, true)
            .Add(x => x.HelperText, "Dia util")
            .Add(x => x.Today, Hoje)
            .Add(x => x.Value, pedido.Entrega)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, v => pedido.Entrega = v))
            .Add(x => x.ValueExpression, () => pedido.Entrega));

        Assert.Equal("Dia util", cortado.Find(".rvm-apoio").TextContent);
        Assert.Equal("(obrigatorio)", cortado.Find(".rvm-rotulo .rvm-so-leitor").TextContent);

        mensagens.Add(contexto.Field(nameof(Pedido.Entrega)), "Escolha a data de entrega.");
        cortado.InvokeAsync(contexto.NotifyValidationStateChanged);

        var botao = cortado.Find("button.rvm-entrada");
        Assert.Equal("true", botao.GetAttribute("aria-invalid"));
        Assert.Equal(cortado.Find(".rvm-apoio").Id, botao.GetAttribute("aria-describedby"));
        Assert.Equal("Escolha a data de entrega.", cortado.Find(".rvm-apoio").TextContent);

        cortado.Find("button.rvm-entrada").Click();
        cortado.FindAll("button.rvm-dia")[0].Click();
        Assert.True(contexto.IsModified(contexto.Field(nameof(Pedido.Entrega))));
    }

    [Fact]
    public void Erro_por_fora_tamanho_e_atributos()
    {
        var cortado = Render<RvmDatePicker>(p => p
            .Add(x => x.Label, "Entrega")
            .Add(x => x.ErrorText, "Data fora do prazo.")
            .Add(x => x.Size, RvmSize.Small)
            .Add(x => x.Placeholder, "Escolha")
            .AddUnmatched("class", "minha")
            .AddUnmatched("style", "width: 200px")
            .AddUnmatched("data-teste", "x"));

        var raiz = cortado.Find("div.rvm-campo-data");
        Assert.Equal("rvm-campo-data rvm-pequeno rvm-erro minha", raiz.GetAttribute("class"));
        Assert.Equal("width: 200px", raiz.GetAttribute("style"));
        Assert.Equal("x", cortado.Find("button.rvm-entrada").GetAttribute("data-teste"));
        Assert.Equal("Escolha", cortado.Find(".rvm-valor").TextContent);
        Assert.Equal("Data fora do prazo.", cortado.Find(".rvm-apoio").TextContent);
    }

    [Fact]
    public void Intervalo_fecha_so_no_segundo_clique_e_envia_as_duas_pontas()
    {
        var pedido = new Pedido();
        var contexto = new EditContext(pedido);
        var cortado = Render<RvmDateRangePicker>(p => p
            .AddCascadingValue(contexto)
            .Add(x => x.Label, "Periodo")
            .Add(x => x.Name, "periodo")
            .Add(x => x.Today, Hoje)
            .Add(x => x.Value, pedido.Periodo)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<RvmDateRange?>(this, v => pedido.Periodo = v))
            .Add(x => x.ValueExpression, () => pedido.Periodo));

        Assert.Equal("dd/mm/aaaa – dd/mm/aaaa", cortado.Find(".rvm-valor").TextContent);
        cortado.Find("button.rvm-entrada").Click();
        Assert.Equal("Escolher Periodo: primeiro o inicio, depois o fim", cortado.Find("[role=dialog]").GetAttribute("aria-label"));

        cortado.FindAll("button.rvm-dia")[9].Click();
        Assert.NotEmpty(cortado.FindAll("[role=dialog]"));
        Assert.Equal("10/12/2021 – …", cortado.Find(".rvm-valor").TextContent);
        Assert.Equal(["2021-12-10"], cortado.FindAll("input[type=hidden]").Select(i => i.GetAttribute("value")));

        cortado.FindAll("button.rvm-dia")[19].Click();
        Assert.Empty(cortado.FindAll("[role=dialog]"));
        Assert.Equal("10/12/2021 – 20/12/2021", cortado.Find(".rvm-valor").TextContent);
        Assert.Equal(["periodoInicio", "periodoFim"], cortado.FindAll("input[type=hidden]").Select(i => i.GetAttribute("name")));
        Assert.True(contexto.IsModified(contexto.Field(nameof(Pedido.Periodo))));
    }
}

public class RvmTimePickerTests : BunitContext
{
    public RvmTimePickerTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Lista_de_24_horas_de_30_em_30_minutos()
    {
        var cortado = Render<RvmTimePicker>(p => p.Add(x => x.Label, "Horario").Add(x => x.Searchable, false));

        cortado.Find("[role=combobox]").Click();

        var opcoes = cortado.FindAll("[role=option]");
        Assert.Equal(48, opcoes.Count);
        Assert.Equal("00:00", opcoes[0].TextContent.Trim());
        Assert.Equal("23:30", opcoes[47].TextContent.Trim());
    }

    [Fact]
    public void Doze_horas_com_passo_e_limites()
    {
        var cortado = Render<RvmTimePicker>(p => p
            .Add(x => x.Label, "Horario")
            .Add(x => x.Use24Hours, false)
            .Add(x => x.Step, 60)
            .Add(x => x.Min, new TimeOnly(11, 0))
            .Add(x => x.Max, new TimeOnly(13, 0))
            .Add(x => x.Searchable, false));

        cortado.Find("[role=combobox]").Click();

        Assert.Equal(["11:00 AM", "12:00 PM", "1:00 PM"], cortado.FindAll("[role=option]").Select(o => o.TextContent.Trim()));
    }

    [Fact]
    public void Escolher_liga_o_valor_e_envia_em_formato_invariante()
    {
        TimeOnly? escolhido = null;
        var cortado = Render<RvmTimePicker>(p => p
            .Add(x => x.Label, "Horario")
            .Add(x => x.Name, "horario")
            .Add(x => x.Min, new TimeOnly(18, 0))
            .Add(x => x.Max, new TimeOnly(19, 0))
            .Add(x => x.Step, 15)
            .Add(x => x.Searchable, false)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => escolhido = v)));

        cortado.Find("[role=combobox]").Click();
        cortado.FindAll("[role=option]")[2].Click();

        Assert.Equal(new TimeOnly(18, 30), escolhido);
        cortado.Render(p => p.Add(x => x.Value, escolhido));
        Assert.Equal("18:30", cortado.Find(".rvm-valor").TextContent);
        Assert.Equal("18:30", cortado.Find("input[type=hidden]").GetAttribute("value"));
    }

    [Fact]
    public void Busca_ligada_por_padrao_com_textos_de_horario()
    {
        var cortado = Render<RvmTimePicker>(p => p.Add(x => x.Label, "Horario"));

        cortado.Find("[role=combobox]").Click();

        Assert.Equal("Buscar horario", cortado.Find("input.rvm-busca").GetAttribute("aria-label"));
        cortado.Find("input.rvm-busca").Input("trigo");
        Assert.Equal("Nenhum horario encontrado para essa busca.", cortado.Find(".rvm-vazio").TextContent);
    }
}
