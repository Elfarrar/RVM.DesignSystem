---
id: DSGN-038
titulo: Borda do chip abaixo de 3:1 no tema escuro
repo: RVM.DesignSystem
tipo: bug
status: todo
criada: 2026-09-09
atualizada: 2026-09-09
---

# DSGN-038 — A fronteira que some

## Descricao

Medido pelo `design-reviewer` em 09/09/2026, tema escuro: a borda do chip fica em
`rgb(47,45,48)` sobre `rgb(8,7,9)` — **~1,5:1**, contra os **3:1** que a WCAG 1.4.11 pede para
fronteira de componente.

⚠️ **Nao foi re-medido por mim** — o numero e do agente e precisa ser confirmado antes do conserto.

O texto do chip passa folgado (7,9–8,5:1), entao o dano e cosmetico: em monitor com brilho baixo a
borda desaparece e o chip perde a forma. Mas e o tipo de coisa que o portao de contraste **nao**
pega: o teste cobre par `x`/`on-x` de paleta, e nao borda contra a superficie em que ela cai.

Isso e o achado maior por tras do achado: **ha uma classe de contraste que o CI nao verifica.**

## Plano

1. Confirmar a medicao nos dois temas.
2. Subir a luminosidade da borda do chip no escuro — pelo motor de tema (`FromSeed`), nunca por
   cinza escrito a mao no CSS, senao fura o portao em silencio.
3. Avaliar (sem obrigacao de fazer nesta task) estender o teste de contraste as bordas.

## Validacao

- [ ] Borda do chip >= 3:1 contra a superficie onde ele cai, nos dois temas.
- [ ] Contraste do texto do chip inalterado.

## Versao

Se mexer no motor de tema: **patch** na biblioteca.
