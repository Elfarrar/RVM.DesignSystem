import AxeBuilder from '@axe-core/playwright';
import { expect, test, type Page } from '@playwright/test';

// Telas de exemplo (DSGN-009): combinacoes reais dos componentes. Uma interacao por tela e axe nos dois
// temas; o detalhe de cada componente ja e testado na pagina dele.

async function semViolacaoSeria(page: Page) {
    const resultado = await new AxeBuilder({ page }).withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa']).analyze();
    const serias = resultado.violations.filter(v => v.impact === 'serious' || v.impact === 'critical');
    expect(serias.map(v => `${v.id}: ${v.nodes.map(n => n.target.join(' ')).join(', ')}`)).toEqual([]);
}

const TELAS = [
    { rota: '/exemplos/dashboard', titulo: 'Painel da safra', menu: 'Dashboard' },
    { rota: '/exemplos/cms', titulo: 'Publicacoes', menu: 'CMS' },
    { rota: '/exemplos/crm', titulo: 'Funil de vendas', menu: 'CRM' },
    { rota: '/exemplos/erp', titulo: 'Operacao', menu: 'ERP' },
    { rota: '/exemplos/planner', titulo: 'Planner de tarefas', menu: 'Planner' },
];

for (const { rota, titulo, menu } of TELAS) {
    test(`@smoke exemplo ${menu} abre pelo menu lateral`, async ({ page }) => {
        await page.goto('/');
        await page.getByRole('navigation', { name: 'Menu do site' }).getByRole('link', { name: menu, exact: true }).click();
        await expect(page).toHaveURL(new RegExp(`${rota}$`));
        await expect(page.getByRole('heading', { name: titulo, level: 1 })).toBeVisible();
    });

    for (const tema of ['claro', 'escuro'] as const) {
        test(`exemplo ${menu} sem violacao seria no tema ${tema}`, async ({ page }) => {
            await page.goto(rota);
            await expect(page.getByRole('heading', { name: titulo, level: 1 })).toBeVisible();
            if (tema === 'escuro') {
                await page.getByRole('button', { name: /Tema/ }).click();
            }
            await semViolacaoSeria(page);
        });
    }
}

test('dashboard: o periodo muda o resumo e exportar avisa', async ({ page }) => {
    await page.goto('/exemplos/dashboard');
    await page.getByRole('button', { name: 'Este mes' }).click();
    await page.getByRole('menuitem', { name: 'Esta safra' }).click();
    await expect(page.getByText('Resumo desta safra da Fazenda Boa Vista.')).toBeVisible();

    await page.getByRole('button', { name: 'Exportar' }).click();
    await expect(page.getByText('Relatorio enviado para o seu e-mail.')).toBeVisible();
});

test('cms: publicar uma nova publicacao pelo dialogo', async ({ page }) => {
    await page.goto('/exemplos/cms');
    await expect(page.getByRole('tab', { name: 'Publicadas (4)' })).toBeVisible();

    await page.getByRole('button', { name: 'Nova publicacao' }).click();
    const dialogo = page.getByRole('dialog', { name: 'Nova publicacao' });
    await dialogo.getByRole('button', { name: 'Publicar', exact: true }).click();
    await expect(dialogo.getByText('Escreva um titulo para a publicacao.')).toBeVisible();

    await dialogo.getByLabel('Titulo').fill('Colheita de milho comeca na segunda');
    await dialogo.getByRole('button', { name: 'Publicar', exact: true }).click();

    await expect(dialogo).toBeHidden();
    await expect(page.getByText('"Colheita de milho comeca na segunda" foi publicada.')).toBeVisible();
    await expect(page.getByRole('tab', { name: 'Publicadas (5)' })).toBeVisible();

    await page.getByRole('tab', { name: 'Arquivadas (0)' }).click();
    await expect(page.getByRole('heading', { name: 'Nenhuma publicacao arquivadas' })).toBeVisible();
});

test('crm: mover para Ganho e registrar contato na ficha', async ({ page }) => {
    await page.goto('/exemplos/crm');
    const ganho = page.getByRole('region', { name: /^Ganho/ });
    await expect(ganho.getByRole('article')).toHaveCount(1);

    await page.getByRole('button', { name: 'Mover Agro Sul Graos para outra etapa' }).click();
    await page.getByRole('menuitem', { name: 'Ganho' }).click();
    await expect(ganho.getByRole('article')).toHaveCount(2);
    await expect(page.getByText('Negocio com Agro Sul Graos ganho!')).toBeVisible();

    await page.getByRole('button', { name: 'Ver ficha de Agro Sul Graos' }).click();
    const ficha = page.getByRole('dialog', { name: 'Ficha do cliente' });
    await ficha.getByLabel('Registrar contato').fill('Enviou contrato assinado');
    await ficha.getByRole('button', { name: 'Registrar', exact: true }).click();
    await expect(ficha.getByRole('list', { name: 'Historico de Agro Sul Graos' })).toContainText('Enviou contrato assinado');
});

test('erp: faturar marcados e emitir pedido em tres etapas', async ({ page }) => {
    await page.goto('/exemplos/erp');
    const grade = page.getByRole('table', { name: 'Pedidos de venda' });
    await grade.getByRole('checkbox', { name: 'Marcar pedido #1047' }).check();
    await grade.getByRole('checkbox', { name: 'Marcar pedido #1045' }).check();
    await page.getByRole('button', { name: 'Faturar marcados' }).click();
    await expect(page.getByText(/1 pedido\(s\) faturado\(s\), R\$ 184\.500\. 1 nao estavam aprovados/)).toBeVisible();
    await expect(grade.getByRole('row', { name: /#1047/ })).toContainText('Faturado');

    await page.getByRole('button', { name: 'Novo pedido' }).click();
    const dialogo = page.getByRole('dialog', { name: 'Novo pedido de venda' });
    await dialogo.getByRole('button', { name: 'Proximo' }).click();
    await expect(dialogo.getByText('Escolha o cliente do pedido.')).toBeVisible();
    await dialogo.getByRole('combobox', { name: 'Cliente' }).click();
    await page.getByRole('option', { name: 'Armazens Planalto' }).click();
    await dialogo.getByRole('button', { name: 'Proximo' }).click();

    await dialogo.getByLabel('Quantidade').fill('500');
    // O campo grava ao sair dele (evento change), como um input comum.
    await page.keyboard.press('Tab');
    await expect(dialogo.getByText('Total estimado: R$ 64.000')).toBeVisible();
    await dialogo.getByRole('button', { name: 'Proximo' }).click();

    await dialogo.getByRole('button', { name: /Entrega/ }).click();
    await page.getByRole('dialog', { name: 'Escolher Entrega' }).getByRole('button', { name: /25 de setembro/ }).click();
    await dialogo.getByRole('button', { name: 'Emitir pedido' }).click();

    await expect(dialogo).toBeHidden();
    await expect(page.getByText('Pedido #1048 emitido para Armazens Planalto. Ele entra em analise de credito.')).toBeVisible();
    await expect(grade.getByRole('row', { name: /#1048/ })).toContainText('Em analise');
});

test('planner: criar tarefa com hora no relogio e concluir', async ({ page }) => {
    await page.goto('/exemplos/planner');
    await page.getByRole('button', { name: 'Nova tarefa' }).click();
    const dialogo = page.getByRole('dialog', { name: 'Nova tarefa' });
    await dialogo.getByLabel('O que precisa ser feito').fill('Vistoriar silo 3');

    await dialogo.getByRole('button', { name: /Hora \(opcional\)/ }).click();
    const relogio = page.getByRole('dialog', { name: 'Escolher Hora (opcional)' });
    await expect(relogio.getByRole('listbox', { name: 'Horas' })).toBeFocused();
    await page.keyboard.press('ArrowUp');
    await page.keyboard.press('ArrowUp');
    await relogio.getByRole('button', { name: 'OK' }).click();
    await expect(dialogo.getByRole('button', { name: /Hora \(opcional\) 13:00/ })).toBeVisible();

    await dialogo.getByRole('button', { name: 'Criar tarefa' }).click();
    await expect(dialogo).toBeHidden();
    await expect(page.getByText('Tarefa criada para 17/09 as 13:00.')).toBeVisible();

    const aFazer = page.getByRole('region', { name: /^A fazer/ });
    const cartao = aFazer.getByRole('article').filter({ hasText: 'Vistoriar silo 3' });
    await expect(cartao).toBeVisible();
    // Marcar move o cartao para outra coluna: o clique basta, a prova e ele aparecer em Concluido.
    await cartao.getByRole('checkbox', { name: 'Concluida' }).click();
    await expect(page.getByRole('region', { name: /^Concluido/ }).getByRole('article').filter({ hasText: 'Vistoriar silo 3' })).toBeVisible();
});
