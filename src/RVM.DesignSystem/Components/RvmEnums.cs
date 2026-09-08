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
