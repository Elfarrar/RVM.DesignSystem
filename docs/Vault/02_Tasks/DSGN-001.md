---
id: DSGN-001
titulo: Bootstrap — repo, esqueleto .NET, CI/CD, DNS/SSL e deploy dev
repo: RVM.DesignSystem
tipo: chore
status: em-revisao
criada: 2026-09-07
---

# DSGN-001 — Bootstrap (fase 0)

Fase 0 do `09-roadmap.md`, executada pela skill `bootstrap-projeto`. Vai do repositorio vazio
ate o site publicado em `design.dev.rvmtech.com.br`. **Producao (`master` + Pages) fica fora**:
o Rafael autorizou o bootstrap "ate dev" em 07/09/2026.

**Motivo de vir primeiro:** ate o commit inicial a spec `01`–`11` existia num disco so, sem backup.

## Criterio de saida

`curl` em `https://design.dev.rvmtech.com.br` devolvendo o conteudo do site publicado pelo
workflow — nao so HTTP 200, que host estatico com fallback devolve para qualquer caminho.

## Escopo

- [x] `git init -b master`, `.gitignore`, `.gitattributes` (`eol=lf` em yml/sh), `LICENSE` MIT
- [x] Repo `Elfarrar/RVM.DesignSystem` **publico**, branches `master` e `dev`
- [x] Esqueleto .NET 10: RCL + Docs (WASM) + Tests (bUnit) + E2E (Playwright + axe)
- [x] `global.json` (10.0.201 + `rollForward`), central package management, `coverlet.runsettings`
- [x] `ci.yml` verde, com portao de cobertura de 80% que **reprova**
- [x] `deploy-development.yml`, `deploy-pages.yml`, `publish-nuget.yml`, `e2e.yml`
- [x] 7 secrets no repositorio
- [x] DNS `design.dev` -> Rivendell; certificado LE `design-dev`; vhost estatico
- [x] Deploy em `design.dev.rvmtech.com.br`
- [ ] Producao (Pages + DNS `design.rvmit.com.br` + monitor no Uptime-Kuma) — **aguarda o Rafael**

## Decisoes

### 1. Sem branch `demo`

O ecossistema cria `master`/`dev`/`demo`. Aqui `demo` nao existe porque **nao ha ambiente demo**
(`10-infra` § Ambientes): demo serve para dado curado de cliente, e este projeto nao tem dado.
Branch sem ambiente correspondente apodrece atras de `dev`.

### 2. Sem Docker, sem `/health`

Blocos 3 e parte do 7 da skill nao se aplicam: o artefato e um pacote NuGet e um site estatico.
A verificacao de deploy e por **conteudo** da pagina, nao por endpoint de saude.

### 3. `ci.yml` proprio, nao caller do `RVM.Actions@v1` — ver [[ADR-011]]

Repositorio publico nao chama reusable workflow de repositorio privado, e o `RVM.Actions` e
privado. Descoberto na pratica: o run morria em 0s, com zero jobs e sem mensagem util.

### 4. Este projeto NAO precisa de runner self-hosted

Consequencia boa da decisao 3: repo publico tem runner do GitHub de graca, e todos os cinco
workflows rodam em `ubuntu-latest`. **A armadilha do job `queued` para sempre nao existe aqui** —
o passo mais fragil da fase 0 saiu do caminho. Nada a registrar no BagEnd.

### 5. `e2e.yml` e proprio, e o `10-infra` estava errado

O `10-infra` lista o `e2e.yml` como caller do `RVM.Actions@v1`. Nao da: o reusable sobe uma stack
local com `docker compose`, espera `/health` de uma API instrumentada e roda Playwright por npm.
Nenhuma das tres premissas vale aqui. O `10-infra` foi corrigido nesta task.

### 6. O E2E mora em `test/playwright/RVM.DesignSystem.E2E`

Nao e capricho de nome: o `ci.yml` do ecossistema roda todo `test/**/*.csproj` e pula so os
caminhos que contem `playwright`. O E2E exige o site publicado no ar. Se alguem "arrumar" o
caminho para `test/RVM.DesignSystem.E2E`, o CI passa a tentar rodar o E2E sem site.

### 7. Bootstrap do Blazor traz Bootstrap — removido

O template `blazorwasm` gera o site vestido em Bootstrap CSS. Manter seria contradizer o
`03-arquitetura` ("sem framework CSS") logo no repositorio que existe para ser o design system.
Removido no esqueleto, com o CSS minimo escrito a mao e um comentario dizendo por que.

## Armadilhas que so apareceram rodando

Nenhuma das tres estava na spec nem na skill. Todas foram registradas fora do repo tambem
(`padrao-rvm` §5 e `ACESSOS-E-SENHAS.md`), porque nao sao especificas deste projeto.

1. **`gh secret set --body '/caminho/unix'` no Git Bash grava o valor errado.** O MSYS converte o
   argumento e `/var/www/design-dev` vira `C:/Program Files/Git/var/www/design-dev` **dentro do
   secret**. O deploy quebrou com `tar (child): Cannot connect to C: resolve failed` — um erro que
   nao aponta para a causa. **Gravar por stdin.** Ja tinha mordido o GestorDeObras em 30/08/2026,
   que deixou `/root/C:/Program Files/Git/srv/...` na Rivendell (ainda la; nao e meu, nao removi).
2. **`dotnet nuget push` contra `http://187.77.48.215:5555` trava sem erro** no runner do GitHub —
   5 minutos `in_progress`, nenhum log, ate ser cancelado. Pelo dominio
   `https://packages.rvmtech.com.br` resolve em segundos.
3. **`workflow_run`, `schedule` e `workflow_dispatch` so disparam do branch padrao.** Como o
   `master` ainda nao tem os workflows, o `e2e.yml` fica inerte ate a promocao. Rodado a mao neste
   bootstrap: 2/2 verde, **axe sem violacao seria**.

## Fora do escopo, mas encontrado

⚠️ **O volume do BaGet nao esta em backup nenhum.** O `10-infra` § Backup mandava verificar isso no
bootstrap; verificado, e o resultado e negativo. O `/opt/puxar-backups-remotos.sh` puxa de
MinasTirith e Rivendell **para** o BagEnd — o BagEnd e o destino, e nada copia os volumes dele para
fora. Perder o BagEnd perde o feed inteiro (`baget_baget-data`, 148K em 07/09/2026).

Nao mexi: e servico compartilhado (serve tambem o `RVM.Common`) e alterar o backup do ecossistema
nao e efeito colateral de bootstrap de design system. **Decisao do Rafael.** Uma saida legitima e
declarar que pacote e reconstruivel do git e nao precisa de backup — mas isso e escolha, nao
esquecimento, e hoje nao esta escrita em lugar nenhum.

## Pendencias que continuam abertas

As seis do `09-roadmap` § Pendencias seguem abertas, menos a **5** (visibilidade/licenca), que o
Rafael respondeu em 07/09/2026: **publico, MIT**. A **6** (pacote x `ProjectReference`) esta
implementada nos dois modos, com `UseLocalDesignSystem=true` por padrao — o pacote ainda nao
existe no feed. Vira `false` quando o `0.1.0-alpha` for publicado.
