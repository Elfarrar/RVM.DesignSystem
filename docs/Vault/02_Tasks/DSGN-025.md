---
id: DSGN-025
titulo: Preview de aparencia — duas direcoes no ar, para escolher olhando
repo: RVM.DesignSystem
tipo: spike
status: aguardando-decisao
criada: 2026-09-08
---

# DSGN-025 — Preview de aparencia

Pedido do Rafael em 08/09/2026: *"eu estou achando muito sem vida, o que voce sugere para dar
mais vida ao design"*. Escolha dele entre as opcoes: **os dois (biblioteca e site), em ondas**,
e **me mostre as duas intensidades**.

Este card e a parte "mostre as duas". **Nenhuma linha da biblioteca mudou.**

## O diagnostico, medido

Fui contar em vez de opinar:

| | |
|---|---|
| Niveis de sombra definidos | 6 |
| Componentes que usam sombra | **9 de 39** |
| Tamanho `display` com `clamp()` | definido, **usado em lugar nenhum** |
| Tokens de movimento | definidos, quase sem uso |
| `RvmCard` | **ja tem elevacao 0–5**; o site usa `Outlined` com 0 em toda parte |
| `--rvm-shadow-1` | `rgb(0 0 0 / 0.06)` — preto a **6%** |

**Nao falta vocabulario. Falta usar o que ja existe** — e as sombras estao fracas demais para
aparecer mesmo quando usadas.

O resto do diagnostico: quase tudo e superficie branca sobre fundo quase branco, separada por
1px de borda cinza, com h1/h2/h3 em tamanhos proximos. Legivel, acessivel e chapado.

## Por que preview em vez de proposta escrita

Porque e decisao de gosto, e gosto se decide olhando. As duas direcoes ficam no ar ao mesmo
tempo, na mesma tela de verdade (`/padroes/listagem`), trocaveis pelo seletor "Aparencia" na
topbar.

**E temporario e nasce com data para morrer**: `wwwroot/css/aparencia.css`,
`wwwroot/js/aparencia.js`, o seletor e o teste de axe correspondente saem inteiros quando a
direcao for escolhida. O que valer vira mudanca no pacote, com o teste de contraste e o axe como
portao.

## O que cada preset muda

| | Sobrio | Marcante |
|---|---|---|
| Sombra | 6% → ~10%, cartao com elevacao 1 | ~12%, elevacao 2, botao levanta no hover |
| `h1` | 2.25rem → 2.75rem | → 3.25rem, peso 700, tracking negativo |
| Raio | md 8→10px, lg 12→14px | md 8→12px, lg 12→16px |
| Casca | topbar com elevacao | topbar em `primary-container`, sidebar afundada |
| Movimento | 150ms em sombra | 150ms + `translateY(-1px)` no botao |

`prefers-reduced-motion` desliga a transicao e o deslocamento nos dois.

## ⚠️ O que os presets NAO mexem, de proposito

**Cor de superficie** — e o lever mais forte que existe para tirar a chatice, e por isso mesmo
esta fora daqui.

A paleta nasce verificada pelo `RvmContrast.Ensure`, e o teste mede 29 pares a cada build. Um
cinza tingido escrito a mao numa folha de estilo **furaria essa garantia em silencio**: a tela
continuaria bonita e o portao nao teria como saber que a superficie mudou por baixo dele. Foi
exatamente esse o defeito que reprovou 14 paginas no axe em 08/09/2026, com o teste unitario
aprovando.

Tingir superficie e mudanca de **motor de tema** — `FromSeed` mais `EnsureContrast()` —, nao de
CSS. Fica para depois da escolha, onde o portao alcanca.

Onde uso cor aqui, uso papel ja verificado (`primary-container` / `on-primary-container`), nunca
valor literal.

## Verificado

- Build 0 erro / 0 aviso; 282 testes
- **Teste de axe novo sobre os dois presets**, em 3 paginas cada. Existe porque o "marcante"
  pinta a topbar: mudar cor de fundo e o tipo de mudanca que quebra contraste sem ninguem notar,
  e o portao da biblioteca **nao alcanca isso** — ele mede pares da paleta, nao qual par foi
  aplicado em qual elemento. So o axe, no navegador, ve.

## Estado

⏳ **Aguardando a escolha do Rafael**: Atual, Sobrio, Marcante — ou "quero ver com a superficie
tingida antes de decidir", que e uma quarta resposta legitima e custa uma rodada a mais.
