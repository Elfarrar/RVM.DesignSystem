---
id: DSGN-035
titulo: Os quatro KPIs do Dashboard prometem destinos diferentes e vao para o mesmo
repo: RVM.DesignSystem
tipo: bug
status: concluido
criada: 2026-09-09
atualizada: 2026-09-09
---

# DSGN-035 — O link que nao cumpre o rotulo

## Descricao

Confirmado no DOM: os quatro cards de KPI de `/padroes/dashboard` tem `aria-label` especificos —
"Pedidos no mes: 487. Ver na listagem.", "Cancelados: 9. Ver na listagem." — e os quatro apontam
para o **mesmo** `padroes/listagem`, sem filtro nenhum.

Quem clica em dois cards diferentes cai na mesma tela duas vezes. O `PRODUCT.md` diz que o leitor
que manda e o entrevistador tecnico com dois minutos: e exatamente ele quem clica em dois cards.
Le como prototipo inacabado, que e o oposto do sinal pretendido.

## Plano

Duas saidas, e a escolha e do Rafael:

1. **Cumprir a promessa** — `padroes/listagem?situacao=cancelado`, e a Listagem le o parametro e ja
   abre filtrada. Mais trabalho, e de quebra demonstra um padrao que todo app de negocio precisa
   (link que chega filtrado).
2. **Parar de prometer** — tirar o link dos cards, ou dizer no texto da pagina que ele e
   ilustrativo.

Recomendo a **1**: a Listagem ja tem filtro; falta so ler da query string.

## Validacao

- [x] Cada card leva a um resultado diferente, ou o texto diz que o link e ilustrativo.
- [x] E2E cobrindo pelo menos um card que chega filtrado (se a saida for a 1).

## Resultado (09/09/2026) — EM PRODUCAO

Os destinos agora sao `padroes/listagem`, `padroes/listagem`,
`padroes/listagem?situacao=Pendente` e `padroes/listagem?situacao=Cancelado`. E2E novo
confere o resultado na tela, e pegou de cara um erro meu: a primeira versao do teste contava
colunas e a quarta e a data.
