import { defineConfig, devices } from '@playwright/test';

// Padrao do ecossistema: padrao-rvm § 12. NAO afrouxar para "resolver" teste instavel — workers 1,
// video ligado, retries 2 e waitForTimeout foram exatamente o que deixou o E2E do ERPAgro em 5-11 min.
//
// Aqui nao ha projeto `setup`: o site e publico e anonimo, nao existe login para salvar.

const baseURL = process.env.BASE_URL ?? 'http://localhost:5000';

export default defineConfig({
  testDir: './tests',
  fullyParallel: true,
  workers: process.env.CI ? '50%' : undefined,
  retries: process.env.CI ? 1 : 0,
  forbidOnly: !!process.env.CI,
  timeout: 30_000,
  expect: { timeout: 10_000 },
  outputDir: 'test-results',
  reporter: [['list'], ['html', { open: 'never' }]],
  use: {
    baseURL,
    trace: 'on-first-retry',
    video: 'off',
    screenshot: 'only-on-failure',
  },
  projects: [
    { name: 'chromium', use: { ...devices['Desktop Chrome'] } },
  ],
});
