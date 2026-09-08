using RVM.DesignSystem.Components;
using RVM.DesignSystem.Theming;

namespace RVM.DesignSystem.Tests.Theming;

/// <summary>
/// O portao da unica cor da biblioteca que NAO sai da paleta.
/// </summary>
/// <remarks>
/// O fundo do avatar e derivado do nome da pessoa, em tempo de execucao. Isso o coloca fora do
/// alcance dos dois portoes que existem: o <see cref="ContrasteTests"/> percorre os papeis
/// semanticos, e o <see cref="CssDeComponenteTests"/> varre arquivos <c>.razor.css</c> — e esta
/// cor nao esta em nenhum dos dois, ela nasce num <c>style</c> inline.
///
/// <para>
/// Sem este arquivo, seria a unica cor da biblioteca capaz de reprovar AA sem nada reclamar. E
/// nao seria hipotetico: um hash que caisse numa matiz amarela com a mesma luminosidade daria
/// exatamente isso, e so apareceria no dia em que entrasse na base alguem chamado "Wesley".
/// </para>
/// </remarks>
public class AvatarCorTests
{
    /// <summary>Nomes reais e casos de borda, cobrindo a volta inteira do circulo de matizes.</summary>
    public static TheoryData<string> Nomes()
    {
        var dados = new TheoryData<string>();

        foreach (var nome in new[]
        {
            "Rafael Venerosi Morici", "Ana", "Jose da Silva", "Wesley", "Zoe Xu",
            "MARIA APARECIDA DE SOUZA", "  espacos  ", "X", "Ana-Beatriz O'Neil",
            "Ff", "Yy", "0000", "Empresa LTDA", "Joao", "Agata", "Ze",
        })
        {
            dados.Add(nome);
        }

        return dados;
    }

    [Theory]
    [MemberData(nameof(Nomes))]
    public void A_cor_derivada_do_nome_atende_AA(string nome)
    {
        var (fundo, texto) = RvmAvatarCor.DoNome(nome);

        var razao = RvmColor.Contrast(RvmColor.Parse(texto), RvmColor.Parse(fundo));

        Assert.True(
            razao >= RvmContrast.NormalText,
            $"O avatar de \"{nome}\" da {razao:0.00}:1 ({texto} sobre {fundo}), abaixo dos "
            + $"{RvmContrast.NormalText}:1 exigidos.");
    }

    [Fact]
    public void NENHUMA_matiz_possivel_reprova()
    {
        // O teste acima cobre nomes; este cobre o ESPACO INTEIRO de saida. Sao 360 matizes
        // possiveis, e um hash qualquer chega em todas elas — entao todas precisam passar,
        // e nao apenas as que os nomes do exemplo calharam de produzir.
        var reprovadas = new List<string>();

        for (var matiz = 0; matiz < 360; matiz++)
        {
            var fundo = RvmColor.FromOklch(0.55, 0.11, matiz);
            var texto = RvmContrast.Ensure(
                RvmContrast.BestForegroundOn(fundo), fundo, RvmContrast.NormalText);

            var razao = RvmColor.Contrast(texto, fundo);

            if (razao < RvmContrast.NormalText)
            {
                reprovadas.Add($"  matiz {matiz}: {razao:0.00}:1");
            }
        }

        Assert.True(reprovadas.Count == 0,
            "Matizes abaixo de AA:\n" + string.Join("\n", reprovadas));
    }

    [Fact]
    public void O_mesmo_nome_da_SEMPRE_a_mesma_cor()
    {
        // Nao e capricho: desde o .NET Core, string.GetHashCode e aleatorizado por processo.
        // Usar ele aqui faria o avatar de uma pessoa mudar de cor a cada reinicio do servidor —
        // e a cor e como ela se acha numa lista.
        var a = RvmAvatarCor.DoNome("Rafael Venerosi Morici");
        var b = RvmAvatarCor.DoNome("Rafael Venerosi Morici");

        Assert.Equal(a, b);

        // O valor esta cravado de proposito: se a derivacao mudar, este teste cai e obriga a
        // decisao a ser consciente — todo avatar do ecossistema muda de cor junto.
        Assert.Equal("#8C6D08", a.Fundo);
    }

    [Fact]
    public void Nomes_diferentes_tendem_a_cores_diferentes()
    {
        var cores = new[] { "Ana Silva", "Bruno Costa", "Carla Dias", "Diego Reis", "Elza Nunes" }
            .Select(n => RvmAvatarCor.DoNome(n).Fundo)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(5, cores.Count);
    }

    [Theory]
    [InlineData("Rafael Venerosi Morici", "RM")]
    [InlineData("Maria Aparecida Silva", "MS")]
    [InlineData("Ana", "A")]
    [InlineData("  joao   pedro  ", "JP")]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("123", "")]
    public void As_iniciais_pegam_a_primeira_e_a_ultima_palavra(string? nome, string esperado)
    {
        // Primeira e ULTIMA, e nao as duas primeiras: "Maria Aparecida Silva" e conhecida como
        // "MS", nao como "MA".
        Assert.Equal(esperado, RvmAvatarCor.Iniciais(nome));
    }
}
