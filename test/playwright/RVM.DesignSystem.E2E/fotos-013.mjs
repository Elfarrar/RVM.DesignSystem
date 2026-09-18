// Fotos da DSGN-013: eixo no topo nas barras, caixa do zoom sendo desenhada e menu com XLSX/Imprimir.
import { chromium } from '@playwright/test';
import { mkdirSync } from 'node:fs';

const base = process.env.BASE_URL ?? 'http://localhost:5199';
const destino = process.argv[2] ?? '../../../docs/fotos/dsgn-013';
mkdirSync(destino, { recursive: true });

const navegador = await chromium.launch();
for (const tema of ['claro', 'escuro']) {
    const pagina = await navegador.newPage({ viewport: { width: 1280, height: 900 } });

    // 1. Barras com o segundo eixo em cima.
    await pagina.goto(`${base}/componentes/bar-chart`);
    await pagina.getByRole('heading', { name: 'RvmBarChart', level: 1 }).waitFor();
    if (tema === 'escuro') {
        await pagina.getByRole('button', { name: /Tema/ }).click();
    }

    const barras = pagina.locator('figure.rvm-grafico').filter({ hasText: 'eixo de cima' });
    await barras.scrollIntoViewIfNeeded();
    await barras.screenshot({ path: `${destino}/eixo-no-topo-${tema}.png` });

    // 2. Caixa do zoom sendo desenhada na dispersao (com o botao ainda apertado).
    await pagina.goto(`${base}/componentes/scatter-chart`);
    const dispersao = pagina.getByRole('group', { name: 'Adubacao e produtividade por talhao' });
    await dispersao.waitFor();
    await dispersao.scrollIntoViewIfNeeded();
    const caixa = await dispersao.boundingBox();
    await pagina.mouse.move(caixa.x + caixa.width * 0.3, caixa.y + caixa.height * 0.25);
    await pagina.mouse.down();
    await pagina.mouse.move(caixa.x + caixa.width * 0.65, caixa.y + caixa.height * 0.7, { steps: 10 });
    await pagina.locator('figure.rvm-grafico').first().screenshot({ path: `${destino}/caixa-de-zoom-${tema}.png` });
    await pagina.mouse.up();

    // 3. Menu de exportar, agora com planilha e imprimir.
    await pagina.goto(`${base}/componentes/column-chart`);
    await pagina.getByRole('heading', { name: 'RvmColumnChart', level: 1 }).waitFor();
    await pagina.getByRole('button', { name: 'Exportar' }).click();
    await pagina.getByRole('menuitem', { name: 'Imprimir' }).waitFor();
    await pagina.screenshot({ path: `${destino}/menu-exportar-${tema}.png` });
    await pagina.close();
}

await navegador.close();
console.log('fotos em', destino);
