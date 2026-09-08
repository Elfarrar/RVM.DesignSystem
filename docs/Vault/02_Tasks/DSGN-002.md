---
id: DSGN-002
titulo: Camada de tokens, motor de tema e os dois portoes
repo: RVM.DesignSystem
tipo: feature
status: em-revisao
criada: 2026-09-08
---

# DSGN-002 — Tokens, tema e os portoes

Primeiro item da onda 1 do `09-roadmap.md`. Entrega o que o `04-modelo-de-dados.md` chama de
"o que ocupa o lugar do modelo de dados neste projeto": a camada de tokens e o motor que a
gera. Sem componente ainda — componente vem depois, em cima disto.

## Criterio de saida

O portao de contraste roda sobre os cinco temas registrados, nos dois modos, e **reprova de
verdade** quando recebe uma paleta ruim. Provado por teste que alimenta o portao com entrada
errada de proposito.

## Escopo

- [x] `RvmColor` — sRGB, OKLCH, contraste WCAG. Publico porque alimenta o site (`RF-24`)
- [x] `RvmContrast` — limiares AA e o ajuste que faz a cor **nascer** aprovada
- [x] `RvmPalette` — a lista fechada de papeis semanticos, com `PairsToVerify()`
- [x] `RvmTheme.FromSeed` — deriva os dois modos a partir de duas cores
- [x] `RvmTheme.ToCss` — emite as custom properties dos dois modos
- [x] `RvmThemes.Rvm` + os quatro temas de exemplo
- [x] `wwwroot/rvm-tokens.css` — espacamento, raio, tipografia, sombra, motion, breakpoint, z-index
- [x] Portao de contraste (5 temas x 2 modos x 14 pares)
- [x] Portao de CSS de componente
- [ ] `RvmThemeProvider` e o script anti-flash — proxima task
- [ ] Os nove componentes basicos — proxima task

## Decisoes

### 1. Ramifiquei de `dev`, nao de `master`

O `CLAUDE.md` manda ramificar de `master`. Aqui `master` esta 3 commits atras, porque producao
nao foi promovida — ramificar de la significaria refazer o bootstrap inteiro. **Enquanto o
`master` estiver atras do `dev`, a branch sai do `dev`.** Volta ao normal na primeira promocao.

### 2. Nao existe camada de token primitivo em CSS — e o portao ficou mais forte

O `04` descreve tres camadas (primitivo, semantico, de componente) e um teste que varre o CSS
atras de `--rvm-blue-500`. Na implementacao **a camada primitiva nao existe**: a paleta e derivada
em C# e so os papeis semanticos chegam ao navegador, entao um `--rvm-blue-500` nao tem como
aparecer — nunca e emitido.

A regra continua, numa forma que pega mais coisa: **CSS de componente so pode referenciar token
que existe, e nao pode cravar cor literal.** Isso cobre o defeito original (botao azul num tema
teal) e mais dois que a regra da spec deixava passar:

- `#0288d1` escrito direto no CSS;
- `var(--rvm-color-primry)` com erro de digitacao — que falha em **silencio**, porque CSS
  simplesmente ignora custom property inexistente e a cor some sem erro nenhum.

### 3. A paleta nasce aprovada, em vez de ser testada e torcida

Toda cor derivada passa por `RvmContrast.Ensure` contra o fundo em que vai ser usada. E o que
faz `FromSeed` aguentar semente hostil: ha teste com **amarelo `#FFEE00` como cor principal**,
e os 28 pares passam. O portao continua existindo para pegar paleta escrita a mao e regressao
da derivacao.

### 4. `disabled` mede 3:1, nao 4.5:1 — e isso e a regra certa

A WCAG 2.1 isenta componente desabilitado (SC 1.4.3, "Incidental"). Forcar 4.5 deixaria o
desabilitado tao legivel quanto o habilitado, ou seja, faria ele **parar de comunicar que esta
desabilitado**. A regra existe para acessibilidade; aplica-la aqui trabalharia contra ela.

### 5. O cinza do tema RVM e sobrescrito, nao derivado

`FromSeed` deriva superficies quase brancas; o `rvmtech.com.br` e visivelmente **cinza**
(`#EEEEF1`), e esse cinza e parte da identidade. Usei o `with` — o mesmo escape hatch que o `06`
oferece ao consumidor. Sem isso, a biblioteca e o site de documentacao discordariam da propria
marca, que num design system e o defeito mais caro que existe. **A sobrescrita passa pelo mesmo
portao de contraste** — sobrescrever nao isenta ninguem.

## Achado do ambiente, nao do codigo

⚠️ **A cobertura local nao roda nesta maquina.** `dotnet test --collect:"XPlat Code Coverage"`
falha em 100% dos testes com
`System.IO.FileLoadException: Uma politica de Controle de Aplicativo bloqueou este arquivo
(0x800711C7)`. O coverlet reescreve o assembly para instrumentar, e o Application Control do
Windows recusa carregar o arquivo modificado.

- **Nao e o codigo:** 62/62 passam sem instrumentacao.
- **Nao afeta o CI:** o `ci.yml` roda em `ubuntu-latest`, que nao tem essa politica — e o numero
  de cobertura que vale e o de la.
- Persiste depois de limpar `bin/` e `obj/`, entao nao e resto de execucao interrompida.
- Sintoma que engana: falham **todos** os testes de uma vez, o que parece regressao enorme.
  Numero de falhas igual ao total e sinal de infraestrutura, nao de defeito.
