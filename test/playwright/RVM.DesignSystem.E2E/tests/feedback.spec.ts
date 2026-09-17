import AxeBuilder from '@axe-core/playwright';
import { expect, test, type Page } from '@playwright/test';

async function semViolacaoSeria(page: Page) {
    const resultado = await new AxeBuilder({ page }).withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa']).analyze();
    const serias = resultado.violations.filter(v => v.impact === 'serious' || v.impact === 'critical');
    expect(serias.map(v => `${v.id}: ${v.nodes.map(n => n.target.join(' ')).join(', ')}`)).toEqual([]);
}

test('dialogo: o foco entra, fica preso, Esc fecha e o foco volta para quem abriu', async ({ page }) => {
    await page.goto('/componentes/dialog');
    const abrir = page.getByRole('button', { name: 'Ver detalhes da safra' });

    await abrir.click();
    const dialogo = page.getByRole('dialog', { name: 'Safra 2026' });
    await expect(dialogo).toBeVisible();
    await expect(dialogo).toBeFocused();

    // Dois controles dentro (x e Fechar): Tab tres vezes da a volta e continua la dentro.
    await page.keyboard.press('Tab');
    await expect(dialogo.getByRole('button', { name: 'Fechar' }).first()).toBeFocused();
    await page.keyboard.press('Tab');
    await page.keyboard.press('Tab');
    await expect.poll(() => dialogo.evaluate(el => el.contains(document.activeElement))).toBe(true);

    await page.keyboard.press('Escape');
    await expect(dialogo).toBeHidden();
    await expect(abrir).toBeFocused();
});

test('dialogo de confirmacao: e alertdialog, clicar fora nao fecha e a resposta chega', async ({ page }) => {
    await page.goto('/componentes/dialog');
    await page.getByRole('button', { name: 'Excluir talhao' }).click();

    const confirmacao = page.getByRole('alertdialog', { name: 'Excluir o Talhao 12?' });
    await expect(confirmacao).toHaveAccessibleDescription(/Nao da para desfazer/);
    await page.mouse.click(5, 5);
    await expect(confirmacao).toBeVisible();

    await confirmacao.getByRole('button', { name: 'Cancelar' }).click();
    await expect(confirmacao).toBeHidden();
    await expect(page.getByText('Exclusao cancelada.')).toBeVisible();
});

test('dialogo com formulario: a validacao do EditForm aparece la dentro', async ({ page }) => {
    await page.goto('/componentes/dialog');
    await page.getByRole('button', { name: 'Novo talhao' }).click();

    const dialogo = page.getByRole('dialog', { name: 'Novo talhao' });
    await dialogo.getByRole('button', { name: 'Salvar' }).click();
    await expect(dialogo.getByRole('textbox', { name: /Nome do talhao/ })).toHaveAttribute('aria-invalid', 'true');

    await dialogo.getByRole('textbox', { name: /Nome do talhao/ }).fill('Talhao 13');
    await dialogo.getByRole('button', { name: 'Salvar' }).click();
    await expect(dialogo).toBeHidden();
    await expect(page.getByText('Talhao Talhao 13 salvo.')).toBeVisible();
});

test('gaveta temporaria: Esc fecha e o foco volta; a permanente e um aside nomeado', async ({ page }) => {
    await page.goto('/componentes/drawer');
    const abrir = page.getByRole('button', { name: 'Da direita' });

    await abrir.click();
    const gaveta = page.getByRole('dialog', { name: 'Caixas de mensagem' });
    await expect(gaveta).toBeVisible();
    await expect(gaveta).toBeFocused();

    await page.keyboard.press('Escape');
    await expect(gaveta).toBeHidden();
    await expect(abrir).toBeFocused();

    await expect(page.getByRole('complementary', { name: 'Menu da fazenda' })).toBeVisible();
});

test('snackbar: a mensagem entra na regiao viva e a acao roda', async ({ page }) => {
    await page.goto('/componentes/snackbar');

    await page.getByRole('button', { name: 'Excluir aplicacao' }).click();
    const mensagem = page.getByRole('status').filter({ hasText: 'Aplicacao excluida.' });
    await expect(mensagem).toBeVisible();

    await mensagem.getByRole('button', { name: 'Desfazer' }).click();
    await expect(page.getByText('Exclusao desfeita.')).toBeVisible();
    await expect(page.getByText('Aplicacao excluida.')).toBeHidden();

    await page.getByRole('button', { name: 'Erro', exact: true }).click();
    await expect(page.getByRole('alert')).toContainText('Nao conseguimos salvar o talhao');
    await page.getByRole('button', { name: 'Fechar mensagem' }).click();
    await expect(page.getByRole('alert')).not.toContainText('Nao conseguimos');
});

// O axe de pagina roda com tudo fechado: dialogo, gaveta e mensagens tem cores proprias abertos.
for (const tema of ['claro', 'escuro'] as const) {
    test(`dialogo, gaveta e snackbar abertos sem violacao seria no tema ${tema}`, async ({ page }) => {
        await page.goto('/componentes/snackbar');
        await expect(page.getByRole('heading', { name: 'RvmSnackbar', level: 1 })).toBeVisible();
        if (tema === 'escuro') {
            await page.getByRole('button', { name: /Tema/ }).click();
        }

        await page.getByRole('button', { name: 'Excluir aplicacao' }).click();
        await page.getByRole('button', { name: 'Sucesso' }).click();
        await page.getByRole('button', { name: 'Erro', exact: true }).click();
        await expect(page.locator('.rvm-mensagem')).toHaveCount(3);
        await semViolacaoSeria(page);

        await page.goto('/componentes/dialog');
        await page.getByRole('button', { name: 'Excluir talhao' }).click();
        await expect(page.getByRole('alertdialog')).toBeVisible();
        await semViolacaoSeria(page);

        await page.goto('/componentes/drawer');
        await page.getByRole('button', { name: 'Da esquerda' }).click();
        await expect(page.getByRole('dialog', { name: 'Caixas de mensagem' })).toBeVisible();
        await semViolacaoSeria(page);
    });
}

// Onda 3 — feedback e sobreposicao. O bUnit prova a marcacao; aqui, o que so o navegador mostra.

test('progresso: o valor anunciado acompanha a barra', async ({ page }) => {
    await page.goto('/componentes/progress');
    const barra = page.getByRole('progressbar', { name: 'Importando notas fiscais' });
    await expect(barra).toHaveAttribute('aria-valuenow', '40');

    await page.getByRole('button', { name: 'Mais 10%' }).click();

    await expect(barra).toHaveAttribute('aria-valuenow', '50');
    await expect(barra.locator('.rvm-barra')).toHaveAttribute('style', 'width: 50%');
    await expect(page.getByRole('progressbar', { name: 'Buscando cotacoes' })).not.toHaveAttribute('aria-valuenow', /.*/);
});

test('progresso: o anel tem geometria valida no site em pt-BR', async ({ page }) => {
    // O site formata numero em pt-BR: r="20,2" era atributo invalido e o anel sumia sem erro nenhum.
    await page.goto('/componentes/progress');
    const arco = page.getByRole('progressbar', { name: 'Sincronizando medio' }).locator('.rvm-arco');

    await expect(arco).toHaveAttribute('r', '20.2');
    const largura = await arco.evaluate(el => (el as SVGGraphicsElement).getBBox().width);
    expect(largura).toBeGreaterThan(30);
});

test('esqueleto: a regiao avisa que carrega e os blocos ficam fora da arvore acessivel', async ({ page }) => {
    await page.goto('/componentes/skeleton');
    const regiao = page.getByRole('group', { name: 'Carregando o card do produtor' });

    await expect(regiao).toHaveAttribute('aria-busy', 'true');
    await expect(regiao.locator('.rvm-esqueleto').first()).toHaveAttribute('aria-hidden', 'true');
});

test('lista: item abre a sublista pelo teclado e a acao do fim nao aciona o item', async ({ page }) => {
    await page.goto('/componentes/list');
    const safras = page.getByRole('button', { name: 'Safras' });
    await expect(safras).toHaveAttribute('aria-expanded', 'false');

    await safras.focus();
    await page.keyboard.press('Enter');
    await expect(safras).toHaveAttribute('aria-expanded', 'true');
    await expect(page.getByRole('list', { name: 'Safras' }).getByRole('link', { name: 'Safra 2026' })).toBeVisible();

    const favoritar = page.getByRole('button', { name: 'Favoritar Clyde West' });
    await favoritar.click();
    await expect(page.getByRole('button', { name: 'Desfavoritar Clyde West' })).toHaveAttribute('aria-pressed', 'true');
    await expect(page.getByText('Contato escolhido: nenhum.')).toBeVisible();

    await page.getByRole('button', { name: 'Jill Ward', exact: true }).click();
    await expect(page.getByText('Contato escolhido: Jill Ward.')).toBeVisible();
    await expect(page.getByRole('button', { name: 'Jill Ward', exact: true })).toHaveAttribute('aria-pressed', 'true');
});

test('acordeao: abre pela regiao ligada e o exclusivo fecha o anterior', async ({ page }) => {
    await page.goto('/componentes/accordion');
    const plantio = page.getByRole('button', { name: /Plantio/ });
    await expect(plantio).toHaveAttribute('aria-expanded', 'false');

    await plantio.focus();
    await page.keyboard.press('Space');
    await expect(page.getByRole('region', { name: /Plantio/ })).toContainText('Semeadura');

    const primeira = page.getByRole('button', { name: 'Como cadastro um talhao?' });
    const segunda = page.getByRole('button', { name: 'Posso importar notas fiscais?' });
    await expect(primeira).toHaveAttribute('aria-expanded', 'true');
    await segunda.click();
    await expect(segunda).toHaveAttribute('aria-expanded', 'true');
    await expect(primeira).toHaveAttribute('aria-expanded', 'false');
    await expect(page.getByRole('heading', { level: 3, name: 'Quem ve os meus dados?' })).toBeVisible();
});

test('estado vazio: e uma regiao nomeada pelo titulo, com a acao alcancavel', async ({ page }) => {
    await page.goto('/componentes/empty-state');
    const regiao = page.getByRole('region', { name: 'Nenhum talhao cadastrado ainda' });

    await expect(regiao.getByRole('heading', { level: 3 })).toHaveText('Nenhum talhao cadastrado ainda');
    await expect(regiao.getByRole('button', { name: 'Cadastrar talhao' })).toBeVisible();
});
