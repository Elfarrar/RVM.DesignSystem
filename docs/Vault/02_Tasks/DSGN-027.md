---
id: DSGN-027
titulo: Aparencia "Admin" e a tela de Dashboard
repo: RVM.DesignSystem
tipo: feature
status: reprovado-e-revertido
criada: 2026-09-08
---

# DSGN-027 — O desenho do AdminLTE

> ⛔ **REPROVADO E REVERTIDO em 08/09/2026** pela [[DSGN-028]]: *"nao ficou bom, remova o
> AdminLTE, mas crie um dashboard"*. A aparencia `Admin` saiu inteira e a tela de painel foi
> refeita na linguagem daqui.
>
> **O card fica** porque duas coisas dele continuam valendo e custariam para redescobrir: a
> tecnica da casca escura tirada da paleta ESCURA do tema (em vez de hexadecimais fixos), e o
> defeito `scrollable-region-focusable` em tela sem nenhum elemento focavel.

Pedido do Rafael em 08/09/2026: *"quero algo parecido com isso"* +
`https://adminlte.io/themes/AdminLTE/index2.html`.

## O que faz o AdminLTE parecer o AdminLTE

Sao tres coisas, e nenhuma delas e a paleta:

1. **Barra lateral escura sobre conteudo claro** — o unico item que a biblioteca nao sabia fazer
2. **Caixa de numero saturada**, com valor grande, rotulo curto, icone de marca d'agua cortado
   pela borda e rodape "Mais informacoes →"
3. **Cartao com faixa colorida no topo**, denso, canto quase reto

Entregue como a aparencia **Admin** (a casca) e a tela **`/padroes/dashboard`** (o conteudo).

## ⚠️ As cores NAO sao as do AdminLTE, e nao e liberdade poetica

As *small-box* de la sao branco sobre `#00c0ef` e `#f39c12`: **2,4:1 e 2,2:1**, contra os 4,5:1
que a WCAG 2.1 AA exige para texto normal. **O AdminLTE reprova em contraste nessas caixas.**

Copiar os hexadecimais entregaria o visual e reprovaria no axe — e o axe estaria certo. O que se
copia e a **estrutura**; a cor sai dos pares `x`/`on-x` da paleta, que nascem medidos. O
resultado e o mesmo desenho, legivel, e funcionando em qualquer tema — inclusive numa paleta
montada pelo visitante em `/fundamentos/paleta`.

## A barra lateral escura sem inventar cinza

O AdminLTE resolve com um punhado de hexadecimais fixos (`#222d32`, `#b8c7ce`). Aqui isso furaria
o portao: seria um par fundo/texto que ninguem mediu.

A solucao usa o que ja existe: o `MainLayout` emite `--rvm-shell-*` a partir da **paleta ESCURA
do tema corrente**. Fundo escuro com texto claro e um par que o portao ja mede a cada build —
reaproveita-lo da a casca escura de graca, em qualquer tema.

## O defeito que o axe pegou, e que valeu a pena

`scrollable-region-focusable` na pagina nova, nos dois modos e nas quatro aparencias.

A causa **nao** era a tela: os elementos roláveis sao a casca da biblioteca (`rvm-sidebar` e
`rvm-app-shell__rolagem`). O que essa pagina tinha de diferente e que ela **nao tinha um unico
elemento focavel** — so texto, chip e barra de progresso. Sem nada que receba foco, nao ha como
rolar a area de conteudo pelo teclado.

Foi a primeira pagina do site sem um link sequer, e por isso a primeira a expor isso.

O conserto e o proprio rodape "Mais informacoes →" do AdminLTE. Ou seja: a peca que faltava para
parecer com a referencia era a mesma que faltava para a pagina ser operavel por teclado.

⚠️ **Fica registrado que isto pode morder um consumidor**: uma tela so de leitura, sem link nem
botao, herda o mesmo problema. Se acontecer de novo, a resposta e no `RvmAppShell` — mas mudar o
foco da casca tem consequencia propria e nao entra de carona neste card.

## Verificado

| | |
|---|---|
| Build | 0 erro, 0 aviso |
| Testes | 309 |
| E2E | axe nas 46 paginas x 2 modos **e** nas 4 aparencias x 4 paginas |

## Nota de metodo

As capturas de viewport do Playwright vieram de um frame velho por varias tentativas — o DOM
dizia `rgb(21,19,22)` e a imagem mostrava claro. **A captura do ELEMENTO (`locator.screenshot`)
mostrou a verdade**, e um `setViewportSize` destravou a captura de tela cheia. Vale lembrar disso
antes de sair investigando um defeito de CSS que nao existe.

## Estado

⏳ **Aguardando o Rafael**: `Atual`, `Sobrio`, `Marcante`, `Vivo` ou `Admin`.

Se for `Admin`, o passo seguinte e promover a caixa de numero a componente (`RvmStatBox`) —
hoje ela e CSS da pagina de exemplo, e o app de obras vai querer usa-la.
