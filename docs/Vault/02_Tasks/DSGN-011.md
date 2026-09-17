---
id: DSGN-011
titulo: Graficos — animacao, exportar, eixo duplo, zoom, arrastar e selecao
repo: RVM.DesignSystem
tipo: feature
status: em andamento
criada: 2026-09-17
---

# DSGN-011 — O que tinha ficado fora dos graficos

Pedido do Rafael em 17/09, depois da `1.1.0`: "nao deixe nada de fora, considere zoom no eixo Y, selecao
de faixa por arrasto para filtrar outros componentes, e exportar PDF ou CSV tambem". Ou seja, tudo o que a
`DSGN-010` deixou de lado, mais tres itens novos. Recursos novos sem quebrar contrato: **1.2.0**.

## Fatias

1. [x] **Animacao** (`Animated`, desligada sozinha em "reduzir movimento") e **exportar** em PNG, SVG, CSV
       e PDF (`ExportAsync`), com menu de exportar no exemplo
2. [x] **Eixo duplo**: `RvmChartAxis.Secondary` na serie, escala e rotulos a direita, legenda dizendo o eixo
3. [ ] **Zoom em X e em Y** e **arrastar**: roda do mouse (Shift para Y), arrastar para deslocar, duplo
       clique para voltar, `+`/`-` e setas no teclado, janela anunciada em `aria-live`
4. [ ] **Selecao de faixa por arrasto** (`SelectionMode`, `Selection`/`SelectionChanged`) para filtrar outros
       componentes, com teclado (Shift+setas, Enter, Esc) e exemplo no Dashboard filtrando a tabela

## Decisoes de partida

- **Zoom e arrastar dependem de JS** (a roda do mouse pede um ouvinte nao passivo): sao melhoria, nao
  pre-requisito — sem JS o grafico continua desenhado e navegavel pelo teclado.
- **PDF sem biblioteca**: a imagem do grafico vira JPEG no `canvas` e o PDF de uma pagina e montado em C#
  (objeto de imagem `DCTDecode`), o que deixa a geracao testavel.
- **CSV a partir da tabela de dados** que ja existe para leitor de tela: separador `;` e BOM, para abrir no
  Excel em PT-BR sem virar uma coluna so.
- **Arrastar com `SelectionMode`**: o arrasto seleciona; deslocar passa a ser Shift+arrastar. Sem selecao, o
  arrasto desloca.
- Toda interacao nova tem caminho de teclado e anuncio, senao quebra o AA que os graficos ja tem.

## Fora

Zoom por caixa desenhada (brush-to-zoom), exportar Excel nativo (.xlsx), imprimir.

## Decisoes

| Decisao | Por que |
|---|---|
| Exportar entra como `ExportAsync(formato)` no componente, sem botao proprio | quem monta a tela decide onde fica o botao; o grafico nao ganha uma barra de ferramentas que o kit nao tem |
| CSV com numero cheio (`45.000`), nao com o compacto do eixo (`45 mil`) | o CSV existe para abrir no Excel e somar; o compacto vira texto. A tabela do leitor de tela continua compacta |
| PDF montado em C# com o JPEG do `canvas` (`DCTDecode`) | sem biblioteca de terceiros (decisao do projeto) e a montagem fica testavel sem navegador |
| `pathLength="1"` na linha para animar o traco | dispensa medir o caminho em JS: a animacao roda so em CSS e morre sozinha em "reduzir movimento" |
| Eixo escreve `0`, nao `0 mil` | achado na foto da entrega; `CompactForAxis` colocava o sufixo do passo na base do eixo |

## Fatia 2 — entregue em 17/09/2026

- `RvmChartAxis` e `RvmChartSeries.Axis`: a serie escolhe o eixo da esquerda (padrao) ou o da direita,
  com escala propria. `SecondaryValueFormat` da a unidade do eixo novo.
- Vale em **colunas, linha e area** — os graficos de eixo Y vertical. Em barras o eixo de valor e o
  horizontal, e em pizza, radar, histograma e dispersao nao ha duas series de unidades diferentes para
  comparar: ali a serie continua lida no eixo unico, mesmo pedindo o secundario.
- Se **todas** as series pedirem o secundario, o grafico segue com um eixo so: dois eixos identicos
  ocupariam as duas bordas para dizer a mesma coisa.
- A legenda diz "eixo esquerdo"/"eixo direito" e a tabela do leitor de tela marca "(eixo direito)" no
  cabecalho da serie — quem nao ve o desenho tambem sabe de que escala o numero veio.
- Empilhado com dois eixos: cada lado tem a sua pilha.
- Exemplo novo na pagina do `RvmLineChart` (receita em reais x margem em porcento).

## Fatia 1 — entregue em 17/09/2026

- `Animated` (padrao ligado, silenciada por `prefers-reduced-motion`) e `ExportAsync(RvmChartExportFormat)`
  em PNG, SVG, CSV e PDF, com `ExportFileName` para o nome do arquivo.
- Menu **Exportar** no primeiro exemplo do `RvmColumnChart`, com aviso de sucesso ou de recusa.
- 562 testes unitarios e 226 E2E verdes; o E2E novo baixa os quatro arquivos e confere assinatura
  (`%PDF-1.4`, `/Filter /DCTDecode`, cabecalho PNG, `Categoria;Receita;Despesa`).
- Fotos: `docs/fotos/dsgn-011/exportar-claro.png` e `exportar-escuro.png` (fora do git).
