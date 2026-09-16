import AxeBuilder from '@axe-core/playwright';
import { expect, test } from '@playwright/test';

// Uma pagina por componente. Estes testes valem para TODA pagina de componente que entrar: a lista
// abaixo cresce a cada fatia da onda 1, e o mesmo par de checagens (renderiza + axe nos dois temas)
// pega o basico sem escrever teste novo do zero.

const PAGINAS = [
    { rota: '/componentes/typography', titulo: 'RvmTypography' },
    { rota: '/componentes/divider', titulo: 'RvmDivider' },
];

test('@smoke o indice de componentes lista o que ja existe', async ({ page }) => {
    await page.goto('/componentes');

    await expect(page.getByRole('heading', { name: 'Componentes', level: 1 })).toBeVisible();
    for (const { titulo } of PAGINAS) {
        await expect(page.getByRole('link', { name: titulo })).toBeVisible();
    }
});

for (const { rota, titulo } of PAGINAS) {
    test(`@smoke ${titulo} tem exemplo, parametros e a referencia do kit`, async ({ page }) => {
        await page.goto(rota);

        await expect(page.getByRole('heading', { name: titulo, level: 1 })).toBeVisible();
        await expect(page.getByRole('heading', { name: 'Parametros' })).toBeVisible();
        // O recorte do kit ao lado do exemplo e criterio do 07-site-de-documentacao: sem ele,
        // "parece o NEATLAB?" vira discussao de memoria.
        await expect(page.getByRole('img')).toBeVisible();
        await expect(page.getByRole('table')).toBeVisible();
    });

    for (const tema of ['claro', 'escuro'] as const) {
        test(`${titulo} sem violacao seria de acessibilidade no tema ${tema}`, async ({ page }) => {
            await page.goto(rota);
            await expect(page.getByRole('heading', { name: titulo, level: 1 })).toBeVisible();

            if (tema === 'escuro') {
                const seletor = page.getByRole('button', { name: /Tema/ });
                await seletor.click();
                await expect(seletor).toHaveAttribute('aria-pressed', 'true');
            }

            const resultado = await new AxeBuilder({ page })
                .withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa'])
                // A amostra do token de texto desabilitado reprova contraste por definicao.
                .exclude('[data-rvm-demo="texto-desabilitado"]')
                .analyze();

            const serias = resultado.violations.filter(v => v.impact === 'serious' || v.impact === 'critical');
            expect(serias.flatMap(v => v.nodes.map(n => `${v.id} em ${n.target.join(' ')}`))).toEqual([]);
        });
    }
}

test('o divisor com rotulo continua sendo um separador para o leitor de tela', async ({ page }) => {
    await page.goto('/componentes/divider');

    // <hr> nao aceita conteudo, entao a versao com rotulo troca de elemento. O que nao pode mudar
    // e o PAPEL: se o separador virar um div qualquer, o leitor de tela perde a informacao.
    const separadores = page.getByRole('separator');
    await expect(separadores.first()).toBeVisible();
    expect(await separadores.count()).toBeGreaterThanOrEqual(4);
    await expect(page.getByRole('separator').filter({ hasText: 'ou' })).toHaveCount(1);
});
