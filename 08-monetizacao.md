# 08 — Monetização

## Não monetiza

O RVM.DesignSystem **não é vendido, não tem plano, não tem cobrança e não tem cliente**. Seção
escrita por exigência do checklist do ecossistema, e para que a ausência seja decisão registrada em
vez de lacuna.

## O que ele devolve, então

| Retorno | Como se mede |
|---|---|
| **Velocidade de app novo** | Tempo até a primeira tela coerente num projeto RVM novo. Hoje: copiar o tema do último app e ajustar na mão |
| **Custo evitado de divergência** | Quatro paletas e quatro `MainLayout` mantidos em paralelo hoje. O quinto app não nasce assim |
| **Acessibilidade que existe de fato** | Contraste e teclado verificados em um lugar, herdados por todos — em vez de nunca verificados em lugar nenhum |
| **Peça de portfólio** | `design.rvmit.com.br` é público. Design system próprio, documentado e acessível é evidência forte de front-end sênior — ver `01` § Persona |

## Custo de operação

Praticamente zero, e isso é consequência de decisão de arquitetura, não sorte:

| Item | Custo |
|---|---|
| Site público | GitHub Pages — sem VPS, sem container, sem banco |
| Ambiente dev | Arquivos estáticos servidos pelo Nginx já existente na Rivendell |
| Distribuição | BaGet interno que já roda no BagEnd para o `RVM.Common` |
| Licença de terceiros | Nenhuma. Sem dependência de UI paga; conjunto de ícones sob licença MIT |

## Se um dia virar produto

Não está no roadmap e não é objetivo. Registrado apenas para que a conversa, se acontecer, comece de
um ponto informado: um design system só é vendável junto com serviço (consultoria, implantação,
suporte), porque o código de UI é facilmente copiável e o valor está em quem mantém. A publicação no
nuget.org (fora da v1, ver `01` § Escopo) seria o primeiro passo natural — e é gratuito.

## Licença

⏳ **PENDENTE — decisão do Rafael:** o repositório é público ou privado, e sob qual licença?
*Pergunta exata: o `design.rvmit.com.br` é público — o código-fonte da biblioteca também deve ser?*

Suposição vigente: **repositório público, licença MIT.** Um design system de portfólio que ninguém
pode ler é meia peça de portfólio; e o site publicado já expõe o CSS e o comportamento de qualquer
maneira. Se a resposta for "privado", o site continua público — o que muda é só a visibilidade do
repositório e, com ela, parte do valor de vitrine.
