# 05 — API Pública dos Componentes

> Este arquivo ocupa o lugar do `05-api.md` do padrão do ecossistema. **Não há API HTTP** neste
> projeto — não há servidor, endpoint nem contrato REST. A API pública aqui é a **superfície C#/Razor
> que o app consumidor enxerga**: nomes de componente, parâmetros, eventos e serviços. É contrato
> igual: publicado no BaGet, quebrar dói em todo consumidor.

## Convenções de nome

| Elemento | Convenção | Exemplo |
|---|---|---|
| Componente | `Rvm` + substantivo em inglês | `RvmButton`, `RvmDataGrid` |
| Parâmetro | PascalCase em inglês | `Variant`, `Size`, `Disabled` |
| Evento | `On` + verbo no passado | `OnClick`, `OnRowSelected` |
| Enum de variante | `Rvm` + componente + conceito | `RvmButtonVariant.Primary` |
| Serviço | `IRvm` + papel + `Service` | `IRvmToastService` |
| Classe CSS pública | `rvm-` + componente + elemento | `rvm-button__icon` |

**Por que inglês numa base de código de ecossistema PT-BR:** a regra do `padrao-rvm` §9 é sobre
*texto visível ao usuário final* — esse continua em PT-BR, inclusive os textos padrão dos
componentes. Nome de API pública de biblioteca de UI é outra coisa: quem lê é desenvolvedor, e todo
material de referência (ARIA, MDN, Blazor) está em inglês. Misturar `RvmBotao.Tamanho` com
`aria-expanded` produz código pior. Decisão registrada aqui para não ser reaberta a cada componente.

## Parâmetros comuns a todo componente

```csharp
[Parameter] public string? Class { get; set; }          // concatenado, nunca substitui o interno
[Parameter] public string? Style { get; set; }
[Parameter(CaptureUnmatchedValues = true)]
public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
[Parameter] public string? Id { get; set; }             // se nulo, gerado; usado por aria-*
```

Regras que valem para os quatro, sem exceção:

- `Class` **soma** com as classes internas do componente. Um componente que descarte o `Class` do
  consumidor obriga o consumidor a lutar com `!important` — e é assim que design system morre.
- `AdditionalAttributes` é repassado ao **elemento raiz semântico** (o `<button>`, não a `<div>`
  de fora), para que `data-testid`, `title` e `aria-*` cheguem onde importam.
- `Id` gerado é estável entre renders (não pode mudar a cada re-render, senão o `aria-describedby`
  aponta para o nada).

## Componentes de entrada (`@bind-Value`)

Todo campo segue o contrato nativo do Blazor, sem invenção:

```csharp
[Parameter] public TValue? Value { get; set; }
[Parameter] public EventCallback<TValue?> ValueChanged { get; set; }
[Parameter] public Expression<Func<TValue?>>? ValueExpression { get; set; }
```

O que dá `@bind-Value="modelo.Nome"` funcionando, e `<ValidationMessage For="..."/>` funcionando
junto, porque os campos herdam de `InputBase<TValue>`. **Não há motor de validação próprio**:
`DataAnnotations` e `EditContext` são do framework, e o componente apenas reflete o estado
(`aria-invalid`, borda de erro, mensagem ligada por `aria-describedby`).

Parâmetros de campo, comuns a todos:

```csharp
Label, Placeholder, HelperText, ErrorText, Required, Disabled, ReadOnly,
Size (Sm|Md|Lg), FullWidth, StartIcon, EndIcon
```

`ErrorText` explícito vence a validação do `EditContext` — é a saída para erro que veio do servidor.

## Exemplo canônico — `RvmButton`

```razor
<RvmButton Variant="RvmButtonVariant.Primary"
           Size="RvmSize.Md"
           Loading="@salvando"
           StartIcon="save"
           OnClick="SalvarAsync">
    Salvar
</RvmButton>
```

```csharp
public enum RvmButtonVariant { Primary, Secondary, Tertiary, Ghost, Danger }
public enum RvmSize { Sm, Md, Lg }

[Parameter] public RvmButtonVariant Variant { get; set; } = RvmButtonVariant.Primary;
[Parameter] public RvmSize Size { get; set; } = RvmSize.Md;
[Parameter] public bool Loading { get; set; }
[Parameter] public bool Disabled { get; set; }
[Parameter] public bool FullWidth { get; set; }
[Parameter] public string? StartIcon { get; set; }
[Parameter] public string? EndIcon { get; set; }
[Parameter] public string ButtonType { get; set; } = "button";
[Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
[Parameter] public RenderFragment? ChildContent { get; set; }
```

Comportamento que a documentação promete e o teste garante:

- `Loading` implica `Disabled` e troca o `StartIcon` pelo spinner, **mantendo a largura do botão**
  (senão o layout salta e o usuário perde o alvo do clique).
- `Loading` expõe `aria-busy="true"`; `Disabled` usa o atributo nativo `disabled`, não `aria-disabled`
  (botão desabilitado de verdade não recebe foco, e é o comportamento esperado num formulário).
- `ButtonType="submit"` dentro de `EditForm` submete — nenhuma mágica de interceptação.

## Enums compartilhados

Um único `RvmSize` (`Sm|Md|Lg`) e um único `RvmSeverity`
(`Info|Success|Warning|Danger`) para toda a biblioteca. **Não** haverá `RvmButtonSize` +
`RvmInputSize` + `RvmChipSize` — três enums idênticos são três lugares para divergir.

## Serviços

```csharp
public interface IRvmToastService
{
    void Show(string mensagem, RvmSeverity severidade = RvmSeverity.Info, RvmToastOptions? opcoes = null);
    void Success(string mensagem);
    void Error(string mensagem);
    void Clear();
}

public interface IRvmDialogService
{
    Task<RvmDialogResult> ShowAsync<TComponent>(string titulo, RvmDialogParameters? parametros = null)
        where TComponent : ComponentBase;
    Task<bool> ConfirmAsync(string titulo, string mensagem,
                            string textoConfirmar = "Confirmar", string textoCancelar = "Cancelar");
}
```

`ConfirmAsync` existe como atalho porque confirmação de exclusão é o diálogo mais escrito de qualquer
app RVM — hoje, reescrito em quatro lugares. Retorna `bool`, não `RvmDialogResult`, porque quem
pergunta "confirma?" quer sim ou não.

## Registro no consumidor

```csharp
builder.Services.AddRvmDesignSystem(opcoes =>
{
    opcoes.Theme = TemaObraEmDia.Theme;      // paleta do produto
    opcoes.DefaultMode = RvmThemeMode.System; // Light | Dark | System
});
```

```razor
@* App.razor / MainLayout.razor *@
<RvmThemeProvider>
    <RvmToastHost />
    <RvmDialogHost />
    @Body
</RvmThemeProvider>
```

Quatro linhas no `Program.cs` e um wrapper no layout — é o teto de fricção aceitável para adoção.
Se a instalação exigir mais que isso, a API está errada.

## Política de versão (SemVer)

| Mudança | Versão | Como se faz |
|---|---|---|
| Componente novo, parâmetro novo **com padrão** | minor | Direto |
| Correção de bug sem mudar assinatura | patch | Direto |
| Renomear ou remover parâmetro | major | `[Obsolete("use X")]` numa minor antes; só some na major seguinte |
| Mudar valor padrão de parâmetro | major | Mesmo sem quebrar compilação, muda a tela de todo mundo em silêncio — o pior tipo |
| Renomear token CSS | major | Alias mantido por uma minor |
| Ajuste de cor/espaçamento dentro do token existente | minor | Documentado no changelog com antes/depois |

Toda linha desta tabela vira uma entrada no changelog do site (`RF-26`), com breaking change marcada
em destaque. Consumidor fixa versão exata; ninguém é atualizado por acidente.

## O que a API deliberadamente não expõe

- **Nenhum `RenderFragment` mágico sem nome.** Slots nomeados (`HeaderContent`, `FooterContent`), para
  que o consumidor descubra pela IntelliSense em vez de pela documentação.
- **Nenhum parâmetro `object`.** Se o tipo não dá para expressar, o componente está mal recortado.
- **Nenhum acesso ao DOM interno.** Sem `ElementReference` público, sem "pegue o input por dentro".
  Precisa disso? É parâmetro faltando — e vira issue, não gambiarra.
