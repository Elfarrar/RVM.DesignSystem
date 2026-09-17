// Impede o efeito padrao do navegador (rolar a pagina) para as teclas que um componente usa para
// navegar por dentro — setas numa lista de abas, espaco num select. O Blazor nao consegue barrar o
// padrao por tecla, so para todas: e barrar Tab tiraria o foco da ordem de tabulacao.
// Nao participa do estado inicial: sem este modulo o componente funciona, so a pagina rola junto.
export function prenderTeclas(elemento, teclas) {
    if (!elemento || elemento.__rvmTeclas) {
        return;
    }

    const alvo = new Set(teclas);
    elemento.__rvmTeclas = true;
    elemento.addEventListener('keydown', (evento) => {
        if (alvo.has(evento.key)) {
            evento.preventDefault();
        }
    });
}

// Traz a opcao ativa de uma lista rolavel para a vista, sem rolar a pagina inteira a toa.
export function rolarParaVer(id) {
    document.getElementById(id)?.scrollIntoView({ block: 'nearest' });
}
