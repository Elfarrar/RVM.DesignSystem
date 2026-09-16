# 07 — Site de documentação

O site é **feito com a própria biblioteca**. Não é vitrine ilustrada: é o primeiro consumidor, e a
prova de que os componentes funcionam fora do teste unitário. Se algo não dá para montar no site, o
componente está errado.

## Stack

**Blazor WebAssembly**, publicado como estático. Sem servidor, sem container, sem `/health` — é
HTML, CSS, WASM e os assets.

⚠️ **Três detalhes quebram Blazor WASM em host estático**, e os três são resolvidos no publish, com
guarda no CI para o caso de sumirem num refactor:

1. **`.nojekyll`** — sem ele o GitHub Pages ignora a pasta `_framework` (começa com `_`) e o app não
   carrega, com 404 em arquivo que existe.
2. **`404.html` cópia do `index.html`** — o Pages não tem `try_files`; sem isso, recarregar
   `/componentes/button` dá 404.
3. **`<base href="/">`** — caminho errado quebra todo asset relativo.

## Estrutura

| Seção | Conteúdo |
|---|---|
| Início | O que é, como instalar, crédito CC BY ao `hello.uiworld` |
| Fundamentos | Cor, tipografia, espaçamento, raio, sombra, ícones — os tokens, renderizados |
| Componentes | Uma página por componente: exemplo vivo, variações, estados, tabela de parâmetros, código copiável |
| Padrões | Formulário, listagem, estados de tela (carregando, vazio, erro), diálogo de confirmação |
| Acessibilidade | O que garantimos, como testamos, o que fica de fora |

- **Seletor de tema no topo**, visível em toda página — é o jeito de conferir claro/escuro sem F5.
- Cada exemplo mostra o **código real** que o produz; código na doc que não é o código que roda vira
  mentira em duas semanas.
- Texto do site em **PT-BR**.

## Fidelidade — como se confere

A página de cada componente tem, ao lado do exemplo, o **recorte do PNG do kit** correspondente
(`referencia-neatlab/`). Aprovação é o Rafael olhando os dois lado a lado.

É o critério do RNF-06, e o motivo de os 99 MB de referência morarem dentro do repositório: sem eles,
"parece o NEATLAB?" vira discussão de memória.

## Fora do escopo da v1

Busca full-text, playground editável, versionamento da doc (v1 e v2 lado a lado), tradução para
inglês.
