# 01 — Visão geral

**RVM.DesignSystem** é a biblioteca de componentes Blazor do ecossistema RVM: uma Razor Class
Library própria, distribuída como pacote NuGet no BaGet interno, acompanhada de um site público de
documentação feito com a própria biblioteca.

O visual é uma **reimplementação do kit NEATLAB** — não uma inspiração vaga: a paleta, a escala
tipográfica e a anatomia dos componentes saem do kit, medidos, e o resultado tem que ser
reconhecível lado a lado com o Figma.

## Crédito obrigatório (CC BY 4.0)

> O NEATLAB — Super Admin Dashboard UI Design Kit é de **`hello.uiworld`**, publicado na Figma
> Community sob **Creative Commons Attribution 4.0**. A licença permite uso comercial e obras
> derivadas **e exige atribuição**.

Onde o crédito aparece, sem exceção: `README.md` do repositório · rodapé de todas as páginas do site
de documentação · campo `PackageProjectUrl`/`Description` do pacote NuGet · este arquivo.

**Isso não é formalidade** — é a condição que torna o projeto legítimo. Repositório público sem o
crédito é violação de licença à vista de todos.

## Por que existe

Os quatro apps do ecossistema usam MudBlazor e cada um resolveu aparência do seu jeito. A biblioteca
existe para que **projeto novo comece com a mesma cara**, sem recomeçar decisões de cor, espaçamento
e estado de tela a cada repositório.

## Faz

- Biblioteca de componentes Blazor **própria** (RCL), publicada no BaGet — **~35 componentes na v1**,
  em ondas (`09-roadmap.md`).
- Camada de **tokens** (cor, tipografia, espaçamento, raio, sombra, z-index, breakpoint) e motor de
  **tematização** claro/escuro (`06-tokens-e-tematizacao.md`).
- **Site público de documentação**, construído com a biblioteca — é a prova viva de que ela funciona.
- **Acessibilidade WCAG 2.1 AA** como critério de aceite de cada componente.
- **Blazor Server e WebAssembly**, os dois, sempre.

## Não faz

- **Não usa MudBlazor**, nem por baixo, nem como ponte. Decisão do Rafael, mantida em 16/09/2026.
- **Não usa nenhuma biblioteca de componentes de terceiros.** Ícones são SVG do Tabler, copiados
  para dentro do repositório — não há dependência de runtime.
- **Não migra os apps existentes.** ERPAgro, ObraEmDia, Fiscal e Propostinha seguem no MudBlazor,
  sem prazo.
- **Não tem backend**: sem banco, sem API HTTP, sem auth, sem multi-tenant, sem container.
- **Não copia as telas prontas do kit** (login, invoice, chat, e-mail). O kit tem 141 telas; a v1
  entrega **componentes**, não páginas de produto.
- Não faz gráfico, editor rico, drag-and-drop nem RTL na v1.

## Fronteira contra os projetos vizinhos

| Projeto | Fronteira |
|---|---|
| **RVM.TradeBinder** | ⏳ **Não é consumidor.** O `ADR-004` de lá segue aberto — a UI dele é decisão em separado, tomada pelo Rafael, e a existência desta biblioteca **não** a resolve automaticamente |
| **RVM.Cockpit** | ⏳ Mesma situação: a spec dele marca a UI como pendente |
| **RVM.Common** | Pacotes utilitários (segurança, formatação). Aqui é só UI; nada de regra de negócio |
| **RVM.Site / RVM.Curriculo** | Sites institucionais em Astro. Não consomem esta biblioteca |
| Apps em MudBlazor | Continuam como estão. Adoção **nunca** é retroativa |

## Estado

Projeto **recriado do zero em 16/09/2026**. A encarnação anterior (07–16/09/2026: 35 componentes,
site no ar, pacote `1.2.0`) foi **apagada por inteiro a mando do Rafael**, sem backup — repositório,
pacotes, sites e DNS. Ver `TASK-834` no hub.

**Nada foi herdado**: nem código, nem histórico, nem versão de pacote. O que sobrevive são as
decisões registradas em `03-arquitetura.md` (por que não segue VSA, por que o CI é próprio) e as
armadilhas de CI/publicação, que custaram tempo e não precisam ser redescobertas.
