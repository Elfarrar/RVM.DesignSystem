# 04 — Modelo de dados

## Não há banco de dados

Este projeto **não tem persistência**: sem PostgreSQL, sem migration, sem `CompanyId`, sem
soft-delete, sem Redis. A seção existe porque o checklist do ecossistema a exige, e a resposta
honesta é "não se aplica" — não "esqueci".

O que existe no lugar é a **estrutura dos tokens**, que é o dado do projeto.

## Estrutura dos tokens

```
token
  categoria   color | typography | spacing | radius | shadow | zindex | breakpoint
  papel       primary | secondary | info | success | warning | error | text | action | surface
  variação    main | alt-light | alt-dark | contrast | hover | resting | disabled | focus
  tema        light | dark          (só para os que mudam entre temas)
  valor       string CSS
```

Materializa como custom property: `--rvm-color-primary-main: #264CC8;`

**Regra:** todo valor visual do sistema é um token. Componente não conhece cor — conhece papel.
É o que permite trocar tema sem tocar em componente, e o que impede que o sistema vire uma coleção
de CSS soltos.

## Estado em runtime

| Estado | Onde vive | Persiste? |
|---|---|---|
| Tema escolhido (claro/escuro/sistema) | `localStorage` do navegador | Por viewer, no navegador dele |
| Densidade | ⏳ fora da v1 | — |

Nada disso é dado de aplicação: é preferência de quem está olhando a tela.

## Pendências que mudam a estrutura

Nenhuma bloqueia o início. A única em aberto é se a densidade entra como eixo de token (afeta o
nome de toda a escala de espaçamento) — decisão para depois da onda 2.
