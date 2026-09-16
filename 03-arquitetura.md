# 03 — Arquitetura

## Desvio declarado: este projeto NÃO segue VSA

⚠️ O padrão do ecossistema (`padrao-rvm` §1) é Vertical Slice Architecture + domínio único, sem
MediatR. **Aqui não se aplica, e isso é decisão, não esquecimento.**

VSA organiza **casos de uso que atravessam camadas**: endpoint → handler → banco. Este projeto não
tem caso de uso, endpoint nem banco — o artefato é uma Razor Class Library, e a unidade de
organização é o **componente**. Forçar VSA aqui produziria pastas vazias com nome de arquitetura.

**MediatR não entra** — proibido no ecossistema (ordem de 13/09/2026) e, de todo modo, sem sentido
numa biblioteca de UI. **Não "corrigir" a arquitetura para VSA em sessão futura.**

## Solução

```
RVM.DesignSystem.slnx
  src/RVM.DesignSystem/           RCL — o pacote. Componentes, tokens, tema, ícones
  src/RVM.DesignSystem.Docs/      Blazor WASM — o site de documentação
  test/RVM.DesignSystem.Tests/    bUnit — render, parâmetros, estados, acessibilidade
  test/playwright/RVM.DesignSystem.E2E/   Playwright + axe — o site publicado
```

⚠️ **O E2E mora sob `test/playwright/` de propósito.** O `ci.yml` do ecossistema roda todo
`test/**/*.csproj` e pula só caminhos que contêm `playwright`; este E2E exige o site no ar. Mover
para `test/RVM.DesignSystem.E2E` faz o CI tentar rodá-lo sem site — e falhar sem motivo aparente.

## Organização interna da biblioteca

```
src/RVM.DesignSystem/
  Components/<Nome>/RvmNome.razor + .razor.cs + .razor.css
  Theming/            RvmThemeProvider, RvmTheme, modo claro/escuro
  Tokens/             tokens.css (custom properties), escala tipográfica
  Icons/              SVG do Tabler, copiados; sem dependência de runtime
  wwwroot/            fonte Inter, CSS compilado, JS mínimo
```

- **CSS isolado por componente** (`.razor.css`), consumindo **apenas** custom properties. Componente
  que traz hex literal no CSS é bug: quebra tematização.
- **Prefixo `Rvm`** em todo componente público (`RvmButton`, `RvmTextField`).
- **Nome de API em inglês** (`RvmButton.Variant`), **texto ao usuário final em PT-BR** explicativo.
- **Blazor Server E WebAssembly**, os dois. Nenhum componente depende de JS para renderizar seu
  estado inicial; JS só em `OnAfterRenderAsync`.

## Decisões registradas

| ADR | Decisão | Alternativa recusada |
|---|---|---|
| ADR-001 | Não segue VSA — é RCL, não aplicação | VSA por uniformidade (pastas vazias) |
| ADR-002 | **CI próprio em `ubuntu-latest`**, não o `RVM.Actions` | Caller do reusable: repositório público **não consegue** chamar reusable de repositório privado — o run morre em 0 s, zero jobs, sem mensagem útil |
| ADR-003 | Sem MudBlazor e sem biblioteca de terceiros | MudBlazor tematizado: mais rápido, mas o visual chega perto e não igual |
| ADR-004 | Versão **por tag `v*`**, nunca literal no csproj | Versão a cada push: vira alvo móvel e reabre a decisão de não fazer backup do BaGet |
| ADR-005 | Ícones **Tabler**, copiados como SVG | Phosphor (traço mais arredondado que o do kit); reexportar do Figma (depende do Rafael) |
| ADR-006 | Fonte Inter servida **pelo pacote** | Google Fonts em runtime: dependência de rede de terceiros em todo app consumidor |

## Auth e multi-tenant

**Não se aplica.** Site público e anônimo, biblioteca não autentica, não há dado nem tenant.
Variação é de **tema**, resolvida em tempo de execução no cliente.
