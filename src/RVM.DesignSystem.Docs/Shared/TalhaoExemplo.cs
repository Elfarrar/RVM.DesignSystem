namespace RVM.DesignSystem.Docs.Shared;

/// <summary>Dados ficticios das paginas de tabela e grade.</summary>
public sealed record TalhaoExemplo(int Id, string Nome, string Responsavel, string Email, string Cultura, DateOnly Plantio, decimal Hectares, string Situacao)
{
    public static readonly TalhaoExemplo[] Lista =
    [
        new(45, "Talhao Norte", "Leticia Kulas", "leticia@fazenda.com.br", "Soja", new DateOnly(2026, 10, 12), 120.5m, "Plantado"),
        new(12, "Baixada", "Dallas Adams", "dallas@fazenda.com.br", "Milho", new DateOnly(2026, 9, 28), 64m, "Em preparo"),
        new(83, "Sede", "Theodore Stiedemann", "theodore@fazenda.com.br", "Cafe", new DateOnly(2025, 11, 3), 18.2m, "Colhido"),
        new(7, "Cerrado", "Hilda Rath", "hilda@fazenda.com.br", "Soja", new DateOnly(2026, 10, 20), 210m, "Atrasado"),
        new(98, "Varzea", "Bertha Heller", "bertha@fazenda.com.br", "Arroz", new DateOnly(2026, 11, 2), 45.8m, "Plantado"),
        new(31, "Capao", "Veronica Raynor", "veronica@fazenda.com.br", "Feijao", new DateOnly(2026, 8, 15), 32m, "Colhido"),
        new(56, "Encosta", "Krista Gerlach", "krista@fazenda.com.br", "Milho", new DateOnly(2026, 9, 30), 88.4m, "Em preparo"),
        new(21, "Represa", "Curtis Schinner", "curtis@fazenda.com.br", "Soja", new DateOnly(2026, 10, 5), 150m, "Plantado"),
        new(64, "Pasto Velho", "Doyle Rempel", "doyle@fazenda.com.br", "Sorgo", new DateOnly(2026, 9, 10), 72.6m, "Atrasado"),
        new(3, "Divisa", "Sonia Dietrich", "sonia@fazenda.com.br", "Algodao", new DateOnly(2026, 12, 1), 300m, "Em preparo"),
        new(77, "Mangueiro", "Lila Cummerata", "lila@fazenda.com.br", "Trigo", new DateOnly(2026, 5, 20), 40m, "Colhido"),
        new(15, "Corrego", "Ethel Bayer", "ethel@fazenda.com.br", "Soja", new DateOnly(2026, 10, 18), 95.3m, "Plantado"),
        new(90, "Chapada", "Morton Kris", "morton@fazenda.com.br", "Milho", new DateOnly(2026, 9, 25), 180m, "Plantado")
    ];
}
