---
id: DSGN-020
titulo: Onda 4 — dados, e a 1.0
repo: RVM.DesignSystem
tipo: feature
status: concluido
criada: 2026-09-08
---

# DSGN-020 — Onda 4

Fecha a onda 4 do `09-roadmap.md` e prepara a **`1.0.0`** — a primeira versao estavel, com o
contrato congelado sob SemVer.

## Criterio de saida (do `09-roadmap`)

> Uma tela de listagem real (dados de exemplo, ~500 linhas) com filtro, ordenacao e paginacao,
> operavel so por teclado, aprovada na auditoria axe.

✅ `/padroes/listagem`, com **487 pedidos**. Verificada pelo E2E
`A_listagem_filtra_ordena_e_pagina_SO_POR_TECLADO` em navegador de verdade, e coberta pela
varredura axe junto com as outras 44 paginas.

## Entregue

- `RvmDataGrid<TItem>` + `RvmColumn<TItem>` + `RvmDataGridRequest`/`RvmDataGridResult<T>`
- `RvmPagination`, `RvmFilterBar`, `RvmDatePicker`, `RvmAutocomplete<TItem>`
- 4 icones novos: `caret-double-left`, `caret-double-right`, `arrows-down-up`, `funnel`
- 5 paginas de componente + a tela de **Listagem** em Padroes

## A decisao que define a onda: NAO ha `role="grid"`

O `11` pede "navegacao por teclado entre celulas". A implementacao e uma `<table>` **semantica**,
sem `role="grid"` e sem tabindex movel — e a razao e que `role="grid"` tornaria a tabela **menos**
navegavel.

Um leitor de tela ja sabe percorrer tabela: NVDA e JAWS tem modo proprio, com `Ctrl+Alt+setas`,
anuncio do cabecalho ao mudar de coluna e leitura da linha inteira. `role="grid"` **desliga** esse
modo e obriga a usar a navegacao que a biblioteca escrever — que sera pior, porque o leitor foi
feito para isso e a biblioteca nao.

O APG e explicito: `grid` e para tabela **interativa**, onde a celula em si e um controle. Aqui a
celula e dado; o que e interativo (ordenar, selecionar) sao botoes e checkboxes de verdade, na
tabulacao normal.

> Edicao in-loco esta fora da v1 (`09` § Fora da v1). No dia em que entrar, ai sim a discussao de
> `role="grid"` faz sentido — e ela muda de resposta.

E o segundo desvio declarado seguido pelo mesmo criterio: na onda 3 foi o `<dialog>` nativo, na
1 foi o `<select>` nativo. **A plataforma primeiro; codigo nosso so onde ela nao resolve.**

## Duas decisoes de API que valem para sempre agora

### `Field` e expressao, nao `Func`

Ela serve para duas coisas: extrair o valor e ser chave de ordenacao. Com `Func`, so a primeira
seria possivel. Compilada **uma vez** no registro da coluna — compilar por celula refaria o mesmo
trabalho a cada linha de cada render.

### No modo remoto o grid NAO ordena nem pagina

Ele informa o que foi pedido e usa o que voltar. Reordenar a resposta embaralharia a pagina em
cima da ordem do servidor. O `SortColumn` da requisicao e o **titulo** da coluna: quem responde
traduz para o campo do banco, porque so ele sabe o nome da coluna la.

## Duas armadilhas que o proprio teste E2E revelou

Nenhuma era defeito do produto — as duas eram **o teste** escrito com locator ingenuo, e as duas
ensinam algo sobre a marcacao que a biblioteca produz:

1. **`GetByLabel("Cliente")` casou com dois elementos**: o campo de filtro e o botao de ordenar
   da coluna Cliente. Os dois estao certos em ter esse nome.
2. **`[aria-current='page']` casou com dois**: o item ativo do menu e a pagina atual da
   paginacao. **Dois marcadores de "voce esta aqui" convivem na mesma pagina**, em contextos
   diferentes, e cada um esta correto — o que o teste precisa e de escopo, nao de menos ARIA.

E um terceiro do mesmo tipo: o `role="status"` da barra de selecao passou a ser o **primeiro** da
pagina assim que apareceu, e o locator que pegava "o primeiro status" deixou de apontar para o
resumo da paginacao.

> Locator de E2E precisa dizer **dentro de que regiao** procura. Sem isso, o teste quebra quando
> a pagina ganha um segundo elemento correto do mesmo tipo — e a leitura errada e "o produto
> quebrou".

## Verificado

| | |
|---|---|
| Testes | 260 (eram 223), 0 aviso em `Release` |
| Portao de contraste | 29 pares x 5 temas x 2 modos |
| E2E | 7 testes, incluindo o criterio de saida |
| axe | 45 paginas x 2 modos de cor |
| Dev no ar | `design.dev.rvmtech.com.br`, verificado por CONTEUDO e no navegador |
| Pre-release | `1.0.0-alpha.17` no BaGet |

Conferido no dev, no modo escuro: ordenacao por clique, selecao de 3 linhas com o total somado
na barra, e a caixa de "selecionar todas" no estado **indeterminado** — que e o unico dos tres
estados que exige JS, porque `indeterminate` e propriedade do elemento e nao atributo.

| Prod no ar | `design.rvmit.com.br`, verificado por CONTEUDO e no navegador |
| Pacote | **`1.0.0`** no BaGet, pela tag `v1.0.0`, presa ao commit `2366388` |

## Promocao para producao — 08/09/2026

Autorizada pelo Rafael. `dev` -> `master` pelo PR #34, deploy do Pages verde. Conferida em
producao: 487 pedidos, ordenacao por Total funcionando e a legenda da tabela anunciando
"ordenada por Total, crescente".

A `1.0.0` saiu em **33 segundos** — o `--timeout` do `DSGN-018` continua valendo.

## O que a 1.0 significa

**A partir daqui, mudanca que quebra e major.** As tres ondas anteriores puderam tornar
`RvmPalette.OnSurfaceVariant` e `RvmPalette.Scrim` `required` sem cerimonia; isso acabou.

Adocao continua sendo **so projeto novo** (`09` § Adocao): ERPAgro, ObraEmDia, Fiscal e
Propostinha seguem no MudBlazor, sem prazo. O proximo projeto RVM que precisar de UI nasce aqui,
e e ele quem prova a biblioteca.
