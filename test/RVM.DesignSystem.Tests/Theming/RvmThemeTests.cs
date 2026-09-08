using RVM.DesignSystem.Theming;

namespace RVM.DesignSystem.Tests.Theming;

public class RvmThemeTests
{
    [Fact]
    public void FromSeed_preserva_a_semente_no_modo_claro_quando_ela_ja_passa()
    {
        // O roxo da marca da 9.38 contra o fundo claro: nao precisa de ajuste, e a
        // derivacao nao deve "melhorar" o que ja esta certo.
        Assert.Equal("#641974", RvmThemes.Rvm.Light.Primary);
        Assert.Equal("#006DBD", RvmThemes.Rvm.Light.Secondary);
    }

    [Fact]
    public void FromSeed_clareia_a_marca_no_modo_escuro()
    {
        var claro = RvmColor.Parse(RvmThemes.Rvm.Light.Primary);
        var escuro = RvmColor.Parse(RvmThemes.Rvm.Dark.Primary);

        var (lClaro, _, hueClaro) = claro.ToOklch();
        var (lEscuro, _, hueEscuro) = escuro.ToOklch();

        Assert.True(lEscuro > lClaro, "A marca no escuro tem que ser mais clara que no claro.");
        Assert.Equal(hueClaro, hueEscuro, 0);
    }

    [Fact]
    public void FromSeed_matiza_o_cinza_com_a_cor_da_marca()
    {
        // O detalhe que faz a interface parecer desenhada em vez de montada (`06` § Escala
        // neutra): o cinza do tema teal puxa para o teal, o do verde puxa para o verde.
        var (_, cromaFiscal, hueFiscal) = RvmColor.Parse(RvmThemes.Samples.Fiscal.Light.SurfaceSunken).ToOklch();
        var (_, _, hueSemente) = RvmColor.Parse("#1B5E5A").ToOklch();

        Assert.True(cromaFiscal > 0, "O cinza saiu completamente dessaturado.");

        // Tolerancia larga de proposito: o croma do neutro e ~4% do da semente, e nessa faixa
        // um passo de 1/255 num canal ja gira o matiz varios graus. O que este teste afirma e
        // que o cinza PUXA para a marca, nao que ele reproduza o matiz com precisao — precisao
        // que o sRGB de 8 bits nao tem nesse croma.
        Assert.InRange(Math.Abs(hueFiscal - hueSemente), 0, 20);
    }

    [Fact]
    public void FromSeed_recusa_nome_vazio()
    {
        Assert.Throws<ArgumentException>(() => RvmTheme.FromSeed("  ", "#641974", "#006DBD"));
    }

    [Fact]
    public void FromSeed_recusa_cor_invalida()
    {
        Assert.Throws<FormatException>(() => RvmTheme.FromSeed("X", "nao-e-cor", "#006DBD"));
    }

    [Fact]
    public void FromSeed_aguenta_uma_semente_quase_branca_sem_produzir_texto_ilegivel()
    {
        // Caso hostil de proposito: um amarelo claro como cor principal. A derivacao tem que
        // escurece-lo ate passar, em vez de devolver amarelo sobre branco.
        var tema = RvmTheme.FromSeed("Hostil", "#FFEE00", "#FFFFFF");

        foreach (var paleta in new[] { tema.Light, tema.Dark })
        {
            foreach (var (nome, frente, fundo, minimo) in paleta.PairsToVerify())
            {
                var razao = RvmColor.Contrast(RvmColor.Parse(frente), RvmColor.Parse(fundo));
                Assert.True(razao >= minimo, $"{nome}: {razao:F2} < {minimo:F1}");
            }
        }
    }

    [Fact]
    public void ToCss_emite_os_tres_blocos_que_o_modo_escuro_precisa()
    {
        var css = RvmThemes.Rvm.ToCss();

        Assert.Contains(":root{", css, StringComparison.Ordinal);
        Assert.Contains(":root[data-rvm-theme=\"dark\"]{", css, StringComparison.Ordinal);

        // O :not() e o que faz a escolha explicita do usuario vencer a preferencia do sistema.
        // Sem ele, quem usa o SO no escuro nao consegue forcar o site no claro.
        Assert.Contains(":root:not([data-rvm-theme=\"light\"])", css, StringComparison.Ordinal);
    }

    [Fact]
    public void ToCss_emite_todo_papel_semantico()
    {
        var css = RvmThemes.Rvm.ToCss();

        string[] esperados =
        [
            "--rvm-color-primary:", "--rvm-color-on-primary:",
            "--rvm-color-primary-container:", "--rvm-color-on-primary-container:",
            "--rvm-color-secondary:", "--rvm-color-on-secondary:",
            "--rvm-color-success:", "--rvm-color-warning:", "--rvm-color-danger:", "--rvm-color-info:",
            "--rvm-color-surface:", "--rvm-color-on-surface:",
            "--rvm-color-surface-raised:", "--rvm-color-surface-sunken:",
            "--rvm-color-background:", "--rvm-color-on-background:",
            "--rvm-color-border:", "--rvm-color-border-strong:",
            "--rvm-color-disabled:", "--rvm-color-on-disabled:", "--rvm-color-focus-ring:",
        ];

        foreach (var token in esperados)
        {
            Assert.Contains(token, css, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ToCss_nao_depende_da_cultura_do_processo()
    {
        // A cultura do teste esta fixada em en-US (TestCulture). Se algum valor fosse formatado
        // com a cultura corrente, um runner em pt-BR emitiria virgula decimal e o CSS quebraria
        // em silencio — o navegador ignora a declaracao invalida e a cor some.
        var css = RvmThemes.Rvm.ToCss();

        Assert.DoesNotContain(",", css.Replace("data-rvm-theme", string.Empty, StringComparison.Ordinal), StringComparison.Ordinal);
    }

    [Fact]
    public void O_tema_da_marca_usa_o_cinza_do_site_e_nao_o_derivado()
    {
        // A sobrescrita deliberada em RvmThemes.BuildRvm: sem ela, a biblioteca e o site de
        // documentacao discordariam da propria identidade.
        Assert.Equal("#EEEEF1", RvmThemes.Rvm.Light.Background);
        Assert.Equal("#1E1E26", RvmThemes.Rvm.Light.OnSurface);
    }

    [Fact]
    public void Todo_tema_registrado_tem_nome_unico()
    {
        var nomes = RvmThemes.All.Select(t => t.Name).ToList();

        Assert.Equal(nomes.Count, nomes.Distinct(StringComparer.Ordinal).Count());
    }
}
