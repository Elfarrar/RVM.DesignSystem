---
id: DSGN-023
titulo: A paleta em construcao repinta o site inteiro
repo: RVM.DesignSystem
tipo: feature
status: concluido
criada: 2026-09-08
---

# DSGN-023 — O site como preview da paleta

Pedido do Rafael em 08/09/2026: *"quando o usuario mudasse as cores da paleta mudaria tbm o
site de design"*.

## Por que isso nao e enfeite

A `DSGN-021` entregou a ferramenta de paleta mostrando **doze quadradinhos** de amostra. Isso
prova que as cores existem e que passam no contraste — e nao prova a unica coisa que importa
antes de adotar uma paleta: **como ela se comporta numa tela de verdade.**

O primeiro uso da ferramenta ja demonstrou a diferenca. Com a primaria em `#B3261E`, as amostras
ficam impecaveis; a pagina do `RvmButton` mostra um **"Salvar" que parece um botao de excluir**.
E exatamente o defeito que o aviso de colisao da `DSGN-021` descreve por escrito — mas ler o
aviso e ver o botao sao coisas diferentes.

## O que foi feito

Nada na biblioteca. **O mecanismo ja existia inteiro** e essa foi a descoberta que definiu o
tamanho da task:

- `RvmThemeProvider` emite `<style>Theme.ToCss()</style>` e re-renderiza no evento `Changed`
- `IRvmThemeService.SetThemeAsync` troca o tema em runtime — o seletor da topbar ja usava

Faltava a pagina chamar. As mudancas sao todas do **site**, nenhuma do pacote:

| Arquivo | O que mudou |
|---|---|
| `PaletaPage.razor` | espelha o tema no site, avisa, e oferece a volta |
| `MainLayout.razor` | o seletor da topbar passa a admitir a paleta do visitante |
| `doc.css` | o aviso |
| `SiteSmokeTests.cs` | o teste E2E que fecha o circuito |

## As tres decisoes

### 1. Liga na primeira mudanca, nao ao abrir

As cores iniciais do formulario **nao sao as do site** (o padrao ali e a paleta do ObraEmDia).
Aplicar ao montar a pagina faria a documentacao inteira trocar de cor sozinha, sem ninguem ter
pedido. O espelho arma quando a pessoa mexe numa cor — que e quando ela quer ver o efeito.

Mexer no **nome** do tema nao arma; mexer no switch das cores de feedback arma, porque
liga-lo muda as cores.

### 2. Aplicar depois do render, e nao no manipulador de cada campo

`OnAfterRenderAsync` compara `Site.Theme != Tema` e so entao aplica. Resolve tres coisas juntas:

- **Sem fogo-e-esquece.** `Definir` recebe o campo por `ref`, e C# proibe `ref` em `async`.
- **Sem laco.** `RvmTheme` e record — a comparacao e por valor. Aplicar dispara `Changed`, que
  re-renderiza, e na segunda passada os temas ja sao iguais. Com igualdade por referencia isso
  giraria para sempre.
- **Uma escrita por rajada.** Arrastar o seletor de cor dispara varias mudancas; chega ao site o
  estado final de cada render.

### 3. Quem manda e o ultimo comando

Os dois controles brigariam: escolher "RVM" na topbar, e a pagina reaplicaria a paleta do
visitante no render seguinte — o seletor pareceria quebrado. A pagina assina o `Changed` e
**desarma o espelho quando o tema muda por fora**.

## ⚠️ O defeito que a mudanca revelou no seletor da topbar

O seletor marcava a opcao **pelo nome** e listava so `RvmThemes.All`. Com um tema fora da lista
aplicado, nenhuma opcao casa — e o navegador seleciona a primeira. **O menu diria "RVM" com o
site em vermelho.**

E o mesmo defeito do `RvmSelect` na onda 1, pela mesma causa, e nao teria aparecido sem esta
task: ate agora todo tema aplicavel estava na lista.

Corrigido em duas frentes:

- a lista passa a incluir a paleta do visitante, rotulada `(sua paleta)`
- o `value` da opcao e o **indice**, nao o nome — nada impede alguem de batizar a paleta de
  "RVM", e por nome escolher a dela devolveria a embutida, **descartando o trabalho em silencio**

A comparacao usa igualdade de record: uma paleta identica a uma embutida nao aparece duas vezes,
porque nao e outro tema.

## Verificado

| | |
|---|---|
| Build | 0 erro, 0 aviso em `Release` |
| Testes | 282, todos passando (nenhum novo — a mudanca e do site, e o projeto unitario so referencia a biblioteca) |
| E2E novo | `Mudar_a_cor_na_pagina_de_paleta_repinta_o_site_inteiro` |
| Screenshot | pagina de paleta, `RvmButton` na paleta do visitante, e o modo escuro |

O teste E2E mede a custom property **calculada** no elemento raiz, nao o que a pagina desenhou
em si mesma: as amostras saem de estilo inline e continuariam certas mesmo se o tema nunca fosse
aplicado — seria um verde falso. Ele confere que o valor aplicado bate com a amostra `primary`
da propria pagina (a cor **derivada**, nao a digitada), que o seletor da topbar admite a paleta,
que o **axe continua limpo com uma paleta de fora**, e que da para voltar.

## O que continua fora, de proposito

**Tema por usuario final em runtime (white-label por tenant)** segue fora do escopo
(`09` § Fora da v1). Esta e uma ferramenta de **documentacao**, para quem constroi um produto
escolher a paleta dele uma vez e levar o CSS embora. O tema nao e persistido: recarregar a
pagina devolve o site ao tema RVM.

## Nota

`PaletaPage.razor` tinha um `@inject IJSRuntime JS` **nunca usado**, sobra minha da `DSGN-021`.
Removido junto, por ser codigo meu da mesma feature.
