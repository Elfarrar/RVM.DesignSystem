# 06 — Tokens e Tematização

> Este é o **motor do domínio** do projeto. É o que faz o RVM.DesignSystem ser um design system, e
> não uma pasta de componentes bonitos. A estrutura dos tokens está em `04-modelo-de-dados.md`;
> aqui está **como o tema é aplicado, trocado e verificado**.

## O problema concreto que este motor resolve

Hoje, para o Fiscal ficar teal e o ObraEmDia ficar verde, cada um escreveu um arquivo de tema
completo — paleta clara, paleta escura, tipografia, tudo. São ~200 linhas duplicadas por app, das
quais **duas linhas** eram realmente diferentes: a cor primária e a secundária.

O motor de tematização existe para que essas duas linhas sejam as únicas que um produto novo escreva.

## Como o tema é aplicado

### 1. O app declara sua paleta

```csharp
public static class TemaObraEmDia
{
    public static readonly RvmTheme Theme = RvmTheme.FromSeed(
        name: "ObraEmDia",
        primary: "#2A6E49",     // verde-terra
        secondary: "#B07D2A");
}
```

`FromSeed` deriva os demais papéis (containers, hover, foco, superfícies, bordas, on-*) a partir das
duas cores-semente e da escala neutra padrão, **para os dois modos**. Quem quiser controle total
sobrescreve papel a papel:

```csharp
public static readonly RvmTheme Theme = RvmTheme.FromSeed("Fiscal", "#1B5E5A", "#2A6F97") with
{
    Dark = ... with { Surface = "#1F2A29" }   // o teal escuro que o Fiscal já usa hoje
};
```

**Derivar por padrão, sobrescrever por exceção.** É o que impede o app novo de ter que entender
trinta papéis antes de desenhar a primeira tela — e o que impede o app maduro de ficar preso à
derivação quando ela não serve.

### 2. O provider emite as custom properties

`<RvmThemeProvider>` renderiza, no topo da árvore, um bloco `<style>` com o tema corrente:

```css
:root {
  --rvm-color-primary: #2A6E49;
  --rvm-color-on-primary: #FFFFFF;
  --rvm-color-surface: #FFFFFF;
  /* ... */
}
:root[data-rvm-theme="dark"] {
  --rvm-color-primary: #6FBF8E;
  --rvm-color-surface: #1E1B16;
  /* ... */
}
```

Trocar de modo é trocar o atributo `data-rvm-theme` no `<html>` — **uma escrita no DOM**, sem
re-render do Blazor, sem flash, sem recarregar. É o motivo de a paleta viver em CSS custom
properties em vez de em `style=` inline por componente.

### 3. O primeiro paint não pisca

Um script minúsculo e síncrono no `<head>` lê `localStorage['rvm-ds-theme']` (ou
`prefers-color-scheme`) e escreve `data-rvm-theme` **antes** do primeiro paint. Sem ele, o usuário
de tema escuro vê um lampejo branco em todo carregamento — defeito clássico, e especialmente feio em
Blazor Server, onde o circuito demora a subir.

Este é o único JavaScript que roda antes do Blazor. Está documentado no guia de adoção como passo
obrigatório, com o snippet pronto para colar.

## Modos e preferência

| Modo | Comportamento |
|---|---|
| `Light` | Fixo claro |
| `Dark` | Fixo escuro |
| `System` (padrão) | Segue `prefers-color-scheme` e **reage à mudança em runtime** |

A escolha do usuário é persistida no `localStorage` do navegador (único dado persistido pelo projeto
inteiro — ver `04`). O app consumidor pode ignorar isso e fixar um modo, se quiser.

## Densidade

Duas densidades, aplicadas por atributo (`data-rvm-density="compact"`), afetando **altura de controle
e espaçamento vertical** — nunca tamanho de fonte:

| Densidade | Altura de controle `md` | Uso |
|---|---|---|
| `Comfortable` (padrão) | 40px | Formulário, tela de cadastro, mobile |
| `Compact` | 32px | Grid, listagem densa, telas de operação |

Alvo de toque **nunca desce de 40px em viewport móvel**, independente da densidade escolhida — a
regra vence a configuração, porque acessibilidade não é preferência do desenvolvedor.

## Verificação de contraste — o portão que torna a acessibilidade real

Todo par `x` / `on-x` de toda paleta (clara e escura, de todo tema registrado) tem sua razão de
contraste calculada em **teste unitário**:

```csharp
[Theory]
[MemberData(nameof(TodosOsTemas))]
public void Todo_par_semantico_atende_WCAG_AA(RvmTheme tema)
```

- Texto normal: **≥ 4.5:1**
- Texto grande e limites de controle: **≥ 3:1**
- Anel de foco contra o fundo adjacente: **≥ 3:1**

Reprovou, o CI fica vermelho. Isso muda a natureza do requisito: contraste deixa de ser algo que
alguém confere na revisão visual e passa a ser algo que **não entra no repositório errado**. É a
razão de a lista de papéis semânticos em `04` ser fechada — papel sem `on-*` é par que ninguém mede.

A mesma função de cálculo alimenta a página de cor do site (`RF-24`), que mostra o número e o
veredito para o visitante. Uma implementação, dois usos.

## A identidade da marca — decidida em 07/09/2026

O design system **não define identidade de marca, consome uma** (`01` § Escopo). A que ele consome
é a do site do Rafael (`rvmtech.com.br`, produzido pelo **RVM.Curriculo**): **roxo do Visual Studio
como principal, azul do VS Code como apoio** — escolha dele em 01/09/2026, agora estendida a este
projeto.

Isso vale para **duas coisas diferentes**, e a distinção importa:

1. **O tema padrão da biblioteca** (`RvmTheme.Rvm`) — o que um app novo herda sem escolher nada.
2. **A cara do site de documentação** — que passa a parecer parte do mesmo conjunto que o currículo,
   em vez de um site genérico.

Não vale para os quatro temas de exemplo (ErpAgro, ObraEmDia, Fiscal, Propostinha): esses continuam
com as cores dos apps de origem, que é justamente o que prova o motor.

### As sementes

```csharp
public static readonly RvmTheme Rvm = RvmTheme.FromSeed(
    name: "RVM",
    primary:   "#641974",   // roxo do Visual Studio
    secondary: "#006DBD");  // azul do VS Code
```

⚠️ **São os valores RENDERIZADOS do site, não os nominais — e a diferença não é cosmética.**
O `CLAUDE.md` do RVM.Curriculo nomeia `#68217A` e `#007ACC`, mas o `global.css` os declara em OKLCH
(`oklch(38% 0.155 320)` e `oklch(52% 0.16 245)`), que renderizam mais escuros. Medindo contra o
fundo do site (`#EEEEF1`):

| Cor | Contraste vs fundo | Veredito AA |
|---|---|---|
| `#68217A` roxo nominal | 8.69 | ✅ |
| `#641974` roxo renderizado | 9.38 | ✅ |
| `#007ACC` **azul nominal** | **3.90** | ❌ **reprova** (mínimo 4.5) |
| `#006DBD` azul renderizado | 4.62 | ✅ |

**Semear com o azul nominal deixaria o CI vermelho no primeiro teste de contraste.** O site já está
correto — quem está desatualizado é o nome escrito no `CLAUDE.md` de lá. Registrado aqui para que
ninguém "corrija" `#006DBD` para `#007ACC` achando que está consertando.

### O modo escuro não existe no site — foi derivado

O `rvmtech.com.br` é `color-scheme: light`, e só. A biblioteca exige os dois modos, então a metade
escura foi **derivada mantendo matiz e croma e invertendo a luminosidade**, e verificada:

| Papel | Claro | Escuro | Contraste no escuro |
|---|---|---|---|
| `surface` | `#EEEEF1` | `#111117` | — |
| `on-surface` | `#1E1E26` | `#E7E7ED` | 15.30 ✅ |
| `on-surface-muted` | `#575760` | `#A3A4AC` | 7.57 ✅ |
| `primary` | `#641974` | `#CF8FDE` | 7.73 ✅ |
| `secondary` | `#006DBD` | `#65B2F1` | 8.25 ✅ |

⏳ **Pendência nova:** a metade escura é derivação minha, não escolha do Rafael. Ele nunca viu a
marca no escuro. Passa em AA, mas "passa em AA" e "é a cara que ele quer" são coisas diferentes —
confirmar antes da 1.0.

### Três tokens do site que não viram token semântico como estão

O portão desta biblioteca é mais rigoroso que o de um site de conteúdo, e três tokens do
RVM.Curriculo reprovariam se copiados direto:

| Token do site | Contraste vs fundo | Alvo | O que fazer aqui |
|---|---|---|---|
| `--color-text-subtle` | 3.42 | 4.5 | escurecer, ou usar **só** em texto grande (onde 3.0 basta) |
| `--color-accent-warm` | 4.42 | 4.5 | escurecer 2-3% de luminosidade |
| `--color-border-strong` | 1.67 | 3.0 | serve como borda **decorativa**; borda de controle precisa de token próprio |

Para a borda de controle (WCAG 1.4.11, limite de componente que comunica estado), os valores que
cruzam 3.0 mais perto do fundo são `oklch(63% 0.012 285)` = `#898991` no claro e
`oklch(49% 0.014 285)` = `#606069` no escuro.

**Isto não é defeito do site.** Um site de conteúdo usa essas cores em texto grande e em divisória
decorativa, onde o mínimo é 3.0 e elas passam. O que muda aqui é que um design system publica esses
nomes como contrato, e um consumidor vai usá-los em texto de 14px.

## Migrando as paletas que já existem

Não há migração de app (`01` § Escopo), mas as quatro paletas atuais entram no repositório como
**temas de exemplo** — `RvmTheme.Samples.ErpAgro`, `.ObraEmDia`, `.Fiscal`, `.Propostinha`. Servem
para três coisas:

1. provar que `FromSeed` aguenta cores reais, não só as bonitas do exemplo;
2. alimentar o seletor de tema do site de documentação, onde o visitante vê o mesmo componente nas
   quatro identidades — a demonstração mais direta da ideia "base comum, paleta por produto";
3. **submeter as cores atuais ao teste de contraste.** É bem possível que alguma das quatro reprove
   hoje; quando reprovar, o achado é registrado como issue no app de origem — sem prazo, sem mexer
   no app, apenas documentado. O tema de exemplo é ajustado para passar, com nota explicando a
   diferença.

## Escala neutra

Uma única escala neutra (`--rvm-gray-0` a `--rvm-gray-1000`) serve superfícies, bordas e texto em
todos os temas. Ela é levemente **matizada pela cor primária** (`color-mix()` com ~4%), o que faz o
cinza do Fiscal puxar para o teal e o do ObraEmDia para o verde. É um detalhe sutil e barato que
faz a interface parecer desenhada em vez de montada.

## O que este motor não faz

- **Não sincroniza com Figma.** Sem plugin, sem export de token, sem Style Dictionary na v1 — os
  tokens são escritos à mão em CSS e C#. Está no roadmap como candidato, não como dívida.
- **Não permite tema por tenant em runtime.** Paleta é por produto, resolvida na configuração da
  aplicação. Trocar por usuário logado é multi-marca, e está fora do escopo (`01`).
- **Não gera paleta automaticamente a partir de logo/imagem.** `FromSeed` parte de cores que alguém
  escolheu.
