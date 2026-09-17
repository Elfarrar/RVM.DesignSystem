---
id: DSGN-004
titulo: Onda 2 — formulario e navegacao (10 componentes)
repo: RVM.DesignSystem
tipo: feature
status: em andamento
criada: 2026-09-17
---

# DSGN-004 — Onda 2: formulario e navegacao

Os dez da onda 2 do `11-catalogo-de-componentes.md`: `RvmSelect`, `RvmCheckbox`, `RvmRadio`,
`RvmSwitch`, `RvmTabs`, `RvmBreadcrumbs`, `RvmMenu`, `RvmPagination`, `RvmBadge`, `RvmTooltip`.

Vale tudo o que a onda 1 pagou (card `DSGN-003`): medir no PNG antes de escrever CSS, contraste de
texto e de indicador de foco nos dois temas, `-text` e nao `-main` para escrever com cor, subir o site
local e rodar axe + E2E daqui antes do PR, portao de teste pelo exit code.

## Fatias (por dependencia e por risco)

1. **Controles nativos**: `RvmCheckbox`, `RvmRadio` + grupo, `RvmSwitch` — todos sobre `<input>`
   nativo, que ja traz teclado e semantica. Herdam de `InputBase` para entrar no `EditForm`.
2. **Exibicao e navegacao simples**: `RvmBadge`, `RvmBreadcrumbs`, `RvmPagination`, `RvmTooltip`
3. **Padroes ARIA com teclado proprio**: `RvmTabs` (setas, Home/End), `RvmMenu` (menu button),
   `RvmSelect` (listbox; multiplo e busca) — os de maior risco, por ultimo

⚠️ Regra que pesa mais nesta onda: **nenhum componente depende de JS para o estado inicial**.
Sobreposicao (menu, select, tooltip) abre por evento do Blazor e se posiciona com CSS; nada de
biblioteca de posicionamento.

## Andamento

- [x] **Fatia 1**: `RvmCheckbox`, `RvmRadioGroup` + `RvmRadio`, `RvmSwitch`
- [x] **Fatia 2**: `RvmBadge`, `RvmBreadcrumbs`, `RvmPagination`, `RvmTooltip`

## Decisoes e medicoes

| O que | Por que |
|---|---|
| **Medidas do kit**: checkbox 16/18/22 px, radio 18/20/22 px, switch 34x14 e 26x10 | Medidas por varredura de linha nos PNGs e conferidas renderizadas no navegador |
| **Sobre os controles NATIVOS do Blazor** (`InputCheckbox`, `InputRadioGroup`, `InputRadio`) | Funcionam sem JS, inclusive em formulario SSR estatico; trazem teclado (espaco, setas no radio) e o `name` do POST. O desenho por cima e so aparencia |
| **Marcado/ligado usa `-text`; o "✓" usa a cor do papel** | Caixa marcada e indicador de estado: 3:1 contra o fundo. O `-main` reprova (warning no claro 1.78, primary no escuro 1.84). Os 12 pares medidos passam |
| **Grupo de radio = `<fieldset>` + `<legend>`** | Semantica nativa de grupo; o `disabled` do fieldset desabilita todas as opcoes no navegador |
| **Switch = checkbox com `role="switch"`** | Anuncia ligado/desligado. Permitido pela especificacao ARIA em HTML |
| **Indeterminado: visual por CSS, semantica por modulo JS importado sob demanda** | A propriedade `indeterminate` so existe no DOM, nao ha atributo. O estado inicial visual sai sem JS; o consumidor nao precisa de `<script>` |
| **CSS com `::deep` nas regras do input** | O input e renderizado pelo `InputCheckbox`/`InputRadio`, outro componente — sem `::deep` as regras nao casam |
| 🔴 **Expressao padrao quando nao ha `@bind-Value`** | Pego no navegador: `InputCheckbox` exige `ValueExpression`, que so o `@bind` fornece. `<RvmCheckbox Value="true" Disabled="true" />` e `Value` + `ValueChanged` faziam o controle estourar e sumir, com o `#blazor-error-ui` no ar. O bUnit sempre passava a expressao e nao viu |
| **`aria-disabled` na raiz do controle desabilitado** | O axe reprovava o contraste do texto do controle desabilitado (isento pela WCAG 1.4.3) porque nao liga o texto do `<label>` ao input |
| **Indexador no `@bind-Value` quebra** | `@bind-Value="dicionario[chave]"` gera expressao que o `FieldIdentifier` nao aceita. As paginas usam objeto com propriedade |
| **Medidas da fatia 2**: badge pilula 20 px e ponto 8 px; paginacao 26/32/40 px com 6 px de vao; tooltip 24 px | Varredura de linha em `Badge.png`, `Pagination.png` e `Tooltip.png` |
| **Tooltip escuro (`#212121`) nos dois temas** | Medido igual em `Tooltip.png` e `Tooltip-1.png`. Tokens `--rvm-color-tooltip-background/-text` (branco da 16:1) |
| **Ponto do badge usa `-text`; a pilula usa `-main` + `-contrast`** | O ponto nao tem texto: ele mesmo e a informacao e precisa de 3:1 (o `-main` do warning da 1.7 no branco). A pilula e o par ja medido do chip preenchido |
| **Tooltip: mostrar/esconder so por CSS (`:hover`, `:focus-within`)** | Funciona antes da interatividade. Esc dispensa por evento do Blazor (WCAG 1.4.13); ponte invisivel sobre a folga deixa passar o mouse para o balao |
| **Tooltip entrega o id pelo contexto do `ChildContent`** | A dica e descricao (`aria-describedby`), nao nome; o atributo tem de ir no elemento focavel, que e do consumidor |
| **Paginacao: reticencias so onde pula mais de uma pagina; `Page` fora da faixa vale como a borda** | "…" no lugar de um unico numero esconde sem economizar espaco; sem o clamp, `Page=99` deixava a seta anterior sem efeito |
| 🔴 **Classe generica no componente colide com CSS global** | A trilha usava `.link`, e o `app.css` do site tem `.link` com padding: o isolamento de CSS protege o componente de vazar, nao de receber. Pego na foto lado a lado; a classe virou `.ligacao` |

## Verifica (cada fatia)

1. Build Release 0 warning · testes verdes pelo exit code · cobertura >= 80%
2. Pagina no site com exemplo, parametros e recorte do kit
3. axe local nos dois temas; E2E local e no dev
4. Teclado: o padrao ARIA de cada componente, testado de verdade
5. Comparacao lado a lado com o PNG do kit, aprovada pelo Rafael
