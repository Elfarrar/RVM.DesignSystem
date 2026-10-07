using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Chart;

/// <summary>
/// O que todo grafico sem eixo tem: o nome acessivel, os tres estados (carregando, erro, vazio), a dica e a animacao
/// de entrada (contrato com o RVM.UI, DSGN-017). Convive com o <see cref="RvmChartBase{TItem}"/>, que e a base dos
/// graficos com itens e series; esta e a dos graficos de fatias (<see cref="RvmRadialChartBase"/>).
/// </summary>
public abstract class RvmChartBase : ComponentBase
{
    /// <summary>
    /// O que o grafico mostra, para quem usa leitor de tela ("Area plantada por cultura"). Obrigatorio: grafico sem
    /// nome e um desenho mudo.
    /// </summary>
    [Parameter, EditorRequired] public string Label { get; set; } = string.Empty;

    /// <summary>Os dados estao chegando: o desenho da lugar ao aviso de carregamento.</summary>
    [Parameter] public bool Loading { get; set; }

    /// <summary>Nao ha nada para desenhar. Use <see cref="Empty"/> para ensinar o proximo passo.</summary>
    [Parameter] public bool IsEmpty { get; set; }

    /// <summary>A carga falhou. Vence <see cref="Loading"/> e <see cref="IsEmpty"/>.</summary>
    [Parameter] public bool Error { get; set; }

    /// <summary>O vazio que ensina o proximo passo. Sem ele, sai um <c>RvmEmptyState</c> com <see cref="EmptyText"/>.</summary>
    [Parameter] public RenderFragment? Empty { get; set; }

    /// <summary>O erro. Sem ele, sai o <c>RvmEmptyState</c> de erro com <see cref="ErrorTitle"/> e <see cref="ErrorText"/>.</summary>
    [Parameter] public RenderFragment? ErrorContent { get; set; }

    /// <summary>Mascote do erro padrao. Sem efeito com <see cref="ErrorContent"/>.</summary>
    [Parameter] public RvmMascotName? ErrorMascot { get; set; }

    /// <summary>Mascote do carregamento, no lugar do indicador circular.</summary>
    [Parameter] public RvmMascotName? LoadingMascot { get; set; }

    /// <summary>Texto do carregamento.</summary>
    [Parameter] public string LoadingText { get; set; } = "Carregando o grafico...";

    /// <summary>Texto do vazio padrao. Prefira <see cref="Empty"/>, que ensina o proximo passo.</summary>
    [Parameter] public string EmptyText { get; set; } = "Ainda nao ha dado para este grafico.";

    /// <summary>Titulo do erro padrao.</summary>
    [Parameter] public string ErrorTitle { get; set; } = "Nao deu para carregar o grafico";

    /// <summary>Texto do erro padrao.</summary>
    [Parameter] public string ErrorText { get; set; } = EstadosDosDados.TextoDeErro;

    /// <summary>
    /// A dica ao passar o mouse ou dar foco numa fatia, com o nome, o valor e o percentual. Sem JS: aparece por
    /// <c>:hover</c> e <c>:focus-visible</c>. Custa uma parada de Tab por fatia; desligue num grafico decorativo, dentro
    /// de um cartao que ja diz o numero. A tabela de dados para leitor de tela continua.
    /// </summary>
    [Parameter] public bool ShowTooltip { get; set; } = true;

    /// <summary>
    /// Anima o desenho ao aparecer. Padrao: sim — e desligada sozinha para quem pediu "reduzir movimento" no sistema,
    /// sem depender deste parametro.
    /// </summary>
    [Parameter] public bool Animated { get; set; } = true;

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos extras, repassados a figura.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Base dos ids gerados (mascaras).</summary>
    internal string IdBase { get; } = GeradorDeIds.Novo("rvm-grafico");

    /// <summary>
    /// Se o grafico ficaria em branco com o dado que recebeu. Cada tipo sabe o seu: sem isto, um grafico sem valor
    /// desenharia so a trilha, o que nao diz a ninguem que nao ha dado.
    /// </summary>
    private protected abstract bool SemDado { get; }

    /// <summary>Em que estado o grafico esta. Erro vence carregando, que vence vazio.</summary>
    internal EstadoDosDados Estado => EstadosDosDados.Qual(Error, Loading, IsEmpty || SemDado);
}
