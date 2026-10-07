using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RVM.DesignSystem.Components.Avatar;
using RVM.DesignSystem.Components.EventCalendar;
using RVM.DesignSystem.Components.Tooltip;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>RvmEventCalendar, RvmEventPill, RvmEventCard e RvmMiniCalendar, do contrato com o RVM.UI (DSGN-017).</summary>
public class RvmCalendarioDeEventosTests : BunitContext
{
    private static readonly DateTime Agora = new(2026, 10, 7, 10, 30, 0);

    public RvmCalendarioDeEventosTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private static RvmCalendarEvent Evento(string titulo, DateTime inicio, RvmColor cor = RvmColor.Accent)
        => new() { Title = titulo, Start = inicio, Color = cor };

    // --- Tipos ---

    [Fact]
    public void Evento_sem_fim_dura_uma_hora_e_tem_Accent_por_padrao()
    {
        var e = new RvmCalendarEvent { Title = "Plantio", Start = Agora };

        Assert.Equal(Agora.AddHours(1), e.EndOrDefault);
        Assert.Equal(RvmColor.Accent, e.Color);
        Assert.Equal(Agora.AddHours(3), (e with { End = Agora.AddHours(3) }).EndOrDefault);
    }

    [Fact]
    public void Padroes_dos_parametros_sao_os_do_RVM_UI()
    {
        var agenda = new RvmEventCalendar();
        Assert.Equal(RvmCalendarView.Month, agenda.View);
        Assert.Equal(6, agenda.FirstHour);
        Assert.Equal(20, agenda.LastHour);
        Assert.Equal(3, agenda.MaxEventsPerDay);
        Assert.Equal("Hoje", agenda.TodayText);
        Assert.Equal(3, new RvmEventCard().MaxAttendees);
        Assert.Equal(RvmColor.Accent, new RvmEventPill().Color);
        Assert.Equal(RvmPlacement.Top, new RvmMiniCalendar().TooltipPlacement);
        var dia = new RvmMiniCalendarDay();
        Assert.Equal((0, RvmColor.Accent, (string?)null), (dia.Count, dia.Color, dia.Tooltip));
    }

    // --- RvmEventCalendar ---

    [Fact]
    public void Mes_tem_titulo_em_PT_BR_semanas_completas_e_marca_hoje()
    {
        var cortado = Render<RvmEventCalendar>(p => p.Add(x => x.Now, Agora).Add(x => x.Class, "minha"));

        Assert.Equal("Outubro de 2026", cortado.Find(".rvm-agenda-titulo").TextContent);
        Assert.Equal("Agenda de Outubro de 2026", cortado.Find("section").GetAttribute("aria-label"));
        Assert.Contains("minha", cortado.Find("section").ClassList);
        Assert.Equal(["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sab"], cortado.FindAll("thead th").Select(t => t.TextContent));
        Assert.Equal("domingo", cortado.Find("thead th").GetAttribute("abbr"));
        // Outubro de 2026 comeca numa quinta: 5 semanas, de 27/09 a 07/11.
        Assert.Equal(35, cortado.FindAll("tbody td").Count);
        Assert.Contains("rvm-agenda-fora", cortado.FindAll("tbody td")[0].ClassList);
        var hoje = cortado.Find("[aria-current='date']");
        Assert.Contains("07", hoje.TextContent);
        Assert.Contains("(hoje)", hoje.TextContent);
    }

    [Fact]
    public void Passa_do_maximo_vira_mais_N_e_o_clique_avisa_o_dia()
    {
        DateOnly? diaDoMais = null;
        RvmCalendarEvent? clicado = null;
        var eventos = Enumerable.Range(0, 5).Select(i => Evento($"Visita {i}", Agora.Date.AddHours(8 + i))).ToList();
        var cortado = Render<RvmEventCalendar>(p => p
            .Add(x => x.Now, Agora)
            .Add(x => x.Events, eventos)
            .Add(x => x.MaxEventsPerDay, 2)
            .Add(x => x.OnEventClick, (RvmCalendarEvent e) => clicado = e)
            .Add(x => x.OnOverflowClick, (DateOnly d) => diaDoMais = d));

        var pilulas = cortado.FindAll("button.rvm-pilula-evento");
        Assert.Equal(2, pilulas.Count);
        var mais = cortado.Find("button.rvm-agenda-mais");
        Assert.Contains("+3", mais.TextContent);
        Assert.Contains("mais 3 compromissos", mais.TextContent);

        mais.Click();
        Assert.Equal(new DateOnly(2026, 10, 7), diaDoMais);

        pilulas[1].Click();
        Assert.Same(eventos[1], clicado);
        Assert.NotNull(cortado.Instance.LastClickedEventElement);
    }

    [Fact]
    public void Sem_callbacks_pilula_e_mais_nao_sao_botao()
    {
        var eventos = Enumerable.Range(0, 4).Select(i => Evento($"Colheita {i}", Agora.Date.AddHours(8 + i))).ToList();
        var cortado = Render<RvmEventCalendar>(p => p.Add(x => x.Now, Agora).Add(x => x.Events, eventos));

        Assert.Empty(cortado.FindAll("tbody button"));
        Assert.Equal(3, cortado.FindAll("span.rvm-pilula-evento").Count);
        Assert.Contains("+1", cortado.Find("span.rvm-agenda-mais").TextContent);
        Assert.Contains("mais 1 compromisso", cortado.Find("span.rvm-agenda-mais").TextContent);
    }

    [Fact]
    public void Setas_e_Hoje_andam_pela_visao_e_avisam_DateChanged()
    {
        var avisos = new List<DateOnly>();
        var cortado = Render<RvmEventCalendar>(p => p
            .Add(x => x.Now, Agora)
            .Add(x => x.View, RvmCalendarView.Week)
            .Add(x => x.DateChanged, (DateOnly d) => avisos.Add(d)));

        Assert.Equal("4 a 10 de outubro de 2026", cortado.Find(".rvm-agenda-titulo").TextContent);
        cortado.Find("button[aria-label='Proxima semana']").Click();
        Assert.Equal("11 a 17 de outubro de 2026", cortado.Find(".rvm-agenda-titulo").TextContent);
        cortado.Find("button[aria-label='Semana anterior']").Click();
        cortado.Find("button[aria-label='Semana anterior']").Click();
        Assert.Equal("27 de setembro a 3 de outubro de 2026", cortado.Find(".rvm-agenda-titulo").TextContent);
        cortado.FindAll(".rvm-agenda-acoes button")[0].Click();

        Assert.Equal([new DateOnly(2026, 10, 14), new DateOnly(2026, 10, 7), new DateOnly(2026, 9, 30), new DateOnly(2026, 10, 7)], avisos);
    }

    [Fact]
    public void Navegacao_nao_e_desfeita_por_render_do_pai_mas_segue_Date_novo()
    {
        var cortado = Render<RvmEventCalendar>(p => p.Add(x => x.Now, Agora).Add(x => x.Date, new DateOnly(2026, 10, 7)));

        cortado.Find("button[aria-label='Proximo mes']").Click();
        cortado.Render(p => p.Add(x => x.Date, new DateOnly(2026, 10, 7)).Add(x => x.TodayText, "Hoje"));
        Assert.Equal("Novembro de 2026", cortado.Find(".rvm-agenda-titulo").TextContent);

        cortado.Render(p => p.Add(x => x.Date, new DateOnly(2027, 1, 15)));
        Assert.Equal("Janeiro de 2027", cortado.Find(".rvm-agenda-titulo").TextContent);
    }

    [Fact]
    public void Dia_tem_faixa_de_horas_compromisso_na_hora_certa_e_a_linha_do_agora()
    {
        var cortado = Render<RvmEventCalendar>(p => p
            .Add(x => x.Now, Agora)
            .Add(x => x.View, RvmCalendarView.Day)
            .Add(x => x.FirstHour, 7)
            .Add(x => x.LastHour, 12)
            .Add(x => x.Events, [Evento("Visita tecnica", Agora.Date.AddHours(9), RvmColor.Info)]));

        Assert.Equal("Quarta-feira, 7 de outubro de 2026", cortado.Find(".rvm-agenda-titulo").TextContent);
        var horas = cortado.FindAll("th.rvm-agenda-hora").Select(h => h.TextContent).ToList();
        Assert.Equal(["07:00", "08:00", "09:00", "10:00", "11:00", "12:00"], horas);
        var linhas = cortado.FindAll("tbody tr");
        Assert.Contains("Visita tecnica", linhas[2].TextContent);
        Assert.Contains("09:00 as 10:00", linhas[2].TextContent);
        Assert.Contains("rvm-info", linhas[2].QuerySelector(".rvm-pilula-evento")!.ClassList);
        var agora = cortado.Find(".rvm-agenda-agora");
        Assert.Contains("--rvm-agenda-agora: 50%", agora.GetAttribute("style"));
        Assert.Same(linhas[3], agora.Closest("tr"));
    }

    [Fact]
    public void Semana_tem_sete_colunas_de_domingo_a_sabado()
    {
        var cortado = Render<RvmEventCalendar>(p => p
            .Add(x => x.Now, Agora)
            .Add(x => x.View, RvmCalendarView.Week)
            .Add(x => x.Label, "Agenda da fazenda"));

        var cabecalhos = cortado.FindAll("thead th");
        Assert.Equal(8, cabecalhos.Count);
        Assert.Equal("domingo, 4 de outubro de 2026", cabecalhos[1].GetAttribute("abbr"));
        Assert.Equal("Agenda da fazenda", cortado.Find("section").GetAttribute("aria-label"));
        Assert.Equal(15, cortado.FindAll("tbody tr").Count);
    }

    [Fact]
    public void Faixa_de_horas_invalida_lanca_so_onde_a_grade_de_horas_existe()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Render<RvmEventCalendar>(p => p
            .Add(x => x.View, RvmCalendarView.Week).Add(x => x.FirstHour, 10).Add(x => x.LastHour, 8)));

        // No mes as horas nem sao usadas: a faixa invalida nao derruba o componente.
        var mes = Render<RvmEventCalendar>(p => p.Add(x => x.Now, Agora).Add(x => x.FirstHour, 10).Add(x => x.LastHour, 24));
        Assert.NotEmpty(mes.FindAll("tbody tr"));
    }

    [Fact]
    public void Evento_fora_da_faixa_de_horas_vai_para_a_primeira_ou_a_ultima_linha()
    {
        var cortado = Render<RvmEventCalendar>(p => p
            .Add(x => x.Now, Agora)
            .Add(x => x.View, RvmCalendarView.Day)
            .Add(x => x.FirstHour, 7)
            .Add(x => x.LastHour, 12)
            .Add(x => x.Events, [Evento("Ordenha", Agora.Date.AddHours(5)), Evento("Reuniao", Agora.Date.AddHours(19))]));

        var linhas = cortado.FindAll("tbody tr");
        Assert.Contains("Ordenha", linhas[0].TextContent);
        Assert.Contains("05:00 as 06:00", linhas[0].TextContent);
        Assert.Contains("Reuniao", linhas[^1].TextContent);
    }

    // --- RvmEventPill ---

    [Fact]
    public void Pilula_com_clique_e_botao_avisa_o_elemento_antes_e_anuncia_o_horario()
    {
        var ordem = new List<string>();
        var cortado = Render<RvmEventPill>(p => p
            .Add(x => x.Title, "Plantio")
            .Add(x => x.Color, RvmColor.Danger)
            .Add(x => x.TimeLabel, "07:00 as 12:00")
            .Add(x => x.Selected, true)
            .Add(x => x.ImageUrl, "foto.png")
            .Add(x => x.OnClick, () => ordem.Add("clique"))
            .Add(x => x.OnClickElement, (ElementReference _) => ordem.Add("elemento"))
            .AddUnmatched("data-id", "7"));

        var botao = cortado.Find("button");
        Assert.Equal("true", botao.GetAttribute("aria-pressed"));
        Assert.Equal("7", botao.GetAttribute("data-id"));
        Assert.Contains("rvm-error", botao.ClassList);
        Assert.Contains("rvm-selecionada", botao.ClassList);
        Assert.Equal("", cortado.Find("img").GetAttribute("alt"));
        Assert.Contains(", 07:00 as 12:00", cortado.Find(".rvm-so-leitor").TextContent);

        botao.Click();
        Assert.Equal(["elemento", "clique"], ordem);
    }

    [Fact]
    public void Pilula_sem_clique_e_span_sem_aria_pressed()
    {
        var cortado = Render<RvmEventPill>(p => p.Add(x => x.Title, "Colheita"));

        var raiz = cortado.Find(".rvm-pilula-evento");
        Assert.Equal("SPAN", raiz.TagName);
        Assert.Null(raiz.GetAttribute("aria-pressed"));
        Assert.Contains("rvm-primary", raiz.ClassList);
    }

    // --- RvmEventCard ---

    [Fact]
    public void Cartao_com_faixa_participantes_e_clique()
    {
        var cliques = 0;
        var cortado = Render<RvmEventCard>(p => p
            .Add(x => x.Title, "Reuniao com a cooperativa")
            .Add(x => x.TimeLabel, "16:00 as 17:00")
            .Add(x => x.LineColor, RvmColor.Success)
            .Add(x => x.Attendees, [new RvmAvatarItem("Ana"), new RvmAvatarItem("Bruno"), new RvmAvatarItem("Carla")])
            .Add(x => x.MaxAttendees, 2)
            .Add(x => x.OnClick, () => cliques++));

        var botao = cortado.Find("button.rvm-cartao-evento");
        Assert.Contains("rvm-success", botao.ClassList);
        Assert.Contains("rvm-com-faixa", botao.ClassList);
        Assert.NotNull(cortado.Find(".rvm-cartao-evento-faixa[aria-hidden='true']"));
        Assert.Contains("16:00 as 17:00", cortado.Find(".rvm-cartao-evento-horario").TextContent);
        Assert.Contains("+1", cortado.Find("[role='group']").TextContent);

        botao.Click();
        Assert.Equal(1, cliques);
    }

    [Fact]
    public void Cartao_sem_clique_e_article_sem_faixa()
    {
        var cortado = Render<RvmEventCard>(p => p.Add(x => x.Title, "Revisar estoque"));

        Assert.Equal("ARTICLE", cortado.Find(".rvm-cartao-evento").TagName);
        Assert.Empty(cortado.FindAll(".rvm-cartao-evento-faixa"));
        Assert.Empty(cortado.FindAll(".rvm-cartao-evento-detalhes"));
    }

    // --- RvmMiniCalendar ---

    private static readonly DateOnly Hoje = new(2026, 10, 7);

    // O dia que esta na ordem de tabulacao (tabindex movel): e onde o foco do teclado esta.
    private static string? NoTab(IRenderedComponent<RvmMiniCalendar> cortado)
        => cortado.Find("button.rvm-mini-dia[tabindex='0']").GetAttribute("data-date");

    [Fact]
    public void Mini_mostra_42_dias_com_um_so_no_Tab_e_marcacoes_do_app()
    {
        var cortado = Render<RvmMiniCalendar>(p => p
            .Add(x => x.Today, Hoje)
            .Add(x => x.TooltipPlacement, RvmPlacement.Bottom)
            .Add(x => x.DayInfo, d => d == Hoje
                ? new RvmMiniCalendarDay(5, RvmColor.Success, "🌕", "Lua cheia, 5 tarefas", "Plantio")
                : null));

        Assert.Equal("Outubro de 2026", cortado.Find(".rvm-mini-titulo").TextContent);
        var dias = cortado.FindAll("button.rvm-mini-dia");
        Assert.Equal(42, dias.Count);
        Assert.Single(dias, b => b.GetAttribute("tabindex") == "0");
        var hoje = cortado.Find("button[data-date='2026-10-07']");
        Assert.Equal("0", hoje.GetAttribute("tabindex"));
        Assert.Equal("date", hoje.GetAttribute("aria-current"));
        Assert.Equal("quarta-feira, 7 de outubro de 2026, hoje, Lua cheia, 5 tarefas", hoje.GetAttribute("aria-label"));
        Assert.Equal(3, hoje.QuerySelectorAll(".rvm-mini-ponto").Length);
        Assert.Contains("rvm-success", hoje.QuerySelector(".rvm-mini-pontos")!.ClassList);
        var balao = cortado.Find("[role='tooltip']");
        Assert.Equal("Plantio", balao.TextContent);
        Assert.Contains("rvm-abaixo", balao.ClassList);
        Assert.Equal(balao.Id, hoje.GetAttribute("aria-describedby"));
        Assert.Contains("rvm-mini-fora", cortado.Find("button[data-date='2026-09-27']").ClassList);
    }

    [Fact]
    public void Mini_escolhe_e_com_AllowDeselect_desfaz()
    {
        DateOnly? recebido = new DateOnly(2000, 1, 1);
        var cortado = Render<RvmMiniCalendar>(p => p
            .Add(x => x.Today, Hoje)
            .Add(x => x.Value, new DateOnly(2026, 10, 12))
            .Add(x => x.AllowDeselect, true)
            .Add(x => x.ValueChanged, (DateOnly? d) => recebido = d));

        var escolhido = cortado.Find("button[data-date='2026-10-12']");
        Assert.Contains("rvm-mini-escolhido", escolhido.ClassList);
        Assert.Equal("0", escolhido.GetAttribute("tabindex"));
        Assert.Equal("true", escolhido.ParentElement!.GetAttribute("aria-selected"));

        escolhido.Click();
        Assert.Null(recebido);

        cortado.Find("button[data-date='2026-10-20']").Click();
        Assert.Equal(new DateOnly(2026, 10, 20), recebido);
    }

    [Fact]
    public void Mini_teclado_anda_e_troca_de_mes_na_borda()
    {
        var meses = new List<DateOnly>();
        var cortado = Render<RvmMiniCalendar>(p => p
            .Add(x => x.Today, new DateOnly(2026, 10, 31))
            .Add(x => x.MonthChanged, (DateOnly m) => meses.Add(m)));

        var grade = cortado.Find("table");
        grade.KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Assert.Equal("2026-11-01", NoTab(cortado));
        Assert.Equal("Novembro de 2026", cortado.Find(".rvm-mini-titulo").TextContent);

        cortado.Find("table").KeyDown(new KeyboardEventArgs { Key = "End" });
        Assert.Equal("2026-11-07", NoTab(cortado));
        cortado.Find("table").KeyDown(new KeyboardEventArgs { Key = "Home" });
        Assert.Equal("2026-11-01", NoTab(cortado));
        cortado.Find("table").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        Assert.Equal("2026-11-08", NoTab(cortado));
        cortado.Find("table").KeyDown(new KeyboardEventArgs { Key = "PageDown", ShiftKey = true });
        Assert.Equal("2027-11-08", NoTab(cortado));
        cortado.Find("table").KeyDown(new KeyboardEventArgs { Key = "PageUp" });
        Assert.Equal("2027-10-08", NoTab(cortado));
        cortado.Find("table").KeyDown(new KeyboardEventArgs { Key = "x" });

        Assert.Equal([new DateOnly(2026, 11, 1), new DateOnly(2027, 11, 1), new DateOnly(2027, 10, 1)], meses);
        Assert.Equal("0", cortado.Find("button[data-date='2027-10-08']").GetAttribute("tabindex"));
    }

    [Fact]
    public void Mini_setas_trocam_mes_e_render_do_pai_nao_desfaz()
    {
        var cortado = Render<RvmMiniCalendar>(p => p.Add(x => x.Today, Hoje).Add(x => x.Month, Hoje));

        cortado.Find("button[aria-label='Proximo mes']").Click();
        Assert.Equal("Novembro de 2026", cortado.Find(".rvm-mini-titulo").TextContent);
        cortado.Render(p => p.Add(x => x.Month, Hoje).Add(x => x.AllowDeselect, true));
        Assert.Equal("Novembro de 2026", cortado.Find(".rvm-mini-titulo").TextContent);

        cortado.Render(p => p.Add(x => x.Month, new DateOnly(2027, 3, 20)));
        Assert.Equal("Marco de 2027", cortado.Find(".rvm-mini-titulo").TextContent);
        Assert.Equal("0", cortado.Find("button[data-date='2027-03-01']").GetAttribute("tabindex"));

        cortado.Find("button[aria-label='Mes anterior']").Click();
        Assert.Equal("Fevereiro de 2027", cortado.Find(".rvm-mini-titulo").TextContent);
    }

    [Fact]
    public void Mini_sem_Month_abre_no_mes_do_valor()
    {
        var cortado = Render<RvmMiniCalendar>(p => p.Add(x => x.Today, Hoje).Add(x => x.Value, new DateOnly(2025, 2, 10)));

        Assert.Equal("Fevereiro de 2025", cortado.Find(".rvm-mini-titulo").TextContent);
        Assert.Empty(cortado.FindAll("[role='tooltip']"));
    }
}
