// Persistencia do tema. O componente NAO depende deste arquivo para renderizar certo: o
// `data-theme` ja sai no elemento do RvmThemeProvider. Isto aqui existe para duas coisas:
//   1. pintar o <html>, para a area fora do provider (margem, barra de rolagem) acompanhar;
//   2. lembrar a escolha de quem esta vendo, por navegador.
//
// Para o tema entrar ANTES da primeira pintura e nao piscar branco, a pagina anfitria repete a
// leitura num <script> inline no <head> — ver o `index.html` do site de documentacao.
window.rvmTheme = {
    apply(theme, persist) {
        document.documentElement.setAttribute('data-theme', theme);
        if (persist) {
            try {
                localStorage.setItem('rvm-theme', theme);
            } catch {
                // Navegacao anonima ou armazenamento bloqueado: a troca continua valendo na sessao.
            }
        }
    },

    read() {
        try {
            const valor = localStorage.getItem('rvm-theme');
            return valor === 'dark' || valor === 'light' ? valor : null;
        } catch {
            return null;
        }
    },

    // Tema do usuario (RvmThemeState, DSGN-017): a escolha inteira serializada (`v1|blue|dark|default|0|0`).
    // Vai no localStorage, que o WebAssembly le de forma sincrona antes do primeiro render, e num cookie, que o
    // store de um app com servidor le na pre-renderizacao — nos dois casos a pagina ja nasce no tema certo.
    save(chave, valor) {
        try {
            localStorage.setItem(chave, valor);
        } catch {
            // localStorage bloqueado: o cookie ainda vale.
        }
        const seguro = location.protocol === 'https:' ? ';secure' : '';
        document.cookie = `${chave}=${encodeURIComponent(valor)};path=/;max-age=${60 * 60 * 24 * 365};samesite=lax${seguro}`;
    },

    // Para o "alternar" sair do tema que esta na tela quando a aparencia e automatica.
    prefersDark() {
        return window.matchMedia('(prefers-color-scheme: dark)').matches;
    },

    load(chave) {
        try {
            return localStorage.getItem(chave);
        } catch {
            return null;
        }
    }
};
