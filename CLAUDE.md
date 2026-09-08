# CLAUDE.md — RVM.DesignSystem

Guia de desenvolvimento do projeto. Complementa as diretrizes globais
(`C:\Users\rvene\.claude\CLAUDE.md`) e o padrão do ecossistema (skill `padrao-rvm`).

**Prefixo de task:** `DSGN-NNN` · contador próprio · card em `docs/Vault/02_Tasks/`,
índice em `docs/Vault/03_Kanban/KANBAN.md`.

> **Estado em 08/09/2026 — onda 1 fechada e em produção.** Repositório **público**
> (MIT), nove componentes básicos, camada de tokens, motor de tema com claro/escuro,
> ícones Phosphor, e site de documentação com 16 páginas — no ar em
> `design.rvmit.com.br` e `design.dev.rvmtech.com.br`.
> **Pacote estável `0.1.0`** no BaGet, publicado pela tag `v0.1.0`.
> Próximo: onda 2 (Layout) — `RvmAppShell`, `RvmSidebar`, `RvmCard`, `RvmTabs`.

## Escopo

**Faz:**
- Biblioteca de componentes Blazor **própria** (Razor Class Library), publicada como pacote NuGet no
  BaGet interno — ~35 componentes na v1, em quatro ondas.
- Camada de tokens (cor, tipografia, espaçamento, raio, sombra, motion, z-index, breakpoint) e
  motor de tematização: claro/escuro, paleta por produto, densidade.
- Site público de documentação em `design.rvmit.com.br`, feito **com** a biblioteca.
- Acessibilidade WCAG 2.1 AA como critério de aceite de cada componente.

**Não faz:**
- **Não usa MudBlazor** — nem por baixo, nem como ponte. Decisão do Rafael em 07/09/2026, com a
  recomendação contrária registrada em `01-visao-geral.md` § Decisão registrada. Se for reaberta,
  que seja de propósito.
- **Não migra os apps existentes.** ERPAgro, ObraEmDia, Fiscal e Propostinha seguem no MudBlazor,
  sem prazo. Adoção é só em projeto novo.
- **Não tem backend**: sem banco, sem API HTTP, sem auth, sem multi-tenant, sem container.
- Não faz gráfico, editor rico, drag-and-drop, RTL nem white-label por tenant (`09` § Fora da v1).
- Não define identidade de marca (logo, tom de voz) — consome uma.

## Arquitetura

⚠️ **Este projeto NÃO segue VSA + MediatR**, e isso é decisão registrada, não esquecimento.
VSA organiza casos de uso que atravessam camadas; aqui não há caso de uso, endpoint nem banco — o
artefato é uma Razor Class Library e a unidade de organização é o componente. Justificativa completa
em `03-arquitetura.md` § Desvio, e registrada em `docs/Vault/05_References/ADR-010-sem-vsa.md`.
**Não "corrigir" a arquitetura para VSA.**

- **Solution:** `RVM.DesignSystem.slnx` — `src/RVM.DesignSystem` (RCL, o pacote) ·
  `src/RVM.DesignSystem.Docs` (Blazor WASM, o site) · `test/RVM.DesignSystem.Tests` (bUnit) ·
  `test/playwright/RVM.DesignSystem.E2E` (Playwright + axe)
- ⚠️ **O E2E mora sob `test/playwright/` de propósito, não por capricho de nome.** O `ci.yml` do
  ecossistema roda todo `test/**/*.csproj` e pula só os caminhos que contêm `playwright`; o E2E
  aqui exige o site publicado no ar. Mover para `test/RVM.DesignSystem.E2E` faz o CI tentar
  rodá-lo sem site.
- **Auth:** none — site público e anônimo, biblioteca não autentica
- **Multi-tenant:** não se aplica (sem dado, sem tenant). Variação é de tema, em tempo de build
- **Blazor Server E WebAssembly** — os dois, sempre. Nenhum componente pode depender de JS para
  renderizar seu estado inicial; JS só em `OnAfterRenderAsync`

## Convenções

- **Nome de API pública em inglês** (`RvmButton.Variant`), **texto ao usuário final em PT-BR**
  explicativo. As duas regras convivem e a razão está em `05-api-dos-componentes.md` § Convenções.
- Componente nunca lê token primitivo (`--rvm-blue-500`); só semântico (`--rvm-color-primary`).
  Há teste que varre o CSS e reprova o build se acontecer.
- Toda classe CSS pública com prefixo `rvm-`, além do CSS isolation do Blazor.
- Sem dependência NuGet de terceiros na biblioteca. Sem CDN em runtime — fonte e ícone
  auto-hospedados.
- `Class` do consumidor **soma** com as classes internas, nunca é descartado.
- Data/número/moeda com `CultureInfo` explícito; BRL por `BrlFormatter`, nunca interpolação manual.
- Todo texto padrão de componente é sobrescrevível por parâmetro — nenhum literal preso no meio.
- `prefers-reduced-motion` respeitado em toda animação.
- Altura e espaçamento vertical de controle saem de `--rvm-control-*`, nunca de `--rvm-space-*`
  direto. É o que faz a densidade `Compact` valer sem o componente saber que ela existe — e um
  componente que use a escala de espaçamento no eixo vertical a ignora **em silêncio**.
- CSS de um componente não alcança elemento renderizado por outro sem `::deep` — escopos `b-*`
  diferentes. Vale também para o conteúdo que o consumidor passa num slot, e aí `::deep` costuma
  ser a resposta errada: a biblioteca garante o contêiner, o consumidor estiliza o que é dele.

## Portões que não se negociam

1. **Contraste**: teste unitário calcula todo par `x`/`on-x` de toda paleta; abaixo de AA, CI vermelho.
2. **Cobertura ≥ 80%** (portão do `ci.yml` do ecossistema).
3. **axe no E2E** do site; violação séria reprova.
4. **Zero warning** no `Release` (`TreatWarningsAsErrors` na biblioteca).

## Infra e ambientes

| Ambiente | Domínio | Branch | Dados | Onde |
|---|---|---|---|---|
| Dev | `design.dev.rvmtech.com.br` | `dev` | não há | Rivendell — estático via Nginx (rsync) |
| Demo | — | — | — | **não existe, de propósito** |
| Prod | `design.rvmit.com.br` | `master` | não há | GitHub Pages |

- **Sem container, sem banco, sem rede `rvmtech`, sem `/health`.** Verificação de deploy é `curl -I`
  devolvendo 200.
- CI/CD: **os cinco workflows são próprios**, nenhum é caller do `RVM.Actions@v1`, e cada um roda
  em `ubuntu-latest`. Motivos, um por um:
  - `ci.yml` — repositório **público** não chama reusable de repositório **privado**, e o
    `RVM.Actions` é privado; o run morria em 0s, sem jobs e sem mensagem útil (**ADR-011**).
  - `e2e.yml` — o `e2e.yml@v1` sobe stack local por `docker compose`, espera `/health` de API
    instrumentada e roda Playwright por npm. Nenhuma das premissas existe aqui.
  - `deploy-development.yml`, `deploy-pages.yml`, `publish-nuget.yml` — o `deploy.yml@v1`
    pressupõe container e aqui não há.
- ✅ **Este projeto NÃO precisa de runner self-hosted.** A regra do ecossistema (runner é por
  repositório; sem ele o job fica `queued` para sempre, sem erro) continua valendo em geral, mas
  não morde aqui: repositório público tem runner do GitHub de graça. **Nada a registrar no BagEnd.**
- Pages exige `.nojekyll`, `404.html` copiado do `index.html` e `<base href="/">` — os três detalhes
  que quebram Blazor WASM em host estático (`10`).

## Segredos

| Segredo | Onde | Secret no GitHub |
|---|---|---|
| SSH da Rivendell (rsync do dev) | `RVM.Infra/docs/ACESSOS.md` | `DEV_VPS_HOST/PORT/USER/SSH_KEY_B64/DEPLOY_PATH` |
| API key do BaGet | `ACESSOS-E-SENHAS.md` global | `BAGET_API_KEY` |
| URL do feed BaGet | `ACESSOS-E-SENHAS.md` global | `BAGET_FEED_URL` |

**Não há `.env`** — não existe aplicação servidora, então nada de `DEV_ENV_FILE`/`PROD_ENV_FILE`.
Pages usa o `GITHUB_TOKEN` nativo. `docs/ACESSOS.md` gitignored desde o primeiro commit.

## Workflow

Fluxo do ecossistema (`padrao-rvm` §7): card `DSGN-NNN` → branch `dsgn-NNN` de `master` → commits
isolados (`git add <arquivos>`, nunca `-A`) → testes ≥ 80% → E2E → review → `dev` → validação do
Rafael pelo **screenshot** → `master` só com sinal verde explícito dele.

Não há `demo`: o fluxo vai de `dev` direto para `master`.

Publicação de pacote: pré-release (`0.x.y-alpha.N`, numerado pelo run) a cada push em `dev`;
**versão estável só de TAG `vX.Y.Z`** — push em `master` não publica.

> Mudou em 08/09/2026 (`DSGN-009`). A versão estável saía do `VersionPrefix` do csproj a cada push
> em `master`, e isso fazia dela um **alvo móvel**: perdido o feed, o republish entregaria o mesmo
> número com código diferente. A tag prende a versão ao commit — e é **a condição que sustenta a
> decisão de não fazer backup do BaGet** (`Vault/05_References/baget-sem-backup.md`).

## Modo de trabalho

**Eu implemento, o Rafael revisa** (decisão de 07/09/2026). Entrega inclui screenshot do Playwright
da tela/componente novo — sem isso, a entrega está incompleta.

> Diferente do RVM.Mimic, onde o modo é "o Rafael coda, eu guio". Aqui não.

## Estado atual

- Spec `01`–`11` escrita em 07/09/2026; bootstrap (`DSGN-001`) executado no mesmo dia.
- No ar: `https://design.dev.rvmtech.com.br`.
- **Produção no ar desde 08/09/2026**: `https://design.rvmit.com.br`, autorizada por ele.
  Certificado do Pages emitido, `Enforce HTTPS` ligado, verificação por conteúdo.
- ✅ **Monitor no Uptime-Kuma**: `DesignSystem - prod`, tipo `keyword` procurando
  `RVM Design System` (não código HTTP — host estático com fallback devolve 200 com a página
  errada), com alerta de expiração de certificado. O canal de e-mail via Resend foi criado no
  mesmo passo: até 08/09/2026 o Kuma tinha 17 monitores e **zero** canais.
- ✅ **Onda 1 fechada e publicada como `0.1.0`** (`DSGN-012`): tokens, motor de tema, nove
  componentes básicos, ícones Phosphor e o site com página por componente.
- ✅ **Onda 2 fechada e EM PRODUÇÃO** (`DSGN-017`, autorizada por ele em 08/09/2026): casca de
  aplicação, layout, navegação e densidade `Compact`. O critério de saída era o próprio site usar
  `RvmAppShell` — cumprido em `Docs/Layout/MainLayout.razor`, e agora é o que serve
  `design.rvmit.com.br`.
- ⚠️ **Rota profunda em produção devolve HTTP 404 com a página certa.** É o fallback do GitHub
  Pages: caminho desconhecido serve o `404.html`, que é cópia do `index.html`, e o Blazor roteia
  no cliente. Não é regressão e não tem conserto em host estático — **verificação de deploy é por
  CONTEÚDO, nunca por código HTTP**, e é por isso que o monitor do Kuma é do tipo `keyword`.
- Próximo passo: onda 3 do `09-roadmap.md` — `RvmDialog`, `RvmToast` e o resto do feedback.

## Pendências que bloqueiam

Detalhe de cada uma em `09-roadmap.md` § Pendências. Depois da 1.0, mudá-las quebra todo consumidor.

**Nenhuma continua aberta.** Todas fechadas até 08/09/2026:

| Pendência | Decisão |
|---|---|
| Idioma de token/API | inglês |
| Escala tipográfica | `rem` fixo, `clamp()` só no `display` |
| Localização | pt-BR fixo, texto sobrescrevível por parâmetro |
| Visibilidade | público, MIT |
| Referência do site | os dois modos, via `UseLocalDesignSystem` |
| **Ícones** | **Phosphor** (MIT), pesos `Regular` e `Fill` na v1 |
| Paleta da marca | roxo VS + azul VS Code, **modo escuro aprovado** |

✅ **A decisão que sobrava — quais ícones entram — foi fechada na onda 2**: o conjunto é
**curado**, hoje com 19 ícones, cada um entrando junto do componente que o usa. A política está
escrita no topo do `tools/gerar-icones.py` e vale para sempre: ícone não entra "porque pode ser
útil". Cada um é um nome público e bytes que todo consumidor baixa.
- ✅ **Repositório público sob MIT** — respondido pelo Rafael em 07/09/2026. Consequência não
  óbvia: foi essa decisão que inviabilizou o caller do `RVM.Actions` (**ADR-011**) e, de quebra,
  dispensou o runner self-hosted.
- 🔧 Site referencia o pacote do BaGet ou `ProjectReference` — **implementado nos dois modos**.
  `UseLocalDesignSystem` no `Directory.Build.props` alterna; hoje `true` por padrão, porque o
  `0.1.0-alpha` ainda não existe no feed. Vira `false` quando o pacote for publicado.
