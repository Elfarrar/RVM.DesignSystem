---
id: DSGN-001
titulo: Bootstrap — repo, esqueleto .NET, CI/CD, DNS/SSL e deploy dev
repo: RVM.DesignSystem
tipo: chore
status: concluido
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
- [x] Producao: `https://design.rvmit.com.br` no ar, autorizado por ele em 08/09/2026
- [ ] Monitor no Uptime-Kuma — **bloqueado**, ver abaixo

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

## Identidade visual — pedido do Rafael em 07/09/2026

Depois do bootstrap fechado, ele pediu que o design system use **as cores do site dele**
(`rvmtech.com.br`, do RVM.Curriculo): roxo do Visual Studio + azul do VS Code.

Aplicado na casca do site de doc; registrado em `06-tokens-e-tematizacao.md` § A identidade da
marca, que e onde a onda 1 vai buscar as sementes do `RvmTheme.Rvm`.

Duas coisas medidas, nao assumidas:

- **As sementes sao `#641974` / `#006DBD`, os valores RENDERIZADOS**, nao os nominais `#68217A` /
  `#007ACC` do `CLAUDE.md` do RVM.Curriculo. O azul nominal da **3.90** de contraste contra o fundo
  e reprovaria no portao AA; o renderizado da 4.62. O site esta certo — o nome escrito la e que
  esta desatualizado.
- **O modo escuro nao existe no site** (`color-scheme: light`). Derivei mantendo matiz e croma,
  invertendo luminosidade, e verifiquei: os cinco pares passam. ✅ **Aprovado por ele em 08/09/2026**,
  depois de ver a marca no escuro no site de dev.

Tres tokens do site (`text-subtle` 3.42, `accent-warm` 4.42, `border-strong` 1.67) nao sobrevivem
como token semantico da biblioteca; o porque e o ajuste estao no `06`. Nao e defeito do site: la
eles vivem em texto grande e divisoria decorativa, onde 3.0 basta.

## Producao — subiu em 08/09/2026

`https://design.rvmit.com.br` no ar. Verificado por conteudo, nao por status code:
`.nojekyll`, `404.html`, `CNAME`, o bundle de CSS isolado e o `rvm-tokens.css` respondendo 200;
`http` redirecionando 301 para `https`; certificado do Let's Encrypt emitido pelo Pages e
`Enforce HTTPS` ligado; zero erro de console.

### O primeiro deploy terminou vermelho, e o site estava certo

O `actions/deploy-pages@v4` passou; falhou so o meu passo de `Verify`. Duas suposicoes minhas:

1. **O arquivo `CNAME` no artefato NAO registra o dominio** quando o deploy e via
   `actions/deploy-pages`. Isso vale para publicacao por branch; aqui o dominio e configuracao
   do repositorio, e so entra por settings ou API.
2. **Ovo-e-galinha:** o dominio so pode ser registrado DEPOIS que existe um deployment. Entao o
   primeiro `Verify` falha por construcao, com o site publicado e funcionando.

Corrigido na `DSGN-004`: a mensagem de erro agora nomeia esse caso e da os comandos, e a janela
subiu para 10x30s — o certificado leva minutos para propagar entre as bordas do Pages, e nesse
intervalo o mesmo endereco alterna entre 200 e erro de TLS conforme o no que atende.

### Duas coisas consertadas antes de subir

- **A borda em volta do titulo** (o Rafael reclamou): era o anel de foco do `<FocusOnNavigate>`.
  Removido so onde nao orienta ninguem (`tabindex="-1"`, fora da ordem de tabulacao); o anuncio
  para leitor de tela e o foco de teclado nos links seguem intactos.
- **404 do bundle de CSS isolado**: nao existia porque nao havia nenhum `.razor.css`. Na dev o
  defeito era invisivel, porque o `try_files` do Nginx devolvia `index.html` com 200 no lugar do
  CSS. No Pages apareceria.

### O que fica pendente de producao

⏳ **Monitor no Uptime-Kuma.** Nao criei de proposito: o Uptime-Kuma tem **17 monitores e ZERO
canais de notificacao** — nenhum deles avisa ninguem hoje, incluindo as producoes de ERPAgro,
Gypsy e Payments. Criar o 18o monitor mudo seria teatro. Espera o Rafael escolher o destino do
alerta (WhatsApp pela Evolution, e-mail ou Telegram).

⚠️ **Rota profunda devolve HTTP 404 no Pages** — o `404.html` renderiza a pagina certa e a URL e
preservada, entao para o usuario funciona; mas o status e 404. Diferente do Nginx da dev, que
devolve 200 via `try_files`. Sem impacto hoje (nao ha pagina de componente ainda); vira questao
de SEO na onda 1, quando as paginas existirem.

⚠️ **A promocao publicou o pacote estavel `0.1.0`**, antes do previsto: o `09-roadmap` reservava
o `0.1.0` para o fim da onda 1. Consequencia: a onda 1 fecha em `0.2.0`. Sem estrago, mas o
numero foi consumido mais cedo.

## Pendencias que continuam abertas

As seis do `09-roadmap` § Pendencias seguem abertas, menos a **5** (visibilidade/licenca), que o
Rafael respondeu em 07/09/2026: **publico, MIT**. A **6** (pacote x `ProjectReference`) esta
implementada nos dois modos, com `UseLocalDesignSystem=true` por padrao — o pacote ainda nao
existe no feed. Vira `false` quando o `0.1.0-alpha` for publicado.
