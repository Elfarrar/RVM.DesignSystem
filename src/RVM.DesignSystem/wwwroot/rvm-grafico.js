// Mede a largura real da area do grafico e avisa o .NET, para o SVG ser desenhado na largura certa (texto
// sem escalar). E melhoria, nao pre-requisito: sem este modulo o grafico sai na largura padrao, escalado.
export function observar(elemento, dotnet) {
    if (!elemento || typeof ResizeObserver === 'undefined') {
        return null;
    }

    let ultima = 0;
    let quadro = 0;
    const observador = new ResizeObserver((entradas) => {
        const largura = Math.round(entradas[0].contentRect.width);
        if (largura <= 0 || largura === ultima) {
            return;
        }

        ultima = largura;
        cancelAnimationFrame(quadro);
        quadro = requestAnimationFrame(() => {
            dotnet.invokeMethodAsync('DefinirLargura', largura).catch(() => { });
        });
    });
    observador.observe(elemento);

    return {
        parar() {
            observador.disconnect();
            cancelAnimationFrame(quadro);
        }
    };
}

// A roda do mouse so pode cancelar a rolagem da pagina num ouvinte NAO passivo, e o Blazor registra os
// dele como passivos: por isso o zoom pela roda entra por aqui, e nao por @onwheel.
export function observarRoda(elemento, dotnet) {
    if (!elemento) {
        return null;
    }

    const aoRodar = (evento) => {
        evento.preventDefault();
        const caixa = elemento.getBoundingClientRect();
        dotnet.invokeMethodAsync('RodarNoGrafico', evento.deltaY,
            evento.clientX - caixa.left, evento.clientY - caixa.top, evento.shiftKey).catch(() => { });
    };

    elemento.addEventListener('wheel', aoRodar, { passive: false });

    return {
        parar() {
            elemento.removeEventListener('wheel', aoRodar);
        }
    };
}

// --- Exportar ---

// Propriedades que o SVG precisa levar embutidas: o CSS do grafico e isolado (mora numa folha do site), e
// um SVG salvo sem elas abre preto e branco em qualquer editor.
const PROPRIEDADES = [
    'fill', 'fill-opacity', 'stroke', 'stroke-width', 'stroke-opacity', 'stroke-dasharray',
    'stroke-linecap', 'stroke-linejoin', 'opacity', 'font-family', 'font-size', 'font-weight',
    'text-anchor', 'dominant-baseline', 'stop-color', 'stop-opacity', 'transform', 'transform-origin'
];

function corDeFundo(elemento) {
    for (let no = elemento; no && no !== document.documentElement; no = no.parentElement) {
        const cor = getComputedStyle(no).backgroundColor;
        if (cor && cor !== 'transparent' && !cor.startsWith('rgba(0, 0, 0, 0)')) {
            return cor;
        }
    }

    return getComputedStyle(document.documentElement).backgroundColor || '#ffffff';
}

/** O SVG como texto, com as cores e fontes embutidas e um fundo solido atras. */
export function serializar(svg) {
    if (!svg) {
        return '';
    }

    const largura = svg.viewBox.baseVal.width || svg.clientWidth;
    const altura = svg.viewBox.baseVal.height || svg.clientHeight;
    const copia = svg.cloneNode(true);
    const originais = [svg, ...svg.querySelectorAll('*')];
    const copias = [copia, ...copia.querySelectorAll('*')];
    for (let i = 0; i < originais.length; i++) {
        const calculado = getComputedStyle(originais[i]);
        const estilo = PROPRIEDADES
            .map((p) => [p, calculado.getPropertyValue(p)])
            .filter(([, v]) => v && v !== 'none' && v !== 'normal' && v !== 'auto')
            .map(([p, v]) => `${p}:${v}`)
            .join(';');
        if (estilo) {
            copias[i].setAttribute('style', estilo);
        }
    }

    copia.setAttribute('xmlns', 'http://www.w3.org/2000/svg');
    copia.setAttribute('width', largura);
    copia.setAttribute('height', altura);
    copia.removeAttribute('aria-hidden');
    const fundo = document.createElementNS('http://www.w3.org/2000/svg', 'rect');
    fundo.setAttribute('width', '100%');
    fundo.setAttribute('height', '100%');
    fundo.setAttribute('fill', corDeFundo(svg));
    copia.insertBefore(fundo, copia.firstChild);

    return new XMLSerializer().serializeToString(copia);
}

/** O grafico como imagem, em base64 (sem o prefixo data:). `escala` 2 dobra a resolucao. */
export async function paraImagem(svg, tipo, escala) {
    const texto = serializar(svg);
    const largura = Math.round((svg.viewBox.baseVal.width || svg.clientWidth) * escala);
    const altura = Math.round((svg.viewBox.baseVal.height || svg.clientHeight) * escala);
    const imagem = new Image();
    imagem.width = largura;
    imagem.height = altura;
    await new Promise((ok, erro) => {
        imagem.onload = ok;
        imagem.onerror = () => erro(new Error('Nao foi possivel desenhar o grafico na imagem.'));
        imagem.src = 'data:image/svg+xml;charset=utf-8,' + encodeURIComponent(texto);
    });

    const tela = document.createElement('canvas');
    tela.width = largura;
    tela.height = altura;
    const pincel = tela.getContext('2d');
    // JPEG nao tem transparencia: sem este fundo, a area vazia sai preta no PDF.
    pincel.fillStyle = corDeFundo(svg);
    pincel.fillRect(0, 0, largura, altura);
    pincel.drawImage(imagem, 0, 0, largura, altura);

    return tela.toDataURL(tipo, 0.92).split(',')[1];
}

/** Tamanho em px da imagem exportada, para o PDF saber o tamanho da pagina. */
export function tamanho(svg, escala) {
    return [
        Math.round((svg.viewBox.baseVal.width || svg.clientWidth) * escala),
        Math.round((svg.viewBox.baseVal.height || svg.clientHeight) * escala)
    ];
}

function escaparHtml(texto) {
    return String(texto).replace(/[&<>"]/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' })[c]);
}

/** Abre a caixa de impressao com o grafico sozinho na pagina, num quadro proprio. */
export function imprimir(svg, titulo) {
    const desenho = serializar(svg);
    const quadro = document.createElement('iframe');
    quadro.setAttribute('aria-hidden', 'true');
    quadro.setAttribute('title', 'Impressao do grafico');
    // Fora da vista, mas NAO display:none: um quadro escondido assim nao imprime em alguns navegadores.
    quadro.style.cssText = 'position: fixed; right: 0; bottom: 0; width: 1px; height: 1px; border: 0; opacity: 0;';
    quadro.srcdoc = `<!DOCTYPE html><html lang="pt-BR"><head><meta charset="utf-8"><title>${escaparHtml(titulo)}</title>`
        + '<style>@page { margin: 16mm } body { margin: 0; font-family: system-ui, sans-serif; color: #111 }'
        + 'h1 { font-size: 16px; margin: 0 0 12px } svg { width: 100%; height: auto }</style></head>'
        + `<body><h1>${escaparHtml(titulo)}</h1>${desenho}</body></html>`;

    quadro.onload = () => {
        quadro.contentWindow.focus();
        quadro.contentWindow.print();
        // O dialogo do navegador e sincrono na maioria dos casos, mas nao em todos: o quadro sai depois.
        setTimeout(() => quadro.remove(), 60_000);
    };

    document.body.appendChild(quadro);
}

/** Salva o arquivo: `base64` verdadeiro para binario, falso para texto. */
export function baixar(nome, mime, dados, base64) {
    const conteudo = base64
        ? Uint8Array.from(atob(dados), (c) => c.charCodeAt(0))
        : dados;
    const url = URL.createObjectURL(new Blob([conteudo], { type: mime }));
    const link = document.createElement('a');
    link.href = url;
    link.download = nome;
    document.body.appendChild(link);
    link.click();
    link.remove();
    // O navegador precisa de um instante com a URL viva para comecar o download.
    setTimeout(() => URL.revokeObjectURL(url), 10_000);
}
