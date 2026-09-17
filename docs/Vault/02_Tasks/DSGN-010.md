---
id: DSGN-010
titulo: Graficos — colunas, barras, linha, area, pizza, histograma, dispersao e radar
repo: RVM.DesignSystem
tipo: feature
status: em andamento
criada: 2026-09-17
---

# DSGN-010 — Graficos

Pedido do Rafael em 17/09 ("voce consegue criar componentes de graficos?" + "pode prosseguir"): os oito
tipos principais. **Reabre o escopo**: o `CLAUDE.md` listava "grafico" em "Nao faz" da v1 — a decisao
agora e dele, registrada aqui. Componentes novos, sem quebrar contrato: **1.1.0**.

Referencia visual: `referencia-neatlab/Chart Card.png` e `Analytics.png` (colunas arredondadas, grade
tracejada, rosca, linha e area com degrade, radar, barras). Histograma e dispersao nao existem no kit:
seguem o mesmo visual.

## Arquitetura

- **SVG gerado no componente, sem biblioteca e sem JS para desenhar**: o grafico sai pronto no primeiro
  render (inclusive SSR). JS so mede a largura real (ResizeObserver) para o texto nao escalar.
- Series declaradas como as colunas da tabela (`RvmChartSeries`), base `RvmChartBase<TItem>` com escalas,
  eixos, legenda, dica, teclado e a tabela de dados escondida para leitor de tela.
- Moldura interna com o CSS isolado; o desenho de cada tipo e marcacao Razor (nada de `OpenElement`).
- Numeros em atributo SVG sempre em cultura invariante.
- Acessibilidade: figura com nome, tabela com os mesmos dados para leitor de tela, area do grafico focavel
  com setas lendo os valores (regiao `aria-live`), dica no mouse e no teclado. Cores de serie nos tokens
  `-text` (3:1 como indicador grafico).

## Fatias

1. [x] Base + `RvmColumnChart` (agrupado e empilhado), `RvmBarChart`, `RvmHistogram`
2. [ ] `RvmLineChart` (reta e suave), `RvmAreaChart`, `RvmScatterChart`
3. [ ] `RvmPieChart` (pizza e rosca), `RvmRadarChart`; Dashboard de exemplo com graficos

## Fora da v1 dos graficos

Zoom, arrastar, animacao, eixo duplo, exportar imagem.

## Decisoes

| Decisao | Por que |
|---|---|
| **Base `RvmChartBase<TItem>` em .razor com a figura; cada grafico e .razor que chama `base.BuildRenderTree(__builder)` e sobrescreve so o `Desenho`** | O desenho fica em marcacao Razor (nada de `OpenElement`, armadilha 4); o CSS mora na base e chega ao SVG do derivado por `::deep`. Cada derivado declara de novo `[CascadingTypeParameter]`: a inferencia do tipo das series nao herda da base |
| **Texto do SVG por `MarkupString` (`Texto(...)`, conteudo codificado)** | Em .razor a tag `<text>` e reservada do Razor ("so texto", sem atributo) e o erro de parse contaminou o arquivo inteiro. O Blazor insere markup em contexto SVG corretamente (conferido no navegador) |
| **Largura: SVG com `viewBox` na largura medida (ResizeObserver); 600 px ate medir** | Texto nitido na largura real sem depender de JS para aparecer. Pego no E2E: `viewBox` igual a largura da area |
| **Leitura acessivel: camada focavel `role=group` + setas + `aria-live`; tabela de dados escondida; SVG `aria-hidden`** | O desenho nao e navegavel por leitor de tela; a tabela entrega tudo, e o teclado percorre ponto a ponto com a dica visivel |
| **Formato padrao `RvmChartFormat.Compact` em PT-BR fixo** ("60 mil", "1,2 mi") | Nao depender da cultura do navegador; o kit escreve "60k" |
| **Barras horizontais e histograma sem recorte do kit** (lista de excecoes do E2E) | O kit so desenha colunas; os dois seguem o visual delas |
