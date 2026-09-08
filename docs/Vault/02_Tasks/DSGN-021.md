---
id: DSGN-021
titulo: Criar sua paleta — cores de estado escolhiveis e gerador de CSS
repo: RVM.DesignSystem
tipo: feature
status: em-andamento
criada: 2026-09-08
---

# DSGN-021 — Criar sua paleta

Pedido do Rafael em 08/09/2026: **uma forma de criar a paleta de cores do usuario**, respeitando
cinco regras. Esclarecido no meio da execucao: **a saida e o CSS**, nao so o snippet C#.

## O balanco antes de comecar

Quatro das cinco regras a biblioteca **ja cumpria**, e uma delas melhor do que a regra pede.
Vale registrar, porque o instinto era construir tudo do zero:

| Regra | Estado no inicio |
|---|---|
| 1. Primaria + secundaria | ✅ `FromSeed(nome, primary, secondary)` |
| 2. Neutros sem preto puro | ✅ texto em `L=0.22` / `L=0.94`, matizado pela marca |
| 3. Cores de feedback | ⚠️ existiam, mas **fixas** em 148/75/25/245 |
| 4. Proporcao 60-30-10 | ❌ inexistente |
| 5. Contraste WCAG | ✅✅ a paleta **nasce** aprovada, e o teste mede a cada build |

O trabalho real foi: abrir a 3, enderecar a 4 e construir a ferramenta.

## Entregue

- `RvmSeed` + sobrecarga `FromSeed(string, RvmSeed)` — **aditiva**, a de tres argumentos intacta
- `/fundamentos/paleta`: seletores de cor, papeis derivados nos dois modos, contraste medido ao
  vivo, a barra de proporcao, e **o CSS pronto para colar**
- `ColorInput` no site (nativo `<input type="color">` + campo hexadecimal)
- `RegrasDePaletaTests` — as regras viradas em portao

## A decisao central: o que a biblioteca aceita de quem escolhe

Da cor informada, o motor aproveita a **matiz e o croma**. A **luminosidade e sempre
recalculada** contra a superficie mais exigente da paleta, ate atender AA.

> A escolha e de identidade; o contraste nao e negociavel.

Ha teste que passa **amarelo puro** para sucesso e **branco** para aviso, em seis marcas
diferentes — incluindo preto, branco e cinza puro como marca —, e exige que os 29 pares
continuem passando nos dois modos. Passa.

⚠️ O croma informado e limitado a **0.20**, e nao por gosto: OKLCH e maior que sRGB, e um croma
alto demais gera uma cor que o monitor nao mostra. Ela seria recortada na conversao, saindo
diferente do que a pessoa escolheu **e com luminosidade diferente da calculada** — furando a
garantia de contraste em silencio.

E um cinza informado continua cinza: o motor nao inventa saturacao que ninguem pediu.

## A regra que NAO tem teste, e por que

**A proporcao 60-30-10 e de composicao, nao de paleta.** Ela fala de quanto de cada cor aparece
numa tela montada; a mesma paleta obedece a regra numa tela e a viola noutra. Um portao que
dissesse verificar isso estaria medindo outra coisa — e portao que mede a coisa errada e pior
que portao nenhum.

Ela vive na pagina, demonstrada numa barra e numa tabela de "que papel usar em cada fatia", com
uma heuristica pratica: **se da para contar mais de dois ou tres elementos na cor primaria numa
tela, ela passou dos 10%**.

## O achado que so apareceu usando a ferramenta

Trocando a primaria para vermelho (`#B3261E`), o `danger` saiu `#BE222A` — **quase a mesma cor**.

Nao e falha de contraste: as duas passam folgado em AA contra o fundo. O problema e outro, e e
mais perigoso — **"Salvar" e "Excluir" passam a parecer o mesmo botao**, e quem confere pela cor
antes de clicar perde a unica pista que tinha.

A ferramenta agora avisa quando a matiz da marca chega a menos de 25 graus de uma cor de estado.
E o tipo de defeito que nenhum teste de contraste pega, porque contraste mede cada cor contra o
**fundo**, nunca uma contra a outra.

## Verificado

| | |
|---|---|
| Testes | 282 (eram 260), 0 aviso em `Release` |
| Portao de contraste | 29 pares x 5 temas x 2 modos, mais 6 marcas x cores de estado extremas |
| E2E | 7 testes, axe em 46 paginas x 2 modos |

## O que fica fora, de proposito

**Tema por usuario final em runtime (white-label por tenant)** continua fora do escopo
(`09` § Fora da v1, `CLAUDE.md` § Escopo). Esta ferramenta e para **quem constroi um produto**
escolher a paleta dele uma vez e levar o CSS embora — nao para o usuario do app escolher cores.
Se isso mudar, e decisao nova.
