# 02 — Requisitos

## Quem usa

| Público | Como usa | Peso na v1 |
|---|---|---|
| **Claude, construindo projeto novo do ecossistema** | Instala o pacote do BaGet e monta tela | Principal — é para isso que existe |
| **Rafael** | Olha o site de documentação para decidir aparência e aprovar | Alto |
| **Desenvolvedor externo** | Acha o repositório público, copia ideia ou usa o pacote | Consequência de ser público, não objetivo |

⚠️ **A v1 nasce sem consumidor.** TradeBinder e Cockpit **não** esperam esta biblioteca (decisão do
Rafael, 16/09/2026). O primeiro consumidor real será um projeto novo, ou um dos dois **se** ele
decidir assim depois — e essa decisão é dele, não uma consequência automática.

## Requisitos funcionais

| # | Requisito | Onde |
|---|---|---|
| RF-01 | Camada de tokens em custom properties, cobrindo cor, tipografia, espaçamento, raio, sombra, z-index e breakpoint | `06` |
| RF-02 | Tema claro e escuro, trocáveis em runtime, persistidos por viewer, sem piscar na carga | `06` |
| RF-03 | ~35 componentes, em quatro ondas, cada um com todos os estados (normal, hover, foco, ativo, desabilitado, erro) | `11` |
| RF-04 | Site público de documentação, uma página por componente, com exemplo e código copiável | `07` |
| RF-05 | Pacote NuGet no BaGet, versão estável saindo de tag `v*` | `10` |
| RF-06 | Funcionar em Blazor Server **e** WebAssembly | `03` |
| RF-07 | Ícones Tabler embutidos, sem dependência de runtime | `03` |
| RF-08 | Crédito CC BY ao `hello.uiworld` no README, no site e no pacote | `01` |

## Requisitos não funcionais

| # | Requisito | Como se mede |
|---|---|---|
| RNF-01 | **WCAG 2.1 AA** | `axe` no E2E; violação séria reprova o build |
| RNF-02 | **Contraste ≥ 4.5:1** em texto normal, nos dois temas | Teste automatizado sobre os pares de token |
| RNF-03 | **Cobertura ≥ 80%** | Portão do `ci.yml` |
| RNF-04 | **Zero warning** em `Release` | `TreatWarningsAsErrors` na biblioteca |
| RNF-05 | **Navegação por teclado** em todo componente interativo, com foco visível | bUnit + E2E |
| RNF-06 | Fidelidade ao kit: o componente tem que ser reconhecível lado a lado com o Figma | Screenshot comparado ao PNG da referência, por olho do Rafael |

⚠️ **RNF-02 pode brigar com a fidelidade.** O texto primário do tema claro medido no kit é `#676C74`
sobre `#FFFFFF` — contraste ~5.1:1, passa. Mas o texto **desabilitado** (`#B4B2B7`) fica em ~2.2:1.
Isso é normal para desabilitado (a WCAG isenta), e está registrado para ninguém "corrigir" o token e
perder a fidelidade sem necessidade.

## Restrições

- Sem MudBlazor e sem biblioteca de componentes de terceiros (`ADR-003`).
- Sem backend, banco, container ou auth.
- Repositório **público** → CI não pode usar o `RVM.Actions` privado (`ADR-002`).
- Licença do código: **MIT**. Licença do design de origem: **CC BY 4.0**, com atribuição.
