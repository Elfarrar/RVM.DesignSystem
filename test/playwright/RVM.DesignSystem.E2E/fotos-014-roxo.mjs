// Foto do fundo suave harmonizado: chips e alertas da familia primary, nos dois temas.
import { chromium } from '@playwright/test';
import { mkdirSync } from 'node:fs';

const base = process.env.BASE_URL ?? 'http://localhost:5199';
const destino = process.argv[2] ?? '../../../docs/fotos/dsgn-014';
mkdirSync(destino, { recursive: true });

const navegador = await chromium.launch();
for (const tema of ['claro', 'escuro']) {
    for (const [rota, titulo] of [['chip', 'RvmChip'], ['alert', 'RvmAlert']]) {
        const pagina = await navegador.newPage({ viewport: { width: 1280, height: 900 } });
        await pagina.goto(`${base}/componentes/${rota}`);
        await pagina.getByRole('heading', { name: titulo, level: 1 }).waitFor();
        if (tema === 'escuro') {
            await pagina.getByRole('button', { name: /Tema/ }).click();
        }

        const exemplo = pagina.locator('section').nth(1);
        await exemplo.scrollIntoViewIfNeeded();
        await exemplo.screenshot({ path: `${destino}/${rota}-suave-${tema}.png` });
        await pagina.close();
    }
}

await navegador.close();
console.log('fotos em', destino);
