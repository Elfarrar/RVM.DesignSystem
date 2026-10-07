using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.EmptyState;

/// <summary>
/// Tela ou bloco sem conteudo que diz o que aconteceu e o proximo passo. Serve para lista vazia,
/// busca sem resultado, erro de carregamento e pagina nao encontrada.
/// </summary>
public partial class RvmEmptyState : ComponentBase
{
    private static int _proximoId;
    private readonly string _idGerado = $"rvm-vazio-{Interlocked.Increment(ref _proximoId)}";

    /// <summary>O que aconteceu, em uma frase ("Nenhum talhao cadastrado ainda").</summary>
    [Parameter, EditorRequired] public string Title { get; set; } = string.Empty;

    /// <summary>Por que, e o que fazer ("Cadastre o primeiro para acompanhar a safra.").</summary>
    [Parameter] public string? Description { get; set; }

    /// <summary>Icone grande acima do titulo, quando nao ha ilustracao.</summary>
    [Parameter] public RvmIconName? Icon { get; set; }

    /// <summary>Ilustracao livre (imagem, SVG). Vence o mascote e o icone. E tratada como decorativa.</summary>
    [Parameter] public RenderFragment? Illustration { get; set; }

    /// <summary>Mascote no lugar do icone (decorativo: o titulo ja diz o que aconteceu). Vence o icone.</summary>
    [Parameter] public RvmMascotName? Mascot { get; set; }

    /// <summary>Papel de cor do circulo do icone: Primary no vazio comum, Error no erro, Secondary no discreto.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Primary;

    /// <summary>
    /// Anuncia ao leitor de tela quando aparece (<c>role="status"</c>). Ligue quando o vazio surge depois de uma
    /// busca, de um filtro ou de um carregamento; deixe desligado no vazio que ja estava na tela.
    /// </summary>
    [Parameter] public bool Status { get; set; }

    /// <summary>Nivel do titulo (2 a 6). Padrao: 2. Escolha o que encaixa na hierarquia da pagina.</summary>
    [Parameter] public int HeadingLevel { get; set; } = 2;

    /// <summary>Conteudo extra entre a descricao e as acoes.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>As acoes — normalmente um botao que resolve o vazio.</summary>
    [Parameter] public RenderFragment? Actions { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos extras, repassados a raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string IdTitulo => $"{_idGerado}-titulo";

    internal int NivelDoTitulo => Math.Clamp(HeadingLevel, 2, 6);

    internal string ClassesDaRaiz
        => ClassesCss.Juntar("rvm-vazio " + Color switch
        {
            RvmColor.Secondary => "rvm-secondary",
            RvmColor.Inverse => "rvm-inverse",
            RvmColor.Info => "rvm-info",
            RvmColor.Success => "rvm-success",
            RvmColor.Warning => "rvm-warning",
            RvmColor.Error => "rvm-error",
            _ => "rvm-primary"
        }, Class, AdditionalAttributes);
}
