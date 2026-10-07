---
id: DSGN-016
titulo: Tokens e API legiveis por IA (tokens.json DTCG + llms.txt)
repo: RVM.DesignSystem
tipo: feature
status: concluida
criada: 2026-10-04
atualizada: 2026-10-04
---

# DSGN-016 — Tokens e API legiveis por IA

> **Publicada na `1.6.0` em 04/10/2026** (tag `v1.6.0`, versao conferida no feed do BaGet) e em
> producao em `https://design.rvmit.com.br/llms.txt`, `/llms-full.txt` e `/tokens.json`. E2E contra
> producao: **237 testes verdes**, a suite inteira. Alphas de `dev` passam a `1.7.0-alpha.N`.
>
> Review independente: um P2 corrigido (ordem de `GetProperties`/`GetMethods` pode diferir entre
> Windows e o Linux do CI, e o teste de sincronia ficaria vermelho para sempre — tudo que vem de
> reflexao sai ordenado por nome) e dois P3 (versao do DTCG no `$description`, script anti-piscar
> do tema nas regras de instalacao).

## Descricao

Hoje os 222 tokens so existem como custom properties em `wwwroot/rvm-design-system.css`, e a API
dos componentes so esta nos comentarios XML e nas tabelas escritas a mao do site. Nenhum agente de
IA consegue ler isso de forma estruturada, e por isso ele erra o nome do token, escreve hex ou
inventa parametro.

Decisao do Rafael em 04/10/2026 (opcao 2 de tres: "tokens legiveis por IA", e nao variante visual
de IA nem componente de chat):

- **`tokens.json` no formato DTCG** (W3C Design Tokens), com o valor claro e escuro de cada token e
  o nome da custom property. **O CSS continua sendo a fonte da verdade**: o JSON e gerado a partir
  dele.
- **`llms.txt` + `llms-full.txt`**: o indice e a API completa dos componentes (parametros, tipos,
  padroes e descricao, por reflexao + XML doc), mais as regras do DS que uma IA erraria sem saber
  (sem hex, prefixo `rvm-`, `AdditionalAttributes`, Server e WASM, credito NEATLAB).
- **Publicados nos dois lugares**: no site (`design.rvmit.com.br/tokens.json`, `/llms.txt`,
  `/llms-full.txt`) e **dentro do pacote NuGet**, para o Claude achar offline num projeto consumidor.
- So acrescenta arquivo, sem quebrar contrato: **`1.6.0`** (minor).

## Plano

1. Gerador CSS → `tokens.json` (DTCG, `$value` claro + extensao com o escuro e a `var`) → verifica:
   teste compara o JSON commitado com o CSS e reprova divergencia
2. Gerador da API (reflexao sobre `[Parameter]` + XML doc) → `llms-full.txt`, e o `llms.txt`
   indice → verifica: teste garante que todo componente publico `Rvm*` aparece
3. Empacotar no NuGet e servir no site → verifica: `dotnet pack` contem os tres arquivos; site
   local responde os tres com o conteudo certo
4. Testes ≥ 80%, E2E (os tres arquivos no ar por conteudo), review `dotnet-blazor-reviewer`
5. PR para `dev`, deploy dev, verificacao por conteudo

## Validacao

- [x] `tokens.json` valido em DTCG, com claro e escuro, sincronizado com o CSS por teste
- [x] `llms.txt` e `llms-full.txt` cobrem todos os componentes publicos e as regras do DS
- [x] Os tres arquivos no pacote NuGet e no site de dev, conferidos por conteudo
- [x] Cobertura ≥ 80%, zero warning em Release, E2E verde
- [x] Review independente sem P1 aberto
- [x] Rafael aprova; `1.6.0` so com sinal verde dele
