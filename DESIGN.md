---
name: RVM Design System
description: Biblioteca Blazor própria do ecossistema RVM — tokens, tematização e WCAG 2.1 AA como portão de CI.
colors:
  primary: "#641974"
  on-primary: "#FFFFFF"
  primary-container: "#F0D0EF"
  on-primary-container: "#4D005C"
  secondary: "#0068B8"
  on-secondary: "#FFFFFF"
  success: "#08762F"
  warning: "#945800"
  danger: "#BE222A"
  info: "#0068B8"
  surface: "#FCFCFE"
  on-surface: "#1E1E26"
  on-surface-variant: "#545155"
  surface-raised: "#FFFFFF"
  surface-sunken: "#E4E4EA"
  background: "#EEEEF1"
  border: "#DEDEE4"
  border-strong: "#8B888B"
  focus-ring: "#641974"
  dark-primary: "#D588E7"
  dark-surface: "#151316"
  dark-on-surface: "#EDEAEE"
  dark-surface-raised: "#211E21"
  dark-background: "#0E0C0F"
  dark-border: "#2F2D30"
typography:
  display:
    fontFamily: "Geist, Inter, -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, sans-serif"
    fontSize: "clamp(2rem, 1.25rem + 3.75vw, 3.5rem)"
    fontWeight: 600
    lineHeight: 1.05
    letterSpacing: "-0.03em"
  h1:
    fontFamily: "Geist, Inter, sans-serif"
    fontSize: "2.25rem"
    fontWeight: 600
    lineHeight: 1.25
    letterSpacing: "-0.015em"
  h3:
    fontFamily: "Geist, Inter, sans-serif"
    fontSize: "1.5rem"
    fontWeight: 600
    lineHeight: 1.25
    letterSpacing: "-0.015em"
  body:
    fontFamily: "Geist, Inter, sans-serif"
    fontSize: "1rem"
    fontWeight: 400
    lineHeight: 1.55
    letterSpacing: "normal"
  caption:
    fontFamily: "Geist, Inter, sans-serif"
    fontSize: "0.75rem"
    fontWeight: 400
    lineHeight: 1.55
    letterSpacing: "normal"
  code:
    fontFamily: "Geist Mono, ui-monospace, SF Mono, Cascadia Mono, Menlo, monospace"
    fontSize: "0.875rem"
    fontWeight: 400
    lineHeight: 1.55
    letterSpacing: "normal"
rounded:
  none: "0"
  sm: "0.25rem"
  md: "0.5rem"
  lg: "0.75rem"
  full: "9999px"
spacing:
  1: "0.25rem"
  2: "0.5rem"
  3: "0.75rem"
  4: "1rem"
  6: "1.5rem"
  8: "2rem"
  12: "3rem"
  16: "4rem"
components:
  button-primary:
    backgroundColor: "{colors.primary}"
    textColor: "{colors.on-primary}"
    typography: "{typography.body}"
    rounded: "{rounded.md}"
    padding: "0.5rem 1rem"
    height: "2.5rem"
  button-secondary:
    backgroundColor: "transparent"
    textColor: "{colors.primary}"
    rounded: "{rounded.md}"
    padding: "0.5rem 1rem"
    height: "2.5rem"
  surface-card:
    backgroundColor: "{colors.surface-raised}"
    textColor: "{colors.on-surface}"
    rounded: "{rounded.md}"
    padding: "1.5rem"
  text-field:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.on-surface}"
    rounded: "{rounded.md}"
    height: "2.5rem"
  topbar:
    backgroundColor: "{colors.surface-raised}"
    textColor: "{colors.on-surface}"
    height: "3.5rem"
  sidebar:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.on-surface}"
    width: "16rem"
---

# RVM Design System

## Overview

Biblioteca de componentes Blazor **escrita do zero** (sem MudBlazor por baixo) mais o site que a
documenta — e que é a primeira aplicação construída com ela. Roda em Blazor Server **e**
WebAssembly, sempre nos dois.

A identidade vem do currículo do Rafael (`rvmtech.com.br`): roxo Visual Studio como primária, azul
VS Code como secundária. O que a interface deve transmitir é **técnico, denso, elegante** —
densidade como sinal de competência, tipografia trabalhando, decoração nenhuma.

A regra que organiza tudo: **token antes de componente**. Componente jamais lê um primitivo
(`--rvm-blue-500`), só o papel semântico (`--rvm-color-primary`), e há teste que varre o CSS e
reprova o build quando alguém escapa.

## Colors

**As cores não moram no CSS.** São derivadas em C# por `RvmTheme.FromSeed` e emitidas em tempo de
execução pelo provider de tema, porque mudam por produto e por modo. O `rvm-tokens.css` guarda só o
que não muda com o tema: espaçamento, raio, tipografia, sombra, motion, breakpoint, z-index.

Estratégia de cor: **restrained**. Neutros levemente tingidos para o roxo da marca, e a primária
usada como acento — nunca como fundo de superfície grande. Nada de `#000` ou `#fff` puros nas
superfícies: o claro é `#FCFCFE` e o escuro é `#151316`.

Todo par `x` / `on-x` de toda paleta passa por teste de contraste no CI. **Abaixo de AA, build
vermelho** — inclusive para paletas que o consumidor monte. `EnsureContrast()` re-deriva os papéis
coloridos até passarem; foi por isso que o amarelo `#FFC107` do Bootstrap virou o âmbar `#955B00`
quando entrou como tema de exemplo.

O modo escuro não é a paleta clara invertida: a primária clareia (`#641974` → `#D588E7`) porque
roxo escuro sobre fundo escuro não tem contraste, e o `on-primary` passa a ser quase-preto.

## Typography

Escala em `rem` com base 16px, fixa. A **única** exceção é o `display`, que usa `clamp()` — um
título de 3rem estoura a linha no celular. Decisão registrada: previsível vale mais que elegante
numa v1.

Família **Geist** (a mesma do `rvmtech.com.br`), auto-hospedada — não há CDN em runtime. Três pesos
só: 400, 500, 600. Hierarquia sai de escala + peso, e o `tracking` fecha conforme o tamanho sobe
(`-0.03em` no display, `0` no corpo).

Corpo com `line-height` 1.55. ⚠️ **Largura de linha ainda não é token**, e medido em 09/09/2026 o
texto corrido do site chega a 109 caracteres — acima dos 65–75 que se lê confortavelmente.

## Elevation

Cinco degraus de sombra (`--rvm-shadow-1` a `5`), sempre em duas camadas: uma sombra de contato
curta mais uma difusa.

⚠️ **No tema escuro a sombra sozinha não comunica elevação** — preto sobre preto não aparece. Por
isso `surface-raised` existe como papel próprio: no escuro, o que sobe também **clareia**
(`#151316` → `#211E21`). Elevação ali é mudança de superfície acompanhada de sombra, nunca sombra
sozinha.

Z-index é faixa fechada e em ordem de quem cobre quem: dropdown 1000, sticky 1100, drawer 1200,
dialog 1300, toast 1400, tooltip 1500. Se o app precisa de algo acima do tooltip, o componente
escolhido é o errado.

## Components

35 componentes na v1, em quatro grupos: básicos, layout, feedback e dados. Todo texto padrão é
sobrescrevível por parâmetro, e a `Class` do consumidor **soma** com as internas, nunca substitui.

**Altura e espaçamento vertical de controle saem de `--rvm-control-*`, nunca de `--rvm-space-*`.**
É o que faz a densidade `Compact` valer sem o componente saber que ela existe — e um componente que
use a escala de espaçamento no eixo vertical a ignora **em silêncio**, que é o pior tipo de defeito.

**A plataforma primeiro**, três vezes e de propósito: `<select>` nativo, `<dialog>` com
`showModal()` (foco preso, ESC e retorno de foco vêm do navegador) e `<table>` semântica **sem**
`role="grid"`. Quando o navegador ou o leitor de tela já resolve, reimplementar entrega pior.

Movimento: 120ms para retorno de toque, 200ms para transição comum, 320ms para camada entrando.
Curvas próprias (`cubic-bezier(0.2, 0, 0, 1)` como padrão), sem bounce. Tudo respeita
`prefers-reduced-motion`.

## Do's and Don'ts

**Do**

- Leia só papel semântico: `--rvm-color-primary`, `--rvm-color-on-surface`, `--rvm-color-border`.
- Altura de controle por `--rvm-control-height-*`; espaçamento por múltiplo da escala.
- Prefixe toda classe pública com `rvm-`, além do CSS isolation do Blazor.
- Deixe o navegador fazer o que ele já faz bem, e diga por escrito quando escolher isso.
- Escreva o texto ao usuário em PT-BR explicativo, e exponha-o como parâmetro.

**Don't**

- Não leia primitivo (`--rvm-blue-500`) num componente. Há teste, e ele reprova.
- Não use `--rvm-space-*` para altura ou padding vertical de controle: mata a densidade em silêncio.
- Não faça `role="alert"` ou `aria-live="assertive"` padrão — atropela o título da página a cada
  render. É opt-in.
- Não alcance com CSS o elemento renderizado por outro componente sem `::deep`; e no conteúdo que o
  consumidor passa num slot, `::deep` costuma ser a resposta errada: a biblioteca garante o
  contêiner, o consumidor estiliza o que é dele.
- Não recrie o AdminLTE: bloco escuro sólido, marca d'água e faixa de rodapé foram **reprovados**.
- Não caia no template de SaaS: número gigante com rótulo pequeno, grade de cards idênticos,
  gradiente em texto.
