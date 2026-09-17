using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace RVM.DesignSystem.Components.Dialog;

/// <summary>
/// O lado .NET do <c>rvm-sobreposicao.js</c>, compartilhado por dialogo e gaveta temporaria: prende o
/// foco ao abrir e devolve ao fechar. Toda falha de JS e engolida — sem JS, a sobreposicao continua
/// abrindo e fechando; so o foco nao fica preso.
/// </summary>
internal sealed class Sobreposicao(IJSRuntime js) : IAsyncDisposable
{
    private IJSObjectReference? _modulo;
    private IJSObjectReference? _aberta;

    public async Task AbrirAsync(ElementReference caixa)
    {
        await FecharAsync();
        await Tentar(async () =>
        {
            _modulo ??= await js.InvokeAsync<IJSObjectReference>("import", "./_content/RVM.DesignSystem/rvm-sobreposicao.js");
            _aberta = await _modulo.InvokeAsync<IJSObjectReference>("abrir", caixa);
        });
    }

    public async Task FecharAsync()
    {
        if (_aberta is null)
        {
            return;
        }

        var aberta = _aberta;
        _aberta = null;
        await Tentar(async () =>
        {
            await aberta.InvokeVoidAsync("fechar");
            await aberta.DisposeAsync();
        });
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
    }
}
