# CLAUDE.md — RVM.DesignSystem

Guia de desenvolvimento do projeto. Complementa as diretrizes globais
(`C:\Users\rvene\.claude\CLAUDE.md`) e o padrão do ecossistema (skill `padrao-rvm`).

**Prefixo de task:** `DSGN-NNN` · contador próprio, **reiniciado em `DSGN-001`** · card em
`docs/Vault/02_Tasks/`, índice em `docs/Vault/03_Kanban/KANBAN.md`.

> **Estado em 17/09/2026 — `1.0.0` publicado e producao no ar (`DSGN-007`).** O projeto foi
> **recriado** em 16/09: a encarnação anterior (07–16/09) foi apagada por inteiro a mando do Rafael,
> sem backup. **Nada foi herdado**.
>
> `DSGN-001` a `DSGN-006`: bootstrap, tokens e os 35 componentes das quatro ondas (mais o relógio
> circular do `RvmTimePicker`), todos aprovados pelo Rafael. **O contrato da API está congelado no
> `1.0.0`**: mudança que quebra consumidor é major. Dev em `design.dev.rvmtech.com.br`, produção em
> `design.rvmit.com.br` (GitHub Pages, monitor 25 do Kuma).
>
> Depois da `1.0.0`, tudo aprovado e em produção: `DSGN-008` (o site passou a usar o próprio
> `RvmAppShell`, com menu lateral), `DSGN-009` (cinco telas de exemplo) e `DSGN-010` (os oito
> gráficos, publicados na `1.1.0`) e `DSGN-011` (exportar, eixo duplo, zoom, arrastar e seleção nos
> gráficos, publicada na **`1.2.0`** em 17/09). Em 18/09, `DSGN-012` (documentação dos gráficos) e
> `DSGN-013` (eixo duplo em barras, zoom por caixa, exportar `.xlsx` e imprimir) saíram na **`1.3.0`**.
> Alphas de `dev` são `1.4.0-alpha.N`.

## O visual vem do NEATLAB — e o crédito é obrigatório

> Kit **NEATLAB — Super Admin Dashboard UI Design Kit**, de **`hello.uiworld`**, Figma Community,
> licença **CC BY 4.0**.

Uso comercial e derivados são permitidos **com atribuição**. O crédito aparece no `README.md`, no
rodapé do site de documentação e na descrição do pacote NuGet. **Repositório público sem o crédito é
violação de licença.** Não remover, não reduzir a nota de rodapé escondida.

**A referência vive em `referencia-neatlab/`** (536 arquivos, 99 MB): PNG de 141 telas em claro e
escuro. É contra esses PNGs que a fidelidade é conferida.

⚠️ **Os SVGs de lá não servem** — todos os 257 são PNG embutido numa tag `<svg>`, sem vetor e sem
texto vivo. Foram exportados do arquivo Community em modo visualização, que rasteriza. **Não perca
tempo tentando extrair path ou `font-family` deles.**

## Escopo

**Faz:** biblioteca Blazor própria (RCL) no BaGet, ~35 componentes na v1 em quatro ondas · camada de
tokens + tema claro/escuro · site público de documentação feito com a própria biblioteca ·
acessibilidade AA como critério de aceite.

**Não faz:** MudBlazor ou qualquer biblioteca de terceiros · migrar os apps existentes · backend,
banco, auth, container · as telas prontas do kit (login, invoice, chat) · editor rico, RTL.

> **Gráfico saiu do "não faz" em 17/09/2026** (`DSGN-010`, decisão do Rafael): oito tipos em SVG próprio,
> sem biblioteca de terceiros — colunas, barras, histograma, linha, área, dispersão, pizza/rosca e radar.
>
> **E na mesma data a `DSGN-011` trouxe o resto** ("não deixe nada de fora"): animação, exportar
> (PNG, SVG, CSV e PDF, o PDF montado byte a byte em C#), eixo duplo, zoom em X e em Y, arrastar e
> seleção de faixa por arrasto para filtrar outros componentes. **Continuam fora:** zoom por caixa
> desenhada, `.xlsx` nativo e imprimir.

## Arquitetura

⚠️ **NÃO segue VSA, e é decisão registrada** (`03-arquitetura.md` ADR-001). VSA organiza caso de uso
que atravessa camadas; aqui não há caso de uso, endpoint nem banco — o artefato é uma Razor Class
Library e a unidade é o componente. **Não "corrigir" para VSA.** MediatR não entra (proibido no
ecossistema, e sem sentido numa biblioteca de UI).

- **Solution:** `src/RVM.DesignSystem` (RCL) · `src/RVM.DesignSystem.Docs` (WASM) ·
  `test/RVM.DesignSystem.Tests` (bUnit) · `test/playwright/RVM.DesignSystem.E2E`
- ⚠️ **O E2E mora sob `test/playwright/` de propósito** — o `ci.yml` roda todo `test/**/*.csproj` e
  pula caminhos com `playwright`; este E2E precisa do site no ar.
- **Blazor Server E WebAssembly**, sempre. Nenhum componente depende de JS para o estado inicial.
- **CSS isolado** por componente, consumindo **só** custom properties. Hex literal em CSS de
  componente é bug — quebra o tema escuro.

## Convenções

- Prefixo `Rvm` em todo componente público.
- ⚠️ **Prefixo `rvm-` em toda classe CSS interna** (`.rvm-caixa`, nunca `.caixa`). CSS isolado
  impede o componente de vazar, **não** de receber regra global: `.conteudo` do consumidor casava
  com o `.conteudo` do diálogo. Decisão do Rafael em 17/09/2026; o teste
  `Toda_classe_de_componente_tem_o_prefixo_rvm` barra classe nova sem prefixo.
- **API em inglês, texto ao usuário final em PT-BR** explicativo.
- Enum, nunca string mágica. `AdditionalAttributes` sempre repassado ao elemento raiz.
- ⚠️ **Componente de formulário renderiza `name`, `id` e `aria-*`.** O `RvmTextField` anterior não
  renderizava `name`, que o form em SSR estático exige para o binding do POST — o consumidor teve
  que contornar por fora.
- Todo interativo: alcançável por teclado, foco visível, todos os estados (normal, hover, foco,
  ativo, desabilitado, erro, carregando).

## Qualidade — portões

1. Cobertura **≥ 80%**
2. **axe** no E2E — violação séria reprova
3. **Zero warning** em `Release` (`TreatWarningsAsErrors` na biblioteca)
4. Contraste **≥ 4.5:1** nos dois temas
5. **Screenshot lado a lado com o PNG do kit**, aprovado pelo Rafael

Sem o item 5 a entrega está incompleta.

## Infra

| Ambiente | Domínio | Branch | Onde |
|---|---|---|---|
| Dev | `design.dev.rvmtech.com.br` | `dev` | Rivendell — estático por `rsync` |
| Demo | — | — | **não existe, de propósito** |
| Prod | `design.rvmit.com.br` | `master` | GitHub Pages |

⚠️ **Os dois domínios foram apagados em 16/09** (CNAME, registro A, vhost, certificado). O bootstrap
recria. Zonas de antes: `/opt/dns-<zona>-pre-design-20260916.json` na Rivendell.

⚠️ **Verificação de deploy é por CONTEÚDO, não por status.** O `try_files` do Nginx e o `404.html`
do Pages devolvem 200 para qualquer caminho — build quebrado responde 200 alegremente. Procurar a
string `RVM Design System`. Monitor no Uptime-Kuma: tipo `keyword`, **vinculado ao canal de e-mail**.

⛔ **Os cinco workflows são PRÓPRIOS, em `ubuntu-latest`** — repositório público não consegue chamar
reusable de repositório privado, e o `RVM.Actions` é privado: o run morre em **0 s, zero jobs, sem
mensagem útil**. De quebra, **não precisa de runner self-hosted**.

⛔ **Publish no BaGet** — quatro armadilhas já pagas uma vez:
1. A rede runner→BagEnd é **intermitente**: retry é obrigatório.
2. `timeout` **por fora** do `dotnet nuget push`: o `--timeout` governa a requisição, mas quem
   pendura é o fetch do índice do serviço, e uma tentativa pendurada consome o job (medido: 1908 s).
3. Feed sempre **`https`** — em `http` o push não dá erro, **pendura**.
4. **Conferir a versão no feed depois**: `--skip-duplicate` sai com sucesso sem publicar nada.

⛔ **`gh secret set --body '/caminho'` no Git Bash grava valor errado** — o MSYS converte o caminho
e `/var/www/design-dev` vira `C:/Program Files/Git/var/www/...` dentro do secret; o deploy quebra lá
na frente com um erro que não aponta a causa. **Sempre por stdin.**

## Segredos

| Segredo | Onde mora | Secret no GitHub |
|---|---|---|
| SSH da Rivendell | `RVM.Infra/docs/ACESSOS.md` | `DEV_VPS_HOST/PORT/USER/SSH_KEY_B64/DEPLOY_PATH` |
| API key do BaGet | `ACESSOS-E-SENHAS.md` global | `BAGET_API_KEY` |
| URL do feed | `ACESSOS-E-SENHAS.md` global | `BAGET_FEED_URL` |

**Sem `.env`** — não há aplicação servidora. `docs/ACESSOS.md` gitignored desde o primeiro commit.

## Versão

`dev` → `0.x-alpha.<run>` descartável · **tag `vX.Y.Z` → estável, presa ao commit**. Começa em
`0.1.0`; `1.0.0` quando as quatro ondas fecharem, e aí o contrato congela.

⚠️ **Versão nunca sai de literal no csproj** — vira alvo móvel e reabre a decisão de não fazer
backup do BaGet.

## Workflow

Card `DSGN-NNN` → branch `dsgn-NNN` de `master` → commits isolados (`git add <arquivos>`, nunca
`-A`) → testes ≥ 80% → E2E → review → `dev` → validação do Rafael **pelo screenshot** → `master` só
com sinal verde explícito dele. Não há `demo`: de `dev` direto para `master`.

## Decisões que não se reabrem sem o Rafael

- **Sem MudBlazor** e sem biblioteca de terceiros — decisão dele, mantida em 16/09/2026.
- **Não segue VSA** — é RCL (ADR-001).
- **CI próprio**, não `RVM.Actions` (ADR-002) — restrição técnica, não preferência.
- **Adoção nunca é retroativa.** TradeBinder e Cockpit **não** são consumidores: a UI dos dois está
  pendente e a decisão é dele, projeto a projeto.
- **Ícones Tabler** (ADR-005) e **Inter servida pelo pacote** (ADR-006).

## Pendências ⏳ (detalhe em `09-roadmap.md`)

> **Resolvidas por amostragem de pixel em 18/09/2026 (`DSGN-014`).** O Rafael confirmou que no Figma
> o kit também é uma imagem, não objetos de desenho: não há valor para ler por API, em lugar nenhum.
> A ferramenta é `tools/amostragem-do-kit.py`.

1. ✅ **24 elevações medidas** (ajuste conjunto — elas se sobrepõem na grade; resíduo 0,008). Os cinco
   níveis da biblioteca levam as elevações 1, 3, 7, 12 e 20. A sombra do kit **não é preta**: escurece
   menos o azul.
2. ✅ **Alphas do tema escuro**: primário 0,87 e secundário 0,68 confirmados; **desabilitado era 0,38 e
   o kit usa 0,26** — corrigido nos dois temas.
3. ⏳ **Decisão do Rafael.** O fundo suave "roxo" é `#9155FD` a 12%, um roxo que **não existe na paleta
   do kit** (primary é `#264CC8`) — resquício do template de origem. Harmonizar daria `#E5EAF8`.
4. ✅ **Encerrada:** não há vetor para exportar em lugar nenhum. **Tabler (ADR-005) é definitivo.**
