---
id: DSGN-012
titulo: Onda 1 — nove componentes, motor de tema e site de documentacao
repo: RVM.DesignSystem
tipo: feature
status: concluido
criada: 2026-09-08
---

# DSGN-012 a DSGN-015 — Onda 1

Fecha a onda 1 do `09-roadmap.md` e publica a **`0.1.0`**. Card unico para as quatro tasks,
porque foram uma entrega so: componentes (`DSGN-012`), site de documentacao (`DSGN-013`),
correcao do XML em build limpo (`DSGN-014`) e preparacao da release (`DSGN-015`).

## Criterio de saida (do `09-roadmap`)

> O site esta no ar com o seletor de tema funcionando nas quatro identidades; um formulario de
> exemplo passa na auditoria axe e e operavel so por teclado.

✅ Os dois, verificados contra `design.rvmit.com.br`.

## Entregue

- **Nove componentes**: `RvmButton`, `RvmIconButton`, `RvmTextField`, `RvmTextArea`,
  `RvmSelect`, `RvmCheckbox`, `RvmRadioGroup`, `RvmSwitch`, `RvmFormField`
- **Motor de tema em runtime**: `RvmThemeProvider`, `IRvmThemeService`, script anti-flash
- **Icones Phosphor**, pesos `Regular` e `Fill`, gerados por `tools/gerar-icones.py`
- **Site de documentacao**: 16 paginas com anatomia fixa, tabela de API gerada do XML,
  contraste medido ao vivo, e o seletor das cinco identidades

## Verificado

| | |
|---|---|
| Testes | 114, 0 aviso em `Release` |
| Portao de contraste | 26 pares x 5 temas x 2 modos |
| Portao de CSS | 10 arquivos de componente |
| axe em producao | 32 auditorias, zero violacao seria |
| Tabelas de API | 18/18 preenchidas |
| Teclado | 14 elementos tabulaveis, todos com anel de foco e nome acessivel |

## Seis defeitos que so apareceram rodando

Nenhum deles seria pego por teste unitario. Estao listados porque a licao de cada um vale
mais que o conserto.

### 1. O motor de tema media contra a superficie errada

`FromSeed` garantia contraste contra a `surface` — quase branca no modo claro. Mas papel de
marca e de estado tambem sao usados como **texto** sobre o `background` da pagina e o
`surface-sunken` de um campo, que sao mais escuros. `success` dava **4.45**, azul **4.23**:
reprovam AA, e o **teste unitario aprovava**.

Corrigido em tres frentes: a derivacao passou a mirar a superficie mais exigente da paleta;
`PairsToVerify` foi de 14 para **26 pares**; e nasceu `RvmPalette.EnsureContrast()` para quem
sobrescreve superficie — porque a **propria sobrescrita dos cinzas do tema RVM** tinha quebrado
a garantia em silencio.

**Consequencia de contrato:** o azul da marca sai `#0068B8`, nao o `#006DBD` nominal.

### 2. CSS isolation nao atravessa componente

`.rvm-checkbox__check` e `.rvm-input__icon` ficam em elementos do `RvmIcon` — outro componente,
outro escopo `b-*`. As regras nunca casavam: **a caixa desmarcada exibia o check** e o icone do
e-mail caia fora do campo. `::deep` resolve.

### 3. O desenho decorativo engolia o clique

Ficava na frente do input invisivel. `pointer-events: none`.

### 4. `opacity` derruba contraste

Ela compoe a cor com o fundo e afeta **tudo** que esta dentro — inclusive um `<code>` colorido,
que herda o efeito e nao a cor. Texto secundario se faz com **cor**.

### 5. `IsGenericType` e true para tipo aninhado em generico

`RvmRadioGroup<T>.Option` nao tem crase no nome; `IndexOf` devolvia -1 e o slice estourava.
**A pagina inteira quebrava.** Quem pegou foi a varredura ver o banner de erro do Blazor.

### 6. O XML de doc sumia em build limpo

Copiar com `AfterTargets="Build"` chega tarde: os static web assets ja foram enumerados. Em
maquina de desenvolvimento nao aparece, porque o arquivo sobrou do build anterior — **so quebra
no CI**. Duas ordenacoes de MSBuild nao resolveram; a saida foi commitar o artefato, com teste
que reprova se ficar velho.

## Um defeito de API do proprio Blazor

`InputBase<T>` **lanca excecao fora de um `EditForm`**, porque exige `ValueExpression`. Um campo
de busca numa barra de ferramentas quebraria. `RvmInputBase` sintetiza a expressao quando falta.

## O que fica para a onda 2

- `RvmAppShell`, `RvmSidebar`, `RvmCard`, `RvmTabs` e o resto do Layout
- ⏳ **Quais icones entram no sprite** — decisao de contrato que nao estava na lista das seis
- Auditoria axe **por pagina de componente** no E2E (hoje o E2E audita so a inicial)
