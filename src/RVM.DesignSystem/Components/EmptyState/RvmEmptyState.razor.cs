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

    /// <summary>Ilustracao livre (imagem, SVG). Vence o icone. E tratada como decorativa.</summary>
    [Parameter] public RenderFragment? Illustration { get; set; }

    /// <summary>Nivel do titulo (2 a 6). Padrao: 2. Escolha o que encaixa na hierarquia da pagina.</summary>
    [Parameter] public int HeadingLevel { get; set; } = 2;

    /// <summary>Conteudo extra entre a descricao e as acoes.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>As acoes — normalmente um botao que resolve o vazio.</summary>
    [Parameter] public RenderFragment? Actions { get; set; }

    /// <summary>Atributos extras, repassados a raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string IdTitulo => $"{_idGerado}-titulo";

    internal int NivelDoTitulo => Math.Clamp(HeadingLevel, 2, 6);

    internal string ClassesDaRaiz
        => AdditionalAttributes is not null
           && AdditionalAttributes.TryGetValue("class", out var informada)
           && informada is string texto
           && !string.IsNullOrWhiteSpace(texto)
            ? $"vazio {texto}"
            : "vazio";
}
