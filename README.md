# RVM.DesignSystem

Biblioteca de componentes **Blazor** do ecossistema RVM — Razor Class Library distribuída como
pacote NuGet, com site público de documentação construído com a própria biblioteca.

> ## Créditos de design
>
> O visual desta biblioteca é uma reimplementação do **NEATLAB — Super Admin Dashboard UI Design
> Kit**, de **[hello.uiworld](https://www.figma.com/community/file/1353029852156886079/neatlab-super-admin-dashboard-ui-design-kit)**,
> publicado na Figma Community sob **[CC BY 4.0](https://creativecommons.org/licenses/by/4.0/)**.
>
> O código deste repositório é MIT; o crédito ao design de origem é obrigatório e permanece em
> todos os canais de distribuição.

## Estado

**Spec pronta, código zero** (16/09/2026). Para construir: abrir uma sessão nesta pasta e rodar a
skill `bootstrap-projeto`.

## Documentação

| Arquivo | Assunto |
|---|---|
| [01-visao-geral.md](01-visao-geral.md) | O que é, faz / não faz, fronteira com os vizinhos |
| [02-requisitos.md](02-requisitos.md) | Quem usa, requisitos funcionais e não funcionais |
| [03-arquitetura.md](03-arquitetura.md) | Solução, organização interna, ADRs |
| [04-modelo-de-dados.md](04-modelo-de-dados.md) | Não há banco — a estrutura dos tokens |
| [05-api-dos-componentes.md](05-api-dos-componentes.md) | Convenções de API e política de versão |
| [06-tokens-e-tematizacao.md](06-tokens-e-tematizacao.md) | **O motor** — paleta, tipografia, tema |
| [07-site-de-documentacao.md](07-site-de-documentacao.md) | O site e as armadilhas de WASM estático |
| [08-monetizacao.md](08-monetizacao.md) | Não monetiza — licenças e a obrigação da CC BY |
| [09-roadmap.md](09-roadmap.md) | Ondas, `1.0.0` e as pendências |
| [10-infra-e-integracoes.md](10-infra-e-integracoes.md) | Ambientes, CI/CD, segredos, monitor |
| [11-catalogo-de-componentes.md](11-catalogo-de-componentes.md) | Os 35 componentes da v1 |
| `CLAUDE.md` | Contrato de desenvolvimento |
| `referencia-neatlab/` | 141 telas do kit em PNG, claro e escuro (99 MB) |

## Stack

.NET 10 · Blazor (Server e WebAssembly) · Razor Class Library · bUnit · Playwright + axe ·
fonte **Inter** (OFL) · ícones **Tabler** (MIT).

**Sem MudBlazor** e sem qualquer biblioteca de componentes de terceiros.
