---
adr: 011
titulo: ci.yml proprio em vez de caller do RVM.Actions@v1
status: aceito
data: 2026-09-07
projeto: RVM.DesignSystem
---

# ADR-011 — CI proprio, porque o repositorio e publico

## Contexto

O `padrao-rvm` §5 manda: callers finos consumindo `Elfarrar/RVM.Actions@v1`, **nao recriar
workflow do zero**. Todo projeto do ecossistema segue isso.

Em 07/09/2026 o Rafael decidiu que o `RVM.DesignSystem` nasce **publico, sob MIT**
(pendencia 5 do `09-roadmap.md`).

## Problema encontrado

Repositorio **publico** nao consegue chamar reusable workflow de repositorio **privado**, e o
`RVM.Actions` e privado. O sintoma nao ajuda: o run termina em **0s, com zero jobs**, conclusao
`failure` e nenhuma anotacao util — a UI so diz "this run likely failed because of a workflow
file issue".

Verificado empiricamente no mesmo dia, alternando a visibilidade deste repositorio:

| Visibilidade | Resultado do caller |
|---|---|
| `private` | job entra em `queued` (comportamento normal) |
| `public`  | run falha em 0s, sem jobs |

Nenhum repositorio publico do ecossistema tinha exercitado isso antes — o `RVM.Portfolio`, o
unico publico, nao usa o `RVM.Actions`.

## Alternativas consideradas

1. **Voltar o repositorio para privado** — desfaz a decisao do Rafael, exige registrar runner
   self-hosted e leva o Pages a exigir GitHub Pro.
2. **Tornar o `RVM.Actions` publico** — expoe a topologia de CI/infra do ecossistema (hostnames,
   caminhos de deploy, nomes de container). Nao vaza segredo, mas vaza mapa, e na pratica nao
   se desfaz.
3. **`ci.yml` proprio** — escolhida.

## Decisao

O `ci.yml` deste repositorio e **proprio**, roda em `ubuntu-latest` e faz restore, build em
`Release`, teste com cobertura e o portao de 80%.

## Consequencias

**Boa, e grande:** repositorio publico tem runner do GitHub de graca, entao **este projeto nao
precisa de runner self-hosted no BagEnd**. Some a armadilha registrada no `padrao-rvm` §5 — o
job que fica `queued` para sempre, sem erro nenhum — que era o passo mais fragil da fase 0.

**Ruim:** o build deste repositorio deixa de herdar melhorias do `RVM.Actions`. Quando o
reusable mudar, alguem tem que decidir se a mudanca vale aqui tambem.

**Diferenca de comportamento, deliberada:** o portao de cobertura de 80% aqui **reprova o
build**. O reusable so imprime o numero no summary.

Se um dia a visibilidade do `RVM.Actions` mudar, este ADR e o lugar de reabrir a decisao.
