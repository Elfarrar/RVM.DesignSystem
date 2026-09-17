// Fotos de entrega: nao faz parte da suite, roda a mao (node fotos.mjs).
import { chromium } from '@playwright/test';
import { mkdirSync } from 'node:fs';

const base = process.env.BASE_URL ?? 'http://localhost:5199';
const destino = process.argv[2] ?? '../../../docs/fotos/dsgn-011';
mkdirSync(destino, { recursive: true });

const navegador = await chromium.launch();
for (const tema of ['claro', 'escuro']) {
    const pagina = await navegador.newPage({ viewport: { width: 1280, height: 900 } });
    await pagina.goto(`${base}/componentes/column-chart`);
    await pagina.getByRole('heading', { name: 'RvmColumnChart', level: 1 }).waitFor();
    if (tema === 'escuro') {
        await pagina.getByRole('button', { name: /Tema/ }).click();
    }

    await pagina.getByRole('button', { name: 'Exportar' }).click();
    await pagina.getByRole('menuitem', { name: 'Dados CSV' }).waitFor();
    await pagina.screenshot({ path: `${destino}/exportar-${tema}.png` });
    await pagina.goto(`${base}/componentes/line-chart`);
    await pagina.getByRole('heading', { name: 'RvmLineChart', level: 1 }).waitFor();
    await pagina.locator('figure.rvm-grafico').filter({ hasText: 'eixo direito' }).scrollIntoViewIfNeeded();
    await pagina.screenshot({ path: `${destino}/eixo-duplo-${tema}.png` });

    const chuva = pagina.getByRole('group', { name: 'Chuva mensal por fazenda' });
    await chuva.scrollIntoViewIfNeeded();
    await chuva.focus();
    await pagina.keyboard.press('+');
    await pagina.keyboard.press('+');
    await pagina.screenshot({ path: `${destino}/zoom-${tema}.png` });
    await pagina.close();
}

await navegador.close();
console.log('fotos em', destino);
