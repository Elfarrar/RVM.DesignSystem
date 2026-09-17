// Fotos da DSGN-012: as tabelas de parametros das cinco paginas. Roda a mao (node fotos-012.mjs).
import { chromium } from '@playwright/test';
import { mkdirSync } from 'node:fs';

const base = process.env.BASE_URL ?? 'http://localhost:5199';
const destino = process.argv[2] ?? '../../../docs/fotos/dsgn-012';
mkdirSync(destino, { recursive: true });

const navegador = await chromium.launch();
for (const [rota, titulo] of [['area-chart', 'RvmAreaChart'], ['radar-chart', 'RvmRadarChart']]) {
    for (const tema of ['claro', 'escuro']) {
        const pagina = await navegador.newPage({ viewport: { width: 1280, height: 1000 } });
        await pagina.goto(`${base}/componentes/${rota}`);
        await pagina.getByRole('heading', { name: titulo, level: 1 }).waitFor();
        if (tema === 'escuro') {
            await pagina.getByRole('button', { name: /Tema/ }).click();
        }

        // A tabela inteira, nao a dobra da tela: e ela que precisa ser lida na conferencia.
        const tabela = pagina.locator('table').last();
        await tabela.scrollIntoViewIfNeeded();
        await tabela.screenshot({ path: `${destino}/${rota}-${tema}.png` });
        await pagina.close();
    }
}

await navegador.close();
console.log('fotos em', destino);
