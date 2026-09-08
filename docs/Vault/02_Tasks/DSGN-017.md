---
id: DSGN-017
titulo: Onda 2 — casca de aplicacao, layout e navegacao
repo: RVM.DesignSystem
tipo: feature
status: concluido
criada: 2026-09-08
---

# DSGN-017 — Onda 2

Fecha a onda 2 do `09-roadmap.md` e prepara a **`0.2.0`**.

## Criterio de saida (do `09-roadmap`)

> O proprio site passa a usar `RvmAppShell` — a casca deixa de ser codigo de exemplo e vira
> codigo em producao.

✅ `src/RVM.DesignSystem.Docs/Layout/MainLayout.razor`. As ~60 linhas de casca escrita a mao
(header, nav com `NavLink`, main com `tabindex`, link de pulo) sairam. O que sobrou no arquivo e
so o que e **deste** site: o seletor de temas, os dados do menu e o texto do rodape.

## Entregue

- **Casca**: `RvmAppShell`, `RvmSidebar`, `RvmTopbar`, `RvmNavItem`, `RvmNavGroup`
- **Layout**: `RvmStack`, `RvmGrid`, `RvmCard`, `RvmDivider`
- **Conteudo e navegacao**: `RvmChip`, `RvmAvatar` (+ `RvmAvatarGroup`), `RvmTabs` (+ `RvmTab`),
  `RvmBreadcrumb`
- **Densidade `Compact`** (`RF-05`)
- **8 icones novos** — os 7 que a casca precisava mais `copy`
- **13 paginas** de documentacao novas (12 componentes + Fundamentos › Densidade)

## Decisoes que mudam contrato

### 1. Papel novo na paleta: `on-surface-variant`

A onda 1 nao tinha papel de texto secundario — pintava tudo com `on-surface`. Divisor com
rotulo, item de trilha ja percorrido, rodape de cartao e titulo de secao do menu **todos**
precisam dele, e a alternativa que aparece sozinha e `opacity` — que foi o defeito nº 4 da
onda 1, duas vezes.

Ele e derivado, passa pelo `EnsureContrast` e entrou no `PairsToVerify` medido contra as tres
superficies em que aparece. O portao foi de **26 para 29 pares** por paleta.

⚠️ `RvmPalette.OnSurfaceVariant` e `required`: quebra quem monta paleta a mao com
`new RvmPalette { ... }`. Quem usa `FromSeed` — o caminho documentado — nao e afetado. Aceitavel
em 0.x, e o motivo de a onda sair como **minor** e nao patch.

### 2. O portao de CSS passou a aceitar custom property do proprio arquivo

Sem isso o `RvmGrid` nao teria como existir: media query **nao pode ser gerada por instancia**
(o CSS isolation gera um arquivo, nao um por uso), entao o unico jeito de fazer grade
responsiva e o componente declarar `--rvm-grid-columns` com padrao e a instancia sobrescrever
por `style` inline.

A folga e estreita e tem teste dos dois lados: declarada no arquivo aprova, usada sem declarar
continua reprovando. O defeito original — `var(--rvm-color-primry)` com erro de digitacao, que
falha em silencio porque CSS ignora propriedade inexistente — continua pego.

### 3. `.rvm-sr-only` virou API publica, dentro do `rvm-tokens.css`

E a unica classe de um arquivo que so tinha tokens. Mora la porque e usada por varios
componentes e porque quem consome a biblioteca precisa dela. `display: none` e
`visibility: hidden` **nao servem**: os dois removem o elemento da arvore de acessibilidade,
que e o oposto do que se quer.

### 4. `RvmAppShell` ganhou slot de rodape

Nao estava na spec (`11` descreve "topbar + sidebar + conteudo"), mas os quatro `MainLayout` do
ecossistema tem rodape, e sem o slot ele teria que ficar fora da casca — perdendo o marco
`contentinfo`. A regra de escopo do HTML e sutil: `footer` so vira `contentinfo` quando o
ancestral de secionamento mais proximo e o `body`. Dentro do `main` ele vira generico, **sem
nenhum aviso**, porque a marcacao continua valida.

## A cor que escapava dos dois portoes

O fundo do `RvmAvatar` e derivado do **nome da pessoa**, em tempo de execucao. Isso o coloca
fora do alcance dos dois portoes que existiam: o de contraste percorre papeis semanticos, e o de
CSS varre arquivos `.razor.css`. Esta cor nao esta em nenhum dos dois — ela nasce num `style`
inline.

Seria a unica cor da biblioteca capaz de reprovar AA sem nada reclamar. O `AvatarCorTests`
fecha o buraco varrendo **as 360 matizes possiveis**, e nao so os nomes do exemplo.

Detalhe que quase passou: a matiz vem de **FNV-1a**, e nao de `string.GetHashCode`. Desde o .NET
Core o hash de string e aleatorizado por processo — usar ele faria o avatar de uma pessoa mudar
de cor a cada reinicio do servidor, e a cor e justamente como ela se acha numa lista.

## Dois defeitos apagados de passagem

- **O botao de copiar do site usava o icone `x`.** Era o mais parecido que o conjunto da onda 1
  tinha. Como esta onda fechava o conjunto de icones, entrou o `copy`.
- **O botao de colapsar a barra lateral rolava para fora da tela.** Ele e o ultimo elemento de
  uma coluna rolavel: com quinze itens de menu, so aparece depois de rolar ate embaixo — onde
  ninguem procura. Virou `position: sticky`.

## Pendencia da onda 1 que foi fechada

**Auditoria axe por pagina.** O E2E auditava so a inicial, deixando as outras 28 sem cobertura —
inclusive as que tem tabela, aba e navegacao, que e onde os defeitos de ARIA moram. Agora ele
varre **toda rota do menu, nos dois modos de cor**.

A lista de rotas e descoberta do proprio menu, e nao escrita no teste: assim ela nao desatualiza
quando entra pagina nova, e a varredura passa a reprovar tambem item de menu apontando para
lugar nenhum.

## Tres defeitos que so apareceram no navegador

Nenhum tem teste unitario capaz de pegar sozinho, e todos foram achados montando as telas.

1. **`RvmSelect` abria na PRIMEIRA opcao, nao no valor ligado.** Faltava o atributo `value` no
   `<select>` — o Blazor o trata como caso especial e escreve a propriedade DEPOIS de renderizar
   os filhos, que e a unica ordem em que o navegador casa o valor com uma `<option>` que ainda
   nao existia. **E defeito da onda 1**, e passou porque todo select de la tinha `Placeholder` e
   comecava sem valor: a primeira opcao era mesmo a certa. Apareceu num select de enum cujo
   padrao nao e o primeiro membro. O formulario parecia preenchido com dado que nao era do modelo.

2. **A barra lateral colapsada virava uma coluna de linhas em branco** quando os itens nao tinham
   `Icon` — cada uma clicavel, nenhuma identificavel. Foi visto colapsando a barra do proprio
   site. A biblioteca oferece o botao de colapsar, entao o estado colapsado e responsabilidade
   dela: entrou a inicial do texto como reserva.

3. **A casca rolava inteira**, e com isso a barra lateral ficava tao alta quanto a pagina e o
   botao de recolher — ultimo elemento dela — ia parar abaixo da dobra. Virou altura fixa com
   coluna de rolagem propria. De quebra, `position: sticky` **nao se desloca com margem
   negativa**, e a regiao em que ele pode parar ja desconta o padding do container: a solucao foi
   tirar o padding de baixo do `nav`, nao compensar com margem.

## Verificado

| | |
|---|---|
| Testes | 187 (eram 114), 0 aviso em `Release` |
| Portao de contraste | 29 pares x 5 temas x 2 modos |
| Portao de CSS | 22 arquivos de componente |
| Varredura de avatar | 360 matizes + 16 nomes |
| axe no E2E | 29 paginas x 2 modos, zero violacao seria |
| Dev no ar | `design.dev.rvmtech.com.br`, verificado por conteudo |
| Prod no ar | `design.rvmit.com.br`, verificado por CONTEUDO (o CSS da onda 2 no bundle da RCL) |
| Pre-release | `0.2.0-alpha.12` no BaGet |

## Promocao para producao — 08/09/2026

Autorizada pelo Rafael. `dev` -> `master` pelo PR #25, deploy do Pages verde, prod conferida no
navegador nos dois modos de cor.

⚠️ **Rota profunda em producao devolve HTTP 404 com a pagina certa.** E o fallback do GitHub
Pages: caminho desconhecido serve o `404.html`, que e copia do `index.html`, e o Blazor roteia no
cliente. Nao e regressao, e nao tem conserto em host estatico — e a razao de a verificacao ser
por CONTEUDO e de o monitor do Kuma ser do tipo `keyword`. A palavra-chave (`RVM Design System`,
no `<title>` do `index.html`) continua no HTML cru, entao o monitor segue valendo.

## O que fica para a onda 3

- `RvmDialog`, `RvmToast`, `RvmAlert`, `RvmSkeleton`, `RvmSpinner`, `RvmProgress`,
  `RvmTooltip`, `RvmEmptyState`
- Densidade so afeta os controles da onda 1 e o `RvmNavItem`. Componente de dado (onda 4) vai
  precisar entrar na conta.
