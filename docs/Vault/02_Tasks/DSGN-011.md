---
id: DSGN-011
titulo: Graficos — animacao, exportar, eixo duplo, zoom, arrastar e selecao
repo: RVM.DesignSystem
tipo: feature
status: concluida
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
3. [x] **Zoom em X e em Y** e **arrastar**: roda do mouse (Shift para Y), arrastar para deslocar, duplo
       clique para voltar, `+`/`-` e setas no teclado, janela anunciada em `aria-live`
4. [x] **Selecao de faixa por arrasto** (`SelectionMode`, `Selection`/`SelectionChanged`) para filtrar outros
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

## Publicada em 17/09/2026

`1.2.0` no BaGet (tag `v1.2.0` presa ao commit do master, versao conferida no feed) e producao no ar
em `https://design.rvmit.com.br` pelo GitHub Pages. Promocao com sinal verde explicito do Rafael
("pode executar"). E2E contra producao: 30 testes verdes, incluindo baixar os quatro arquivos, o eixo
duplo, o zoom pela roda e pelo teclado e a selecao filtrando a tabela. Alphas de `dev` passam a
`1.3.0-alpha.N`.

## Review independente (Sonnet, 17/09/2026) — 2 P1 e 5 menores, todos corrigidos

| # | O que | Correcao |
|---|---|---|
| P1 | Arrastar a selecao ate passar da borda do desenho COLAPSAVA a faixa (a camada de eventos cobre a figura inteira; `PontoEm` devolvia nulo na margem e o codigo caia de volta no ponto inicial) | `PontoDoArrasto` prende o ponteiro na area do desenho antes de perguntar o ponto |
| P1 | `ExportAsync(Pdf)` estourava `ArgumentException` (imagem de altura zero) em vez de devolver `false`, quebrando o contrato que os outros formatos cumprem | `ArgumentException` e `FormatException` entraram no filtro do catch |
| P2 | Instrucao e `aria-live` de zoom/selecao apareciam pelo parametro cru: uma rosca com `Zoomable` prometia teclas que nao existem | a marcacao passou a perguntar por `ZoomLigado`/`SelecaoLigada` |
| P2 | Arrastar para deslocar comecava selecao de texto do navegador | `user-select: none` na camada e `preventDefault` tambem no zoom |
| P2 | Ligar `Zoomable` depois do primeiro render nao ligava a roda do mouse (e desligar deixava o ouvinte pendurado) | `AcertarARoda` roda a cada render |
| P2 | Zoom num grafico de um ponto so podia empurrar o ponto para fora do recorte | com um ponto, `XDaFracao` ignora a janela |
| P2 | `RvmChartRange(4, 2)` passava, com `Count` negativo | o construtor recusa fim antes do inicio |
| P3 | `Escala.Marcas` somava o passo a cada volta (erro acumulado); `LadosDaFaixa` nao prendia indices velhos; o exemplo do Dashboard indexava o array pelo indice cru | marcas por indice, clamp nos tres graficos, legenda pela lista ja filtrada |

591 testes unitarios (6 novos so destes achados) e 229 E2E verdes depois das correcoes.

## Fatia 4 — entregue em 17/09/2026

- `SelectionMode="RvmChartSelectionMode.Range"`, `Selection` (`RvmChartRange`) e `SelectionChanged`.
  A faixa e de **indices** dos itens: quem recebe filtra os proprios dados, o grafico so diz qual e.
- Arrastar marca; com `Zoomable` junto, Shift+arrastar passa a ser o que desloca. Clicar sem arrastar
  limpa. No teclado: Shift+setas estendem a partir do ponto que esta sendo lido, Enter marca so ele e
  Esc limpa — as setas sozinhas continuam a leitura ponto a ponto.
- `Selection` de fora e adotado pelo mesmo criterio da `RvmTable` (compara a referencia), para um
  render do pai nao desfazer o que o leitor acabou de marcar.
- Exemplo no **Dashboard**: a faixa no grafico de receita filtra a tabela de fechamento, com "Limpar
  filtro" no cabecalho do cartao.
- O veu da faixa usa `--rvm-color-primary-text` (a primeira tentativa, `--rvm-primary`, nao existe e
  saiu invisivel — achado na foto).

## Fatia 3 — entregue em 17/09/2026

- `Zoomable` (padrao **desligado**: a roda do mouse pertence a pagina ate o consumidor decidir o contrario).
- Roda aproxima o eixo X, Shift+roda o Y, arrasto desloca, duplo clique volta. No teclado: `+`, `-`
  (com Shift, o eixo vertical), `0` e Ctrl+setas — as setas sozinhas continuam lendo ponto a ponto.
- O zoom mexe na **janela do dominio**, nao numa transformacao do desenho: os rotulos do eixo sao
  recalculados (`Escala.Recortada`) e continuam em numeros redondos, e o que sai da area e cortado.
- Vale em colunas, linha, area e dispersao — os graficos com area de desenho cartesiana.
- A janela e anunciada em `aria-live` e a instrucao do leitor de tela ganha as teclas.
- A roda entra pelo JS (`observarRoda`): o ouvinte precisa poder cancelar a rolagem da pagina, e o
  Blazor registra os dele como passivos. Sem JS, o grafico continua desenhado, navegavel e com zoom
  pelo teclado.
- De quebra: `-0` no eixo (a marca da base cai em -1e-14 depois do recorte) virou `0`.

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
