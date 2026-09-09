---
id: DSGN-036
titulo: A mesma situacao com duas cores na mesma tela
repo: RVM.DesignSystem
tipo: bug
status: concluido
criada: 2026-09-09
atualizada: 2026-09-09
---

# DSGN-036 — Aprovado e verde no chip e roxo na barra

## Descricao

Medido na producao, na mesma tela:

| Onde | Aprovado | Entregue |
|---|---|---|
| Chip de "Ultimos pedidos" | `#08762F` (verde) | `#0068B8` (azul) |
| Barra de "Faturamento por situacao" | `#641974` (primaria) | `#641974` (primaria) |

As quatro situacoes tem quatro cores num lugar e uma cor so no outro.

Isso **pode** ser deliberado — num grafico de proporcao, pintar cada fatia com a cor semantica faz
o vermelho de "cancelado" parecer alarme quando ele e so uma fatia pequena de um total saudavel. Se
for esse o motivo, tudo bem; o problema e que a pagina documenta **cada outra decisao** numa tabela
("O que esta tela decide por voce") e esta ficou de fora.

## Plano

Decidir e registrar, nesta ordem:

1. Se for deliberado: uma linha na tabela de decisoes explicando por que o grafico nao usa a cor
   semantica. Custo: um paragrafo.
2. Se nao for: alinhar as barras as cores semanticas.

⚠️ Se a escolha for alinhar, **medir contraste antes**: quatro cores de estado lado a lado numa
barra fina tem que se distinguir tambem para quem nao enxerga cor — o que exige rotulo, nao so cor.

## Validacao

- [x] A tela mostra uma regra so, ou a excecao esta escrita na tabela de decisoes.

## Resultado (09/09/2026, verificado no dev)

A decisao esta na tabela da propria pagina. Ficou registrado o que sustenta a escolha: chip
e **estado** de um pedido, barra e **participacao** no faturamento — pintar 8% de cancelado de
vermelho seria alarme permanente.
