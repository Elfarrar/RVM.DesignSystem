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

// Traz o item da pagina atual (aria-current="page") para a vista DENTRO da caixa que rola (o menu
// lateral). So mexe no scrollTop da caixa: scrollIntoView rolaria tambem a pagina em volta — num
// exemplo de moldura dentro do site, a pagina pulava ate o exemplo ao abrir.
export function rolarAtualParaVer(caixa) {
    const atual = caixa?.querySelector('[aria-current="page"]');
    if (!atual) {
        return;
    }

    const item = atual.getBoundingClientRect();
    const area = caixa.getBoundingClientRect();
    if (item.top < area.top || item.bottom > area.bottom) {
        caixa.scrollTop += item.top - area.top - (area.height - item.height) / 2;
    }
}
