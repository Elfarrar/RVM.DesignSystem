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

## Andamento

- [x] **Fatia 1** (PR #10, #11): `RvmTypography`, `RvmDivider`, enums compartilhados, paginas de
      componente (exemplo + codigo + parametros + recorte do kit)
- [x] **Fatia 2**: `RvmIcon` (42 icones Tabler curados) e `RvmButton`
- [ ] Fatia 3: `RvmAvatar`, `RvmChip`, `RvmAlert`, `RvmCard`
- [ ] Fatia 4: `RvmTextField`

## Decisoes e medicoes desta task

| O que | Por que |
|---|---|
| **`RvmTypography` em C# puro, sem `.razor.css`** | A tag e dinamica e o Razor nao tem sintaxe para isso. O estilo dele E a escala tipografica, que ja mora nos tokens como `rvm-text-*` |
| **`Border.png` do catalogo NAO e pagina de divisor** | E uma tela de dashboard. A referencia do `RvmDivider` e o divisor dentro dela; corrigido no `11-catalogo` |
| **Icones: 42 Tabler curados, gerados para enum + catalogo** | Enum: nome errado nao compila. O conjunto e o que os 35 componentes precisam, nao o Tabler inteiro. Teste cobra desenho para todo nome do enum |
| **Botao medido no `Button.png`: 30 / 38 / 42 px, raio 6, recuo 14 / 22 / 26, caixa alta** | Renderizado e conferido no navegador: bate nos tres tamanhos |
| **Borda do outlined = `-outlined-resting`, nao `-main`** | Medido no kit: primary `#626B9C` contra o token `#646D9F`, secondary `#5C5B6E` contra `#5E5D6F`. Com `-main` ficava saturada demais |
| **Anel de foco = `-text`, nao `-main`** | ⚠️ O axe nao mede isto. No tema escuro o `primary-main` dava **1.84:1** sobre o papel — o anel sumia para quem navega por teclado (WCAG 2.4.11 pede 3:1). Com `-text`: 7.12 no claro, 6.38 no escuro. Virou teste E2E |
| **Botao `Type` padrao = `Button`, nao `submit`** | O padrao do HTML e `submit`: um "cancelar" dentro de formulario enviaria o form |
| **Clique ignorado em `Disabled` e `Loading` tambem no C#** | O `disabled` do elemento barra o navegador, mas nao chamada programatica — clique que escapa enquanto carrega vira requisicao duplicada |

## Divida herdada da DSGN-002

- [x] Guarda automatica contra **hex literal** em `Components/**/*.razor.css` — teste
- [x] `<link>` do bundle de CSS isolado de volta ao `index.html`
- [ ] ⚠️ Texto secundario e desabilitado foram calibrados contra `paper` e `body`. Componente que
      puser texto secundario sobre `-soft` ou `-outlined-*` **mede de novo** — sao tokens com alpha

## Verifica (cada fatia)

1. `dotnet build -c Release` 0 erro e 0 warning · `dotnet test` verde · cobertura >= 80%
2. Pagina do componente no site, com exemplo vivo, variacoes, estados, tabela de parametros e o
   **recorte do PNG do kit ao lado** (`07-site-de-documentacao.md`)
3. axe sem violacao seria, nos dois temas
4. Teclado: alcancar, ativar e enxergar o foco
5. Screenshot lado a lado com o PNG do kit, aprovado pelo Rafael
