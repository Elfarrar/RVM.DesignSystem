---
id: DSGN-029
titulo: As cores do Bootstrap como tema de exemplo
repo: RVM.DesignSystem
tipo: feature
status: em-revisao
criada: 2026-09-08
---

# DSGN-029 — As cores do Bootstrap

Pedido do Rafael em 08/09/2026: *"eu gosto das cores do bootstrap pode me dar um exemplo usando
as cores do bootstrap?"*.

## Entregue

- `TemasDeExemplo.Bootstrap` — as seis cores do Bootstrap 5 como *seed*
- O tema no **seletor da topbar**, para ver o site inteiro nelas
- A pagina **`/fundamentos/bootstrap`**, que mede ao vivo o que entra e o que sai

⚠️ **O tema vive no SITE, nao na biblioteca.** O `RvmThemes.Samples` existe para identidades dos
apps RVM; paleta de terceiro nao e identidade nossa e nao deve virar API publica do pacote —
nome publico e compromisso, e a paleta deles muda entre versoes.

## ⚠️ O achado, e ele contraria o que eu mesmo escrevi primeiro

Eu abri a pagina afirmando *"quatro passam intactas; duas nao passam"*. **A medicao desmentiu:**

| Papel | Bootstrap | Contraste | Derivado | Contraste |
|---|---|---|---|---|
| primary | `#0D6EFD` | **3.90** | `#0062F1` | 4.53 |
| secondary | `#6C757D` | **4.06** | `#636C74` | 4.63 |
| success | `#198754` | **3.93** | `#017D4B` | 4.51 |
| info | `#0DCAF0` | **1.70** | `#007598` | 4.55 |
| warning | `#FFC107` | **1.41** | `#955B00` | 4.82 |
| danger | `#DC3545` | **3.92** | `#C11431` | 5.33 |

**Nenhuma das seis atinge 4.5:1** contra a superficie mais exigente. Quatro erram por uma casa
decimal e saem quase iguais; `info` e `warning` erram por muito e mudam de cara — o amarelo vira
ambar escuro.

Corrigi o texto para bater com a tabela. **Um numero medido na tela contradizendo a frase que eu
escrevi acima dele e o pior tipo de documentacao que existe** — a pessoa acredita na frase e
descobre o contrario em producao.

## Por que isso NAO e defeito do Bootstrap

As cores dele sao pensadas como **fundo**: `.btn-warning` pinta o botao de amarelo e escreve
texto escuro por cima, e o contraste fecha. Solucao correta.

Aqui a mesma cor tem **dois papeis**: fundo (com o `on-x` por cima) **e** cor de texto/icone
sobre a superficie — alerta, chip contornado, rotulo de estado. E o segundo uso que aperta.

A pagina diz isso explicitamente, e mostra a saida: quem quer o amarelo vivo usa `warning` como
**fundo**, com `on-warning` por cima — que e o que o Bootstrap faz.

## Verificado

| | |
|---|---|
| Build | 0 erro, 0 aviso |
| Testes | 309 |
| E2E | 9 testes; a varredura do axe descobre a rota nova pelo menu e ja a auditou nos dois modos |

A pagina nova entrou na varredura **sem eu mexer no teste**: a lista de rotas e descoberta do
proprio menu, exatamente para isso.
