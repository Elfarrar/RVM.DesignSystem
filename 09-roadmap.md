# 09 — Roadmap

Prefixo de task: **`DSGN-`**, contador próprio, começando em `DSGN-001`.

Não há data prometida. A ordem é o que importa, e a ordem é escolhida por uma regra só: **o site
publica desde a onda 1**. Nada de meses de biblioteca invisível — cada onda termina com algo público
no ar em `design.rvmit.com.br`.

## Fase 0 — Bootstrap (`DSGN-001` … `DSGN-00n`)

Executada pela skill `bootstrap-projeto`, **dentro** de `C:\IA\RVM.DesignSystem`.

| Entrega | Verificação |
|---|---|
| `git init`, repositório no GitHub, `.gitattributes` com `eol=lf` | `git push` na `master` |
| Solution .NET 10: RCL + Docs (WASM) + Tests + E2E | `dotnet build` sem warning |
| `ci.yml` caller do `RVM.Actions@v1` | Um PR com CI verde |
| **Runner self-hosted registrado no BagEnd para este repositório** | Job sai de `queued` |
| Workflow de publicação no BaGet | Versão `0.1.0-alpha` instalável |
| Workflow de publicação do site (Pages) + DNS `design.rvmit.com.br` | `curl -I` devolve 200 e certificado válido |
| ADR-010 no Vault registrando o desvio de VSA | Arquivo commitado |

⚠️ O runner é o passo que trava sem avisar: repositório novo tem zero runner, e o job fica `queued`
para sempre, **sem erro nenhum** (`padrao-rvm` §5).

## Onda 1 — Tokens e básicos

**Meta:** um formulário completo, coerente e acessível, montado só com componentes `Rvm*`.

- Camada de tokens completa (cor, tipografia, espaçamento, raio, sombra, motion, z-index, breakpoint)
- `RvmTheme` + `FromSeed` + `RvmThemeProvider` + modo claro/escuro + script anti-flash
- **Teste de contraste de toda paleta** — o portão do `06`
- `RvmButton`, `RvmIconButton`, `RvmTextField`, `RvmTextArea`, `RvmSelect`, `RvmCheckbox`,
  `RvmRadioGroup`, `RvmSwitch`, `RvmLabel`/`RvmFormField`
- Site: casca, páginas de fundamentos, páginas dos componentes acima, código copiável, guia de
  instalação, changelog
- Pacote `0.1.0` no BaGet

**Verifica:** o site está no ar com o seletor de tema funcionando nas quatro identidades; um
formulário de exemplo passa na auditoria axe e é operável só por teclado.

## Onda 2 — Layout

**Meta:** a casca de aplicação que hoje está duplicada em quatro `MainLayout`.

- `RvmAppShell`, `RvmSidebar` (colapsável, drawer no mobile), `RvmTopbar`, `RvmNavItem`
- `RvmCard`, `RvmGrid`/`RvmStack`, `RvmTabs`, `RvmBreadcrumb`, `RvmDivider`, `RvmChip`, `RvmAvatar`
- `RvmIcon` + conjunto de ícones resolvido (pendência do `03`)
- Densidade `Compact` (`RF-05`)
- Site: página de API gerada da doc XML (`RF-29`), playground (`RF-22`)
- Pacote `0.2.0`

**Verifica:** o próprio site passa a usar `RvmAppShell` — a casca deixa de ser código de exemplo e
vira código em produção.

## Onda 3 — Feedback

**Meta:** os estados que hoje cada app resolve à sua maneira.

- `RvmDialog` + `IRvmDialogService` + `ConfirmAsync` (foco preso, `ESC`, retorno de foco)
- `RvmToast` + `IRvmToastService` + `RvmToastHost`
- `RvmAlert`, `RvmSkeleton`, `RvmSpinner`, `RvmProgress`, `RvmEmptyState`, `RvmTooltip`
- Site: seção Padrões (formulário, estados vazios), busca (`RF-27`)
- Pacote `0.3.0`

**Verifica:** E2E abre o diálogo, navega só por teclado, fecha com `ESC` e confirma que o foco
voltou ao botão de origem.

## Onda 4 — Dados

**Meta:** a listagem completa. É a onda mais cara do projeto — três dos componentes mais difíceis de
qualquer biblioteca de UI estão aqui (ver `01` § ideia 2).

- `RvmDataGrid<T>`: colunas tipadas, ordenação, seleção, template por célula, estado vazio
- Modo remoto (callback de página + total) além do modo em memória (`RF-12`)
- `RvmPagination`, `RvmFilterBar`
- `RvmDatePicker` (teclado em `dd/MM/yyyy`, calendário pt-BR), `RvmAutocomplete` (busca assíncrona,
  debounce, navegação por teclado)
- Site: seção Padrões § Listagem
- Pacote **`1.0.0`** — primeira versão estável, contrato congelado sob SemVer

**Verifica:** uma tela de listagem real (dados de exemplo, ~500 linhas) com filtro, ordenação e
paginação, operável só por teclado, aprovada na auditoria axe.

## Adoção — o que acontece depois da 1.0

Decisão de 07/09/2026: **só projetos novos.** ERPAgro, ObraEmDia, Fiscal e Propostinha seguem no
MudBlazor, sem prazo de migração e sem trabalho de compatibilidade.

O próximo projeto RVM que precisar de UI nasce no design system, e é ele quem prova a biblioteca.
Migrar um app existente só entra em pauta **depois** disso, se entrar — e será decisão nova, com
custo próprio, não continuação automática deste roadmap.

## Fora da v1 — com destino declarado

| Item | Destino |
|---|---|
| Gráficos (chart, sparkline) | v2, se algum app RVM precisar. Provavelmente embrulhando uma biblioteca existente, não do zero |
| Virtualização de lista longa | v2 — só quando houver tela real com mais de 5 mil linhas |
| Editor de texto rico | Não previsto. Se surgir, embrulhar biblioteca existente |
| Drag-and-drop / kanban | Não previsto |
| RTL | Não previsto — nenhum app RVM atende idioma RTL |
| Pipeline de tokens (Style Dictionary, Figma) | v2, só se surgir uma segunda superfície (mobile, e-mail) consumindo os mesmos tokens |
| Publicação no nuget.org | Depois da 1.0, se o repositório for público |
| Migração de app existente | Ver § Adoção |

## Pendências que travam o contrato

O análogo, aqui, de "pendência que muda o modelo de dados": **depois da 1.0 publicada, mudar
qualquer uma destas quebra todo consumidor.**

> **Estado em 08/09/2026: as seis fechadas.** A paleta da marca — roxo do Visual Studio + azul
> do VS Code, do `rvmtech.com.br` — entrou depois e também está fechada, **incluindo o modo
> escuro** (`06` § A identidade da marca).
>
> Fica **uma decisão de contrato para a onda 2**, que não estava nesta lista: **quais ícones
> entram no sprite**. Cada um é um nome público e bytes que todo consumidor baixa, então a lista
> é curada junto dos componentes que os usam — não "por precaução".

1. ✅ **Inglês.** Não por omissão — o `CLAUDE.md` do projeto já traz a regra ("nome de API
   pública em inglês, texto ao usuário final em PT-BR"), e a `DSGN-002` implementou assim
   (`RvmPalette.OnSurface`, `--rvm-color-on-surface`).
2. ✅ **Phosphor** — decidido pelo Rafael em 08/09/2026. 1512 ícones, MIT puro, re-empacotados no
   build. O que decidiu foi o **peso preenchido**: o Phosphor publica `regular` e `fill` (entre
   outros quatro), e esse par é o que expressa estado selecionado sem improviso. A v1 expõe só
   esses dois — acrescentar peso depois é aditivo, remover é que quebra (`03` § Ícones).
3. ✅ **`rem` fixo, com `clamp()` só no `display`.** Implementado na `DSGN-002`
   (`rvm-tokens.css`). Previsível vale mais que elegante numa v1; o `display` é a exceção porque
   um título de 3rem estoura a linha no celular.
4. ✅ **pt-BR fixo, sem `.resx`.** O `CLAUDE.md` do projeto já exige que todo texto padrão de
   componente seja sobrescrevível por parâmetro, o que resolve o caso real (app que precisa de
   outra palavra) sem o peso de um pipeline de recursos.
5. ✅ **Público sob MIT** — decidido pelo Rafael em 07/09/2026. Consequência que ninguém previa:
   foi essa decisão que inviabilizou o caller do `RVM.Actions` (repositório público não chama
   reusable de repositório privado, **ADR-011**) e, de quebra, dispensou o runner self-hosted.
6. ✅ **Implementado nos dois modos.** `UseLocalDesignSystem` no `Directory.Build.props` alterna
   entre `ProjectReference` e pacote do BaGet. Hoje `true` por padrão, porque o site precisa
   compilar contra o código local enquanto a biblioteca muda a cada task.
