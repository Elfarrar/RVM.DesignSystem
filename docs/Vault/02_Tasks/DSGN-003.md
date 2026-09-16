---
id: DSGN-003
titulo: Onda 1 — os nove componentes da fundacao
repo: RVM.DesignSystem
tipo: feature
status: em andamento
criada: 2026-09-16
---

# DSGN-003 — Onda 1: os nove componentes

Segunda metade da onda 1 do `09-roadmap.md`. A camada de tokens saiu na `DSGN-002`; agora vem o que
consome ela. **Nenhum componente escreve cor**: so `var(--rvm-...)`.

Os nove, do `11-catalogo-de-componentes.md`: `RvmButton`, `RvmIcon`, `RvmTextField`, `RvmCard`,
`RvmTypography`, `RvmAlert`, `RvmChip`, `RvmAvatar`, `RvmDivider`.

## Ordem de construcao (dependencia, nao a do catalogo)

1. **Base**: enums compartilhados (`RvmColor`, `RvmSize`), `RvmTypography`, `RvmDivider`
2. **Icone**: `RvmIcon` com os SVG do Tabler copiados para dentro do repo
3. **Acao**: `RvmButton` (depende do icone, para o slot de icone e o estado carregando)
4. **Conteudo**: `RvmAvatar`, `RvmChip`, `RvmAlert`, `RvmCard`
5. **Formulario**: `RvmTextField` — o que tem mais armadilha, entra por ultimo

Cada fatia entra por PR proprio, verde de ponta a ponta. Fatiar nao e preferencia: PR de nove
componentes ninguem revisa, e o primeiro erro de padrao se repetiria nove vezes.

## Contrato de cada componente (`05-api-dos-componentes.md`)

- Prefixo `Rvm`, API em ingles, texto ao usuario final em PT-BR
- Enum, nunca string magica · `AdditionalAttributes` repassado ao elemento raiz
- Parametro novo e opcional, com default que preserva o comportamento
- Todo interativo: teclado, foco visivel, e os estados normal, hover, foco, ativo, desabilitado,
  erro e carregando
- **Componente de formulario renderiza `name`, `id` e `aria-*`** — o `RvmTextField` anterior nao
  renderizava `name`, e o form em SSR estatico ficou sem binding do POST
- CSS isolado (`.razor.css`) consumindo **so** custom properties

## Divida herdada da DSGN-002, que fecha aqui

- [ ] Guarda automatica contra **hex literal** em `Components/**/*.razor.css`
- [ ] Devolver o `<link>` do bundle de CSS isolado ao `index.html` do site (saiu na `DSGN-002`
      porque nao existia componente com `.razor.css`; **sem ele nenhum componente tem estilo**)
- [ ] ⚠️ Texto secundario e desabilitado foram calibrados contra `paper` e `body`. Componente que
      puser texto secundario sobre `-soft` ou `-outlined-*` **mede de novo** — sao tokens com alpha

## Verifica (cada fatia)

1. `dotnet build -c Release` 0 erro e 0 warning · `dotnet test` verde · cobertura >= 80%
2. Pagina do componente no site, com exemplo vivo, variacoes, estados, tabela de parametros e o
   **recorte do PNG do kit ao lado** (`07-site-de-documentacao.md`)
3. axe sem violacao seria, nos dois temas
4. Teclado: alcancar, ativar e enxergar o foco
5. Screenshot lado a lado com o PNG do kit, aprovado pelo Rafael
