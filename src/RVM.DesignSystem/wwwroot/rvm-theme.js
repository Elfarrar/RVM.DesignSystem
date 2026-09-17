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
    }
};
