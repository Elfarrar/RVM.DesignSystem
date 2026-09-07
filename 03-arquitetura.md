# 03 — Arquitetura

## Desvio do padrão do ecossistema — justificado por escrito

O padrão RVM é **Vertical Slice Architecture + MediatR + domínio único**, espelhando o RVM.ERPAgro
(skill `padrao-rvm` §1). **Este projeto não segue esse padrão**, e a justificativa é a seguinte:

VSA organiza *casos de uso* que atravessam camadas (endpoint → handler → banco). Aqui não há caso de
uso, não há endpoint, não há banco e não há requisição: o artefato é uma **Razor Class Library**, e
sua unidade de organização é o **componente**. Aplicar MediatR a um botão seria cerimônia sem
mensagem para carregar.

O que o padrão do ecossistema continua valendo aqui, sem exceção: .NET 10, testes ≥ 80%, CI/CD por
callers do `RVM.Actions`, fluxo de branch/PR, convenções de cultura e texto em PT-BR, segredos fora
do repositório.

> Este desvio deve ser registrado como **ADR-010** no Vault durante o bootstrap, para que a próxima
> sessão que abrir este repositório não tente "corrigir" a arquitetura para VSA.

## Forma do artefato

```
RVM.DesignSystem/
  src/
    RVM.DesignSystem/                 # Razor Class Library — O PACOTE
      Components/
        Basicos/                      # RvmButton, RvmTextField, RvmSelect, ...
        Layout/                       # RvmAppShell, RvmSidebar, RvmCard, ...
        Feedback/                     # RvmDialog, RvmToast, RvmAlert, ...
        Dados/                        # RvmDataGrid, RvmPagination, RvmDatePicker, ...
      Theming/                        # RvmTheme, RvmPalette, RvmThemeProvider, RvmThemeService
      Services/                       # IRvmToastService, IRvmDialogService
      Icons/                          # RvmIcon + sprite SVG
      wwwroot/
        rvm-design-system.css         # tokens + base + todos os componentes (build)
        js/rvm-ds.js                  # módulo ES único, importado sob demanda
        fonts/                        # fontes auto-hospedadas
      DependencyInjection.cs          # AddRvmDesignSystem(...)
    RVM.DesignSystem.Docs/            # Blazor WebAssembly — O SITE
      Pages/                          # uma página por componente + fundamentos
      Playground/                     # motor do playground de props
      wwwroot/
  test/
    RVM.DesignSystem.Tests/           # bUnit — unidade por componente
    RVM.DesignSystem.E2E/             # Playwright + axe sobre o site publicado
  docs/
    Vault/02_Tasks/  03_Kanban/
  landing/                            # NÃO EXISTE — o site de doc é a vitrine (ver 07)
```

**Dois projetos entregáveis, um pacote.** O `.Docs` referencia o pacote **do BaGet**, não por
`ProjectReference` — assim a documentação exercita o caminho real de instalação do consumidor. Um
`Directory.Build.props` permite alternar para `ProjectReference` em desenvolvimento local via
propriedade `UseLocalDesignSystem=true`, para não exigir publicação a cada alteração.

> Esta é uma **suposição minha**, marcada como tal: se a fricção do BaGet em desenvolvimento local
> se mostrar alta na onda 1, a decisão inverte para `ProjectReference` + um smoke test separado que
> valida a instalação por pacote.

## Camadas

```
   Aplicação RVM (ERPAgro, projeto novo, o próprio site de doc)
        │  usa <RvmButton>, <RvmDataGrid>
        ▼
   Componentes            Razor + CSS isolado, sem estado global
        │  consomem var(--rvm-*)
        ▼
   Tokens semânticos      --rvm-color-surface, --rvm-color-on-surface, --rvm-space-4
        │  apontam para
        ▼
   Tokens primitivos      --rvm-blue-500, --rvm-gray-900, --rvm-size-4
```

A regra que sustenta a camada: **componente nunca referencia token primitivo**. Um botão consome
`--rvm-color-primary`, jamais `--rvm-blue-500`. É o que permite trocar a paleta de um produto sem
tocar em nenhum componente. Detalhe em `06-tokens-e-tematizacao.md`.

## Decisões técnicas

### CSS — isolamento e nomeação

- Cada componente tem seu `.razor.css` (CSS isolation do Blazor, que gera o atributo `b-*`).
- Além disso, **toda classe pública leva o prefixo `rvm-`**. O isolamento protege de fora para
  dentro; o prefixo protege de dentro para fora, e é o que permite conviver na mesma página com o
  CSS de um app que use outra biblioteca.
- Nenhum reset global agressivo. O pacote traz `rvm-base.css` opcional, que o consumidor inclui se
  quiser; sem ele, os componentes continuam corretos.
- Sem framework CSS, sem Tailwind, sem SASS. CSS moderno puro: custom properties, `:has()`,
  `color-mix()`, container queries onde couber. É a decisão que mantém "zero dependência" honesta.

### JavaScript — mínimo, sob demanda, e nunca para renderizar

Um módulo ES único (`rvm-ds.js`), importado via `IJSObjectReference` pelo componente que precisa —
nunca por `<script>` global. Cobre exatamente cinco coisas:

1. foco preso e devolução de foco (`Dialog`, `Drawer`);
2. posicionamento de camada flutuante (`Select`, `Autocomplete`, `DatePicker`, `Tooltip`);
3. detecção de clique fora e de `ESC`;
4. cópia para a área de transferência;
5. leitura de `prefers-color-scheme` e persistência da preferência de tema.

**Nenhum estado visual vive no JS.** Isso é o que faz o mesmo componente funcionar em Server e em
WASM, e é o que faz o teste bUnit valer alguma coisa — o que o bUnit não consegue exercitar
(as cinco coisas acima) é exatamente o que o E2E cobre.

### Estado e serviços

- Componentes são **sem estado global**. Parâmetro entra, `EventCallback` sai.
- Duas exceções, ambas registradas no DI e ambas escopadas ao consumidor:
  - `IRvmToastService` — fila de notificações, consumida por um `<RvmToastHost>` no layout;
  - `IRvmThemeService` — tema corrente e sua persistência.
- Registro único: `builder.Services.AddRvmDesignSystem(opções)`. Sem `AddRvmButton()`.

### Formulários

Integração com o `EditContext` nativo do Blazor: os campos herdam de `InputBase<T>`, de modo que
`DataAnnotations`, `EditForm` e `ValidationMessage` funcionam sem adaptação. **Não haverá motor de
validação próprio** — seria reinventar o que o framework já entrega e quebraria a expectativa de
quem já escreve Blazor.

### Ícones

Sprite SVG único, auto-hospedado, consumido por `<RvmIcon Name="check" />`. Sem Material Icons por
CDN (requisito de privacidade do `02`), sem font-icon.
⏳ **PENDENTE — decisão do Rafael:** qual conjunto de ícones? *Pergunta exata: adotamos um conjunto
livre existente (Lucide ou Phosphor, licença MIT, ~1500 ícones) ou desenhamos um conjunto próprio?*
Suposição vigente: **Lucide**, importado e re-empacotado no build — desenhar ícone à mão é trabalho
de designer, não de biblioteca, e a licença MIT permite a redistribuição.

## Multi-tenant, auth e soft-delete

**Não se aplicam** — declarado explicitamente porque o checklist do ecossistema os exige:

| Item | Situação |
|---|---|
| Multi-tenant / `CompanyId` | Não há dado, não há tenant. A "variação por produto" é de tema (build-time), não de linha em banco |
| Autenticação | A biblioteca não autentica. O site de documentação é **público e anônimo**, de propósito |
| Soft-delete | Não há persistência |
| Global query filter | Não há `DbContext` |

## Testes

| Camada | Ferramenta | O que cobre |
|---|---|---|
| Componente | bUnit + xUnit | Render, variantes, estados, callbacks, atributos ARIA, classes CSS aplicadas |
| Tema | xUnit | Geração das custom properties, contraste calculado de cada par da paleta (falha se < AA) |
| Site | Playwright + axe | Navegação, playground, cópia de código, e **auditoria de acessibilidade por página** |

O teste de contraste é o que impede o requisito de acessibilidade de virar boa intenção: uma paleta
nova que reprove no cálculo derruba o CI antes de qualquer humano olhar.

## O que esta arquitetura conscientemente não faz

- **Não tem sistema de plugin.** Componente novo é código neste repositório.
- **Não tem camada de abstração sobre renderizador.** É Blazor, e só.
- **Não tem gerador de código.** Componente é escrito à mão. (O `RVM.Mimic` opera em CRUD de
  domínio; não há sobreposição.)
- **Não tem pipeline de design tokens** (Style Dictionary, Figma sync) na v1. Os tokens são escritos
  em CSS/C# à mão — ver `09-roadmap.md`.
