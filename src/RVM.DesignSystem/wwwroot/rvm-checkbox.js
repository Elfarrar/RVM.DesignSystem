// Modulo carregado sob demanda pelo RvmCheckbox (import dinamico) — o consumidor nao precisa de tag
// <script>. Existe por UMA razao: o estado "indeterminado" de um checkbox nativo e PROPRIEDADE do DOM,
// nao atributo, e so da para definir por JS. O VISUAL do indeterminado sai por CSS no primeiro render;
// isto aqui so completa a semantica para o leitor de tela ("misto").
export function definirIndeterminado(elemento, valor) {
    if (elemento) {
        elemento.indeterminate = !!valor;
    }
}
