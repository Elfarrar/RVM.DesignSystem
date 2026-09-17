---
id: DSGN-009
titulo: Paginas de exemplo — dashboard, CMS, CRM, ERP e planner
repo: RVM.DesignSystem
tipo: feature
status: concluido
criada: 2026-09-17
---

# DSGN-009 — Paginas de exemplo

Pedido do Rafael em 17/09: "faca algumas paginas de exemplo, um dashboard, um CMS, um CRM, um ERP, um
Planner de tasks". Telas completas no site, montadas SO com componentes da biblioteca, com dados
ficticios e interacao na memoria — mostram como os 35 componentes se combinam numa aplicacao real.

## Escopo

- Secao **Exemplos** no menu lateral do site, rota `/exemplos/*`, area de conteudo sem o limite de
  largura das paginas de componente
- **Dashboard**: indicadores, metas com progresso, atividade recente, ultimos pedidos, equipe, alerta
- **CMS**: publicacoes filtradas por abas, grade com filtro e acoes, dialogo de nova publicacao
- **CRM**: funil em colunas com oportunidades que mudam de etapa, gaveta de detalhe com historico
- **ERP**: pedidos (selecao e faturamento, novo pedido em etapas), estoque com nivel, financeiro
- **Planner**: quadro A fazer / Em andamento / Concluido, calendario do dia, nova tarefa com data e
  relogio, concluir e mover
- E2E: cada tela abre, uma interacao por tela, axe nos dois temas

## Fora (e por que)

- **Graficos**: a biblioteca nao tem grafico (fora do escopo da v1). O dashboard usa indicadores,
  barras de progresso, tabela e linha do tempo
- Persistencia: tudo na memoria da pagina; recarregar volta aos dados iniciais

## Decisoes

| Decisao | Por que |
|---|---|
| **Secao Exemplos no menu + `Catalogo.Exemplos`**; conteudo sem o limite de 62rem nas rotas `/exemplos/*` (90rem) | Dashboard e quadros precisam de largura; as paginas de componente continuam estreitas para leitura |
| **Dinheiro, numero e data formatados em PT-BR fixo** (`Docs/Shared/Formato.cs`) | No WASM a cultura segue o navegador e so os dados dele sao carregados: `C0` sairia `$184,500` num navegador em ingles |
| **Acoes de linha como botoes, nao `RvmMenu`, dentro das grades** | A raiz da tabela corta o que transborda (`overflow: hidden`): um menu aberto dentro da celula ficaria cortado. Nos quadros (CRM, planner) o menu fica, porque a coluna nao corta |
| **Botao que depende de texto digitado nao fica desabilitado; valida no clique** (CRM "Registrar") | O `RvmTextField` grava no `change` (ao sair do campo): desabilitado "ate ter texto", o botao seguia desabilitado no proprio clique que tira o foco do campo. Pego no E2E |
| **`.site-conteudo .ex-coluna { margin: 0 }`** | A regra `.site-conteudo section` da documentacao dava 40 px de margem as colunas `<section>` dos quadros e desalinhava tudo |

## Verifica

- bUnit 502; E2E local 195/195 (20 novos: abrir pelo menu, axe nos dois temas e uma interacao por tela —
  periodo do dashboard, publicar no CMS, mover e registrar no CRM, faturar e emitir pedido em 3 etapas
  no ERP, criar tarefa com relogio e concluir no planner)
- Fotos: `ex-<tela>-claro/escuro.png` (scratchpad de 17/09)
- Dev: PR #38. Producao: sinal verde do Rafael em 17/09 ("pode promover e deployar")
