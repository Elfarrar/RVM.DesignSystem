---
id: DSGN-002
titulo: Camada de tokens e tematizacao clara/escura
repo: RVM.DesignSystem
tipo: feature
status: concluido no dev — espera o screenshot aprovado pelo Rafael
criada: 2026-09-16
---

# DSGN-002 — Tokens e tematizacao

Primeiro passo da onda 1 (`09-roadmap.md`). **A ordem importa:** componente antes de token vira hex
solto no CSS, e ai o tema escuro nao fecha. Os nove componentes da onda 1 sao a `DSGN-003` em diante.

⚠️ Branch tirada de `dev`, e nao de `master` como manda o `CLAUDE.md`: `master` ainda esta so com a
spec, porque a promocao para producao espera o sinal verde do Rafael. Volta ao normal quando prod subir.

## Feito

- [x] `wwwroot/rvm-design-system.css` — paleta (6 papeis x 5 variacoes), superficies, texto, acoes,
      fundos suaves, 24 estilos de tipografia, espacamento base 4, raios, 5 elevacoes, foco e movimento
- [x] Tema escuro redefinindo **superficie e estado**; a cor de marca nao muda entre temas
- [x] Fonte **Inter** variavel (300-500) servida pelo pacote: `latin` + `latin-ext` + `OFL.txt`
- [x] `RvmThemeProvider` + enum `RvmTheme`, com `@bind-Theme`, `ToggleAsync` e persistencia opcional
- [x] `rvm-theme.js` (persistencia e pintura do `<html>`) + snippet inline no `index.html` para o
      tema entrar **antes da primeira pintura**
- [x] Site: seletor de tema no topo, pagina **Fundamentos** renderizando os tokens, chrome do site
      reescrito **sem um hex sequer** — so `var(--rvm-...)`
- [x] 15 testes bUnit no provider — **cobertura 97.4%** medida no CI
- [x] `--rvm-color-<papel>-text`: a cor do papel usada COMO TEXTO, por tema
- [x] E2E **10/10** contra o site publicado, com axe nos dois temas
- [x] Pacote **`0.1.0-alpha.6`** conferido no feed do BaGet

## Decisoes tomadas aqui (e por que)

| Decisao | Motivo |
|---|---|
| **Texto do tema escuro = `#E7E3FC` com alpha 0.87 / 0.68 / 0.38** | ⭐ **Resolve a pendencia 2 do `09-roadmap`.** Nao e chute: `#E7E3FC` a 0.87 sobre o papel `#312D4B` da **exatamente** o `#CFCBE5` medido no PNG. O kit e Material (Materio), e sao os alphas dele — o bitmap tinha perdido so a opacidade |
| **Texto do tema claro: secundario por alpha, primario e desabilitado medidos** | O secundario (`#3A3541` a 0.68) e o desabilitado (0.38) reproduzem **exatamente** os swatches medidos. O primario do kit **nao** segue esse modelo — ver a linha abaixo |
| **Secundario claro subiu de 0.68 para 0.72** | 0.68 da 4.46:1 no papel e 4.32:1 no corpo: reprova o portao de 4.5:1. 0.72 da 5.01 e 4.79 — a **menor** mudanca que passa nos dois fundos |
| **`-contrast` e branco so no primary; os outros cinco usam `#212121`** | O kit manda branco nos seis, mas branco reprova AA em cinco (info 2.40, success 2.13, warning 1.78, error 3.28, secondary 3.33). Contraste e criterio de aceite (RNF-02) |
| **Fundos suaves mantidos ROXOS no tema claro** | ⏳ A spec reserva esta escolha ao Rafael (pendencia 3). Default = **fidelidade ao kit**, porque o criterio de aceite e o screenshot lado a lado com o PNG. Harmonizar com o azul e mudar uma linha de `--rvm-color-primary-soft` |
| **`--rvm-color-<papel>-text`, uma variante por tema** | O `-main` NAO serve para escrever: no claro reprova AA em cinco dos seis papeis (warning 1.55:1), e no escuro o primary da **1.84:1** — link ilegivel. Os valores saem das variantes do proprio kit (`alt-dark` no claro, `alt-light` no escuro), ajustadas so onde nem elas alcancam 4.5:1 |
| **Texto primario do tema claro = `#676C74` medido, nao `#544F5A` do modelo** | Amostrando o PNG: o secundario e o desabilitado batem com `#3A3541` a 0.68 e 0.38 **exatamente**, mas o primario do kit **nao segue o proprio modelo**. O pixel manda, porque a fidelidade e conferida contra o PNG — e ele passa AA (5.28:1) |
| **`data-theme` no elemento do provider, nao so no `<html>`** | Faz o tema certo sair na **primeira renderizacao, sem JS** — a regra dos dois modos de hospedagem. O JS so persiste e pinta o `<html>` |
| **Tokens moram em `wwwroot/`, nao em `Tokens/`** | RCL so serve o que esta em `wwwroot/`, e nao ha passo de build para copiar; duas copias sairiam de sincronia. `Tokens/LEIA-ME.md` explica |

## Armadilhas que esta task pagou

1. **Comentario Razor entre atributos derruba a pagina inteira.** `@* ... *@` dentro da lista de
   atributos de um elemento **compila sem reclamar** e vira `setAttribute('@* ... *@')` no
   navegador: `InvalidCharacterError`, componente nao renderiza, pagina em branco. Passou pelo
   build, pelos testes e pelo deploy — quem pegou foi abrir o site no navegador.
2. **`aria-pressed="@(bool)"` nao funciona.** O Blazor **omite** o atributo quando o valor e
   `false` e emite `aria-pressed=""` quando e `true`. Atributo `aria-*` quer `"true"`/`"false"` em
   texto. Vale para todo componente da onda 1 que tenha estado.
3. **Filtro de cobertura apagava os metodos async.** `ExcludeByAttribute` com
   `CompilerGeneratedAttribute` exclui a maquina de estado dos metodos `async` — o relatorio saiu
   **"100% (8 linhas)"** com o provider inteiro de fora. Numero verde que nao media nada.
   Sem o filtro: 97.4% de 39 linhas.
4. **Divisor e contorno nao sao fundo de texto.** A pagina os pintava como card com texto em cima,
   par que nao existe no uso real — e o axe reprovou (4.25:1). Viraram amostra de LINHA.
5. ⚠️ **O merge do PR #6 nao disparou os workflows de push** (CI, deploy e publish). Nao houve
   erro: simplesmente nenhum run foi criado para o commit de merge. Resolvido com
   `gh workflow run <arquivo> --ref dev`. Se repetir, conferir antes de concluir que "o deploy
   passou" — o run anterior continua verde na lista e engana.

## Pendente

- [ ] ⏳ Elevacoes: as 5 sao **aproximacao visual**; o kit mostra 24 quadrados sem escrever valores
- [ ] Guarda automatica contra hex literal em CSS de componente — entra junto com o primeiro
      componente (`DSGN-003`), quando houver `Components/**/*.razor.css` para varrer

## Verificado

1. ✅ `dotnet build -c Release` 0 erro e 0 warning · `dotnet test` 15/15
2. ✅ Cobertura **97.4%** (portao de 80%), medida no CI
3. ✅ Troca de tema sem recarregar, conferida no navegador nos dois temas
4. ✅ axe sem violacao seria nas duas paginas, nos dois temas (E2E 10/10 no site publicado)
5. ⏳ Screenshot lado a lado com `Theme/Light.png` e `Theme/Dark.png` — **falta a aprovacao do Rafael**
