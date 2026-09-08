# 11 — Catálogo de Componentes (v1)

35 componentes, em quatro ondas. Convenções de API e parâmetros comuns em `05-api-dos-componentes.md`
— aqui está **o que cada um é, o que tem de próprio e o que a acessibilidade exige dele**.

Todo componente, sem exceção, precisa de: teste bUnit do render padrão, de cada variante, do estado
desabilitado e dos callbacks; página no site; e passar na auditoria axe.

## Onda 1 — Básicos (9)

| Componente | Próprio dele | Acessibilidade |
|---|---|---|
| `RvmButton` | `Variant` (Primary/Secondary/Tertiary/Ghost/Danger), `Loading` que preserva a largura, ícone inicial/final | `disabled` nativo; `aria-busy` quando carregando; foco visível com anel de 3:1 |
| `RvmIconButton` | Só ícone, área de toque de 40px mesmo com ícone de 20px | `aria-label` **obrigatório** — sem ele, o componente lança em tempo de desenvolvimento |
| `RvmTextField` | Tipos text/email/password/number/tel, prefixo e sufixo, contador de caracteres | `<label>` ligado por `for`; erro em `aria-describedby`; `aria-invalid` |
| `RvmTextArea` | Auto-crescimento opcional com altura máxima | Mesmo contrato do TextField |
| `RvmSelect` | Lista de opções tipada, opção vazia com texto configurável, busca opcional | `role="listbox"`, `aria-expanded`, setas/Home/End/Esc, digitação para saltar |
| `RvmCheckbox` | Estado indeterminado real (`indeterminate`) | `input` nativo por baixo; `aria-checked="mixed"` no indeterminado |
| `RvmRadioGroup` | Orientação horizontal/vertical, `@bind-Value` no grupo | `role="radiogroup"`, setas circulam entre opções, um só ponto de tabulação |
| `RvmSwitch` | Rótulo dos dois lados, estado "ligando" (assíncrono) | `role="switch"` + `aria-checked`; estado nunca comunicado só por cor |
| `RvmFormField` | Casca compartilhada: label, ajuda, erro, obrigatório — usada pelos campos acima | Garante o vínculo label↔controle↔erro **em um lugar só** |

`RvmFormField` é o que impede o defeito clássico: cinco campos, cinco jeitos de ligar a mensagem de
erro, três deles errados.

## Onda 2 — Layout (12) ✅ entregue em 08/09/2026

| Componente | Próprio dele | Acessibilidade |
|---|---|---|
| `RvmAppShell` | Casca: topbar + sidebar + conteúdo. Responsivo — sidebar vira drawer sobreposto abaixo de `md` | Landmarks (`header`, `nav`, `main`); `skip to content` como primeiro foco |
| `RvmSidebar` | Colapsável (só ícones), grupos, item ativo pela rota | `nav` + `aria-current="page"`; tooltip do item colapsado é acessível |
| `RvmTopbar` | Slots de início/meio/fim, ação de menu no mobile | `aria-expanded` no botão de menu |
| `RvmNavItem` | Ícone, texto, badge, sub-itens | Estado ativo nunca só por cor |
| `RvmCard` | Slots header/body/footer, elevação por token, variante clicável | Card clicável é `<button>`/`<a>` de verdade, não `div` com `onclick` |
| `RvmStack` | Empilhamento com `Gap` da escala, direção e alinhamento | — |
| `RvmGrid` | Grid responsivo por colunas nos breakpoints | — |
| `RvmTabs` | Abas com painel associado, ativa por rota ou por estado | `role="tablist"`, setas navegam, `aria-controls`/`aria-selected` |
| `RvmBreadcrumb` | Colapsa no meio quando não cabe | `nav` + `aria-label="Trilha"`; último item é `aria-current` |
| `RvmDivider` | Horizontal/vertical, com rótulo opcional | Decorativo → `aria-hidden` |
| `RvmChip` | Variantes por severidade, removível, selecionável | Chip removível tem botão com `aria-label` próprio |
| `RvmAvatar` | Iniciais, imagem, cor derivada do nome, tamanhos, grupo empilhado | Imagem com `alt`; iniciais com `aria-label` do nome completo |

Mais `RvmIcon` (`aria-hidden` por padrão — ícone decorativo não deve ser anunciado).

Entrou também o **`RvmNavGroup`**, que não estava na lista: é ele que torna real o "grupos" do
`RvmSidebar`. Dentro de uma lista, todo filho direto precisa ser `li`, então um `h2` solto seguido
de outra `ul` seria marcação inválida — a seção nomeada precisa ser `li > h2 + ul[aria-labelledby]`.
Documentado na página do Sidebar, sem página própria.

## Onda 3 — Feedback (8) ✅ entregue em 08/09/2026

| Componente | Próprio dele | Acessibilidade |
|---|---|---|
| `RvmDialog` | Tamanhos, slots, resultado tipado, `ConfirmAsync` pronto | **Foco preso**, `ESC` fecha, foco volta ao elemento de origem, `role="dialog"` + `aria-modal`, fundo inerte |
| `RvmDialogHost` | Pilha de diálogos, um por vez, montado no layout | — |
| `RvmToast` | Severidade, duração, ação (desfazer), posição configurável | `role="status"` (informativo) / `role="alert"` (erro); não some antes de 5s; pausa no hover |
| `RvmToastHost` | Fila e limite de simultâneos | Região `aria-live` única, criada uma vez |
| `RvmAlert` | Severidade, título, dispensável, ação inline | `role="alert"` só quando aparece dinamicamente — alerta estático anunciado a cada render é ruído |
| `RvmSkeleton` | Formas texto/círculo/retângulo, animação de brilho | `aria-hidden`; a região real anuncia "carregando" por `aria-busy` |
| `RvmSpinner` | Tamanhos, rótulo opcional | `role="status"` com texto acessível |
| `RvmEmptyState` | Ilustração/ícone, título, descrição, ação primária. Variantes: vazio, sem resultado de filtro, erro, sem permissão | Texto em PT-BR explicativo, sem código interno |

`RvmProgress` (barra determinada/indeterminada, `role="progressbar"` com `aria-valuenow`) e
`RvmTooltip` (aparece no foco além do hover, `aria-describedby`, some com `ESC`) completam a onda.

## Onda 4 — Dados (5)

A onda cara. Cada linha aqui vale, em esforço, várias linhas das tabelas acima.

### `RvmDataGrid<T>`

- Colunas tipadas por expressão (`Field="x => x.Nome"`), template de célula e de cabeçalho
- Ordenação por coluna (clique e teclado), multi-coluna opcional
- Paginação integrada ou externa
- Seleção (nenhuma, única, múltipla com "selecionar todos da página")
- Dois modos: **em memória** e **remoto** (callback recebe página, tamanho, ordenação, filtros e
  devolve itens + total)
- Estados de carregando (skeleton nas linhas), vazio (`RvmEmptyState`) e erro com "tentar de novo"
- Densidade respeitada; rolagem horizontal com cabeçalho fixo
- Acessibilidade: `<table>` semântica de verdade, `scope` nos cabeçalhos, `aria-sort` na coluna
  ordenada, navegação por teclado entre células, seleção anunciada

**Explicitamente fora:** edição in-loco, agrupamento, coluna congelada, virtualização, exportação.
Cada um é um projeto próprio; todos estão no `09-roadmap.md` § Fora da v1.

### `RvmPagination`

Página atual, tamanho de página, total de itens, saltos primeiro/último. Textos em PT-BR
("21–40 de 137"). Botões com `aria-label` descritivo, não só o número.

### `RvmFilterBar`

Casca de filtros: campos, botão limpar, contador de filtros ativos, colapso no mobile. **Não decide
os filtros** — recebe-os como conteúdo. Não é gerador de filtro; é o lugar onde eles ficam.

### `RvmDatePicker`

- Entrada por teclado em `dd/MM/yyyy` **antes** do calendário — quem digita rápido não quer abrir
  nada; é o defeito mais comum de date picker
- Calendário em pt-BR, semana começando no domingo, mínimo/máximo, datas desabilitadas
- Variante de intervalo (`RvmDateRangePicker`) fica para a v2
- Acessibilidade: grade com `role="grid"`, setas movem dia, PageUp/PageDown mês, `ESC` fecha, dia
  focado anunciado por extenso

### `RvmAutocomplete<T>`

- Busca assíncrona com debounce configurável e cancelamento da busca anterior
- Item template, "nenhum resultado", mínimo de caracteres, seleção livre opcional
- Múltipla seleção com chips fica para a v2
- Acessibilidade: padrão ARIA combobox completo — `aria-expanded`, `aria-activedescendant`,
  `aria-autocomplete="list"`, setas/Enter/Esc, resultado anunciado por região `aria-live`

## Resumo por onda

| Onda | Componentes | Pacote | O que passa a ser possível |
|---|---|---|---|
| 1 | 9 | `0.1.0` | Formulário completo e acessível |
| 2 | 13 | `0.2.0` | Aplicação inteira com casca, navegação e conteúdo |
| 3 | 8 | `0.3.0` | Todos os estados: confirmação, aviso, carregando, vazio, erro |
| 4 | 5 | `1.0.0` | Listagem com grid, filtro, paginação, data e busca |

**35 no total.** O que não está nesta lista não está na v1 — e o destino de cada ausência está
declarado em `09-roadmap.md` § Fora da v1.
