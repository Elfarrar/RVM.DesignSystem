---
id: DSGN-015
titulo: Animacao de entrada propria para cada tipo de grafico
repo: RVM.DesignSystem
tipo: feature
status: concluida
criada: 2026-09-18
---

# DSGN-015 — Cada grafico entra do jeito que faz sentido para ele

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
| Colunas, histograma | desce do proprio topo | `scaleY(0->1)` com `transform-origin: top` e `transform-box: fill-box` |
| Barras | abre da esquerda | `scaleX(0->1)` com `transform-origin: left` |
| Linha, area | traco se desenha (como antes) e **os pontos saem de (0,0)** | grupo `rvm-grafico-pontos` com a origem no canto de baixo a esquerda do plot |
| Pizza/rosca | **varredura de relogio**, do meio-dia fechando a volta | mascara com um circulo de `pathLength="1"`, `stroke-dasharray` animado e giro de -90 graus |
| Dispersao, radar | do centro para fora | grupo `rvm-grafico-nuvem` com a origem no centro do desenho |

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
