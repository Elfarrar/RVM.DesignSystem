# 01 — Visão Geral

## Problema

Cada aplicação Blazor do ecossistema RVM inventou a própria linguagem visual, sozinha, do zero.
Isto não é uma suspeita — está no disco, hoje:

| App | Arquivo de tema | Cor primária |
|---|---|---|
| `RVM.ERPAgro` | `Web/Services/ThemePalettes.cs` | `#0288d1` (azul) |
| `RVM.ObraEmDia` | `Web/Servicos/TemaObraEmDia.cs` | `#2A6E49` (verde-terra) |
| `RVM.Fiscal` | `Web/FiscalTheme.cs` | `#1B5E5A` (teal) |
| `RVM.Propostinha` | `Web/PropostinhaTheme.cs` | `#302B9E` (índigo) |

Quatro arquivos, quatro paletas completas — **cada uma com seu próprio modo escuro escrito à mão**,
cada uma com seu próprio `MainLayout`, seu próprio estado vazio, seu próprio diálogo de confirmação.
Nenhum dos quatro sabe da existência dos outros três.

O custo disso aparece em quatro lugares:

- **Tela nova começa do zero.** Não existe "o card do RVM", existe o card que o ObraEmDia fez naquele
  dia. A decisão de espaçamento é retomada em cada página.
- **App novo copia do último app.** Foi como as quatro paletas nasceram. O defeito do primeiro viaja.
- **Acessibilidade é sorte.** Contraste, foco visível e navegação por teclado não são verificados em
  lugar nenhum — porque não há lugar nenhum onde verificar.
- **Nada disso é demonstrável.** Não há uma página que mostre o que o ecossistema RVM parece.

## Solução

**RVM.DesignSystem** — duas entregas que só fazem sentido juntas:

1. **Uma biblioteca de componentes Blazor própria**, distribuída como pacote NuGet versionado no
   BaGet interno. Sem MudBlazor, sem dependência de UI de terceiros: Razor + CSS escritos aqui.
2. **Um site público de documentação** em `design.rvmit.com.br`, com galeria de componentes, código
   copiável, playground de propriedades e as páginas de fundamentos.

O site não é subproduto: **é a primeira aplicação construída com a biblioteca**. Se um componente é
difícil de documentar, ele está mal projetado — e isso aparece no mesmo dia, não seis meses depois.

## As três ideias que sustentam o produto

### 1. Token antes de componente

O que padroniza um ecossistema não é o botão — é a escala de espaçamento, a família tipográfica, o
raio de borda e a paleta que o botão consome. Os tokens são a camada estável; os componentes são
consumidores dela. Um app RVM que use **só** os tokens já fica coerente com os outros, mesmo sem
usar um único componente.

Consequência prática: a v1 entrega os tokens completos antes de entregar o segundo componente.

### 2. A biblioteca é escrita do zero — e isso é uma decisão com preço

Não há MudBlazor, nem Radzen, nem Fluent UI por baixo. O que se ganha: controle total do visual, sem
brigar com CSS de terceiros; sem breaking change de biblioteca externa; acessibilidade que é
responsabilidade nossa em vez de esperança; e um artefato que demonstra engenharia de front-end de
verdade.

O que se paga, declarado para não ser esquecido: **`DataGrid`, `Autocomplete` e `DatePicker`
acessíveis são, juntos, mais trabalho que os outros trinta componentes somados.** Estão na v1 por
decisão do Rafael (07/09/2026), com a recomendação contrária registrada e superada. Ver
`09-roadmap.md` § Ondas — são a onda 4 justamente por isso.

### 3. Base comum, paleta por produto

Tipografia, espaçamento, raio, sombra, densidade e comportamento são **idênticos** em todo o
ecossistema. A cor primária e a secundária são token trocável: o ObraEmDia continua verde, o Fiscal
continua teal. Padronizar não é apagar identidade — é parar de reescrever o que nunca precisou
variar.

## Persona

Este projeto tem **dois leitores**, e os dois importam:

1. **A sessão que constrói o próximo app RVM** (Claude ou o Rafael). Para ela, o design system é
   ferramenta: instala o pacote, lê o site, monta a tela. O sucesso se mede em tempo até a primeira
   tela coerente.
2. **O entrevistador técnico.** `design.rvmit.com.br` é peça de portfólio pública: um design system
   próprio, documentado e acessível, é evidência mais forte de senioridade de front-end do que
   qualquer CRUD.

Quando "mais poderoso" e "mais fácil de explicar" entrarem em conflito, ganha o que fica mais fácil
de explicar — a mesma regra do RVM.Mimic, pelo mesmo motivo.

## Escopo — faz

- Camada de **tokens** (cor, tipografia, espaçamento, raio, sombra, z-index, motion, breakpoints)
  como CSS custom properties, com espelho tipado em C#.
- **Tematização**: claro/escuro, paleta trocável por produto, densidade confortável/compacta.
- **Biblioteca de componentes Blazor** (~35 na v1) em quatro grupos — básicos, layout, feedback e
  dados. Catálogo completo em `11-catalogo-de-componentes.md`.
- Compatibilidade com **Blazor Server e Blazor WebAssembly** — os dois, sempre.
- **Acessibilidade WCAG 2.1 nível AA** como critério de aceite de cada componente, não como fase.
- **Site público de documentação** com galeria, código copiável, playground de props, fundamentos,
  guia de adoção e changelog.
- Distribuição por **NuGet no BaGet interno**, com SemVer e política de obsolescência.

## Escopo — não faz

- **Não migra os apps existentes.** ERPAgro, ObraEmDia, Fiscal e Propostinha seguem no MudBlazor,
  sem prazo. Decisão de 07/09/2026 — ver `09-roadmap.md` § Adoção.
- **Não é wrapper de MudBlazor.** Não há adaptador, ponte, nem modo de compatibilidade.
- **Não tem backend.** Sem banco, sem API HTTP, sem autenticação, sem multi-tenant. O `04` explica
  o que ocupa o lugar do modelo de dados.
- **Não faz gráficos** (chart, sparkline) na v1 — ver `09-roadmap.md` § Fora da v1.
- **Não faz editor de texto rico, upload com preview, nem kanban/drag-and-drop** na v1.
- **Não publica no nuget.org** na v1. O BaGet interno atende; publicação pública é decisão futura.
- **Não é white-label por tenant.** Paleta é por *produto*, definida em build/configuração — não por
  cliente em runtime.
- **Não define a identidade de marca RVM** (logo, tom de voz, papelaria). O design system consome
  uma identidade; não a cria.

## Fronteira contra o ecossistema RVM

| Projeto | Fronteira |
|---|---|
| `RVM.Common` | Complementar, não sobreposto: o `RVM.Common.Security` distribui código de segurança, o `RVM.DesignSystem` distribui UI. Mesmo BaGet, mesma disciplina de versão, repositórios separados — misturar auth com CSS no mesmo pacote obrigaria todo consumidor de um a arrastar o outro |
| `RVM.ERPAgro` | É a **referência arquitetural do ecossistema**, mas não de UI. O design system não espelha o VSA dele (ver `03-arquitetura.md` § Desvio). Consumidor potencial, sem prazo |
| `RVM.Template` | O `RVM.Template` dá o esqueleto de projeto novo; quando o design system estabilizar, o template passa a referenciar o pacote. Não se sobrepõem hoje |
| `RVM.Site` / `RVM.Curriculo` | São sites institucionais em Astro/Pages. O site de doc é Blazor e mora **neste** repositório. Não é landing de produto — o ADR-009 não se aplica (ver `07-site-de-documentacao.md`) |
| `RVM.Actions` | Consumido para `ci.yml` e `e2e.yml`. Publicação NuGet e site estático **não** têm reusable hoje — ver `10-infra-e-integracoes.md` § CI/CD |
| ⛔ `RVM.Portfolio` | Descontinuado em 01/09/2026. O site de doc **não** depende do portfólio futuro e não espera por ele |

## Decisão registrada — por que não MudBlazor

Em 07/09/2026 a recomendação foi construir sobre o MudBlazor (aproveitando os quatro apps que já o
usam) e o Rafael decidiu o contrário: **componentes próprios do zero**.

Motivo de registrar em vez de só executar: daqui a alguns meses, no meio da onda 4, o `DataGrid` vai
parecer um erro evitável e alguém — provavelmente eu — vai propor "só usar o MudDataGrid". Esta
seção existe para que essa proposta seja uma reabertura consciente da decisão, e não um conserto
por esquecimento.

Do que se abriu mão: velocidade da v1, e a possibilidade de os quatro apps existentes adotarem o
design system sem reescrever tela. O segundo item já está aceito no escopo (não há migração).
