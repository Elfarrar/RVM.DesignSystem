---
id: DSGN-004
titulo: Onda 2 — formulario e navegacao (10 componentes)
repo: RVM.DesignSystem
tipo: feature
status: concluido
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
- [x] **Fatia 3**: `RvmTabs` + `RvmTab`, `RvmMenu` + `RvmMenuItem` + `RvmMenuDivider`, `RvmSelect` e `RvmMultiSelect`

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
| **Medidas da fatia 3**: aba 48 px (72 com icone), indicador 2 px, aba preenchida 130x48; item de menu 36 px sobre o `paper`; select 56/40 px | Busca de regioes azuis e varredura de linha em `Tabs.png`, `Menu.png` e `Select.png` — o select tem as medidas do campo de texto |
| **Tabs: selecao segue o foco; so o painel ativo e renderizado** | Padrao APG com tabindex movel. `aria-controls` so na aba ativa, porque so o painel dela existe no DOM |
| **Rolagem por overflow no lugar das setas de rolagem do kit** | A seta do teclado ja traz a aba focada a vista; botoes de rolagem seriam controles a mais sem ganho de acesso |
| **`rvm-teclado.js`: impedir a rolagem da pagina so para as teclas usadas** | O Blazor so barra o padrao de TODAS as teclas do evento, e barrar Tab tira o foco da ordem de tabulacao. O modulo e importado sob demanda e nada do estado inicial depende dele |
| **Menu: itens sao `<button role=menuitem>`; clicar fora fecha por camada transparente** | Enter e Espaco acionam sem codigo; clicar fora dispensa ouvir o documento por JS. `RvmButton` ganhou `FocusAsync()` para o menu devolver o foco |
| **Select: `RvmSelect` e `RvmMultiSelect` sobre uma base comum, sem `InputBase`** | Os dois tem tipo de valor diferente (`TValue` e `IReadOnlyList<TValue>`); a base acha o campo pela expressao e fala com o `EditContext` direto (validacao, `modified`) |
| **Select-only combobox: foco no campo, opcao ativa por `aria-activedescendant`, anel por dentro** | Padrao APG. A opcao ativa nao tem foco real, entao o anel e o que mostra onde o teclado esta |
| 🔴 **Select de tipo valor nao anulavel comeca com o `default` escolhido — contrato, nao bug a esconder** | Review independente (P1): `Value is not null` e sempre verdadeiro para `int`/enum, e 0 costuma ser opcao legitima. Nao da para distinguir "escolheu 0" de "nao escolheu": para comecar vazio, `TValue` anulavel (`Status?`), como no `InputSelect`. Documentado e testado. Valor fora das opcoes continua nao contando |
| **Busca ignora maiusculas e acentos** (`CompareOptions.IgnoreNonSpace`) | luis acha Luís |
| 🔴 **Item de menu focado: fundo de hover, nao `action-focus`** | No tema claro o texto sobre `action-focus` dava 4.2:1 — so aparece com o menu ABERTO, e o axe de pagina roda com tudo fechado. Virou teste E2E com menu e select abertos nos dois temas |

## Verifica (cada fatia)

1. Build Release 0 warning · testes verdes pelo exit code · cobertura >= 80%
2. Pagina no site com exemplo, parametros e recorte do kit
3. axe local nos dois temas; E2E local e no dev
4. Teclado: o padrao ARIA de cada componente, testado de verdade
5. Comparacao lado a lado com o PNG do kit, aprovada pelo Rafael

## Review independente (onda 2 inteira)

Sonnet, 17/09. Um P1 (o de cima, select com tipo valor) e dois P2: cache do `FieldIdentifier` por
instancia da expressao era inutil (o `@bind` gera expressao nova a cada render) — agora resolve uma
vez por `OnParametersSet`; `Items` percorrido a cada render — documentado que deve vir materializado.
Verificado OK pelo reviewer: ids por contador estatico, dispose das abas, `CriarCampo` com modelo
aninhado, CSS sem hex e com `::deep` onde precisa, atributos repassados, nada de JS no estado inicial,
padroes APG de tabs, menu e combobox.

## Fechamento

Comparacoes lado a lado com o kit aprovadas pelo Rafael em 17/09/2026. Onda 2 fechada no dev.
