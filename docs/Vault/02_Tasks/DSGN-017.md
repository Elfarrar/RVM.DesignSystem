---
id: DSGN-017
titulo: Paridade de API com o RVM.UI — etapa 3 (componentes do UI no DS, comuns alinhados ao contrato, 2.0.0)
repo: RVM.DesignSystem
tipo: feature
status: em-andamento
criada: 2026-10-06
atualizada: 2026-10-07
---

# DSGN-017 — Paridade de API com o RVM.UI (etapa 3)

## Decisão (Rafael, 06/10/2026)

O RVM.UI (biblioteca privada do ecossistema) e o RVM.DesignSystem (público, visual do kit NEATLAB) passam a seguir
**um contrato único de API**: mesmos nomes de componente, mesmos parâmetros, mesmos tipos e enums, mesmos
eventos. **Cada biblioteca mantém o próprio visual.**

- **Nomenclatura do RVM.DesignSystem é o padrão** do contrato: pares de binding completos
  (`Value`/`ValueChanged`/`ValueExpression`, `Expanded`/`ExpandedChanged`, `ActiveIndex`/`ActiveIndexChanged`,
  `SelectedItems`/`SelectedItemsChanged`), `AriaLabel` separado do rótulo visível, `HeadingLevel`, `HelperText`/
  `ErrorText`/`Placeholder`/`Required`, nomes de mercado (`SiblingCount`/`BoundaryCount`, `Orientation`,
  `Placement`, `StartContent`/`EndContent`, `TopBar`), cor e estilo sempre por `Color`/`Variant`.
- **Do RVM.UI entram no contrato:** `Class` em todos os componentes e os estados prontos de listas e tabelas
  (`Loading`/`Empty`/`Error` com texto e conteúdo).
- **Três etapas:**
  1. **RVM.UI** (`RUI-066`): contrato publicado, os 17 componentes que só existem no DS entram no UI **no design
     do UI**, parâmetros dos componentes em comum renomeados para o padrão, com o nome antigo mantido como alias
     `[Obsolete]` (minor, não quebra consumidor).
  2. **Migrar os sistemas que usam o RVM.UI** para os nomes novos e remover os aliases (RVM.UI **major**)
     — também na `RUI-066`, com um card em cada repo consumidor.
  3. **RVM.DesignSystem** (`DSGN-017`): os 101 componentes que só existem no UI entram no DS **no design do DS**,
     os comuns são alinhados ao contrato, DS **2.0.0**.
- **Motivo:** um produto que usa o RVM.UI precisa poder trocar o pacote pelo RVM.DesignSystem (CC BY 4.0) **sem
  reescrever as telas**, já que o RVM.UI não pode ser redistribuído. De quebra, as duas bibliotecas ganham a
  nomenclatura mais profissional.
- ⛔ **Nada do visual do RVM.UI entra no DS** (repo público): nem CSS, nem ícones, nem medidas. Só a API (nomes e
  assinaturas) é compartilhada.

## Contrato e verificação

- Fonte do contrato: `contrato-api.json` (componentes `Rvm*`, parâmetros com nome e tipo, enums com valores,
  eventos), gerado por reflexão a partir do RVM.UI ao fim da etapa 1 e copiado para o repo do DS.
- Teste de contrato no CI de **cada** biblioteca: falha se faltar componente, parâmetro, tipo ou valor de enum do
  contrato. Componente novo em uma biblioteca quebra o build da outra até ser espelhado.
- Namespaces continuam diferentes (`RVM.UI` × `RVM.DesignSystem`): trocar de biblioteca é trocar o
  `PackageReference` e os `@using`.

## Medição (06/10/2026, varredura automática dos `[Parameter]`)

UI **139** componentes · DS **55** · **38** em comum · só **3** dos comuns com API compatível · **101** só no UI ·
**17** só no DS. ⚠️ A varredura não segue herança: parâmetros declarados em classe base (gráficos, campos de
formulário) podem aparecer como ausentes. Refazer por reflexão no início da execução.

## Pré-requisito

Etapa 1 (`RUI-066` no RVM.UI) concluída: o `contrato-api.json` existe e foi copiado para este repo. A etapa 2
(migração dos consumidores do RVM.UI) pode correr em paralelo.

## Execução — remedição e decisões (07/10/2026)

Pré-requisito cumprido: RVM.UI **`v3.0.0`** em prd (`RUI-068`), `contrato-api.pendentes.json` do UI vazio.
Contrato copiado para `design/contrato-api.json`.

**Remedição por reflexão** (`tools/contrato-api` do UI, DS em `master`): contrato com **166** componentes, DS
com **65**. Faltam no DS **100 componentes**, **399 parâmetros** em 65 componentes existentes (`Class`, estados
`Loading`/`Empty`/`Error` nos gráficos, tabela e lista, `Shape`/`LinkHref`/`DisplayName` nos campos, tema),
**60 enums** (50 novos, 10 com valores a mais) e **1.228 nomes** no `RvmIconName` (os do Solar, do UI).

**Decisões do Rafael (07/10/2026):**

1. **Ícones: mapear para Tabler.** Cada nome Solar do contrato aponta para o Tabler mais próximo (estilo
   `Bold`/`BoldDuotone` → variante *filled* do Tabler onde existir). Mapa gerado por script e revisado; os SVGs
   entram no pacote. **Reabre o "cresce sob demanda" do ADR-005** — o conjunto continua Tabler, só deixa de ser
   curado pelo uso.
2. **Mascotes (`RvmMascotName`): ícone Tabler grande em círculo de fundo suave**, um por mascote — invenção
   declarada no estilo NEATLAB. Nada da arte do UI entra.
3. **Entrega em ondas com lista de pendentes**, como no UI: o `ContratoApiTests` nasce com
   `design/contrato-api.pendentes.json`, que só encolhe. Onda 0 = contrato, enums, ícones e parâmetros dos
   componentes em comum; depois as ondas do card (shell/navegação → formulários → tabelas e listas → feedback →
   restante), cada uma em `dev` com screenshot para aprovação. **2.0.0 só com zero pendentes.**

## Onda 0 — feita em 07/10/2026 (branch `dsgn-017`)

Pendentes do contrato: **1.926 → 241**. O que sobrou sao os 101 componentes novos e os enums deles, mais tres
itens movidos para as ondas onde fazem sentido: `RvmThemeProvider` (Accent, ColorScheme, Settings, UserTheme, Class
— entra com o `RvmThemePicker` e o `AddRvmTheme`), `RvmBarChart.Layout` e `RvmPieChart.Palette`/`Total` (entram com
os graficos radiais, que dividem a mesma API).

- **Contrato no CI**: `design/contrato-api.json` + `ContratoApiTests` (lista de pendentes que so encolhe; toda API
  publica do DS tem que estar no contrato).
- **Enums**: aliases do RVM.UI pelo mesmo valor (`Accent`, `Neutral`, `Danger`, `Filled`, `Regular`, `Default`...).
  Valores novos: papel **`Inverse`** (branco sobre fundo colorido, tokens `--rvm-color-inverse-*`), botao **`Soft`**,
  abas **`Page`**, cores de texto, ordenacao `None`. ⚠️ Com alias, `Enum.ToString()` e ambiguo: o grafico deixou de
  montar classe por `ToString()`.
- **Icones**: 1.228 nomes Solar mapeados para 635 desenhos Tabler (`tools/icones-do-contrato.json`, 163 marcados como
  aproximacao pelos agentes de mapeamento e revisados por amostra), `Style` cheio onde o Tabler tem (276), `SizePx`.
  DLL foi a ~990 KB.
- **`RvmMascot`** (decisao 2) e os estados `Loading`/`Empty`/`Error` em lista, tabela/grade e graficos.
- **`Class` em todos os componentes** (`ClassesCss`). ⚠️ O Blazor casa parametro sem caixa: `class="x"`, `id="x"` e
  `name="x"` no markup do consumidor agora caem em `Class`/`Id`/`Name` — os componentes leem os dois caminhos.
- **Campos**: Id, Name, DisplayName, HelperText/ErrorText, link acima do campo, `Shape` pilula, `Background`,
  `ItemImage`, `ReadOnly`, `Clearable`, `Alignment`, `Immediate`, icones no `RvmTextField`; `Value` no MultiSelect
  (mesmo valor que `Values`).
- **Resto**: Card (Header/Footer/Padding/Variant), ListItem (valor e variacao), Menu (gatilho so icone, posicao),
  Tabs por valor e contador, AppShell (Sidebar pronta, cortina vira botao com `CloseMenuLabel`), NavItem sem link,
  NavGroup por rota, Rating compacta, Stepper de bolinhas, Calendar com `TrapFocus`, Dense, Skeleton em linhas.
- **Site**: tabelas de parametros GERADAS da biblioteca (`Shared/ParametrosGerados.g.cs`, mesma fonte do
  `llms-full.txt`, regerada com `RVM_ATUALIZAR_IA=1`) — as 43 tabelas escritas a mao foram trocadas. Pagina do
  `RvmMascot`; exemplos de Soft, Inverse, abas por valor e estados da lista.
- Invencoes declaradas (o NEATLAB nao desenha): papel Inverse, abas Page, mascotes, link acima do campo, bolinhas do
  stepper, botao limpar da data.

**Onda 0 aprovada pelo Rafael em 07/10/2026** pelos screenshots do dev (PRs #74 a #77).

## Onda 1 — feita em 07/10/2026 (PR #79)

Pendentes do contrato: **241 → 224**. Os 15 componentes de menu lateral, pagina e navegacao: `RvmSidebar`,
`RvmTopbar`, `RvmPageHeader`, `RvmPageToolbar`, `RvmNavSubItem`, `RvmBackButton`, `RvmLink`, `RvmIconButton`,
`RvmMenuButton`, `RvmButtonGroup`, `RvmCollapse`, `RvmExpansionPanel`, `RvmSteps`, `RvmStepIndicator`,
`RvmDetailProfileLayout`.

- **Refactor:** o conteudo do menu lateral do `RvmAppShell` virou o `RvmSidebar`; a casca o usa na coluna e so cuida da
  coluna e da gaveta. Os itens de navegacao leem o recolhido do Sidebar (e, se o Sidebar for do consumidor, o da
  casca). O CSS dos itens mudou de casa — mexeu no visual do menu, e no `RvmSidebar.razor.css`.
- O tema (`RvmThemeProvider`, `RvmThemePicker`, `RvmThemeSettings`, paletas) vai numa onda propria.
- Site: secao "Navegacao e pagina" com 14 paginas, todas no E2E; o `RvmMascot` entrou no E2E tambem.
- Review independente: 4 P2 (container da ficha, fundo da gaveta, Sidebar do consumidor, componentes que escreviam o
  proprio parametro) e 1 P3 corrigidos, com teste.

⚠️ **Teste instavel que ja existia:** `planner: criar tarefa com hora no relogio` (foco no mostrador do relogio) falhou
em 4 noturnos do `master` antes desta task (29/09 a 06/10) e numa rodada do dev em 07/10. Nao e da DSGN-017; fica para
um card proprio.

## Onda 2a — feita em 07/10/2026

Pendentes do contrato: **224 → 211**. Campos de formulario do contrato: bases `RvmInputBase<TValue>` e
`RvmStringInputBase`, a moldura `RvmFieldFrame` (o visual do `RvmTextField`; o CSS dela e GERADO do CSS do campo de
texto por `tools/moldura-do-campo.py` — mudou o campo, regere), `RvmTextArea`, `RvmNumericField<TValue>`,
`RvmMultiTextField`, `RvmTextFieldSelect<TOption>`, `RvmAutocomplete<TValue>`, `RvmOptionList<TItem>`, `RvmTagOption`,
`RvmChoiceChip<TValue>`. Quatro deles feitos por agentes em paralelo (worktrees), integrados e revisados.

- ⚠️ O `RvmNumericField` traz a leitura de numero do `RvmNumberParser` do RVM.UI (codigo, nao visual): o milhar so vale
  em grupos de 3, para nao virar 1000x.
- Review independente: 1 P1 (cultura pt-BR criada no estatico derrubava o campo sem ICU — agora cai numa copia da
  invariante com os separadores do Brasil), 2 P2 e 3 P3 corrigidos.
- ⏳ **P3 que ficaram** (nao bloqueiam): seta do `RvmNumericField` sem Immediate parte do valor antigo, e nao do texto
  digitado; o Enter no `RvmAutocomplete` nunca envia o formulario (barrado pelo JS mesmo com a lista fechada);
  `Required` so existe no `RvmTextArea` (o contrato nao tem nos outros campos).

## Etapa 3 — RVM.DesignSystem

1. Alinhar os componentes em comum ao contrato (a nomenclatura já é a do DS; entram os acréscimos do UI:
   `Class` e estados `Loading`/`Empty`/`Error` em listas e tabelas).
2. Criar no DS, **no design do DS (NEATLAB)**, os 101 componentes que só existem no RVM.UI, em ondas pela ordem
   de uso mais comum: shell e navegação → formulários → tabelas e listas → feedback →
   restante. Lista em 06/10/2026: `RvmActivity`, `RvmArtisticIconBadge`, `RvmAutocomplete`, `RvmBackButton`, `RvmBubbleChart`, `RvmButtonGroup`, `RvmCellAction`, `RvmCellBadge`, `RvmCellCircleImage`, `RvmCellCode`, `RvmCellFiles`, `RvmCellLabelBadge`, `RvmCellNumber`, `RvmCellProgress`, `RvmCellRating`, `RvmCellSelect`, `RvmCellSquareImage`, `RvmCellStatus`, `RvmCellText`, `RvmCellUser`, `RvmCellUserGroup`, `RvmChartLegend`, `RvmChat`, `RvmChatContact`, `RvmChatFile`, `RvmChatGroup`, `RvmChatMessage`, `RvmChatVoice`, `RvmCollapse`, `RvmColorField`, `RvmColorPicker`, `RvmColumn`, `RvmComment`, `RvmConfirmModal`, `RvmCurrencyConverter`, `RvmDataTable`, `RvmDatePicker`, `RvmDateRangePicker`, `RvmDetailProfileLayout`, `RvmDonutChart`, `RvmEventCalendar`, `RvmEventCard`, `RvmEventPill`, `RvmExpansionPanel`, `RvmFieldFrame`, `RvmFileCard`, `RvmFileIcon`, `RvmFileTypeCard`, `RvmFileUpload`, `RvmFilter`, `RvmFilterGroup`, `RvmHeaderCell`, `RvmHistory`, `RvmIconBadge`, `RvmIconButton`, `RvmIconSelector`, `RvmKanbanBoard`, `RvmLabel`, `RvmLink`, `RvmListGroup`, `RvmMap`, `RvmMarkerButton`, `RvmMascot`, `RvmMediaUpload`, `RvmMenuButton`, `RvmMeterChart`, `RvmMiniCalendar`, `RvmModal`, `RvmMultiSelect`, `RvmMultiTextField`, `RvmMultilayerDonutChart`, `RvmNavDropdown`, `RvmNavSubItem`, `RvmNotificationItem`, `RvmNumericField`, `RvmOptionList`, `RvmPageHeader`, `RvmPageToolbar`, `RvmPaymentCard`, `RvmProfileImageUpload`, `RvmProgressBar`, `RvmProgressCard`, `RvmProjectCard`, `RvmReview`, `RvmRow`, `RvmSelect`, `RvmSidebar`, `RvmSpinner`, `RvmStatCard`, `RvmStepIndicator`, `RvmSteps`, `RvmTab`, `RvmTagOption`, `RvmTaskCard`, `RvmText`, `RvmTextArea`, `RvmTextFieldSelect`, `RvmThemePicker`, `RvmToastProvider`, `RvmTopbar`, `RvmWidget`.
3. Teste de contrato no CI do DS (falha se faltar componente, parâmetro, tipo ou valor de enum).
4. ⛔ Nada do visual do RVM.UI: o visual de cada componente novo vem dos tokens e das telas do NEATLAB; onde o NEATLAB não
   desenha o formato, é **invenção declarada** (como já é feito no DS).
5. Versão **2.0.0** (quebra a API congelada na 1.x). Consumidor conhecido: **RVM.TradeBinder** — card de migração
   lá quando a 2.0.0 sair.
6. Atualizar o escopo no `CLAUDE.md` (hoje "~35 componentes" e "não faz telas prontas do kit").

## Critério de pronto

Teste de contrato verde (DS cobre 100% do contrato), componentes documentados no site e no `llms.txt`, E2E e axe
verdes, crédito do NEATLAB preservado, 2.0.0 publicada, TradeBinder migrado.
