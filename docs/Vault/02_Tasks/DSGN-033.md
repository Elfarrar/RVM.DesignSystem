---
id: DSGN-033
titulo: Timeout POR FORA no push do BaGet, e a 1.1.1 no feed
repo: RVM.DesignSystem
tipo: chore
status: concluido
criada: 2026-09-09
atualizada: 2026-09-09
---

# DSGN-033 — O card proprio que o DSGN-018 pediu

## Descricao

Pedido do Rafael em 09/09/2026: *"publica o pacote no baget"*.

Nao da para publicar sem consertar antes o que trava — e o proprio `CLAUDE.md` diz isso:
**"nao republicar movendo a tag sem decidir o conserto antes"**.

Estado do feed em 09/09, consultado direto:

```
{"versions":[... "1.0.0-alpha.17","1.0.0","1.1.0-alpha.19","1.1.0-alpha.21","1.1.0-alpha.22"]}
```

A ultima **estavel** no feed e a `1.0.0`. A `1.1.0` tem tag (`ad48171`) e nao tem pacote: duas
execucoes do passo "Push no BaGet" ficaram penduradas (uma medida em **1908s**) e o
`--timeout 120` **nao as limitou**, porque a flag governa a requisicao de push e nao o fetch de
indice do cliente (`DSGN-018` § O que falta decidir).

O `DSGN-018` deixou tres saidas e escolheu a primeira, dizendo em quantas palavras que ela
"fica para um card proprio". Este e o card proprio.

## Plano

1. **`timeout 180` do coreutils por fora da tentativa**, no `publish-nuget.yml`. Limita o que a
   flag nao alcanca; o `--timeout 120` interno fica, porque continua sendo o limite certo para
   a requisicao de push em si. Sao dois limites de coisas diferentes, nao redundancia.
   O laco de 3 tentativas passa a valer de verdade: pior caso ~9 min em vez de 32.

2. **`VersionPrefix` para `1.1.2`**. Ele aponta para a PROXIMA versao, nunca para a ultima
   publicada (comentario no `csproj`): parado em `1.1.0`, os alphas de `dev` sairiam
   `1.1.0-alpha.N` e o NuGet os ordena **abaixo** da `1.1.0` — codigo mais novo com numero
   menor.

3. **Publicar a `1.1.1`**, e nao a `1.1.0`.

   ⚠️ **Por que pular a `1.1.0`**: a tag `v1.1.0` prende um commit (`ad48171`) que **nao tem** o
   `DSGN-032`. Republicar aquele numero com o codigo de hoje e exatamente o "alvo movel" que a
   decisao de 08/09 (`DSGN-009`) proibiu, e reexecutar o workflow na tag antiga usaria o YAML
   antigo — sem o conserto do passo 1, travaria de novo. A `1.1.0` fica sendo um numero que
   existe no git e nunca existiu no feed; como ninguem chegou a consumi-la, isso nao quebra
   ninguem.

   `1.1.1` e patch porque o unico commit de biblioteca desde a `1.1.0` e o `DSGN-032` (titulo do
   calendario) — o `DSGN-030` e o `DSGN-031` foram **so o site**.

4. A tag sai do `master`, depois da promocao — que depende do sinal verde do Rafael.

## Validacao

- [x] `1.1.2-alpha.23` publicada no merge em `dev` — run inteiro em **27 segundos**.
- [x] `1.1.1` no feed, do commit `7e6cc03`, conferida pelo passo do workflow e por `curl` direto.
- [x] O passo "Push no BaGet" respondeu `Your package was pushed` em ~2s; o run da tag levou
      27s de ponta a ponta, contra os 1908s de uma unica tentativa na `v1.1.0`.

⚠️ **O que isto NAO provou**: a rede estava boa hoje. O `timeout 180` so mostra servico quando
ela travar de novo — o que ficou provado e que o limite existe e que o caminho feliz nao
regrediu. A prova real vem no dia ruim.

## Versao

Publica a **`1.1.1`** e deixa o `VersionPrefix` em `1.1.2` para os proximos alphas.
