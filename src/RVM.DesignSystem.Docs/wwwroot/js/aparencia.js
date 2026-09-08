// PREVIEW DE APARENCIA — temporario, apagado junto com aparencia.css quando a direcao for
// escolhida (DSGN-025).
//
// Escreve um atributo no elemento raiz, como o motor de tema ja faz com o modo de cor. Fica
// FORA do Blazor de proposito: sobrevive a navegacao entre paginas sem nenhum estado no C#,
// que e tudo o que um preview precisa.
(function () {
    var CHAVE = 'rvm-docs-aparencia';

    function aplicar(valor) {
        if (valor && valor !== 'atual') {
            document.documentElement.setAttribute('data-rvm-aparencia', valor);
        } else {
            document.documentElement.removeAttribute('data-rvm-aparencia');
        }
    }

    window.rvmAparencia = {
        definir: function (valor) {
            aplicar(valor);
            try { localStorage.setItem(CHAVE, valor); } catch (e) { /* modo privado */ }
        },
        atual: function () {
            try { return localStorage.getItem(CHAVE) || 'atual'; } catch (e) { return 'atual'; }
        }
    };

    // Antes do primeiro paint, como o script anti-flash do tema: aplicar depois faria a pagina
    // piscar na aparencia errada a cada F5.
    aplicar(window.rvmAparencia.atual());
})();
