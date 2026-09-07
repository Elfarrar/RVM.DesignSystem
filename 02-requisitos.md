# 02 — Requisitos

Requisito com `⏳ PENDENTE` é decisão que ainda não foi tomada; a pergunta exata está escrita junto.

## Requisitos funcionais — biblioteca

| # | Requisito | Onda |
|---|---|---|
| RF-01 | Expor a camada de tokens como CSS custom properties num único arquivo servido pelo pacote | 1 |
| RF-02 | Expor os mesmos tokens tipados em C# (`RvmTheme`), para consumo em código | 1 |
| RF-03 | Alternar tema claro/escuro em runtime, sem recarregar a página | 1 |
| RF-04 | Permitir que o app consumidor troque a paleta primária/secundária sem recompilar a biblioteca | 1 |
| RF-05 | Suportar duas densidades (`Comfortable`, `Compact`) afetando altura de controle e espaçamento | 2 |
| RF-06 | Entregar os componentes básicos de formulário com `@bind-Value` e validação integrada ao `EditContext` | 1 |
| RF-07 | Entregar a casca de aplicação (`RvmAppShell` + `Sidebar` + `Topbar`) responsiva, com menu colapsável | 2 |
| RF-08 | Entregar os componentes de feedback (`Dialog`, `Toast`, `Alert`, `Skeleton`, `Spinner`, `EmptyState`) | 3 |
| RF-09 | `RvmDialog` com foco preso, retorno de foco ao fechar, fechamento por `ESC` e resultado tipado | 3 |
| RF-10 | Serviço imperativo de toast (`IRvmToastService.Show(...)`) chamável de qualquer componente | 3 |
| RF-11 | `RvmDataGrid` com colunas tipadas, ordenação, paginação e seleção | 4 |
| RF-12 | `RvmDataGrid` com modo de dados remoto (callback de página + total), além do modo em memória | 4 |
| RF-13 | `RvmAutocomplete` com busca assíncrona, debounce e navegação por teclado | 4 |
| RF-14 | `RvmDatePicker` com entrada por teclado em `dd/MM/yyyy` e calendário em pt-BR | 4 |
| RF-15 | Todo componente aceitar atributos HTML não mapeados (`AdditionalAttributes`) e `Class` externo | 1 |
| RF-16 | Catálogo de ícones — conjunto único, consumido por nome, sem `<img>` externo | 2 |

Catálogo completo com propriedades e estados: `11-catalogo-de-componentes.md`.

## Requisitos funcionais — site de documentação

| # | Requisito | Onda |
|---|---|---|
| RF-20 | Página por componente com exemplo renderizado ao vivo | 1 |
| RF-21 | Bloco de código Razor copiável (botão copiar, com confirmação visual) | 1 |
| RF-22 | Playground: alterar propriedades do componente por controles e ver o resultado **e o código** mudarem | 2 |
| RF-23 | Páginas de fundamentos: cor, tipografia, espaçamento, raio, sombra, ícones, motion | 1 |
| RF-24 | Página de cor mostrando a razão de contraste de cada par texto/fundo, com o veredito AA | 1 |
| RF-25 | Guia de adoção: instalar o pacote, registrar serviços, aplicar o tema, primeira tela | 1 |
| RF-26 | Changelog com versões publicadas e marcação explícita de breaking change | 1 |
| RF-27 | Busca por nome de componente e de token | 3 |
| RF-28 | Alternância clara/escura do próprio site, persistida no navegador | 1 |
| RF-29 | Tabela de API por componente (parâmetro, tipo, padrão, descrição) | 2 |

## Requisitos não funcionais

### Acessibilidade — critério de aceite, não fase

- **WCAG 2.1 nível AA** em todo componente entregue. Componente sem isso não fecha a task.
- Todo elemento interativo alcançável e operável por **teclado**, na ordem visual, com foco visível
  que nunca depende só de cor.
- Contraste mínimo **4.5:1** para texto normal e **3:1** para texto grande e limites de controle —
  verificado nos dois temas (claro e escuro), não só no claro.
- Papéis e estados ARIA corretos por componente (`aria-expanded`, `aria-selected`, `aria-invalid`,
  `aria-describedby` ligando erro ao campo).
- `prefers-reduced-motion` respeitado: animação vira transição instantânea, nunca é ignorada.
- Verificação automatizada com **axe** no E2E do site; violação séria reprova o build.

### Compatibilidade

- **Blazor Server e Blazor WebAssembly**, os dois. Nenhum componente pode depender de JS para
  renderizar seu estado inicial — JS entra só em `OnAfterRenderAsync`, para comportamento
  (foco preso, posicionamento, clique fora, cópia).
- .NET 10, alinhado ao ecossistema.
- Navegadores: duas últimas versões de Chrome, Edge, Firefox e Safari. **Sem IE, sem polyfill.**
- Responsivo de 360px a 1920px. A casca de aplicação muda de layout (drawer sobreposto) abaixo de
  `md`.

### Qualidade

- Cobertura de teste **≥ 80%** (portão do `ci.yml` do ecossistema), com bUnit por componente.
- Todo componente tem, no mínimo: teste de render padrão, de cada variante, de estado desabilitado,
  de callback disparado e de atributo ARIA esperado.
- E2E Playwright sobre o **site de documentação** — ele exercita a biblioteca de verdade.
- Zero warning de compilação no `Release`. `TreatWarningsAsErrors` ligado na biblioteca.

### Desempenho

- CSS total da biblioteca **≤ 60 KB** minificado (sem os ícones); ícones carregados sob demanda.
- JS total **≤ 20 KB** minificado, em módulos ES carregados sob demanda pelo componente que precisa.
- Nenhuma dependência NuGet de terceiros na biblioteca de componentes. `Microsoft.AspNetCore.*`
  apenas.
- O site publicado (WASM) deve carregar a primeira página útil em **≤ 3s** em conexão 4G simulada.

### Texto e idioma

- Todo texto padrão visível ao usuário final em **PT-BR explicativo**, sem código interno
  (`padrao-rvm` §9). Exemplos: rótulo de paginação, "Nenhum resultado encontrado", meses do
  calendário, texto de `EmptyState`.
- Todo texto padrão é **sobrescrevível por parâmetro** — nenhum literal preso no meio do componente.
- Formatação de número, data e moeda com `CultureInfo` explícito. Moeda nunca por interpolação
  manual (`padrao-rvm` §9).
- ⏳ **PENDENTE — decisão do Rafael:** a v1 tem localização real (recursos `.resx`, en-US além do
  pt-BR) ou apenas parâmetros de texto em português? *Pergunta exata: algum app RVM vai precisar de
  interface em inglês nos próximos 12 meses?* Suposição vigente: **não** — pt-BR fixo, textos
  sobrescrevíveis por parâmetro, sem `.resx`.

### Segurança

- Nenhum componente renderiza HTML fornecido pelo consumidor sem que ele peça explicitamente
  (`MarkupString` só onde documentado, com aviso na página do componente).
- Sem chamada de rede pela biblioteca: nada de CDN de fonte ou de ícone em runtime. Fontes e ícones
  são servidos pelo próprio pacote — requisito de privacidade e de funcionamento em rede fechada.

## Requisitos de distribuição

| # | Requisito |
|---|---|
| RD-01 | Pacote `RVM.DesignSystem` publicado no BaGet interno a cada merge em `master` |
| RD-02 | SemVer: breaking change só em major; parâmetro novo com padrão é minor |
| RD-03 | Símbolos e `README` embutidos no pacote; `PackageProjectUrl` apontando para o site |
| RD-04 | Parâmetro removido passa por um ciclo de `[Obsolete]` numa minor antes de sumir numa major |
| RD-05 | Changelog do site gerado a partir das notas de versão do pacote — uma fonte só |

## Fora de escopo (v1) — declarado para não voltar como surpresa

Gráficos, editor de texto rico, upload com preview, drag-and-drop/kanban, tabela com colunas
editáveis in-loco, virtualização de lista longa, temas por tenant em runtime, RTL, publicação no
nuget.org, e migração de qualquer app existente. Cada item com destino em `09-roadmap.md`.
