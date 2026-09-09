using System.Text.RegularExpressions;
using Deque.AxeCore.Playwright;
using Microsoft.Playwright;

namespace RVM.DesignSystem.E2E;

/// <summary>
/// Smoke + auditoria de acessibilidade sobre o site JA PUBLICADO.
/// </summary>
/// <remarks>
/// Alvo vem de <c>E2E_BASE_URL</c>; sem a variavel, os testes sao pulados —
/// e por isso que este projeto nao pode rodar no ci.yml de build, e por isso
/// que ele mora sob test/playwright/ (o reusable do RVM.Actions pula esse caminho).
/// </remarks>
public class SiteSmokeTests : IAsyncLifetime
{
    private static readonly string? BaseUrl = Environment.GetEnvironmentVariable("E2E_BASE_URL");

    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public async Task InitializeAsync()
    {
        if (BaseUrl is null) return;
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync();
    }

    public async Task DisposeAsync()
    {
        if (_browser is not null) await _browser.CloseAsync();
        _playwright?.Dispose();
    }

    [SkippableFact]
    public async Task Home_responde_e_renderiza_o_titulo()
    {
        Skip.If(BaseUrl is null, "E2E_BASE_URL nao definida — rodando fora do pipeline de E2E.");

        var page = await _browser!.NewPageAsync();
        var response = await page.GotoAsync(BaseUrl!);

        Assert.NotNull(response);
        Assert.True(response!.Ok, $"HTTP {response.Status} em {BaseUrl}");
        await Assertions.Expect(page.Locator("h1")).ToContainTextAsync("RVM Design System");
    }

    /// <summary>
    /// Audita TODA pagina do menu, nos dois modos de cor.
    /// </summary>
    /// <remarks>
    /// <b>Ate a onda 2 o axe rodava so na inicial</b>, e isso deixava 28 paginas sem cobertura
    /// nenhuma — inclusive as que tem tabela, aba e navegacao, que e onde os defeitos de ARIA
    /// realmente moram. A pendencia estava registrada no card da onda 1.
    ///
    /// <para>
    /// A lista de rotas e <b>descoberta do proprio menu</b>, e nao escrita aqui. Duas razoes: ela
    /// nao desatualiza quando entra pagina nova, e a varredura passa a reprovar tambem um item de
    /// menu apontando para lugar nenhum — defeito que uma lista escrita a mao esconderia, porque
    /// a lista e o menu sao a mesma informacao em dois lugares.
    /// </para>
    ///
    /// <para>
    /// Os DOIS modos importam: o contraste do modo escuro sai de uma paleta derivada
    /// separadamente, e ja houve papel aprovado no claro e reprovado no escuro.
    /// </para>
    /// </remarks>
    [SkippableFact]
    public async Task Toda_pagina_do_menu_passa_no_axe_nos_dois_modos()
    {
        Skip.If(BaseUrl is null, "E2E_BASE_URL nao definida — rodando fora do pipeline de E2E.");

        var page = await _browser!.NewPageAsync();
        await page.GotoAsync(BaseUrl!);
        await Assertions.Expect(page.Locator("h1")).ToBeVisibleAsync();

        var rotas = await DescobrirRotas(page);

        Assert.True(rotas.Count >= 20,
            $"O menu devolveu so {rotas.Count} rotas. Ou a navegacao quebrou, ou o seletor mudou "
            + "— e uma varredura que nao varre nada e pior que varredura nenhuma.");

        var problemas = new List<string>();

        foreach (var rota in rotas)
        {
            foreach (var modo in new[] { "light", "dark" })
            {
                var resposta = await page.GotoAsync(rota);

                if (resposta is null || !resposta.Ok)
                {
                    problemas.Add($"{rota}: HTTP {resposta?.Status.ToString() ?? "sem resposta"}");
                    continue;
                }

                // O modo e um atributo no elemento raiz — a mesma coisa que o seletor do site
                // escreve. Trocar por aqui evita depender do localStorage entre navegacoes.
                await page.EvaluateAsync(
                    "modo => document.documentElement.setAttribute('data-rvm-theme', modo)", modo);

                // Sem esperar o h1, o axe rodaria no esqueleto do WASM e aprovaria uma pagina
                // vazia. Ja aconteceu: e o falso verde mais caro que existe.
                await Assertions.Expect(page.Locator("h1")).ToBeVisibleAsync();

                // O banner de erro do Blazor nao gera violacao de axe, mas significa que a
                // pagina estourou. Foi assim que o RvmRadioGroup<T>.Option apareceu na onda 1.
                var erro = await page.Locator("#blazor-error-ui").IsVisibleAsync();

                if (erro)
                {
                    problemas.Add($"{rota} [{modo}]: a pagina estourou (banner de erro do Blazor).");
                    continue;
                }

                var resultado = await page.RunAxe();

                foreach (var v in resultado.Violations.Where(v => v.Impact is "serious" or "critical"))
                {
                    problemas.Add($"{rota} [{modo}]: {v.Id} ({v.Impact}) — {v.Help}");
                }
            }
        }

        // Portao 3 do CLAUDE.md: violacao seria reprova.
        Assert.True(problemas.Count == 0,
            $"{problemas.Count} problema(s) em {rotas.Count} paginas x 2 modos:\n"
            + string.Join("\n", problemas));
    }

    /// <summary>Os destinos da navegacao lateral, absolutos e sem repeticao.</summary>
    private static async Task<IReadOnlyList<string>> DescobrirRotas(IPage page)
    {
        var hrefs = await page.EvalOnSelectorAllAsync<string[]>(
            "nav a[href]",
            "links => links.map(a => a.href)");

        return hrefs
            .Where(h => h.StartsWith(BaseUrl!, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// O link "pular para o conteudo" e o primeiro focavel do documento e leva mesmo ao conteudo.
    /// </summary>
    /// <remarks>
    /// O axe verifica que o link EXISTE, nao que ele funciona. Um alvo com id errado passa na
    /// auditoria e falha na vida real, em silencio — o link continua parecendo certo.
    ///
    /// <para>
    /// A ordem e conferida pelo DOM, e nao apertando <c>Tab</c>. Em navegador sem janela o
    /// documento nem sempre esta com o foco, e o <c>Tab</c> nao move nada: o teste falharia por
    /// causa do ambiente, e nao por causa da pagina. Como nada aqui usa <c>tabindex</c> positivo
    /// — e ha uma asserção abaixo garantindo isso — ordem do DOM E ordem de tabulacao.
    /// </para>
    /// </remarks>
    [SkippableFact]
    public async Task O_link_de_pulo_e_o_primeiro_focavel_e_leva_ao_conteudo()
    {
        Skip.If(BaseUrl is null, "E2E_BASE_URL nao definida — rodando fora do pipeline de E2E.");

        var page = await _browser!.NewPageAsync();
        await page.GotoAsync(BaseUrl!);
        await Assertions.Expect(page.Locator("h1")).ToBeVisibleAsync();

        const string primeiroFocavel =
            "() => { const s = 'a[href], button:not([disabled]), input, select, textarea, "
            + "[tabindex]:not([tabindex=\"-1\"])'; const e = document.querySelector(s); "
            + "return e ? e.tagName + '|' + (e.getAttribute('href') || '') : null; }";

        Assert.Equal("A|#rvm-conteudo", await page.EvaluateAsync<string?>(primeiroFocavel));

        // Sem tabindex positivo, que reordenaria a tabulacao por cima da ordem do DOM e
        // invalidaria a verificacao acima.
        Assert.Equal(0, await page
            .Locator("[tabindex]:not([tabindex='-1']):not([tabindex='0'])")
            .CountAsync());

        // E o alvo existe de verdade.
        await Assertions.Expect(page.Locator("main#rvm-conteudo")).ToBeAttachedAsync();
    }

    /// <summary>
    /// A casca entrega os marcos de pagina, e um de cada.
    /// </summary>
    /// <remarks>
    /// Dois <c>main</c> na mesma pagina e o mesmo que nenhum: o leitor de tela nao sabe qual e o
    /// conteudo principal. O axe pega o caso de zero, nao o de dois.
    ///
    /// <para>
    /// Conta <b>marco</b>, e nao tag — e a diferenca nao e teorica. O cabecalho de cada pagina de
    /// documentacao e um <c>&lt;header&gt;</c> dentro de um <c>&lt;article&gt;</c>: isso da DUAS
    /// tags <c>header</c> no documento e <b>um</b> marco <c>banner</c>, porque um <c>header</c>
    /// so vira banner quando nao esta dentro de conteudo de secionamento. A primeira versao
    /// deste teste contava a tag, e reprovava marcacao correta.
    /// </para>
    /// </remarks>
    [SkippableFact]
    public async Task A_pagina_tem_exatamente_um_marco_de_cada()
    {
        Skip.If(BaseUrl is null, "E2E_BASE_URL nao definida — rodando fora do pipeline de E2E.");

        var page = await _browser!.NewPageAsync();
        await page.GotoAsync(BaseUrl!);
        await Assertions.Expect(page.Locator("h1")).ToBeVisibleAsync();

        Assert.Equal(1, await page.GetByRole(AriaRole.Main).CountAsync());
        Assert.Equal(1, await page.GetByRole(AriaRole.Banner).CountAsync());
        Assert.Equal(1, await page.GetByRole(AriaRole.Contentinfo).CountAsync());

        // A navegacao lateral tem nome proprio: sem ele, todo `nav` da pagina e anunciado
        // igual e a lista de marcos do leitor de tela deixa de orientar.
        Assert.Equal(1, await page
            .GetByRole(AriaRole.Navigation, new() { Name = "Navegação da documentação" })
            .CountAsync());
    }

    /// <summary>
    /// O criterio de saida da onda 3 (`09-roadmap`), inteiro, num navegador de verdade.
    /// </summary>
    /// <remarks>
    /// <b>Este teste nao tem equivalente unitario, e nao por falta de vontade.</b> Foco preso,
    /// ESC e retorno de foco sao entregues pelo <c>&lt;dialog&gt;</c> nativo com
    /// <c>showModal()</c> — o bUnit nao tem camada de topo, nem inertizacao, nem foco de
    /// verdade. Um teste desses no bUnit provaria que o mock funciona.
    ///
    /// <para>
    /// O retorno de foco e a parte que mais importa: e ela que faz quem navega por teclado nao
    /// se perder. Fechar um dialogo e ver o foco voltar para o <c>&lt;body&gt;</c> significa
    /// recomecar a tabulacao do topo da pagina.
    /// </para>
    /// </remarks>
    [SkippableFact]
    public async Task O_dialogo_prende_o_foco_fecha_no_ESC_e_DEVOLVE_o_foco()
    {
        Skip.If(BaseUrl is null, "E2E_BASE_URL nao definida — rodando fora do pipeline de E2E.");

        var page = await _browser!.NewPageAsync();
        await page.GotoAsync($"{BaseUrl!.TrimEnd('/')}/componentes/dialog");
        await Assertions.Expect(page.Locator("h1")).ToBeVisibleAsync();

        var abrir = page.GetByRole(AriaRole.Button, new() { Name = "Abrir o diálogo" });
        await abrir.ClickAsync();

        var dialogo = page.Locator("dialog.rvm-dialog");
        await Assertions.Expect(dialogo).ToBeVisibleAsync();

        // 1. O dialogo tem nome acessivel — sem ele o leitor anuncia so "diálogo".
        await Assertions.Expect(page.GetByRole(AriaRole.Dialog, new() { Name = "Detalhes do pedido" }))
            .ToBeVisibleAsync();

        // 2. FOCO PRESO — e a asserção certa aqui NÃO é "o foco está sempre dentro do diálogo".
        //
        //    O Chromium, ao passar do último focável de um `<dialog>` modal, leva o foco ao
        //    `<body>` por uma parada antes de voltar ao primeiro. Cobrar "sempre dentro"
        //    REPROVA UMA IMPLEMENTACAO CORRETA — foi o que este teste fez na primeira versao.
        //
        //    O que `showModal()` garante, e o que importa, e que o foco nunca alcanca um
        //    CONTROLE da pagina atras: o resto do documento fica inerte. O `<body>` e a
        //    passagem neutra da volta, nao um escape.
        const string ondeEstaOFoco =
            "() => { const a = document.activeElement; "
            + "if (!a || a === document.body) return 'passagem'; "
            + "return a.closest('dialog.rvm-dialog') ? 'dialogo' : 'PAGINA'; }";

        var voltouParaDentro = false;

        for (var i = 1; i <= 12; i++)
        {
            await page.Keyboard.PressAsync("Tab");

            var onde = await page.EvaluateAsync<string>(ondeEstaOFoco);

            Assert.False(onde == "PAGINA",
                $"O foco alcançou um controle da página atrás do diálogo no {i}º Tab.");

            // Depois de passar do último focável, o foco TEM que voltar para dentro — senão
            // ele ficou preso no body, que é uma armadilha de outro tipo.
            if (i > 4 && onde == "dialogo")
            {
                voltouParaDentro = true;
            }
        }

        Assert.True(voltouParaDentro,
            "O foco saiu do diálogo e não voltou: o laço de tabulação não fecha.");

        // 3. Shift+Tab, a outra ponta do mesmo laço.
        for (var i = 1; i <= 6; i++)
        {
            await page.Keyboard.PressAsync("Shift+Tab");

            Assert.False(await page.EvaluateAsync<string>(ondeEstaOFoco) == "PAGINA",
                $"O foco alcançou a página no {i}º Shift+Tab.");
        }

        // 4. ESC fecha.
        await page.Keyboard.PressAsync("Escape");
        await Assertions.Expect(dialogo).ToBeHiddenAsync();

        // 5. E o FOCO VOLTA para o botao que abriu.
        await Assertions.Expect(abrir).ToBeFocusedAsync();
    }

    /// <summary>
    /// O criterio de saida da onda 4 (`09-roadmap`): a listagem inteira, so por teclado.
    /// </summary>
    /// <remarks>
    /// Nao e um teste de componente — as paginas de componente ja cobrem cada peca. Este prova
    /// que elas funcionam <b>juntas</b>, com volume de verdade (~500 linhas): filtrar, ordenar,
    /// selecionar e paginar sem tocar no mouse.
    /// </remarks>
    [SkippableFact]
    public async Task A_listagem_filtra_ordena_e_pagina_SO_POR_TECLADO()
    {
        Skip.If(BaseUrl is null, "E2E_BASE_URL nao definida — rodando fora do pipeline de E2E.");

        // ⚠️ Locale FIXO, e de proposito em ingles (DSGN-030). O Blazor WASM escolhe o pedaco de
        // ICU pelo idioma do NAVEGADOR, e sem o dado de pt-BR o BrlFormatter cai para invariante:
        // "BRL96.90" em vez de "R$ 96,90". Herdando o idioma da maquina, este teste passava no
        // Windows e falhava no runner. Fixo em en-US, ele prova que o site nao depende disso.
        var page = await _browser!.NewPageAsync(new() { Locale = "en-US" });
        await page.GotoAsync($"{BaseUrl!.TrimEnd('/')}/padroes/listagem");
        await Assertions.Expect(page.Locator("h1")).ToBeVisibleAsync();

        // O resumo e buscado DENTRO da paginacao, e nao por "o primeiro role=status da pagina":
        // a barra de selecao tambem e um status, e assim que ela aparece passa a ser a primeira.
        var resumo = page
            .GetByRole(AriaRole.Navigation, new() { Name = "Paginação" })
            .GetByRole(AriaRole.Status);
        var tabela = page.Locator("table");

        // ---- 1. O volume e real ----
        await Assertions.Expect(resumo).ToContainTextAsync("de 487");

        // ---- 2. ORDENAR pelo teclado ----
        // Focar o cabecalho e apertar Enter: sem handler de teclado nosso, porque ele e um
        // <button> de verdade dentro do <th>.
        var ordenarPorTotal = page.GetByRole(AriaRole.Button, new() { Name = "Ordenar por Total, crescente" });
        await ordenarPorTotal.FocusAsync();
        await page.Keyboard.PressAsync("Enter");

        await Assertions.Expect(page.Locator("th[aria-sort='ascending']")).ToHaveCountAsync(1);

        // A ordem REAL das linhas, e nao so o atributo: o aria-sort podia estar mentindo.
        var totais = await LerColunaDeTotais(page);
        Assert.True(
            totais.SequenceEqual(totais.OrderBy(v => v)),
            "As linhas nao vieram em ordem crescente de total:\n"
            + string.Join(", ", totais.Take(5)));

        // Enter de novo inverte.
        await page.Keyboard.PressAsync("Enter");
        await Assertions.Expect(page.Locator("th[aria-sort='descending']")).ToHaveCountAsync(1);

        var invertidos = await LerColunaDeTotais(page);
        Assert.True(
            invertidos.SequenceEqual(invertidos.OrderByDescending(v => v)),
            "O segundo Enter nao inverteu a ordem.");

        // ---- 3. A legenda diz a ordenacao ----
        // Quem nao ve a seta precisa saber por que as linhas estao nessa ordem.
        await Assertions.Expect(tabela.Locator("caption"))
            .ToContainTextAsync("ordenada por Total");

        // ---- 4. FILTRAR pelo teclado ----
        // GetByRole com o papel explicito, e nao GetByLabel: "Cliente" tambem e o nome do
        // botao de ordenar da coluna, e o locator resolveria para dois elementos.
        var cliente = page.GetByRole(AriaRole.Textbox, new() { Name = "Cliente" });
        await cliente.FocusAsync();
        await page.Keyboard.TypeAsync("Aurora");

        await Assertions.Expect(resumo).Not.ToContainTextAsync("de 487");

        // Toda linha que sobrou casa com o filtro.
        var clientes = await page.EvalOnSelectorAllAsync<string[]>(
            "tbody tr td:nth-child(3)", "tds => tds.map(td => td.textContent.trim())");

        Assert.NotEmpty(clientes);
        Assert.All(clientes, c => Assert.Contains("Aurora", c, StringComparison.OrdinalIgnoreCase));

        // ---- 5. SELECIONAR pelo teclado ----
        // Espaco na caixa de "selecionar todas desta pagina".
        var todas = page.GetByRole(AriaRole.Checkbox,
            new() { Name = "Selecionar todas as linhas desta página" });
        await todas.FocusAsync();
        await page.Keyboard.PressAsync(" ");

        // A barra de selecao aparece com a contagem — e ela e `Live`, entao e anunciada.
        await Assertions.Expect(page.GetByText("selecionado", new() { Exact = false })).ToBeVisibleAsync();

        // ---- 6. PAGINAR pelo teclado ----
        await page.GetByRole(AriaRole.Button, new() { Name = "Limpar filtros" }).ClickAsync();
        await Assertions.Expect(resumo).ToContainTextAsync("de 487");

        var paginacao = page.GetByRole(AriaRole.Navigation, new() { Name = "Paginação" });

        var seguinte = page.GetByRole(AriaRole.Button, new() { Name = "Página seguinte, 2" });
        await seguinte.FocusAsync();
        await page.Keyboard.PressAsync("Enter");

        await Assertions.Expect(resumo).ToContainTextAsync("21–40 de 487");

        // Dentro da paginacao, de novo: o item ATIVO DO MENU tambem e aria-current="page", e
        // um locator solto pegaria os dois. Dois marcadores de "voce esta aqui" convivem na
        // mesma pagina — em contextos diferentes, e cada um esta certo.
        await Assertions.Expect(paginacao.Locator("[aria-current='page']")).ToHaveTextAsync("2");
    }

    /// <summary>Os valores da coluna Total da pagina corrente, como numeros.</summary>
    private static async Task<IReadOnlyList<decimal>> LerColunaDeTotais(IPage page)
    {
        var textos = await page.EvalOnSelectorAllAsync<string[]>(
            "tbody tr td:last-child", "tds => tds.map(td => td.textContent.trim())");

        // "R$ 1.234,56" -> 1234.56. A cultura da string e pt-BR, e nao a do runner.
        var ptBr = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");

        return textos
            .Select(t => decimal.Parse(
                t.Replace("R$", string.Empty, StringComparison.Ordinal).Trim(),
                System.Globalization.NumberStyles.Currency,
                ptBr))
            .ToList();
    }

    /// <summary>
    /// O card do painel leva a listagem JA FILTRADA pela situacao que ele resume.
    /// </summary>
    /// <remarks>
    /// Ate 09/09/2026 as quatro caixas apontavam para a mesma listagem sem filtro, com rotulos
    /// que prometiam destinos diferentes (<c>DSGN-035</c>). O teste clica na caixa e confere o
    /// RESULTADO na tela, e nao a URL: uma query string que a pagina ignorasse passaria por uma
    /// verificacao de endereco.
    /// </remarks>
    [SkippableFact]
    public async Task O_card_do_painel_leva_a_listagem_JA_FILTRADA()
    {
        Skip.If(BaseUrl is null, "E2E_BASE_URL nao definida — rodando fora do pipeline de E2E.");

        var page = await _browser!.NewPageAsync();
        await page.GotoAsync($"{BaseUrl!.TrimEnd('/')}/padroes/dashboard");

        await page.GetByRole(AriaRole.Link, new() { NameRegex = new Regex("Cancelados") }).ClickAsync();

        await Assertions.Expect(page.Locator("h1")).ToHaveTextAsync("Listagem");

        // Pelo CHIP, e nao por indice de coluna: a quarta coluna e a data, e um teste que conta
        // colunas quebra no dia em que alguem reordena a tabela — dizendo que o filtro parou de
        // funcionar quando o que mudou foi o layout.
        var situacoes = page.Locator("tbody tr .rvm-chip");
        var quantas = await situacoes.CountAsync();
        Assert.True(quantas > 0, "A listagem filtrada nao trouxe nenhuma linha.");

        for (var i = 0; i < quantas; i++)
        {
            await Assertions.Expect(situacoes.Nth(i)).ToContainTextAsync("Cancelado");
        }
    }

    /// <summary>
    /// O calendario fala portugues mesmo com o navegador em ingles.
    /// </summary>
    /// <remarks>
    /// A outra ponta do <c>DSGN-030</c>, e a pior das duas: sem os dados de pt-BR, a moeda troca
    /// a pontuacao, mas o calendario troca o <b>idioma</b> — "Su/Mo/Tu", "March 2026" — numa
    /// biblioteca que se declara pt-BR fixo. Confirmado em producao em 09/09/2026, antes do fix.
    /// </remarks>
    [SkippableFact]
    public async Task O_calendario_fala_portugues_com_o_navegador_em_ingles()
    {
        Skip.If(BaseUrl is null, "E2E_BASE_URL nao definida — rodando fora do pipeline de E2E.");

        var page = await _browser!.NewPageAsync(new() { Locale = "en-US" });
        await page.GotoAsync($"{BaseUrl!.TrimEnd('/')}/componentes/date-picker");
        await page.GetByRole(AriaRole.Button, new() { Name = "Abrir o calendário" }).First.ClickAsync();

        // Pelo `abbr` do <th>, e nao pelo texto: a celula carrega "dom." para os olhos e
        // "domingo" para o leitor de tela, e comparar textContent compararia os dois colados.
        var diasDaSemana = await page.EvalOnSelectorAllAsync<string[]>(
            ".rvm-date-picker__dia-semana", "ths => ths.map(th => th.getAttribute('abbr'))");

        Assert.Equal(
            ["domingo", "segunda-feira", "terça-feira", "quarta-feira", "quinta-feira", "sexta-feira", "sábado"],
            diasDaSemana);

        // O titulo e "Marco de 2026". Comparar com o mes de HOJE seria refem do fuso do runner,
        // e quebraria sozinho na virada do mes; a lista dos doze prova a mesma coisa.
        var titulo = await page.Locator(".rvm-date-picker__titulo").First.InnerTextAsync();
        string[] meses =
        [
            "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
            "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
        ];

        Assert.True(
            meses.Any(m => titulo.StartsWith(m, StringComparison.Ordinal)),
            $"O titulo do mes nao esta em portugues: \"{titulo}\".");
    }

    /// <summary>
    /// Sair do <c>ConfirmAsync</c> sem decidir devolve "nao".
    /// </summary>
    /// <remarks>
    /// O <c>&lt;dialog&gt;</c> nativo fecha no ESC <b>sem avisar o componente</b>. Sem a
    /// interceptacao, a confirmacao sumiria da tela e quem chamou ficaria esperando para sempre
    /// — e o pior: num codigo que so avanca quando a resposta chega, a tela simplesmente trava.
    /// </remarks>
    [SkippableFact]
    public async Task ESC_no_ConfirmAsync_responde_NAO_em_vez_de_sumir()
    {
        Skip.If(BaseUrl is null, "E2E_BASE_URL nao definida — rodando fora do pipeline de E2E.");

        var page = await _browser!.NewPageAsync();
        await page.GotoAsync($"{BaseUrl!.TrimEnd('/')}/componentes/dialog");
        await Assertions.Expect(page.Locator("h1")).ToBeVisibleAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Excluir (ConfirmAsync)" }).ClickAsync();
        await Assertions.Expect(page.GetByRole(AriaRole.Dialog, new() { Name = "Excluir o pedido?" }))
            .ToBeVisibleAsync();

        await page.Keyboard.PressAsync("Escape");

        // A pagina so escreve a resposta quando o await volta — entao ver este texto prova que
        // quem chamou foi respondido, e nao apenas que a caixa sumiu.
        await Assertions.Expect(page.GetByText("cancelou (ou saiu com ESC)")).ToBeVisibleAsync();
    }

    /// <summary>
    /// Mudar uma cor em /fundamentos/paleta repinta o site inteiro, e da para voltar.
    /// </summary>
    /// <remarks>
    /// Este teste mora no E2E, e nao no bUnit, porque o que ele verifica NAO cabe num
    /// componente: a cor sai da pagina, passa pelo servico de tema, vira uma custom property no
    /// elemento raiz e so entao chega a barra lateral. E o navegador que fecha esse circuito.
    ///
    /// <para>
    /// Ele mede a variavel CSS <b>calculada</b>, nao o que a pagina desenhou em si mesma. As
    /// amostras da propria pagina saem de estilo inline e continuariam certas mesmo se o tema
    /// nunca fosse aplicado — seria um verde falso.
    /// </para>
    /// </remarks>
    /// <summary>
    /// Espera generosa para a casca aparecer depois de uma carga completa.
    /// </summary>
    /// <remarks>
    /// O teste da paleta recarrega a pagina tres vezes, e cada recarga paga o boot do WASM
    /// inteiro de novo. Os 5s padrao do Playwright dao conta de um site quente e reprovam um
    /// frio — uma falha que fala do servidor ter acabado de subir, nunca do produto.
    /// </remarks>
    private static LocatorAssertionsToBeVisibleOptions Carregou => new() { Timeout = 30_000 };

    private static LocatorAssertionsToContainTextOptions CarregouTexto => new() { Timeout = 30_000 };

    [SkippableFact]
    public async Task Mudar_a_cor_na_pagina_de_paleta_repinta_o_site_inteiro()
    {
        Skip.If(BaseUrl is null, "E2E_BASE_URL nao definida — rodando fora do pipeline de E2E.");

        const string Primaria = "getComputedStyle(document.documentElement)"
            + ".getPropertyValue('--rvm-color-primary').trim()";

        var page = await _browser!.NewPageAsync();
        await page.GotoAsync($"{BaseUrl!.TrimEnd('/')}/fundamentos/paleta");
        await Assertions.Expect(page.Locator("h1")).ToBeVisibleAsync(Carregou);

        var antes = await page.EvaluateAsync<string>(Primaria);
        Assert.False(string.IsNullOrWhiteSpace(antes), "O site subiu sem --rvm-color-primary.");

        // Pelo campo hexadecimal, nao pelo seletor de cor: o <input type="color"> abre um dialogo
        // do sistema operacional, que o Playwright nao alcanca.
        await page.GetByLabel("Primária, em hexadecimal").FillAsync("#B3261E");
        await page.Keyboard.PressAsync("Tab");   // @onchange do Blazor dispara no blur

        await Assertions.Expect(page.GetByText("O site inteiro está nas suas cores."))
            .ToBeVisibleAsync();

        var depois = await page.EvaluateAsync<string>(Primaria);
        Assert.NotEqual(antes, depois);

        // A cor aplicada e a DERIVADA, nao a digitada: o motor recalcula a luminosidade ate
        // atender AA. Conferir contra a amostra "primary" da propria pagina prova que o site
        // recebeu a paleta inteira, e nao um valor solto.
        var amostra = await page.Locator(".paleta__hex").First.InnerTextAsync();
        Assert.Equal(amostra.Trim(), depois, ignoreCase: true);

        // O seletor da topbar precisa ADMITIR a paleta do visitante. Sem isto ele mostraria
        // "RVM" com o site em vermelho.
        var selecionado = await page.Locator("#seletor-tema option:checked").InnerTextAsync();
        Assert.Contains("sua paleta", selecionado, StringComparison.Ordinal);

        // Uma paleta de fora tambem precisa passar no axe — o portao nao vale so para as cores
        // que nos escolhemos.
        var auditoria = await page.RunAxe();
        var serias = auditoria.Violations
            .Where(v => v.Impact is "serious" or "critical")
            .Select(v => $"{v.Id} ({v.Impact}) — {v.Help}")
            .ToList();

        Assert.True(serias.Count == 0,
            "A paleta do visitante quebrou a acessibilidade da pagina:\n" + string.Join("\n", serias));

        // ---- A paleta e GUARDADA. Foi o defeito que o Rafael achou usando: sair da pagina e
        // voltar trazia o formulario nos valores iniciais, com o site ainda pintado. ----

        var menu = page.GetByRole(AriaRole.Navigation, new() { Name = "Navegação da documentação" });

        await menu.GetByRole(AriaRole.Link, new() { Name = "Button", Exact = true }).ClickAsync();
        await Assertions.Expect(page.Locator("h1")).ToContainTextAsync("RvmButton", CarregouTexto);
        Assert.Equal(depois, await page.EvaluateAsync<string>(Primaria));

        await menu.GetByRole(AriaRole.Link, new() { Name = "Criar sua paleta" }).ClickAsync();
        await Assertions.Expect(page.GetByLabel("Primária, em hexadecimal")).ToHaveValueAsync("#B3261E");
        await Assertions.Expect(page.GetByText("O site inteiro está nas suas cores.")).ToBeVisibleAsync();

        // E sobrevive ao F5 — o que a navegacao entre paginas sozinha nao provaria, porque o
        // servico de tema vive no mesmo circuito.
        await page.ReloadAsync();
        await Assertions.Expect(page.Locator("h1")).ToBeVisibleAsync(Carregou);
        Assert.Equal(depois, await page.EvaluateAsync<string>(Primaria));
        await Assertions.Expect(page.GetByLabel("Primária, em hexadecimal")).ToHaveValueAsync("#B3261E");

        // E da para sair. Sem este caminho, quem experimentasse uma cor ficaria preso nela.
        await page.GetByRole(AriaRole.Button, new() { Name = "Restaurar o tema do site" }).ClickAsync();
        await Assertions.Expect(page.GetByText("O site inteiro está nas suas cores.")).ToBeHiddenAsync();

        Assert.Equal(antes, await page.EvaluateAsync<string>(Primaria));

        // Restaurar tem de APAGAR o que estava guardado. Se so repintasse a tela, o proximo F5
        // traria a paleta de volta e o botao pareceria nao ter funcionado.
        await page.ReloadAsync();
        await Assertions.Expect(page.Locator("h1")).ToBeVisibleAsync(Carregou);
        Assert.Equal(antes, await page.EvaluateAsync<string>(Primaria));
    }

    /// <summary>
    /// As duas aparencias do preview passam no axe.
    /// </summary>
    /// <remarks>
    /// O preview e temporario (DSGN-025) e este teste sai junto com ele. Existe enquanto durar
    /// porque <b>o "marcante" pinta a topbar com <c>primary-container</c></b>, e mudar a cor de
    /// um fundo e exatamente o tipo de mudanca que quebra contraste sem ninguem notar.
    ///
    /// <para>
    /// O portao de contraste da biblioteca <b>nao alcanca isto</b>: ele mede pares da paleta, e
    /// aqui a questao e qual par foi aplicado em qual elemento. So o axe, no navegador, ve isso.
    /// </para>
    /// </remarks>
    [SkippableFact]
    public async Task As_duas_aparencias_do_preview_passam_no_axe()
    {
        Skip.If(BaseUrl is null, "E2E_BASE_URL nao definida — rodando fora do pipeline de E2E.");

        var page = await _browser!.NewPageAsync();
        var problemas = new List<string>();

        foreach (var aparencia in new[] { "sobrio", "marcante", "vivo" })
        {
            // ⚠️ Pelo SELETOR, e nao escrevendo o atributo direto. O "vivo" nao e so CSS: ele
            // troca o TEMA por um derivado com as superficies tingidas, e essa metade acontece
            // no C#. Um teste que so escrevesse o atributo aprovaria uma aparencia pela metade
            // — justamente sem a parte que mexe nas cores.
            await page.GotoAsync(BaseUrl!);
            await Assertions.Expect(page.Locator("h1")).ToBeVisibleAsync(Carregou);
            await page.Locator("#seletor-aparencia").SelectOptionAsync(aparencia);

            foreach (var rota in new[] { "", "/padroes/listagem", "/padroes/dashboard", "/componentes/button" })
            {
                // A escolha sobrevive a navegacao pelo localStorage, e a casca re-aplica o
                // tingimento na entrada — o mesmo caminho que o visitante percorre.
                await page.GotoAsync($"{BaseUrl!.TrimEnd('/')}{rota}");
                await Assertions.Expect(page.Locator("h1")).ToBeVisibleAsync(Carregou);

                var resultado = await page.RunAxe();

                foreach (var v in resultado.Violations.Where(v => v.Impact is "serious" or "critical"))
                {
                    problemas.Add($"[{aparencia}] {(rota.Length == 0 ? "/" : rota)}: {v.Id} ({v.Impact}) — {v.Help}");
                }
            }
        }

        Assert.True(problemas.Count == 0,
            $"{problemas.Count} problema(s) nas aparencias do preview:\n" + string.Join("\n", problemas));
    }
}
