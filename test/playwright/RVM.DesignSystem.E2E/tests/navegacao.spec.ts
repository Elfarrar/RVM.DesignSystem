import AxeBuilder from '@axe-core/playwright';
import { expect, test, type Page } from '@playwright/test';

async function semViolacaoSeria(page: Page) {
    const resultado = await new AxeBuilder({ page }).withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa']).analyze();
    const serias = resultado.violations.filter(v => v.impact === 'serious' || v.impact === 'critical');
    expect(serias.map(v => `${v.id}: ${v.nodes.map(n => n.target.join(' ')).join(', ')}`)).toEqual([]);
}

// O axe de cada pagina roda com tudo fechado. Menu e lista abertos tem cores proprias (item focado,
// opcao ativa) — no tema claro o item focado do menu ja reprovou contraste so aberto.
for (const tema of ['claro', 'escuro'] as const) {
    test(`menu e select abertos sem violacao seria no tema ${tema}`, async ({ page }) => {
        await page.goto('/componentes/menu');
        await expect(page.getByRole('heading', { name: 'RvmMenu', level: 1 })).toBeVisible();
        if (tema === 'escuro') {
            await page.getByRole('button', { name: /Tema/ }).click();
        }

        await page.getByRole('button', { name: 'Acoes do talhao' }).focus();
        await page.keyboard.press('ArrowDown');
        await expect(page.getByRole('menuitem', { name: 'Editar' })).toBeFocused();
        await semViolacaoSeria(page);

        await page.goto('/componentes/select');
        await page.getByRole('combobox', { name: 'Talhoes' }).click();
        await page.getByRole('option', { name: 'Talhao 2' }).click();
        await page.keyboard.press('ArrowDown');
        await expect(page.getByRole('listbox', { name: 'Talhoes' })).toBeVisible();
        await semViolacaoSeria(page);
    });
}

// Os tres padroes ARIA com teclado proprio da onda 2 (tabs, menu button, select-only combobox). O
// bUnit prova a logica; aqui se prova o que so o navegador mostra: para onde o foco foi de verdade.

test('abas: setas trocam de aba, pulam a desabilitada e levam o foco junto', async ({ page }) => {
    await page.goto('/componentes/tabs');
    const lista = page.getByRole('tablist', { name: 'Dados do talhao' });
    const resumo = lista.getByRole('tab', { name: 'Resumo' });
    await expect(resumo).toHaveAttribute('aria-selected', 'true');

    await resumo.focus();
    await page.keyboard.press('ArrowRight');
    const aplicacoes = lista.getByRole('tab', { name: 'Aplicacoes' });
    await expect(aplicacoes).toBeFocused();
    await expect(page.getByRole('tabpanel', { name: 'Aplicacoes' })).toContainText('Historico de defensivos');

    await page.keyboard.press('ArrowRight');
    await expect(lista.getByRole('tab', { name: 'Colheita' })).toBeFocused();

    await page.keyboard.press('Home');
    await expect(resumo).toBeFocused();
    await expect(resumo).toHaveAttribute('aria-selected', 'true');
});

test('menu: seta abre no primeiro item, Esc fecha e devolve o foco ao botao', async ({ page }) => {
    await page.goto('/componentes/menu');
    const botao = page.getByRole('button', { name: 'Acoes do talhao' });

    await botao.focus();
    await page.keyboard.press('ArrowDown');
    const menu = page.getByRole('menu', { name: 'Acoes do talhao' });
    await expect(menu).toBeVisible();
    await expect(botao).toHaveAttribute('aria-expanded', 'true');
    await expect(menu.getByRole('menuitem', { name: 'Editar' })).toBeFocused();

    // Exportar esta desabilitado: a seta vai de Duplicar direto para Excluir.
    await page.keyboard.press('ArrowDown');
    await page.keyboard.press('ArrowDown');
    await expect(menu.getByRole('menuitem', { name: 'Excluir' })).toBeFocused();

    await page.keyboard.press('Escape');
    await expect(menu).toBeHidden();
    await expect(botao).toBeFocused();
});

test('menu: Enter no item executa a acao e fecha', async ({ page }) => {
    await page.goto('/componentes/menu');
    const botao = page.getByRole('button', { name: 'Acoes do talhao' });

    await botao.click();
    await page.getByRole('menuitem', { name: 'Duplicar' }).focus();
    await page.keyboard.press('Enter');

    await expect(page.getByRole('menu')).toBeHidden();
    await expect(page.getByText('Ultima acao: Duplicar')).toBeVisible();
    await expect(botao).toBeFocused();
});

test('select: teclado abre, anda pela lista e escolhe sem tirar o foco do campo', async ({ page }) => {
    await page.goto('/componentes/select');
    const campo = page.getByRole('combobox', { name: 'Cultura do plantio' });

    await campo.focus();
    await page.keyboard.press('ArrowDown');
    const lista = page.getByRole('listbox', { name: 'Cultura do plantio' });
    await expect(lista).toBeVisible();
    await expect(campo).toBeFocused();

    await page.keyboard.press('ArrowDown');
    const milho = lista.getByRole('option', { name: 'Milho' });
    await expect(campo).toHaveAttribute('aria-activedescendant', (await milho.getAttribute('id'))!);

    await page.keyboard.press('Enter');
    await expect(lista).toBeHidden();
    await expect(campo).toHaveText('Milho');
    await expect(campo).toBeFocused();
});

test('select: salvar vazio mostra a validacao do EditForm no campo', async ({ page }) => {
    await page.goto('/componentes/select');

    await page.getByRole('button', { name: 'Salvar plantio' }).click();

    const campo = page.getByRole('combobox', { name: 'Cultura do plantio' });
    await expect(campo).toHaveAttribute('aria-invalid', 'true');
    await expect(campo).toHaveAccessibleDescription('Escolha a cultura para salvar o plantio.');
});

test('select com busca: digitar filtra ignorando acento e Enter escolhe', async ({ page }) => {
    await page.goto('/componentes/select');
    const campo = page.getByRole('combobox', { name: 'Municipio' });

    await campo.click();
    const busca = page.getByRole('textbox', { name: 'Buscar municipio' });
    await expect(busca).toBeFocused();

    await busca.fill('luis');
    const lista = page.getByRole('listbox', { name: 'Municipio' });
    await expect(lista.getByRole('option')).toHaveCount(1);

    await page.keyboard.press('Enter');
    await expect(lista).toBeHidden();
    await expect(campo).toHaveText('Luis Eduardo Magalhaes');
});

test('multiplo: marca varias e a lista continua aberta', async ({ page }) => {
    await page.goto('/componentes/select');
    const campo = page.getByRole('combobox', { name: 'Talhoes' });

    await campo.click();
    const lista = page.getByRole('listbox', { name: 'Talhoes' });
    await lista.getByRole('option', { name: 'Talhao 2' }).click();
    await lista.getByRole('option', { name: 'Talhao 5' }).click();

    await expect(lista).toBeVisible();
    await expect(lista.getByRole('option', { name: 'Talhao 2' })).toHaveAttribute('aria-selected', 'true');
    await expect(page.getByText('Talhoes escolhidos: Talhao 2, Talhao 5.')).toBeVisible();

    await page.keyboard.press('Escape');
    await expect(lista).toBeHidden();
});
