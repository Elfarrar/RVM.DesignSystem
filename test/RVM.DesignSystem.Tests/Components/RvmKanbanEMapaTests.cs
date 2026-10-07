using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Kanban;
using RVM.DesignSystem.Components.Map;
using RVM.DesignSystem.Components.Menu;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>RvmKanbanBoard e RvmMap, do contrato com o RVM.UI (DSGN-017).</summary>
public class RvmKanbanEMapaTests : BunitContext
{
    public RvmKanbanEMapaTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    // --- RvmKanbanBoard ---

    private static List<RvmKanbanColumn<string>> Colunas() =>
    [
        new("fazer", "A fazer", ["Calibrar plantadeira", "Comprar semente"]),
        new("andamento", "Em andamento", ["Plantar talhao 3"]),
        new("feito", "Feito", [])
    ];

    // O cartao com o menu do proprio cartao, onde entram os itens "Mover para X".
    private static readonly RenderFragment<(string Item, RenderFragment MoveItems)> Cartao = ctx => b =>
    {
        b.OpenElement(0, "span");
        b.AddAttribute(1, "class", "texto");
        b.AddContent(2, ctx.Item);
        b.CloseElement();
        b.OpenComponent<RvmMenu>(3);
        b.AddComponentParameter(4, nameof(RvmMenu.Label), $"Acoes de {ctx.Item}");
        b.AddComponentParameter(5, nameof(RvmMenu.Trigger), RvmMenuTrigger.Icon);
        b.AddComponentParameter(6, nameof(RvmMenu.ChildContent), ctx.MoveItems);
        b.CloseComponent();
    };

    private IRenderedComponent<RvmKanbanBoard<string>> Quadro(Action<RvmKanbanMove<string>>? aoMover = null, bool comTitulo = true)
        => Render<RvmKanbanBoard<string>>(p =>
        {
            p.Add(x => x.Columns, Colunas())
             .Add(x => x.ItemTemplate, Cartao)
             .Add(x => x.OnMove, (RvmKanbanMove<string> m) => aoMover?.Invoke(m));
            if (comTitulo)
            {
                p.Add(x => x.ItemTitleSelector, (Func<string, string>)(s => s));
            }
        });

    [Fact]
    public void Colunas_sao_secoes_nomeadas_com_contagem_e_cartoes_em_lista()
    {
        var cortado = Render<RvmKanbanBoard<string>>(p => p
            .Add(x => x.Columns, Colunas())
            .Add(x => x.ItemTemplate, Cartao)
            .Add(x => x.Class, "safra")
            .Add(x => x.ColumnActions, c => b => b.AddMarkupContent(0, $"<button class=\"add\">Nova em {c.Title}</button>"))
            .AddUnmatched("data-testid", "quadro"));

        var raiz = cortado.Find(".rvm-kanban");
        Assert.Equal("group", raiz.GetAttribute("role"));
        Assert.Equal("Quadro de tarefas", raiz.GetAttribute("aria-label"));
        Assert.Contains("safra", raiz.ClassList);
        Assert.Equal("quadro", raiz.GetAttribute("data-testid"));

        var secoes = cortado.FindAll("section");
        Assert.Equal(3, secoes.Count);
        var titulo = cortado.Find($"#{secoes[0].GetAttribute("aria-labelledby")}");
        Assert.Equal("A fazer (2)", titulo.TextContent.Trim());
        Assert.Equal(2, secoes[0].QuerySelectorAll("li[draggable=true]").Length);
        Assert.Equal("Nova em Feito", secoes[2].QuerySelector(".add")!.TextContent);
        Assert.Empty(cortado.FindAll(".rvm-area-de-soltar"));
    }

    [Fact]
    public void Arrastar_e_soltar_em_outra_coluna_avisa_OnMove_no_fim_e_anuncia()
    {
        RvmKanbanMove<string>? movido = null;
        var cortado = Quadro(m => movido = m);

        cortado.FindAll("li")[0].DragStart();
        Assert.Contains("rvm-arrastando", cortado.FindAll("li")[0].ClassList);

        // Entrar num filho antes de sair do pai nao apaga a area de soltar.
        cortado.FindAll("section")[1].DragEnter();
        cortado.FindAll("section")[1].DragEnter();
        cortado.FindAll("section")[1].DragLeave();
        Assert.Contains("rvm-alvo", cortado.FindAll("section")[1].ClassList);
        Assert.Single(cortado.FindAll(".rvm-area-de-soltar"));

        cortado.FindAll("section")[1].Drop();

        Assert.Equal(new RvmKanbanMove<string>("Calibrar plantadeira", "fazer", "andamento", 1), movido);
        Assert.Equal("Calibrar plantadeira movido para Em andamento.", cortado.Find("[role=status]").TextContent);
        Assert.Empty(cortado.FindAll(".rvm-area-de-soltar"));
        Assert.Empty(cortado.FindAll(".rvm-arrastando"));
    }

    [Fact]
    public void Soltar_na_propria_coluna_ou_sem_arrastar_nao_move()
    {
        var avisos = 0;
        var cortado = Quadro(_ => avisos++);

        cortado.FindAll("li")[0].DragStart();
        cortado.FindAll("section")[0].DragEnter();
        Assert.Empty(cortado.FindAll(".rvm-area-de-soltar"));
        cortado.FindAll("section")[0].Drop();

        cortado.FindAll("section")[2].Drop();

        cortado.FindAll("li")[0].DragStart();
        cortado.FindAll("section")[1].DragEnter();
        cortado.FindAll("section")[1].DragLeave();
        Assert.Empty(cortado.FindAll(".rvm-area-de-soltar"));
        cortado.FindAll("li")[0].DragEnd();
        cortado.FindAll("section")[1].Drop();

        Assert.Equal(0, avisos);
        Assert.Equal("", cortado.Find("[role=status]").TextContent);
    }

    [Fact]
    public void Mover_pelo_menu_do_cartao_lista_as_outras_colunas_e_avisa()
    {
        RvmKanbanMove<string>? movido = null;
        var cortado = Quadro(m => movido = m, comTitulo: false);

        cortado.Find("button[aria-label='Acoes de Plantar talhao 3']").Click();
        var itens = cortado.FindAll("[role=menuitem]");
        Assert.Equal(["Mover para A fazer", "Mover para Feito"], itens.Select(i => i.TextContent.Trim()));

        itens[1].Click();

        Assert.Equal(new RvmKanbanMove<string>("Plantar talhao 3", "andamento", "feito", 0), movido);
        Assert.Equal("Cartao movido para Feito.", cortado.Find("[role=status]").TextContent);
    }

    // --- RvmMap ---

    private static readonly RvmMapMarker[] Fazendas =
    [
        new("Fazenda Boa Vista", -12.55, -55.72, "1.240 ha"),
        new("Fazenda Santa Rita", -13.05, -55.90, "860 ha", RvmColor.Success),
        new("Armazem Sorriso", -12.80, -55.40)
    ];

    private static double Pct(string estilo, string propriedade)
    {
        var trecho = estilo.Split(';').Select(s => s.Trim()).First(s => s.StartsWith(propriedade + ":"));
        return double.Parse(trecho[(propriedade.Length + 1)..].Trim().TrimEnd('%'), System.Globalization.CultureInfo.InvariantCulture);
    }

    [Fact]
    public void Cabecalho_nomeia_a_secao_e_repassa_classe_e_atributos()
    {
        var cortado = Render<RvmMap>(p => p
            .Add(x => x.Title, "Fazendas")
            .Add(x => x.Subtitle, "Safra 2026/27")
            .Add(x => x.Actions, b => b.AddMarkupContent(0, "<button class=\"filtro\">Filtrar</button>"))
            .Add(x => x.Class, "painel")
            .AddUnmatched("data-testid", "mapa"));

        var raiz = cortado.Find("section.rvm-mapa");
        Assert.Contains("painel", raiz.ClassList);
        Assert.Equal("mapa", raiz.GetAttribute("data-testid"));
        Assert.Equal("Fazendas", cortado.Find($"#{raiz.GetAttribute("aria-labelledby")}").TextContent);
        Assert.Contains("Safra 2026/27", raiz.TextContent);
        Assert.NotNull(cortado.Find(".rvm-acoes .filtro"));
        Assert.Empty(cortado.FindAll(".rvm-marcador"));
        Assert.Empty(cortado.FindAll("ul"));
        Assert.NotEmpty(cortado.FindAll(".rvm-grade line"));
    }

    [Fact]
    public void Marcadores_sao_focaveis_nomeados_e_enquadrados_no_miolo()
    {
        var cortado = Render<RvmMap>(p => p.Add(x => x.Title, "Fazendas").Add(x => x.Markers, Fazendas));

        Assert.Equal("Lugares no mapa: Fazendas", cortado.Find(".rvm-marcadores").GetAttribute("aria-label"));
        var marcadores = cortado.FindAll(".rvm-marcador");
        Assert.Equal(3, marcadores.Count);
        Assert.All(marcadores, m => Assert.Equal("0", m.GetAttribute("tabindex")));
        Assert.Equal("Fazenda Boa Vista: 1.240 ha", marcadores[0].GetAttribute("aria-label"));
        Assert.Equal("Armazem Sorriso", marcadores[2].GetAttribute("aria-label"));
        Assert.Contains("rvm-primary", marcadores[0].ClassList);
        Assert.Contains("rvm-success", marcadores[1].ClassList);

        var (x, y) = (marcadores.Select(m => Pct(m.GetAttribute("style")!, "left")).ToList(),
                      marcadores.Select(m => Pct(m.GetAttribute("style")!, "top")).ToList());
        Assert.All(x.Concat(y), v => Assert.InRange(v, 14.9, 85.1));
        Assert.True(y[0] < y[2] && y[2] < y[1], "mais ao norte fica mais acima");
        Assert.True(x[1] < x[0] && x[0] < x[2], "mais a oeste fica mais a esquerda");
        Assert.Contains("rvm-rotulo-a-esquerda", marcadores[2].ClassList);

        // Sem lista propria, a lista em texto oculta carrega o dado.
        var oculta = cortado.Find("ul.rvm-so-leitor");
        Assert.Equal(["Fazenda Boa Vista: 1.240 ha", "Fazenda Santa Rita: 860 ha", "Armazem Sorriso"],
            oculta.QuerySelectorAll("li").Select(li => li.TextContent));
    }

    [Fact]
    public void Um_marcador_so_fica_no_centro_e_lista_propria_substitui_a_oculta()
    {
        var cortado = Render<RvmMap>(p => p
            .Add(x => x.Title, "Sede")
            .Add(x => x.Markers, [new RvmMapMarker("Sede", -15.6, -56.1)])
            .Add(x => x.ChildContent, b => b.AddMarkupContent(0, "<ol class=\"minha\"><li>Sede</li></ol>")));

        var estilo = cortado.Find(".rvm-marcador").GetAttribute("style")!;
        Assert.Equal(50, Pct(estilo, "left"), 3);
        Assert.Equal(50, Pct(estilo, "top"), 3);
        Assert.NotNull(cortado.Find(".rvm-lista ol.minha"));
        Assert.Empty(cortado.FindAll("ul.rvm-so-leitor"));
    }

    [Fact]
    public void Coordenada_invalida_nao_e_desenhada_e_vira_aviso()
    {
        var cortado = Render<RvmMap>(p => p
            .Add(x => x.Title, "Fazendas")
            .Add(x => x.Markers, [.. Fazendas, new RvmMapMarker("Talhao sem GPS", 0, 999)]));

        Assert.Equal(3, cortado.FindAll(".rvm-marcador").Count);
        Assert.Equal("Talhao sem GPS nao aparece no mapa: a coordenada esta fora do intervalo valido.", cortado.Find(".rvm-aviso").TextContent);
        Assert.Equal(4, cortado.FindAll("ul.rvm-so-leitor li").Count);

        cortado.Render(p => p.Add(x => x.Markers,
            [new RvmMapMarker("A", 91, 0), new RvmMapMarker("B", 0, -181)]));
        Assert.Empty(cortado.FindAll(".rvm-marcador"));
        Assert.Equal("2 lugares nao aparecem no mapa (A, B): as coordenadas estao fora do intervalo valido.", cortado.Find(".rvm-aviso").TextContent);
    }
}
