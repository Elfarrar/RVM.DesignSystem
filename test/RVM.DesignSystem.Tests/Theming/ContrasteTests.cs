using System.Globalization;
using RVM.DesignSystem.Theming;

namespace RVM.DesignSystem.Tests.Theming;

/// <summary>
/// O portao que torna a acessibilidade real (`06` § Verificacao de contraste).
/// </summary>
/// <remarks>
/// Contraste deixa de ser algo que alguem confere na revisao visual e passa a ser algo que
/// <b>nao entra no repositorio errado</b>. Uma paleta nova que reprove derruba o CI antes de
/// qualquer humano olhar.
/// </remarks>
public class ContrasteTests
{
    public static TheoryData<string> TodosOsTemas()
    {
        var data = new TheoryData<string>();
        foreach (var tema in RvmThemes.All)
        {
            data.Add(tema.Name);
        }

        return data;
    }

    private static RvmTheme Tema(string nome) => RvmThemes.All.Single(t => t.Name == nome);

    [Theory]
    [MemberData(nameof(TodosOsTemas))]
    public void Todo_par_semantico_do_modo_claro_atende_WCAG_AA(string nome)
    {
        VerificarPaleta(Tema(nome).Light, $"{nome}/claro");
    }

    [Theory]
    [MemberData(nameof(TodosOsTemas))]
    public void Todo_par_semantico_do_modo_escuro_atende_WCAG_AA(string nome)
    {
        VerificarPaleta(Tema(nome).Dark, $"{nome}/escuro");
    }

    private static void VerificarPaleta(RvmPalette paleta, string rotulo)
    {
        var reprovados = new List<string>();

        foreach (var (par, frente, fundo, minimo) in paleta.PairsToVerify())
        {
            var razao = RvmColor.Contrast(RvmColor.Parse(frente), RvmColor.Parse(fundo));
            if (razao < minimo)
            {
                reprovados.Add(string.Create(
                    CultureInfo.InvariantCulture,
                    $"  {rotulo} · {par}: {frente} sobre {fundo} = {razao:F2}:1, minimo {minimo:F1}:1"));
            }
        }

        Assert.True(
            reprovados.Count == 0,
            $"Pares abaixo do minimo AA:\n{string.Join("\n", reprovados)}");
    }

    [Fact]
    public void A_lista_de_pares_cobre_todo_papel_de_fundo_da_paleta()
    {
        // Guarda contra o defeito que o `04` descreve: papel de fundo acrescentado sem o On*
        // correspondente vira par que ninguem mede. Se alguem incluir um papel novo em
        // RvmPalette e esquecer de registrar em PairsToVerify, este teste cai.
        var paleta = RvmThemes.Rvm.Light;
        var pares = paleta.PairsToVerify().Select(p => p.Nome).ToList();

        string[] fundosQueDevemSerMedidos =
        [
            "primary", "primary-container", "secondary",
            "success", "warning", "danger", "info",
            "surface", "surface-raised", "surface-sunken", "background",
            "border-strong", "focus-ring", "disabled",
        ];

        var naoMedidos = fundosQueDevemSerMedidos
            .Where(f => !pares.Any(p => p.StartsWith(f + "/", StringComparison.Ordinal)))
            .ToArray();

        Assert.True(
            naoMedidos.Length == 0,
            $"Papeis sem par medido: {string.Join(", ", naoMedidos)}");
    }

    [Fact]
    public void Uma_paleta_ruim_escrita_a_mao_REPROVA()
    {
        // O teste do teste. Sem isto, um bug em PairsToVerify (um yield esquecido, um par
        // trocado) faria o portao passar a aprovar tudo em silencio — e o pior defeito de um
        // portao e ele parecer verde sem estar medindo nada.
        var cinzaSobreCinza = RvmThemes.Rvm.Light with { OnSurface = "#B9B9C0" };

        var reprovou = cinzaSobreCinza.PairsToVerify()
            .Any(p => RvmColor.Contrast(RvmColor.Parse(p.Frente), RvmColor.Parse(p.Fundo)) < p.Minimo);

        Assert.True(reprovou, "O portao aprovou uma paleta com texto cinza sobre fundo claro.");
    }
}
