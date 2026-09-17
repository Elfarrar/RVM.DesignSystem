---
id: DSGN-005
titulo: Onda 3 — feedback e sobreposicao (8 componentes)
repo: RVM.DesignSystem
tipo: feature
status: em andamento
criada: 2026-09-17
---

# DSGN-005 — Onda 3: feedback e sobreposicao

Os oito da onda 3 do `11-catalogo-de-componentes.md`: `RvmDialog`, `RvmDrawer`, `RvmSnackbar`,
`RvmProgress`, `RvmSkeleton`, `RvmAccordion`, `RvmList`, `RvmEmptyState`.

Vale tudo o que as ondas 1 e 2 pagaram (cards `DSGN-003` e `DSGN-004`): medir no PNG antes de
escrever CSS; `-text` para escrever ou indicar com cor; subir o site local e rodar axe + E2E antes do
PR; portao pelo exit code; axe tambem com o que abre ABERTO; classe CSS nao generica; arquivo novo
pela ferramenta Write.

## Fatias (por dependencia e por risco)

1. **Indicadores sem estado**: `RvmProgress`, `RvmSkeleton`, `RvmEmptyState` — o kit nao tem pagina
   para nenhum dos tres; o visual sai dos tokens e das telas do kit onde eles aparecem
2. **Conteudo que expande**: `RvmList`, `RvmAccordion`
3. **Sobreposicao**: `RvmDialog`, `RvmDrawer`, `RvmSnackbar` — foco preso, Esc, fundo inerte, fila;
   os de maior risco, por ultimo

⚠️ Regra que pesa mais nesta onda: **nenhum componente depende de JS para o estado inicial**. Um
dialogo aberto no primeiro render tem de aparecer aberto sem JS; mover e prender o foco pode usar o
`FocusAsync` do Blazor, que e melhoria, nao pre-requisito.

## Andamento

## Decisoes e medicoes

| O que | Por que |
|---|---|

## Verifica (cada fatia)

1. Build Release 0 warning · testes verdes pelo exit code · cobertura >= 80%
2. Pagina no site com exemplo, parametros e recorte do kit (quando o kit tem pagina)
3. axe local nos dois temas, inclusive com o que abre aberto; E2E local e no dev
4. Teclado: o padrao ARIA de cada componente, testado de verdade
5. Comparacao lado a lado com o PNG do kit, aprovada pelo Rafael
