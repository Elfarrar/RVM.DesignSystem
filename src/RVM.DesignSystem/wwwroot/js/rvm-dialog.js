// Ponte para o <dialog> nativo.
//
// É deliberadamente MINÚSCULA, e essa é a questão. A alternativa — foco preso escrito à mão —
// custa umas 150 linhas de JS que precisam: achar todo elemento focável (a lista muda entre
// navegadores), tratar Shift+Tab, tratar foco que sai por clique, tratar conteúdo que aparece
// depois, guardar e restaurar o foco de origem, e tornar o resto da página inerte. Cada um
// desses é um lugar conhecido de errar.
//
// `showModal()` entrega os seis de graça, no navegador, testado por quem escreveu o navegador:
// foco preso, ESC, `::backdrop`, inertização do resto do documento, camada de topo (acima de
// qualquer z-index) e retorno do foco ao elemento que estava ativo quando o diálogo abriu.
//
// É a mesma escolha do <select> nativo no RvmSelect, pelo mesmo motivo.

export function abrir(dialog) {
    if (!dialog || dialog.open) {
        return;
    }

    dialog.showModal();
}

export function fechar(dialog) {
    if (!dialog || !dialog.open) {
        return;
    }

    // close() dispara o evento `close`, que o Blazor escuta para sincronizar o estado. Sem
    // isto, fechar pelo C# deixaria o componente achando que ainda está aberto.
    dialog.close();
}

// O ESC do <dialog> dispara `cancel` e fecha SEM passar pelo nosso callback. Isso é um problema
// real: um diálogo de confirmação fechado por ESC precisa devolver "cancelado", e não sumir em
// silêncio deixando quem chamou esperando para sempre.
//
// Cancelar o evento e deixar o C# decidir devolve o controle sem perder o comportamento nativo.
export function interceptarEsc(dialog, referencia) {
    if (!dialog) {
        return;
    }

    dialog.addEventListener('cancel', (evento) => {
        evento.preventDefault();
        referencia.invokeMethodAsync('AoCancelarPeloNavegador');
    });
}
