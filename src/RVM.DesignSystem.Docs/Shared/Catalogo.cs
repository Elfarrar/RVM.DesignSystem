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
            new("componentes/mascot", "RvmMascot", "O icone da funcao (erro, vazio, carregando) num circulo, no lugar de ilustracao."),
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
        new("Campos e escolhas",
        [
            new("componentes/text-area", "RvmTextArea", "Area de texto: a moldura do campo de texto com varias linhas."),
            new("componentes/numeric-field", "RvmNumericField", "Numero lido na cultura do campo, com faixa, setas, casas e formato."),
            new("componentes/multi-text-field", "RvmMultiTextField", "Varios textos como tags dentro do campo."),
            new("componentes/text-field-select", "RvmTextFieldSelect", "Texto livre com a unidade escolhida no fim do campo."),
            new("componentes/autocomplete", "RvmAutocomplete", "Campo que busca sugestoes enquanto se digita."),
            new("componentes/option-list", "RvmOptionList", "Lista de opcoes no padrao listbox."),
            new("componentes/tag-option", "RvmTagOption", "Tag em pilula, com remover."),
            new("componentes/choice-chip", "RvmChoiceChip", "Escolha unica em forma de chip, dentro de um RvmRadioGroup."),
            new("componentes/color-picker", "RvmColorPicker", "Escolha de cor entre itens, como bolinhas de radio."),
            new("componentes/color-field", "RvmColorField", "Cor em hex: paleta com nome e, se permitido, cor composta."),
            new("componentes/icon-selector", "RvmIconSelector", "Escolha de icone numa janela com busca e grade."),
            new("componentes/file-upload", "RvmFileUpload", "Envio de arquivos por botao ou caixa de arrastar, com progresso."),
            new("componentes/media-upload", "RvmMediaUpload", "Envio de imagens com miniaturas, remover e tentar de novo."),
            new("componentes/profile-image-upload", "RvmProfileImageUpload", "Foto de perfil: avatar e o botao de enviar.")
        ]),
        new("Tabelas e listas",
        [
            new("componentes/data-table", "RvmDataTable", "Tabela de dados com colunas declaradas, ordenacao avisada ao app e selecao."),
            new("componentes/table-layout", "RvmTableLayout", "Moldura da tabela montada a mao, com linhas e cabecalhos."),
            new("componentes/table-cells", "RvmCell*", "As celulas prontas: texto, numero, usuario, status, progresso e mais."),
            new("componentes/file-icon", "RvmFileIcon", "Icone de arquivo pela extensao."),
            new("componentes/filter", "RvmFilter", "Botao de filtros com painel, grupos, limpar e aplicar."),
            new("componentes/list-group", "RvmListGroup", "Cartao de lista com cabecalho, menu e abas.")
        ]),
        new("Feedback e texto",
        [
            new("componentes/modal", "RvmModal", "Janela modal com titulo, foco preso, Esc e clique no fundo."),
            new("componentes/confirm-modal", "RvmConfirmModal", "Confirmacao de acao com icone, descricao e dois botoes."),
            new("componentes/toast", "RvmToastProvider", "Avisos passageiros disparados pelo servico IRvmToast."),
            new("componentes/spinner", "RvmSpinner", "Indicador de carregamento com nome para o leitor de tela."),
            new("componentes/progress-bar", "RvmProgressBar", "Barra de progresso com rotulo, valor e estado sem valor."),
            new("componentes/label", "RvmLabel", "Etiqueta de texto colorida, suave ou solida."),
            new("componentes/text", "RvmText", "Texto na escala tipografica do contrato."),
            new("componentes/icon-badge", "RvmIconBadge", "Icone em fundo colorido, simples ou artistico.")
        ]),
        new("Painel e aplicativos",
        [
            new("componentes/theme-provider", "RvmThemeProvider", "Tema do app: modo, destaque, paleta, fonte e acessibilidade."),
            new("componentes/theme-picker", "RvmThemePicker", "Seletor de tema para o usuario: aparencia, paleta e acessibilidade."),
            new("componentes/stat-card", "RvmStatCard", "Cartao de indicador com tendencia, visual e acao."),
            new("componentes/progress-card", "RvmProgressCard", "Cartao de meta com icone e barra de progresso."),
            new("componentes/project-card", "RvmProjectCard", "Cartoes de projeto e de tarefa, com equipe e prazo."),
            new("componentes/payment-card", "RvmPaymentCard", "Cartao de pagamento e conversor de moeda."),
            new("componentes/activity", "RvmActivity", "Linha do tempo de atividades e o cartao de historico."),
            new("componentes/comment", "RvmComment", "Comentarios com resposta e avaliacoes com nota."),
            new("componentes/notification-item", "RvmNotificationItem", "Item de notificacao com marcar como lida."),
            new("componentes/marker-button", "RvmMarkerButton", "Favoritar ou salvar, como botao de alternancia."),
            new("componentes/widget", "RvmWidget", "Gatilho de icone ou cartao que abre um painel.")
        ]),
        new("Navegacao e pagina",
        [
            new("componentes/sidebar", "RvmSidebar", "O menu lateral: marca, navegacao, rodape; branco ou colorido, recolhivel."),
            new("componentes/topbar", "RvmTopbar", "A barra do topo: titulo ou busca a esquerda, widgets e perfil a direita."),
            new("componentes/page-header", "RvmPageHeader", "O cabecalho da pagina: voltar, foto, trilha, titulo e acoes."),
            new("componentes/page-toolbar", "RvmPageToolbar", "A faixa de busca, abas e filtros abaixo do cabecalho."),
            new("componentes/back-button", "RvmBackButton", "Voltar: link com Href, botao sem ele."),
            new("componentes/link", "RvmLink", "Link de texto na cor do papel, com icone."),
            new("componentes/icon-button", "RvmIconButton", "Botao so de icone, com nome acessivel obrigatorio."),
            new("componentes/menu-button", "RvmMenuButton", "Botao com texto que abre um menu."),
            new("componentes/button-group", "RvmButtonGroup", "Escolha unica em botoes colados."),
            new("componentes/collapse", "RvmCollapse", "Mostra ou esconde um bloco sem perder o que foi digitado."),
            new("componentes/expansion-panel", "RvmExpansionPanel", "Painel expansivel sozinho, que mantem o conteudo montado."),
            new("componentes/steps", "RvmSteps", "Etapas de um fluxo, com conteudo e Voltar/Avancar."),
            new("componentes/step-indicator", "RvmStepIndicator", "O numero do passo num circulo."),
            new("componentes/detail-profile-layout", "RvmDetailProfileLayout", "Ficha de detalhe: miolo e perfil numa coluna fixa.")
        ]),
        new("Graficos",
        [
            new("componentes/column-chart", "RvmColumnChart", "Colunas lado a lado ou empilhadas."),
            new("componentes/bar-chart", "RvmBarChart", "Barras horizontais para nomes longos."),
            new("componentes/histogram", "RvmHistogram", "Distribuicao de valores em faixas."),
            new("componentes/line-chart", "RvmLineChart", "Evolucao no tempo, reta ou suave."),
            new("componentes/area-chart", "RvmAreaChart", "Linha com o volume preenchido em degrade."),
            new("componentes/scatter-chart", "RvmScatterChart", "Relacao entre duas variaveis numericas."),
            new("componentes/pie-chart", "RvmPieChart", "Pizza e rosca, com percentuais."),
            new("componentes/radar-chart", "RvmRadarChart", "Varias variaveis a partir de um centro."),
            new("componentes/donut-chart", "RvmDonutChart", "Rosca inteira ou meia, fina ou grossa, com o valor no centro."),
            new("componentes/meter-chart", "RvmMeterChart", "Medidor em arco: quanto da meta foi atingido."),
            new("componentes/bubble-chart", "RvmBubbleChart", "Bolhas com a area proporcional ao valor."),
            new("componentes/multilayer-donut-chart", "RvmMultilayerDonutChart", "Aneis concentricos, um por item, ate o maximo."),
            new("componentes/chart-legend", "RvmChartLegend", "Legenda solta, com as cores dos graficos.")
        ])
    ];
}
