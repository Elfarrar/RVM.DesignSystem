# 10 — Infra e ambientes

## Ambientes

| Ambiente | Domínio | Branch | Dados | Onde |
|---|---|---|---|---|
| Dev | `design.dev.rvmtech.com.br` | `dev` | não há | Rivendell — estático servido por Nginx, publicado por `rsync` |
| Demo | — | — | — | **não existe, de propósito**: sem dado, um segundo ambiente igual não prova nada |
| Prod | `design.rvmit.com.br` | `master` | não há | GitHub Pages |

- **Sem container, sem banco, sem Redis, sem rede `rvmtech`, sem `/health`.**
- ⚠️ **Os dois domínios existiram e foram apagados em 16/09/2026** — o CNAME `design` na zona
  `rvmit.com.br` e o registro A `design.dev` em `rvmtech.com.br`, junto com o vhost, o certificado e
  o conteúdo na Rivendell. **O bootstrap recria tudo do zero.** Backup das zonas de antes da remoção:
  `/opt/dns-<zona>-pre-design-20260916.json` na Rivendell.

## Verificação de deploy — por conteúdo, não por status

⚠️ **`curl -I` devolvendo 200 não prova nada aqui.** O `try_files` do Nginx e o `404.html` do Pages
devolvem a página inicial para qualquer caminho — então um build quebrado responde 200 alegremente.

A verificação procura **uma string da página** (`RVM Design System`). Vale para o passo de verify do
deploy e para o monitor no Uptime-Kuma, que usa tipo `keyword`, não código HTTP.

## CI/CD — cinco workflows PRÓPRIOS

⛔ **Nenhum é caller do `RVM.Actions@v1`, e isso é obrigatório, não preferência.** Repositório
público **não consegue** chamar reusable workflow de repositório privado, e o `RVM.Actions` é
privado. O sintoma não ajuda: o run termina em **0 s, zero jobs, `failure`, sem anotação**.

De quebra, repositório público tem runner do GitHub de graça — **este projeto não precisa de runner
self-hosted**, e não há nada a registrar no BagEnd.

| Workflow | Gatilho | Faz |
|---|---|---|
| `ci.yml` | push e PR | build, bUnit, cobertura ≥ 80%, zero warning |
| `e2e.yml` | após deploy de dev; noturno | Playwright + axe sobre o site publicado |
| `deploy-development.yml` | push em `dev` | publish → `rsync --delete` para a Rivendell → verify por conteúdo |
| `deploy-pages.yml` | push em `master` | publish → GitHub Pages → verify por conteúdo |
| `publish-nuget.yml` | push em `dev` (alpha) e tag `v*` (estável) | `pack` + push no BaGet |

### Armadilhas do publish no BaGet — já pagas uma vez

1. **A rede runner→BagEnd é intermitente.** O mesmo feed respondeu em 277 ms num run e deu timeout
   uma hora depois, com o BagEnd saudável. **Retry é obrigatório.**
2. **`--timeout` no `dotnet nuget push` não basta.** Ele governa a requisição de push; quem pendura é
   o fetch do índice do serviço. Sem um `timeout` **por fora**, uma tentativa pendurada consome o job
   inteiro e o laço de retry nunca roda — medido: 1908 s numa única tentativa.
3. **Feed `http` exige `--allow-insecure-connections`** — sem a flag o push não dá erro, **pendura**.
   Usar sempre `https://packages.rvmtech.com.br/v3/index.json`.
4. **`--skip-duplicate` sai com sucesso sem publicar nada.** Depois do push, **conferir a versão no
   feed**; o flat container fica em `/v3/package/<id>/index.json` (um `dirname`, não dois).

## Segredos

| Segredo | Onde mora | Secret no GitHub |
|---|---|---|
| SSH da Rivendell (rsync do dev) | `RVM.Infra/docs/ACESSOS.md` | `DEV_VPS_HOST`, `DEV_VPS_PORT`, `DEV_VPS_USER`, `DEV_VPS_SSH_KEY_B64`, `DEV_DEPLOY_PATH` |
| API key do BaGet | `ACESSOS-E-SENHAS.md` global | `BAGET_API_KEY` |
| URL do feed do BaGet | `ACESSOS-E-SENHAS.md` global | `BAGET_FEED_URL` |

**Não há `.env`** — não existe aplicação servidora, então nada de `DEV_ENV_FILE`/`PROD_ENV_FILE`.
O Pages usa o `GITHUB_TOKEN` nativo. `docs/ACESSOS.md` é gitignored desde o primeiro commit.

⛔ **`gh secret set --body '/caminho/unix'` no Git Bash grava valor ERRADO.** O MSYS converte o
argumento e `/var/www/design-dev` vira `C:/Program Files/Git/var/www/design-dev` dentro do secret; o
deploy quebra lá na frente com `tar (child): Cannot connect to C: resolve failed`. **Sempre por
stdin:** `printf '%s' '/var/www/design-dev' | gh secret set DEV_DEPLOY_PATH --repo <repo>`.

## Monitoramento

Um monitor no Uptime-Kuma por ambiente, **tipo `keyword`**, vinculado ao canal de e-mail (Resend,
`alertas@rvmtech.com.br`) — monitor sem canal é gráfico que ninguém olha na hora que importa.

## Integrações

Nenhuma. Sem Payments, sem Fiscal, sem WhatsApp, sem LLM, sem storage. As únicas dependências
externas são **o BaGet** (destino do pacote) e **o GitHub Pages** (host de prod).
