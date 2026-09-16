---
id: DSGN-001
titulo: Bootstrap — repo publico, esqueleto RCL + Docs WASM, 5 workflows proprios, borda de dev
repo: RVM.DesignSystem
tipo: chore
status: em andamento
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
