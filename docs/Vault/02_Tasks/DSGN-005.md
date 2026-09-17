---
id: DSGN-005
titulo: Onda 3 — feedback e sobreposicao (8 componentes)
repo: RVM.DesignSystem
tipo: feature
status: em andamento
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

## Decisoes e medicoes

| O que | Por que |
|---|---|
| **Progresso e esqueleto sem referencia no kit** — barra de 4 px, anel nos tamanhos do avatar (24/40/56), esqueleto no `action-selected` | O kit nao tem pagina deles (nem em tela de dashboard). No E2E viraram excecao declarada do recorte do kit, como o `RvmIcon` |
| **Estado vazio segue a composicao da tela `Error.png`** | Titulo, apoio e acao centralizados. O titulo e cabecalho de verdade (`HeadingLevel`) e nomeia a `section` |
| **Trilho do progresso neutro (`action-selected`), nao o tom suave da cor** | Com `-outlined-resting`, o preenchido dava 2.3 a 3.1:1 contra o trilho no escuro (WCAG 1.4.11 pede 3:1). Contra o neutro passa de 5:1 nos dois temas (medido) |
| **Indeterminado sem `aria-valuenow`; esqueleto sempre `aria-hidden`** | E como o leitor de tela sabe que nao ha previsao. Quem avisa "carregando" e a regiao com `aria-busy`, nao cada bloco cinza |
| **Movimento reduzido: progresso desacelera, brilho do esqueleto some** | Parar o indeterminado faria parecer travado; o brilho e so enfeite |
| 🔴 **Numero decimal em atributo de markup sai na cultura corrente** | `r="@Raio"` virou `r="20,2"` no site em pt-BR: atributo invalido, anel invisivel, sem erro nenhum. O bUnit roda em en-US e passou. Pego na foto; agora literal + teste bUnit em pt-BR + E2E medindo a geometria |

## Verifica (cada fatia)

1. Build Release 0 warning · testes verdes pelo exit code · cobertura >= 80%
2. Pagina no site com exemplo, parametros e recorte do kit (quando o kit tem pagina)
3. axe local nos dois temas, inclusive com o que abre aberto; E2E local e no dev
4. Teclado: o padrao ARIA de cada componente, testado de verdade
5. Comparacao lado a lado com o PNG do kit, aprovada pelo Rafael
