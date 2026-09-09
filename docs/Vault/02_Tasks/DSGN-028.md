---
id: DSGN-028
titulo: Remove a aparencia Admin e refaz o Dashboard na linguagem daqui
repo: RVM.DesignSystem
tipo: feature
status: concluido
criada: 2026-09-08
---

# DSGN-028 — Fora o AdminLTE, fica o painel

Retorno do Rafael sobre a `DSGN-027`: *"nao ficou bom, remova o AdminLTE, mas crie um
dashboard"*.

Duas instrucoes distintas, e a segunda e a que importa: **a tela de painel fica**. O que sai e a
IMITACAO.

## O que saiu

- A aparencia **`Admin`** por inteiro: o bloco CSS, a opcao no seletor, as variaveis
  `--rvm-shell-*` que o `MainLayout` emitia e a entrada no teste de axe. Sobram quatro
  aparencias: `Atual`, `Sobrio`, `Marcante`, `Vivo`.
- Da tela de painel, as tres marcas registradas do AdminLTE: **bloco de cor solida**, **icone de
  marca d'agua** cortado pela borda e **faixa de rodape** "Mais informacoes".

⚠️ A casca escura saiu junto, e vale registrar o que se perde: era o unico jeito que a biblioteca
tinha de fazer **barra lateral escura sobre conteudo claro**, e a solucao era boa — as cores
vinham da paleta ESCURA do tema, que ja e um par medido, em vez dos hexadecimais fixos do
AdminLTE. Se um app do ecossistema quiser essa casca um dia, a tecnica esta descrita aqui e no
`DSGN-027`; nao precisa ser redescoberta.

## O que ficou

O **idioma** de painel, que nao pertence ao AdminLTE: numero que resume, barra que compara,
lista que detalha.

A caixa de numero virou uma **superficie da paleta** com a cor concentrada num distintivo redondo
e no valor. Continua colorida — a cor sai dos pares `x`/`on-x` — sem ser um bloco saturado.

## A parte que NAO podia sair junto

**Cada caixa e um link**, e isso e requisito, nao estilo.

Sem nenhum elemento focavel, o axe reprova a pagina por `scrollable-region-focusable`: a area de
conteudo do `RvmAppShell` rola, e sem nada que receba foco nao ha como rola-la pelo teclado. Na
`DSGN-027` quem resolvia isso era o rodape "Mais informacoes" do AdminLTE — que acabou de sair.

O link mudou de forma, nao de existencia: agora a caixa inteira e o link, com `aria-label`
dizendo o valor e para onde vai. O numero ja e a porta de entrada natural para a lista que ele
resume.

> ⚠️ **Isto continua valendo para qualquer consumidor.** Tela so de leitura, sem link nem botao,
> reprova. A resposta definitiva seria no `RvmAppShell`, e continua nao feita.

## Detalhe pequeno que nao e cosmetico

O valor nao quebra linha (`white-space: nowrap`). "R$ 6,2 mi" em duas linhas desalinhava a
fileira inteira e fazia o numero — a coisa que a pessoa veio ler — parecer dois numeros. O rotulo
abaixo quebra a vontade.

## Verificado

| | |
|---|---|
| Build | 0 erro, 0 aviso |
| Testes | 309 |
| E2E | 9 testes; axe em 46 paginas x 2 modos e nas 4 aparencias x 4 paginas |

## Estado

A escolha de aparencia continua aberta: `Atual`, `Sobrio`, `Marcante` ou `Vivo`.
