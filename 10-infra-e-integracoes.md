# 10 — Infraestrutura e Integrações

## Ambientes

| Ambiente | Domínio | Branch | Onde | Dados |
|---|---|---|---|---|
| **Dev** | `design.dev.rvmtech.com.br` | `dev` | Rivendell — arquivos estáticos servidos pelo Nginx existente | Não há dados |
| **Demo** | — | — | **Não existe** | — |
| **Prod** | `design.rvmit.com.br` | `master` | GitHub Pages | Não há dados |

Três declarações explícitas, para não serem lidas como esquecimento:

- **Não haverá ambiente demo.** Demo existe para dado curado de cliente; aqui não há dado nenhum, e
  o site de produção já é a demonstração.
- **Não há container, banco nem rede `rvmtech`** para este projeto. O site é HTML/CSS/WASM estático.
  Nenhuma linha no `docker-compose` de nenhuma VPS.
- **Não há `/health`.** A verificação de deploy é `curl -I` devolvendo 200 na página inicial e o
  hash do build publicado batendo com o commit.

### DNS

| Registro | Tipo | Alvo | Onde |
|---|---|---|---|
| `design.rvmit.com.br` | `CNAME` | `<usuario>.github.io` | Zona `rvmit.com.br` (Hostinger) |
| `design.dev.rvmtech.com.br` | `A` | IP da Rivendell | Zona `rvmtech.com.br` (Hostinger) |

Certificado de produção é gerenciado pelo próprio GitHub Pages (Let's Encrypt automático, com
"Enforce HTTPS" ligado). O de dev entra no certbot da Rivendell, junto dos demais — e vale a
armadilha registrada do ecossistema: **renovar sem recarregar o Nginx derruba o deploy seguinte**.

## Deploy

### Produção — GitHub Pages

`dotnet publish` do projeto `.Docs` → `wwwroot` → artefato → Pages.

Três detalhes que quebram Blazor WASM no Pages, listados porque cada um já custou uma tarde a
alguém, em algum lugar:

1. **`.nojekyll`** na raiz publicada — sem ele, o Jekyll do Pages descarta as pastas com `_`, e
   `_framework/` é exatamente onde mora o Blazor.
2. **`404.html`** copiado do `index.html` — roteamento client-side em host estático depende disso;
   sem ele, `design.rvmit.com.br/componentes/button` devolve 404 ao recarregar a página.
3. **`<base href="/">`** — correto porque o site fica na raiz de um domínio próprio. Se um dia mudar
   para subcaminho, isto muda junto.

Como o domínio é próprio (não `usuario.github.io/repo`), não há prefixo de caminho a tratar.

### Dev — Rivendell

`rsync` do `wwwroot` publicado para `/var/www/design-dev`, com vhost Nginx servindo estático e
`try_files $uri $uri/ /index.html`. Mesmo padrão do deploy de landing do ecossistema, adaptado.

> **Desvio declarado:** o `deploy.yml@v1` do `RVM.Actions` pressupõe `docker compose` e container.
> Aqui não há container. O deploy de dev é, portanto, um workflow próprio deste repositório. Se um
> segundo projeto estático aparecer, o caminho certo é promover isto a um reusable
> `static-deploy.yml@v1` no `RVM.Actions` — registrado como candidato, não como dívida.

## CI/CD

| Workflow | Base | Gatilho | O que faz |
|---|---|---|---|
| `ci.yml` | caller de `RVM.Actions/ci.yml@v1` | PR e push | Build, teste, cobertura ≥ 80% |
| `e2e.yml` | caller de `RVM.Actions/e2e.yml@v1` | Após deploy de dev; nightly | Playwright + **axe** sobre `design.dev.rvmtech.com.br` |
| `publish-nuget.yml` | próprio | Push em `master` com mudança em `src/RVM.DesignSystem/**` | `dotnet pack` + push no BaGet |
| `deploy-development.yml` | próprio | Push em `dev` | Publica e faz rsync para a Rivendell |
| `deploy-pages.yml` | próprio | Push em `master` | Publica no GitHub Pages |

Os três workflows próprios usam `paths` para não republicar tudo a cada commit — a mesma disciplina
que o ADR-009 exige das landings, pela mesma razão.

⚠️ **Runner self-hosted:** o reusable do `RVM.Actions` roda em `runs-on: self-hosted`, e `Elfarrar`
é conta de usuário, não organização — **runner é por repositório**. Repositório novo tem zero, e o
job fica `queued` para sempre, sem erro. Registrar o runner no BagEnd é passo da fase 0
(`09-roadmap.md`).

## Distribuição do pacote

**BaGet interno no BagEnd**, o mesmo feed que serve o `RVM.Common.Security`. Feed e credenciais em
`Vault/05_References/baget-nuget.md`; segredo na fonte global.

- Versionamento por SemVer (`05` § Política de versão).
- Pré-release (`0.1.0-alpha.N`) a cada merge em `dev`; versão estável só a partir de `master`.
- Consumidor fixa versão exata. Ninguém é atualizado por acidente.
- `README` e símbolos embutidos no pacote; `PackageProjectUrl` aponta para `design.rvmit.com.br`.

## Segredos

| Segredo | Para que serve | Onde mora | Secret no GitHub |
|---|---|---|---|
| Chave SSH da Rivendell | rsync do site de dev | `RVM.Infra/docs/ACESSOS.md` | `DEV_VPS_SSH_KEY_B64`, `DEV_VPS_HOST`, `DEV_VPS_USER`, `DEV_VPS_PORT` |
| Caminho de deploy dev | destino do rsync | idem | `DEV_DEPLOY_PATH` |
| API key do BaGet | publicar o pacote | `ACESSOS-E-SENHAS.md` global | `BAGET_API_KEY` |
| URL do feed BaGet | restaurar e publicar | idem | `BAGET_FEED_URL` |

Não há `.env`, porque não há aplicação servidora — o projeto **não usa** `DEV_ENV_FILE` /
`PROD_ENV_FILE`. Publicação no Pages usa o `GITHUB_TOKEN` nativo, sem segredo adicional.

**Nenhuma credencial no repositório, nunca.** `docs/ACESSOS.md` é gitignored desde o primeiro commit.

## Monitoramento

Monitor no **Uptime-Kuma** (BagEnd) para `design.rvmit.com.br`, com **alerta de expiração de
certificado ligado** — regra do ecossistema (`padrao-rvm` §11), criado no mesmo passo do primeiro
deploy. O ambiente de dev não recebe monitor: queda de dev não acorda ninguém.

## Integrações externas

**Nenhuma.** Sem Payments, sem Fiscal, sem Evolution/WhatsApp, sem LLM, sem storage, sem terceiros.
O projeto não faz chamada de rede em runtime — nem para CDN de fonte ou de ícone, que são
auto-hospedados por decisão de privacidade (`02` § Segurança).

A única "integração" é a de saída: outros projetos RVM consomem o pacote do BaGet.

## Backup

Não há dado para backupear. O que precisa sobreviver é **o repositório** (código e histórico) e as
**versões publicadas no BaGet**. O primeiro está coberto pelo GitHub a partir do bootstrap; o
segundo entra no backup do BagEnd que já roda — verificar, no bootstrap, se o volume do BaGet está
na lista de origens do pull de backup.

> Vale a lição já registrada no ecossistema: **origem nova de backup precisa entrar na lista do pull
> no mesmo dia**, senão a falha fica invisível dos dois lados.
