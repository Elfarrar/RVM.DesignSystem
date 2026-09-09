---
id: DSGN-039
titulo: Linha de texto com 109 caracteres
repo: RVM.DesignSystem
tipo: bug
status: concluido
criada: 2026-09-09
atualizada: 2026-09-09
---

# DSGN-039 — Larga demais para ler

## Descricao

Medido em 09/09/2026 no `/padroes/dashboard`, a 1280px: os paragrafos de texto corrido chegam a
**92 e 109 caracteres** por linha. O intervalo confortavel e **65–75**; acima disso o olho perde a
volta da linha e reencontra a mesma duas vezes.

Nao e defeito de uma pagina: e o site inteiro, porque nao existe token de largura de leitura.
Componente nenhum tem culpa — o texto simplesmente ocupa o que a coluna oferece.

## Plano

1. Token novo, `--rvm-measure` (ou nome equivalente), em `ch`, na camada de tokens. Largura de
   leitura e decisao de sistema, igual a escala tipografica.
2. Aplicar no texto corrido do site.
3. ⚠️ Nao aplicar em tabela, grade ou codigo — `ch` para bloco de codigo estraga a leitura de
   linha longa, que ali e legitima.

## Validacao

- [x] Paragrafo de texto corrido entre 65 e 75 caracteres a 1280px.
- [x] Tabela e bloco de codigo intactos.

## Versao

Se o token entrar na biblioteca: **minor** (token novo e aditivo).

## Resultado (09/09/2026) — EM PRODUCAO

Texto corrido a **73 caracteres** por linha, contra os 109 medidos antes. Tabela, grade e
bloco de codigo intactos.
