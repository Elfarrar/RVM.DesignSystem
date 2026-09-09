---
id: DSGN-030
titulo: O site WASM sem os dados de pt-BR — moeda sai "BRL96.90" em navegador estrangeiro
repo: RVM.DesignSystem
tipo: bug
status: concluido
criada: 2026-09-09
atualizada: 2026-09-09
---

# DSGN-030 — Cultura explicita nao basta no WASM

## Descricao

O E2E noturno esta vermelho desde **08/09 19:18**:

```
System.FormatException : The input string 'BRL96.90' was not in a correct format
  SiteSmokeTests.cs:416, LerColunaDeTotais
```

**Nao e o teste que esta errado.** O Blazor WASM publica o ICU em tres pedacos —
`icudt_EFIGS`, `icudt_no_CJK` e `icudt_CJK` — e escolhe **um** deles em runtime, pelo idioma do
navegador. `EFIGS` e ingles, frances, italiano, alemao e espanhol: **pt-BR nao esta la**. Com o
Chromium do runner em `en-US`, `CultureInfo.GetCultureInfo("pt-BR")` devolve uma cultura **sem
dados**, e `value.ToString("C2", PtBr)` do `BrlFormatter` sai `BRL96.90` em vez de `R$ 96,90`.

Verificado nos dois lados em 09/09:

| Navegador | Shard carregado | Coluna Total |
|---|---|---|
| `pt-BR` (producao, por Playwright) | `icudt_no_CJK` | `R$ 3.348,65` ✅ |
| `en-US` (runner do CI) | `icudt_EFIGS` | `BRL96.90` ❌ |

O defeito e de **produto**, nao de teste: um visitante com o navegador em outra lingua ve a moeda
errada. E ele nao para na moeda — data e numero saem do mesmo lugar (`RvmDatePicker`,
`ToString("N2")`).

⚠️ **A regra do `CLAUDE.md § Convencoes` — "moeda por `BrlFormatter`, nunca interpolacao manual" —
assume que passar `CultureInfo` explicito basta. No WASM nao basta**: a cultura existe como
objeto, os dados dela e que nao foram baixados. E falha em silencio, com um simbolo plausivel.

Por que so apareceu em 08/09 19:18: o teste nasceu na onda 4 (`DSGN-020`, `5dda16d`) e o primeiro
run depois do merge em `dev` ja foi vermelho. **Ele nunca passou contra o site publicado** — o CI
de PR nao roda E2E.

## Plano

1. **Site**: `BlazorWebAssemblyLoadAllGlobalizationData=true` no
   `src/RVM.DesignSystem.Docs`, que troca os tres shards por um `icudt.dat` unico e completo.
   Medido em 09/09, com publish `Release` dos dois jeitos:

   | | publicado (`_framework`) | o navegador baixa (brotli) |
   |---|---|---|
   | Antes (tres shards) | 19.700 KB | 144 KB (`EFIGS`) ou 222 KB (`no_CJK`) |
   | Depois (ICU completo) | **17.924 KB** | 322 KB, sempre |

   Ou seja: **o publish encolhe 1,8 MB** (some a duplicacao dos tres shards e dos `.br`/`.gz` de
   cada um) e o download cresce, no pior caso, **~180 KB brotli**. Em troca, o site formata
   pt-BR em qualquer navegador do mundo.

2. **Teste**: fixar `Locale = "en-US"` na pagina do teste de listagem. Hoje ele herda o idioma da
   maquina — passa no Windows do Rafael e falha no runner, que e a pior combinacao possivel. Com
   o locale fixo, o teste vira **prova** de que a formatacao nao depende do navegador.

3. **Biblioteca**: nada de codigo. O `BrlFormatter` esta certo; o que faltava era o dado. Fica
   registrada a exigencia para o consumidor WASM (§ Consequencia).

## Consequencia para quem consome o pacote

Um app Blazor **WebAssembly** que use a biblioteca herda a armadilha: sem
`BlazorWebAssemblyLoadAllGlobalizationData` (ou um ICU customizado que inclua pt-BR), toda moeda,
data e numero formatados pela biblioteca saem errados para quem estiver com o navegador em outra
lingua. Em Blazor **Server** o problema nao existe — o ICU e o do servidor.

## Validacao

- [x] Reproduzir: o teste com `Locale = "en-US"` falha contra o dev **antes** do deploy.
- [x] Deploy em dev e o mesmo teste passa, sem tocar no `LerColunaDeTotais`.
- [x] E2E completo verde (9 testes), incluindo o axe nas duas aparencias.
- [x] Conferir no navegador que `/padroes/listagem` mostra `R$` com o navegador em `en-US`.

## Resultado (09/09/2026)

Verde. O E2E do pipeline passou as 11:41 — **primeiro sucesso desde 08/09 14:55** — e o `dev`
serve `icudt.<hash>.dat`; `icudt_EFIGS` devolve 404, ou seja, o shard sumiu de vez.

Com o navegador em `en-US`, lado a lado na mesma tela:

| | coluna Total |
|---|---|
| dev antes do deploy | `BRL3,348.65` |
| dev depois | `R$ 3.348,65` |

O que o conserto **nao** resolve, e fica dito: um app consumidor em WASM continua tendo que
ligar a mesma flag. Isso agora esta no `CLAUDE.md § Convencoes`.

## Versao

Nao mexe na biblioteca — **nao gera versao nova do pacote**. E o site.
