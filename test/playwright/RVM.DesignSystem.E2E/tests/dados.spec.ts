import AxeBuilder from '@axe-core/playwright';
import { expect, test, type Page } from '@playwright/test';

async function semViolacaoSeria(page: Page) {
    const resultado = await new AxeBuilder({ page }).withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa']).analyze();
    const serias = resultado.violations.filter(v => v.impact === 'serious' || v.impact === 'critical');
    expect(serias.map(v => `${v.id}: ${v.nodes.map(n => n.target.join(' ')).join(', ')}`)).toEqual([]);
}

test('data: o calendario abre com o foco no dia, anda pelo teclado e escolher fecha', async ({ page }) => {
    await page.goto('/componentes/date-picker');
    const campo = page.getByRole('button', { name: /Data de entrega/ });

    await campo.click();
    const dialogo = page.getByRole('dialog', { name: 'Escolher Data de entrega' });
    await expect(dialogo).toBeVisible();
    const focado = dialogo.locator('button.rvm-dia[tabindex="0"]');
    await expect(focado).toBeFocused();

    // Tab nao escapa do dialogo modal.
    for (let i = 0; i < 4; i++) {
        await page.keyboard.press('Tab');
        await expect.poll(() => dialogo.evaluate(el => el.contains(document.activeElement))).toBe(true);
    }

    await focado.focus();
    await page.keyboard.press('PageDown');
    await expect(dialogo.locator('button.rvm-dia[tabindex="0"]')).toBeFocused();
    const nome = await dialogo.locator('button.rvm-dia[tabindex="0"]').getAttribute('aria-label');
    // Fim de semana esta bloqueado: anda ate um dia util antes de escolher.
    if (/sabado|domingo/.test(nome ?? '')) {
        await page.keyboard.press(nome!.startsWith('sabado') ? 'ArrowLeft' : 'ArrowRight');
    }
    await page.keyboard.press('Enter');

    await expect(dialogo).toBeHidden();
    await expect(campo).toBeFocused();
    await expect(campo).toHaveAccessibleName(/Data de entrega \d{2}\/\d{2}\/\d{4}/);
});

test('data: Esc fecha e salvar vazio mostra a validacao', async ({ page }) => {
    await page.goto('/componentes/date-picker');
    const campo = page.getByRole('button', { name: /Data do plantio/ });

    await campo.click();
    await expect(page.getByRole('dialog')).toBeVisible();
    await page.keyboard.press('Escape');
    await expect(page.getByRole('dialog')).toBeHidden();
    await expect(campo).toBeFocused();

    await page.getByRole('button', { name: 'Salvar plantio' }).click();
    await expect(campo).toHaveAttribute('aria-invalid', 'true');
    await expect(campo).toHaveAccessibleDescription('Escolha a data do plantio para salvar.');
});

test('horario: escolher pela lista liga o valor', async ({ page }) => {
    await page.goto('/componentes/time-picker');
    const campo = page.getByRole('combobox', { name: 'Inicio da aplicacao' });

    await campo.click();
    await page.getByRole('textbox', { name: 'Buscar horario' }).fill('18');
    await page.getByRole('option', { name: '18:15' }).click();

    await expect(campo).toHaveText('18:15');
    await expect(page.getByText('Inicio: 18:15.')).toBeVisible();
});

for (const tema of ['claro', 'escuro'] as const) {
    test(`calendario aberto sem violacao seria no tema ${tema}`, async ({ page }) => {
        await page.goto('/componentes/date-picker');
        await expect(page.getByRole('heading', { name: 'RvmDatePicker', level: 1 })).toBeVisible();
        if (tema === 'escuro') {
            await page.getByRole('button', { name: /Tema/ }).click();
        }

        await page.getByRole('button', { name: /Periodo da safra/ }).click();
        const dialogo = page.getByRole('dialog');
        await dialogo.locator('button.rvm-dia').nth(9).click();
        await expect(dialogo).toBeVisible();
        await semViolacaoSeria(page);
    });
}

// Onda 4 — dados e shell. O bUnit prova a marcacao; aqui, teclado e foco de verdade.

test('avaliacao: setas mudam a nota pelo teclado, com o grupo nomeado', async ({ page }) => {
    await page.goto('/componentes/rating');
    const grupo = page.getByRole('group', { name: 'Nota do atendimento' });
    const quatro = grupo.getByRole('radio', { name: '4 estrelas de 5' });
    await expect(quatro).toBeChecked();

    await quatro.focus();
    await page.keyboard.press('ArrowLeft');

    await expect(grupo.getByRole('radio', { name: '3 estrelas de 5' })).toBeChecked();
    await expect(page.getByText(/Atendimento: 3 de 5/)).toBeVisible();
});

test('avaliacao: somente leitura e uma imagem com a nota por extenso', async ({ page }) => {
    await page.goto('/componentes/rating');

    await expect(page.getByRole('img', { name: 'Nota grande: 2,5 de 5' })).toBeVisible();
});

test('etapas: avancar move a etapa atual e o estado vai em texto', async ({ page }) => {
    await page.goto('/componentes/stepper');
    const etapas = page.getByRole('list', { name: 'Cadastro da fazenda' });
    await expect(etapas.locator('[aria-current="step"]')).toContainText('Dados da fazenda');

    await page.getByRole('button', { name: 'Avancar' }).click();

    await expect(etapas.locator('[aria-current="step"]')).toContainText('Talhoes');
    await expect(etapas.getByRole('listitem').nth(1)).toContainText('concluida');
});

test('linha do tempo: lista ordenada com os acontecimentos na ordem', async ({ page }) => {
    await page.goto('/componentes/timeline');
    const linha = page.getByRole('list', { name: 'Safra com datas' });

    await expect(linha.getByRole('listitem')).toHaveCount(3);
    await expect(linha.getByRole('listitem').first()).toContainText('Plantio concluido');
});

test('tabela: ordena pelo teclado e a caixa do cabecalho marca todas', async ({ page }) => {
    await page.goto('/componentes/table');
    const tabela = page.getByRole('table', { name: 'Talhoes da fazenda' });
    const area = tabela.getByRole('columnheader', { name: 'Area (ha)' });

    await area.getByRole('button').focus();
    await page.keyboard.press('Enter');
    await expect(area).toHaveAttribute('aria-sort', 'ascending');
    await expect(tabela.getByRole('row').nth(1).getByRole('cell').first()).toHaveText('Sede');
    await page.keyboard.press('Enter');
    await expect(area).toHaveAttribute('aria-sort', 'descending');
    await expect(tabela.getByRole('row').nth(1).getByRole('cell').first()).toHaveText('Cerrado');

    const selecao = page.getByRole('table', { name: 'Talhoes para a aplicacao' });
    await selecao.getByRole('checkbox', { name: 'Selecionar todas as linhas' }).check();
    await expect(selecao.getByRole('checkbox', { name: /^Selecionar (?!todas)/ })).toHaveCount(5);
    for (const caixa of await selecao.getByRole('checkbox', { name: /^Selecionar (?!todas)/ }).all()) {
        await expect(caixa).toBeChecked();
    }
    await expect(page.getByText(/^5 marcado\(s\), 458[.,]5 ha\.$/)).toBeVisible();
});

test('grade: filtra por coluna, pagina e anuncia o intervalo', async ({ page }) => {
    await page.goto('/componentes/data-grid');
    const grade = page.getByRole('table', { name: 'Talhoes da fazenda' });
    const regiao = page.getByRole('region', { name: 'Talhoes da fazenda' }).locator('..');
    const intervalo = regiao.getByRole('status');

    await expect(intervalo).toHaveText('1–5 de 13');
    await regiao.getByRole('button', { name: 'Proxima pagina' }).click();
    await expect(intervalo).toHaveText('6–10 de 13');

    await grade.getByRole('searchbox', { name: 'Filtrar por Cultura' }).fill('soja');
    await expect(intervalo).toHaveText('1–4 de 4');
    await expect(grade.getByRole('row')).toHaveCount(2 + 4);

    await grade.getByRole('searchbox', { name: 'Filtrar por Cultura' }).fill('');
    await regiao.getByLabel('Linhas por pagina:').selectOption('10');
    await expect(intervalo).toHaveText('1–10 de 13');
});

for (const tema of ['claro', 'escuro'] as const) {
    test(`grade com linha marcada e filtro sem violacao seria no tema ${tema}`, async ({ page }) => {
        await page.goto('/componentes/data-grid');
        await expect(page.getByRole('heading', { name: 'RvmDataGrid', level: 1 })).toBeVisible();
        if (tema === 'escuro') {
            await page.getByRole('button', { name: /Tema/ }).click();
        }

        await page.getByRole('checkbox', { name: 'Hilda Rath' }).check();
        await page.getByRole('searchbox', { name: 'Filtrar por Talhao' }).first().fill('a');
        await semViolacaoSeria(page);
    });
}

test('moldura: recolher deixa so os icones com o nome dos links intacto', async ({ page }) => {
    await page.goto('/componentes/app-shell');
    const menu = page.getByRole('navigation', { name: 'Menu do exemplo', exact: true });
    const inicio = menu.getByRole('link', { name: 'Inicio' });
    await expect(inicio).toHaveAttribute('aria-current', 'page');
    const larguraAberta = (await inicio.boundingBox())!.width;

    const recolher = page.getByRole('region', { name: 'Menu fixo, recolhivel' }).getByRole('button', { name: 'Recolher menu' });
    await recolher.click();
    await expect(recolher).toHaveAttribute('aria-pressed', 'true');
    await expect(inicio).toHaveAccessibleName('Inicio');
    await expect.poll(async () => (await inicio.boundingBox())!.width).toBeLessThan(larguraAberta / 2);

    await menu.getByRole('button', { name: 'Lavouras' }).click();
    await expect(recolher).toHaveAttribute('aria-pressed', 'false');
    await expect(menu.getByRole('link', { name: 'Talhoes' })).toBeVisible();
});

test('moldura estreita: a gaveta prende o foco, fecha com Esc e devolve o foco', async ({ page }) => {
    await page.goto('/componentes/app-shell');
    const menu = page.getByRole('navigation', { name: 'Menu do exemplo estreito' });
    await expect(menu).toBeHidden();

    const abrir = page.getByRole('region', { name: 'Tela estreita' }).getByRole('button', { name: 'Abrir menu' });
    await abrir.click();
    await expect(abrir).toHaveAttribute('aria-expanded', 'true');
    await expect(menu).toBeVisible();
    const lateral = page.locator('aside', { has: menu });
    await expect(lateral).toBeFocused();

    for (let i = 0; i < 12; i++) {
        await page.keyboard.press('Tab');
        await expect.poll(() => lateral.evaluate(el => el.contains(document.activeElement))).toBe(true);
    }

    await page.keyboard.press('Escape');
    await expect(menu).toBeHidden();
    await expect(abrir).toBeFocused();
});

for (const tema of ['claro', 'escuro'] as const) {
    test(`moldura recolhida e gaveta aberta sem violacao seria no tema ${tema}`, async ({ page }) => {
        await page.goto('/componentes/app-shell');
        await expect(page.getByRole('heading', { name: 'RvmAppShell', level: 1 })).toBeVisible();
        if (tema === 'escuro') {
            await page.getByRole('button', { name: /Tema/ }).click();
        }

        await page.getByRole('region', { name: 'Menu fixo, recolhivel' }).getByRole('button', { name: 'Recolher menu' }).click();
        await page.getByRole('region', { name: 'Tela estreita' }).getByRole('button', { name: 'Abrir menu' }).click();
        await expect(page.getByRole('navigation', { name: 'Menu do exemplo estreito' })).toBeVisible();
        await semViolacaoSeria(page);
    });
}

test('relogio: teclado escolhe hora e minuto, Enter confirma e devolve o foco', async ({ page }) => {
    await page.goto('/componentes/time-picker');
    const campo = page.getByRole('button', { name: /Inicio da colheita/ });

    await campo.click();
    const dialogo = page.getByRole('dialog', { name: 'Escolher Inicio da colheita' });
    const mostrador = dialogo.getByRole('listbox', { name: 'Horas' });
    await expect(mostrador).toBeFocused();

    await page.keyboard.press('ArrowUp');
    await page.keyboard.press('ArrowUp');
    await expect(dialogo.getByRole('option', { name: '13 horas' })).toHaveAttribute('aria-selected', 'true');
    await page.keyboard.press('Enter');
    await expect(dialogo.getByRole('listbox', { name: 'Minutos' })).toBeFocused();
    await page.keyboard.press('ArrowUp');
    await page.keyboard.press('Enter');

    await expect(dialogo).toBeHidden();
    await expect(campo).toHaveAccessibleName(/Inicio da colheita 13:01/);
    await expect(campo).toBeFocused();
    await expect(page.getByText(/Colheita: 13:01/)).toBeVisible();
});

test('relogio: clicar no mostrador escolhe e CANCELAR descarta', async ({ page }) => {
    await page.goto('/componentes/time-picker');
    const campo = page.getByRole('button', { name: /Visita tecnica/ });

    await campo.click();
    const dialogo = page.getByRole('dialog');
    const mostrador = dialogo.getByRole('listbox', { name: 'Horas' });
    await expect(mostrador).toBeFocused();
    // Clique a direita do centro: 3 horas (da tarde, a janela e das 7 as 18); soltar leva aos minutos.
    await mostrador.click({ position: { x: 238, y: 130 } });
    await expect(dialogo.getByRole('button', { name: 'Horas: 03' })).toBeVisible();
    await expect(dialogo.getByRole('listbox', { name: 'Minutos' })).toBeVisible();

    await dialogo.getByRole('button', { name: 'Cancelar' }).click();
    await expect(dialogo).toBeHidden();
    await expect(campo).toHaveAccessibleName('Visita tecnica hh:mm');
});

for (const tema of ['claro', 'escuro'] as const) {
    test(`relogio aberto sem violacao seria no tema ${tema}`, async ({ page }) => {
        await page.goto('/componentes/time-picker');
        await expect(page.getByRole('heading', { name: 'RvmTimePicker', level: 1 })).toBeVisible();
        if (tema === 'escuro') {
            await page.getByRole('button', { name: /Tema/ }).click();
        }

        await page.getByRole('button', { name: /Visita tecnica/ }).click();
        await expect(page.getByRole('dialog').getByRole('listbox')).toBeFocused();
        await page.keyboard.press('ArrowUp');
        await semViolacaoSeria(page);
    });
}
