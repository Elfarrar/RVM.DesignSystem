---
id: DSGN-032
titulo: "Março De 2026" — o ToTitleCase capitaliza a preposicao
repo: RVM.DesignSystem
tipo: bug
status: todo
criada: 2026-09-09
atualizada: 2026-09-09
---

# DSGN-032 — Em portugues, so a primeira letra sobe

## Descricao

Visto na verificacao do `DSGN-031` e apontado pelo Rafael: o titulo do calendario do
`RvmDatePicker` sai **"Março De 2026"**.

`RvmDatePicker.razor:216` faz `PtBr.TextInfo.ToTitleCase(...)` sobre `"março de 2026"`, e o
`ToTitleCase` capitaliza **cada palavra** — regra de ingles (*Title Case*). Em portugues so a
primeira letra sobe; a preposicao fica minuscula.

Nao e regressao do `DSGN-030`: esta assim desde a onda 4 (`DSGN-020`). Aparecia menos porque,
sem os dados de pt-BR, o titulo saia em ingles de qualquer forma — onde "March 2026" nem tem
preposicao para errar.

## Plano

Capitalizar **so a primeira letra** do texto ja formatado, em vez de chamar `ToTitleCase`.
Mantem o formato `"MMMM 'de' yyyy"` como esta — nao e hora de inventar parametro novo numa
`1.0.0` congelada.

## Validacao

- [ ] Teste bUnit com data fixa: o titulo de 15/03/2026 e exatamente `Março de 2026`.
- [ ] Suite unitaria e E2E verdes.
- [ ] Conferido no navegador.

## Versao

⚠️ **Mexe na biblioteca** — diferente do `DSGN-030` e do `DSGN-031`, que eram so o site. Muda
comportamento visual, nao a API: entra como **patch** na proxima publicacao do pacote.
