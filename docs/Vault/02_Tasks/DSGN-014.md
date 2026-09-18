---
id: DSGN-014
titulo: Tokens que o kit nao escreve, medidos por amostragem de pixel
repo: RVM.DesignSystem
tipo: chore
status: em andamento
criada: 2026-09-18
---

# DSGN-014 — As quatro pendencias de token, resolvidas na marra

As quatro pendencias do `CLAUDE.md` esperavam "duplicar o kit no Figma e ler pela API". Em 18/09 o
Rafael informou que **no Figma o kit tambem e uma imagem, nao objetos de desenho**: nao ha valor para
ler, em lugar nenhum. Ele mandou fazer por amostragem.

Ferramenta: `tools/amostragem-do-kit.py` (`sombras`, `texto`, `fundos`). Nao roda no CI — e para
repetir a mao se a referencia mudar.

## 1. As 24 elevacoes — **medidas**

A tela `Shadow/Light.png` tem os 24 quadrados numa grade de 71 px com 25 px de folga. **As sombras se
sobrepoem**: medir uma de cada vez dava "espalhamento" de 20 px que nao existe, so porque a mancha do
vizinho entrava na conta. O ajuste final e conjunto — cada quadrado sobre o residuo dos outros, em
passadas ate parar de mudar. Residuo final: **0,008 de opacidade** (0 a 1).

| Kit | dy | blur | alpha | | Kit | dy | blur | alpha |
|---|---|---|---|---|---|---|---|---|
| 1 | 1 | 2 | 0,47 | | 13 | 10 | 21 | 0,31 |
| 2 | 2 | 3 | 0,35 | | 14 | 12 | 22 | 0,30 |
| 3 | 4 | 4 | 0,12 | | 15 | 12 | 24 | 0,30 |
| 4 | 6 | 9 | 0,13 | | 16 | 12 | 25 | 0,30 |
| 5 | 4 | 8 | 0,30 | | 17 | 12 | 26 | 0,31 |
| 6 | 2 | 9 | 0,09 | | 18 | 14 | 28 | 0,31 |
| 7 | 6 | 11 | 0,30 | | 19 | 14 | 30 | 0,30 |
| 8 | 7 | 12 | 0,29 | | 20 | 14 | 32 | 0,31 |
| 9 | 8 | 13 | 0,29 | | 21 | 15 | 34 | 0,31 |
| 10 | 8 | 15 | 0,31 | | 22 | 16 | 35 | 0,30 |
| 11 | 9 | 17 | 0,28 | | 23 | 16 | 39 | 0,31 |
| 12 | 10 | 19 | 0,31 | | 24 | 18 | 40 | 0,31 |

**A sombra do kit nao e preta**: no pixel mais escuro, o canal azul cai para 0,780 do fundo enquanto o
vermelho cai para 0,753 — ela escurece menos o azul. A cor que sai da conta e `#34334A`, praticamente
o roxo escuro que a biblioteca ja usava (`#3A3541`).

**Aplicado:** a biblioteca tem cinco niveis, nao 24. Cada um passou a levar os numeros de uma elevacao
do kit — 1, 3, 7, 12 e 20 — em vez das duas camadas estimadas de antes.

Limites honestos: `spread` fica em 0 (com as manchas sobrepostas, espalhamento e deslocamento nao se
separam) e o ajuste e de **uma** camada, enquanto o kit provavelmente empilha duas ou tres. O que sai
e a sombra equivalente, nao a receita original.

## 2. Opacidade do texto no tema escuro — **confirmada, menos o desabilitado**

Medido linha a linha (o percentil mais cheio de cada linha de texto; erodir o traco apagaria as
legendas pequenas) em `Typography`, `Text Field`, `Checkbox` e `Switch`:

| Nivel | Token antes | Medido | Situacao |
|---|---|---|---|
| primario | 0,87 de `#E7E3FC` | 0,84–0,86 | **confirmado** (o metodo subestima: fonte pequena nao atinge a cor cheia) |
| secundario | 0,68 | 0,66–0,67 | **confirmado** |
| desabilitado | 0,38 | **0,22–0,23** | **corrigido para 0,26** |

O mesmo vale no tema claro: o campo desabilitado do kit e `#D2D1D4` (23% da base), e o token era
`#B4B2B7` (38%) — mais escuro que o kit. WCAG isenta componente desabilitado do contraste minimo
(criterio 1.4.3), entao aqui a fidelidade ganha.

Contraste conferido no tema escuro sobre `#312D4B`: primario 7,88:1 e secundario 5,44:1 — os dois
passam AA com folga.

## 3. Fundos suaves roxos com primary azul — **medido, decisao do Rafael**

No `Chip-1.png` (claro), o primary cheio e `#264CC8` (matiz 226°) e o fundo suave do primary e
`#F2EAFF` (matiz 263°). A conta fecha exata: **`#F2EAFF` e `#9155FD` a 12%** — um roxo que **nao
existe na paleta do kit**. E resquicio do template de onde o kit saiu.

As outras familias sao coerentes: error `#FF4C51` → `#FEE8E7`, info `#16B1FF` → `#E4F2FE`.

Duas saidas, e a escolha e do Rafael:
- **Harmonizar** (recomendado): derivar do proprio primary — `#264CC8` a 12% da `#E5EAF8`.
- **Manter fiel**: seguir o kit e preservar um roxo que o template anterior deixou para tras.

## 4. Icones em vetor — **pendencia encerrada**

Dependia de reexportar o kit do Figma. Com o arquivo sendo imagem, **nao ha vetor para exportar em
lugar nenhum**: os 257 SVGs da referencia sao PNG embutido, e o Figma nao tem nada melhor. O conjunto
Tabler (ADR-005) deixa de ser interino e passa a ser a decisao final.

## Decisoes

| Decisao | Por que |
|---|---|
| Ajustar as 24 sombras de uma vez, nao uma a uma | elas se sobrepoem na grade do kit; isolar cada uma inventava espalhamento que nao existe |
| Nossos cinco niveis pegam as elevacoes 1, 3, 7, 12 e 20 | a biblioteca nao tem 24 niveis e o contrato da API nao muda por causa de medicao |
| Texto desabilitado passa a 0,26 nos dois temas | e o que o kit mostra; WCAG isenta desabilitado do contraste minimo |
| A ferramenta fica versionada em `tools/` | medicao que ninguem consegue repetir vira folclore na proxima duvida |
