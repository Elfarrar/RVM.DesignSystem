---
id: DSGN-019
titulo: Onda 3 — feedback
repo: RVM.DesignSystem
tipo: feature
status: em-revisao
criada: 2026-09-08
---

# DSGN-019 — Onda 3

Fecha a onda 3 do `09-roadmap.md` e prepara a **`0.3.0`**.

## Criterio de saida (do `09-roadmap`)

> O E2E abre o dialogo, navega so por teclado, fecha com `ESC` e confirma que o foco voltou ao
> botao de origem.

✅ `SiteSmokeTests.O_dialogo_prende_o_foco_fecha_no_ESC_e_DEVOLVE_o_foco`, contra navegador de
verdade.

## Entregue

- **Dialogo**: `RvmDialog`, `RvmDialogHost`, `IRvmDialogService` com `ConfirmAsync`
- **Notificacao**: `RvmToast`, `RvmToastHost`, `IRvmToastService`
- **Estado**: `RvmAlert`, `RvmEmptyState`, `RvmSpinner`, `RvmProgress`, `RvmSkeleton`,
  `RvmTooltip`
- **Site**: 8 paginas novas, a secao **Padroes** e a **busca** (`RF-27`)
- 4 icones novos: `check-circle`, `x-circle`, `lock`, `tray`

## A decisao que define a onda: o `<dialog>` nativo

O `11` pede "foco preso" implementado. A implementacao usa `<dialog>` + `showModal()`, pela
mesma razao do `<select>` nativo no `RvmSelect`: o navegador faz melhor.

Vem de graca, testado por quem escreveu o navegador:

| | |
|---|---|
| Foco preso | inclusive `Shift+Tab` e conteudo que aparece depois |
| `ESC` fecha | nativo |
| Fundo inerte | nem clique, nem foco, nem leitor de tela |
| Camada de topo | acima de qualquer `z-index` de qualquer app |
| `::backdrop` | sem elemento de veu no DOM |
| **Retorno do foco** | ao elemento ativo quando o dialogo abriu |

A alternativa sao ~150 linhas de JS, e cada item da lista e um lugar conhecido de errar.

⚠️ **O que o nativo NAO entrega**: o `ESC` fecha **sem avisar o componente**. Num `ConfirmAsync`
isso e grave — a caixa sumiria e quem chamou ficaria esperando para sempre, travando a tela.
O JS cancela o evento `cancel` e devolve a decisao ao C#. Tem teste E2E proprio.

## Papel novo na paleta: `scrim`

E o **unico papel com transparencia** e o unico que nao entra no teste de contraste: ele nao
recebe texto, e a funcao dele e justamente reduzir o contraste do que esta atras. Tambem e o
unico que nao deriva da marca — veu e preto nos dois modos; qualquer matiz tinge a tela inteira
e briga com a cor do produto. O que muda entre claro e escuro e a opacidade.

Existe porque cor literal em CSS de componente e reprovada pelo portao, **corretamente**, e o
`::backdrop` precisa de alfa.

## Tres decisoes de acessibilidade que sao o oposto do intuitivo

### 1. `role="alert"` NAO e o padrao do `RvmAlert`

Ele interrompe o leitor de tela na hora. Certo para "nao foi possivel salvar" que apareceu
agora; **ruido puro** num aviso que ja estava na tela quando ela abriu — que passa a ser gritado
a cada render, atropelando o titulo da pagina. Por isso `Live` e opt-in. E, com `Live`, so
`Danger` interrompe: anunciar "salvo com sucesso" por cima do que o leitor estava dizendo custa
mais do que informa.

### 2. As regioes `aria-live` do toast existem SEMPRE, vazias

Uma regiao so e observada a partir do instante em que entra no DOM. Se ela nascesse junto com o
primeiro toast, **a mensagem nao seria anunciada** — e o defeito passa no teste manual com os
olhos, porque visualmente tudo funciona. Sao **duas** regioes porque mudar o `aria-live` de uma
regiao viva nao e confiavel.

### 3. O balao do `RvmTooltip` fica sempre no DOM

`aria-describedby` precisa apontar para um elemento que exista, e leitores de tela leem a
descricao quando o controle recebe **foco**, nao quando o mouse passa. Escondido por
`opacity`, nunca por `display:none` ou `visibility:hidden` — os dois tiram o elemento da arvore
de acessibilidade.

## O que o teste de foco preso ensinou

A primeira versao do teste E2E cobrava "o foco esta sempre dentro do dialogo" e **reprovou uma
implementacao correta**.

O Chromium, ao passar do ultimo focavel de um `<dialog>` modal, leva o foco ao `<body>` por uma
parada antes de voltar ao primeiro. O que `showModal()` garante — e o que importa — e que o foco
nunca alcanca um **controle da pagina atras**.

> A assercao certa nao era "sempre dentro", e sim "nunca em um controle de fora, e volta para
> dentro". Um teste bom precisa saber o que exatamente esta prometido.

## Decisao de escopo: nao ha `ShowAsync<T>` generico

O `IRvmDialogService` resolve o caso que se repete em toda tela (`ConfirmAsync`, devolvendo
`bool`). Dialogo com conteudo arbitrario e o `RvmDialog` declarativo com `@bind-Open` — e o
"resultado tipado" do `09` ali e o proprio campo de quem usa, com o tipo que ele quiser.

Um `ShowAsync<T>` teria de renderizar componente dinamico e devolver `object`. **Formulario
dentro de dialogo e marcacao, e marcacao se escreve na tela, nao se monta por chamada de
metodo.**

## Verificado

| | |
|---|---|
| Testes | 223 (eram 187), 0 aviso em `Release` |
| Portao de contraste | 29 pares x 5 temas x 2 modos |
| E2E | 6 testes, incluindo o criterio de saida da onda |
| axe | toda rota do menu x 2 modos de cor |
| Dev no ar | `design.dev.rvmtech.com.br`, verificado por CONTEUDO |
| Pre-release | `0.3.0-alpha.15` no BaGet |

O publish desta onda foi o primeiro com o `--timeout 120` do `DSGN-018`, e passou em tempo
normal.
