// Foco de dialogo e gaveta temporaria: guarda quem tinha o foco, leva o foco para dentro, prende o Tab
// ali e trava a rolagem da pagina por baixo. Ao fechar, desfaz tudo e devolve o foco.
// E melhoria, nao pre-requisito: sem este modulo o dialogo abre, fecha e responde ao Esc; so o foco
// nao fica preso.

const FOCAVEIS = [
    'a[href]', 'area[href]', 'button:not([disabled])', 'input:not([disabled]):not([type="hidden"])',
    'select:not([disabled])', 'textarea:not([disabled])', 'iframe', 'summary',
    '[contenteditable="true"]', '[tabindex]:not([tabindex="-1"])'
].join(',');

let abertos = 0;
let overflowOriginal = '';

// As caixas abertas, a de cima por ultimo. Quando o foco cai no body (clique no fundo nao focavel, botao focado que
// fica desabilitado durante a acao), o Tab e o Esc ouvidos na caixa param de valer: o foco volta para a caixa de cima,
// e um Tab dado fora dela e puxado de volta (achado do review da onda 3, DSGN-017).
const pilha = [];

function noTopo(caixa) {
    return pilha.length > 0 && pilha[pilha.length - 1] === caixa;
}

document.addEventListener('keydown', (evento) => {
    const topo = pilha[pilha.length - 1];
    if (evento.key !== 'Tab' || !topo || topo.contains(document.activeElement)) {
        return;
    }

    evento.preventDefault();
    (focaveis(topo)[0] ?? topo).focus();
}, true);

function focaveis(caixa) {
    // So o que entra na tabulacao: um botao com tabindex=-1 (dia nao focado da grade do calendario,
    // aba inativa) casa com o seletor, mas o Tab nao para nele. Sem este filtro o "ultimo focavel" era
    // o dia 31 e o Tab escapava do dialogo a partir do dia em foco (pego no E2E do seletor de data).
    return [...caixa.querySelectorAll(FOCAVEIS)].filter(el => el.tabIndex >= 0 && el.getClientRects().length > 0);
}

export function abrir(caixa) {
    if (!caixa) {
        return null;
    }

    const anterior = document.activeElement;

    const aoTeclar = (evento) => {
        if (evento.key !== 'Tab') {
            return;
        }

        const lista = focaveis(caixa);
        if (lista.length === 0) {
            evento.preventDefault();
            caixa.focus();
            return;
        }

        const primeiro = lista[0];
        const ultimo = lista[lista.length - 1];
        const atual = document.activeElement;
        if (evento.shiftKey && (atual === primeiro || atual === caixa)) {
            evento.preventDefault();
            ultimo.focus();
        } else if (!evento.shiftKey && atual === ultimo) {
            evento.preventDefault();
            primeiro.focus();
        }
    };

    caixa.addEventListener('keydown', aoTeclar);

    const aoPerderFoco = () => {
        setTimeout(() => {
            const atual = document.activeElement;
            if (noTopo(caixa) && document.hasFocus() && (!atual || atual === document.body)) {
                caixa.focus();
            }
        }, 0);
    };
    caixa.addEventListener('focusout', aoPerderFoco);
    pilha.push(caixa);

    if (abertos === 0) {
        overflowOriginal = document.body.style.overflow;
        document.body.style.overflow = 'hidden';
    }
    abertos++;

    // O foco vai para a propria caixa (tabindex=-1): o leitor de tela le o titulo, e o primeiro Tab
    // chega no primeiro controle. Quem quiser outro alvo marca o elemento com data-rvm-foco-inicial.
    const alvo = caixa.querySelector('[data-rvm-foco-inicial]') ?? caixa;
    alvo.focus();

    let fechado = false;
    return {
        fechar() {
            if (fechado) {
                return;
            }

            fechado = true;
            caixa.removeEventListener('keydown', aoTeclar);
            caixa.removeEventListener('focusout', aoPerderFoco);
            const posicao = pilha.lastIndexOf(caixa);
            if (posicao >= 0) {
                pilha.splice(posicao, 1);
            }
            abertos = Math.max(0, abertos - 1);
            if (abertos === 0) {
                document.body.style.overflow = overflowOriginal;
            }

            if (anterior && anterior.isConnected && typeof anterior.focus === 'function') {
                anterior.focus();
            }
        }
    };
}
