using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace RVM.DesignSystem.Components.Radio;

/// <summary>
/// Grupo de opcoes em que so uma vale. Os filhos sao <see cref="RvmRadio{TValue}"/>, que herdam a
/// cor, o tamanho e o estado desabilitado daqui.
/// </summary>
/// <typeparam name="TValue">Tipo do valor escolhido — um enum costuma ser o mais claro.</typeparam>
public partial class RvmRadioGroup<TValue> : ComponentBase
{
    /// <summary>O valor escolhido. Aceita <c>@bind-Value</c>.</summary>
    [Parameter] public TValue? Value { get; set; }

    /// <summary>Disparado quando a escolha muda.</summary>
    [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }

    /// <summary>Expressao do valor ligado. O <c>@bind-Value</c> preenche sozinho.</summary>
    [Parameter] public Expression<Func<TValue?>>? ValueExpression { get; set; }

    private Expression<Func<TValue?>>? _expressaoPadrao;

    /// <summary>
    /// O InputRadioGroup EXIGE uma expressao, e so o <c>@bind-Value</c> a fornece. Sem esta
    /// alternativa, <c>Value</c> + <c>ValueChanged</c> fazia o grupo estourar em tempo de execucao.
    /// </summary>
    internal Expression<Func<TValue?>> ExpressaoEfetiva => ValueExpression ?? (_expressaoPadrao ??= () => Value);

    /// <summary>A pergunta do grupo ("Forma de pagamento"). Vira a legenda do fieldset.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>
    /// <c>name</c> dos radios. Sem ele, o Blazor gera um a partir do <c>@bind-Value</c> — que e o que
    /// o formulario em SSR estatico precisa.
    /// </summary>
    [Parameter] public string? Name { get; set; }

    /// <summary>Opcoes em linha (<see cref="RvmOrientation.Horizontal"/>) ou empilhadas (padrao).</summary>
    [Parameter] public RvmOrientation Orientation { get; set; } = RvmOrientation.Vertical;

    /// <summary>Papel de cor da opcao escolhida. Padrao: <see cref="RvmColor.Primary"/>.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Primary;

    /// <summary>18, 20 (padrao) ou 22 px de circulo — medidos no kit.</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>Desabilita o grupo inteiro.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>As opcoes.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Id do fieldset do grupo.</summary>
    [Parameter] public string? Id { get; set; }

    /// <summary>Nome do grupo nas mensagens de validacao do <c>EditForm</c>.</summary>
    [Parameter] public string? DisplayName { get; set; }

    /// <summary>Texto de apoio abaixo das opcoes. Da lugar ao erro quando ha erro.</summary>
    [Parameter] public string? HelperText { get; set; }

    /// <summary>Erro informado por fora. Dentro de um <c>EditForm</c>, a mensagem da validacao ja aparece sozinha.</summary>
    [Parameter] public string? ErrorText { get; set; }

    /// <summary>Texto do link ao lado da pergunta.</summary>
    [Parameter] public string? LinkText { get; set; }

    /// <summary>Destino do link ao lado da pergunta.</summary>
    [Parameter] public string? LinkHref { get; set; }

    /// <summary>Do contrato com o RVM.UI, onde todo campo tem. Grupo de opcoes nao tem texto de exemplo: sem efeito.</summary>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>Do contrato com o RVM.UI, onde todo campo tem. Grupo de opcoes nao tem caixa: sem efeito.</summary>
    [Parameter] public RvmFieldShape Shape { get; set; }

    [CascadingParameter] private EditContext? ContextoDoFormulario { get; set; }

    private readonly string _idDoApoio = GeradorDeIds.Novo("rvm-grupo-apoio");

    internal string IdDoApoio => _idDoApoio;

    // class ja entrou no ClassesDaRaiz; repassada por ultimo, ela apagaria as classes do proprio grupo.
    internal IReadOnlyDictionary<string, object>? AtributosSemClasse
        => AdditionalAttributes?.Where(a => !string.Equals(a.Key, "class", StringComparison.OrdinalIgnoreCase)).ToDictionary(a => a.Key, a => a.Value);

    internal string? MensagemDeErro
        => !string.IsNullOrWhiteSpace(ErrorText) ? ErrorText
            : ContextoDoFormulario?.GetValidationMessages(FieldIdentifier.Create(ExpressaoEfetiva)).FirstOrDefault();

    internal string? MensagemDeApoio => MensagemDeErro ?? (string.IsNullOrWhiteSpace(HelperText) ? null : HelperText);

    /// <summary>Atributos extras, repassados ao fieldset.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = Orientation == RvmOrientation.Horizontal ? "rvm-grupo rvm-horizontal" : "rvm-grupo rvm-vertical";
            return ClassesCss.Juntar(proprias, Class, AdditionalAttributes);
        }
    }
}
