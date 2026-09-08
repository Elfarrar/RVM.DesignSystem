// Script ANTI-FLASH. Vai no <head>, sincrono, ANTES de qualquer CSS ou do Blazor.
//
// Sem ele, quem usa tema escuro ve um lampejo branco em todo carregamento: o HTML pinta
// com o padrao claro e so depois o Blazor sobe e corrige. Como o Blazor WASM leva segundos
// para iniciar, o lampejo aqui nao seria um piscar — seria a pagina inteira errada por um
// tempo visivel.
//
// E de proposito que ele NAO e um modulo e NAO e async: precisa rodar antes do primeiro
// paint. Sao poucas linhas justamente porque bloqueia a renderizacao.
(function () {
    try {
        var pref = localStorage.getItem('rvm-ds-theme');
        if (pref === 'light' || pref === 'dark') {
            document.documentElement.setAttribute('data-rvm-theme', pref);
        }
        // 'system' ou ausente: nao escreve nada e o @media prefers-color-scheme do CSS decide.
    } catch (e) {
        // Storage bloqueado — segue o sistema. Nunca deixar este script derrubar a pagina.
    }
})();
