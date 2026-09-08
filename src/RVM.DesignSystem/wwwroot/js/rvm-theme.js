// Modulo de tema — a UNICA coisa que ele faz e ler/gravar a preferencia e refletir no
// elemento raiz. Nenhum estado visual vive aqui (`03` § JavaScript): o CSS ja sabe pintar
// os dois modos, e o que este arquivo controla e so QUAL deles vale.

const CHAVE = 'rvm-ds-theme';

export function getPreference() {
    try {
        return localStorage.getItem(CHAVE);
    } catch {
        // Navegador com storage bloqueado (janela anonima, politica corporativa). Sem
        // preferencia gravada o site segue o sistema, que e um padrao correto — nao um erro.
        return null;
    }
}

export function setPreference(preference) {
    const raiz = document.documentElement;

    if (preference === 'light' || preference === 'dark') {
        raiz.setAttribute('data-rvm-theme', preference);
    } else {
        // 'system': remover o atributo devolve o controle ao prefers-color-scheme do CSS.
        raiz.removeAttribute('data-rvm-theme');
    }

    try {
        localStorage.setItem(CHAVE, preference);
    } catch {
        // Idem: a troca ja aconteceu na tela; so nao sobrevive ao reload.
    }
}
