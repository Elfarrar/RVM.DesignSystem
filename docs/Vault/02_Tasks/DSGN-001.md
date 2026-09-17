---
id: DSGN-001
titulo: Bootstrap — repo publico, esqueleto RCL + Docs WASM, 5 workflows proprios, borda de dev
repo: RVM.DesignSystem
tipo: chore
status: concluido ate dev (monitor incluso) — producao espera autorizacao do Rafael
criada: 2026-09-16
---

# DSGN-001 — Bootstrap do RVM.DesignSystem (recriado)

Segue a skill `bootstrap-projeto`, com os desvios que a spec declara (sem Docker, sem banco,
sem demo, CI proprio). Fase 0 do `09-roadmap.md`. Os tokens e os 9 primeiros componentes sao a
`DSGN-002`.

## Escopo desta rodada

Ate **dev no ar**. Producao (GitHub Pages + `design.rvmit.com.br`) espera sinal verde explicito
do Rafael — decidido por ele em 16/09/2026, no inicio desta task.

## Feito

- [x] Repo **publico** `Elfarrar/RVM.DesignSystem` (MIT), branches `master` e `dev`
- [x] `LICENSE` MIT + nota CC BY ao NEATLAB; credito no `README.md` desde o primeiro commit
- [x] `referencia-neatlab/` (536 arquivos, 101 MB) versionada — e contra ela que a fidelidade e conferida
- [x] `.gitattributes` com `eol=lf` em yml/sh/json/cs/razor/css
- [x] Esqueleto `.slnx`: RCL, site WASM, bUnit e Playwright + axe; `global.json` (`10.0.100`,
      `rollForward: latestFeature`), `Directory.Build.props`, CPM e `coverlet.runsettings`
- [x] `dotnet build -c Release`: **0 erro, 0 warning**; `dotnet test`: 2/2
- [x] Cinco workflows proprios em `ubuntu-latest`, com guarda das tres armadilhas de WASM estatico
- [x] Sete secrets no GitHub, todos por **stdin** (`DEV_VPS_*`, `DEV_DEPLOY_PATH`, `BAGET_*`)
- [x] DNS: A `design.dev` -> `185.137.92.224` na zona `rvmtech.com.br`
      (backup em `/opt/dns-rvmtech.com.br-pre-design-dev-20260916.json` na Rivendell)
- [x] Certificado `design-dev` (Let's Encrypt, webroot) e vhost `49-design.dev.conf`
- [x] Bind-mount orfao de `/var/www/design-dev` reparado por `docker restart rvmtech-nginx`
      (autorizado pelo Rafael); os 12 vhosts de dev/demo conferidos depois
- [x] **Dev no ar** — `https://design.dev.rvmtech.com.br` com a string `RVM Design System`,
      rota profunda abrindo pelo `404`/`try_files` e o WASM subindo de verdade (screenshot)
- [x] Pacote **`0.1.0-alpha.1`** confirmado NO FEED do BaGet (nao so no log do push)
- [x] E2E verde contra o site publicado: 4/4, axe sem violacao seria

## Armadilhas encontradas nesta task

1. **`types { application/wasm wasm; }` dentro do `server` SUBSTITUI o mapa inteiro** herdado do
   `mime.types` em vez de acrescentar. O `index.html` passou a sair como `application/octet-stream`
   e o navegador **baixava** a pagina em vez de abrir — e o `curl` continuava devolvendo 200 com a
   string certa, entao a verificacao por conteudo **nao pegou**. Quem pegou foi o screenshot. O
   `mime.types` do nginx 1.29 ja traz `application/wasm`: o bloco era desnecessario.
2. **`workflow_run` e `schedule` so disparam do branch padrao.** Com os workflows so em `dev`, o
   E2E depois do deploy e o noturno ficam inertes ate a promocao para `master`.
3. **A guarda do E2E se mordia**: o proprio `playwright.config.ts` explica por que `waitForTimeout`
   e proibido, e o `grep` achava a palavra no comentario. Agora ela le so as linhas de codigo.

- [x] Monitor **28 `DesignSystem - dev`** no Uptime-Kuma: tipo `keyword` procurando
      `RVM Design System`, canal **E-mail (Resend)**, aviso de expiracao de certificado, 60 s com
      1 nova tentativa. Primeiro heartbeat verde: `[Up] 200 - OK, keyword is found`, 160 ms
- [x] Monitor **25 `DesignSystem - prod` pausado** — apontava para o ambiente apagado em 16/09
      (`getaddrinfo ENOTFOUND design.rvmit.com.br`). ⚠️ **Retomar junto com a subida de prod.**

## Pendente

- [ ] Producao: GitHub Pages + CNAME `design` em `rvmit.com.br` — so com sinal verde do Rafael
- [ ] Fora desta task, visto no painel do Kuma: **`Backup remoto - GestorDeObras` em 0%**

## Desvios da skill, e por que

| Passo da skill | Aqui | Motivo |
|---|---|---|
| Branch `demo` | nao existe | `10-infra`: sem dado, um segundo ambiente igual nao prova nada |
| Docker (passo 3) | nao existe | o artefato e uma RCL + site estatico; nao ha aplicacao servidora |
| Caller do `RVM.Actions` (passo 4) | 5 workflows proprios em `ubuntu-latest` | ADR-002: repo publico nao chama reusable de repo privado (run morre em 0 s) |
| Runner self-hosted | nao precisa | repo publico tem runner do GitHub de graca |
| `.env` por ambiente (passo 5) | nao existe | nao ha aplicacao servidora |
| `/health` (passos 2 e 7) | verificacao por **conteudo** | `try_files` e `404.html` devolvem 200 com build quebrado; procurar `RVM Design System` |
| Banco no backup (passo 7.5) | nao se aplica | nao ha banco |

## Verifica

1. `dotnet build -c Release` 0 erro e 0 warning → `dotnet test` verde
2. CI verde no PR
3. `https://design.dev.rvmtech.com.br` responde com a string `RVM Design System`
4. Pacote `0.1.0-alpha.<run>` visivel no feed do BaGet (conferido no feed, nao no log do push)
5. Monitor `DesignSystem - dev` no Uptime-Kuma, tipo `keyword`, vinculado ao canal de e-mail
