# Product

## Register

brand

## Users

Dois leitores, e a ordem entre eles importa (`01-visao-geral.md` § Persona):

1. **O entrevistador técnico** — chega pelo `design.rvmit.com.br` sem contexto nenhum, dá dois
   minutos ao site e decide se ali tem senioridade de front-end. É quem manda quando os dois
   discordam: decisão do Rafael em 09/09/2026.
2. **A sessão que constrói o próximo app RVM** (Claude ou o Rafael). Instala o pacote do BaGet, lê
   o site, monta a tela. Mede sucesso em tempo até a primeira tela coerente.

Contexto de uso: navegador de desktop, tela larga, luz de escritório, leitura atenta e curta. O
segundo leitor volta muitas vezes ao mesmo lugar e sabe o que procura; o primeiro passa uma vez só
e não vai perguntar nada a ninguém.

## Product Purpose

Cada aplicação Blazor do ecossistema RVM inventou a própria linguagem visual do zero — quatro apps,
quatro paletas completas, cada uma com seu modo escuro escrito à mão e nenhuma sabendo das outras.
O `RVM.DesignSystem` encerra isso com duas entregas que só fazem sentido juntas: uma **biblioteca de
componentes Blazor própria** (sem MudBlazor por baixo), publicada como pacote no BaGet, e um **site
público** que é a primeira aplicação construída com ela.

O site não é subproduto. Se um componente é difícil de documentar, ele está mal projetado — e isso
aparece no mesmo dia, não seis meses depois.

Sucesso: um app RVM novo nasce coerente sem ninguém decidir espaçamento de novo; e alguém de fora
consegue julgar a engenharia por trás sem ler uma linha de código.

## Brand Personality

**Técnico, denso, elegante.**

Densidade como sinal de competência: muita informação bem organizada, e não pouca informação com
muito respiro. A tipografia trabalha; a decoração não existe. O tom do texto segue o do próprio
projeto — direto, sem jargão, explicando o *porquê* junto com o *quê*, em PT-BR.

Referências de sensação, na combinação escolhida pelo Rafael: **Material 3 e Fluent** pela regra
explícita de elevação, movimento e cor — nada acontece por acaso; **Radix** pela honestidade de
deixar o sistema à mostra, com token e componente no primeiro plano.

## Anti-references

- **MudBlazor genérico.** A tela que parece "o Material padrão que veio na caixa". A decisão de não
  usar MudBlazor está registrada em `01-visao-geral.md`; parecer com ele desfaz a decisão de graça.
- **AdminLTE.** Tentado e **reprovado pelo Rafael** (`DSGN-027` → `DSGN-028`): bloco escuro sólido,
  marca d'água, faixa de rodapé. Não recriar sem pedido.
- **Landing de SaaS.** Número gigante com rótulo pequeno, grade de cards idênticos com ícone +
  título + parágrafo, gradiente em texto. O `08-monetizacao.md` deixa claro que aqui não se vende
  nada.
- **Documentação que só mostra o resultado.** Galeria bonita sem o código, sem o porquê e sem o
  estado de erro — o site existe justamente para provar o contrário.

## Design Principles

1. **Pratique o que você prega.** O site é feito com a biblioteca, usando o `RvmAppShell`. Nenhuma
   tela do site pode precisar de CSS que a biblioteca não ofereça a um consumidor.
2. **Token antes de componente.** O que padroniza não é o botão, é a escala. Componente nunca lê
   token primitivo (`--rvm-blue-500`), só papel semântico (`--rvm-color-primary`) — e há teste que
   reprova o build quando alguém esquece.
3. **A plataforma primeiro.** Quando o navegador ou o leitor de tela já resolve, reimplementar
   entrega pior: `<select>` nativo, `<dialog>` com `showModal()`, `<table>` semântica sem
   `role="grid"`. Três desvios declarados, nenhum deles acidente.
4. **Acessibilidade é critério de aceite, não fase.** Contraste medido em teste unitário para todo
   par de paleta, axe no E2E em toda página nos dois modos. Abaixo de AA, o CI fica vermelho.
5. **Mais fácil de explicar ganha de mais poderoso.** Mesma regra do RVM.Mimic, pelo mesmo motivo:
   o que não se explica em duas frases não se adota.

## Accessibility & Inclusion

- **WCAG 2.1 nível AA** como portão de CI, não como intenção: 4,5:1 em texto, 3:1 em texto grande e
  borda, verificado por teste em **todo par `x`/`on-x` de toda paleta**.
- Teclado completo em todo componente, foco sempre visível, e o link de pulo como primeiro focável.
- `prefers-reduced-motion` respeitado em toda animação.
- Interrupção de leitor de tela (`role="alert"`, `aria-live="assertive"`) é **opt-in**, nunca padrão.
- Cor nunca é o único portador de significado — estado sempre tem texto ou ícone junto.
- Conteúdo em **pt-BR fixo**, com todo texto padrão sobrescrevível por parâmetro.
