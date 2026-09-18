---
id: DSGN-013
titulo: Graficos — eixo duplo em barras, zoom por caixa, exportar .xlsx e imprimir
repo: RVM.DesignSystem
tipo: feature
status: concluida
criada: 2026-09-18
---

# DSGN-013 — O que a DSGN-011 deixou de fora, a pedido

Pedido do Rafael em 18/09, depois do levantamento de pendencias: "pode implementar o eixo duplo [em
barras], pode implementar tbm o zoom [por caixa], o .xls e o imprimir". Tudo o que a `DSGN-011` tinha
registrado em "Fora", mais a lacuna do eixo duplo nas barras horizontais.

Recursos novos sem quebrar contrato: **1.3.0**.

## Fatias

1. [x] **Arrumacao do repo** (pedido junto): `docs/fotos/` no `.gitignore`; `.claude/` deixa de ser
       ignorado, com o lock de estado local de fora.
2. [x] **Eixo duplo em barras horizontais**: nas barras o eixo de valor e o X, entao o segundo eixo vai
       no **topo**, nao a direita.
3. [x] **Zoom por caixa desenhada** (`RvmChartSelectionMode.ZoomBox`): arrastar desenha um retangulo e
       soltar aproxima nele, nos dois eixos de uma vez.
4. [x] **Exportar `.xlsx`** (`RvmChartExportFormat.Xlsx`, planilha montada como ZIP de XML, sem
       biblioteca de terceiros) e **imprimir** (`PrintAsync()`).

## Decisoes de partida

- **`ZoomBox` entra no `RvmChartSelectionMode`**, que ja e "o que o arrasto faz" — e nao num parametro
  novo. Valor novo em enum e aditivo: nao quebra quem esta na `1.2.0`.
- **`ZoomBox` liga o zoom por inteiro** (roda, teclado, duplo clique para voltar). Um grafico que
  aproxima e nao sabe voltar pelo teclado nao passaria no AA.
- **`.xlsx` leva numero, nao texto formatado.** O CSV existe para abrir e ler; a planilha existe para
  somar. As celulas numericas saem como `t="n"`, as demais como texto embutido (sem `sharedStrings`).
- **Imprimir e do navegador.** `PrintAsync()` monta um iframe com o SVG ja serializado (cores e fontes
  embutidas, como no exportar) e chama `print()`. Sem JS, devolve `false` — como o `ExportAsync`.
- O eixo do topo faz a **margem de cima crescer**, do mesmo jeito que o eixo da direita fez a margem
  direita crescer na `DSGN-011`.

## Fora

Grafico de barras empilhado com os dois eixos misturados numa pilha so (cada eixo empilha o seu, como
nas colunas), `.xls` antigo (formato binario de 1997) e imprimir varios graficos numa pagina so.

## Decisoes

| Decisao | Por que |
|---|---|
| `ZoomBox` entrou no `RvmChartSelectionMode` | o enum ja e "o que o arrasto faz"; valor novo e aditivo e nao quebra quem esta na `1.2.0` |
| `ZoomBox` liga o zoom inteiro, mesmo sem `Zoomable` | aproximar com o mouse sem caminho de volta pelo teclado reprovaria no AA |
| Caixa menor que 8 px e clique, nao arrasto | sem isso, um clique distraido aproximava 50x |
| `.xlsx` com numero de verdade (`<v>`), texto embutido (`inlineStr`) | o CSV e para ler, a planilha e para somar; `inlineStr` dispensa a tabela de textos compartilhados e uma parte a mais no ZIP |
| Imprimir monta um `iframe` com `srcdoc`, e nao `document.write` | `srcdoc` nao depende de escrever num documento aberto, e o quadro sai de cena sozinho depois |
| Icones `Printer` e `Table` copiados do Tabler oficial | o menu pedia os dois e o conjunto curado nao tinha; desenhar "parecido" seria atribuir ao Tabler um traco que nao e dele |
| O exemplo de barras virou "Area plantada e produtividade de cada talhao" | o nome anterior CONTINHA o de outro grafico da mesma pagina, e o Playwright casa nome acessivel por substring: os dois viravam o mesmo alvo |

## Review independente (Sonnet, 18/09/2026) — 1 P1 e 2 menores, corrigidos

| # | O que | Correcao |
|---|---|---|
| **P1** | `.xlsx` saia **corrompido, sem erro nenhum**, quando o valor era `NaN`/infinito (conta de razao com denominador zero: produtividade = producao / area) ou quando o texto trazia caractere de controle. `"NaN"`/`"Infinity"` dentro de `<v>` nao e double valido, e byte de controle deixa o XML malformado — o navegador baixava um arquivo que o Excel recusa | `NaN` e infinito saem do ramo numerico e viram texto; `Texto()` descarta os caracteres de controle que o XML 1.0 nao aceita (tab, LF e CR continuam valendo) |
| P2 | Zoom por caixa apertava **sempre os dois eixos**: quem arrastasse pensando em "so a faixa de datas" perdia o eixo vertical por uma deriva de mao de 3 px | so aperta o eixo em que a caixa tem 8 px ou mais; abaixo disso aquele eixo fica como estava |
| P2 | Dois cliques seguidos em "Imprimir" deixavam dois quadros e duas caixas de dialogo | o quadro anterior sai de cena antes de o novo entrar |
| P3 | Aba comecando ou terminando com apostrofo (o Excel recusa) | filtrada junto com `:\/?*[]` |

O E2E do `.xlsx` so conferia a assinatura `PK` do ZIP: agora ha teste unitario que **abre todas as
partes do pacote e valida o XML** com `XmlDocument`. 604 testes unitarios e 232 E2E verdes.

## Entregue em 18/09/2026

- Eixo duplo em barras: segundo eixo no **topo**, com a margem de cima crescendo para caber os rotulos;
  legenda e tabela dizendo "eixo de baixo"/"eixo de cima".
- `RvmChartSelectionMode.ZoomBox`: caixa tracejada enquanto o botao esta apertado, aproxima nos dois
  eixos ao soltar; Shift+arrastar continua deslocando.
- `RvmChartExportFormat.Xlsx` (`RvmChartXlsx`, ZIP de XML montado a mao) e `PrintAsync()`.
- Menu do `RvmColumnChart` com "Planilha XLSX" e "Imprimir"; dispersao com zoom por caixa ligado.
- 601 testes unitarios e 232 E2E verdes; fotos em `docs/fotos/dsgn-013`.
