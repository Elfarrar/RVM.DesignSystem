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
}
