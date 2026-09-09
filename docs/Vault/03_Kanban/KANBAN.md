# Kanban — RVM.DesignSystem

> **Este arquivo e INDICE: uma linha por card.** Contexto, plano e decisao vivem no card
> (`02_Tasks/DSGN-NNN.md`), nunca aqui. **Alvo: cabe em duas telas.**
>
> **O card nasce ANTES do codigo**, com o escopo e as decisoes em aberto.
>
> Modo de trabalho: **Claude implementa, o Rafael revisa** (`CLAUDE.md § Modo de trabalho`).

## A decidir

- [[DSGN-027]] — **Aparencia "Admin" e a tela de Dashboard**, no desenho do AdminLTE: barra
  lateral escura, caixa de numero com marca d'agua, cartao com faixa no topo. ⚠️ As cores NAO
  sao as do AdminLTE — as small-box de la dao 2,4:1, e reprovariam no axe. A casca escura sai da
  paleta ESCURA do tema, que ja e um par medido.

- [[DSGN-026]] — **Aparencia "Vivo" e 164 icones**. Resposta ao "nao vi muita diferenca, quero
  mais cor, e pouco icone": o "vivo" tinge a superficie **pelo motor de tema** (o que a
  `DSGN-025` nao podia fazer em CSS sem furar o portao), e o conjunto de icones passa a ser
  curado **por aplicacao**, nao por componente. ⚠️ Revisa a regra de icones da onda 2, e a
  medicao derrubou o argumento de bytes que eu tinha usado para justifica-la.

- [[DSGN-025]] — **Preview de aparencia**: as duas direcoes ("sobrio" e "marcante") no ar ao
  mesmo tempo, trocaveis na topbar, porque gosto se decide olhando. Temporario — sai inteiro
  quando a direcao for escolhida. ⚠️ Nao mexe em cor de superficie de proposito: isso e motor de
  tema, nao folha de estilo, senao fura o portao de contraste em silencio.

- [[DSGN-022]] — **Graficos** (obras). A condicao do `09` foi satisfeita: um app RVM precisa.
  ⚠️ **Card aberto sem codigo**: o roadmap diz "embrulhar biblioteca existente" e o `CLAUDE.md`
  diz "sem dependencia NuGet de terceiros" e "nada depende de JS para renderizar" — as tres
  frases nao cabem juntas. Quatro saidas descritas no card; a decisao e do Rafael.

## Em revisao

_Nada em revisao._

## Concluido

- [[DSGN-024]] — ✅ **EM PRODUCAO** (08/09/2026): a paleta do visitante fica guardada no
  navegador. Achado pelo Rafael usando a `DSGN-023`: navegar e voltar deixava o site pintado e o
  formulario nos valores iniciais — dois lugares discordando sobre a mesma escolha. Guarda as
  cores **escolhidas**, nunca o tema derivado, para que uma paleta antiga passe pelas regras de
  contraste de hoje.

- [[DSGN-023]] — ✅ **EM PRODUCAO** (08/09/2026, autorizada pelo Rafael): a paleta em construcao
  repinta o site inteiro. E o unico jeito de ver que uma primaria vermelha faz "Salvar" parecer
  "Excluir" — as amostras de cor nao mostram isso. **Zero mudanca na biblioteca**: o mecanismo
  ja existia. Revelou e corrigiu um seletor de tema que **mentia** com tema fora da lista.

- [[DSGN-021]] — ✅ **EM PRODUCAO** (08/09/2026): cores de estado escolhiveis (`RvmSeed`,
  aditivo) e a ferramenta em `/fundamentos/paleta` que gera **o CSS pronto**.
  ⚠️ **A tag `v1.1.0` existe e aponta para `7013b46`, mas o pacote NAO esta no feed** — duas
  execucoes do push no BaGet falharam por rede. O site nao depende disso (compila por
  `ProjectReference`); quem depende e o primeiro consumidor. Ver [[DSGN-018]].

- [[DSGN-020]] — 🎉 **Onda 4 EM PRODUCAO e a `1.0.0` PUBLICADA** (08/09/2026, autorizada pelo
  Rafael): grid, paginacao, filtro, data e busca assincrona, mais a tela de Listagem com 487
  linhas. **Os 35 componentes da v1 estao entregues e o contrato esta congelado.** 260 testes.
- [[DSGN-019]] — **Onda 3 EM PRODUCAO** (08/09/2026, autorizada pelo Rafael): dialogo, toast,
  alerta, estado vazio, spinner, progresso, esqueleto e tooltip. Mais a secao Padroes e a busca
  do site. **`0.3.0` no BaGet** pela tag `v0.3.0`. 223 testes.
- [[DSGN-018]] — `--timeout` por tentativa no push do BaGet. Validado na pratica: o publish da
  `0.3.0` levou 21s, contra 25 min de uma tentativa pendurada na `0.2.0`.
- [[DSGN-017]] — **Onda 2 EM PRODUCAO** (08/09/2026, autorizada pelo Rafael): casca de
  aplicacao, layout, navegacao e densidade `Compact`. `design.rvmit.com.br` servindo o site
  montado com `RvmAppShell`. 187 testes, 29 pares de contraste, axe em 29 paginas x 2 modos.
- [[DSGN-001]] — Bootstrap + **producao no ar** (08/09/2026): repo publico, esqueleto .NET 10,
  5 workflows, DNS/SSL nos dois ambientes, `design.dev.rvmtech.com.br` e `design.rvmit.com.br`.
  ⏳ So o monitor no Uptime-Kuma ficou de fora — o Kuma nao tem canal de notificacao nenhum.
- [[DSGN-003]] — `CNAME` no artefato, borda do titulo removida e 404 do CSS isolado consertado.
- [[DSGN-004]] — `Verify` do primeiro deploy de Pages: o site subia certo e o job ficava vermelho.

## A fazer

### ✅ Onda 2 — fechada em 08/09/2026, sai como `0.2.0`

Doze componentes de layout e navegacao, densidade `Compact`, 8 icones novos e 13 paginas de
documentacao. O conjunto de icones — a decisao de contrato que sobrava da onda 1 — ficou
**curado**: 19 icones, cada um entrando junto do componente que o usa. Card: [[DSGN-017]].

### ✅ Onda 1 — fechada em 08/09/2026, publicada como `0.1.0`

Nove componentes, camada de tokens, motor de tema, icones Phosphor e o site de documentacao.
`design.rvmit.com.br` no ar. Cards: [[DSGN-002]], [[DSGN-012]], [[DSGN-013]], [[DSGN-014]],
[[DSGN-015]].

### ✅ Onda 3 — fechada em 08/09/2026, sai como `0.3.0`

Card: [[DSGN-019]].

### ✅ Onda 4 — fechada em 08/09/2026, sai como `1.0.0`

Card: [[DSGN-020]]. **Os 35 componentes da v1 estao entregues.**

<details>
<summary>Escopo original da onda 4, para referencia</summary>

### Onda 4 — Dados

`RvmDataGrid<T>` com colunas tipadas, ordenacao, paginacao e selecao, mais o resto de Dados.
Fecha em `0.4.0`. **E a onda cara** — cada linha dela vale, em esforco, varias das anteriores.

⚠️ **O que as ondas 2 e 3 deixam para ela:**
- A densidade alcanca os controles da onda 1 e o `RvmNavItem`. O componente de dado precisa
  entrar na conta dos tokens `--rvm-control-*`, ou nasce ignorando a densidade **em silencio**.
- Tabela e o lugar onde `aria-busy` + `RvmSkeleton` e `RvmEmptyState` (as quatro variantes)
  finalmente se combinam. O padrao ja esta escrito e demonstrado em `/padroes`.

</details>

### Depois da 1.0

**Nao ha onda 5.** O que vem e adocao, e ela e so em projeto novo (`09` § Adocao): ERPAgro,
ObraEmDia, Fiscal e Propostinha seguem no MudBlazor, sem prazo. O proximo projeto RVM que
precisar de UI nasce no design system, e e ele quem prova a biblioteca.

<details>
<summary>Escopo original da onda 1, para referencia</summary>

### Onda 1 — Tokens e basicos

Meta: um formulario completo, coerente e acessivel, montado so com componentes `Rvm*`.
Fecha com o pacote `0.1.0` no BaGet e o site publicando os fundamentos.

- ~~Camada de tokens (cor, tipografia, espacamento, raio, sombra, motion, z-index, breakpoint)~~ [[DSGN-002]]
- ~~`RvmTheme` + `FromSeed`~~ [[DSGN-002]] · falta `RvmThemeProvider` + script anti-flash
- ~~Teste de contraste de toda paleta — o portao do `06`~~ [[DSGN-002]]
- `RvmButton`, `RvmIconButton`, `RvmTextField`, `RvmTextArea`, `RvmSelect`, `RvmCheckbox`,
  `RvmRadioGroup`, `RvmSwitch`, `RvmLabel`/`RvmFormField`
- Site: casca, fundamentos, pagina por componente, codigo copiavel, instalacao, changelog

✅ Todas as pendencias de contrato do `09-roadmap` foram fechadas antes da release.

</details>

### Ondas 2 a 4

Layout, Feedback e Dados — escopo e criterio de saida em `09-roadmap.md`. Cards saem quando a
onda anterior fechar.


