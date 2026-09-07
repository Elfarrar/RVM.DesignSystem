# CLAUDE.md — RVM.DesignSystem

Guia de desenvolvimento do projeto. Complementa as diretrizes globais
(`C:\Users\rvene\.claude\CLAUDE.md`) e o padrão do ecossistema (skill `padrao-rvm`).

**Prefixo de task:** `DSGN-NNN` · contador próprio · card em `docs/Vault/02_Tasks/`,
índice em `docs/Vault/03_Kanban/KANBAN.md`.

> **Estado em 07/09/2026: só documentação.** Existem os MD `01`–`11`, o `README.md` e este arquivo.
> Não há código, solution, git, repositório no GitHub, CI/CD nem deploy. Para construir, rodar a
> skill `bootstrap-projeto` **dentro** desta pasta. Até lá, o projeto está **sem backup**.

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
em `03-arquitetura.md` § Desvio; registrar como **ADR-010** no Vault durante o bootstrap.
**Não "corrigir" a arquitetura para VSA.**

- **Solution:** `src/RVM.DesignSystem` (RCL, o pacote) · `src/RVM.DesignSystem.Docs` (Blazor WASM,
  o site) · `test/RVM.DesignSystem.Tests` (bUnit) · `test/RVM.DesignSystem.E2E` (Playwright + axe)
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
- CI/CD: `ci.yml` e `e2e.yml` são callers de `Elfarrar/RVM.Actions@v1`. `publish-nuget.yml`,
  `deploy-development.yml` e `deploy-pages.yml` são próprios — o `deploy.yml@v1` pressupõe container
  e aqui não há (desvio declarado em `10`).
- ⚠️ **Runner self-hosted é por repositório.** Sem ele registrado no BagEnd, o job fica `queued`
  para sempre, sem erro. Passo da fase 0.
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

Publicação de pacote acompanha a branch: pré-release (`0.x.y-alpha.N`) a partir de `dev`, versão
estável só de `master`.

## Modo de trabalho

**Eu implemento, o Rafael revisa** (decisão de 07/09/2026). Entrega inclui screenshot do Playwright
da tela/componente novo — sem isso, a entrega está incompleta.

> Diferente do RVM.Mimic, onde o modo é "o Rafael coda, eu guio". Aqui não.

## Estado atual

- Spec `01`–`11` escrita em 07/09/2026. Código zero.
- Próximo passo: `bootstrap-projeto` nesta pasta → fase 0 do `09-roadmap.md`.

## Pendências que bloqueiam

Detalhe e suposição vigente de cada uma em `09-roadmap.md` § Pendências. As três primeiras precisam
ser respondidas **antes de fechar a onda 1** — depois da 1.0, mudá-las quebra todo consumidor.

- ⏳ Idioma dos nomes de token e API (suposição: inglês)
- ⏳ Conjunto de ícones (suposição: Lucide, MIT)
- ⏳ Escala tipográfica: `rem` fixo ou `clamp()` (suposição: `rem` fixo)
- ⏳ Localização: `.resx` ou pt-BR fixo (suposição: pt-BR fixo)
- ⏳ Repositório público sob MIT ou privado (suposição: público, MIT)
- ⏳ Site referencia o pacote do BaGet ou `ProjectReference` (suposição: pacote, com alternância local)
