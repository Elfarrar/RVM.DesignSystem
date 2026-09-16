import AxeBuilder from '@axe-core/playwright';
import { expect, test } from '@playwright/test';

// A troca de tema e a coisa mais facil de quebrar sem ninguem ver: o CSS continua valido, a pagina
// continua 200, e so o olho percebe. Por isso os testes olham o TOKEN resolvido pelo navegador, e
// nao a classe nem o atributo — e o valor que o componente vai usar de verdade.

async function corDeFundo(page: import('@playwright/test').Page) {
    return page.evaluate(() =>
        getComputedStyle(document.querySelector('.rvm-root')!).getPropertyValue('--rvm-color-background-body').trim());
}

test('@smoke a pagina de fundamentos renderiza os tokens', async ({ page }) => {
    await page.goto('/fundamentos');

    await expect(page.getByRole('heading', { name: 'Fundamentos', level: 1 })).toBeVisible();
    await expect(page.getByText('--rvm-color-primary-main')).toBeVisible();
    await expect(page.getByRole('heading', { name: 'Tipografia' })).toBeVisible();
});

test('@smoke o seletor troca o tema sem recarregar', async ({ page }) => {
    await page.goto('/fundamentos');

    const seletor = page.getByRole('button', { name: /Tema/ });
    await expect(seletor).toHaveAttribute('aria-pressed', 'false');
    expect(await corDeFundo(page)).toBe('#F4F5FA');

    await seletor.click();

    // Espera pelo SINAL (o estado do botao), nunca por relogio.
    await expect(seletor).toHaveAttribute('aria-pressed', 'true');
    expect(await corDeFundo(page)).toBe('#28243D');
    await expect(page.locator('html')).toHaveAttribute('data-theme', 'dark');
});

test('o tema escolhido sobrevive ao recarregamento', async ({ page }) => {
    await page.goto('/');
    await page.getByRole('button', { name: /Tema/ }).click();
    await expect(page.getByRole('button', { name: /Tema/ })).toHaveAttribute('aria-pressed', 'true');

    await page.reload();

    await expect(page.getByRole('button', { name: /Tema/ })).toHaveAttribute('aria-pressed', 'true');
    expect(await corDeFundo(page)).toBe('#28243D');
});

test('a fonte Inter vem do proprio pacote, nao de CDN', async ({ page }) => {
    const deTerceiros: string[] = [];
    page.on('request', r => {
        const url = new URL(r.url());
        if (url.host !== new URL(page.url() || 'http://localhost').host && /font|\.woff2?$/.test(url.pathname + url.host)) {
            deTerceiros.push(r.url());
        }
    });

    await page.goto('/fundamentos');
    await expect(page.getByRole('heading', { name: 'Fundamentos', level: 1 })).toBeVisible();

    expect(deTerceiros).toEqual([]);
    const familia = await page.evaluate(() =>
        getComputedStyle(document.querySelector('.rvm-root')!).fontFamily);
    expect(familia).toContain('Inter');
});

for (const tema of ['claro', 'escuro'] as const) {
    test(`fundamentos sem violacao seria de acessibilidade no tema ${tema}`, async ({ page }) => {
        await page.goto('/fundamentos');
        await expect(page.getByRole('heading', { name: 'Fundamentos', level: 1 })).toBeVisible();

        if (tema === 'escuro') {
            const seletor = page.getByRole('button', { name: /Tema/ });
            await seletor.click();
            await expect(seletor).toHaveAttribute('aria-pressed', 'true');
        }

        const resultado = await new AxeBuilder({ page })
            .withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa'])
            // A amostra do token de texto desabilitado reprova contraste por definicao — e disso
            // que ela trata. WCAG 1.4.3 isenta componente inativo. Exclusao de UM no, nomeado.
            .exclude('[data-rvm-demo="texto-desabilitado"]')
            .analyze();

        const serias = resultado.violations.filter(v => v.impact === 'serious' || v.impact === 'critical');
        // Com o alvo junto: falha de contraste sem dizer ONDE custa outra rodada inteira de deploy.
        expect(serias.flatMap(v => v.nodes.map(n => `${v.id} em ${n.target.join(' ')}`))).toEqual([]);
    });
}
