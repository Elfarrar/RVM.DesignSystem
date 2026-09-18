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

// Selecao de faixa (DSGN-011): arrastar sobre o grafico filtra a tabela ao lado — e o mesmo existe pelo
// teclado, que e onde a maioria das bibliotecas de grafico deixa o usuario de fora.
test('selecao: arrastar no grafico filtra a tabela, e o teclado faz o mesmo', async ({ page }) => {
    await page.goto('/exemplos/dashboard');
    const grafico = page.getByRole('group', { name: 'Receita e despesa por mes' });
    const fechamento = page.getByRole('table', { name: /Fechamento/ });
    await expect(grafico).toBeVisible();
    await expect(fechamento.getByRole('row')).toHaveCount(8); // cabecalho + 7 meses

    // O mouse do Playwright nao rola a pagina: o grafico precisa estar na viewport antes do arrasto.
    await grafico.scrollIntoViewIfNeeded();
    const caixa = (await grafico.boundingBox())!;
    await page.mouse.move(caixa.x + caixa.width * 0.35, caixa.y + caixa.height / 2);
    await page.mouse.down();
    await page.mouse.move(caixa.x + caixa.width * 0.65, caixa.y + caixa.height / 2, { steps: 8 });
    await page.mouse.up();

    // Quantos meses caem na faixa depende da largura da tela; o que importa e a tabela encolher.
    await expect.poll(() => fechamento.getByRole('row').count()).toBeLessThan(8);
    await expect(page.locator('.rvm-grafico-faixa-marcada')).toBeVisible();
    await expect(page.getByText(/Fechamento de \w+ a \w+/)).toBeVisible();

    await page.getByRole('button', { name: 'Limpar filtro' }).click();
    await expect(fechamento.getByRole('row')).toHaveCount(8);

    // Pelo teclado: Shift com as setas marca, e o leitor de tela ouve o que ficou selecionado.
    await grafico.focus();
    await page.keyboard.press('Home');
    await page.keyboard.press('Shift+ArrowRight');
    await expect(fechamento.getByRole('row')).toHaveCount(3);
    await expect(page.locator('[aria-live=polite]').filter({ hasText: 'Selecionado de' }))
        .toHaveText(/Selecionado de \w+ a \w+: 2 de 7\./);

    await page.keyboard.press('Escape');
    await expect(fechamento.getByRole('row')).toHaveCount(8);
});

// DSGN-013: planilha, zoom por caixa e o segundo eixo das barras (que vai para CIMA, nao para a direita).
test('exportar planilha: o xlsx sai como ZIP de XML com numero de verdade', async ({ page }) => {
    await page.goto('/componentes/column-chart');
    await expect(page.getByRole('heading', { name: 'RvmColumnChart', level: 1 })).toBeVisible();

    await page.getByRole('button', { name: 'Exportar' }).click();
    const espera = page.waitForEvent('download');
    await page.getByRole('menuitem', { name: 'Planilha XLSX' }).click();
    const arquivo = await espera;

    expect(arquivo.suggestedFilename()).toBe('receita-e-despesa-por-ano.xlsx');
    const bytes = readFileSync((await arquivo.path())!);
    // Assinatura de ZIP ("PK") e a parte que o Excel exige.
    expect(bytes.subarray(0, 2).toString('latin1')).toBe('PK');
    expect(bytes.toString('latin1')).toContain('xl/worksheets/sheet1.xml');
});

test('zoom por caixa: arrastar desenha a area e o grafico aproxima nela', async ({ page }) => {
    await page.goto('/componentes/scatter-chart');
    const grafico = page.getByRole('group', { name: 'Adubacao e produtividade por talhao' });
    await expect(grafico).toBeVisible();
    const figura = page.locator('figure.rvm-grafico').first();
    const rotulos = () => figura.locator('text').count();

    await expect.poll(rotulos).toBeGreaterThan(0);
    const antes = await rotulos();
    const caixa = (await grafico.boundingBox())!;

    await page.mouse.move(caixa.x + caixa.width * 0.3, caixa.y + caixa.height * 0.3);
    await page.mouse.down();
    await page.mouse.move(caixa.x + caixa.width * 0.6, caixa.y + caixa.height * 0.7, { steps: 10 });
    // Enquanto o botao esta apertado, a area aparece desenhada.
    await expect(page.locator('.rvm-grafico-caixa-de-zoom')).toBeVisible();
    await page.mouse.up();

    await expect(page.locator('.rvm-grafico-caixa-de-zoom')).toHaveCount(0);
    await expect(figura.locator('[aria-live=polite]').filter({ hasText: 'Mostrando de' })).toBeVisible();

    // E ha caminho de volta, mesmo tendo aproximado com o mouse.
    await grafico.dblclick();
    await expect.poll(rotulos).toBe(antes);
});

test('barras: o segundo eixo fica no topo, nao a direita', async ({ page }) => {
    await page.goto('/componentes/bar-chart');
    const figura = page.locator('figure.rvm-grafico').filter({ hasText: 'eixo de cima' });
    await expect(figura.getByText('eixo de baixo')).toBeVisible();

    const emSacas = figura.locator('text', { hasText: /sc\/ha$/ }).first();
    // "sc/ha" tambem termina em "ha": o rotulo do eixo de baixo precisa do numero antes.
    const emHectares = figura.locator('text', { hasText: /^[\d.,]+ ha$/ }).last();
    await expect(emSacas).toBeAttached();

    const yDeCima = Number(await emSacas.getAttribute('y'));
    const yDeBaixo = Number(await emHectares.getAttribute('y'));
    expect(yDeCima).toBeLessThan(yDeBaixo);
    await expect(figura.getByRole('columnheader', { name: 'Produtividade (eixo de cima)' })).toBeAttached();
});

// Animacao de entrada por tipo (DSGN-015). O que o Rafael viu antes — colunas entrando de lado — era
// a transicao de x/width se acertando quando o JS mede a largura real, nao animacao de entrada.
test('cada grafico entra do seu jeito, e "reduzir movimento" desliga tudo', async ({ page }) => {
    const animacaoDe = (seletor: string) =>
        page.locator(seletor).first().evaluate(e => {
            const s = getComputedStyle(e);
            // O CSS isolado embaralha o nome do @keyframes: fica so a parte legivel.
            return { nome: s.animationName.split('-b-')[0], origem: s.transformOrigin, duracao: s.animationDuration };
        });

    await page.goto('/componentes/column-chart');
    await expect(page.getByRole('heading', { name: 'RvmColumnChart', level: 1 })).toBeVisible();
    const coluna = await animacaoDe('.rvm-grafico-de-cima');
    expect(coluna.nome).toBe('rvm-grafico-descer');
    expect(coluna.origem).toMatch(/^[\d.]+px 0px$/); // topo da propria coluna

    await page.goto('/componentes/bar-chart');
    await expect(page.getByRole('heading', { name: 'RvmBarChart', level: 1 })).toBeVisible();
    const barra = await animacaoDe('.rvm-grafico-da-esquerda');
    expect(barra.nome).toBe('rvm-grafico-abrir');
    expect(barra.origem).toMatch(/^0px [\d.]+px$/); // ponta esquerda da propria barra

    await page.goto('/componentes/histogram');
    await expect(page.getByRole('heading', { name: 'RvmHistogram', level: 1 })).toBeVisible();
    expect((await animacaoDe('.rvm-grafico-de-cima')).nome).toBe('rvm-grafico-descer');

    await page.goto('/componentes/pie-chart');
    await expect(page.getByRole('heading', { name: 'RvmPieChart', level: 1 })).toBeVisible();
    expect((await animacaoDe('.rvm-grafico-relogio')).nome).toBe('rvm-grafico-relogio');
    // As fatias entram por uma mascara, entao ficam intactas.
    await expect(page.locator('g[mask]').first()).toBeAttached();

    await page.goto('/componentes/scatter-chart');
    await expect(page.getByRole('heading', { name: 'RvmScatterChart', level: 1 })).toBeVisible();
    expect((await animacaoDe('.rvm-grafico-nuvem')).nome).toBe('rvm-grafico-espalhar');

    await page.goto('/componentes/line-chart');
    await expect(page.getByRole('heading', { name: 'RvmLineChart', level: 1 })).toBeVisible();
    expect((await animacaoDe('.rvm-grafico-pontos')).nome).toBe('rvm-grafico-espalhar');
    expect((await animacaoDe('.rvm-grafico-linha')).nome).toBe('rvm-grafico-desenhar');
});

test('com "reduzir movimento" nenhum grafico anima', async ({ page }) => {
    await page.emulateMedia({ reducedMotion: 'reduce' });
    await page.goto('/componentes/column-chart');
    await expect(page.getByRole('heading', { name: 'RvmColumnChart', level: 1 })).toBeVisible();

    const coluna = page.locator('.rvm-grafico-de-cima').first();
    expect(await coluna.evaluate(e => getComputedStyle(e).animationName)).toBe('none');
    expect(await coluna.evaluate(e => getComputedStyle(e).transitionDuration)).toBe('0s');
});
