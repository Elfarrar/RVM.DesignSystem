---
id: DSGN-017
titulo: Paridade de API com o RVM.UI — etapa 3 (componentes do UI no DS, comuns alinhados ao contrato, 2.0.0)
repo: RVM.DesignSystem
tipo: feature
status: todo
criada: 2026-10-06
atualizada: 2026-10-06
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
