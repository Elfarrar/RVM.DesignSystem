# 04 — Modelo de Dados

## Não há banco de dados

Declarado de forma explícita, porque o checklist do ecossistema exige esta seção e a ausência de
banco é uma decisão, não um esquecimento:

- Sem PostgreSQL, sem `DbContext`, sem EF Core, sem migration.
- Sem `CompanyId`, sem global query filter, sem soft-delete, sem `Active = true`.
- O site de documentação é **estático**: não grava nada no servidor, não tem formulário que persiste,
  não tem conta de usuário.
- A única coisa que persiste em qualquer lugar é a **preferência de tema do visitante**, no
  `localStorage` do próprio navegador (chave `rvm-ds-theme`, valores `light` | `dark` | `system`).

**O que ocupa o lugar do modelo de dados neste projeto é o modelo de tokens.** Ele tem a mesma
propriedade que faz um esquema de banco ser caro: nome publicado é contrato, e renomear depois quebra
todo consumidor. Por isso está aqui, e por isso as pendências dele estão no fim do `09-roadmap.md`.

## O modelo de tokens

### Três camadas

| Camada | Exemplo | Quem consome | Muda quando |
|---|---|---|---|
| **Primitivo** | `--rvm-blue-500: #0288d1` | Só a camada semântica | Quase nunca |
| **Semântico** | `--rvm-color-primary: var(--rvm-blue-500)` | Componentes e apps | Ao trocar a paleta do produto |
| **De componente** | `--rvm-button-height-md: 2.5rem` | Um componente só | Ao ajustar aquele componente |

**Regra dura:** componente lê semântico ou de componente; **nunca** primitivo. Um `RvmButton` que
escreva `var(--rvm-blue-500)` fica azul no Fiscal, que é teal — e é exatamente o defeito que este
projeto existe para eliminar. Teste automatizado varre o CSS compilado e reprova o build se um
seletor de componente referenciar um token primitivo.

### Convenção de nome

```
--rvm-<categoria>-<papel>[-<variante>][-<estado>]

--rvm-color-primary            --rvm-color-primary-hover
--rvm-color-on-primary         --rvm-color-surface-raised
--rvm-space-4                  --rvm-radius-md
--rvm-font-size-body           --rvm-shadow-2
--rvm-motion-duration-fast     --rvm-z-dialog
```

Sempre em inglês, sempre `kebab-case`, sempre com o prefixo `rvm`. O prefixo não é enfeite: é o que
permite o pacote conviver com o CSS de um app que tenha suas próprias custom properties.

### Escalas

| Categoria | Escala | Observação |
|---|---|---|
| Espaçamento | `0, 1, 2, 3, 4, 5, 6, 8, 10, 12, 16, 20, 24` → `0` a `6rem` | Base 4px. Sem valor fora da escala em componente |
| Raio | `none, sm, md, lg, full` | `md` é o padrão de card e input |
| Tipografia | `display, h1..h6, body-lg, body, body-sm, caption, code` | Cada uma define tamanho, peso, entrelinha e tracking |
| Sombra | `0..5` | Sobe com a elevação; no tema escuro vira também mudança de superfície |
| Motion | `instant, fast (120ms), base (200ms), slow (320ms)` + curvas | Zerado sob `prefers-reduced-motion` |
| Breakpoint | `sm 640, md 768, lg 1024, xl 1280, 2xl 1536` | Mesmos nomes no C# e no CSS |
| Z-index | `dropdown 1000, sticky 1100, drawer 1200, dialog 1300, toast 1400, tooltip 1500` | Faixa fechada; app não deve competir |

### Papéis semânticos de cor

Os papéis que todo tema (claro, escuro, de qualquer produto) obrigatoriamente define:

```
primary / on-primary / primary-container / on-primary-container
secondary / on-secondary
success / warning / danger / info  (+ on-* de cada)
surface / on-surface / surface-raised / surface-sunken
background / on-background
border / border-strong
disabled / on-disabled
focus-ring
```

O par `x` / `on-x` é o coração da acessibilidade: **todo par tem contraste calculado em teste**, e
uma paleta que reprove derruba o CI. É por isso que a lista é fechada — um papel sem `on-*` seria
um par que ninguém verifica.

## O espelho tipado em C#

```csharp
public sealed record RvmTheme
{
    public required string Name { get; init; }
    public required RvmPalette Light { get; init; }
    public required RvmPalette Dark { get; init; }
    public RvmTypography Typography { get; init; } = RvmTypography.Default;
    public RvmDensity Density { get; init; } = RvmDensity.Comfortable;
    public RvmRadius Radius { get; init; } = RvmRadius.Default;
}

public sealed record RvmPalette
{
    public required string Primary { get; init; }
    public required string OnPrimary { get; init; }
    // ... um por papel semântico
}
```

O `RvmThemeProvider` emite essas propriedades como um bloco `:root { --rvm-*: ... }` no início do
documento. O C# é a **fonte da verdade do tema do app**; o CSS é a superfície que os componentes
consomem. Detalhe de aplicação em `06-tokens-e-tematizacao.md`.

## Estado em memória (o que não é dado, mas tem forma)

Três estruturas vivem em runtime e valem ser nomeadas, porque aparecem na API pública:

| Estrutura | Onde | Forma |
|---|---|---|
| Fila de toasts | `IRvmToastService` | `Id`, `Severidade`, `Titulo`, `Mensagem`, `Duracao`, `Acao?` |
| Pilha de diálogos | `IRvmDialogService` | `Id`, tipo do componente, parâmetros, `TaskCompletionSource<RvmDialogResult>` |
| Estado do grid | `RvmDataGrid<T>` | `Pagina`, `TamanhoPagina`, `Ordenacao (campo, direção)`, `Selecionados`, `Filtros` |

O estado do grid é **serializável de propósito** (record simples, sem referência a componente): é o
que permite um app guardar a preferência de listagem do usuário na URL ou no banco *dele*, sem que a
biblioteca precise saber que banco é esse.

## Pendências que travam o contrato

Vão repetidas no fim do `09-roadmap.md`, porque aqui têm o mesmo peso que "pendência que muda
migration" tem num projeto com banco: **depois de publicada a 1.0, mudar qualquer uma quebra
consumidor.**

1. ⏳ Nome dos papéis semânticos em inglês (`surface`, `on-surface`) versus português (`superficie`).
   Suposição vigente: **inglês** — é o vocabulário universal de design token e o que qualquer
   referência externa usa; o PT-BR do ecossistema vale para texto de usuário final, não para nome de
   API.
2. ⏳ A escala tipográfica é em `rem` com base 16px, ou fluida com `clamp()`? Suposição vigente:
   **`rem` fixo**, com fluidez só no `display` — previsível é mais importante que elegante numa v1.
3. ⏳ O conjunto de ícones (ver `03-arquitetura.md`). Nome de ícone é API pública tanto quanto nome
   de token.
