---
id: DSGN-018
titulo: Timeout por tentativa no push do BaGet — INCOMPLETO, ver medicao de 08/09
repo: RVM.DesignSystem
tipo: fix
status: reaberto
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

## ⚠️ CORRECAO — a medicao desmente o conserto (08/09/2026, mesma noite)

**O `--timeout 120` NAO limita o travamento.** Isto esta medido, nao suposto.

Publicando a `v1.1.0`, com a flag comprovadamente presente no commit da tag
(`git show 7013b46:.github/workflows/publish-nuget.yml`), o passo "Push no BaGet" levou
**1908 segundos — 31,8 minutos** e falhou. O orcamento esperado era ~420s: 3 tentativas de 120s
mais dois `sleep 30`.

Cada tentativa consumiu **~616 segundos**, cinco vezes o que a flag pede.

### O que este card afirmava, e que estava errado

> "O que o timeout garante e outra coisa, e e ela que importa: **uma tentativa ruim agora custa
> 2 minutos em vez do job inteiro**, e as outras duas do laco chegam a rodar."

A segunda metade e verdade — o laco rodou as tres tentativas e falhou de forma limpa, em vez de
pendurar para sempre. **A primeira metade e falsa.** Uma tentativa ruim custou 10 minutos, nao 2.

A publicacao da `1.0.0` em 21 segundos, citada aqui como validacao, **nao validava nada**: ela
so mostra que a rede estava boa naquele run. Foi exatamente a leitura contra a qual este mesmo
card advertia — e eu a fiz assim mesmo.

### A hipotese, marcada como hipotese

`--timeout` do `dotnet nuget push` provavelmente governa a requisicao de **push**, mas o cliente
busca antes o **indice do servico** (`/v3/index.json`) para descobrir o endpoint de publicacao, e
esse fetch tem timeout proprio que a flag nao alcanca — o que casa com o erro historico
`Unable to load the service index ... timed out after 100000ms`.

**Nao esta confirmado.** O que esta confirmado e a duracao.

### O que ficou descartado na investigacao

- **BagEnd fora do ar**: nao. O feed responde em 150ms e o endpoint de publicacao devolve
  **401 em 100ms** num `PUT` sem chave — resposta correta e rapida.
- **Indice anunciando endpoint errado**: nao. Ele aponta `https://packages.rvmtech.com.br/api/v2/package`,
  mesmo host, mesmo esquema.
- **Tamanho do pacote**: nao. O `1.1.0-alpha.19`, do mesmo codigo, publicou dez minutos antes.

Sobra o que ja estava escrito: **a rede entre o runner do GitHub e o BagEnd**. Desta vez de
forma persistente, e nao intermitente — dois runs seguidos travaram.

### O que falta decidir

O conserto real nao e obvio e nao deve ser escolhido as pressas:

1. Envolver a tentativa num `timeout 180 dotnet nuget push ...` do coreutils — limita de fora,
   independente do que a flag alcanca. Simples e eficaz.
2. Publicar de um runner com rota melhor ate o BagEnd (self-hosted na propria rede).
3. Aceitar e re-executar quando falhar.

A (1) e barata e resolve o sintoma medido. Fica para um card proprio.

## Onde mais isso vale

⚠️ **Todo retry de rede do ecossistema merece DUAS perguntas**, e a segunda so ficou clara
depois da medicao acima:

1. Cada tentativa tem prazo para desistir? Se nao tiver, o laco e decorativo.
2. **O prazo que voce configurou realmente alcanca o que trava?** Uma flag de timeout que
   governa so parte da operacao da a impressao de limite sem entregar limite — e essa e a
   versao pior, porque parece resolvido. Vale para o `curl` de verificacao de deploy, para o
`docker pull` e para qualquer `dotnet nuget push` copiado deste arquivo.

## Verificado

`0.2.0` no feed, confirmada pelo passo "Confere que a versao chegou no feed" e pelo
`curl` direto:

```
{"versions":[... "0.1.0", "0.2.0-alpha.12", "0.2.0"]}
```
