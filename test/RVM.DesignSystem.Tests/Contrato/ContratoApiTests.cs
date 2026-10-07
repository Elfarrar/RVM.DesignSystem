using System.Text.Json;
using RVM.DesignSystem.Components.Button;

namespace RVM.DesignSystem.Tests.Contrato;

/// <summary>
/// Contrato de API com o RVM.UI (DSGN-017): <c>design/contrato-api.json</c> e a uniao das duas APIs, copiada do
/// repo do RVM.UI. O DS tem que cumprir tudo, menos o que esta em <c>design/contrato-api.pendentes.json</c> — e essa
/// lista so encolhe. A 2.0.0 sai com ela vazia.
/// Tirar da lista o que ja foi feito: <c>RVM_ATUALIZAR_PENDENTES=1 dotnet test --filter FullyQualifiedName~ContratoApiTests</c>
/// (so remove; item novo na lista e falha, nunca e gravado).
/// ⚠️ Alcance: nome e tipo de <c>[Parameter]</c> e NOMES de membro de enum — igual ao teste do RVM.UI.
/// </summary>
public class ContratoApiTests
{
    private static readonly string PastaDesign = Path.Combine(RaizDoRepositorio.Caminho, "design");
    private static readonly string CaminhoPendentes = Path.Combine(PastaDesign, "contrato-api.pendentes.json");
    private static readonly ApiPublica Atual = ApiPublica.Ler(typeof(RvmButton).Assembly);
    private static readonly ApiPublica Contrato = ApiPublica.DeJson(File.ReadAllText(Path.Combine(PastaDesign, "contrato-api.json")));

    [Fact]
    public void DS_cumpre_o_contrato_menos_os_pendentes()
    {
        var naoCumpre = NaoCumpre();
        var pendentes = File.Exists(CaminhoPendentes)
            ? JsonSerializer.Deserialize<SortedSet<string>>(File.ReadAllText(CaminhoPendentes))!
            : null;

        if (Environment.GetEnvironmentVariable("RVM_ATUALIZAR_PENDENTES") == "1")
        {
            // Primeira geracao grava tudo; depois, so remove o que ja foi cumprido.
            var nova = pendentes is null ? naoCumpre : new SortedSet<string>(pendentes.Where(naoCumpre.Contains), StringComparer.Ordinal);
            File.WriteAllText(CaminhoPendentes, JsonSerializer.Serialize(nova, new JsonSerializerOptions { WriteIndented = true, NewLine = "\n" }) + "\n");
            pendentes = nova;
        }

        Assert.True(pendentes is not null, "Falta design/contrato-api.pendentes.json.");
        var falhas = naoCumpre.Where(p => !pendentes.Contains(p)).Select(p => $"{p} esta no contrato e o DS nao cumpre.")
            .Concat(pendentes.Where(p => !naoCumpre.Contains(p)).Select(p => $"{p} ja cumpre o contrato: tire da lista de pendentes."))
            .ToList();
        Assert.True(falhas.Count == 0, string.Join("\n", falhas));
    }

    [Fact]
    public void Toda_API_publica_do_DS_esta_no_contrato()
    {
        var fora = new List<string>();
        foreach (var (componente, parametros) in Atual.Componentes)
        {
            if (!Contrato.Componentes.TryGetValue(componente, out var doContrato))
            {
                fora.Add(componente);
                continue;
            }

            fora.AddRange(parametros.Keys.Where(p => !doContrato.ContainsKey(p)).Select(p => $"{componente}.{p}"));
        }

        foreach (var (nome, membros) in Atual.Enums)
        {
            var doContrato = Contrato.Enums.GetValueOrDefault(nome) ?? [];
            fora.AddRange(membros.Where(m => !doContrato.Contains(m)).Select(m => $"{nome}.{m}"));
        }

        Assert.True(fora.Count == 0, "Fora do contrato (o RVM.UI precisa gerar o contrato de novo):\n" + string.Join("\n", fora));
    }

    /// <summary>Componente inteiro, <c>Componente.Parametro</c> e <c>Enum.Membro</c> que o DS ainda nao tem.</summary>
    private static SortedSet<string> NaoCumpre()
    {
        var falta = new SortedSet<string>(StringComparer.Ordinal);
        foreach (var (componente, parametros) in Contrato.Componentes)
        {
            if (!Atual.Componentes.TryGetValue(componente, out var atuais))
            {
                falta.Add(componente);
                continue;
            }

            foreach (var (parametro, tipo) in parametros)
            {
                if (!atuais.TryGetValue(parametro, out var tipoAtual) || tipoAtual != tipo) falta.Add($"{componente}.{parametro}");
            }
        }

        foreach (var (nome, membros) in Contrato.Enums)
        {
            var atuais = Atual.Enums.GetValueOrDefault(nome) ?? [];
            foreach (var membro in membros.Where(m => !atuais.Contains(m))) falta.Add($"{nome}.{membro}");
        }

        return falta;
    }
}
