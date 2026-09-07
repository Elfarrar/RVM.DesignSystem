# RVM.DesignSystem

Design system do ecossistema RVM: uma **biblioteca de componentes Blazor própria**, distribuída como
pacote NuGet, e um **site público de documentação** em `design.rvmit.com.br`.

> ⚠️ **Estado em 07/09/2026 — só especificação.** Existem os documentos abaixo e nada mais: sem
> código, sem solution, sem git, sem repositório remoto, sem CI/CD, sem deploy. **O projeto está sem
> backup até o bootstrap rodar.** Para construir: abrir uma sessão nesta pasta e rodar a skill
> `bootstrap-projeto`.

## Por que existe

Quatro aplicações Blazor do ecossistema escreveram, cada uma sozinha, um tema completo — paleta
clara, paleta escura, tipografia, layout:

| App | Arquivo de tema | Cor primária |
|---|---|---|
| `RVM.ERPAgro` | `Web/Services/ThemePalettes.cs` | `#0288d1` |
| `RVM.ObraEmDia` | `Web/Servicos/TemaObraEmDia.cs` | `#2A6E49` |
| `RVM.Fiscal` | `Web/FiscalTheme.cs` | `#1B5E5A` |
| `RVM.Propostinha` | `Web/PropostinhaTheme.cs` | `#302B9E` |

Dessas ~200 linhas duplicadas por app, **duas** eram de fato diferentes. O resto é decisão retomada
do zero — inclusive acessibilidade, que hoje não é verificada em lugar nenhum porque não há lugar
nenhum onde verificar.

## O que é, em uma tela

```csharp
// Program.cs do app consumidor
builder.Services.AddRvmDesignSystem(o =>
{
    o.Theme = RvmTheme.FromSeed("MeuApp", primary: "#2A6E49", secondary: "#B07D2A");
    o.DefaultMode = RvmThemeMode.System;
});
```

```razor
<RvmThemeProvider>
    <RvmToastHost /><RvmDialogHost />
    @Body
</RvmThemeProvider>

<RvmButton Variant="RvmButtonVariant.Primary" Loading="@salvando" OnClick="SalvarAsync">
    Salvar
</RvmButton>
```

Duas cores definem a identidade do produto; tipografia, espaçamento, comportamento e acessibilidade
vêm prontos e idênticos em todo o ecossistema.

## Decisões que definem o projeto

| Decisão | Consequência |
|---|---|
| **Componentes próprios, sem MudBlazor** | Controle total e acessibilidade sob nossa responsabilidade. Custa caro em `DataGrid`, `Autocomplete` e `DatePicker` — decisão do Rafael, com o preço registrado em `01` |
| **Base comum, paleta por produto** | Fiscal continua teal, ObraEmDia continua verde. Só a cor varia |
| **Só projetos novos adotam** | Os quatro apps atuais seguem no MudBlazor, sem prazo de migração |
| **O site é feito com a biblioteca** | Componente ruim de usar dói primeiro em quem escreve a documentação dele |
| **Acessibilidade é portão de CI** | Contraste calculado em teste unitário; axe no E2E. Reprovou, não entra |
| **Sem backend** | Sem banco, sem API, sem auth, sem container. O site é estático no GitHub Pages |

## Documentos

| # | Arquivo | O que responde |
|---|---|---|
| 01 | [`01-visao-geral.md`](01-visao-geral.md) | Problema, solução, escopo faz/não faz, fronteira com o ecossistema |
| 02 | [`02-requisitos.md`](02-requisitos.md) | Requisitos funcionais e não funcionais, acessibilidade, desempenho |
| 03 | [`03-arquitetura.md`](03-arquitetura.md) | Estrutura da solution, CSS, JS, testes — e por que não é VSA |
| 04 | [`04-modelo-de-dados.md`](04-modelo-de-dados.md) | Não há banco: o modelo de **tokens** ocupa o lugar |
| 05 | [`05-api-dos-componentes.md`](05-api-dos-componentes.md) | Contrato público C#/Razor, convenções, SemVer |
| 06 | [`06-tokens-e-tematizacao.md`](06-tokens-e-tematizacao.md) | **Motor do domínio**: como o tema é aplicado, trocado e verificado |
| 07 | [`07-site-de-documentacao.md`](07-site-de-documentacao.md) | Estrutura do site, playground, por que Blazor e não Astro |
| 08 | [`08-monetizacao.md`](08-monetizacao.md) | Não monetiza — o que devolve e quanto custa operar |
| 09 | [`09-roadmap.md`](09-roadmap.md) | Fase 0 + quatro ondas + **pendências que travam o contrato** |
| 10 | [`10-infra-e-integracoes.md`](10-infra-e-integracoes.md) | Ambientes, DNS, CI/CD, BaGet, segredos, monitoramento |
| 11 | [`11-catalogo-de-componentes.md`](11-catalogo-de-componentes.md) | Os 35 componentes da v1, por onda |
| — | [`CLAUDE.md`](CLAUDE.md) | Contrato para a sessão que for construir |

## Roadmap em uma linha

**Fase 0** bootstrap → **Onda 1** tokens + básicos (`0.1.0`) → **Onda 2** layout (`0.2.0`) →
**Onda 3** feedback (`0.3.0`) → **Onda 4** dados (`1.0.0`).

O site publica desde a onda 1 — nada de meses de biblioteca invisível.

## Ambientes

| Ambiente | Domínio | Branch |
|---|---|---|
| Dev | `design.dev.rvmtech.com.br` | `dev` |
| Prod | `design.rvmit.com.br` | `master` |

Não há demo, de propósito: demo existe para dado curado, e aqui não há dado.

## Como construir

```
cd C:\IA\RVM.DesignSystem
# abrir sessão do Claude Code aqui e rodar a skill: bootstrap-projeto
```

O bootstrap cria git, repositório, solution, CI/CD, DNS e o primeiro deploy — nesta ordem, e é ele
quem tira o projeto do estado "sem backup".
