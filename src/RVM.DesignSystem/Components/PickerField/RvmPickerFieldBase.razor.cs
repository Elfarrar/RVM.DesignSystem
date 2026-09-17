using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RVM.DesignSystem.Components.Dialog;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.PickerField;

/// <summary>
/// O campo que abre um dialogo para escolher o valor: o botao, o dialogo com foco preso, o teclado, a
/// validacao e o envio. Base dos seletores de data e do relogio do seletor de horario.
/// </summary>
public abstract partial class RvmPickerFieldBase : ComponentBase, IAsyncDisposable
{
    private static int _proximoId;
    private readonly string _idBase = $"rvm-campo-dialogo-{Interlocked.Increment(ref _proximoId)}";
    private ElementReference _gatilho;
    private ElementReference _popup;
    private Sobreposicao? _sobreposicao;
    private bool _focoPreso;
    private EditContext? _contextoAssinado;
    private bool _aberto;
    private bool _focarConteudo;

    [Inject] private IJSRuntime JS { get; set; } = default!;

    [CascadingParameter] private EditContext? EditContext { get; set; }

    /// <summary>Rotulo do campo. Entra no nome acessivel junto com o valor.</summary>
    [Parameter, EditorRequired] public string Label { get; set; } = string.Empty;

    /// <summary>Texto de apoio abaixo. Da lugar ao erro quando ha erro.</summary>
    [Parameter] public string? HelperText { get; set; }

    /// <summary>Erro informado por fora. Dentro de um EditForm, a validacao do campo ja aparece sozinha.</summary>
    [Parameter] public string? ErrorText { get; set; }

    /// <summary>Texto no campo vazio. Padrao: o formato esperado ("dd/mm/aaaa", "hh:mm").</summary>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>56 px (padrao) ou 40 px (<see cref="RvmSize.Small"/>), como o campo de texto.</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>Asterisco no rotulo. A validacao em si e do EditForm.</summary>
    [Parameter] public bool Required { get; set; }

    /// <summary>Indisponivel.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// <c>name</c> para envio de formulario: o valor vai em <c>input hidden</c> em formato invariante
    /// (data em <c>yyyy-MM-dd</c>, com <c>{Name}Inicio</c> e <c>{Name}Fim</c> no intervalo; horario em <c>HH:mm</c>).
    /// </summary>
    [Parameter] public string? Name { get; set; }

    /// <summary>Atributos extras: <c>class</c> e <c>style</c> na raiz; o resto no botao do campo.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal abstract bool TemValor { get; }

    internal abstract string TextoExibido { get; }

    internal abstract IEnumerable<(string Sufixo, string Valor)> ValoresParaEnvio { get; }

    internal abstract LambdaExpression? ExpressaoDoCampo { get; }

    internal abstract string DialogLabel { get; }

    internal abstract string PlaceholderPadrao { get; }

    /// <summary>Icone a direita do campo.</summary>
    internal abstract RvmIconName IconeDoCampo { get; }

    internal string PlaceholderEfetivo => string.IsNullOrWhiteSpace(Placeholder) ? PlaceholderPadrao : Placeholder;

    /// <summary>O que o dialogo mostra (calendario, relogio).</summary>
    internal abstract RenderFragment ConteudoDoDialogo { get; }

    /// <summary>Leva o foco para dentro do conteudo aberto. Falso enquanto ele ainda nao existe.</summary>
    internal abstract Task<bool> FocarConteudoAsync();

    /// <summary>Chamado ao abrir, antes do render: o relogio copia o valor para o rascunho aqui.</summary>
    internal virtual void AoAbrir()
    {
    }

    /// <summary>Chamado ao fechar: solta a referencia ao conteudo que saiu da tela.</summary>
    internal virtual void AoFechar()
    {
    }

    internal bool Aberto => _aberto;

    internal FieldIdentifier? Campo { get; private set; }

    internal string IdGatilho => $"{_idBase}-campo";

    internal string IdRotulo => $"{_idBase}-rotulo";

    internal string IdValor => $"{_idBase}-valor";

    internal string IdApoio => $"{_idBase}-apoio";

    internal string? MensagemDeErro
        => !string.IsNullOrWhiteSpace(ErrorText)
            ? ErrorText
            : Campo is { } campo ? EditContext!.GetValidationMessages(campo).FirstOrDefault() : null;

    internal bool TemErro => MensagemDeErro is not null;

    internal string? MensagemDeApoio => MensagemDeErro ?? (string.IsNullOrWhiteSpace(HelperText) ? null : HelperText);

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = "rvm-campo-data " + (Size == RvmSize.Small ? "rvm-pequeno" : "rvm-medio");
            if (TemValor || _aberto) proprias += " rvm-rotulo-fixo";
            if (_aberto) proprias += " rvm-aberto";
            if (TemErro) proprias += " rvm-erro";
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

    internal IReadOnlyDictionary<string, object>? AtributosDoGatilho
        => AdditionalAttributes?
            .Where(a => !string.Equals(a.Key, "class", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(a.Key, "style", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(a => a.Key, a => a.Value);

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (!ReferenceEquals(EditContext, _contextoAssinado))
        {
            if (_contextoAssinado is not null) _contextoAssinado.OnValidationStateChanged -= AoMudarValidacao;
            if (EditContext is not null) EditContext.OnValidationStateChanged += AoMudarValidacao;
            _contextoAssinado = EditContext;
        }

        Campo = ExpressaoDoCampo is { } expressao && EditContext is not null ? CampoDoFormulario.Criar(expressao) : null;
    }

    private void AoMudarValidacao(object? sender, ValidationStateChangedEventArgs e) => StateHasChanged();

    private Task AbrirOuFechar()
    {
        if (Disabled)
        {
            return Task.CompletedTask;
        }

        if (_aberto)
        {
            return FecharAsync();
        }

        _aberto = true;
        _focarConteudo = true;
        AoAbrir();
        StateHasChanged();
        return Task.CompletedTask;
    }

    // Vale no dialogo e tambem no botao: logo depois de abrir, o foco ainda esta no botao ate o
    // rvm-sobreposicao.js carregar e move-lo — num site servido de longe, um Esc nesse intervalo nao
    // fechava nada (pego no E2E do dev; na maquina local o modulo chega antes da tecla).
    private async Task AoTeclarNoDialogoAsync(KeyboardEventArgs e)
    {
        if (e.Key == "Escape" && _aberto)
        {
            await FecharAsync();
        }
    }

    /// <summary>Fecha o dialogo. O foco volta ao botao pelo <c>rvm-sobreposicao.js</c>, que o guardou ao abrir.</summary>
    internal Task FecharAsync()
    {
        _aberto = false;
        _focarConteudo = false;
        AoFechar();
        StateHasChanged();
        return Task.CompletedTask;
    }

    /// <summary>Chamado pelos derivados depois de aplicar uma escolha.</summary>
    internal void AvisarFormulario()
    {
        if (Campo is { } campo)
        {
            EditContext!.NotifyFieldChanged(campo);
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_aberto && !_focoPreso)
        {
            // Prende o Tab no dialogo (aria-modal promete isso) e trava a rolagem da pagina.
            _focoPreso = true;
            _sobreposicao ??= new Sobreposicao(JS);
            await _sobreposicao.AbrirAsync(_popup);
        }
        else if (!_aberto && _focoPreso)
        {
            _focoPreso = false;
            if (_sobreposicao is not null)
            {
                await _sobreposicao.FecharAsync();
            }
        }

        if (_focarConteudo && await FocarConteudoAsync())
        {
            _focarConteudo = false;
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_contextoAssinado is not null)
        {
            _contextoAssinado.OnValidationStateChanged -= AoMudarValidacao;
        }

        if (_sobreposicao is not null)
        {
            await _sobreposicao.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }
}
