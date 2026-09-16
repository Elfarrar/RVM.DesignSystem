# Tokens

Os tokens **moram em `wwwroot/rvm-design-system.css`**, e nao aqui: uma RCL so serve arquivo que
esta em `wwwroot/`, e nao ha passo de build para copiar. Manter uma copia "fonte" nesta pasta so
criaria duas versoes do mesmo arquivo para sair de sincronia.

Consumidor carrega assim:

```html
<link rel="stylesheet" href="_content/RVM.DesignSystem/rvm-design-system.css" />
<script src="_content/RVM.DesignSystem/rvm-theme.js"></script>
```

Regras:

- **Todo valor foi medido do kit** (`referencia-neatlab/Theme/*.png`) e esta no `06-tokens-e-tematizacao.md`.
- Componente **nunca** escreve hex: usa `var(--rvm-...)`. Hex literal em CSS de componente quebra o tema escuro.
- Nome: `--rvm-<categoria>-<papel>-<variacao>`.
- Tema escuro redefine **superficie e estado**; a cor de marca NAO muda entre temas.
