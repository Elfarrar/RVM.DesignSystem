using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace RVM.DesignSystem.Components.Switch;

/// <summary>
/// Liga e desliga algo que vale na hora (notificacoes, modo compacto). Para uma escolha que so vale
/// quando o formulario e enviado, o checkbox costuma ser o controle mais honesto.
/// </summary>
public partial class RvmSwitch : ComponentBase
{
    /// <summary>Ligado ou desligado. Aceita <c>@bind-Value</c>.</summary>
    [Parameter] public bool Value { get; set; }

    /// <summary>Disparado quando muda.</summary>
    [Parameter] public EventCallback<bool> ValueChanged { get; set; }

    /// <summary>Expressao do valor ligado. O <c>@bind-Value</c> preenche sozinho.</summary>
    [Parameter] public Expression<Func<bool>>? ValueExpression { get; set; }

    private Expression<Func<bool>>? _expressaoPadrao;

    /// <summary>
    /// A expressao que vai para o InputCheckbox. Ele EXIGE uma, e so o <c>@bind-Value</c> a fornece:
    /// sem ela, usos comuns como <c>Value="true" Disabled="true"</c> (so exibir) ou
    /// <c>Value</c> + <c>ValueChanged</c> faziam o controle estourar e sumir da tela — com o
    /// <c>#blazor-error-ui</c> no ar. Pego no navegador; o bUnit sempre passava a expressao.
    /// </summary>
    internal Expression<Func<bool>> ExpressaoEfetiva => ValueExpression ?? (_expressaoPadrao ??= () => Value);

    /// <summary>Texto ao lado. Tambem e o nome acessivel.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Rotulo livre, quando <see cref="Label"/> esta vazio.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Papel de cor do ligado. Padrao: <see cref="RvmColor.Primary"/>.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Primary;

    /// <summary>
    /// Trilho de 34 x 14 px (padrao) ou 26 x 10 px (<see cref="RvmSize.Small"/>), medidos no kit.
    /// O kit nao define um switch grande, entao <see cref="RvmSize.Large"/> sai igual ao medio.
    /// </summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>Indisponivel.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Id do input nativo. Sem ele, o Blazor nao gera um: o rotulo envolve o input e dispensa o <c>for</c>.</summary>
    [Parameter] public string? Id { get; set; }

    /// <summary>Nome enviado no formulario. Sem ele, vem do <c>@bind-Value</c> (necessario em SSR estatico).</summary>
    [Parameter] public string? Name { get; set; }

    /// <summary>Nome do campo nas mensagens de validacao do <c>EditForm</c>.</summary>
    [Parameter] public string? DisplayName { get; set; }

    /// <summary>Texto de apoio abaixo do controle. Da lugar ao erro quando ha erro.</summary>
    [Parameter] public string? HelperText { get; set; }

    /// <summary>Erro informado por fora. Dentro de um <c>EditForm</c>, a mensagem da validacao ja aparece sozinha.</summary>
    [Parameter] public string? ErrorText { get; set; }

    /// <summary>Texto do link depois do rotulo ("Li e aceito os <u>termos</u>").</summary>
    [Parameter] public string? LinkText { get; set; }

    /// <summary>Destino do link depois do rotulo.</summary>
    [Parameter] public string? LinkHref { get; set; }

    /// <summary>De que lado do controle fica o rotulo. Padrao: depois.</summary>
    [Parameter] public RvmLabelPosition LabelPosition { get; set; } = RvmLabelPosition.End;

    /// <summary>Do contrato com o RVM.UI, onde todo campo tem. Controle de marcar nao tem texto de exemplo: sem efeito.</summary>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>Do contrato com o RVM.UI, onde todo campo tem. Controle de marcar nao tem caixa de texto: sem efeito.</summary>
    [Parameter] public RvmFieldShape Shape { get; set; }

    [CascadingParameter] private EditContext? ContextoDoFormulario { get; set; }

    private readonly string _idDoApoio = GeradorDeIds.Novo("rvm-controle-apoio");

    internal string? MensagemDeErro
        => !string.IsNullOrWhiteSpace(ErrorText) ? ErrorText
            : ContextoDoFormulario is not null && ExpressaoEfetiva is not null
                ? ContextoDoFormulario.GetValidationMessages(FieldIdentifier.Create(ExpressaoEfetiva)).FirstOrDefault()
                : null;

    internal string? MensagemDeApoio => MensagemDeErro ?? (string.IsNullOrWhiteSpace(HelperText) ? null : HelperText);

    internal bool TemLink => !string.IsNullOrWhiteSpace(LinkText) && !string.IsNullOrWhiteSpace(LinkHref);

    internal string IdDoApoio => _idDoApoio;

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos extras: <c>class</c> e <c>style</c> na raiz; o resto no input nativo.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = string.Join(' ',
                "rvm-controle rvm-switch",
                Size == RvmSize.Small ? "rvm-pequeno" : "rvm-medio",
                Color switch
                {
                    RvmColor.Secondary => "rvm-secondary",
                    RvmColor.Inverse => "rvm-inverse",
                    RvmColor.Info => "rvm-info",
                    RvmColor.Success => "rvm-success",
                    RvmColor.Warning => "rvm-warning",
                    RvmColor.Error => "rvm-error",
                    _ => "rvm-primary"
                });

            if (Disabled) proprias += " rvm-desabilitado";
            if (LabelPosition == RvmLabelPosition.Start) proprias += " rvm-rotulo-antes";

            return ClassesCss.Juntar(proprias, Class, AdditionalAttributes);
        }
    }

    internal string? EstiloDoConsumidor
        => AdditionalAttributes is not null
           && AdditionalAttributes.TryGetValue("style", out var valor)
           && valor is string texto
           && !string.IsNullOrWhiteSpace(texto)
            ? texto
            : null;

    internal IReadOnlyDictionary<string, object>? AtributosDoInput
        => AtributosDoControle.Montar(AdditionalAttributes, Id, Name, MensagemDeApoio is null ? null : IdDoApoio, MensagemDeErro is not null, false);
}
