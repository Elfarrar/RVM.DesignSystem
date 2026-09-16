import AxeBuilder from '@axe-core/playwright';
import { expect, test } from '@playwright/test';

// O site e Blazor WebAssembly: o HTML inicial chega vazio e o conteudo aparece quando o runtime
// carrega. Por isso a espera e por SINAL (o elemento renderizado), nunca por relogio.

test('@smoke o site publicado carrega e se identifica', async ({ page }) => {
  await page.goto('/');

  await expect(page.getByRole('heading', { name: 'RVM Design System', level: 1 })).toBeVisible();
});

test('@smoke o credito CC BY ao NEATLAB aparece no rodape', async ({ page }) => {
  await page.goto('/');

  // Obrigacao de licenca (08-monetizacao): sem o credito, a publicacao e violacao de CC BY.
  const rodape = page.locator('footer');
  await expect(rodape.getByRole('link', { name: /NEATLAB/ })).toBeVisible();
  await expect(rodape.getByRole('link', { name: /CC BY 4.0/ })).toBeVisible();
});

test('rota profunda recarregada continua abrindo o site', async ({ page }) => {
  // Host estatico nao tem try_files. O `404.html` copia do `index.html` e o que segura isto.
  await page.goto('/rota/que/nao/existe');

  await expect(page.getByRole('heading', { level: 1 })).toBeVisible();
});

test('pagina inicial sem violacao seria de acessibilidade', async ({ page }) => {
  await page.goto('/');
  await expect(page.getByRole('heading', { name: 'RVM Design System', level: 1 })).toBeVisible();

  const resultado = await new AxeBuilder({ page })
    .withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa'])
    .analyze();

  const serias = resultado.violations.filter(v => v.impact === 'serious' || v.impact === 'critical');
  expect(serias.map(v => `${v.id}: ${v.help}`)).toEqual([]);
});
