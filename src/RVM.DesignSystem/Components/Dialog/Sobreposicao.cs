using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace RVM.DesignSystem.Components.Dialog;

/// <summary>
/// O lado .NET do <c>rvm-sobreposicao.js</c>, compartilhado por dialogo e gaveta temporaria: prende o
/// foco ao abrir e devolve ao fechar. Toda falha de JS e engolida — sem JS, a sobreposicao continua
/// abrindo e fechando; so o foco nao fica preso.
/// </summary>
/// <remarks>
/// Abrir e fechar sao SERIALIZADOS e seguem o estado DESEJADO, nao a ordem das chamadas. O
/// <c>OnAfterRenderAsync</c> nao espera o anterior terminar: abrir e fechar rapido fazia o fechar rodar
/// antes de o abrir voltar do JS, achar nada para fechar, e o abrir concluir depois — rolagem da pagina
/// travada para sempre. Achado do review independente da onda 3.
/// </remarks>
internal sealed class Sobreposicao(IJSRuntime js) : IAsyncDisposable
{
    private readonly SemaphoreSlim _vez = new(1, 1);
    private IJSObjectReference? _modulo;
    private IJSObjectReference? _aberta;
    private bool _desejada;

    public Task AbrirAsync(ElementReference caixa)
    {
        _desejada = true;
        return AplicarAsync(caixa);
    }

    public Task FecharAsync()
    {
        _desejada = false;
        return AplicarAsync(null);
    }

    private async Task AplicarAsync(ElementReference? caixa)
    {
        await _vez.WaitAsync();
        try
        {
            // Quem chega depois ve o estado final: se enquanto esperava a vez alguem pediu o contrario,
            // faz o contrario.
            if (_desejada && _aberta is null && caixa is { } alvo)
            {
                await Tentar(async () =>
                {
                    _modulo ??= await js.InvokeAsync<IJSObjectReference>("import", "./_content/RVM.DesignSystem/rvm-sobreposicao.js");
                    _aberta = await _modulo.InvokeAsync<IJSObjectReference>("abrir", alvo);
                });
            }

            if (!_desejada && _aberta is not null)
            {
                var aberta = _aberta;
                _aberta = null;
                await Tentar(async () =>
                {
                    await aberta.InvokeVoidAsync("fechar");
                    await aberta.DisposeAsync();
                });
            }
        }
        finally
        {
            _vez.Release();
        }
    }

    private static async Task Tentar(Func<Task> acao)
    {
        try
        {
            await acao();
        }
        catch (Exception e) when (e is JSException or JSDisconnectedException or InvalidOperationException or TaskCanceledException)
        {
            // Sem JS (pre-renderizacao, circuito caindo): segue sem prender o foco.
        }
    }

    public async ValueTask DisposeAsync()
    {
        await FecharAsync();
        if (_modulo is not null)
        {
            await Tentar(async () => await _modulo.DisposeAsync());
        }

        _vez.Dispose();
    }
}
