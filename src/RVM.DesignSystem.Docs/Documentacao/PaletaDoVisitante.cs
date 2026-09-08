using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;
using RVM.DesignSystem.Theming;

namespace RVM.DesignSystem.Docs.Documentacao;

/// <summary>
/// A paleta que o visitante montou em <c>/fundamentos/paleta</c>.
/// </summary>
/// <remarks>
/// Guarda as <b>cores escolhidas</b>, nunca o tema derivado. Sao oito campos contra 27 papeis x
/// 2 modos — mas o motivo nao e tamanho: derivar na leitura garante que uma paleta guardada
/// ontem passe pelas regras de contraste de <b>hoje</b>. Um tema serializado seria uma copia
/// congelada, e uma correcao no <c>FromSeed</c> nunca a alcancaria.
/// </remarks>
public sealed record PaletaSalva
{
    /// <summary>Nome do tema, exibido no seletor do site.</summary>
    public string Nome { get; init; } = "MeuProduto";

    /// <summary>Cor principal, em hexadecimal.</summary>
    public string Primaria { get; init; } = "#2A6E49";

    /// <summary>Cor de apoio, em hexadecimal.</summary>
    public string Secundaria { get; init; } = "#B07D2A";

    /// <summary>Se as cores de estado foram escolhidas em vez de derivadas da convencao.</summary>
    public bool Estados { get; init; }

    /// <summary>Cor de sucesso, usada so quando <see cref="Estados"/>.</summary>
    public string Sucesso { get; init; } = "#2E7D32";

    /// <summary>Cor de aviso, usada so quando <see cref="Estados"/>.</summary>
    public string Aviso { get; init; } = "#F9A825";

    /// <summary>Cor de erro, usada so quando <see cref="Estados"/>.</summary>
    public string Erro { get; init; } = "#C62828";

    /// <summary>Cor de informacao, usada so quando <see cref="Estados"/>.</summary>
    public string Info { get; init; } = "#0277BD";

    /// <summary>Deriva o tema completo a partir das cores escolhidas.</summary>
    public RvmTheme ParaTema() => RvmTheme.FromSeed(
        string.IsNullOrWhiteSpace(Nome) ? "MeuProduto" : Nome,
        new RvmSeed
        {
            Primary = Primaria,
            Secondary = Secundaria,
            Success = Estados ? Sucesso : null,
            Warning = Estados ? Aviso : null,
            Danger = Estados ? Erro : null,
            Info = Estados ? Info : null,
        });
}

/// <summary>
/// Serializacao por codigo gerado.
/// </summary>
/// <remarks>
/// Blazor WASM publica com <c>PublishTrimmed</c> em Release. O serializador por reflexao
/// funciona no <c>dotnet run</c> e falha no site publicado — a pior ordem possivel para
/// descobrir um defeito.
/// </remarks>
[JsonSerializable(typeof(PaletaSalva))]
internal sealed partial class PaletaJson : JsonSerializerContext;

/// <summary>
/// Le e guarda a paleta do visitante no navegador dele, e a aplica ao site.
/// </summary>
/// <remarks>
/// ⚠️ <b>Isto e o site de documentacao, nao a biblioteca.</b> "Tema por usuario final em runtime"
/// (white-label por tenant) continua fora do escopo da v1 (`09` § Fora da v1). O que existe aqui
/// e uma preferencia de uma ferramenta de documentacao, guardada no <c>localStorage</c> de quem
/// visita — nao ha servidor, nao ha conta, nao ha tenant.
/// </remarks>
public sealed class PaletaDoVisitante(IJSRuntime js, IRvmThemeService tema)
{
    private const string Chave = "rvm-docs-paleta";

    private bool _lida;

    /// <summary>A paleta guardada, ou nulo se o visitante nao montou nenhuma.</summary>
    public PaletaSalva? Atual { get; private set; }

    /// <summary>
    /// Le a paleta do navegador e a aplica. Idempotente: le o <c>localStorage</c> uma vez so.
    /// </summary>
    /// <remarks>
    /// Chamar de <c>OnAfterRenderAsync</c>, nunca antes — em pre-render nao ha JS. A casca e a
    /// pagina de paleta chamam as duas, e a primeira que chegar faz a leitura.
    /// </remarks>
    public async Task<PaletaSalva?> CarregarAsync()
    {
        if (_lida)
        {
            return Atual;
        }

        _lida = true;

        try
        {
            var json = await js.InvokeAsync<string?>("localStorage.getItem", Chave);

            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            var salva = JsonSerializer.Deserialize(json, PaletaJson.Default.PaletaSalva);

            if (salva is not null)
            {
                // ParaTema() antes de guardar: uma cor invalida no armazenamento estoura AQUI,
                // dentro do catch, em vez de derrubar toda pagina do site depois.
                var derivado = salva.ParaTema();
                Atual = salva;
                await tema.SetThemeAsync(derivado);
            }
        }
        catch (Exception e) when (e is JSException or InvalidOperationException or TaskCanceledException
                                    or JsonException or ArgumentException or FormatException)
        {
            // Sem JS, com JSON corrompido ou com uma cor invalida guardada: seguir com o tema
            // padrao e o comportamento certo. Uma preferencia de documentacao ilegivel nao pode
            // impedir a documentacao de abrir.
            Atual = null;
            await ApagarAsync();
        }

        return Atual;
    }

    /// <summary>Guarda a paleta no navegador e a aplica ao site.</summary>
    public async Task SalvarAsync(PaletaSalva paleta)
    {
        ArgumentNullException.ThrowIfNull(paleta);

        Atual = paleta;
        await tema.SetThemeAsync(paleta.ParaTema());

        try
        {
            await js.InvokeVoidAsync(
                "localStorage.setItem", Chave, JsonSerializer.Serialize(paleta, PaletaJson.Default.PaletaSalva));
        }
        catch (Exception e) when (e is JSException or InvalidOperationException or TaskCanceledException)
        {
            // Mesma escolha do RvmThemeService: o que se perde e a PERSISTENCIA, e a tela ja
            // reagiu. Deixar propagar quebraria a troca de cor por um detalhe de ciclo de vida.
        }
    }

    /// <summary>Esquece a paleta guardada, <b>sem</b> mexer no tema aplicado.</summary>
    /// <remarks>
    /// Separado de <see cref="RestaurarAsync"/> de proposito: escolher "ERPAgro" no seletor da
    /// topbar precisa apagar a paleta guardada — senao o proximo F5 a traria de volta por cima
    /// da escolha — mas nao pode forcar o tema RVM.
    /// </remarks>
    public async Task EsquecerAsync()
    {
        Atual = null;
        await ApagarAsync();
    }

    /// <summary>Esquece a paleta guardada e devolve o site ao tema RVM.</summary>
    public async Task RestaurarAsync()
    {
        await EsquecerAsync();
        await tema.SetThemeAsync(RvmThemes.Rvm);
    }

    private async Task ApagarAsync()
    {
        try
        {
            await js.InvokeVoidAsync("localStorage.removeItem", Chave);
        }
        catch (Exception e) when (e is JSException or InvalidOperationException or TaskCanceledException)
        {
        }
    }
}
