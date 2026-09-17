import AxeBuilder from '@axe-core/playwright';
import { readFileSync } from 'node:fs';
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

// Exportar (DSGN-011): o arquivo so existe depois de passar pelo navegador — canvas, Blob e download.
// Nenhum teste unitario alcanca isso; aqui a prova e o arquivo que cai no disco.
test('exportar entrega os quatro arquivos do grafico', async ({ page }) => {
    await page.goto('/componentes/column-chart');
    await expect(page.getByRole('heading', { name: 'RvmColumnChart', level: 1 })).toBeVisible();

    const baixar = async (item: string) => {
        await page.getByRole('button', { name: 'Exportar' }).click();
        const espera = page.waitForEvent('download');
        await page.getByRole('menuitem', { name: item }).click();
        const arquivo = await espera;
        const caminho = await arquivo.path();
        return { nome: arquivo.suggestedFilename(), bytes: readFileSync(caminho!) };
    };

    const csv = await baixar('Dados CSV');
    expect(csv.nome).toBe('receita-e-despesa-por-ano.csv');
    expect(csv.bytes.toString('utf8')).toContain('Categoria;Receita;Despesa');
    expect(csv.bytes.toString('utf8')).toContain('2022;45.000;26.000');

    const svg = await baixar('Vetor SVG');
    expect(svg.nome).toBe('receita-e-despesa-por-ano.svg');
    // Cores embutidas: um SVG salvo sem elas abre sem tema em qualquer editor.
    expect(svg.bytes.toString('utf8')).toMatch(/<svg[^>]+xmlns="http:\/\/www.w3.org\/2000\/svg"/);
    expect(svg.bytes.toString('utf8')).toContain('style="fill:rgb(');

    const png = await baixar('Imagem PNG');
    expect(png.nome).toBe('receita-e-despesa-por-ano.png');
    expect(png.bytes.subarray(0, 8)).toEqual(Buffer.from([0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a]));

    const pdf = await baixar('PDF');
    expect(pdf.nome).toBe('receita-e-despesa-por-ano.pdf');
    expect(pdf.bytes.subarray(0, 8).toString('latin1')).toBe('%PDF-1.4');
    expect(pdf.bytes.toString('latin1')).toContain('/Filter /DCTDecode');
    expect(pdf.bytes.toString('latin1')).toContain('%%EOF');

    await expect(page.getByRole('status').filter({ hasText: 'PDF' })).toBeVisible();
});

// Eixo duplo (DSGN-011): duas unidades no mesmo grafico sem que a serie pequena vire um risco no chao.
test('eixo duplo: escala propria a direita, e a legenda diz qual serie le onde', async ({ page }) => {
    await page.goto('/componentes/line-chart');
    const figura = page.locator('figure.rvm-grafico').filter({ hasText: 'eixo direito' });

    await expect(figura.getByText('eixo esquerdo')).toBeVisible();
    const rotulosEmPorcento = figura.locator('text', { hasText: /^\d+(,\d+)?%$/ });
    await expect(rotulosEmPorcento.first()).toBeVisible();

    // Os rotulos em porcento ficam a direita dos rotulos em reais (x no sistema do proprio SVG).
    const direita = Number(await rotulosEmPorcento.first().getAttribute('x'));
    const esquerda = Number(await figura.locator('text', { hasText: /mil$/ }).first().getAttribute('x'));
    expect(esquerda).toBeLessThan(direita);

    // A tabela do leitor de tela diz de que eixo o numero veio.
    await expect(figura.getByRole('columnheader', { name: 'Margem (eixo direito)' })).toBeAttached();

    await page.getByRole('group', { name: 'Receita e margem por mes' }).focus();
    await page.keyboard.press('Home');
    await expect(page.locator('[aria-live=polite]').filter({ hasText: 'Margem' })).toHaveText('Jan: Receita 128 mil; Margem 11,5%');
});

// Zoom e arrastar (DSGN-011): a roda so cancela a rolagem da pagina num ouvinte nao passivo, coisa que
// nenhum teste unitario alcanca. Aqui a prova e o desenho mudar e a pagina ficar parada.
test('zoom: roda aproxima sem rolar a pagina, teclado tambem, e o duplo clique volta', async ({ page }) => {
    await page.goto('/componentes/line-chart');
    const grafico = page.getByRole('group', { name: 'Chuva mensal por fazenda' });
    const figura = page.locator('figure.rvm-grafico').filter({ hasText: 'Boa Vista' });
    const meses = () => figura.locator('text').filter({ hasText: /^[A-Z][a-z]{2}$/ }).count();

    await expect(grafico).toBeVisible();
    await expect.poll(meses).toBeGreaterThan(0);
    const inteiro = await meses();
    const rolagem = await page.evaluate(() => window.scrollY);
    await grafico.hover();
    await page.mouse.wheel(0, -200);

    await expect.poll(meses).toBeLessThan(inteiro);
    expect(await page.evaluate(() => window.scrollY)).toBe(rolagem);
    await expect(figura.locator('[aria-live=polite]').filter({ hasText: 'Mostrando de' })).toBeVisible();

    // Duplo clique volta ao grafico inteiro.
    await grafico.dblclick();
    await expect.poll(meses).toBe(inteiro);

    // E o mesmo pelo teclado, sem mouse nenhum.
    await grafico.focus();
    await page.keyboard.press('+');
    await expect.poll(meses).toBeLessThan(inteiro);
    await page.keyboard.press('0');
    await expect.poll(meses).toBe(inteiro);
    await expect(figura.locator('[aria-live=polite]').filter({ hasText: 'Grafico inteiro a vista' })).toBeVisible();
});
