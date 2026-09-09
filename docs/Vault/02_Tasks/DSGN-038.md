---
id: DSGN-038
titulo: A borda de controle nao vive so na `surface`
repo: RVM.DesignSystem
tipo: bug
status: concluido
criada: 2026-09-09
atualizada: 2026-09-09
---

# DSGN-038 — O par que existia na tela e nao existia na lista

## O que o card dizia, e por que estava errado

O `critique` reportou "borda do chip a ~1,5:1 no tema escuro, abaixo dos 3:1 da WCAG 1.4.11".
**Fui medir antes de consertar, e o achado nao se sustentou como descrito:**

- O elemento medido em 1,35:1 nao era o chip: era a **borda do card**, `--rvm-color-border`
  (`#2F2D30`) sobre a superficie escura. Borda decorativa **nao esta sujeita** a 1.4.11 — ela nao
  comunica estado nem delimita controle, e o proprio `RvmPalette` diz isso na documentacao do
  papel.
- Os chips tem `border-width: 0` e usam a cor semantica: 8,14–8,55:1. Passam folgado.

E a premissa que eu mesmo escrevi no card ("o portao nao cobre borda contra superficie") tambem
estava errada: `PairsToVerify` ja media `border-strong/surface` e `focus-ring/surface`.

## O defeito que apareceu no lugar — e este e real

Medindo os controles de verdade no tema escuro: a borda do campo dava **3,34:1** sobre a
`surface`... e **3,07:1** sobre a superficie elevada. Sete centesimos acima do minimo, sem nada
vigiando — porque **o par so era medido contra `surface`**, e um campo tambem cai sobre
`surface-raised` (dentro de um card), sobre `background` (solto na pagina) e sobre
`surface-sunken` (dentro de um bloco rebaixado).

Ao acrescentar os pares faltantes, **6 das 12 paletas reprovaram na hora**:

```
Fiscal/escuro       · border-strong/surface-raised: #606262 sobre #1E201F = 2.67:1
Propostinha/escuro  · border-strong/surface-raised: #626367 sobre #1E1F22 = 2.75:1
RVM/claro           · border-strong/surface-sunken: #8B888B sobre #E4E4EA = 2.77:1
```

Violacao de 1.4.11 **em producao**, em todo tema escuro: a borda do campo dentro de um card estava
abaixo de 3:1. E o mesmo defeito que o axe pegou em 08/09 com os papeis de estado — um par que
existe na tela e nao existia na lista.

## O que foi feito

1. `PairsToVerify` passa a medir `border-strong` e `focus-ring` contra **as quatro** superficies.
2. `FromSeed` deriva os dois garantindo o minimo contra a superficie que **mais aperta**, e nao
   contra a `surface` por padrao. Afastar-se de um fundo pode aproximar de outro, entao a busca
   repete ate todas passarem.
3. `EnsureContrast()` ganha o mesmo tratamento, com uma conta **propria**: para borda, a
   superficie mais exigente nao e a de luminancia media (que vale para texto), e sim a mais
   proxima da propria borda — um cinza medio some contra o cinza rebaixado muito antes de sumir
   contra o branco. Era esse o caso do `RVM/claro`, que o tema sobrescreve a mao.

## Validacao

- [x] `ContrasteTests` 12/12 verdes, com os pares novos.
- [x] 311 testes, Release com 0 avisos.
- [x] Confirmado que o achado original (borda do chip) **nao** era violacao — borda decorativa.

## Versao

Muda cor derivada em toda paleta: **patch** na biblioteca.
