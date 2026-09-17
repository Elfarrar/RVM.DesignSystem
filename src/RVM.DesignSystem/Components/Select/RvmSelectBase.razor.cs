using System.Globalization;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RVM.DesignSystem.Components.TextField;

namespace RVM.DesignSystem.Components.Select;

/// <summary>
/// O que <see cref="RvmSelect{TValue}"/> e <see cref="RvmMultiSelect{TValue}"/> tem em comum: a
/// caixa, a lista, o teclado, a busca e a validacao. So o que e "escolher" muda entre os dois.
/// </summary>
/// <typeparam name="TValue">Tipo de cada opcao.</typeparam>
public abstract partial class RvmSelectBase<TValue> : ComponentBase, IAsyncDisposable
{
    private readonly string _idBase = GeradorDeIds.Novo("rvm-select");
    private ElementReference _gatilho;
    private ElementReference _busca;
    private IJSObjectReference? _modulo;
    private EditContext? _contextoAssinado;
    private bool _aberto;
    private bool _focarBuscaAoAbrir;
    private bool _rolarAteAtiva;
    private string _termo = string.Empty;
    private List<TValue> _filtradas = [];
    private int _ativa = -1;

    [Inject] private IJSRuntime JS { get; set; } = default!;

    [CascadingParameter] private EditContext? EditContext { get; set; }

    /// <summary>
    /// As opcoes. Passe uma lista ja materializada: a colecao e percorrida a cada render do pai, e um
    /// <c>IQueryable</c> iria ao banco toda vez.
    /// </summary>
    [Parameter, EditorRequired] public IEnumerable<TValue> Items { get; set; } = [];

    /// <summary>Texto de cada opcao. Padrao: <c>ToString()</c>.</summary>
    [Parameter] public Func<TValue, string>? ItemText { get; set; }

    /// <summary>Opcoes visiveis mas nao escolhiveis.</summary>
    [Parameter] public Func<TValue, bool>? ItemDisabled { get; set; }

    /// <summary>Rotulo do campo. Tambem e o nome acessivel do combobox e da lista.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Texto de apoio abaixo. Da lugar ao erro quando ha erro.</summary>
    [Parameter] public string? HelperText { get; set; }

    /// <summary>Erro informado por fora. Dentro de um EditForm, a validacao do campo ja aparece sozinha.</summary>
    [Parameter] public string? ErrorText { get; set; }

    /// <summary>Texto no campo vazio. Com ele, o rotulo fica sempre la em cima.</summary>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>Estilo — os mesmos tres do campo de texto. Padrao: <see cref="RvmTextFieldVariant.Outlined"/>.</summary>
    [Parameter] public RvmTextFieldVariant Variant { get; set; } = RvmTextFieldVariant.Outlined;

    /// <summary>56 px (padrao) ou 40 px (<see cref="RvmSize.Small"/>), como o campo de texto.</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>Campo de busca no topo da lista, que filtra as opcoes enquanto se digita.</summary>
    [Parameter] public bool Searchable { get; set; }

    /// <summary>Dica e nome acessivel do campo de busca. Padrao: "Buscar".</summary>
    [Parameter] public string SearchPlaceholder { get; set; } = "Buscar";

    /// <summary>Mensagem quando a busca nao acha nada.</summary>
    [Parameter] public string NoResultsText { get; set; } = "Nenhuma opcao encontrada para essa busca.";

    /// <summary>Asterisco no rotulo e <c>aria-required</c>. A validacao em si e do EditForm.</summary>
    [Parameter] public bool Required { get; set; }

    /// <summary>Indisponivel.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// <c>name</c> para envio de formulario: cada valor escolhido vira um <c>input hidden</c>, com o
    /// valor em cultura invariante.
    /// </summary>
    [Parameter] public string? Name { get; set; }

    /// <summary>Atributos extras: <c>class</c> e <c>style</c> na raiz; o resto no combobox.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Escolhe varios (a lista fica aberta e marca cada um) ou um so.</summary>
    internal abstract bool Multiplo { get; }

    /// <summary>A expressao do valor ligado, para achar o campo no EditForm.</summary>
    internal abstract LambdaExpression? ExpressaoDoCampo { get; }

    internal abstract bool EstaSelecionado(TValue item);

    internal abstract IEnumerable<TValue> Selecionados { get; }

    /// <summary>Aplica a escolha. Devolve se a lista deve fechar.</summary>
    internal abstract Task<bool> AplicarEscolhaAsync(TValue item);

    internal bool Aberto => _aberto;

    internal IReadOnlyList<TValue> Filtradas => _filtradas;

    internal int IndiceAtivo => _ativa;

    internal string IdGatilho
        => AdditionalAttributes is not null
           && AdditionalAttributes.TryGetValue("id", out var informado)
           && informado is string texto
           && !string.IsNullOrWhiteSpace(texto)
            ? texto
            : $"{_idBase}-campo";

    internal string IdRotulo => $"{_idBase}-rotulo";

    internal string IdLista => $"{_idBase}-lista";

    internal string IdApoio => $"{_idBase}-apoio";

    internal string IdDaOpcao(int indice) => $"{_idBase}-opcao-{indice}";

    internal string? IdDaOpcaoAtiva => _ativa >= 0 && _ativa < _filtradas.Count ? IdDaOpcao(_ativa) : null;

    internal string TextoDe(TValue item) => ItemText?.Invoke(item) ?? item?.ToString() ?? string.Empty;

    internal bool OpcaoDesabilitada(TValue item) => ItemDisabled?.Invoke(item) ?? false;

    internal bool TemValor => Selecionados.Any();

    internal string TextoExibido => string.Join(", ", Selecionados.Select(TextoDe));

    internal IEnumerable<string> ValoresParaEnvio
        => Selecionados.Select(v => Convert.ToString(v, CultureInfo.InvariantCulture) ?? string.Empty);

    /// <summary>
    /// O campo no EditForm, resolvido uma vez por <see cref="OnParametersSet"/>. Cachear pela instancia
    /// da expressao nao adiantava: o <c>@bind-Value</c> gera uma expressao nova a cada render do pai.
    /// </summary>
    private FieldIdentifier? Campo { get; set; }

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
            var proprias = string.Join(' ',
                "rvm-campo",
                Variant switch { RvmTextFieldVariant.Filled => "rvm-preenchido", RvmTextFieldVariant.Standard => "rvm-padrao", _ => "rvm-contorno" },
                Size == RvmSize.Small ? "rvm-pequeno" : "rvm-medio");

            if (TemValor || !string.IsNullOrEmpty(Placeholder) || _aberto) proprias += " rvm-rotulo-fixo";
            if (_aberto) proprias += " rvm-aberto";
            if (TemErro) proprias += " rvm-erro";
            if (Disabled) proprias += " rvm-desabilitado";
            if (Campo is { } campo && EditContext!.IsModified(campo)) proprias += " modified";

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
                        && !string.Equals(a.Key, "style", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(a.Key, "id", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(a => a.Key, a => a.Value);

    internal string ClassesDaOpcao(int indice, bool selecionada, bool desabilitada)
    {
        var classes = "rvm-opcao";
        if (indice == _ativa) classes += " rvm-ativa";
        if (selecionada) classes += " rvm-selecionada";
        if (desabilitada) classes += " rvm-desabilitada";
        return classes;
    }

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
        Filtrar();
    }

    private void AoMudarValidacao(object? sender, ValidationStateChangedEventArgs e) => StateHasChanged();

    private void Filtrar()
    {
        var termo = _termo.Trim();
        var comparador = CultureInfo.InvariantCulture.CompareInfo;
        _filtradas = string.IsNullOrEmpty(termo)
            ? Items.ToList()
            : Items.Where(i => comparador.IndexOf(TextoDe(i), termo, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0).ToList();

        if (_ativa >= _filtradas.Count)
        {
            _ativa = _filtradas.Count - 1;
        }
    }

    private Task AoClicarNoGatilhoAsync()
        => Disabled ? Task.CompletedTask : _aberto ? FecharAsync(devolverFoco: true) : AbrirAsync(-1);

    /// <summary>
    /// Clicar no rotulo faz o mesmo que clicar no campo. Com o campo vazio o rotulo fica POR CIMA do
    /// combobox, e antes ele so dava foco — o clique no meio do campo nao abria a lista (pego no E2E
    /// do seletor de horario).
    /// </summary>
    private async Task AoClicarNoRotuloAsync()
    {
        await FocarGatilhoAsync();
        await AoClicarNoGatilhoAsync();
    }

    private async Task FocarGatilhoAsync()
    {
        if (!Disabled)
        {
            await Tentar(() => _gatilho.FocusAsync());
        }
    }

    private async Task AoTeclarNoGatilhoAsync(KeyboardEventArgs e)
    {
        if (Disabled)
        {
            return;
        }

        if (!_aberto)
        {
            switch (e.Key)
            {
                case "Enter" or " " or "ArrowDown":
                    await AbrirAsync(-1);
                    break;
                case "ArrowUp":
                    await AbrirAsync(-2);
                    break;
                case "Home":
                    await AbrirAsync(0);
                    break;
                case "End":
                    await AbrirAsync(-2);
                    break;
            }

            return;
        }

        await NavegarAsync(e, aceitaEspaco: true);
    }

    private Task AoTeclarNaBuscaAsync(KeyboardEventArgs e) => NavegarAsync(e, aceitaEspaco: false);

    private async Task NavegarAsync(KeyboardEventArgs e, bool aceitaEspaco)
    {
        switch (e.Key)
        {
            case "ArrowDown":
                MoverAtiva(Proxima(_ativa, +1));
                break;
            case "ArrowUp":
                MoverAtiva(Proxima(_ativa < 0 ? _filtradas.Count : _ativa, -1));
                break;
            case "Home" when aceitaEspaco:
                MoverAtiva(Proxima(-1, +1));
                break;
            case "End" when aceitaEspaco:
                MoverAtiva(Proxima(_filtradas.Count, -1));
                break;
            case "Enter":
                await EscolherAsync(_ativa);
                break;
            case " " when aceitaEspaco:
                await EscolherAsync(_ativa);
                break;
            case "Escape":
                await FecharAsync(devolverFoco: true);
                break;
            case "Tab":
                await FecharAsync(devolverFoco: false);
                break;
        }
    }

    private async Task AoBuscarAsync(ChangeEventArgs e)
    {
        _termo = e.Value?.ToString() ?? string.Empty;
        Filtrar();
        _ativa = Proxima(-1, +1);
        await Task.CompletedTask;
    }

    /// <summary>A proxima opcao habilitada na direcao, sem dar a volta — o padrao do select.</summary>
    internal int Proxima(int de, int passo)
    {
        for (var i = de + passo; i >= 0 && i < _filtradas.Count; i += passo)
        {
            if (!OpcaoDesabilitada(_filtradas[i]))
            {
                return i;
            }
        }

        return _ativa;
    }

    private void MoverAtiva(int indice)
    {
        _ativa = indice;
        _rolarAteAtiva = true;
    }

    /// <param name="inicial">Indice a ativar; -1 para a primeira escolhida (ou a primeira), -2 para a ultima.</param>
    private Task AbrirAsync(int inicial)
    {
        _termo = string.Empty;
        Filtrar();
        _aberto = true;
        var escolhida = _filtradas.FindIndex(EstaSelecionado);
        _ativa = inicial switch
        {
            -2 => Proxima(_filtradas.Count, -1),
            -1 when escolhida >= 0 => escolhida,
            -1 => Proxima(-1, +1),
            _ => Proxima(inicial - 1, +1)
        };
        _focarBuscaAoAbrir = Searchable;
        _rolarAteAtiva = true;
        StateHasChanged();
        return Task.CompletedTask;
    }

    internal async Task FecharAsync(bool devolverFoco)
    {
        _aberto = false;
        _termo = string.Empty;
        Filtrar();
        StateHasChanged();
        if (devolverFoco)
        {
            await Tentar(() => _gatilho.FocusAsync());
        }
    }

    internal async Task EscolherAsync(int indice)
    {
        if (indice < 0 || indice >= _filtradas.Count || OpcaoDesabilitada(_filtradas[indice]))
        {
            return;
        }

        _ativa = indice;
        var fechar = await AplicarEscolhaAsync(_filtradas[indice]);
        if (Campo is { } campo)
        {
            EditContext!.NotifyFieldChanged(campo);
        }

        if (fechar)
        {
            await FecharAsync(devolverFoco: true);
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Tentar(async () =>
            {
                _modulo = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/RVM.DesignSystem/rvm-teclado.js");
                await _modulo.InvokeVoidAsync("prenderTeclas", _gatilho, new[] { "ArrowUp", "ArrowDown", "Home", "End", " " });
            });
        }

        if (!_aberto || _modulo is null)
        {
            return;
        }

        if (_focarBuscaAoAbrir)
        {
            _focarBuscaAoAbrir = false;
            await Tentar(async () =>
            {
                await _modulo.InvokeVoidAsync("prenderTeclas", _busca, new[] { "ArrowUp", "ArrowDown", "Enter" });
                await _busca.FocusAsync();
            });
        }

        if (_rolarAteAtiva && IdDaOpcaoAtiva is { } id)
        {
            _rolarAteAtiva = false;
            await Tentar(() => _modulo.InvokeVoidAsync("rolarParaVer", id));
        }
    }

    private static async Task Tentar(Func<ValueTask> acao)
    {
        try
        {
            await acao();
        }
        catch (Exception e) when (e is JSException or JSDisconnectedException or InvalidOperationException or TaskCanceledException)
        {
            // Sem JS (pre-renderizacao, circuito caindo): abre, fecha e escolhe; so foco e rolagem nao andam.
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_contextoAssinado is not null)
        {
            _contextoAssinado.OnValidationStateChanged -= AoMudarValidacao;
        }

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
