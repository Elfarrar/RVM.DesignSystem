---
id: DSGN-006
titulo: Onda 4 — dados e shell (8 componentes)
repo: RVM.DesignSystem
tipo: feature
status: em andamento
criada: 2026-09-17
---

# DSGN-006 — Onda 4: dados e shell

Os oito da onda 4 do `11-catalogo-de-componentes.md`: `RvmTable`, `RvmDataGrid`, `RvmDatePicker`,
`RvmTimePicker`, `RvmStepper`, `RvmTimeline`, `RvmAppShell`, `RvmRating`. Com ela fecham as quatro
ondas — o criterio do `1.0.0` (`09-roadmap.md`), que so sai com sinal verde do Rafael.

Vale tudo o que as ondas 1 a 3 pagaram (cards `DSGN-003` a `DSGN-005`): medir no PNG antes de escrever
CSS; `-text` para escrever ou indicar com cor; prefixo `rvm-` em toda classe interna (guardado por
teste); numero em atributo na cultura invariante; nada de `RenderTreeBuilder.OpenElement` com CSS
isolado; subir o site local e rodar axe + E2E antes do PR, inclusive com o que abre aberto; portao pelo
exit code; arquivo novo pela ferramenta Write.

## Fatias (por dependencia e por risco)

1. **Exibicao**: `RvmRating`, `RvmStepper`, `RvmTimeline`
2. **Escolha de data e hora**: `RvmDatePicker` (data e intervalo), `RvmTimePicker` (12 h e 24 h)
3. **Dados**: `RvmTable` (basica, ordenavel, com selecao, densa), `RvmDataGrid` (paginacao, ordenacao,
   filtro por coluna)
4. **Shell**: `RvmAppShell` (topo + menu lateral + conteudo, responsivo, menu recolhido)

## Andamento

- [x] **Fatia 1**: `RvmRating`, `RvmStepper` (+ `RvmStep`), `RvmTimeline` + `RvmTimelineItem`
- [x] **Fatia 2**: `RvmCalendar`, `RvmDatePicker`, `RvmDateRangePicker`, `RvmTimePicker`

## Decisoes e medicoes

| O que | Por que |
|---|---|
| **Medidas da fatia 1**: estrelas com passo de 18/24/30 px e vazia no tom do contorno; marcador de etapa de 20 px, numero grande, conector de 4 px; ponto da linha do tempo de 16 px e 70 px entre itens | Varredura em `Rating.png`, `Stepper.png` e `Timeline.png` |
| **Avaliacao editavel = radios NATIVOS invisiveis sobre as estrelas** (fieldset + legend) | Setas, Tab e envio de formulario de graca, sem JS. Cada radio cobre o trecho da nota que representa (meia estrela com `AllowHalf`). Somente leitura vira `role="img"` com a nota por extenso ("Nota: 3,5 de 5") |
| **Estrela cheia, marcadores de etapa e pontos da linha do tempo no `-text` das cores** | Sem texto junto, sao o proprio indicador e precisam de 3:1 contra o fundo; o amarelo `-main` do kit da 1.7:1 no claro |
| **Etapa diz o estado em texto** (concluida, etapa atual, pendente, com erro) e a atual leva `aria-current="step"` | Check e cor sao desenho. "Com validacao" do catalogo = `HasError` na etapa |
| **Numero da etapa pendente no tom secundario, nao no desabilitado** | O axe reprovou o desabilitado (texto visivel, mesmo com `aria-hidden`) |
| **Estrela desenhada no proprio componente (star-filled do Tabler), nao pelo `RvmIcon`** | O catalogo de icones tem so a estrela em contorno; a avaliacao precisa da cheia e de recorte parcial |
| **Medidas da fatia 2**: calendario de 310 px no papel, colunas de 42 px, linhas de 38 px, dia escolhido num circulo de 36 px | Varredura em `Date & TIme Picker.png` |
| **`RvmCalendar` publico, usavel solto; `RvmDatePicker`/`RvmDateRangePicker` abrem ele num dialogo** | Padrao "Date Picker Dialog" do APG: `role="grid"`, tabindex movel, setas, Home/End, Page Up/Down (Shift = ano), Esc fecha, foco preso e devolvido pela `Sobreposicao` da onda 3 |
| **Campo do seletor de data e um BOTAO com o valor no nome acessivel** (`aria-labelledby` = rotulo + valor), so no estilo contorno | Popup e dialogo, nao listbox. `aria-required` nao vale em botao: o obrigatorio vai em texto so para leitor de tela |
| **`RvmTimePicker` = lista de horarios sobre o `RvmSelect`**; o relogio circular do kit ficou de fora | Lista e melhor pelo teclado e pelo leitor de tela; campo, busca, teclado e validacao vem prontos. 24 h e 12 h, `Step`, `Min`/`Max` |
| **Nomes de mes e dia da semana fixos em PT-BR, datas formatadas a mao** (`dd/mm/aaaa`; envio em `yyyy-MM-dd`) | Nao depender da cultura do servidor ou do navegador |
| **Ajudante `CampoDoFormulario` extraido do select** | Seletor de data e select acham o campo do EditForm pela expressao do `@bind` do mesmo jeito |
| 🔴 **Armadilha de foco deixava o Tab escapar do dialogo** | O `rvm-sobreposicao.js` contava botoes com `tabindex=-1` (dias nao focados) como focaveis: o "ultimo" virava o dia 31. Filtra `tabIndex >= 0`. Pego no E2E; vale para qualquer dialogo com tabindex movel dentro |
| 🔴 **Clicar no meio do select vazio nao abria a lista** (onda 2) | O rotulo fica por cima do combobox e so dava foco. Agora ele deixa o clique passar enquanto esta dentro do campo e abre a lista quando flutua. Pego no E2E do seletor de horario |
| 🔴 **Foco do calendario ia para o dia errado ao trocar de mes** | Sem `@key`, o Blazor reaproveitava o botao pela posicao e nao recapturava o `@ref`. `@key` com a data em cada celula. Pego no E2E; o teste manual passou por sorte (mesma celula) |

## Verifica (cada fatia)

1. Build Release 0 warning · testes verdes pelo exit code · cobertura >= 80%
2. Pagina no site com exemplo, parametros e recorte do kit
3. axe local nos dois temas, inclusive com o que abre aberto; E2E local e no dev
4. Teclado: o padrao ARIA de cada componente, testado de verdade
5. Comparacao lado a lado com o PNG do kit, aprovada pelo Rafael
