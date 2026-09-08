---
id: DSGN-018
titulo: Timeout por tentativa no push do BaGet
repo: RVM.DesignSystem
tipo: fix
status: concluido
criada: 2026-09-08
---

# DSGN-018 — Retry sem timeout nao e retry

## O que aconteceu

Publicando a `0.2.0` (tag `v0.2.0`, 08/09/2026), o passo **Push no BaGet** ficou **25 minutos**
numa unica tentativa. Cancelado a mao, o re-run do **mesmo commit** publicou em **40 segundos**.

Durante o travamento o BagEnd estava saudavel: `https://packages.rvmtech.com.br/v3/index.json`
respondendo em ~200ms daqui, o tempo todo.

## A causa

A intermitencia da rede entre o runner do GitHub e o BagEnd **ja estava registrada**, e o
workflow **ja tinha** um laco de 3 tentativas por causa dela. O que faltava era o timeout **por
tentativa**:

```bash
# antes
dotnet nuget push "$pkg" --source "$FEED" --api-key "$KEY" --skip-duplicate
```

O padrao do `dotnet nuget push` e alto o bastante para uma tentativa pendurada consumir o job
inteiro. As outras duas nunca chegam a rodar.

> **Retry sem timeout por tentativa nao e retry: e uma espera longa com aparencia de retry.**

E o pior tipo de falso conforto — olhando o workflow, da a impressao de que a intermitencia esta
tratada.

## O conserto

```bash
dotnet nuget push "$pkg" --source "$FEED" --api-key "$KEY" --skip-duplicate --timeout 120
```

120s e folgado para um pacote de 96 KB num feed que responde em 200ms. Com ele, as tres
tentativas cabem em ~7 minutos e **uma delas efetivamente acontece**.

## Onde mais isso vale

⚠️ **Todo retry de rede do ecossistema merece a mesma pergunta**: cada tentativa tem prazo para
desistir? Se nao tiver, o laco e decorativo. Vale para o `curl` de verificacao de deploy, para o
`docker pull` e para qualquer `dotnet nuget push` copiado deste arquivo.

## Verificado

`0.2.0` no feed, confirmada pelo passo "Confere que a versao chegou no feed" e pelo
`curl` direto:

```
{"versions":[... "0.1.0", "0.2.0-alpha.12", "0.2.0"]}
```
