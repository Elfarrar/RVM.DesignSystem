# Kanban — RVM.DesignSystem

> **Este arquivo e INDICE: uma linha por card.** Contexto, plano e decisao vivem no card
> (`02_Tasks/DSGN-NNN.md`), nunca aqui. **Alvo: cabe em duas telas.**
>
> **O card nasce ANTES do codigo**, com o escopo e as decisoes em aberto.
>
> Modo de trabalho: **Claude implementa, o Rafael revisa** (`CLAUDE.md § Modo de trabalho`).

## Em revisao

- [[DSGN-017]] — **Onda 2**: casca de aplicacao (`RvmAppShell`/`Sidebar`/`Topbar`/`NavItem`),
  layout (`Stack`/`Grid`/`Card`/`Divider`), conteudo e navegacao (`Chip`/`Avatar`/`Tabs`/
  `Breadcrumb`) e densidade `Compact`. **O proprio site ja usa a casca** — criterio de saida
  cumprido. 184 testes, 29 pares de contraste. Sai como `0.2.0`.

## Concluido

- [[DSGN-001]] — Bootstrap + **producao no ar** (08/09/2026): repo publico, esqueleto .NET 10,
  5 workflows, DNS/SSL nos dois ambientes, `design.dev.rvmtech.com.br` e `design.rvmit.com.br`.
  ⏳ So o monitor no Uptime-Kuma ficou de fora — o Kuma nao tem canal de notificacao nenhum.
- [[DSGN-003]] — `CNAME` no artefato, borda do titulo removida e 404 do CSS isolado consertado.
- [[DSGN-004]] — `Verify` do primeiro deploy de Pages: o site subia certo e o job ficava vermelho.

## A fazer

### ✅ Onda 2 — fechada em 08/09/2026, sai como `0.2.0`

Doze componentes de layout e navegacao, densidade `Compact`, 8 icones novos e 13 paginas de
documentacao. O conjunto de icones — a decisao de contrato que sobrava da onda 1 — ficou
**curado**: 19 icones, cada um entrando junto do componente que o usa. Card: [[DSGN-017]].

### ✅ Onda 1 — fechada em 08/09/2026, publicada como `0.1.0`

Nove componentes, camada de tokens, motor de tema, icones Phosphor e o site de documentacao.
`design.rvmit.com.br` no ar. Cards: [[DSGN-002]], [[DSGN-012]], [[DSGN-013]], [[DSGN-014]],
[[DSGN-015]].

### Onda 3 — Feedback (proxima)

`RvmDialog` + `IRvmDialogService`, `RvmToast` + `IRvmToastService`, `RvmAlert`, `RvmSkeleton`,
`RvmSpinner`, `RvmProgress`, `RvmTooltip`, `RvmEmptyState`. Fecha em `0.3.0`.

> **Meta (`09-roadmap`):** o E2E abre o dialogo, navega so por teclado, fecha com `ESC` e
> confirma que o foco voltou ao botao de origem.

⚠️ **O que a onda 2 deixa para ela:** a densidade hoje alcanca os controles da onda 1 e o
`RvmNavItem`. Componente de dado (onda 4) precisa entrar na conta dos tokens `--rvm-control-*`,
ou nasce ignorando a densidade em silencio.

<details>
<summary>Escopo original da onda 1, para referencia</summary>

### Onda 1 — Tokens e basicos

Meta: um formulario completo, coerente e acessivel, montado so com componentes `Rvm*`.
Fecha com o pacote `0.1.0` no BaGet e o site publicando os fundamentos.

- ~~Camada de tokens (cor, tipografia, espacamento, raio, sombra, motion, z-index, breakpoint)~~ [[DSGN-002]]
- ~~`RvmTheme` + `FromSeed`~~ [[DSGN-002]] · falta `RvmThemeProvider` + script anti-flash
- ~~Teste de contraste de toda paleta — o portao do `06`~~ [[DSGN-002]]
- `RvmButton`, `RvmIconButton`, `RvmTextField`, `RvmTextArea`, `RvmSelect`, `RvmCheckbox`,
  `RvmRadioGroup`, `RvmSwitch`, `RvmLabel`/`RvmFormField`
- Site: casca, fundamentos, pagina por componente, codigo copiavel, instalacao, changelog

✅ Todas as pendencias de contrato do `09-roadmap` foram fechadas antes da release.

</details>

### Ondas 2 a 4

Layout, Feedback e Dados — escopo e criterio de saida em `09-roadmap.md`. Cards saem quando a
onda anterior fechar.


