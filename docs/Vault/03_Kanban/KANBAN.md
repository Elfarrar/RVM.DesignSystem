# Kanban — RVM.DesignSystem

> **Este arquivo e INDICE: uma linha por card.** Contexto, plano e decisao vivem no card
> (`02_Tasks/DSGN-NNN.md`), nunca aqui. **Alvo: cabe em duas telas.**
>
> **O card nasce ANTES do codigo**, com o escopo e as decisoes em aberto.
>
> Modo de trabalho: **Claude implementa, o Rafael revisa** (`CLAUDE.md § Modo de trabalho`).

## Em revisao

- [[DSGN-001]] — Bootstrap: repo publico, esqueleto .NET 10, 5 workflows, DNS/SSL e o site no ar
  em `design.dev.rvmtech.com.br`. **Producao aguarda sinal verde do Rafael.**
- [[DSGN-002]] — Camada de tokens, `RvmTheme.FromSeed` e os dois portoes. 62 testes; o portao de
  contraste cobre 5 temas x 2 modos x 14 pares e reprova entrada ruim de proposito.

## A fazer

### Onda 1 — Tokens e basicos

Meta: um formulario completo, coerente e acessivel, montado so com componentes `Rvm*`.
Fecha com o pacote `0.1.0` no BaGet e o site publicando os fundamentos.

- ~~Camada de tokens (cor, tipografia, espacamento, raio, sombra, motion, z-index, breakpoint)~~ [[DSGN-002]]
- ~~`RvmTheme` + `FromSeed`~~ [[DSGN-002]] · falta `RvmThemeProvider` + script anti-flash
- ~~Teste de contraste de toda paleta — o portao do `06`~~ [[DSGN-002]]
- `RvmButton`, `RvmIconButton`, `RvmTextField`, `RvmTextArea`, `RvmSelect`, `RvmCheckbox`,
  `RvmRadioGroup`, `RvmSwitch`, `RvmLabel`/`RvmFormField`
- Site: casca, fundamentos, pagina por componente, codigo copiavel, instalacao, changelog

> ⏳ **As tres primeiras pendencias do `09-roadmap` travam o fechamento da onda 1**: idioma de
> token/API, conjunto de icones e escala tipografica. Depois da 1.0, mudar qualquer uma quebra
> todo consumidor.

### Ondas 2 a 4

Layout, Feedback e Dados — escopo e criterio de saida em `09-roadmap.md`. Cards saem quando a
onda anterior fechar.

## Concluido

_(vazio — a DSGN-001 fecha quando o Rafael aprovar)_
