---
id: DSGN-015
titulo: Animacao de entrada propria para cada tipo de grafico
repo: RVM.DesignSystem
tipo: feature
status: concluida
criada: 2026-09-18
---

# DSGN-015 — Cada grafico entra do jeito que faz sentido para ele

> **Publicada na `1.5.0` em 18/09/2026** (tag `v1.5.0`, versao conferida no feed do BaGet) e em
> producao em `https://design.rvmit.com.br`. E2E contra producao: **234 testes verdes**, a suite
> inteira. Alphas de `dev` passam a `1.6.0-alpha.N`.

O Rafael olhou as animacoes no ar e notou que **as colunas entram da esquerda para a direita**. Isso
nao era animacao de entrada: e efeito colateral da `transition` de `x`/`width`. Quando o JS mede a
largura real do grafico (de 600 px de chute para a largura do cartao), o SVG se redesenha e as barras
deslizam. Nas barras **horizontais** o acaso coincide com o certo; nas colunas, nao.

Direcao dada por ele em 18/09:

| Grafico | Como deve entrar |
|---|---|
| Colunas | de **cima para baixo** |
| Barras | da esquerda para a direita (ja parecia certo) |
| Histograma | igual as colunas |
| Linha e area | o traco continua como esta; **os pontos saem de (0,0)** e param no lugar |
| Pizza/rosca | **como um relogio**: comeca ao meio-dia e fecha a volta |
| Dispersao | os pontos **comecam no centro e se espalham** ate o lugar certo |

## Fora

Radar nao foi citado. Entra no mesmo criterio da dispersao (cresce do centro) por coerencia — se nao
for isso, e uma linha de CSS para mudar.

## Correcoes depois de ver no dev (18/09)

O Rafael conferiu no ar e apontou tres erros:

1. **Colunas e histograma desciam do topo** — deveria ser o oposto: crescem do eixo para cima
   (`transform-origin: bottom`, `rvm-grafico-subir`).
2. **A dispersao crescia do centro**, igual ao radar. Ela passa a sair da **origem do grafico (0,0)**,
   como a linha; do centro fica so o radar.

As classes passaram a dizer o que fazem: `rvm-grafico-de-baixo` (era "de-cima") e
`rvm-grafico-do-centro` (era "nuvem", que so descrevia a dispersao).

## Duracao: 1 s para todos (18/09)

As entradas nasceram com tempos diferentes (450 ms a 700 ms). O Rafael pediu **1 s para todas**, e a
duracao passou a sair de um token unico, `--rvm-grafico-entrada`, definido em `.rvm-grafico` — quem
consome pode sobrescrever num gráfico ou no tema inteiro.

A transicao de MUDANCA DE VALOR continua em 250 ms (`--rvm-transition-medium`): ela responde a uma
acao (trocar o periodo, aproximar, filtrar), e 1 s ali faria o grafico parecer travado.

## Decisoes de partida

- **Tudo em CSS, nada de estado inicial no C#.** Fazer a barra "nascer em zero" e crescer no segundo
  render deixaria o grafico vazio em SSR estatico, onde `OnAfterRender` nao roda — e a regra do
  projeto e que nenhum componente dependa de interatividade para o estado inicial.
- **A origem da transformacao vai em `style` inline** quando depende do layout (o canto do plot, o
  centro da rosca): o CSS isolado nao sabe calcular, e o C# ja tem os numeros.
- O bloco inteiro continua dentro de `@media (prefers-reduced-motion: no-preference)`.

## Entregue em 18/09/2026

| Grafico | Como entra agora | Como |
|---|---|---|
| Colunas, histograma | crescem do eixo para cima | `scaleY(0->1)` com `transform-origin: bottom` e `transform-box: fill-box` |
| Barras | abre da esquerda | `scaleX(0->1)` com `transform-origin: left` |
| Linha, area | traco se desenha (como antes) e **os pontos saem de (0,0)** | grupo `rvm-grafico-pontos` com a origem no canto de baixo a esquerda do plot |
| Pizza/rosca | **varredura de relogio**, do meio-dia fechando a volta | mascara com um circulo de `pathLength="1"`, `stroke-dasharray` animado e giro de -90 graus |
| Dispersao | da origem do grafico, como a linha | grupo `rvm-grafico-pontos` com a origem no canto de baixo a esquerda |
| Radar | do centro para fora | grupo `rvm-grafico-do-centro` com a origem no centro do desenho |

Tambem: `x`/`width` sairam da `transition` das colunas — era exatamente o que fazia as barras
deslizarem de lado quando o JS media a largura real. Nas barras horizontais a transicao de `x`/`width`
FICA, porque ali o valor cresce na horizontal.

## Decisoes

| Decisao | Por que |
|---|---|
| Animar em CSS, nunca com estado inicial no C# | barra "nascendo em zero" pelo segundo render deixaria o grafico vazio em SSR estatico, onde `OnAfterRender` nao roda |
| A origem que depende do layout vai em `style` inline | o CSS isolado nao sabe onde fica o centro da rosca nem o canto do plot; o C# ja tem o numero |
| A rosca usa mascara, e nao um `@keyframes` por fatia | animar cada fatia daria um efeito de pipoca; a mascara faz uma varredura unica, que e o que um relogio parece |
| `pathLength="1"` na mascara | dispensa calcular a circunferencia para o `stroke-dasharray` — o mesmo truque que a linha ja usava |
| Radar entrou junto com a dispersao | nao foi citado, e "crescer do centro" e o equivalente natural; uma linha de CSS se for outra coisa |
