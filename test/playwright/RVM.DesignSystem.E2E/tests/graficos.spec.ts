import AxeBuilder from '@axe-core/playwright';
import { expect, test, type Page } from '@playwright/test';

// Graficos (DSGN-010): o SVG e so desenho; a prova e o que o leitor de tela e o teclado recebem.

async function semViolacaoSeria(page: Page) {
    const resultado = await new AxeBuilder({ page }).withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa']).analyze();
    const serias = resultado.violations.filter(v => v.impact === 'serious' || v.impact === 'critical');
    expect(serias.map(v => `${v.id}: ${v.nodes.map(n => n.target.join(' ')).join(', ')}`)).toEqual([]);
}

test('colunas: as setas percorrem os anos e anunciam os valores', async ({ page }) => {
    await page.goto('/componentes/column-chart');
    const grafico = page.getByRole('group', { name: 'Receita e despesa por ano' });

    await grafico.focus();
    await page.keyboard.press('ArrowRight');
    const anuncio = page.locator('[aria-live=polite]').filter({ hasText: '2019' });
    await expect(anuncio).toHaveText('2019: Receita 28 mil; Despesa 18 mil');
    await page.keyboard.press('End');
    await expect(page.locator('[aria-live=polite]').filter({ hasText: '2025' })).toHaveText('2025: Receita 58,5 mil; Despesa 31,5 mil');

    // A tabela escondida tem os mesmos numeros.
    await expect(page.getByRole('table', { name: 'Dados do grafico: Receita e despesa por ano' }).getByRole('row', { name: /2022/ })).toContainText('45 mil');
});

test('colunas: o SVG sai na largura real e a dica acompanha o mouse', async ({ page }) => {
    await page.goto('/componentes/column-chart');
    const figura = page.locator('figure.rvm-grafico').first();
    const area = figura.locator('.rvm-grafico-area');
    const largura = Math.round((await area.boundingBox())!.width);
    await expect(figura.locator('svg')).toHaveAttribute('viewBox', `0 0 ${largura} 300`);

    const caixa = (await area.boundingBox())!;
    await page.mouse.move(caixa.x + caixa.width - 40, caixa.y + caixa.height / 2);
    await expect(figura.locator('.rvm-grafico-dica-titulo')).toHaveText('2025');
    await page.mouse.move(0, 0);
    await expect(figura.locator('.rvm-grafico-dica')).toHaveCount(0);
});

test('histograma e barras respondem ao teclado', async ({ page }) => {
    await page.goto('/componentes/histogram');
    await page.getByRole('group', { name: 'Distribuicao do peso dos bovinos' }).focus();
    await page.keyboard.press('Home');
    await expect(page.locator('[aria-live=polite]').filter({ hasText: 'Animais' }).first()).toHaveText(/^\d+ kg a \d+ kg: Animais \d+$/);

    await page.goto('/componentes/bar-chart');
    await page.getByRole('group', { name: 'Produtividade por talhao' }).focus();
    await page.keyboard.press('ArrowDown');
    await expect(page.locator('[aria-live=polite]').filter({ hasText: 'Talhao Norte' })).toHaveText('Talhao Norte (pivo 1): Produtividade 72 sc/ha');
});

for (const tema of ['claro', 'escuro'] as const) {
    test(`grafico com dica aberta sem violacao seria no tema ${tema}`, async ({ page }) => {
        await page.goto('/componentes/column-chart');
        await expect(page.getByRole('heading', { name: 'RvmColumnChart', level: 1 })).toBeVisible();
        if (tema === 'escuro') {
            await page.getByRole('button', { name: /Tema/ }).click();
        }

        await page.getByRole('group', { name: 'Receita e despesa por ano' }).focus();
        await page.keyboard.press('ArrowRight');
        await expect(page.locator('.rvm-grafico-dica').first()).toBeVisible();
        await semViolacaoSeria(page);
    });
}

test('rosca, radar e area anunciam o ponto ativo', async ({ page }) => {
    await page.goto('/componentes/pie-chart');
    await page.getByRole('group', { name: 'Area plantada por cultura' }).focus();
    await page.keyboard.press('ArrowRight');
    await expect(page.locator('[aria-live=polite]').filter({ hasText: 'Soja' })).toHaveText('Soja: 620 ha (50%)');

    await page.goto('/componentes/radar-chart');
    await page.getByRole('group', { name: 'Avaliacao dos talhoes por criterio' }).focus();
    await page.keyboard.press('End');
    await expect(page.locator('[aria-live=polite]').filter({ hasText: 'Sanidade' })).toHaveText('Sanidade: Talhao Norte 8,5; Varzea 6,5');

    await page.goto('/componentes/area-chart');
    await page.getByRole('group', { name: 'Vendas acumuladas no ano' }).focus();
    await page.keyboard.press('ArrowLeft');
    await expect(page.locator('[aria-live=polite]').filter({ hasText: 'Jul' })).toHaveText('Jul: Vendas R$ 42,6 mil');
});
