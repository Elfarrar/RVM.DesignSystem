# 05 — API dos componentes

## Convenções

- **Prefixo `Rvm`** em todo componente público.
- **Nome de API em inglês**; **texto ao usuário final em PT-BR**. As duas regras convivem: a API é
  lida por quem programa (e o resto do ecossistema é inglês no código), a mensagem é lida por quem
  usa o produto, e o padrão do ecossistema exige PT-BR explicativo ali.
- **Parâmetro enum, nunca string mágica**: `Variant="RvmButtonVariant.Contained"`.
- **`AdditionalAttributes` sempre habilitado** (`CaptureUnmatchedValues`), e repassado ao elemento
  raiz.

> ⚠️ O quarto item nasce de um bug real do design system anterior: o `RvmTextField` não renderizava
> `name`, que o formulário em SSR estático exige para o binding do POST. O consumidor teve que
> contornar por fora. **Todo componente de formulário renderiza `name`, `id` e `aria-*`.**

## Forma padrão

```razor
<RvmButton Variant="RvmButtonVariant.Contained"
           Color="RvmColor.Primary"
           Size="RvmSize.Medium"
           Disabled="false"
           OnClick="Salvar">
    Salvar
</RvmButton>
```

| Parâmetro | Tipo | Presente em |
|---|---|---|
| `Color` | `RvmColor` (Primary, Secondary, Info, Success, Warning, Error) | Tudo que tem cor semântica |
| `Size` | `RvmSize` (Small, Medium, Large) | Botão, input, chip, avatar |
| `Variant` | enum por componente | Botão, chip, alerta, campo |
| `Disabled` | `bool` | Todo interativo |
| `ChildContent` | `RenderFragment` | Todo container |
| `Class` / `Style` | `string` | Todos, via `AdditionalAttributes` |

## Regras de contrato

1. **Parâmetro novo é opcional** e tem default que preserva o comportamento anterior.
2. **Remover ou renomear parâmetro público é major.**
3. **Nada de `Task` em `EventCallback` sem tratamento** — exceção em handler derruba o circuito no
   Blazor Server.
4. **Todo componente interativo é alcançável por teclado** e tem foco visível (RNF-05).
5. **Componente não conhece cor** — só papel (`06`).

## Política de versão

- `dev` → pré-release `0.x-alpha.<run>`, descartável.
- Tag `vX.Y.Z` → estável, presa ao commit da tag (`ADR-004`).
- A v1 começa em **`0.1.0`**. O `1.0.0` sai quando as quatro ondas fecharem, e **a partir dele o
  contrato congela**: mudança que quebra é major.

⚠️ **O feed foi limpo em 16/09/2026** — as versões `0.0.1` a `1.2.0` do projeto anterior não existem
mais. A numeração recomeça sem colidir com nada, mas **um consumidor que tenha cache local antigo
pode resolver uma versão fantasma**. Se aparecer `RVM.DesignSystem 1.1.2` em algum `restore`, é
cache, não o feed.
