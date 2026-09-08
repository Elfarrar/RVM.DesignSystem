using System.Globalization;

namespace RVM.DesignSystem.Docs.Documentacao;

/// <summary>Um pedido da listagem de exemplo.</summary>
/// <param name="Numero">Numero do pedido.</param>
/// <param name="Cliente">Nome do cliente.</param>
/// <param name="Emissao">Data de emissao.</param>
/// <param name="Total">Valor total.</param>
/// <param name="Situacao">Situacao corrente.</param>
public sealed record Pedido(
    int Numero,
    string Cliente,
    DateTime Emissao,
    decimal Total,
    string Situacao);

/// <summary>
/// Os ~500 pedidos que a tela de listagem usa.
/// </summary>
/// <remarks>
/// Gerados, e nao escritos: 500 linhas a mao seriam 500 linhas de arquivo para provar a mesma
/// coisa. O que importa e que o volume seja REAL — o criterio de saida da onda 4
/// (`09-roadmap.md`) pede uma listagem de verdade, e um grid que so foi visto com tres linhas
/// nao provou nada sobre ordenacao, paginacao ou rolagem.
///
/// <para>
/// A semente e fixa: a mesma lista em todo carregamento, em toda maquina. Sem isso, o E2E que
/// verifica a ordenacao compararia contra dados diferentes a cada run.
/// </para>
/// </remarks>
public static class Pedidos
{
    private static readonly string[] Clientes =
    [
        "Construtora Alvorada", "Aurora Materiais", "Zeta Engenharia", "Marcenaria Bom Tempo",
        "Ferragens Sao Jorge", "Eletrica Cordeiro", "Hidraulica Vale Verde", "Vidracaria Cristal",
        "Metalurgica Ipiranga", "Tintas Bandeirante", "Madeireira Peroba", "Concreteira Serra",
        "Terraplanagem Norte", "Andaimes Uniao", "Esquadrias Lumiar",
    ];

    private static readonly string[] Situacoes =
    [
        "Aprovado", "Em analise", "Pendente", "Cancelado", "Entregue",
    ];

    private static readonly Lazy<IReadOnlyList<Pedido>> _todos = new(Gerar);

    /// <summary>Todos os pedidos de exemplo.</summary>
    public static IReadOnlyList<Pedido> Todos => _todos.Value;

    /// <summary>As situacoes possiveis, para alimentar o filtro.</summary>
    public static IReadOnlyList<string> TodasAsSituacoes => Situacoes;

    private static IReadOnlyList<Pedido> Gerar()
    {
        // Semente fixa: a mesma lista sempre, em qualquer maquina.
        var aleatorio = new Random(20260908);
        var inicio = new DateTime(2026, 1, 1);

        return Enumerable.Range(1, 487)
            .Select(i => new Pedido(
                Numero: 4000 + i,
                Cliente: Clientes[aleatorio.Next(Clientes.Length)],
                Emissao: inicio.AddDays(aleatorio.Next(250)),
                Total: Math.Round((decimal)(aleatorio.NextDouble() * 24_000) + 80m, 2),
                Situacao: Situacoes[aleatorio.Next(Situacoes.Length)]))
            .ToList();
    }

    /// <summary>Formata um valor em reais.</summary>
    /// <param name="valor">O valor.</param>
    /// <returns>O texto em pt-BR.</returns>
    public static string EmReais(decimal valor) =>
        valor.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"));
}
