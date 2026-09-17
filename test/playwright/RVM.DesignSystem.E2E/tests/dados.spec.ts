import { expect, test } from '@playwright/test';

// Onda 4 — dados e shell. O bUnit prova a marcacao; aqui, teclado e foco de verdade.

test('avaliacao: setas mudam a nota pelo teclado, com o grupo nomeado', async ({ page }) => {
    await page.goto('/componentes/rating');
    const grupo = page.getByRole('group', { name: 'Nota do atendimento' });
    const quatro = grupo.getByRole('radio', { name: '4 estrelas de 5' });
    await expect(quatro).toBeChecked();

    await quatro.focus();
    await page.keyboard.press('ArrowLeft');

    await expect(grupo.getByRole('radio', { name: '3 estrelas de 5' })).toBeChecked();
    await expect(page.getByText(/Atendimento: 3 de 5/)).toBeVisible();
});

test('avaliacao: somente leitura e uma imagem com a nota por extenso', async ({ page }) => {
    await page.goto('/componentes/rating');

    await expect(page.getByRole('img', { name: 'Nota grande: 2,5 de 5' })).toBeVisible();
});

test('etapas: avancar move a etapa atual e o estado vai em texto', async ({ page }) => {
    await page.goto('/componentes/stepper');
    const etapas = page.getByRole('list', { name: 'Cadastro da fazenda' });
    await expect(etapas.locator('[aria-current="step"]')).toContainText('Dados da fazenda');

    await page.getByRole('button', { name: 'Avancar' }).click();

    await expect(etapas.locator('[aria-current="step"]')).toContainText('Talhoes');
    await expect(etapas.getByRole('listitem').nth(1)).toContainText('concluida');
});

test('linha do tempo: lista ordenada com os acontecimentos na ordem', async ({ page }) => {
    await page.goto('/componentes/timeline');
    const linha = page.getByRole('list', { name: 'Safra com datas' });

    await expect(linha.getByRole('listitem')).toHaveCount(3);
    await expect(linha.getByRole('listitem').first()).toContainText('Plantio concluido');
});
