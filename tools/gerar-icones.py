"""
Gera Components/Icons/RvmIconData.cs a partir dos SVGs do Phosphor.

Rodar so quando a lista de icones mudar:
    python tools/gerar-icones.py <pasta-com-svgs>

O resultado e COMMITADO. O build nao baixa nada e nao depende de rede — reconstruir o
pacote offline continua funcionando, e o conjunto de icones fica visivel em code review
em vez de aparecer por magica no CI.
"""
import re, sys, pathlib, collections

# A lista e CURADA, nao o conjunto inteiro do Phosphor (1512 icones), e a curadoria e POR
# APLICACAO: o conjunto cobre o vocabulario de que uma tela de negocio precisa (acao, estado,
# arquivo, dinheiro, pessoa, tempo, obra, agro), nao apenas o que os componentes daqui usam.
#
# ⚠️ Isto REVISA a regra anterior ("icone entra quando um componente precisa"), em 08/09/2026,
# a pedido do Rafael. O motivo esta no bloco da DSGN-026, mais abaixo, junto com a medicao que
# derrubou o argumento de bytes que eu tinha usado para justificar a regra antiga.
ICONES = [
    # Onda 1 — formulario e estado
    'check', 'x', 'caret-down', 'caret-up', 'eye', 'eye-slash',
    'warning', 'info', 'circle-notch', 'magnifying-glass', 'calendar-blank',
    # Onda 2 — layout e navegacao
    'list',                  # menu hamburguer do RvmTopbar no mobile
    'caret-right',           # separador do RvmBreadcrumb e seta do RvmNavItem com filhos
    'caret-left',            # voltar
    'sidebar-simple',        # colapsar/expandir o RvmSidebar
    'user',                  # fallback do RvmAvatar sem imagem nem iniciais
    'house',                 # item de navegacao mais comum
    'dots-three-vertical',   # menu de acoes secundarias
    'copy',                  # botao de copiar do bloco de codigo (site de documentacao)
    # Onda 3 — feedback
    'check-circle',          # severidade Success no RvmAlert e no RvmToast
    'x-circle',              # severidade Danger (o `warning` triangular fica com Warning)
    'lock',                  # RvmEmptyState, variante "sem permissao"
    'tray',                  # RvmEmptyState, variante "vazio"
    # Onda 4 — dados
    'caret-double-left',     # RvmPagination: primeira pagina
    'caret-double-right',    # RvmPagination: ultima pagina
    'arrows-down-up',        # RvmDataGrid: coluna ordenavel ainda NAO ordenada
    'funnel',                # RvmFilterBar

    # ---------------------------------------------------------------------------
    # DSGN-026 — o conjunto deixa de ser POR COMPONENTE e passa a ser POR APLICACAO.
    #
    # A regra antiga era 'icone entra quando um componente precisa'. Ela fazia sentido
    # enquanto a biblioteca estava sendo construida: o conjunto crescia junto com o que
    # existia para desenhar. Mas ela otimizava a coisa errada — quem consome a
    # biblioteca nao esta montando componentes, esta montando TELAS, e uma tela de
    # pedido precisa de um icone de pedido que nenhum componente meu jamais vai pedir.
    # Com 27 icones, o consumidor cai fora do design system no primeiro botao de
    # 'imprimir' — e um conjunto que empurra para fora falhou no proposito dele.
    #
    # ⚠️ E o argumento do custo, que eu mesmo escrevi, NAO se sustentava: medido, cada
    # icone-peso custa ~300 bytes de string no assembly. Os 27 davam 16 KB. Passar de
    # 27 para 142 custa ~70 KB de fonte, o que e ruido perto do runtime do WASM. O
    # argumento REAL para curar nunca foi byte: e superficie de API e manutencao —
    # e esse continua valendo, que e porque sao 142 e nao os 1512 do Phosphor.
    #
    # Nenhum nome antigo saiu. Remover um seria quebra depois da 1.0.
    # ---------------------------------------------------------------------------
    # Navegacao e estrutura
    'arrow-left', 'arrow-right', 'arrow-up', 'arrow-down', 'arrow-square-out', 'dots-three',
    'squares-four', 'stack', 'tree-structure', 'list-checks',
    # Acoes
    'plus', 'minus', 'pencil-simple', 'trash', 'floppy-disk', 'clipboard-text',
    'download-simple', 'upload-simple', 'share-network', 'printer', 'arrow-clockwise',
    'arrows-clockwise', 'sort-ascending', 'sort-descending', 'dots-six-vertical',
    'check-square',
    # Estado
    'warning-circle', 'question', 'prohibit', 'seal-check', 'clock', 'hourglass', 'bell',
    'bell-ringing',
    # Arquivos
    'file', 'file-text', 'file-pdf', 'file-xls', 'file-csv', 'folder', 'folder-open',
    'paperclip', 'image', 'images', 'note-pencil',
    # Comunicacao
    'envelope', 'envelope-simple', 'chat-circle', 'chat-teardrop-text', 'phone',
    'whatsapp-logo', 'megaphone',
    # Pessoas
    'users', 'user-circle', 'user-plus', 'identification-card', 'address-book', 'user-gear',
    # Comercio e financeiro
    'currency-circle-dollar', 'money', 'receipt', 'shopping-cart', 'package', 'barcode',
    'credit-card', 'bank', 'calculator', 'percent', 'tag', 'hand-coins', 'wallet',
    # Tempo
    'calendar-check', 'calendar-dots', 'timer', 'clock-counter-clockwise',
    # Dados e graficos
    'chart-line', 'chart-bar', 'chart-pie-slice', 'chart-donut', 'trend-up', 'trend-down',
    'table', 'database',
    # Obra e campo
    'hammer', 'wrench', 'hard-hat', 'ruler', 'buildings', 'truck', 'map-pin', 'path',
    'crane-tower', 'toolbox',
    # Agro
    'plant', 'tractor', 'drop', 'sun', 'cloud-rain', 'thermometer-simple', 'leaf',
    # Sistema
    'gear', 'gear-six', 'sliders-horizontal', 'key', 'lock-open', 'shield-check', 'sign-out',
    'sign-in', 'power', 'cloud-arrow-up', 'link-simple', 'globe', 'translate', 'moon',
    'archive',

    # Vocabulario de interface — o que o proprio site de documentacao usa no menu, e o
    # que qualquer app precisa para dar forma a uma tela (controle, secao, estado visual).
    'palette', 'dots-nine', 'arrows-out-line-horizontal', 'rows', 'cursor-click', 'selection',
    'textbox', 'text-align-left', 'radio-button', 'toggle-right', 'note', 'browser',
    'rectangle', 'signpost', 'tabs', 'flow-arrow', 'cards', 'frame-corners', 'spinner-gap',
    'placeholder', 'book-open', 'person-arms-spread',
]

origem = pathlib.Path(sys.argv[1])
dados = collections.OrderedDict()

for nome in ICONES:
    for peso in ('regular', 'fill'):
        arq = origem / f'{nome}-{peso}.svg'
        svg = arq.read_text(encoding='utf-8')
        corpo = re.sub(r'^.*?<svg[^>]*>', '', svg, flags=re.S)
        corpo = re.sub(r'</svg>\s*$', '', corpo, flags=re.S).strip()
        # fill="currentColor" ja vem no <svg> raiz do Phosphor; no <path> ele e redundante
        # e atrapalha quando o consumidor quer pintar por CSS.
        corpo = corpo.replace(' fill="currentColor"', '')
        dados[(nome, peso)] = corpo

linhas = []
for (nome, peso), corpo in dados.items():
    linhas.append(f'        ["{nome}:{peso}"] = """{corpo}""",')

pathlib.Path('src/RVM.DesignSystem/Components/Icons/RvmIconData.cs').write_text(
'''// <auto-generated>
//     GERADO por tools/gerar-icones.py a partir dos SVGs do Phosphor (MIT).
//     Nao editar a mao — rodar o script de novo quando a lista de icones mudar.
// </auto-generated>

using System.Collections.Frozen;

namespace RVM.DesignSystem.Components;

/// <summary>Os desenhos dos icones, por nome e peso.</summary>
internal static class RvmIconData
{
    /// <summary>Todos os icones do conjunto, chaveados por <c>nome:peso</c>.</summary>
    internal static readonly FrozenDictionary<string, string> Todos = new Dictionary<string, string>(StringComparer.Ordinal)
    {
''' + '\n'.join(linhas) + '''
    }.ToFrozenDictionary(StringComparer.Ordinal);

    /// <summary>Os nomes disponiveis, para a pagina de icones do site e para a mensagem de erro.</summary>
    internal static readonly IReadOnlyList<string> Nomes =
    [
''' + '\n'.join(f'        "{n}",' for n in ICONES) + '''
    ];
}
''', encoding='utf-8', newline='\n')

print(f'gerado: {len(dados)} desenhos, {len(ICONES)} icones x 2 pesos')
