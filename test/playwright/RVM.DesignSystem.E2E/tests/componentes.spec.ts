import AxeBuilder from '@axe-core/playwright';
import { expect, test } from '@playwright/test';

// Uma pagina por componente. Estes testes valem para TODA pagina de componente que entrar: a lista
// abaixo cresce a cada fatia da onda 1, e o mesmo par de checagens (renderiza + axe nos dois temas)
// pega o basico sem escrever teste novo do zero.

const PAGINAS = [
    { rota: '/componentes/typography', titulo: 'RvmTypography' },
    { rota: '/componentes/divider', titulo: 'RvmDivider' },
    { rota: '/componentes/icon', titulo: 'RvmIcon' },
    { rota: '/componentes/button', titulo: 'RvmButton' },
    { rota: '/componentes/avatar', titulo: 'RvmAvatar' },
    { rota: '/componentes/chip', titulo: 'RvmChip' },
    { rota: '/componentes/alert', titulo: 'RvmAlert' },
    { rota: '/componentes/card', titulo: 'RvmCard' },
    { rota: '/componentes/text-field', titulo: 'RvmTextField' },
    { rota: '/componentes/checkbox', titulo: 'RvmCheckbox' },
    { rota: '/componentes/radio', titulo: 'RvmRadio' },
    { rota: '/componentes/switch', titulo: 'RvmSwitch' },
    { rota: '/componentes/badge', titulo: 'RvmBadge' },
    { rota: '/componentes/breadcrumbs', titulo: 'RvmBreadcrumbs' },
    { rota: '/componentes/pagination', titulo: 'RvmPagination' },
    { rota: '/componentes/tooltip', titulo: 'RvmTooltip' },
    { rota: '/componentes/tabs', titulo: 'RvmTabs' },
    { rota: '/componentes/menu', titulo: 'RvmMenu' },
    { rota: '/componentes/select', titulo: 'RvmSelect' },
    { rota: '/componentes/progress', titulo: 'RvmProgress' },
    { rota: '/componentes/skeleton', titulo: 'RvmSkeleton' },
    { rota: '/componentes/empty-state', titulo: 'RvmEmptyState' },
    { rota: '/componentes/list', titulo: 'RvmList' },
    { rota: '/componentes/accordion', titulo: 'RvmAccordion' },
    { rota: '/componentes/dialog', titulo: 'RvmDialog' },
    { rota: '/componentes/drawer', titulo: 'RvmDrawer' },
    { rota: '/componentes/snackbar', titulo: 'RvmSnackbar' },
    { rota: '/componentes/rating', titulo: 'RvmRating' },
    { rota: '/componentes/stepper', titulo: 'RvmStepper' },
    { rota: '/componentes/timeline', titulo: 'RvmTimeline' },
    { rota: '/componentes/date-picker', titulo: 'RvmDatePicker' },
    { rota: '/componentes/time-picker', titulo: 'RvmTimePicker' },
    { rota: '/componentes/table', titulo: 'RvmTable' },
    { rota: '/componentes/data-grid', titulo: 'RvmDataGrid' },
    { rota: '/componentes/app-shell', titulo: 'RvmAppShell' },
    { rota: '/componentes/column-chart', titulo: 'RvmColumnChart' },
    { rota: '/componentes/bar-chart', titulo: 'RvmBarChart' },
    { rota: '/componentes/histogram', titulo: 'RvmHistogram' },
];

// Graficos que o kit nao desenhou: seguem o visual das colunas (DSGN-010).
const SEM_PAGINA_NO_KIT = new Set(['RvmIcon', 'RvmProgress', 'RvmSkeleton', 'RvmBarChart', 'RvmHistogram', 'RvmScatterChart']);

test('@smoke o indice de componentes lista o que ja existe', async ({ page }) => {
    await page.goto('/componentes');

    await expect(page.getByRole('heading', { name: 'Componentes', level: 1 })).toBeVisible();
    const indice = page.getByRole('main');
    const menu = page.getByRole('navigation', { name: 'Menu do site' });
    for (const { titulo } of PAGINAS) {
        await expect(indice.getByRole('link', { name: titulo, exact: true })).toBeVisible();
        // O menu lateral leva a toda pagina, com o nome sem o prefixo Rvm.
        await expect(menu.getByRole('link', { name: titulo.slice(3), exact: true })).toBeVisible();
    }
});

for (const { rota, titulo } of PAGINAS) {
    test(`@smoke ${titulo} tem exemplo, parametros e a referencia do kit`, async ({ page }) => {
        await page.goto(rota);

        await expect(page.getByRole('heading', { name: titulo, level: 1 })).toBeVisible();
        await expect(page.getByRole('heading', { name: 'Parametros' })).toBeVisible();
        await expect(page.getByRole('table').last()).toBeVisible();

        // O recorte do kit ao lado do exemplo e criterio do 07-site-de-documentacao: sem ele,
        // "parece o NEATLAB?" vira discussao de memoria. Excecoes declaradas: RvmIcon (o kit exportou
        // os icones rasterizados, ADR-005) e RvmProgress/RvmSkeleton (o kit nao tem pagina deles).
        if (!SEM_PAGINA_NO_KIT.has(titulo)) {
            await expect(page.getByRole('img').first()).toBeVisible();
        }
    });

    for (const tema of ['claro', 'escuro'] as const) {
        test(`${titulo} sem violacao seria de acessibilidade no tema ${tema}`, async ({ page }) => {
            await page.goto(rota);
            await expect(page.getByRole('heading', { name: titulo, level: 1 })).toBeVisible();

            if (tema === 'escuro') {
                const seletor = page.getByRole('button', { name: /Tema/ });
                await seletor.click();
                await expect(seletor).toHaveAttribute('aria-pressed', 'true');
            }

            const resultado = await new AxeBuilder({ page })
                .withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa'])
                // A amostra do token de texto desabilitado reprova contraste por definicao.
                .exclude('[data-rvm-demo="texto-desabilitado"]')
                .analyze();

            const serias = resultado.violations.filter(v => v.impact === 'serious' || v.impact === 'critical');
            expect(serias.flatMap(v => v.nodes.map(n => `${v.id} em ${n.target.join(' ')}`))).toEqual([]);
        });
    }
}

test('o divisor com rotulo continua sendo um separador para o leitor de tela', async ({ page }) => {
    await page.goto('/componentes/divider');

    // <hr> nao aceita conteudo, entao a versao com rotulo troca de elemento. O que nao pode mudar
    // e o PAPEL: se o separador virar um div qualquer, o leitor de tela perde a informacao.
    const separadores = page.getByRole('separator');
    await expect(separadores.first()).toBeVisible();
    expect(await separadores.count()).toBeGreaterThanOrEqual(4);
    await expect(page.getByRole('separator').filter({ hasText: 'ou' })).toHaveCount(1);
});

// O axe NAO mede o contraste do indicador de foco. Foi medido a mao na DSGN-003: no tema escuro o
// anel saia em primary-main e dava 1.84:1 sobre o papel — some para quem navega por teclado.
// WCAG 2.4.11 / 1.4.11 pedem 3:1 contra o que esta em volta.
function luminancia(rgb: string) {
    const [r, g, b] = rgb.match(/\d+/g)!.slice(0, 3).map(Number).map(v => {
        const c = v / 255;
        return c <= 0.03928 ? c / 12.92 : ((c + 0.055) / 1.055) ** 2.4;
    });
    return 0.2126 * r + 0.7152 * g + 0.0722 * b;
}

function contraste(a: string, b: string) {
    const [x, y] = [luminancia(a), luminancia(b)].sort((m, n) => n - m);
    return (x + 0.05) / (y + 0.05);
}

for (const tema of ['claro', 'escuro'] as const) {
    test(`o anel de foco do botao passa 3:1 no tema ${tema}`, async ({ page }) => {
        await page.goto('/componentes/button');
        await expect(page.getByRole('heading', { name: 'RvmButton', level: 1 })).toBeVisible();

        if (tema === 'escuro') {
            const seletor = page.getByRole('button', { name: /Tema/ });
            await seletor.click();
            await expect(seletor).toHaveAttribute('aria-pressed', 'true');
        }

        // Foco POR TECLADO: o anel e `:focus-visible`, que o foco programatico nem sempre acende.
        await page.locator('main button').first().focus();
        await page.keyboard.press('Shift+Tab');
        await page.keyboard.press('Tab');

        const { anel, fundo } = await page.evaluate(() => {
            const el = document.activeElement as HTMLElement;
            let pai: HTMLElement | null = el.parentElement;
            let cor = 'rgba(0, 0, 0, 0)';
            while (pai && /rgba\(0, 0, 0, 0\)|transparent/.test(cor)) {
                cor = getComputedStyle(pai).backgroundColor;
                pai = pai.parentElement;
            }
            return { anel: getComputedStyle(el).outlineColor, fundo: cor };
        });

        expect(contraste(anel, fundo)).toBeGreaterThanOrEqual(3);
    });
}

test('campo de texto: validacao do EditForm aparece no lugar do apoio e marca aria-invalid', async ({ page }) => {
    await page.goto('/componentes/text-field');
    await expect(page.getByRole('heading', { name: 'RvmTextField', level: 1 })).toBeVisible();

    await page.getByRole('button', { name: 'Enviar' }).click();

    const nome = page.getByLabel('Nome completo');
    await expect(nome).toHaveAttribute('aria-invalid', 'true');
    await expect(page.getByText('Informe o nome completo para continuar.')).toBeVisible();
});

test('campo de texto: clicar no rotulo foca o campo e o rotulo flutua', async ({ page }) => {
    await page.goto('/componentes/text-field');
    await expect(page.getByRole('heading', { name: 'RvmTextField', level: 1 })).toBeVisible();

    const campo = page.getByLabel('Peso da carga');
    // O campo ja tem valor, entao o rotulo esta FLUTUANDO sobre a borda. E o caso que quebrou: com
    // pointer-events: none no rotulo, o clique caia na borda e o campo nao recebia foco.
    await page.locator('label', { hasText: 'Peso da carga' }).click();

    await expect(campo).toBeFocused();
});

// O `name` do RvmTextField NAO e testado aqui, de proposito: este site e WebAssembly, e no navegador o
// EditContext desliga a geracao de nomes de campo (nao existe POST). O cenario que importa — formulario
// em SSR estatico — e coberto no bUnit, que roda fora do navegador pelo mesmo caminho do Blazor Server
// (RvmTextFieldTests.Renderiza_name_que_o_formulario_em_SSR_estatico_exige).

// --- Controles: o padrao de teclado de cada um, testado de verdade ---

test('checkbox: espaco marca e desmarca, e clicar no texto tambem', async ({ page }) => {
    await page.goto('/componentes/checkbox');
    await expect(page.getByRole('heading', { name: 'RvmCheckbox', level: 1 })).toBeVisible();

    const medio = page.getByRole('checkbox', { name: 'Medio' });
    await expect(medio).toBeChecked();
    await medio.focus();
    await page.keyboard.press('Space');
    await expect(medio).not.toBeChecked();

    await page.getByText('Medio', { exact: true }).click();
    await expect(medio).toBeChecked();
});

test('checkbox: a caixa-mae fica indeterminada quando so alguns filhos estao marcados', async ({ page }) => {
    await page.goto('/componentes/checkbox');
    const mae = page.getByRole('checkbox', { name: 'Todos os talhoes' });
    await expect(mae).toBeVisible();

    // Espera por SINAL: a propriedade `indeterminate` e aplicada pelo modulo JS depois do render.
    await expect.poll(() => mae.evaluate(el => (el as HTMLInputElement).indeterminate)).toBe(true);

    await page.getByRole('checkbox', { name: 'Talhao 12' }).check();
    await expect(mae).toBeChecked();
    await expect.poll(() => mae.evaluate(el => (el as HTMLInputElement).indeterminate)).toBe(false);
});

test('radio: as setas movem a escolha dentro do grupo', async ({ page }) => {
    await page.goto('/componentes/radio');
    const grupo = page.getByRole('group', { name: 'Forma de pagamento' });
    await expect(grupo).toBeVisible();

    await grupo.getByRole('radio', { name: 'Pix' }).focus();
    await page.keyboard.press('ArrowDown');

    await expect(grupo.getByRole('radio', { name: 'Boleto' })).toBeChecked();
    await expect(page.getByText(/Escolhido: Boleto/)).toBeVisible();
});

test('radio: grupo desabilitado nao deixa escolher', async ({ page }) => {
    await page.goto('/componentes/radio');
    const grupo = page.getByRole('group', { name: 'Desabilitado' });

    await expect(grupo.getByRole('radio', { name: 'Pix' })).toBeDisabled();
    await expect(grupo.getByRole('radio', { name: 'Boleto' })).toBeDisabled();
});

test('switch: anuncia o papel de switch e liga pelo espaco', async ({ page }) => {
    await page.goto('/componentes/switch');
    const chave = page.getByRole('switch', { name: 'Modo compacto' });
    await expect(chave).not.toBeChecked();

    await chave.focus();
    await page.keyboard.press('Space');

    await expect(chave).toBeChecked();
});

test('paginacao: clicar e usar as setas move a pagina atual', async ({ page }) => {
    await page.goto('/componentes/pagination');
    const paginacao = page.getByRole('navigation', { name: 'Paginacao texto circular' });
    await expect(paginacao.getByRole('button', { name: 'Pagina 1', exact: true })).toHaveAttribute('aria-current', 'page');
    await expect(paginacao.getByRole('button', { name: 'Pagina anterior' })).toBeDisabled();

    await paginacao.getByRole('button', { name: 'Pagina 2', exact: true }).click();
    await expect(paginacao.getByRole('button', { name: 'Pagina 2', exact: true })).toHaveAttribute('aria-current', 'page');

    await paginacao.getByRole('button', { name: 'Proxima pagina' }).focus();
    await page.keyboard.press('Enter');
    await expect(paginacao.getByRole('button', { name: 'Pagina 3', exact: true })).toHaveAttribute('aria-current', 'page');
    await expect(page.getByText('Pagina 3 de 13.')).toBeVisible();
});

test('trilha: o ultimo passo e a pagina atual e os anteriores sao links', async ({ page }) => {
    await page.goto('/componentes/breadcrumbs');
    const trilha = page.getByRole('navigation', { name: 'Trilha com barra', exact: true });

    await expect(trilha.getByRole('listitem')).toHaveCount(3);
    await expect(trilha.locator('[aria-current="page"]')).toHaveText('Breadcrumbs');
    await expect(trilha.getByRole('link', { name: 'Componentes' })).toBeVisible();
});

test('tooltip: aparece no foco, vira descricao do botao e some com Esc', async ({ page }) => {
    await page.goto('/componentes/tooltip');
    const botao = page.getByRole('button', { name: 'Abaixo' });
    const dica = page.getByRole('tooltip', { name: 'Dica abaixo' });
    await expect(botao).toHaveAccessibleDescription('Dica abaixo');
    await expect(dica).toBeHidden();

    await botao.focus();
    await expect(dica).toBeVisible();

    await page.keyboard.press('Escape');
    await expect(dica).toBeHidden();
});
