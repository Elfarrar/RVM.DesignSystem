---
id: DSGN-002
titulo: Camada de tokens e tematizacao clara/escura
repo: RVM.DesignSystem
tipo: feature
status: em andamento
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
- [x] 15 testes bUnit no provider

## Decisoes tomadas aqui (e por que)

| Decisao | Motivo |
|---|---|
| **Texto do tema escuro = `#E7E3FC` com alpha 0.87 / 0.68 / 0.38** | ⭐ **Resolve a pendencia 2 do `09-roadmap`.** Nao e chute: `#E7E3FC` a 0.87 sobre o papel `#312D4B` da **exatamente** o `#CFCBE5` medido no PNG. O kit e Material (Materio), e sao os alphas dele — o bitmap tinha perdido so a opacidade |
| **Texto do tema claro = `#3A3541` com alpha 0.87 / 0.72 / 0.38** | Mesma estrutura: 0.68 e 0.38 sobre branco reproduzem **exatamente** o secundario e o desabilitado medidos. O "primario" que estava medido como `#676C74` era swatch trocado — o certo e `#544F5A` (0.87) |
| **Secundario claro subiu de 0.68 para 0.72** | 0.68 da 4.46:1 no papel e 4.32:1 no corpo: reprova o portao de 4.5:1. 0.72 da 5.01 e 4.79 — a **menor** mudanca que passa nos dois fundos |
| **`-contrast` e branco so no primary; os outros cinco usam `#212121`** | O kit manda branco nos seis, mas branco reprova AA em cinco (info 2.40, success 2.13, warning 1.78, error 3.28, secondary 3.33). Contraste e criterio de aceite (RNF-02) |
| **Fundos suaves mantidos ROXOS no tema claro** | ⏳ A spec reserva esta escolha ao Rafael (pendencia 3). Default = **fidelidade ao kit**, porque o criterio de aceite e o screenshot lado a lado com o PNG. Harmonizar com o azul e mudar uma linha de `--rvm-color-primary-soft` |
| **`data-theme` no elemento do provider, nao so no `<html>`** | Faz o tema certo sair na **primeira renderizacao, sem JS** — a regra dos dois modos de hospedagem. O JS so persiste e pinta o `<html>` |
| **Tokens moram em `wwwroot/`, nao em `Tokens/`** | RCL so serve o que esta em `wwwroot/`, e nao ha passo de build para copiar; duas copias sairiam de sincronia. `Tokens/LEIA-ME.md` explica |

## Pendente

- [ ] ⏳ Elevacoes: as 5 sao **aproximacao visual**; o kit mostra 24 quadrados sem escrever valores
- [ ] Guarda automatica contra hex literal em CSS de componente — entra junto com o primeiro
      componente (`DSGN-003`), quando houver `Components/**/*.razor.css` para varrer

## Verifica

1. `dotnet build -c Release` 0 erro e 0 warning · `dotnet test` verde
2. **Cobertura >= 80%** — o portao passa a valer de verdade a partir desta task
3. Troca de tema sem recarregar, conferida no navegador nos dois temas
4. axe sem violacao seria nas duas paginas, nos dois temas
5. Screenshot lado a lado com `referencia-neatlab/Theme/Light.png` e `Dark.png`, aprovado pelo Rafael
