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
                    RvmColor.Info => "rvm-info",
                    RvmColor.Success => "rvm-success",
                    RvmColor.Warning => "rvm-warning",
                    RvmColor.Error => "rvm-error",
                    _ => "rvm-primary"
                });

            if (Indeterminate) proprias += " rvm-indeterminado";
            if (Disabled) proprias += " rvm-desabilitado";

            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
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
        => AdditionalAttributes?
            .Where(a => !string.Equals(a.Key, "class", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(a.Key, "style", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(a => a.Key, a => a.Value);

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
        catch (Exception e) when (e is JSException or InvalidOperationException or TaskCanceledException)
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
