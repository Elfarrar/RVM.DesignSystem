# Kanban — RVM.DesignSystem

> **Este arquivo e INDICE: uma linha por card.** Contexto, plano e decisao vivem no card
> (`02_Tasks/DSGN-NNN.md`), nunca aqui. **Alvo: cabe em duas telas.**
>
> **O card nasce ANTES do codigo**, com o escopo e as decisoes em aberto.
>
> Modo de trabalho: **Claude implementa, o Rafael revisa** (`CLAUDE.md § Modo de trabalho`).

## Em revisao

_(vazio — a onda 1 fechou)_

## Concluido

- [[DSGN-001]] — Bootstrap + **producao no ar** (08/09/2026): repo publico, esqueleto .NET 10,
  5 workflows, DNS/SSL nos dois ambientes, `design.dev.rvmtech.com.br` e `design.rvmit.com.br`.
  ⏳ So o monitor no Uptime-Kuma ficou de fora — o Kuma nao tem canal de notificacao nenhum.
- [[DSGN-003]] — `CNAME` no artefato, borda do titulo removida e 404 do CSS isolado consertado.
- [[DSGN-004]] — `Verify` do primeiro deploy de Pages: o site subia certo e o job ficava vermelho.

## A fazer

### ✅ Onda 1 — fechada em 08/09/2026, publicada como `0.1.0`

Nove componentes, camada de tokens, motor de tema, icones Phosphor e o site de documentacao.
`design.rvmit.com.br` no ar. Cards: [[DSGN-002]], [[DSGN-012]], [[DSGN-013]], [[DSGN-014]],
[[DSGN-015]].

### Onda 2 — Layout (proxima)

`RvmAppShell`, `RvmSidebar` (colapsavel, drawer no mobile), `RvmTopbar`, `RvmNavItem`,
`RvmCard`, `RvmGrid`/`RvmStack`, `RvmTabs`, `RvmBreadcrumb`, `RvmDivider`, `RvmChip`,
`RvmAvatar`, densidade `Compact`. Fecha em `0.2.0`.

⚠️ **Decisao de contrato que sobra da onda 1:** quais icones entram no sprite. Cada um e nome
publico e bytes que todo consumidor baixa — a lista e curada junto dos componentes que os usam.

> **Meta da onda 2 (`09-roadmap`):** o proprio site passa a usar `RvmAppShell` — a casca deixa
> de ser codigo de exemplo e vira codigo em producao.

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


