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

    // Quem rola e a navegacao do RvmSidebar, dentro da coluna (DSGN-017).
    caixa = atual.closest('.rvm-navegacao') ?? caixa;

    const item = atual.getBoundingClientRect();
    const area = caixa.getBoundingClientRect();
    if (item.top < area.top || item.bottom > area.bottom) {
        caixa.scrollTop += item.top - area.top - (area.height - item.height) / 2;
    }
}

// Prende o Tab dentro da caixa, sem mais nada (o TrapFocus do RvmCalendar, DSGN-017): do ultimo focavel o Tab volta
// ao primeiro, e o Shift+Tab do primeiro vai ao ultimo. So conta o que entra na tabulacao (tabindex >= 0).
export function prenderTab(caixa) {
    if (!caixa) {
        return;
    }

    caixa.addEventListener('keydown', (evento) => {
        if (evento.key !== 'Tab') {
            return;
        }

        const lista = [...caixa.querySelectorAll('button:not([disabled]), [tabindex]')]
            .filter(el => el.tabIndex >= 0 && el.getClientRects().length > 0);
        if (lista.length === 0) {
            return;
        }

        const primeiro = lista[0];
        const ultimo = lista[lista.length - 1];
        if (evento.shiftKey && document.activeElement === primeiro) {
            evento.preventDefault();
            ultimo.focus();
        } else if (!evento.shiftKey && document.activeElement === ultimo) {
            evento.preventDefault();
            primeiro.focus();
        }
    });
}
