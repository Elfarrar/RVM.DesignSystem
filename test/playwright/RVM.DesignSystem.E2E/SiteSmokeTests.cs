using Deque.AxeCore.Playwright;
using Microsoft.Playwright;

namespace RVM.DesignSystem.E2E;

/// <summary>
/// Smoke + auditoria de acessibilidade sobre o site JA PUBLICADO.
/// </summary>
/// <remarks>
/// Alvo vem de <c>E2E_BASE_URL</c>; sem a variavel, os testes sao pulados —
/// e por isso que este projeto nao pode rodar no ci.yml de build, e por isso
/// que ele mora sob test/playwright/ (o reusable do RVM.Actions pula esse caminho).
/// </remarks>
public class SiteSmokeTests : IAsyncLifetime
{
    private static readonly string? BaseUrl = Environment.GetEnvironmentVariable("E2E_BASE_URL");

    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public async Task InitializeAsync()
    {
        if (BaseUrl is null) return;
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync();
    }

    public async Task DisposeAsync()
    {
        if (_browser is not null) await _browser.CloseAsync();
        _playwright?.Dispose();
    }

    [SkippableFact]
    public async Task Home_responde_e_renderiza_o_titulo()
    {
        Skip.If(BaseUrl is null, "E2E_BASE_URL nao definida — rodando fora do pipeline de E2E.");

        var page = await _browser!.NewPageAsync();
        var response = await page.GotoAsync(BaseUrl!);

        Assert.NotNull(response);
        Assert.True(response!.Ok, $"HTTP {response.Status} em {BaseUrl}");
        await Assertions.Expect(page.Locator("h1")).ToContainTextAsync("RVM Design System");
    }

    [SkippableFact]
    public async Task Home_passa_na_auditoria_axe_sem_violacao_seria()
    {
        Skip.If(BaseUrl is null, "E2E_BASE_URL nao definida — rodando fora do pipeline de E2E.");

        var page = await _browser!.NewPageAsync();
        await page.GotoAsync(BaseUrl!);
        await Assertions.Expect(page.Locator("h1")).ToBeVisibleAsync();

        var resultado = await page.RunAxe();

        // Portao 3 do CLAUDE.md: violacao seria reprova.
        var serias = resultado.Violations
            .Where(v => v.Impact is "serious" or "critical")
            .Select(v => $"{v.Id} ({v.Impact}): {v.Help}")
            .ToArray();

        Assert.True(serias.Length == 0, string.Join("\n", serias));
    }
}
