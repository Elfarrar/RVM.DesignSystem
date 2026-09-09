---
id: DSGN-026
titulo: Aparencia "Vivo" e o conjunto de icones por aplicacao
repo: RVM.DesignSystem
tipo: feature
status: aguardando-decisao
criada: 2026-09-08
---

# DSGN-026 — Mais impacto, mais cor, mais icone

Retorno do Rafael sobre a `DSGN-025`: *"eu nao vi muita diferenca, gostaria de algo mais
impactante um pouco mais de cor, e os icones achei muito pouco icone"*.

Ele esta certo nas duas, e a primeira e culpa de uma escolha minha que eu declarei no card
anterior: **os presets nao mexiam em superficie**, que e o lever mais forte que existe. Eu deixei
de fora porque nao dava para fazer em CSS sem furar o portao de contraste. A resposta certa nao
era desistir — era fazer pelo caminho que mantem o portao.

## 1. "Vivo" — a superficie tingida, pelo motor de tema

`Documentacao/AparenciaViva.cs` deriva as superficies da **matiz da primaria** e passa o
resultado por `EnsureContrast()`, exatamente como o `RvmThemes.Rvm` faz com os cinzas do site.

⚠️ **O tingimento mexe muito na CROMA e pouco na LUMINOSIDADE**, e isso nao e gosto: o
`EnsureContrast()` re-deriva os papeis de frente **coloridos**, mas nao o `on-surface` nem o
`on-background`. E a luminosidade que carrega o contraste do texto. Tingir e diferente de
escurecer, e so o primeiro e seguro sem mexer no texto junto.

A casca inteira vai para `primary` + `on-primary` — par verificado, nunca literal.

### O defeito que o primeiro screenshot pegou

Pintar o fundo da topbar deixou **os rotulos "Tema"/"Aparencia" e o botao de icone escuros sobre
o roxo**: eles herdavam `on-surface`, que continuou valendo. Texto ilegivel, na barra que fica
em toda tela. Corrigido — cada preset usa o `on-*` do papel que ele mesmo aplicou no fundo.

E o motivo de o teste de axe ter passado a **usar o seletor** em vez de escrever o atributo: o
"vivo" e metade CSS e metade C#, e um teste que so escrevesse o atributo aprovaria a aparencia
pela metade — justamente sem a parte que mexe nas cores.

### Como o tingimento se cola no tema

A casca reage ao evento `Changed` do servico de tema, o que cobre os tres caminhos de troca (o
seletor de tema, o de aparencia e a pagina de paleta) sem eu ter de lembrar de tingir em cada um.

**Nao ha laco porque `Tingir` e idempotente**: as superficies saem de valores fixos calculados da
matiz da primaria, que o tingimento nao altera. Tingir duas vezes da o mesmo tema, a comparacao
por valor da igual, e a segunda passada nao faz nada.

E ha um `_temaBase` separado no `MainLayout`: sem ele, o seletor nao acharia o tema tingido na
lista e passaria a chama-lo de "sua paleta", mentindo sobre o que o visitante escolheu.

## 2. Icones — de 27 para 164, e a regra muda

### A regra antiga otimizava a coisa errada

Era *"icone entra quando um componente precisa, nunca porque pode ser util"*. Fazia sentido
enquanto a biblioteca estava sendo construida. Mas **quem consome nao monta componentes, monta
telas** — e uma tela de pedido precisa de um icone de pedido que nenhum componente meu jamais vai
pedir. Com 27 icones, o consumidor cai fora do design system no primeiro botao de "imprimir", e
um conjunto que empurra para fora falhou no proposito dele.

### ⚠️ E o argumento que eu usei para sustenta-la nao se sustentava

Eu escrevi que "cada um e um nome publico e **bytes que todo consumidor baixa**". **Medido**:
~300 bytes por icone-peso. Os 27 davam 16 KB; os 164 dao 113 KB de fonte C#, ruido perto do
runtime do WASM.

O argumento **real** para curar nunca foi byte — e superficie de API e manutencao. Esse continua
valendo, e e por isso que sao 164 e nao os 1512 do Phosphor.

### Nenhum nome saiu

Remover um nome depois da 1.0 e quebra, **em tempo de execucao**. E o risco e concreto: a lista
vive no `tools/gerar-icones.py` e o `RvmIconData.cs` e **regerado por inteiro** a cada mudanca —
uma linha apagada por descuido apagaria o icone do pacote sem nenhum outro aviso.

Por isso ha o `ConjuntoDeIconesTests`: renderiza os 27 da v1 nos dois pesos e exige desenho
nao-vazio. A lista dele e **escrita a mao**, nao gerada da lista atual — um teste que le a mesma
fonte que verifica nao verifica nada.

## 3. Todo item de menu ganhou icone

Antes so "Inicio" tinha, e a barra lateral era uma coluna de texto — a maior superficie da tela
sem nenhuma forma para o olho ancorar. Sao 44 itens novos com icone.

Alguns sao metafora frouxa (nao existe desenho obvio para "Skeleton"). Mesmo assim forma distinta
ajuda a varrer, e e por isso que nenhum ficou sem.

## Verificado

| | |
|---|---|
| Build | 0 erro, 0 aviso |
| Testes | **309** (eram 282; +27 do portao de icones) |
| E2E | axe nos **tres** presets x 3 paginas, pelo seletor |

## Estado

⏳ **Aguardando o Rafael**: `Atual`, `Sobrio`, `Marcante` ou `Vivo`.

Os icones **nao** dependem dessa escolha — ja estao valendo nas quatro.
