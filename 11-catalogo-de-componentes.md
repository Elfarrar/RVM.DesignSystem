# 11 — Catálogo de componentes (v1)

35 componentes, em quatro ondas. A coluna **Kit** aponta o arquivo de `referencia-neatlab/` que
serve de verdade visual — é contra ele que a fidelidade é conferida.

## Onda 1 — fundação (9)

Sem estes, nenhuma tela existe.

| Componente | Variações | Kit |
|---|---|---|
| `RvmButton` | contained, outlined, text, 3 tamanhos, ícone à esquerda/direita, loading | `Button.png` |
| `RvmIcon` | Tabler, 3 tamanhos, cor por papel | — |
| `RvmTextField` | outlined, filled, rótulo, apoio, erro, prefixo/sufixo | `Text Field.png` |
| `RvmCard` | básico, com cabeçalho, com ações, com mídia | `Basic Card.png`, `Card.png` |
| `RvmTypography` | toda a escala do `06` | `Typography/Light.png` |
| `RvmAlert` | 6 papéis, contained/outlined, com/sem ícone, fechável | `Alert.png` |
| `RvmChip` | filled, outlined, com avatar, removível | `Chip.png` |
| `RvmAvatar` | imagem, iniciais, ícone, 3 tamanhos, grupo | `Avatar.png` |
| `RvmDivider` | horizontal, vertical, com texto | `Border.png` ⚠️ **não é uma página de divisores** — é uma tela de dashboard. A referência é o divisor **dentro** dela (a linha em pé entre as duas colunas de saques). Conferido em 16/09/2026, na `DSGN-003` |

## Onda 2 — formulário e navegação (10)

| Componente | Variações | Kit |
|---|---|---|
| `RvmSelect` | simples, múltiplo, com busca | `Select.png` |
| `RvmCheckbox` | sozinho, com rótulo, indeterminado | `Checkbox.png` |
| `RvmRadio` | grupo horizontal e vertical | `Radio.png` |
| `RvmSwitch` | 2 tamanhos, com rótulo | `Switch.png` |
| `RvmTabs` | horizontal, vertical, com ícone, scroll | `Tabs.png` |
| `RvmBreadcrumbs` | separador padrão e custom | `Breadcrumbs.png` |
| `RvmMenu` | dropdown, com ícone, com divisor | `Menu.png` |
| `RvmPagination` | numérica, com salto, compacta | `Pagination.png` |
| `RvmBadge` | contador, ponto, posicionado | `Badge.png` |
| `RvmTooltip` | 4 posições | `Tooltip.png` |

## Onda 3 — feedback e sobreposição (8)

| Componente | Variações | Kit |
|---|---|---|
| `RvmDialog` | simples, confirmação, formulário, fullscreen | `Dailog.png` |
| `RvmDrawer` | 4 lados, temporário e permanente | `Drawer.png` |
| `RvmSnackbar` | 6 papéis, com ação, fila | `Snackbar.png`, `Toast.png` |
| `RvmProgress` | linear e circular, determinado e indeterminado | — |
| `RvmSkeleton` | texto, retângulo, círculo | — |
| `RvmAccordion` | simples, exclusivo, com ícone | `Accordion.png` |
| `RvmList` | simples, com ícone, com ação, aninhada | `List.png` |
| `RvmEmptyState` | com ilustração, com ação | — |

## Onda 4 — dados e shell (8)

| Componente | Variações | Kit |
|---|---|---|
| `RvmTable` | básica, ordenável, com seleção, densa | `Table.png` |
| `RvmDataGrid` | paginação, ordenação, filtro por coluna | `Data Grid.png` |
| `RvmDatePicker` | data, intervalo | `Date & TIme Picker.png` |
| `RvmTimePicker` | 12 h e 24 h | `Date/Time pickers.png` |
| `RvmStepper` | horizontal, vertical, com validação | `Stepper.png` |
| `RvmTimeline` | alternada, alinhada, com ícone | `Timeline.png` |
| `RvmAppShell` | topo + sidebar + conteúdo, responsivo | `Vertical Menu.png`, `Collapsed Menu.png` |
| `RvmRating` | leitura e edição, meia estrela | `Rating.png` |

## Estados obrigatórios

Todo componente interativo entrega **normal, hover, foco, ativo, desabilitado** e, quando faz
sentido, **erro** e **carregando**. Toda lista entrega **carregando, vazio e erro** — vazio que
ensina o próximo passo, não "Nenhum registro encontrado".

**Componente sem todos os estados não fecha a onda**, mesmo que o caso feliz esteja perfeito. Foi o
que separou o design system anterior de uma pasta de exemplos.

## Fora da v1

Autocomplete, transfer list, tree view, calendário, upload, editor rico, gráficos, e as telas
prontas do kit (login, invoice, chat, e-mail, e-commerce, CRM). O kit tem 141 telas; copiar tudo é
semanas antes do primeiro uso real — e sem consumidor, não se paga.
