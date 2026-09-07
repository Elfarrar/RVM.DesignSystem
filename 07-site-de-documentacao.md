# 07 — Site de Documentação

> Ocupa o lugar do `07-painel-blazor.md` do padrão do ecossistema. Não há painel administrativo neste
> projeto: a única interface é **pública, anônima e estática**.

## O que é

`design.rvmit.com.br` — um Blazor WebAssembly publicado como site estático, que documenta a
biblioteca **usando a biblioteca**. Não há servidor, não há login, não há banco.

**Dogfooding é regra, não estilo:** o site é construído inteiramente com componentes `Rvm*`. A casca
é `RvmAppShell`, a navegação é `RvmSidebar`, os avisos são `RvmAlert`, o seletor de tema é
`RvmSelect`. A consequência prática é dura de propósito — se um componente for desagradável de usar,
quem sofre primeiro sou eu, montando a página dele.

## Estrutura de navegação

```
Início              — o que é, para quem, instalação em 4 linhas
Começar
  Instalação        — pacote, DI, provider, o script anti-flash
  Primeira tela     — do zero a um formulário funcional
  Migrando do Mud   — equivalências, para quem vier de um app existente
Fundamentos
  Cor               — paleta, papéis semânticos, contraste medido (RF-24)
  Tipografia        — escala, pesos, quando usar cada nível
  Espaçamento       — escala, ritmo vertical, densidade
  Raio e sombra     — elevação e o que ela comunica
  Ícones            — grade completa, busca por nome, clique para copiar
  Motion            — durações, curvas, prefers-reduced-motion
Componentes
  Básicos / Layout / Feedback / Dados   — uma página por componente
Padrões
  Formulário        — validação, erro do servidor, campos obrigatórios
  Listagem          — grid + filtro + paginação como um conjunto
  Estados vazios    — vazio, erro, carregando, sem permissão
Sobre
  Acessibilidade    — o que garantimos e como verificamos
  Changelog         — versões, breaking changes
```

## Anatomia de uma página de componente

Toda página de componente tem exatamente as mesmas seções, na mesma ordem — previsibilidade é o que
faz documentação ser consultada em vez de lida:

1. **Nome e uma frase** dizendo quando usar (e quando não usar).
2. **Exemplo principal**, renderizado ao vivo, com o Razor copiável ao lado.
3. **Playground** — controles que alteram as props e regeneram o exemplo **e o código** (`RF-22`).
4. **Variantes** — uma linha por variante, todas visíveis de uma vez.
5. **Estados** — normal, hover, foco, desabilitado, carregando, erro.
6. **Tabela de API** (`RF-29`) — parâmetro, tipo, padrão, descrição. Gerada da documentação XML do
   código, não escrita à mão: doc escrita à mão desatualiza no primeiro parâmetro novo.
7. **Acessibilidade** — teclas suportadas, papéis ARIA aplicados, o que o leitor de tela anuncia.
8. **Quando não usar** — a seção que impede o design system de virar catálogo. Ex.: "não use
   `RvmDialog` para confirmar ação reversível; use um `RvmToast` com desfazer."

## O playground

Cada página declara os controles do seu exemplo:

```csharp
new PlaygroundSpec<RvmButton>()
    .Enum(x => x.Variant)
    .Enum(x => x.Size)
    .Bool(x => x.Disabled)
    .Bool(x => x.Loading)
    .Text(x => x.StartIcon, "save")
    .Slot("Salvar");
```

Isso gera três coisas de uma fonte só: os controles, o componente renderizado e o snippet Razor
correspondente — **com os valores atuais**, omitindo o que estiver no padrão (código de exemplo com
`Variant="Primary"` explícito ensina o hábito errado).

Declarativo, não por reflexão automática: reflexão sobre todos os parâmetros produziria trinta
controles inúteis por página. O autor da página escolhe os quatro que importam.

## Seletor de tema — a demonstração central

No topo do site, dois controles:

- **modo**: claro / escuro / sistema;
- **tema**: RVM (padrão) · ERPAgro · ObraEmDia · Fiscal · Propostinha.

Trocar o segundo repinta o site inteiro na identidade daquele produto, instantaneamente. É a prova
visual da tese do projeto — os mesmos componentes, quatro identidades, zero código diferente — e é
provavelmente o que um entrevistador vai lembrar depois de fechar a aba.

## Requisitos próprios do site

- **Acessível ele mesmo.** Auditoria axe por página no E2E; violação séria reprova o build. Um site
  de design system com problema de acessibilidade destrói a própria credibilidade.
- **Funciona sem JavaScript pesado**, com navegação por teclado completa e `skip to content`.
- **Rota por componente** (`/componentes/button`) — a URL é o que se cola em conversa.
- **Modo escuro do site** persistido (`RF-28`), com o mesmo mecanismo anti-flash do `06`.
- **Sem analytics, sem cookie, sem CDN externo.** Fonte e ícone auto-hospedados (mesma razão do
  `02` § Segurança). Sem cookie, não há banner de consentimento — e não há o que explicar em política
  de privacidade além de "este site não coleta nada".
- **`<meta>` de compartilhamento** com imagem — o link vai ser colado em lugar público.

## Por que Blazor WASM, e não Astro

O padrão de landing do ecossistema (ADR-009) manda Astro em `landing/` do repositório do produto.
**Não se aplica aqui**, por uma razão que não é preferência:

O site precisa **executar os componentes Blazor de verdade**, com o playground alterando parâmetros
em runtime. Em Astro, isso exigiria reimplementar cada componente em outra tecnologia — e a
documentação passaria a mentir no dia em que as duas versões divergissem. Documentação de biblioteca
Blazor tem que ser Blazor.

Continua valendo do ADR-009: publicação como **estático**, sem servidor de aplicação, por workflow
próprio com filtro de `paths`.

## Não é landing de produto

Não há preço, não há CTA de cadastro, não há `08-monetizacao.md` para conferir. A "conversão" que
este site persegue é outra: que a próxima sessão de desenvolvimento consiga montar uma tela sem
perguntar nada a ninguém.
