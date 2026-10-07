---
id: DSGN-018
titulo: Pele do RVM.UI.Tutor no DesignSystem — tema, página de documentação e tour de demonstração
repo: RVM.DesignSystem
tipo: feature
status: a-fazer
criada: 2026-10-07
origem: decisão do Rafael em 07/10/2026; depende do TUTOR-008 (Tutor aberto com MIT)
---

# DSGN-018 — tour guiado no visual NEATLAB

O RVM.UI.Tutor é neutro: o visual vem de variáveis CSS `--tutor-*` (`07-visual-e-tema.md` do Tutor). A RVM.UI
já publica a pele Protask; o DesignSystem passa a publicar a sua.

## Plano
1. **Pele**: CSS (ou `<RvmTutorTheme/>`) que liga os tokens do DesignSystem às `--tutor-*`, claro e escuro.
   **Sem dependência de pacote**: a biblioteca não referencia o Tutor; quem quer tour instala os dois.
2. **Página de documentação** no site (`design.rvmit.com.br`): como instalar o Tutor, aplicar a pele, roteiro em C#.
3. **Tour de demonstração** no próprio site (o site sim referencia o Tutor).
4. Entrada no `llms-full.txt` e no `tokens.json` (as `--tutor-*` mapeadas), para agentes de IA.
5. Versão minor (sem quebra de contrato do `1.x`; se cair depois do `2.0.0` do [[DSGN-017]], minor do `2.x`).

## Critério de validação
- E2E: a página de demonstração abre o tour, avança os passos, nos dois temas.
- Screenshot da página e do tour (claro e escuro) na entrega.
- `llms-full.txt` contém a seção do Tutor.

Depende do TUTOR-008 (licença MIT + espelho público). Depois do corte WSL (MAES-011, 07/10/2026).
