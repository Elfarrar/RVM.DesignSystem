---
id: DSGN-007
titulo: 1.0.0 e producao em design.rvmit.com.br
repo: RVM.DesignSystem
tipo: release
status: em andamento
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

1. [ ] Card, Kanban, `CLAUDE.md` com o estado real e prefixo das alphas de `dev` para `1.1.0`
       (depois do `1.0.0`, `0.1.0-alpha.N` ordenaria ANTES do estavel) — PR em `dev`
2. [ ] Habilitar o Pages por workflow no repositorio
3. [ ] DNS: CNAME `design` -> `elfarrar.github.io.` em `rvmit.com.br` (API Hostinger, sem sobrescrever a zona)
4. [ ] PR `dev` -> `master` e merge: dispara o `deploy-pages.yml`. **O primeiro `Verify` falha por
       construcao** (o dominio so registra depois do primeiro deployment)
5. [ ] Registrar o dominio (`gh api -X PUT .../pages -f cname=design.rvmit.com.br`), esperar o
       certificado, `https_enforced=true`, rodar de novo o workflow e ver o `Verify` verde
6. [ ] Tag `v1.0.0` no commit do `master` -> `publish-nuget.yml` -> conferir `1.0.0` NO FEED
7. [ ] Retomar o monitor 25 do Kuma (keyword `RVM Design System`, canal de e-mail)

## Verifica

- `curl -fsS https://design.rvmit.com.br/` com `text/html` e a string `RVM Design System`, e uma rota
  profunda (`/componentes/time-picker`) tambem
- `1.0.0` em `packages.rvmtech.com.br/v3/package/rvm.designsystem/index.json`
- Monitor 25 verde

## Decisoes

| Decisao | Por que |
|---|---|
