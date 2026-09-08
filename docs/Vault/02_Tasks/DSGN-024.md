---
id: DSGN-024
titulo: A paleta do visitante fica guardada no navegador
repo: RVM.DesignSystem
tipo: bugfix
status: em-revisao
criada: 2026-09-08
---

# DSGN-024 — A paleta fica guardada

Achado pelo Rafael **usando** a `DSGN-023`, no mesmo dia: *"eu mudei e naveguei quando voltei
pra tela de paleta ela resetou as cores"*.

## O defeito

O tema aplicado vivia no `IRvmThemeService`, que e **scoped** — sobrevive a navegacao. Os
**campos do formulario** viviam na `PaletaPage`, que o Blazor **destroi e recria** a cada
navegacao.

O resultado nao era "perdeu tudo": era pior. O site continuava pintado com a paleta escolhida e
o formulario mostrava as cores iniciais. **Dois lugares dizendo coisas diferentes sobre a mesma
escolha** — e o formulario, que e onde a pessoa olha, dizendo a errada.

E um F5 perdia mesmo.

## O conserto

Guardar no `localStorage` e ler na entrada. O servico novo (`PaletaDoVisitante`) e do **site**,
nao da biblioteca.

⚠️ **Guarda as cores ESCOLHIDAS, nunca o tema derivado.** Sao oito campos contra 27 papeis x 2
modos, mas o motivo nao e tamanho: derivar na leitura garante que uma paleta guardada ontem
passe pelas regras de contraste de **hoje**. Um tema serializado seria uma copia congelada, e
uma correcao no `FromSeed` nunca a alcancaria.

### Quem le

A **casca** (`MainLayout`), nao so a pagina que monta a paleta. Ela vale para o site inteiro,
entao entrar direto em `/componentes/button` ja tem que chegar pintado. A pagina tambem chama, e
o servico garante que o `localStorage` e lido uma vez so.

### Escolher um tema embutido APAGA a guardada

Sem isso o proximo F5 traria a paleta de volta por cima da escolha feita no seletor da topbar.
Por isso ha `EsquecerAsync` (so apaga) separado de `RestaurarAsync` (apaga e volta ao RVM):
escolher "ERPAgro" precisa apagar, mas nao pode forcar o tema RVM.

### Serializacao por codigo gerado

Blazor WASM publica com `PublishTrimmed` em Release. O serializador por reflexao funciona no
`dotnet run` e falha no site publicado — **a pior ordem possivel para descobrir um defeito.**
Por isso ha um `JsonSerializerContext`.

### JSON corrompido nao derruba o site

Uma cor invalida guardada estouraria no `FromSeed`. A derivacao acontece **dentro do try**, e o
catch apaga o registro ruim: uma preferencia de documentacao ilegivel nao pode impedir a
documentacao de abrir.

## Escopo — o que isto NAO e

**Nao e "tema por usuario final em runtime"**, que segue fora da v1 (`09` § Fora da v1). Nao ha
servidor, conta nem tenant: e a preferencia de uma ferramenta de documentacao, no navegador de
quem visita. A biblioteca nao mudou nesta task.

## Verificado

| | |
|---|---|
| Build | 0 erro, 0 aviso |
| Testes | 282 |
| E2E | o teste da paleta agora cobre **navegar e voltar** e **F5**, nos dois sentidos |

O teste tambem confere que "Restaurar" **apaga** o que estava guardado — se so repintasse a tela
atual, o proximo F5 traria a paleta de volta e o botao pareceria nao ter funcionado.

## Duas armadilhas de teste, nenhuma do produto

1. **`Name = "Button"` casa com "IconButton".** Terceira vez que ambiguidade de locator me
   morde nesta suite. `Exact = true`.
2. **O boot do WASM em Debug passa dos 5s padrao do Playwright.** Este teste recarrega a pagina
   tres vezes; as esperas de carga completa tem 30s. Uma falha ali fala do servidor ter acabado
   de subir, nunca do produto.
