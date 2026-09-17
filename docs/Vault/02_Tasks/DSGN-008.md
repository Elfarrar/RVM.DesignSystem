---
id: DSGN-008
titulo: Site de documentacao com menu lateral (RvmAppShell)
repo: RVM.DesignSystem
tipo: feature
status: em andamento
criada: 2026-09-17
---

# DSGN-008 — Site com menu lateral

Pedido do Rafael em 17/09, depois do `1.0.0`: "falta menu lateral, inclusive eu quero o RVM.Design com
menu lateral". O site trocava de secao por tres links no topo e so chegava a um componente pela pagina
de indice.

## Feito

- [x] `MainLayout` do site sobre o proprio `RvmAppShell`: marca no alto do menu, Inicio, Fundamentos,
      Componentes e os 35 componentes nas quatro secoes das ondas; seletor de tema na barra do topo;
      credito CC BY no rodape da moldura
- [x] `Catalogo.cs`: fonte unica da lista de componentes (menu e pagina de indice); indice agrupado por
      onda e sem o texto velho de "onda 4 chegando"
- [x] `RvmAppShell` (biblioteca, sem quebrar contrato): so a lista do menu rola, a marca fica fixa; o
      menu rola ate o item da pagina atual ao abrir e a cada navegacao
- [x] E2E: menu leva ao componente e marca `aria-current`; no celular a gaveta mostra o item atual e
      fecha ao navegar; testes do exemplo da moldura restritos ao quadro (agora ha dois menus na pagina)

## Decisoes

| Decisao | Por que |
|---|---|
| **Itens de componente sem icone, com o nome sem `Rvm`** ("Button") | 35 icones distintos nao existem no conjunto; o circulo de subitem do kit resolve. Sem o prefixo o menu fica legivel e o nome nao colide com o link do indice |
| **Rolar ate o item atual so dentro do menu** (`scrollTop`, nao `scrollIntoView`) | `scrollIntoView` rolaria a pagina em volta: no exemplo de moldura dentro do site, a pagina pulava ate o exemplo |
| **Contorno do `h1` focado pelo roteador removido so no site** (`h1[tabindex="-1"]:focus`) | O `FocusOnNavigate` foca o titulo para o leitor de tela; o anel num titulo nao clicavel aparecia como caixa azul. O seletor com `[tabindex]` vence o `.rvm-root :focus-visible` global |

## Verifica

- bUnit 502 verdes; E2E local 175/175 (axe nos dois temas em todas as paginas, com o menu)
- Screenshots: `site-menu-claro/escuro/recolhido/celular/celular-aberto.png` (scratchpad de 17/09)
- Producao so com novo sinal verde do Rafael
