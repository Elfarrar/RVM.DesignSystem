---
id: DSGN-003
titulo: Onda 1 — os nove componentes da fundacao
repo: RVM.DesignSystem
tipo: feature
status: concluido
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
- [x] **Fatia 2** (PR #12): `RvmIcon` (42 icones Tabler curados) e `RvmButton`
- [x] **Fatia 3** (PR #13): `RvmAvatar` + `RvmAvatarGroup`, `RvmChip`, `RvmAlert`, `RvmCard`
- [x] **Fatia 4**: `RvmTextField` — os nove da onda 1 prontos

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
| **Avatar 24 / 40 / 56 px, chip 32 / 24 px, alerta 48 px, raio 6 px em todos** | Medidos por varredura de linha nos PNGs (flood fill falhou em forma antialiasada pequena). O avatar pequeno mede 25 com o antialias do circulo |
| **Card: recuo de 20 px, nao 40** | A primeira medicao automatica deu 40 e estava errada; conferido com regua sobre o recorte ampliado 3x |
| **Borda do ALERTA outlined = `-main` cheio; do BOTAO = `-outlined-resting`** | Medido nos dois PNGs: alerta de erro `#F74B51` (≈ `-main`), botao primary `#626B9C` (≈ resting). O proprio kit trata os dois diferente |
| **`-text` sobre `-soft` medido ANTES de construir** | Divida da DSGN-002: Chip suave, Avatar suave e Alerta standard poem texto colorido no fundo suave. Os 12 pares passam; o pior e info no claro, 4.71:1 |
| **Alerta: `role=alert` so para erro e aviso** | Informacao e sucesso sao `status`. Tudo como `alert` interromperia o leitor de tela a cada "salvo com sucesso" |
| **Alerta tem as 6 cores do `RvmColor`, o kit desenha 4** | O catalogo pede 6 papeis e o contrato do enum da isso de graca; as 4 do kit sao as documentadas como referencia |
| **Chip: `.chip.desabilitado`, nao `.desabilitado`** | Especificidade: a regra de cor (0,2,0) venceria a de desabilitado (0,1,0) e o chip colorido seguiria colorido |
| **Remover do chip e fechar do alerta: `<button>` com nome acessivel em PT-BR** | O "x" sozinho e so desenho para quem usa leitor de tela, e fora de `<button>` nao entra na tabulacao |
| **Foco do fechar do alerta em `currentColor`** | Dentro de alerta preenchido primary, o anel azul do token sumiria no fundo azul |
| **Campo: 56 / 40 px, borda ativa de 2 px, medidos no kit** | Conferidos no navegador: outlined 56, filled 56, standard 48, pequeno 40 |
| **Novo token `--rvm-color-input-border` = `secondary-main`** | ⚠️ O `input-line` do kit da **1.49:1** no claro e **1.85:1** no escuro; contorno de campo precisa de 3:1 (WCAG 1.4.11). O `#8A8D93`, que ja e cor do kit, da 3.33 e 3.93. O `input-line` fiel segue valendo para linha decorativa |
| **Borda ATIVA do campo usa o token do anel de foco** | Ela e o indicador de foco (o input tem `outline: none`); com `primary-main` o escuro dava 1.84:1. Medido no navegador: 7.12 claro, 6.38 escuro |
| **`Required` = so `aria-required`, SEM o `required` nativo** | 🔴 Pego no navegador: com `required` nativo, clicar em Enviar com campo vazio **nem disparava o evento submit** — o navegador barrava com o balao dele, em ingles, antes da validacao do `EditForm`. A mensagem em PT-BR nunca aparecia. bUnit nao pega isso: nao existe validacao nativa fora do navegador |
| **`name` vazio no site e do framework, nao bug** | No WebAssembly o `EditContext` desliga os nomes de campo (nao ha POST). No SSR, que e onde o binding do POST precisa, o nome sai — provado no bUnit, que roda pelo mesmo caminho do Blazor Server. O componente omite `name=""` |
| **Rotulo flutuante so com CSS (`:placeholder-shown`)** | Estado inicial certo sem JS, nos dois modos de hospedagem. O retalho que "corta" a borda assume o papel como fundo; `--rvm-textfield-notch-background` troca |
| **Clique ignorado em `Disabled` e `Loading` tambem no C#** | O `disabled` do elemento barra o navegador, mas nao chamada programatica — clique que escapa enquanto carrega vira requisicao duplicada |

## Review independente (Sonnet) — achados corrigidos

| Sev | Achado | Correcao |
|---|---|---|
| P1 | `RvmTextField` mandava todo `AdditionalAttributes` para o `<input>`: o `style="max-width"` do consumidor era engolido pelo `flex: 1`, sem erro | `class` e `style` vao para a raiz; o resto (`autocomplete`, `maxlength`, `aria-*`) segue no input, onde atributo de campo precisa chegar. Desvio consciente do "tudo na raiz", comentado no componente |
| P1 | `RvmAvatarGroup` com excedente de 2+ digitos mostrava "+1" no lugar de "+10": o "+N" passava pelo corte de iniciais em duas letras | "+N" vai como conteudo livre, sem corte. Teste com 10 e 128 |
| P2 | `RvmAvatar` so com icone, sem `Alt` nem `Initials`, virava `role="img"` sem nome — leitor de tela diz so "imagem" | Sem nome, sai `aria-hidden` (decorativo), e nao imagem muda |

## Rede runner -> Rivendell e intermitente

Dois episodios no mesmo dia: um E2E com todos os testes estourando em 40 s e um deploy que morreu no
`ssh-keyscan` em 5 s. Nos dois, a tentativa seguinte **sem mudanca nenhuma** passou. O
`deploy-development.yml` ganhou retry com timeout no keyscan (5x) e no rsync (3x) — a mesma licao que
o publish no BaGet ja tinha pago.

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
5. Screenshot lado a lado com o PNG do kit, aprovado pelo Rafael — ✅ **aprovado em 17/09/2026**
   ("prossiga", depois das seis comparacoes `comparacao-*.png`)
