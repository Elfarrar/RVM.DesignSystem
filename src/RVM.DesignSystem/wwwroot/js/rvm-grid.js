// Uma função só, e ela existe por um detalhe do HTML: `indeterminate` de um checkbox é
// PROPRIEDADE do elemento, não atributo. Não há como escrevê-la em marcação — nem em Blazor,
// nem em HTML puro.
//
// Sem ela, a caixa de "selecionar todos" teria só dois estados. Com 3 de 20 linhas marcadas,
// ela apareceria DESMARCADA, dizendo "nada selecionado" enquanto três coisas estão — e o
// próximo clique marcaria tudo em vez de limpar, que é o oposto do que a pessoa espera.
export function definirIndeterminado(checkbox, valor) {
    if (checkbox) {
        checkbox.indeterminate = valor;
    }
}
