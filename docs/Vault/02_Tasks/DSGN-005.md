---
id: DSGN-005
titulo: Onda 3 — feedback e sobreposicao (8 componentes)
repo: RVM.DesignSystem
tipo: feature
status: em revisao
criada: 2026-09-17
---

# DSGN-005 — Onda 3: feedback e sobreposicao

Os oito da onda 3 do `11-catalogo-de-componentes.md`: `RvmDialog`, `RvmDrawer`, `RvmSnackbar`,
`RvmProgress`, `RvmSkeleton`, `RvmAccordion`, `RvmList`, `RvmEmptyState`.

Vale tudo o que as ondas 1 e 2 pagaram (cards `DSGN-003` e `DSGN-004`): medir no PNG antes de
escrever CSS; `-text` para escrever ou indicar com cor; subir o site local e rodar axe + E2E antes do
PR; portao pelo exit code; axe tambem com o que abre ABERTO; classe CSS nao generica; arquivo novo
pela ferramenta Write.

## Fatias (por dependencia e por risco)

1. **Indicadores sem estado**: `RvmProgress`, `RvmSkeleton`, `RvmEmptyState` — o kit nao tem pagina
   para nenhum dos tres; o visual sai dos tokens e das telas do kit onde eles aparecem
2. **Conteudo que expande**: `RvmList`, `RvmAccordion`
3. **Sobreposicao**: `RvmDialog`, `RvmDrawer`, `RvmSnackbar` — foco preso, Esc, fundo inerte, fila;
   os de maior risco, por ultimo

⚠️ Regra que pesa mais nesta onda: **nenhum componente depende de JS para o estado inicial**. Um
dialogo aberto no primeiro render tem de aparecer aberto sem JS; mover e prender o foco pode usar o
`FocusAsync` do Blazor, que e melhoria, nao pre-requisito.

## Andamento

- [x] **Fatia 1**: `RvmProgress`, `RvmSkeleton`, `RvmEmptyState`
- [x] **Fatia 2**: `RvmList` + `RvmListItem`, `RvmAccordion` + `RvmAccordionPanel`
- [x] **Fatia 3**: `RvmDialog`, `RvmDrawer`, `RvmSnackbarService` + `RvmSnackbarHost` (com `AddRvmDesignSystem()`)

## Decisoes e medicoes

| O que | Por que |
|---|---|
| **Progresso e esqueleto sem referencia no kit** — barra de 4 px, anel nos tamanhos do avatar (24/40/56), esqueleto no `action-selected` | O kit nao tem pagina deles (nem em tela de dashboard). No E2E viraram excecao declarada do recorte do kit, como o `RvmIcon` |
| **Estado vazio segue a composicao da tela `Error.png`** | Titulo, apoio e acao centralizados. O titulo e cabecalho de verdade (`HeadingLevel`) e nomeia a `section` |
| **Trilho do progresso neutro (`action-selected`), nao o tom suave da cor** | Com `-outlined-resting`, o preenchido dava 2.3 a 3.1:1 contra o trilho no escuro (WCAG 1.4.11 pede 3:1). Contra o neutro passa de 5:1 nos dois temas (medido) |
| **Indeterminado sem `aria-valuenow`; esqueleto sempre `aria-hidden`** | E como o leitor de tela sabe que nao ha previsao. Quem avisa "carregando" e a regiao com `aria-busy`, nao cada bloco cinza |
| **Movimento reduzido: progresso desacelera, brilho do esqueleto some** | Parar o indeterminado faria parecer travado; o brilho e so enfeite |
| 🔴 **Numero decimal em atributo de markup sai na cultura corrente** | `r="@Raio"` virou `r="20,2"` no site em pt-BR: atributo invalido, anel invisivel, sem erro nenhum. O bUnit roda em en-US e passou. Pego na foto; agora literal + teste bUnit em pt-BR + E2E medindo a geometria |
| **Medidas da fatia 2**: item de lista 48 px (40 denso), hover no `action-hover`; painel de acordeao 52 px com 20 px de recuo, aberto descolado 16 px no "Simple", cabecalho preenchido no "Customized" | Varredura de linha em `List.png` e `Accordion.png` |
| **Namespace `Components.Lists`, no plural** | Um namespace `List` dentro de `Components` faz todo `List<T>` dos outros componentes resolver para o namespace e quebra o build |
| **Selecionado da lista: `action-selected` + barra de 3 px no `-text`, nao o cinza forte do kit** | O kit poe texto claro sobre #797992, abaixo de 4.5:1. A barra carrega o estado com 3:1 |
| **Item de lista: acao do fim FORA da area clicavel** | Botao dentro de botao (ou de link) e HTML invalido e o leitor de tela so ve um. O `StartContent` fica dentro — documentado que nao aceita controle interativo |
| **Acordeao no padrao APG: botao dentro de cabecalho (h3 por padrao), regiao ligada; fechado nao renderiza** | Quem navega por titulos acha cada painel. `aria-controls` so existe junto com a regiao |
| **Painel e sublista seguem o `Expanded` so quando o parametro MUDA** | Senao um re-render do pai fecharia o que a pessoa abriu sem `@bind` |
| **Variantes do acordeao no CSS do PAI, com `::deep`** | A classe da variante esta no elemento do `RvmAccordion`; o CSS isolado do painel nao enxerga o pai |
| 🔴 **Elemento criado por `RenderTreeBuilder.OpenElement` nao recebe o escopo do CSS isolado** | O `h{n}` dinamico do painel e do estado vazio ficou sem `b-xxxx`: `.cabecalho { margin: 0 }` nao pegou e o painel saiu com 85 px. Pego na foto; agora `@switch` em marcacao Razor + teste bUnit que exige o atributo `b-` |
| **Medidas da fatia 3**: dialogo de 600 px (400/900 nos outros tamanhos) no papel; gaveta de 320 px; snackbar de 320 x 48 px; toast no papel com o icone do papel | Varredura em `Dailog.png`, `Drawer.png`, `Snackbar.png` e `Toast.png` |
| **Dialogo e gaveta temporaria: sem JS abrem e fecham; o `rvm-sobreposicao.js` so prende o foco, trava a rolagem e devolve o foco** | Regra da onda: estado inicial sem JS. O foco vai para a propria caixa (`tabindex=-1`), para o leitor de tela ler o titulo; o primeiro Tab chega no primeiro controle |
| **Confirmacao = `alertdialog`, corpo em `aria-describedby`, fundo nao fecha** | Padrao APG: a pessoa precisa responder |
| **Gaveta temporaria e dialogo modal; a permanente e `aside` no fluxo** | A temporaria bloqueia a pagina enquanto aberta; a permanente e so um painel lateral |
| **Snackbar e toast viraram um componente: `Color` nulo e o neutro do kit; com cor, o toast com icone do papel** | O catalogo pede "6 papeis"; o kit tem as duas paginas. Uma API so |
| **Snackbar por servico (`AddRvmDesignSystem()` + `RvmSnackbarHost` no layout), fila com no maximo 3 na tela** | Chamar de qualquer lugar (depois de salvar, num catch) sem passar referencia de componente |
| **Duas regioes vivas SEMPRE no DOM: `status` para o resto, `alert` so para erro** | Leitor de tela so anuncia o que entra numa regiao que ja existia |
| **O tempo da mensagem para com mouse ou foco em cima e recomeca inteiro ao sair** | WCAG 2.2.1. Recomecar inteiro evita sumir no meio da leitura |
| **Snackbar neutro no tema escuro: `#212121` com texto branco, como o kit** | O token dizia `#FFFFFF`, mas o `Snackbar.png` escuro mostra `#212121`. **Rafael decidiu em 17/09: vale o kit.** Token e `06-tokens` corrigidos |
| 🔴 **Classes genericas do CSS GLOBAL do site vazavam para 11 componentes** (`.conteudo`, `.topo`, `.marca`, `.menu`, `.apoio`, `.barra`, `.quadrado`) | O isolamento de CSS protege o componente de vazar, nao de RECEBER regra global. Pego no dialogo (linha sob o titulo); a varredura achou card, avatar quadrado, checkbox, select e outros atingidos. O site agora usa prefixo `site-`. **Rafael decidiu em 17/09: prefixar.** Ver a secao "Prefixo rvm-" abaixo |

## Verifica (cada fatia)

1. Build Release 0 warning · testes verdes pelo exit code · cobertura >= 80%
2. Pagina no site com exemplo, parametros e recorte do kit (quando o kit tem pagina)
3. axe local nos dois temas, inclusive com o que abre aberto; E2E local e no dev
4. Teclado: o padrao ARIA de cada componente, testado de verdade
5. Comparacao lado a lado com o PNG do kit, aprovada pelo Rafael

## Review independente (onda 3 inteira)

Sonnet, 17/09. Achados e o que foi feito:

| Achado | Resolucao |
|---|---|
| 🔴 **P0 — abrir e fechar rapido travava a rolagem da pagina para sempre.** O `OnAfterRenderAsync` nao espera o anterior: o fechar rodava antes de o abrir voltar do JS, nao achava nada para fechar, e o abrir concluia depois | `Sobreposicao` serializa abrir/fechar (`SemaphoreSlim`) e segue o estado DESEJADO, nao a ordem das chamadas. Teste bUnit com `IJSRuntime` controlado (o abrir so volta depois de o fechar ser pedido) — vermelho no codigo antigo, verde no novo |
| **P1 — Esc num dialogo aberto sobre outro fechava os dois** | `@onkeydown:stopPropagation` no dialogo e na gaveta. Teste com dialogo aninhado — vermelho no antigo, verde no novo |
| **P1 — sem portal: dentro de ancestral com `transform`/`filter`/`perspective`/`contain`, o `position: fixed` nao cobre a tela** | Documentado no XML doc e na pagina. Portal exige mover o no por JS, o que conflita com "aberto no primeiro render sem JS": **decisao para o Rafael** |
| P2 — `Thread.Sleep` no teste de pausa do snackbar | O host le o relogio do DI (`TimeProvider`, registrado pelo `AddRvmDesignSystem`); os testes usam `FakeTimeProvider` e eventos AGUARDADOS. A versao sincrona do evento do bUnit voltava antes do handler e o teste falhava 1 vez em 3; com eventos aguardados, 5 rodadas seguidas verdes |
| P2 — `Href` e `OnClick` juntos: o link vence em silencio | Documentado no parametro |
| P2 — lista de classes genericas cresceu nesta onda | Ja registrado acima como decisao do Rafael antes do `1.0.0` |

Verificado OK pelo reviewer: fila e lock do servico de snackbar, dispatcher no host, escopo por circuito,
nenhum outro numero em atributo na cultura corrente, nenhum outro `OpenElement` com CSS isolado, padroes
ARIA de dialogo, acordeao, disclosure e progresso, regioes vivas, contraste dos pares novos.

## Prefixo `rvm-` nas classes internas (decisao do Rafael, 17/09/2026)

As 159 classes internas dos 27 componentes das ondas 1 a 3 passaram a `rvm-*` (`.caixa` virou
`.rvm-caixa`). Motivo: CSS isolado impede o componente de vazar, nao de receber regra global — o
`app.css` do proprio site ja tinha acertado 11 componentes.

- Feito por script (CSS, atributos `class` da marcacao, membros C# que montam classe), com ensaio antes
  de aplicar; sobras em string interpolada e em `@code` de `.razor` corrigidas a mao
- Ficam de fora de proposito: `modified`/`invalid`/`valid` (convencao do Blazor que o consumidor
  estiliza) e as classes globais `rvm-text-*`
- **Guarda:** teste `Toda_classe_de_componente_tem_o_prefixo_rvm` reprova classe sem prefixo em CSS
  de componente; regra registrada no `CLAUDE.md`
- **Conferido visualmente:** 54 fotos (27 paginas x 2 temas) da versao anterior e da nova, comparadas
  pixel a pixel — zero diferenca. bUnit 386 e E2E 128 verdes

## Portal de dialogo e gaveta — explicado ao Rafael em 17/09, sem decisao ainda

Hoje dialogo e gaveta renderizam onde foram declarados; um ancestral com `transform`, `filter`,
`perspective` ou `contain` faz o `position: fixed` deixar de cobrir a tela. Documentado no componente
e na pagina. Portal (mover o no para o fim do `body`) exigiria JS, o que conflita com "aberto no
primeiro render sem JS".
