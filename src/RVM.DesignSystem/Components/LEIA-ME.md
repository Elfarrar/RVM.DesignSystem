# Components

Um diretorio por componente: `<Nome>/RvmNome.razor` + `.razor.cs` + `.razor.css`
(`03-arquitetura.md`). Esta pasta nasce vazia de proposito — os nove primeiros componentes
chegam na `DSGN-002`, depois da camada de tokens.

Regras que valem para todo componente daqui:

- prefixo `Rvm`, API em ingles, texto ao usuario final em PT-BR;
- CSS isolado consumindo **so** custom properties — hex literal aqui e bug, quebra o tema escuro;
- `AdditionalAttributes` repassado ao elemento raiz; componente de formulario renderiza
  `name`, `id` e `aria-*`;
- enum, nunca string magica.
