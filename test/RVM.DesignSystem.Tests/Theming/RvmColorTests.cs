using RVM.DesignSystem.Theming;

namespace RVM.DesignSystem.Tests.Theming;

public class RvmColorTests
{
    [Theory]
    [InlineData("#641974", 100, 25, 116)]
    [InlineData("641974", 100, 25, 116)]
    [InlineData("#FFF", 255, 255, 255)]
    [InlineData("  #000  ", 0, 0, 0)]
    public void Parse_aceita_as_formas_validas(string entrada, int r, int g, int b)
    {
        var cor = RvmColor.Parse(entrada);

        Assert.Equal((byte)r, cor.R);
        Assert.Equal((byte)g, cor.G);
        Assert.Equal((byte)b, cor.B);
    }

    [Theory]
    [InlineData("#12345")]
    [InlineData("roxo")]
    [InlineData("#GGGGGG")]
    [InlineData("")]
    public void Parse_recusa_o_que_nao_e_cor(string entrada)
    {
        Assert.Throws<FormatException>(() => RvmColor.Parse(entrada));
    }

    [Fact]
    public void ToHex_devolve_o_que_Parse_leu()
    {
        Assert.Equal("#641974", RvmColor.Parse("#641974").ToHex());
    }

    [Fact]
    public void Preto_contra_branco_da_o_contraste_maximo_da_WCAG()
    {
        // 21:1 e o teto da escala — se esta conta estiver errada, todo o portao esta errado.
        var razao = RvmColor.Contrast(new RvmColor(0, 0, 0), new RvmColor(255, 255, 255));

        Assert.Equal(21.0, razao, 2);
    }

    [Fact]
    public void Cor_contra_ela_mesma_da_1()
    {
        var roxo = RvmColor.Parse("#641974");

        Assert.Equal(1.0, RvmColor.Contrast(roxo, roxo), 5);
    }

    [Fact]
    public void Contraste_e_simetrico()
    {
        var a = RvmColor.Parse("#641974");
        var b = RvmColor.Parse("#EEEEF1");

        Assert.Equal(RvmColor.Contrast(a, b), RvmColor.Contrast(b, a), 10);
    }

    [Fact]
    public void O_azul_nominal_do_VS_Code_REPROVA_e_o_renderizado_passa()
    {
        // Este teste guarda a decisao registrada em 06 § A identidade da marca. Se alguem
        // "corrigir" a semente para o valor nominal achando que esta consertando, cai aqui —
        // com o numero na mensagem, nao com um erro generico de contraste.
        var fundo = RvmColor.Parse("#EEEEF1");

        var nominal = RvmColor.Contrast(RvmColor.Parse("#007ACC"), fundo);
        var renderizado = RvmColor.Contrast(RvmColor.Parse("#006DBD"), fundo);

        Assert.True(nominal < RvmContrast.NormalText, $"#007ACC deu {nominal:F2}, esperava reprovar.");
        Assert.True(renderizado >= RvmContrast.NormalText, $"#006DBD deu {renderizado:F2}, esperava passar.");
    }

    [Fact]
    public void Oklch_faz_ida_e_volta_sem_perder_a_cor()
    {
        var original = RvmColor.Parse("#641974");
        var (l, c, h) = original.ToOklch();

        var volta = RvmColor.FromOklch(l, c, h);

        // 1 de tolerancia por canal: a ida e volta passa por cbrt e gamma, e arredondar
        // para byte no fim custa no maximo um passo.
        Assert.InRange(Math.Abs(volta.R - original.R), 0, 1);
        Assert.InRange(Math.Abs(volta.G - original.G), 0, 1);
        Assert.InRange(Math.Abs(volta.B - original.B), 0, 1);
    }

    [Fact]
    public void WithLightness_clareia_sem_mudar_o_matiz()
    {
        var escuro = RvmColor.Parse("#641974");
        var claro = escuro.WithLightness(0.80);

        var (_, _, hueOriginal) = escuro.ToOklch();
        var (lClaro, _, hueClaro) = claro.ToOklch();

        Assert.True(lClaro > 0.75, "Nao clareou.");

        // 2 graus de tolerancia: clarear um roxo saturado empurra a cor para fora do gamut
        // sRGB, e o recorte de volta desloca o matiz um pouco. Exigir igualdade exata seria
        // exigir do sRGB algo que ele nao consegue entregar.
        Assert.InRange(Math.Abs(hueClaro - hueOriginal), 0, 2);
    }

    [Fact]
    public void MixWith_cruza_o_vermelho_pelo_caminho_curto()
    {
        // Misturar 350 graus com 10 graus tem que passar por 0 (vermelho), nao dar a volta
        // inteira pelo verde. E o bug classico de interpolacao de matiz.
        var quaseVermelhoPorBaixo = RvmColor.FromOklch(0.6, 0.15, 350);
        var quaseVermelhoPorCima = RvmColor.FromOklch(0.6, 0.15, 10);

        var meio = quaseVermelhoPorBaixo.MixWith(quaseVermelhoPorCima, 0.5);
        var (_, _, hue) = meio.ToOklch();

        var distanciaDeZero = Math.Min(hue, 360 - hue);
        Assert.True(distanciaDeZero < 15, $"O meio caiu em {hue:F0} graus — deu a volta pelo verde.");
    }

    [Fact]
    public void MixWith_nos_extremos_devolve_as_pontas()
    {
        var a = RvmColor.Parse("#641974");
        var b = RvmColor.Parse("#006DBD");

        Assert.Equal(a.ToHex(), a.MixWith(b, 0).ToHex());
        Assert.Equal(b.ToHex(), a.MixWith(b, 1).ToHex());
    }

    [Fact]
    public void Ensure_devolve_a_cor_intacta_quando_ela_ja_passa()
    {
        var roxo = RvmColor.Parse("#641974");
        var fundoClaro = RvmColor.Parse("#EEEEF1");

        // 9.38 contra o fundo: nao ha o que ajustar, e ajustar seria estragar.
        Assert.Equal(roxo.ToHex(), RvmContrast.Ensure(roxo, fundoClaro, RvmContrast.NormalText).ToHex());
    }

    [Fact]
    public void Ensure_satura_no_extremo_quando_o_alvo_e_impossivel()
    {
        // Contra um cinza medio, NENHUMA cor atinge 21:1 — nem preto nem branco. O contrato
        // aqui e devolver o melhor possivel e deixar o TESTE de contraste reprovar depois:
        // falhar visivel, em vez de entrar em laco ou devolver algo aleatorio.
        var cinzaMedio = new RvmColor(128, 128, 128);

        var resultado = RvmContrast.Ensure(RvmColor.Parse("#641974"), cinzaMedio, 21.0);

        var ehExtremo = resultado.ToHex() is "#000000" or "#FFFFFF";
        Assert.True(ehExtremo, $"Esperava preto ou branco, veio {resultado.ToHex()}.");
        Assert.True(RvmColor.Contrast(resultado, cinzaMedio) < 21.0, "Se chegou a 21, o teste esta errado.");
    }

    [Theory]
    [InlineData("#FFFFFF", true)]
    [InlineData("#000000", false)]
    [InlineData("#641974", false)]
    public void BestForegroundOn_escolhe_o_lado_legivel(string fundo, bool esperaEscuro)
    {
        var frente = RvmContrast.BestForegroundOn(RvmColor.Parse(fundo));

        var ficouEscuro = frente.RelativeLuminance < 0.5;
        Assert.Equal(esperaEscuro, ficouEscuro);
    }

    [Fact]
    public void ScaleChroma_com_zero_devolve_cinza()
    {
        var cinza = RvmColor.Parse("#641974").ScaleChroma(0);

        var (_, c, _) = cinza.ToOklch();
        Assert.True(c < 0.01, $"Sobrou croma: {c:F4}");
    }
}
