using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Docs.Shared;

/// <summary>
/// Os componentes do site, na ordem das quatro ondas. Fonte unica do menu lateral e da pagina de
/// indice: componente novo entra aqui e aparece nos dois.
/// </summary>
public static class Catalogo
{
    public sealed record Componente(string Rota, string Nome, string Resumo)
    {
        /// <summary>O nome sem o prefixo, como aparece no menu ("Button").</summary>
        public string NomeCurto => Nome.StartsWith("Rvm", StringComparison.Ordinal) ? Nome[3..] : Nome;
    }

    public sealed record Grupo(string Titulo, IReadOnlyList<Componente> Componentes);

    public sealed record Exemplo(string Rota, string Nome, RvmIconName Icone, string Resumo);

    /// <summary>As telas completas montadas com a biblioteca (secao Exemplos do menu).</summary>
    public static readonly IReadOnlyList<Exemplo> Exemplos =
    [
        new("exemplos/dashboard", "Dashboard", RvmIconName.Eye, "Indicadores, metas, atividade e pedidos da safra."),
        new("exemplos/cms", "CMS", RvmIconName.Pencil, "Publicacoes com abas, grade, acoes e editor em dialogo."),
        new("exemplos/crm", "CRM", RvmIconName.User, "Funil de oportunidades e ficha do cliente em gaveta."),
        new("exemplos/erp", "ERP", RvmIconName.File, "Pedidos, estoque e financeiro, com pedido em etapas."),
        new("exemplos/planner", "Planner", RvmIconName.CircleCheck, "Quadro de tarefas com calendario e nova tarefa.")
    ];

    public static readonly IReadOnlyList<Grupo> Grupos =
    [
        new("Fundacao",
        [
            new("componentes/typography", "RvmTypography", "Toda a escala tipografica do kit, em 24 estilos."),
            new("componentes/divider", "RvmDivider", "Linha que separa blocos, com ou sem rotulo no meio."),
            new("componentes/icon", "RvmIcon", "Icones Tabler, em tres tamanhos e por papel de cor."),
            new("componentes/button", "RvmButton", "Contained, outlined e text; tres tamanhos; icone e carregando."),
            new("componentes/avatar", "RvmAvatar", "Imagem, iniciais ou icone, em tres tamanhos."),
            new("componentes/chip", "RvmChip", "Filled e outlined, com avatar, removivel."),
            new("componentes/alert", "RvmAlert", "Seis papeis, contained e outlined, fechavel."),
            new("componentes/card", "RvmCard", "Basico, com cabecalho, com acoes, com midia."),
            new("componentes/text-field", "RvmTextField", "Outlined e filled, com rotulo, apoio e erro.")
        ]),
        new("Formulario e navegacao",
        [
            new("componentes/checkbox", "RvmCheckbox", "Nativo, com rotulo e indeterminado."),
            new("componentes/radio", "RvmRadio", "Grupo empilhado ou em linha; setas movem a escolha."),
            new("componentes/switch", "RvmSwitch", "Liga e desliga, em dois tamanhos."),
            new("componentes/select", "RvmSelect", "Simples, multiplo e com busca, com validacao."),
            new("componentes/tabs", "RvmTabs", "Sublinhadas e preenchidas, horizontal e vertical, com teclado."),
            new("componentes/breadcrumbs", "RvmBreadcrumbs", "Trilha de navegacao, com barra ou seta."),
            new("componentes/menu", "RvmMenu", "Botao que abre acoes, com icone, divisor e teclado."),
            new("componentes/pagination", "RvmPagination", "Numerica, com setas, reticencias e tres tamanhos."),
            new("componentes/badge", "RvmBadge", "Contador e ponto, sozinho ou sobre um icone."),
            new("componentes/tooltip", "RvmTooltip", "Dica em quatro posicoes, so com CSS.")
        ]),
        new("Feedback e sobreposicao",
        [
            new("componentes/progress", "RvmProgress", "Barra e anel, determinado e indeterminado."),
            new("componentes/skeleton", "RvmSkeleton", "Texto, retangulo e circulo enquanto carrega."),
            new("componentes/empty-state", "RvmEmptyState", "O vazio que ensina o proximo passo."),
            new("componentes/list", "RvmList", "Simples, com icone, com acao e aninhada."),
            new("componentes/accordion", "RvmAccordion", "Simples, exclusivo e com icone."),
            new("componentes/dialog", "RvmDialog", "Simples, confirmacao, formulario e tela cheia."),
            new("componentes/drawer", "RvmDrawer", "Quatro lados, temporario e permanente."),
            new("componentes/snackbar", "RvmSnackbar", "Seis papeis, com acao e fila.")
        ]),
        new("Dados e shell",
        [
            new("componentes/rating", "RvmRating", "Leitura e edicao, meia estrela."),
            new("componentes/stepper", "RvmStepper", "Horizontal, vertical, com validacao."),
            new("componentes/timeline", "RvmTimeline", "Alternada, alinhada, com data."),
            new("componentes/date-picker", "RvmDatePicker", "Data e intervalo."),
            new("componentes/time-picker", "RvmTimePicker", "Lista ou relogio circular, 12 h e 24 h."),
            new("componentes/table", "RvmTable", "Basica, ordenavel, com selecao, densa."),
            new("componentes/data-grid", "RvmDataGrid", "Paginacao, ordenacao, filtro por coluna."),
            new("componentes/app-shell", "RvmAppShell", "Topo, menu lateral e conteudo, responsivo.")
        ]),
        new("Graficos",
        [
            new("componentes/column-chart", "RvmColumnChart", "Colunas lado a lado ou empilhadas."),
            new("componentes/bar-chart", "RvmBarChart", "Barras horizontais para nomes longos."),
            new("componentes/histogram", "RvmHistogram", "Distribuicao de valores em faixas.")
        ])
    ];
}
