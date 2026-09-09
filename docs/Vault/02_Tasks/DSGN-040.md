---
id: DSGN-040
titulo: Tela so de leitura reprova no axe — a resposta e no RvmAppShell
repo: RVM.DesignSystem
tipo: bug
status: concluido
criada: 2026-09-09
atualizada: 2026-09-09
---

# DSGN-040 — Rolar com o teclado

## Descricao

Pendencia registrada no `CLAUDE.md` desde 08/09/2026 e reaberta pela medicao de hoje: o `<main>`
do Dashboard tem **4 elementos focaveis**, e nao por design — as caixas viraram link para
satisfazer o axe.

A area de conteudo do `RvmAppShell` rola. Uma regiao que rola e nao tem nada focavel nao pode ser
rolada pelo teclado, e o axe reprova com `scrollable-region-focusable`. Hoje cada tela contorna
isso sozinha, inventando um foco que a tela nao precisava ter.

⚠️ **Todo app consumidor com tela so de leitura herda isso** — e vai contornar do mesmo jeito
torto, ou nem perceber ate o axe reprovar.

## Plano

1. `RvmAppShell`: a regiao que rola recebe `tabindex="0"` e nome acessivel, para que o teclado
   alcance a rolagem sem que a tela precise inventar link.
2. ⚠️ Verificar o efeito colateral: um `tabindex="0"` no contorno acrescenta uma parada na
   navegacao de toda tela. Medir se a parada aparece antes do conteudo (seria ruim) e se o foco
   fica visivel.
3. Depois disso, avaliar se as caixas do Dashboard ainda precisam ser link — o `DSGN-035` decide
   o destino delas de qualquer forma.

## Validacao

- [x] Uma tela sem nenhum elemento focavel passa no axe dentro do `RvmAppShell`.
- [x] A ordem de tabulacao continua comecando pelo link de pulo.
- [x] E2E verde nas 45 paginas.

## Versao

Mexe na biblioteca: **patch**.

## Resultado (09/09/2026) — EM PRODUCAO

`.rvm-app-shell__rolagem` com `tabindex="0"` em producao de dev, e o E2E (10/10) confirma
que o link de pulo continua sendo o primeiro focavel.
