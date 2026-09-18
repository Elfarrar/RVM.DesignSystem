// Fotos da DSGN-014: as elevacoes recalibradas pela medicao do kit, nos dois temas.
import { chromium } from '@playwright/test';
import { mkdirSync } from 'node:fs';

const base = process.env.BASE_URL ?? 'http://localhost:5199';
const destino = process.argv[2] ?? '../../../docs/fotos/dsgn-014';
mkdirSync(destino, { recursive: true });

const navegador = await chromium.launch();
for (const tema of ['claro', 'escuro']) {
    const pagina = await navegador.newPage({ viewport: { width: 1280, height: 900 } });
    await pagina.goto(`${base}/fundamentos`);
    await pagina.getByRole('heading', { name: 'Fundamentos', level: 1 }).waitFor();
    if (tema === 'escuro') {
        await pagina.getByRole('button', { name: /Tema/ }).click();
    }

    const elevacoes = pagina.locator('section').filter({ hasText: '--rvm-shadow-1' }).first();
    await elevacoes.scrollIntoViewIfNeeded();
    await elevacoes.screenshot({ path: `${destino}/elevacoes-${tema}.png` });
    await pagina.close();
}

await navegador.close();
console.log('fotos em', destino);
