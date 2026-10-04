import { expect, test } from '@playwright/test';

// DSGN-016: os arquivos para agentes de IA. Verificacao por CONTEUDO e por content-type, nunca so por
// status: o `try_files` do Nginx devolve o index.html com 200 para arquivo que nao existe, e um bloco
// `types` no vhost ja fez o servidor entregar o tipo errado com o conteudo certo (16/09).

test('@smoke o llms.txt responde como texto e aponta para a API completa e os tokens', async ({ request }) => {
  const resposta = await request.get('/llms.txt');

  expect(resposta.ok()).toBeTruthy();
  expect(resposta.headers()['content-type']).toMatch(/^text\/plain/);
  const corpo = await resposta.text();
  expect(corpo.startsWith('# RVM Design System')).toBeTruthy();
  expect(corpo).toContain('/llms-full.txt');
  expect(corpo).toContain('/tokens.json');
  expect(corpo).toContain('CC BY 4.0');
});

test('o llms-full.txt traz as regras, os componentes e os tokens', async ({ request }) => {
  const resposta = await request.get('/llms-full.txt');

  expect(resposta.ok()).toBeTruthy();
  expect(resposta.headers()['content-type']).toMatch(/^text\/plain/);
  const corpo = await resposta.text();
  expect(corpo).toContain('## Regras que um agente precisa seguir');
  expect(corpo).toContain('### RvmButton');
  expect(corpo).toContain('`--rvm-color-primary-main`');
});

test('@smoke o tokens.json e DTCG valido, com o tema escuro e a variavel CSS', async ({ request }) => {
  const resposta = await request.get('/tokens.json');

  expect(resposta.ok()).toBeTruthy();
  expect(resposta.headers()['content-type']).toMatch(/^application\/json/);
  const tokens = await resposta.json();
  const primario = tokens.color['primary-main'];
  expect(primario.$type).toBe('color');
  expect(primario.$value).toMatch(/^#[0-9A-Fa-f]{6}$/);
  expect(primario.$extensions['br.com.rvmit.design'].cssVar).toBe('--rvm-color-primary-main');
  expect(tokens.color['background-paper'].$extensions['br.com.rvmit.design'].modes.dark).toBeDefined();
});
