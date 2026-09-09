---
id: DSGN-031
titulo: O calendario tambem falava ingles — teste, e a pergunta de arquitetura que sobra
repo: RVM.DesignSystem
tipo: bug
status: todo
criada: 2026-09-09
atualizada: 2026-09-09
---

# DSGN-031 — A outra ponta do DSGN-030

## Descricao

O review independente do `DSGN-030` apontou, e **eu confirmei em producao antes de aceitar**: o
defeito nao parava na moeda. Com o navegador em `en-US`, o calendario do `RvmDatePicker` sai
**inteiro em ingles** — `Su/Mo/Tu`, `Sunday/Monday`, `March 2026`.

| | cabecalho da semana |
|---|---|
| `design.rvmit.com.br` (sem o fix, 09/09 11:50) | `Su`, `Mo`, `Tu` / `Sunday`, `Monday` |
| `design.dev.rvmtech.com.br` (com o fix) | `dom.`, `seg.`, `ter.` / `domingo`, `segunda-feira` |

Mesma causa, mesmos dados que faltavam: `RvmDatePicker.razor:211-216` chama
`GetShortestDayName`, `GetDayName` e `ToString("MMMM 'de' yyyy", PtBr)` — tudo isso **exige** o
`DateTimeFormatInfo` do pt-BR.

⚠️ **E pior que o defeito da moeda**, ainda que menos visivel: a moeda troca a pontuacao e o
simbolo; o calendario troca o **idioma** de uma biblioteca que se declara pt-BR fixo.

A flag do `DSGN-030` ja conserta — nao ha codigo de produto a escrever. O que faltava era
**prova de que continua consertado**.

## Plano

1. **Teste E2E** com `Locale = "en-US"`: abrir o calendario e exigir que os sete dias da semana
   estejam em portugues, pelo atributo `abbr` do `<th>` (o nome completo, que o leitor de tela
   le), e que o titulo do mes seja um dos doze nomes em portugues.

   Por que pelo `abbr` e nao pelo texto visivel: o `<th>` carrega duas coisas — `dom.` para os
   olhos e `domingo` para o leitor de tela. Comparar `textContent` acabaria comparando os dois
   colados e a asserção viraria refem do espacamento do markup.

   Por que nao comparar com o mes de hoje formatado: o navegador do teste e o runner podem
   estar em fusos diferentes, e a asserção quebraria sozinha na virada do mes. A lista dos doze
   nomes prova a mesma coisa sem essa fragilidade.

2. ⏳ **A decisao que fica aberta para o Rafael** (nao entra neste card sem ele dizer): a
   biblioteca deveria **parar de depender do ICU**? Hoje `BrlFormatter` e `RvmDatePicker` pedem
   `CultureInfo.GetCultureInfo("pt-BR")` e, com isso, **exigem** que todo consumidor WASM ligue
   `BlazorWebAssemblyLoadAllGlobalizationData`. Isso hoje e so documentacao — e quem esqueceu a
   flag da primeira vez foi este proprio repositorio, no proprio site.

   | | montar `NumberFormatInfo`/`DateTimeFormatInfo` proprios | continuar com o ICU |
   |---|---|---|
   | Consumidor WASM | funciona sem flag nenhuma, e ate com `InvariantGlobalization` | precisa lembrar da flag |
   | Payload | menor | +180 KB brotli |
   | Codigo | nomes de mes e dia escritos a mao na biblioteca | zero |
   | Risco | bug de implementacao nosso | bug de configuracao do consumidor |

   O projeto ja se comprometeu com **pt-BR fixo** (`CLAUDE.md` § Pendencias), o que enfraquece o
   argumento de "usar a cultura do sistema". Mas e mudanca dentro da `1.0.0` congelada e nao e
   urgente: a flag resolve hoje.

## Validacao

- [ ] O teste novo passa contra o `dev` (com o fix).
- [ ] O mesmo teste **falha** contra a producao enquanto ela nao for promovida — se passar nos
      dois, ele nao esta provando nada.
- [ ] Suite E2E completa verde.

## Versao

Nao mexe na biblioteca — nao gera versao nova do pacote.
