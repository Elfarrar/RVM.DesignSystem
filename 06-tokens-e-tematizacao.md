# 06 — Tokens e tematização (o motor)

Este é o **motor do projeto**. Componente é consequência: quem define a aparência é a camada de
tokens, e é ela que faz o resultado parecer NEATLAB em vez de "mais um Blazor".

Todos os valores abaixo foram **medidos do kit** (`referencia-neatlab/`), não inventados. O método e
o que ficou impreciso estão no fim.

## Paleta — papéis semânticos

Cada papel tem sete variações, na mesma estrutura do kit (`Theme/Light.png`, `Theme/Dark.png`).

### Cores de marca — iguais nos dois temas

| Papel | Main | Alt. Light | Alt. Dark | Contraste | Hover (contido) |
|---|---|---|---|---|---|
| Primary | `#264CC8` | `#4067E9` | `#1B368F` | `#FFFFFF` | `#264CC8` |
| Secondary | `#8A8D93` | `#9C9FA4` | `#4D5056` | `#FFFFFF` | `#777B82` |
| Info | `#16B1FF` | `#32BAFF` | `#0E71A3` | `#FFFFFF` | `#139CE0` |
| Success | `#56CA00` | `#6AD01F` | `#378100` | `#FFFFFF` | `#4CB200` |
| Warning | `#FFB400` | `#FFB547` | `#A37300` | `#FFFFFF` | `#E09E00` |
| Error | `#FF4C51` | `#FF6166` | `#A33134` | `#FFFFFF` | `#E04347` |

⚠️ **As cores de marca não mudam entre claro e escuro** — conferido swatch a swatch. O que muda é
superfície e estado. Quem "escurecer" a paleta no tema escuro está inventando.

### Fundo de hover/resting do variante *outlined* — muda por tema

| Papel | Claro (hover / resting) | Escuro (hover / resting) |
|---|---|---|
| Primary | `#F5F7FE` / `#CBD6F9` | `#797992` / `#646D9F` |
| Secondary | `#F6F6F7` / `#C5C6C9` | `#383451` / `#5E5D6F` |
| Info | `#EFFAFF` / `#99DDFF` | `#313859` / `#3274A5` |
| Success | `#F2FBEB` / `#ABE580` | `#343945` / `#447C26` |
| Warning | `#FFF9EB` / `#FFDA80` | `#413745` / `#987126` |
| Error | `#FFF1F1` / `#FFA6A8` | `#412F4B` / `#983D4E` |

### Superfícies, texto, ações

| Token | Claro | Escuro |
|---|---|---|
| Fundo do corpo | `#F4F5FA` | `#28243D` |
| Papel / card | `#FFFFFF` | `#312D4B` |
| Divisor | `#E7E6E8` | `#474360` |
| Borda de contorno | `#D1D0D3` | `#5B5774` |
| Linha de input | `#D4D3D5` | `#595572` |
| Overlay | `#9D9AA0` | `#2C2A43` |
| Fundo do snackbar | `#212121` | `#212121` (texto `#FFFFFF`) — conferido no `Snackbar.png` escuro; decidido em 17/09/2026 |
| Texto primário | `#676C74` | `#CFCBE5` |
| Texto secundário | `#79767E` | ⏳ ver nota |
| Texto desabilitado | `#B4B2B7` | ⏳ ver nota |
| Ação — hover | `#F7F7F8` | `#383452` |
| Ação — selecionado | `#F0EFF0` | `#3F3B59` |
| Ação — desabilitado | `#CCCBCE` | `#605C79` |
| Ação — foco | `#E7E6E8` | `#474360` |

### Fundos suaves (`Custom BG`) — tema claro

Primary `#F2EAFF` · Secondary `#F1F1F2` · Info `#E4F2FE` · Success `#E9F5EA` · Warning `#FDEDE0` ·
Error `#FEE8E7` · Menu ativo `#2C78E1` (gradiente no kit).

Tema escuro: Primary `#3D3261` · Secondary `#3C3954` · Info `#2F3A60` · Success `#343D4C` ·
Warning `#4A3E42` · Error `#493049`. **No escuro não existe "Menu ativo"** nesta faixa — a coluna
não está no kit.

> ⚠️ Os fundos suaves do tema claro são **roxos e não azuis** (`#F2EAFF` contra um Primary
> `#264CC8`). Não é erro de medição: é assim no kit, provável resquício da paleta roxa original que
> o autor usou antes de trocar o primary para azul. **Decisão a tomar no `DSGN-002`:** manter fiel
> ou harmonizar com o azul. Fidelidade e coerência brigam aqui, e a escolha é do Rafael.

## Tipografia — fonte Inter

A escala veio **escrita** na página `Typography` do kit (tamanho / entrelinha / peso), então não
depende de medição:

| Estilo | Tam. | Entrelinha | Peso |  | Estilo | Tam. | Entrelinha | Peso |
|---|---|---|---|---|---|---|---|---|
| h1 | 96 | 112 | 300 |  | Botão grande | 15 | 26 | 500 |
| h2 | 60 | 72 | 300 |  | Botão médio | 14 | 24 | 500 |
| h3 | 48 | 56 | 400 |  | Botão pequeno | 13 | 22 | 500 |
| h4 | 34 | 42 | 400 |  | Rótulo de input | 12 | 12 | 400 |
| h5 | 24 | 32 | 400 |  | Texto de apoio | 12 | 20 | 400 |
| h6 | 20 | 32 | 500 |  | Texto de input | 16 | 24 | 400 |
| Subtitle 1 | 16 | 28 | 400 |  | Iniciais de avatar | 20 | 20 | 400 |
| Subtitle 2 | 14 | 22 | 500 |  | Chip | 13 | 18 | 400 |
| Body 1 | 14 | 24 | 400 |  | Tooltip | 10 | 14 | 500 |
| Body 2 | 14 | 20 | 400 |  | Título de alerta | 16 | 24 | 500 |
| Caption | 12 | 19.9 | 400 |  | Cabeçalho de tabela | 14 | 24 | 500 |
| Overline | 12 | 31.9 | 400 |  | Rótulo de badge | 12 | 20 | 500 |

- **Fonte: Inter** (SIL OFL), servida **do próprio pacote** — nunca do Google Fonts em runtime, para
  o site e os apps não dependerem de rede de terceiros.
- Pesos necessários: 300, 400, 500. Variable font resolve os três num arquivo.
- Caption e Overline têm entrelinha quebrada (19.9 / 31.9) — é o kit. Arredondar para 20 e 32 é
  aceitável e será registrado como desvio consciente.

## Sombra, raio e espaçamento

- **Sombra:** o kit tem **24 elevações** (`Shadow/Light.png`, grade 6×4) e **não escreve os valores**.
  ⏳ **PENDENTE — precisa do Figma:** os `box-shadow` exatos. Até lá, a v1 usa 5 elevações derivadas
  por medição visual, marcadas como aproximação no código.
- **Raio e espaçamento:** também não estão escritos. Saem por régua sobre o PNG, com erro de 1–2 px.
  A base é 4 px, e os raios observados giram em 6, 8 e 10 px.

## Tematização

- Tokens como **custom properties CSS** em `:root`, redefinidas sob `[data-theme="dark"]`.
- Nome: `--rvm-<categoria>-<papel>-<variação>` (`--rvm-color-primary-main`).
- Troca de tema **sem recarregar**, persistida por viewer; o tema inicial é aplicado antes da
  primeira pintura, para não piscar branco.
- **Nenhum componente pode depender de JS para renderizar seu estado inicial** — regra dos dois
  modos de hospedagem (Server e WASM).

## Como os valores foram obtidos

Amostragem de pixel (Python + PIL) no centro de cada swatch dos PNGs de 2760×4600, com a grade
conferida contra a imagem. O que o PNG **não** entrega:

1. **Opacidade.** No tema escuro, os três swatches de texto foram desenhados com a mesma cor sólida
   (`#CFCBE5`) — no kit real, a diferença é alpha. Por isso texto secundário e desabilitado do escuro
   estão `⏳ PENDENTE`: chutar alpha aqui estraga contraste, que é critério de aceite.
2. **Nome de estilo e de variável** — a ligação entre swatch e token foi feita pelo rótulo impresso.
3. **Medidas exatas** de espaçamento, raio e sombra.

Os três se resolvem de uma vez **se o kit for duplicado para a conta do Rafael** e lido pela API do
Figma. Enquanto isso, o que está medido é fiel e o que não está aparece como pendência — nunca como
número inventado.
