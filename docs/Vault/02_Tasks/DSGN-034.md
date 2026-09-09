---
id: DSGN-034
titulo: Secao ativa nao e pagina atual — `aria-current` em dois links
repo: RVM.DesignSystem
tipo: bug
status: concluido
criada: 2026-09-09
atualizada: 2026-09-09
---

# DSGN-034 — Duas paginas atuais ao mesmo tempo

## Descricao

Achado no `critique` de 09/09/2026 e **confirmado no DOM da producao**: em `/padroes/dashboard`,
dois links da navegacao carregam `aria-current="page"` ao mesmo tempo — "Padroes" (`href=padroes`)
e "Dashboard" (`href=padroes/dashboard`) — e os dois recebem o preenchimento solido de item ativo.

Para quem enxerga, e ambiguidade visual. Para quem usa leitor de tela, e **contradicao**:
`aria-current="page"` responde "onde eu estou", e a resposta nao pode ser dois lugares.

⚠️ **Nao e defeito do casamento por prefixo.** O `RvmNavItem.razor:171` casa por prefixo de
proposito, e existe `Exact` para quando isso nao serve — um menu com filhos PRECISA destacar o
ancestral, senao a secao inteira parece apagada quando um filho esta aberto. O defeito e ter
juntado duas coisas diferentes numa so:

| | hoje | deveria |
|---|---|---|
| Rota exata | classe ativa + `aria-current="page"` | igual |
| Rota **filha** (ancestral) | classe ativa + `aria-current="page"` | classe ativa, **sem** `aria-current` |

Consertar so o site (`Exact="true"` no item pai) esconderia o sintoma e deixaria o proximo
consumidor com menu aninhado repetir tudo. Decisao do Rafael em 09/09: **conserta na biblioteca**.

## Plano

1. `RvmNavItem`: separar os dois estados. `Ativo` continua governando a aparencia; um segundo
   estado (rota exata) governa o `aria-current`. Ancestral pinta, mas nao anuncia.
2. Teste bUnit: com a rota em `padroes/dashboard`, o item `padroes` tem a classe ativa e **nao**
   tem `aria-current`; o item `padroes/dashboard` tem os dois.
3. Nada muda no site — e essa e a prova de que o conserto esta no lugar certo.

## Validacao

- [x] Teste bUnit novo cobrindo ancestral e rota exata.
- [x] Na producao, depois do deploy, `document.querySelectorAll('[aria-current]')` devolve **um**.
- [x] Suite unitaria e E2E verdes.

## Versao

Mexe na biblioteca. Muda comportamento de acessibilidade, nao API: **patch**.

## Resultado (09/09/2026) — EM PRODUCAO

`document.querySelectorAll("[aria-current]")` devolve **um** elemento — "Dashboard".
Eram dois. Nada mudou no site: o conserto ficou inteiro na biblioteca.
