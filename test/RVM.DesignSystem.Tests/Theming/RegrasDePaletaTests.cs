using RVM.DesignSystem.Theming;

namespace RVM.DesignSystem.Tests.Theming;

/// <summary>
/// As regras de construcao de paleta, viradas em portao.
/// </summary>
/// <remarks>
/// Sao as cinco regras que o Rafael definiu em 08/09/2026. Quatro delas dao para verificar; a
/// quinta, nao — e a distincao esta escrita aqui de proposito, para ninguem procurar depois o
/// teste que "faltou".
///
/// <para>
/// <b>A regra 4 (proporcao 60-30-10) nao tem teste, e nao pode ter.</b> Ela e de COMPOSICAO:
/// fala de quanto de cada cor aparece numa tela montada, nao de quais cores a paleta tem. A
/// mesma paleta obedece a regra numa tela e a viola noutra. Um teste que dissesse verificar isso
/// estaria medindo outra coisa — e um portao que mede a coisa errada e pior que portao nenhum.
/// Ela vive na documentacao (`/fundamentos/paleta`), demonstrada.
/// </para>
/// </remarks>
public class RegrasDePaletaTests
{
    /// <summary>Cores de marca reais e casos de borda, para as regras valerem em todas.</summary>
    public static TheoryData<string, string> Marcas()
    {
        var dados = new TheoryData<string, string>();

        foreach (var (primaria, secundaria) in new[]
        {
            ("#641974", "#006DBD"),   // RVM
            ("#2E7D32", "#F9A825"),   // ERPAgro
            ("#1B5E5A", "#2A6F97"),   // Fiscal
            ("#000000", "#FFFFFF"),   // extremos: preto e branco como marca
            ("#FFFF00", "#00FFFF"),   // saturacao maxima, matizes claras
            ("#7F7F7F", "#808080"),   // cinza puro: croma zero
        })
        {
            dados.Add(primaria, secundaria);
        }

        return dados;
    }

    // ---------- Regra 2: neutros sem preto puro ----------

    [Theory]
    [MemberData(nameof(Marcas))]
    public void O_texto_NUNCA_e_preto_puro_nem_branco_puro(string primaria, string secundaria)
    {
        // "O preto puro (#000000) cansa a vista" — regra 2. Vale para os dois modos: no escuro,
        // branco puro sobre fundo quase preto e o mesmo problema ao contrario.
        var tema = RvmTheme.FromSeed("Teste", primaria, secundaria);

        foreach (var (modo, paleta) in new[] { ("claro", tema.Light), ("escuro", tema.Dark) })
        {
            foreach (var (papel, cor) in new[]
            {
                ("on-surface", paleta.OnSurface),
                ("on-background", paleta.OnBackground),
                ("on-surface-variant", paleta.OnSurfaceVariant),
            })
            {
                Assert.False(
                    cor.Equals("#000000", StringComparison.OrdinalIgnoreCase),
                    $"{papel} do modo {modo} saiu preto puro com a marca {primaria}.");

                Assert.False(
                    cor.Equals("#FFFFFF", StringComparison.OrdinalIgnoreCase),
                    $"{papel} do modo {modo} saiu branco puro com a marca {primaria}.");
            }
        }
    }

    [Theory]
    [MemberData(nameof(Marcas))]
    public void O_fundo_claro_e_claro_e_o_escuro_e_escuro(string primaria, string secundaria)
    {
        // "Branco ou cinza muito claro para fundos de telas claras; cinzas escuros ou preto
        // suave no modo escuro" — regra 2. Uma marca escura demais nao pode arrastar o fundo
        // junto: os neutros sao matizados pela primaria, e sem limite isso viraria uma tela roxa.
        var tema = RvmTheme.FromSeed("Teste", primaria, secundaria);

        var claro = RvmColor.Parse(tema.Light.Background).ToOklch().L;
        var escuro = RvmColor.Parse(tema.Dark.Background).ToOklch().L;

        Assert.True(claro > 0.90, $"O fundo claro saiu com L={claro:0.00} (esperado > 0.90).");
        Assert.True(escuro < 0.25, $"O fundo escuro saiu com L={escuro:0.00} (esperado < 0.25).");
    }

    // ---------- Regra 3: cores de feedback escolhiveis ----------

    [Fact]
    public void Sem_escolha_as_cores_de_estado_sao_a_convencao()
    {
        // Verde, ambar, vermelho, azul. E o padrao porque e a leitura mais comum no Brasil.
        var tema = RvmTheme.FromSeed("Teste", "#641974", "#006DBD");

        var matizes = new[]
        {
            ("sucesso", tema.Light.Success, 148.0),
            ("aviso", tema.Light.Warning, 75.0),
            ("erro", tema.Light.Danger, 25.0),
            ("info", tema.Light.Info, 245.0),
        };

        foreach (var (nome, cor, esperada) in matizes)
        {
            var hue = RvmColor.Parse(cor).ToOklch().H;

            Assert.True(
                Math.Abs(hue - esperada) < 12,
                $"A matiz de {nome} saiu em {hue:0} (esperado ~{esperada:0}).");
        }
    }

    [Fact]
    public void A_cor_de_estado_ESCOLHIDA_mantem_a_matiz_de_quem_escolheu()
    {
        // "As cores de feedback nao precisam ser necessariamente estas" — um produto de saude,
        // de sinalizacao ou cuja marca ja e vermelha tem motivo real para discordar.
        var tema = RvmTheme.FromSeed("Teste", new RvmSeed
        {
            Primary = "#641974",
            Secondary = "#006DBD",
            Success = "#0057B8",   // sucesso em AZUL
            Danger = "#7B2D8E",    // erro em ROXO
        });

        var sucesso = RvmColor.Parse(tema.Light.Success).ToOklch();
        var erro = RvmColor.Parse(tema.Light.Danger).ToOklch();

        var azul = RvmColor.Parse("#0057B8").ToOklch();
        var roxo = RvmColor.Parse("#7B2D8E").ToOklch();

        Assert.True(Math.Abs(sucesso.H - azul.H) < 12, $"Sucesso saiu na matiz {sucesso.H:0}, nao na do azul {azul.H:0}.");
        Assert.True(Math.Abs(erro.H - roxo.H) < 12, $"Erro saiu na matiz {erro.H:0}, nao na do roxo {roxo.H:0}.");

        // O que NAO foi informado continua no padrao.
        var aviso = RvmColor.Parse(tema.Light.Warning).ToOklch().H;
        Assert.True(Math.Abs(aviso - 75) < 12, $"O aviso, que nao foi informado, saiu em {aviso:0}.");
    }

    [Fact]
    public void A_sobrecarga_de_TRES_argumentos_continua_dando_o_mesmo_tema()
    {
        // A 1.0 congelou o contrato: a API antiga nao pode mudar de comportamento por causa da
        // sobrecarga nova.
        var antiga = RvmTheme.FromSeed("Teste", "#641974", "#006DBD");
        var nova = RvmTheme.FromSeed("Teste", new RvmSeed { Primary = "#641974", Secondary = "#006DBD" });

        Assert.Equal(antiga.Light, nova.Light);
        Assert.Equal(antiga.Dark, nova.Dark);
    }

    // ---------- Regra 5: contraste, mesmo com cor escolhida ----------

    [Theory]
    [MemberData(nameof(Marcas))]
    public void Cor_de_estado_ESCOLHIDA_continua_passando_no_contraste(string primaria, string secundaria)
    {
        // ⚠️ O teste que mais importa desta lista. Aceitar a cor de quem usa NAO pode abrir um
        // buraco no portao — a escolha e de identidade; o contraste nao e negociavel.
        //
        // As cores de estado aqui sao de proposito as PIORES possiveis: amarelo puro e branco
        // sobre fundo claro seriam ilegiveis se entrassem como foram informados.
        var tema = RvmTheme.FromSeed("Teste", new RvmSeed
        {
            Primary = primaria,
            Secondary = secundaria,
            Success = "#FFFF00",
            Warning = "#FFFFFF",
            Danger = "#00FF00",
            Info = "#FFFF80",
        });

        foreach (var (modo, paleta) in new[] { ("claro", tema.Light), ("escuro", tema.Dark) })
        {
            foreach (var (nome, frente, fundo, minimo) in paleta.PairsToVerify())
            {
                var razao = RvmColor.Contrast(RvmColor.Parse(frente), RvmColor.Parse(fundo));

                Assert.True(
                    razao >= minimo,
                    $"[{modo}] {nome} deu {razao:0.00}:1, abaixo de {minimo}:1 — com a marca "
                    + $"{primaria} e cores de estado extremas.");
            }
        }
    }

    [Fact]
    public void Cinza_informado_continua_CINZA()
    {
        // O motor nao inventa saturacao que ninguem pediu: quem escolhe um cinza para "info"
        // esta dizendo que nao quer cor ali.
        var tema = RvmTheme.FromSeed("Teste", new RvmSeed
        {
            Primary = "#641974",
            Secondary = "#006DBD",
            Info = "#808080",
        });

        var croma = RvmColor.Parse(tema.Light.Info).ToOklch().C;

        Assert.True(croma < 0.03, $"O cinza informado saiu com croma {croma:0.000}.");
    }
}
