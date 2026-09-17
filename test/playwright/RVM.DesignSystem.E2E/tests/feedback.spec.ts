import { expect, test } from '@playwright/test';

// Onda 3 — feedback e sobreposicao. O bUnit prova a marcacao; aqui, o que so o navegador mostra.

test('progresso: o valor anunciado acompanha a barra', async ({ page }) => {
    await page.goto('/componentes/progress');
    const barra = page.getByRole('progressbar', { name: 'Importando notas fiscais' });
    await expect(barra).toHaveAttribute('aria-valuenow', '40');

    await page.getByRole('button', { name: 'Mais 10%' }).click();

    await expect(barra).toHaveAttribute('aria-valuenow', '50');
    await expect(barra.locator('.barra')).toHaveAttribute('style', 'width: 50%');
    await expect(page.getByRole('progressbar', { name: 'Buscando cotacoes' })).not.toHaveAttribute('aria-valuenow', /.*/);
});

test('progresso: o anel tem geometria valida no site em pt-BR', async ({ page }) => {
    // O site formata numero em pt-BR: r="20,2" era atributo invalido e o anel sumia sem erro nenhum.
    await page.goto('/componentes/progress');
    const arco = page.getByRole('progressbar', { name: 'Sincronizando medio' }).locator('.arco');

    await expect(arco).toHaveAttribute('r', '20.2');
    const largura = await arco.evaluate(el => (el as SVGGraphicsElement).getBBox().width);
    expect(largura).toBeGreaterThan(30);
});

test('esqueleto: a regiao avisa que carrega e os blocos ficam fora da arvore acessivel', async ({ page }) => {
    await page.goto('/componentes/skeleton');
    const regiao = page.getByRole('group', { name: 'Carregando o card do produtor' });

    await expect(regiao).toHaveAttribute('aria-busy', 'true');
    await expect(regiao.locator('.esqueleto').first()).toHaveAttribute('aria-hidden', 'true');
});

test('estado vazio: e uma regiao nomeada pelo titulo, com a acao alcancavel', async ({ page }) => {
    await page.goto('/componentes/empty-state');
    const regiao = page.getByRole('region', { name: 'Nenhum talhao cadastrado ainda' });

    await expect(regiao.getByRole('heading', { level: 3 })).toHaveText('Nenhum talhao cadastrado ainda');
    await expect(regiao.getByRole('button', { name: 'Cadastrar talhao' })).toBeVisible();
});
