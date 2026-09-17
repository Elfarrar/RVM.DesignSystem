---
id: DSGN-007
titulo: 1.0.0 e producao em design.rvmit.com.br
repo: RVM.DesignSystem
tipo: release
status: concluido
criada: 2026-09-17
---

# DSGN-007 — 1.0.0 e producao

As quatro ondas fecharam (`DSGN-003` a `DSGN-006`, aprovadas). Criterio do `1.0.0` cumprido: o contrato
da API congela. **Sinal verde explicito do Rafael em 17/09/2026** ("Sim, 1.0.0 e producao"), dado
depois de a onda 4 ser aprovada — aprovar a onda NAO autorizava a promocao; esta pergunta sim.

## Estado de partida (conferido em 17/09)

- `master` so tem a spec; codigo e workflows vivem em `dev`.
- GitHub Pages **nao habilitado** no repositorio (`GET /pages` = 404).
- `design.rvmit.com.br` **nao existe** na zona (Hostinger; NXDOMAIN no 8.8.8.8).
- Feed BaGet: so `0.1.0-alpha.*` — nenhuma versao estavel, nada da encarnacao apagada.
- Credito CC BY do NEATLAB presente no README, no rodape do site e na descricao do pacote.
- Monitor 25 do Kuma (`DesignSystem - prod`) pausado desde 16/09.

## Passos

1. [x] Card, Kanban, `CLAUDE.md` com o estado real e prefixo das alphas de `dev` para `1.1.0`
       (depois do `1.0.0`, `0.1.0-alpha.N` ordenaria ANTES do estavel) — PR em `dev`
2. [x] Habilitar o Pages por workflow no repositorio
3. [x] DNS: CNAME `design` -> `elfarrar.github.io.` em `rvmit.com.br` (API Hostinger, sem sobrescrever a zona)
4. [x] PR `dev` -> `master` e merge: dispara o `deploy-pages.yml`. **O primeiro `Verify` falha por
       construcao** (o dominio so registra depois do primeiro deployment)
5. [x] Registrar o dominio (`gh api -X PUT .../pages -f cname=design.rvmit.com.br`), esperar o
       certificado, `https_enforced=true`, rodar de novo o workflow e ver o `Verify` verde
6. [x] Tag `v1.0.0` no commit do `master` -> `publish-nuget.yml` -> conferir `1.0.0` NO FEED
7. [x] Retomar o monitor 25 do Kuma (keyword `RVM Design System`, canal de e-mail)

## Verifica

- `curl -fsS https://design.rvmit.com.br/` com `text/html` e a string `RVM Design System`, e uma rota
  profunda (`/componentes/time-picker`) tambem
- `1.0.0` em `packages.rvmtech.com.br/v3/package/rvm.designsystem/index.json`
- Monitor 25 verde

## Decisoes

| Decisao | Por que |
|---|---|
| **Pages por workflow, dominio pela API do repositorio** | Como previsto: o 1o `Verificar` falhou por construcao (run do merge do PR #33); com o `cname` registrado o certificado ja saiu `approved` na primeira consulta, `https_enforced=true`, e o run 35220676198 ficou verde |
| **CNAME `design` na Hostinger com `overwrite:false`** | A zona serve ERP, Gypsy e Payments; o registro foi somado sem tocar os outros (8 -> 9 registros). A API deu *connection reset* intermitente na leitura — a escrita entrou; conferido depois |
| **`v1.0.0` na ponta do `master`** (`ae621ae`, merge do PR #33) | `publish-nuget.yml` publicou; `1.0.0` conferido no flat container do BaGet |
| **Monitor 25 retomado pela API do Kuma** (`uptime-kuma-api`) | keyword `RVM Design System` em `https://design.rvmit.com.br`, canal de e-mail, aviso de expiracao; primeiro beat UP. O script quebrou so no `print` final (a lista de notificacoes veio como lista), depois de retomar |
| **Verificacao em producao** | `text/html` com a string; rota profunda devolve o `404.html` (HTTP 404, conteudo do app) e abre no navegador; smoke E2E `@smoke` 40/40 contra `design.rvmit.com.br`. `curl` do Windows falha por revogacao offline do certificado novo (schannel) — problema da maquina local, nao do site |
