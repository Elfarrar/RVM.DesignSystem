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
