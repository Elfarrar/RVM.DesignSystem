using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace RVM.DesignSystem.Components.Checkbox;

/// <summary>
/// Caixa de marcar. Sozinha, com rotulo ou indeterminada. Dentro de um <see cref="EditForm"/>
/// participa da validacao; em formulario SSR estatico funciona sem JS.
/// </summary>
public partial class RvmCheckbox : ComponentBase, IAsyncDisposable
{
    private InputCheckbox? _entrada;
    private IJSObjectReference? _modulo;
    private bool? _indeterminadoAplicado;

    [Inject] private IJSRuntime JS { get; set; } = default!;

    /// <summary>Marcado ou nao. Aceita <c>@bind-Value</c>.</summary>
    [Parameter] public bool Value { get; set; }

    /// <summary>Disparado quando a marcacao muda.</summary>
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

    /// <summary>Texto ao lado da caixa. Tambem e o nome acessivel.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>
    /// Rotulo livre, quando o texto precisa de marcacao (um link para os termos, por exemplo). Vale
    /// quando <see cref="Label"/> esta vazio.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Papel de cor da caixa marcada. Padrao: <see cref="RvmColor.Primary"/>.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Primary;

    /// <summary>16, 18 (padrao) ou 22 px de caixa — medidos no kit.</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>
    /// Estado "alguns marcados" — a caixa-mae de uma lista. O visual sai por CSS ja no primeiro
    /// render; a semantica "misto" para o leitor de tela e completada por JS quando disponivel.
    /// Quem liga isto e responsavel por desligar quando os filhos ficarem todos iguais.
    /// </summary>
    [Parameter] public bool Indeterminate { get; set; }

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

    /// <summary>Obrigatorio: asterisco no rotulo e <c>aria-required</c> no input. A validacao e do <c>EditForm</c>.</summary>
    [Parameter] public bool Required { get; set; }

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

    /// <summary>
    /// Atributos extras. <c>class</c> e <c>style</c> vao para a raiz (onde layout faz efeito); o
    /// resto vai para o input nativo (<c>aria-*</c>, <c>data-*</c>, <c>id</c>, <c>name</c>).
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = string.Join(' ',
                "rvm-controle rvm-checkbox",
                Size switch { RvmSize.Small => "rvm-pequeno", RvmSize.Large => "rvm-grande", _ => "rvm-medio" },
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

            if (Indeterminate) proprias += " rvm-indeterminado";
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
        => AtributosDoControle.Montar(AdditionalAttributes, Id, Name, MensagemDeApoio is null ? null : IdDoApoio, MensagemDeErro is not null, Required);

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // So conversa com o JS quando o estado muda — e o primeiro render so precisa se estiver
        // indeterminado, porque o padrao do DOM ja e "nao indeterminado".
        if (_indeterminadoAplicado == Indeterminate || (_indeterminadoAplicado is null && !Indeterminate))
        {
            _indeterminadoAplicado ??= Indeterminate;
            return;
        }

        try
        {
            _modulo ??= await JS.InvokeAsync<IJSObjectReference>(
                "import", "./_content/RVM.DesignSystem/rvm-checkbox.js");
            await _modulo.InvokeVoidAsync("definirIndeterminado", _entrada?.Element, Indeterminate);
            _indeterminadoAplicado = Indeterminate;
        }
        catch (Exception e) when (e is JSException or JSDisconnectedException or InvalidOperationException or TaskCanceledException)
        {
            // Sem JS (pre-renderizacao, circuito caindo): o visual indeterminado ja esta no CSS.
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_modulo is not null)
        {
            try
            {
                await _modulo.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Circuito ja caiu no Blazor Server: nao ha o que liberar do lado do navegador.
            }
        }

        GC.SuppressFinalize(this);
    }
}
