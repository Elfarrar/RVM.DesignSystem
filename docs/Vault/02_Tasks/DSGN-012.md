---
id: DSGN-012
titulo: Documentar animacao e exportar nas cinco paginas de grafico que ficaram de fora
repo: RVM.DesignSystem
tipo: docs
status: concluida
criada: 2026-09-17
---

# DSGN-012 — O que a DSGN-011 entregou mas nao documentou

A `DSGN-011` documentou os recursos novos so onde ficaram os exemplos: `ColumnChart`, `LineChart` e
`ScatterChart`. **`Animated`, `ExportAsync` e `ExportFileName` valem em TODOS os graficos** (moram na
base), e quem abrisse a pagina do radar ou da rosca nao descobria que dava para exportar.

Pedido do Rafael em 17/09, depois de perguntar o que estava pendente.

## O que entra

- `AreaChart`: herda da linha, entao ganha a lista inteira — `Animated`, `ExportAsync`/`ExportFileName`,
  `Zoomable`, `SelectionMode`/`Selection`/`SelectionChanged`, `RvmChartSeries.Axis` e `SecondaryValueFormat`.
- `BarChart`, `Histogram`, `PieChart`, `RadarChart`: `Animated` e `ExportAsync`/`ExportFileName`, mais
  uma linha dizendo **o que NAO vale ali** — zoom, arrasto, selecao de faixa e eixo duplo aparecem no
  IntelliSense por heranca e nao tem efeito, porque nao ha eixo cartesiano para recortar.

Essa ultima linha existe por causa do review da `DSGN-011`: a rosca com `Zoomable` chegou a **anunciar
teclas que nao funcionam** para o leitor de tela. O codigo ja foi corrigido; a documentacao fechava o
mesmo buraco pelo lado de quem le.

## Entregue em 17/09/2026

PR #54 no dev: as cinco paginas com as linhas novas, 591 testes unitarios e 229 E2E verdes, fotos das
tabelas nos dois temas em `docs/fotos/dsgn-012`. Publicada na **`1.3.0`** em 18/09/2026, junto da DSGN-013.

## Fora

Ligar zoom ou selecao em exemplo novo (o pedido foi documentar, nao mudar as telas) e qualquer
mudanca de codigo da biblioteca.

## Decisoes

| Decisao | Por que |
|---|---|
| Dizer na tabela o que **nao** vale, em vez de omitir | o parametro aparece no IntelliSense de qualquer jeito; silencio deixaria o consumidor descobrir na tentativa |
| Sem exemplo novo de exportar nessas paginas | o menu de exportar ja esta demonstrado no `RvmColumnChart`; repetir cinco vezes so engorda o site |
