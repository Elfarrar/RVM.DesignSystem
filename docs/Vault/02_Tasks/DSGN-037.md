---
id: DSGN-037
titulo: O padrao de Dashboard so mostra o caminho feliz
repo: RVM.DesignSystem
tipo: feature
status: todo
criada: 2026-09-09
atualizada: 2026-09-09
---

# DSGN-037 — Sem vazio, sem carregando, sem erro

## Descricao

O padrao de Dashboard demonstra a tela cheia de dados e mais nada. Sem estado vazio, sem
carregando, sem erro.

Isso contraria uma anti-referencia que o proprio `PRODUCT.md` acabou de registrar: *"documentacao
que so mostra o resultado, sem codigo, sem o porque e sem o estado de erro"*. E e a pergunta que o
consumidor faz primeiro quando copia o padrao para um app real: **o que aparece no dia 1, quando
nao ha pedido nenhum?** Hoje a resposta seria quatro caixas com zero solto, sem contexto.

O padrao de Listagem ja combina `aria-busy` + `RvmSkeleton` + `RvmEmptyState` e esta demonstrado em
`/padroes`. Falta trazer isso para o Dashboard.

## Plano

1. Alternar o padrao entre tres estados na propria pagina (carregando / vazio / com dados) —
   controle visivel, nao so texto explicando.
2. O vazio precisa dizer o que fazer, nao so "sem dados": e a diferenca entre `RvmEmptyState`
   generico e um que serve.
3. Nao inventar componente novo. Se faltar algo, isso e achado da task, nao licenca para crescer.

## Validacao

- [ ] Os tres estados visiveis na pagina, alternaveis.
- [ ] axe passa nos tres, nos dois temas.
