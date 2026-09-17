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
