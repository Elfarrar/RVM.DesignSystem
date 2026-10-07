using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace RVM.DesignSystem.Components.TextField;

/// <summary>
/// Campo de texto que busca enquanto se digita e oferece as opcoes numa lista (contrato com o RVM.UI, DSGN-017). O valor
/// e o item escolhido; o texto do campo vira o texto dele.
/// </summary>
/// <typeparam name="TValue">Tipo do item.</typeparam>
public partial class RvmAutocomplete<TValue> : IAsyncDisposable
{
    private enum EstadoDaBusca { Parada, Buscando, Pronta, Falhou }

    private string _texto = "";
    private bool _sincronizado;
    private TValue? _valorConhecido;
    private bool _aberto;
    private int _ativo = -1;
    private List<TValue> _resultados = [];
    private EstadoDaBusca _estado;
    private CancellationTokenSource? _busca;
    private bool _rolarAteAtiva;
    private ElementReference _entrada;
    private IJSObjectReference? _modulo;

    [Inject] private IJSRuntime JS { get; set; } = default!;

    [Inject] private IServiceProvider Provedor { get; set; } = default!;

    private TimeProvider Relogio => Provedor.GetService(typeof(TimeProvider)) as TimeProvider ?? TimeProvider.System;

    /// <summary>A busca: recebe o texto digitado e o cancelamento (uma busca nova cancela a anterior).</summary>
    [Parameter] public Func<string, CancellationToken, Task<IEnumerable<TValue>>>? SearchFunc { get; set; }

    /// <summary>Texto de cada item, na lista e no campo depois de escolhido. Sem ele, <c>ToString()</c>.</summary>
    [Parameter] public Func<TValue, string>? ItemText { get; set; }

    /// <summary>Espera, em milissegundos, depois da ultima tecla antes de buscar. Padrao: 300.</summary>
    [Parameter] public int DebounceInterval { get; set; } = 300;

    /// <summary>Letras minimas para buscar. Padrao: 0 (focar o campo ja busca).</summary>
    [Parameter] public int MinCharacters { get; set; }

    /// <summary>Quantas opcoes a lista mostra, no maximo. Padrao: 10.</summary>
    [Parameter] public int MaxItems { get; set; } = 10;

    /// <summary>Apagar todo o texto limpa o valor. Padrao: ligado.</summary>
    [Parameter] public bool ResetValueOnEmptyText { get; set; } = true;

    /// <summary>Mostra o botao de limpar no fim do campo.</summary>
    [Parameter] public bool Clearable { get; set; }

    /// <summary>Nome acessivel do botao de limpar.</summary>
    [Parameter] public string ClearLabel { get; set; } = "Limpar";

    /// <summary>Aviso enquanto a busca roda.</summary>
    [Parameter] public string SearchingText { get; set; } = "Buscando...";

    /// <summary>Aviso quando a busca nao acha nada.</summary>
    [Parameter] public string NoResultsText { get; set; } = "Nenhum resultado para essa busca.";

    /// <summary>Aviso quando a busca falha.</summary>
    [Parameter] public string ErrorSearchText { get; set; } = "Nao deu para buscar agora. Tente de novo em alguns minutos.";

    /// <summary>Classe CSS extra na raiz do campo.</summary>
    [Parameter] public string? Class { get; set; }

    /// <inheritdoc />
    protected override string? ClasseDoCampo => Class;

    private string IdDaLista => $"{IdDoControle}-lista";

    private string IdDaOpcao(int indice) => $"{IdDoControle}-opcao-{indice}";

    private bool ListaVisivel => _aberto && _resultados.Count > 0 && _estado == EstadoDaBusca.Pronta;

    private bool TemValor => !EqualityComparer<TValue>.Default.Equals(Value, default);

    private string? MensagemDaBusca => _estado switch
    {
        EstadoDaBusca.Buscando => SearchingText,
        EstadoDaBusca.Falhou => ErrorSearchText,
        EstadoDaBusca.Pronta when _resultados.Count == 0 => NoResultsText,
        _ => null,
    };

    private string TextoDe(TValue? item) => item is null ? "" : ItemText?.Invoke(item) ?? item.ToString() ?? "";

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // O valor mudou por fora (ou e o primeiro render): o texto do campo passa a ser o dele.
        if (!_sincronizado || !EqualityComparer<TValue>.Default.Equals(Value, _valorConhecido))
        {
            _sincronizado = true;
            _valorConhecido = Value;
            _texto = TextoDe(Value);
        }
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        // O valor vem da escolha na lista, nunca do texto: texto solto nao vira valor.
        result = default!;
        if (string.IsNullOrEmpty(value))
        {
            validationErrorMessage = null;
            return true;
        }

        validationErrorMessage = "Escolha uma das opcoes da lista.";
        return false;
    }

    private async Task AoDigitarAsync(ChangeEventArgs e)
    {
        _texto = e.Value?.ToString() ?? "";
        if (_texto.Length == 0 && ResetValueOnEmptyText && TemValor)
        {
            DefinirValor(default);
        }

        await BuscarAsync(_texto);
    }

    private async Task AoFocarAsync()
    {
        if (!_aberto && _texto.Length >= MinCharacters)
        {
            await BuscarAsync(_texto);
        }
    }

    // Saiu do campo sem escolher: fecha e devolve o texto do valor. O clique numa opcao nao tira o foco
    // (mousedown com preventDefault), entao nao passa por aqui.
    private void AoSair() => Fechar(devolverTexto: true);

    private async Task AoTeclarAsync(KeyboardEventArgs e)
    {
        switch (e.Key)
        {
            case "ArrowDown":
                if (!ListaVisivel)
                {
                    if (!_aberto) await BuscarAsync(_texto);
                    return;
                }
                Ativar(_ativo + 1 >= _resultados.Count ? 0 : _ativo + 1);
                break;
            case "ArrowUp":
                if (ListaVisivel) Ativar(_ativo <= 0 ? _resultados.Count - 1 : _ativo - 1);
                break;
            case "Enter":
                if (ListaVisivel && _ativo >= 0) await EscolherAsync(_ativo);
                break;
            case "Escape":
                Fechar(devolverTexto: true);
                break;
            case "Tab":
                Fechar(devolverTexto: true);
                break;
        }
    }

    private void Ativar(int indice)
    {
        _ativo = indice;
        _rolarAteAtiva = true;
    }

    private async Task BuscarAsync(string termo)
    {
        _busca?.Cancel();
        _busca?.Dispose();
        _busca = null;

        if (termo.Length < MinCharacters)
        {
            Fechar(devolverTexto: false);
            return;
        }

        var busca = new CancellationTokenSource();
        var token = busca.Token;
        _busca = busca;
        try
        {
            if (DebounceInterval > 0)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(DebounceInterval), Relogio, token);
            }

            _aberto = true;
            _estado = EstadoDaBusca.Buscando;
            _ativo = -1;
            StateHasChanged();

            var achados = SearchFunc is null ? [] : await SearchFunc(termo, token);
            token.ThrowIfCancellationRequested();
            _resultados = achados?.Take(Math.Max(0, MaxItems)).ToList() ?? [];
            _estado = EstadoDaBusca.Pronta;
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // Uma busca mais nova (ou o fechamento) tomou o lugar desta: o resultado dela nao interessa.
            return;
        }
#pragma warning disable CA1031 // A busca e do consumidor: qualquer falha dela vira o aviso, nao derruba o circuito.
        catch (Exception)
#pragma warning restore CA1031
        {
            if (token.IsCancellationRequested) return;
            _resultados = [];
            _estado = EstadoDaBusca.Falhou;
        }

        StateHasChanged();
    }

    private Task EscolherAsync(int indice)
    {
        if (indice < 0 || indice >= _resultados.Count) return Task.CompletedTask;
        var item = _resultados[indice];
        DefinirValor(item);
        _texto = TextoDe(item);
        Fechar(devolverTexto: false);
        return Task.CompletedTask;
    }

    private async Task LimparAsync()
    {
        DefinirValor(default);
        _texto = "";
        Fechar(devolverTexto: false);
        await Tentar(() => _entrada.FocusAsync());
    }

    private void DefinirValor(TValue? valor)
    {
        _valorConhecido = valor;
        CurrentValue = valor;
    }

    private void Fechar(bool devolverTexto)
    {
        _busca?.Cancel();
        _busca?.Dispose();
        _busca = null;
        _aberto = false;
        _ativo = -1;
        _resultados = [];
        _estado = EstadoDaBusca.Parada;
        if (devolverTexto)
        {
            _texto = TextoDe(Value);
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Setas e Enter nao movem o cursor nem enviam o formulario: no campo, elas andam e escolhem na lista.
            await Tentar(async () =>
            {
                _modulo = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/RVM.DesignSystem/rvm-teclado.js");
                await _modulo.InvokeVoidAsync("prenderTeclas", _entrada, new[] { "ArrowUp", "ArrowDown", "Enter" });
            });
        }

        if (_rolarAteAtiva && _modulo is not null && ListaVisivel && _ativo >= 0)
        {
            _rolarAteAtiva = false;
            await Tentar(() => _modulo.InvokeVoidAsync("rolarParaVer", IdDaOpcao(_ativo)));
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
            // Sem JS (pre-renderizacao, circuito caindo, bUnit): busca e escolha funcionam; so foco e rolagem nao andam.
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _busca?.Cancel();
            _busca?.Dispose();
            _busca = null;
        }

        base.Dispose(disposing);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        ((IDisposable)this).Dispose();
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
