namespace RVM.DesignSystem.Components;

/// <summary>
/// Tamanho de um controle.
/// </summary>
/// <remarks>
/// Escala fechada e compartilhada por todos os componentes de propósito: é o que faz um botão
/// e um campo lado a lado terem a mesma altura sem ninguém ajustar pixel.
/// </remarks>
public enum RvmSize
{
    /// <summary>Compacto — tabela, barra de ferramentas.</summary>
    Small,

    /// <summary>Padrão.</summary>
    Medium,

    /// <summary>Destaque — ação principal de uma tela vazia.</summary>
    Large,
}

/// <summary>Peso visual de um botão.</summary>
public enum RvmButtonVariant
{
    /// <summary>Ação principal. Uma por tela.</summary>
    Primary,

    /// <summary>Ação de apoio, com contorno.</summary>
    Secondary,

    /// <summary>Sem preenchimento nem contorno — ação terciária.</summary>
    Ghost,

    /// <summary>Ação destrutiva.</summary>
    Danger,
}

/// <summary>Tipo HTML do botão.</summary>
public enum RvmButtonType
{
    /// <summary>Não submete formulário. É o padrão, e é o padrão certo.</summary>
    /// <remarks>
    /// O HTML puro usa <c>submit</c> quando o tipo é omitido, e isso é uma das causas mais
    /// comuns de "a página recarregou sozinha": um botão de ação dentro de um formulário
    /// submete sem ninguém pedir. Aqui o padrão é o seguro; quem quer submeter, diz.
    /// </remarks>
    Button,

    /// <summary>Submete o formulário.</summary>
    Submit,

    /// <summary>Limpa o formulário.</summary>
    Reset,
}

/// <summary>Peso do traço de um ícone (Phosphor).</summary>
/// <remarks>
/// A v1 expõe só estes dois — o par que expressa estado selecionado. O conjunto de origem tem
/// seis; acrescentar membro depois é aditivo e não quebra ninguém, remover é que quebra
/// (`03` § Ícones).
/// </remarks>
public enum RvmIconWeight
{
    /// <summary>Traço. O padrão.</summary>
    Regular,

    /// <summary>Preenchido — estado selecionado, ativo, marcado.</summary>
    Fill,
}

/// <summary>Tipo de um campo de texto.</summary>
/// <remarks>
/// A escolha muda o teclado que o celular abre e o autofill que o navegador oferece — não é
/// só validação. Um campo de telefone marcado como <c>Text</c> obriga o usuário a procurar os
/// números no teclado alfabético.
/// </remarks>
public enum RvmTextFieldType
{
    /// <summary>Texto livre.</summary>
    Text,

    /// <summary>E-mail.</summary>
    Email,

    /// <summary>Senha — o conteúdo é mascarado.</summary>
    Password,

    /// <summary>Telefone.</summary>
    Tel,

    /// <summary>Endereço web.</summary>
    Url,

    /// <summary>Busca.</summary>
    Search,
}

/// <summary>Densidade da interface (<c>RF-05</c>).</summary>
/// <remarks>
/// Afeta <b>altura de controle e espaçamento vertical</b>, nunca tamanho de fonte — encolher a
/// letra não deixa a tela densa, deixa a tela ilegível. A troca acontece na camada de token
/// (<c>data-rvm-density</c>), então nenhum componente precisa saber que densidade existe.
///
/// <para>
/// <b>A regra vence a configuração:</b> em tela estreita ou ponteiro grosso, a compacta é
/// desfeita e o alvo de toque volta aos 40px. Acessibilidade não é preferência de quem
/// desenvolve.
/// </para>
/// </remarks>
public enum RvmDensity
{
    /// <summary>Padrão — 40px de altura de controle. Formulário, cadastro, celular.</summary>
    Comfortable,

    /// <summary>32px — grid, listagem densa, tela de operação.</summary>
    Compact,
}

/// <summary>Direção de um eixo.</summary>
public enum RvmOrientation
{
    /// <summary>Da esquerda para a direita.</summary>
    Horizontal,

    /// <summary>De cima para baixo.</summary>
    Vertical,
}

/// <summary>
/// Um degrau da escala de espaçamento.
/// </summary>
/// <remarks>
/// Enum e não <c>string</c> ou <c>int</c> de propósito: é o que impede <c>Gap="13px"</c>. A
/// escala fechada é o que faz telas de autores diferentes parecerem a mesma tela.
/// </remarks>
public enum RvmSpacing
{
    /// <summary>Sem espaço.</summary>
    None,

    /// <summary>4px.</summary>
    Xs,

    /// <summary>8px.</summary>
    Sm,

    /// <summary>16px — o padrão.</summary>
    Md,

    /// <summary>24px.</summary>
    Lg,

    /// <summary>32px.</summary>
    Xl,

    /// <summary>48px.</summary>
    Xxl,
}

/// <summary>Alinhamento no eixo transversal (<c>align-items</c>).</summary>
public enum RvmAlign
{
    /// <summary>Ocupa a altura (ou largura) toda. Padrão do empilhamento vertical.</summary>
    Stretch,

    /// <summary>Início do eixo.</summary>
    Start,

    /// <summary>Centro.</summary>
    Center,

    /// <summary>Fim do eixo.</summary>
    End,

    /// <summary>Alinha pela linha de base do texto.</summary>
    Baseline,
}

/// <summary>Distribuição no eixo principal (<c>justify-content</c>).</summary>
public enum RvmJustify
{
    /// <summary>Agrupado no início.</summary>
    Start,

    /// <summary>Agrupado no centro.</summary>
    Center,

    /// <summary>Agrupado no fim.</summary>
    End,

    /// <summary>Extremos nas pontas, espaço igual entre os itens.</summary>
    SpaceBetween,

    /// <summary>Espaço igual em volta de cada item.</summary>
    SpaceAround,
}

/// <summary>Aparência de um cartão.</summary>
public enum RvmCardVariant
{
    /// <summary>Superfície elevada com sombra. O padrão.</summary>
    Elevated,

    /// <summary>Contorno, sem sombra — melhor em tela com muitos cartões lado a lado.</summary>
    Outlined,

    /// <summary>Superfície rebaixada, sem sombra nem contorno.</summary>
    Filled,
}

/// <summary>Severidade de um chip.</summary>
/// <remarks>
/// Severidade nunca é comunicada <b>só</b> por cor (`02` § Acessibilidade): o texto do chip
/// carrega o significado, a cor reforça.
/// </remarks>
public enum RvmChipVariant
{
    /// <summary>Sem carga semântica. O padrão.</summary>
    Neutral,

    /// <summary>Marca — categoria, filtro selecionado.</summary>
    Primary,

    /// <summary>Sucesso, aprovado, ativo.</summary>
    Success,

    /// <summary>Atenção, pendente.</summary>
    Warning,

    /// <summary>Erro, recusado, vencido.</summary>
    Danger,

    /// <summary>Informação neutra em destaque.</summary>
    Info,
}
