---
id: DSGN-022
titulo: Graficos — decisao de abordagem antes de qualquer codigo
repo: RVM.DesignSystem
tipo: feature
status: aberto
criada: 2026-09-08
---

# DSGN-022 — Graficos

**Card aberto sem codigo, de proposito** (`KANBAN` § "o card nasce ANTES do codigo"). Ha uma
decisao de arquitetura aqui que nao e minha, e implementar antes dela seria escolher em silencio.

## Por que agora

O `09-roadmap.md` § Fora da v1 poe graficos na v2 **com condicao**: *"se algum app RVM
precisar"*. O Rafael informou em 08/09/2026 que **o projeto de obras vai precisar**.

A condicao foi satisfeita. Falta decidir o **como**.

## ⚠️ O conflito que este card existe para resolver

O roadmap diz:

> Graficos (chart, sparkline) — v2, se algum app RVM precisar. **Provavelmente embrulhando uma
> biblioteca existente, nao do zero.**

E o `CLAUDE.md` § Convencoes diz:

> **Sem dependencia NuGet de terceiros na biblioteca. Sem CDN em runtime.**

E o `CLAUDE.md` § Arquitetura diz:

> **Nenhum componente pode depender de JS para renderizar seu estado inicial**; JS so em
> `OnAfterRenderAsync`.

**As tres frases nao cabem juntas.** "Embrulhar uma biblioteca existente" foi escrito em
07/09/2026 como um palpite, antes de as outras duas terem sido exercidas quatro ondas seguidas.
Nao e contradicao esquecida — e um palpite que as restricoes seguintes invalidaram, e por isso
esta aqui em vez de ser resolvido por conta propria.

## As quatro saidas

### A. SVG puro, gerado em C#

| | |
|---|---|
| Dependencia | nenhuma |
| Renderiza sem JS | **sim** — SVG e marcacao, nasce no primeiro paint |
| Server + WASM | identico nos dois |
| Cor | sai dos papeis semanticos; o portao de contraste alcanca |
| Leitor de tela | o proprio SVG aceita `role="img"` + `<title>`/`<desc>` |
| Imprime | sim |

Custo: a geometria e nossa. Escopo realista de uma primeira entrega: **linha, area, barra/coluna
e rosca**, com eixo, grade, legenda e rotulo. Fora: zoom, pan, brush, 100 mil pontos.

E o mesmo criterio das ondas 1, 3 e 4 — `<select>`, `<dialog>` e `<table>` nativos. **A
plataforma primeiro.**

### B. Embrulhar biblioteca JS (Chart.js, ApexCharts, ECharts)

Tres problemas, e o segundo e eliminatorio pelas regras de hoje:

1. **Sem CDN** significa auto-hospedar o bundle dentro da RCL — centenas de KB baixados por
   **todo** consumidor, inclusive quem nunca usa grafico.
2. **Canvas nao renderiza sem JS.** Viola a regra do `CLAUDE.md` de forma direta, nao de
   nuance: a tela sai vazia ate o script rodar.
3. **Canvas e invisivel ao leitor de tela.** Precisaria de uma tabela equivalente em paralelo —
   que, se e necessaria de qualquer jeito, enfraquece o argumento da opcao B.

⚠️ **A licenca precisa ser conferida antes de considerar esta opcao.** O pacote hoje declara
`PackageLicenseExpression` = MIT puro, e foi por isso que o Phosphor venceu o Lucide na onda 1
(`03` § Icones). Uma biblioteca Apache-2.0 mudaria a expressao de licenca do pacote.

### C. Embrulhar pacote .NET (Blazor-ApexCharts, Plotly.Blazor, ...)

Viola "sem dependencia NuGet de terceiros" **diretamente**, e arrasta dependencias transitivas
para dentro de todo consumidor — com o conflito de versao que vem junto.

### D. Nao entrar na biblioteca

O app de obras escolhe a ferramenta que quiser e consome os **tokens** do design system para as
cores. Custo zero para a biblioteca.

Preco: todo app futuro redecide, e nenhum grafico fica consistente com o tema automaticamente —
que e exatamente o problema que o design system existe para resolver.

## Recomendacao

**A**, com escopo apertado. Mas a decisao e do Rafael, porque contraria uma linha do roadmap.

## O que decidir antes de comecar

1. **Qual das quatro saidas.**
2. **Quais tipos de grafico o obras precisa de verdade** — a diferenca entre "linha e barra" e
   "linha, barra, area, rosca, dispersao e combinado" e a diferenca entre uma onda e tres.
3. **Paleta categorica de series.** Problema **novo**: hoje toda cor da biblioteca e medida
   contra o **fundo**. Serie de grafico precisa ser distinguivel **de outra serie** — e o
   portao de contraste nao mede isso. E o mesmo tipo de defeito que o aviso de colisao da
   `DSGN-021` pegou, e vai precisar de garantia propria.
4. **Alternativa textual.** Grafico sem equivalente em texto e inacessivel por construcao. Com
   o `RvmDataGrid` pronto, o padrao natural e "grafico + tabela equivalente" — decidir se ela e
   obrigatoria no componente ou responsabilidade de quem usa.

## Versao

**`1.2.0`, nao "v2".** O roadmap escreveu "v2" antes de a numeracao existir; componente novo e
aditivo, e aditivo e **minor** sob SemVer. A `1.0.0` congelou o contrato — nada aqui precisa
quebra-lo.

## Estado

⏳ **Aguardando a decisao do Rafael.** Nenhuma linha de codigo ate la.
