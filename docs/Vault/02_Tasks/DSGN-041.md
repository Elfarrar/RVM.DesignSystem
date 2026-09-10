---
id: DSGN-041
titulo: O changelog do site parou na 1.1.0 — e anuncia uma versao que nao existe no feed
repo: RVM.DesignSystem
tipo: bug
status: concluido
criada: 2026-09-09
atualizada: 2026-09-09
---

# DSGN-041 — O changelog mente duas vezes

## Descricao

Notado pelo Rafael em 09/09/2026: o `/changelog` mostra ate a **1.1.0**, e hoje o feed tem
**1.1.1** e **1.1.2**.

Sao dois defeitos, e o segundo e pior que o primeiro:

1. **Esta atrasado.** O `ChangelogPage.razor` e escrito a mao, uma `<section>` por versao, e
   ninguem o atualiza ao publicar. Nenhuma task tinha isso no escopo — o `DSGN-033` tratou do
   *push*, nao da comunicacao.
2. ⚠️ **A secao "1.1.0" anuncia uma versao que NUNCA chegou ao feed.** A tag existe
   (`ad48171`), o pacote nao — o push falhou por rede e a decisao registrada foi **nao**
   republicar aquele numero (`DSGN-033`). Quem ler o site vai tentar instalar um pacote que nao
   existe e concluir que o feed esta quebrado.

O que a `1.1.0` trazia (cores de estado escolhiveis e a pagina de paleta) **esta na `1.1.1`** —
o codigo nunca se perdeu, so o numero.

## Plano

1. Acrescentar as secoes **`1.1.2`** e **`1.1.1`**, no idioma do consumidor: o que muda para
   quem usa a biblioteca, nao o numero do card.
2. Na secao `1.1.0`, dizer que ela **nao esta no feed** e apontar a `1.1.1` como substituta.
3. ⚠️ Nao automatizar agora. Gerar o changelog das tags seria trocar texto escrito por lista de
   commit, e o valor desta pagina e justamente explicar o **porque** de cada mudanca.

## O que fica registrado como risco

Enquanto o changelog for manual, ele vai atrasar de novo — a menos que "atualizar o changelog"
entre no fluxo de publicacao, junto da tag. Vale escrever isso no `CLAUDE.md § Workflow`.

## Validacao

- [x] `/changelog` mostra `1.1.2`, `1.1.1` e o aviso na `1.1.0`.
- [x] axe passa na pagina, nos dois modos.

## Resultado (10/09/2026, verificado em producao)

`/changelog` abre com `1.1.2`, `1.1.1` e a `1.1.0` carregando o aviso "Esta versao nao esta no
feed — use a 1.1.1". E2E 11/11 depois do deploy.

⚠️ **Este card foi promovido antes de ser fechado** — ficou `status: todo` e fora do Kanban ate o
handoff de 10/09. O codigo estava certo e verificado; faltou a rastreabilidade, e a causa foi
nascer no meio de uma resposta a uma pergunta, fora do fluxo normal de task.
