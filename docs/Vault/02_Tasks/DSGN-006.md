---
id: DSGN-006
titulo: Onda 4 — dados e shell (8 componentes)
repo: RVM.DesignSystem
tipo: feature
status: em andamento
criada: 2026-09-17
---

# DSGN-006 — Onda 4: dados e shell

Os oito da onda 4 do `11-catalogo-de-componentes.md`: `RvmTable`, `RvmDataGrid`, `RvmDatePicker`,
`RvmTimePicker`, `RvmStepper`, `RvmTimeline`, `RvmAppShell`, `RvmRating`. Com ela fecham as quatro
ondas — o criterio do `1.0.0` (`09-roadmap.md`), que so sai com sinal verde do Rafael.

Vale tudo o que as ondas 1 a 3 pagaram (cards `DSGN-003` a `DSGN-005`): medir no PNG antes de escrever
CSS; `-text` para escrever ou indicar com cor; prefixo `rvm-` em toda classe interna (guardado por
teste); numero em atributo na cultura invariante; nada de `RenderTreeBuilder.OpenElement` com CSS
isolado; subir o site local e rodar axe + E2E antes do PR, inclusive com o que abre aberto; portao pelo
exit code; arquivo novo pela ferramenta Write.

## Fatias (por dependencia e por risco)

1. **Exibicao**: `RvmRating`, `RvmStepper`, `RvmTimeline`
2. **Escolha de data e hora**: `RvmDatePicker` (data e intervalo), `RvmTimePicker` (12 h e 24 h)
3. **Dados**: `RvmTable` (basica, ordenavel, com selecao, densa), `RvmDataGrid` (paginacao, ordenacao,
   filtro por coluna)
4. **Shell**: `RvmAppShell` (topo + menu lateral + conteudo, responsivo, menu recolhido)

## Andamento

## Decisoes e medicoes

| O que | Por que |
|---|---|

## Verifica (cada fatia)

1. Build Release 0 warning · testes verdes pelo exit code · cobertura >= 80%
2. Pagina no site com exemplo, parametros e recorte do kit
3. axe local nos dois temas, inclusive com o que abre aberto; E2E local e no dev
4. Teclado: o padrao ARIA de cada componente, testado de verdade
5. Comparacao lado a lado com o PNG do kit, aprovada pelo Rafael
