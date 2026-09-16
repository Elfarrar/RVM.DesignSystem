# 09 — Roadmap

## Fase 0 — bootstrap (`DSGN-001`)

Repositório público MIT, esqueleto `.slnx` (RCL + Docs WASM + bUnit + Playwright), os cinco
workflows próprios, `.gitattributes` com `eol=lf`, CI verde, e o crédito CC BY no README desde o
primeiro commit. DNS, vhost, certificado e Pages recriados (foram apagados em 16/09).

**Verifica:** CI verde · site de dev no ar respondendo com a string esperada · pacote `0.1.0-alpha`
no feed.

## Onda 1 — tokens + 9 componentes (`DSGN-002` …)

Primeiro os tokens do `06`, depois os componentes da onda 1 do `11`. A ordem importa: componente
antes de token vira hex solto no CSS, e aí o tema escuro não fecha.

**Verifica:** tema claro e escuro trocando sem recarregar · cobertura ≥ 80% · axe sem violação séria
· screenshot lado a lado com o PNG do kit aprovado pelo Rafael.

## Onda 2 — formulário e navegação (10 componentes)
## Onda 3 — feedback e sobreposição (8 componentes)
## Onda 4 — dados e shell (8 componentes)

Cada onda fecha com: todos os estados obrigatórios, página no site de documentação, testes, axe
limpo e aprovação por screenshot.

## `1.0.0`

Sai quando as quatro ondas fecharem. **A partir dele o contrato congela** — mudança que quebra é
major. Antes disso, a versão é `0.x` e pode mexer em API sem cerimônia.

## Adoção

**Nenhum projeto adota automaticamente.** TradeBinder e Cockpit estão com a UI pendente e a decisão
é do Rafael, projeto a projeto. Os apps em MudBlazor (ERPAgro, ObraEmDia, Fiscal, Propostinha)
**não migram** — adoção nunca é retroativa.

## Pendências que mudam o modelo de dados

Não há banco, então nada bloqueia migration. O que **bloqueia decisão de token** — e por isso mora
aqui — é:

1. ⏳ **Os `box-shadow` das 24 elevações.** O kit mostra os quadrados e não escreve os valores. Sem
   isso, a v1 usa 5 elevações aproximadas por medição visual. **Resolve-se duplicando o kit para a
   conta do Rafael e lendo pela API do Figma.**
2. ⏳ **Alpha do texto secundário e desabilitado no tema escuro.** Os três swatches foram desenhados
   com a mesma cor sólida no PNG; a diferença real é opacidade, que o bitmap não guarda. Chutar
   estraga o contraste, que é critério de aceite (RNF-02).
3. ⏳ **Fundos suaves do tema claro são roxos com um primary azul** (`#F2EAFF` contra `#264CC8`).
   Manter fiel ao kit ou harmonizar com o azul? **Decisão do Rafael no `DSGN-002`** — fidelidade e
   coerência brigam, e o custo de trocar depois cresce a cada componente que usar o token.
4. ⏳ **Ícones em vetor.** Os do kit vieram rasterizados; a v1 usa Tabler. Se o Rafael reexportar os
   originais, a troca é de arquivo, não de arquitetura — mas quanto mais tarde, mais componentes
   para revisitar.

Os quatro têm a mesma origem: **o kit foi exportado do arquivo Community em modo visualização**, que
rasteriza. Duplicar para a conta dele e reexportar resolve os quatro de uma vez.
