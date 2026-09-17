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
2. [x] `RvmLineChart` (reta e suave), `RvmAreaChart`, `RvmScatterChart`
3. [x] `RvmPieChart` (pizza e rosca), `RvmRadarChart`; Dashboard de exemplo com graficos

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
| **Curva suave monotona (Fritsch-Carlson, o `monotoneX` do d3)** | Passa por todos os pontos sem subir alem do maior nem descer abaixo do menor entre vizinhos; Catmull-Rom inventava picos. Teste confere que nenhum ponto de controle passa do topo |
| **Area: degrade `linearGradient` por serie, com a cor herdada pela variavel CSS; eixo sempre do zero** | Area cortada no meio exagera o volume |
| 🔴 **Campo estatico em classe generica repetia ids** (`RvmChartBase<TItem>`, e ja antes `RvmSelectBase<TValue>` e `RvmTable<TItem>`) | Existe um campo por tipo: dois graficos com tipos de dado diferentes nasciam "rvm-grafico-1" e o degrade da area verde saiu azul (o do outro grafico). Selects `string` e `TimeOnly?` na mesma pagina repetiam o id do combobox. `GeradorDeIds` unico na biblioteca + teste de regressao |
| **Pizza: valor por item (`Value` no proprio grafico), fatias a partir do alto no sentido horario; 100% vira arco de 359,999 graus; zero e negativo nao desenham fatia mas ficam na tabela** | O arco de 360 graus nao existe no SVG. A fatia ativa se afasta 6 px, como no Chart Card do kit |
| **Radar: pelo menos 3 eixos; `Max` para escala comum** | Com 2 eixos nao ha poligono; sem `Max`, o maior valor arredondado |
| **Dashboard de exemplo ganhou colunas, rosca e area** | Os graficos na tela que motivou o pedido |
