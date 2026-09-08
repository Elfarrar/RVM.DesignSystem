using System.Text.RegularExpressions;
using RVM.DesignSystem.Theming;

namespace RVM.DesignSystem.Docs.Documentacao;

/// <summary>Um resultado da busca.</summary>
/// <param name="Titulo">O que casou.</param>
/// <param name="Grupo">De onde veio — "Componentes", "Token", "Fundamentos".</param>
/// <param name="Rota">Para onde ir.</param>
public sealed record ResultadoDeBusca(string Titulo, string Grupo, string Rota);

/// <summary>
/// Busca por nome de componente e de token (<c>RF-27</c>).
/// </summary>
/// <remarks>
/// <b>O indice e construido do que ja existe</b>, e nao escrito a mao: as paginas saem da
/// <see cref="Navegacao"/>, os papeis de cor saem do tema emitido, e as escalas saem do proprio
/// <c>rvm-tokens.css</c> que o navegador carregou.
///
/// <para>
/// Uma lista paralela desatualizaria no primeiro componente novo — e busca que nao acha o que
/// existe e pior que busca nenhuma, porque faz a pessoa concluir que a coisa nao existe. Segue o
/// mesmo principio da tabela de API, que e gerada da doc XML em vez de escrita.
/// </para>
/// </remarks>
public sealed partial class Busca(HttpClient http)
{
    private IReadOnlyList<ResultadoDeBusca>? _indice;

    /// <summary>
    /// Monta o indice na primeira chamada e reaproveita depois.
    /// </summary>
    /// <returns>Tudo que a busca alcanca.</returns>
    public async Task<IReadOnlyList<ResultadoDeBusca>> IndiceAsync()
    {
        if (_indice is not null)
        {
            return _indice;
        }

        var itens = new List<ResultadoDeBusca>();

        foreach (var grupo in Navegacao.Grupos)
        {
            foreach (var item in grupo.Itens)
            {
                itens.Add(new ResultadoDeBusca(item.Texto, grupo.Titulo, item.Rota));

                // O nome no menu e "Button", mas ninguem procura por "Button" — procura por
                // "RvmButton". Os dois entram no indice.
                if (item.Rota.StartsWith("componentes/", StringComparison.Ordinal))
                {
                    itens.Add(new ResultadoDeBusca($"Rvm{item.Texto}", grupo.Titulo, item.Rota));
                }
            }
        }

        // Papeis de cor: saem do TEMA de verdade, pelo mesmo metodo que o navegador consome.
        // Um papel novo na paleta aparece na busca sem ninguem tocar aqui.
        foreach (var token in NomesDeToken(RvmThemes.Rvm.ToCss()))
        {
            itens.Add(new ResultadoDeBusca(token, "Token", "fundamentos/cor"));
        }

        foreach (var token in await EscalasAsync())
        {
            var rota = token.Contains("control", StringComparison.Ordinal)
                ? "fundamentos/densidade"
                : "fundamentos/espacamento";

            itens.Add(new ResultadoDeBusca(token, "Token", rota));
        }

        _indice = itens.DistinctBy(r => r.Titulo, StringComparer.OrdinalIgnoreCase).ToList();
        return _indice;
    }

    /// <summary>
    /// Filtra o indice.
    /// </summary>
    /// <param name="termo">O que a pessoa digitou.</param>
    /// <param name="maximo">Quantos resultados devolver.</param>
    /// <returns>Os que casam, com os que COMECAM pelo termo na frente.</returns>
    public async Task<IReadOnlyList<ResultadoDeBusca>> FiltrarAsync(string? termo, int maximo = 8)
    {
        // Dois caracteres: com um so, "a" devolveria metade do indice e a lista viraria ruido.
        if (string.IsNullOrWhiteSpace(termo) || termo.Trim().Length < 2)
        {
            return [];
        }

        var t = termo.Trim();
        var indice = await IndiceAsync();

        return indice
            .Where(r => r.Titulo.Contains(t, StringComparison.OrdinalIgnoreCase))
            // Quem COMECA com o termo vem antes: digitando "car", `RvmCard` interessa mais que
            // `--rvm-color-scrim`, que so casa no meio.
            .OrderBy(r => r.Titulo.StartsWith(t, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ThenBy(r => r.Titulo.Length)
            .ThenBy(r => r.Titulo, StringComparer.OrdinalIgnoreCase)
            .Take(maximo)
            .ToList();
    }

    private async Task<IEnumerable<string>> EscalasAsync()
    {
        try
        {
            var css = await http.GetStringAsync("_content/RVM.DesignSystem/rvm-tokens.css");
            return NomesDeToken(css).ToList();
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            // Sem o CSS, a busca continua achando componente e cor. Degradar e melhor que
            // derrubar o campo de busca inteiro por causa de uma escala.
            return [];
        }
    }

    private static IEnumerable<string> NomesDeToken(string css)
    {
        foreach (Match m in DeclaracaoDeToken().Matches(css))
        {
            yield return m.Groups[1].Value;
        }
    }

    [GeneratedRegex(@"(--rvm-[a-z0-9-]+)\s*:")]
    private static partial Regex DeclaracaoDeToken();
}
