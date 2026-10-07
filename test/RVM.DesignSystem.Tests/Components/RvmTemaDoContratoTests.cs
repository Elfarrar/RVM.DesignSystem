using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using RVM.DesignSystem.Theming;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>
/// A parte de TEMA do contrato com o RVM.UI (DSGN-017): Accent, ColorScheme, Settings e UserTheme no provider, o
/// RvmThemePicker, o RvmThemeState e o CSS das paletas — com o contraste AA MEDIDO no proprio CSS, nos dois temas.
/// </summary>
public class RvmTemaDoContratoTests : BunitContext
{
    public RvmTemaDoContratoTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    // ---------------------------------------------------------------- CSS: contraste das paletas (medido)

    private static readonly string Css = Regex.Replace(
        File.ReadAllText(RaizDoRepositorio.Biblioteca("wwwroot", "rvm-design-system.css")), @"/\*.*?\*/", "", RegexOptions.Singleline);

    public static TheoryData<string> Paletas()
    {
        var dados = new TheoryData<string>();
        foreach (var paleta in RvmPalettes.All)
        {
            dados.Add(paleta.Id);
        }

        return dados;
    }

    private static Dictionary<string, string> Bloco(string seletor)
    {
        var achados = Regex.Matches(Css, @"(?m)^\s*" + Regex.Escape(seletor).Replace(@"\n", @"\s*", StringComparison.Ordinal) + @"\s*\{([^}]*)\}");
        var bloco = Assert.Single(achados);
        return Regex.Matches(bloco.Groups[1].Value, @"--(rvm-[\w-]+)\s*:\s*([^;]+);")
            .ToDictionary(m => m.Groups[1].Value, m => Regex.Replace(m.Groups[2].Value.Trim(), @"\s+", " "));
    }

    private static Dictionary<string, string> BaseClara() => Bloco("[data-theme='system'],\n:root,\n[data-theme='light']");

    private static double Luminancia(string hex)
    {
        Assert.Matches("^#[0-9A-Fa-f]{6}$", hex);
        double Canal(int i)
        {
            var v = int.Parse(hex.AsSpan(1 + (i * 2), 2), NumberStyles.HexNumber) / 255.0;
            return v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
        }

        return (0.2126 * Canal(0)) + (0.7152 * Canal(1)) + (0.0722 * Canal(2));
    }

    private static double Contraste(string a, string b)
    {
        var (la, lb) = (Luminancia(a), Luminancia(b));
        return (Math.Max(la, lb) + 0.05) / (Math.Min(la, lb) + 0.05);
    }

    private static void AoMenosAA(string frente, string fundo, string oQue)
    {
        var razao = Contraste(frente, fundo);
        Assert.True(razao >= 4.5, $"{oQue}: {frente} sobre {fundo} da {razao:0.00}:1, abaixo de 4.5:1.");
    }

    [Theory]
    [MemberData(nameof(Paletas))]
    public void Paleta_passa_AA_no_tema_claro(string id)
    {
        var p = Bloco($"[data-rvm-accent=\"{id}\"]");
        var baseClara = BaseClara();
        var texto = p["rvm-color-primary-text"];

        AoMenosAA(texto, baseClara["rvm-color-background-body"], $"{id} claro, texto no corpo");
        AoMenosAA(texto, baseClara["rvm-color-background-paper"], $"{id} claro, texto no papel");
        AoMenosAA(texto, p["rvm-color-primary-soft"], $"{id} claro, texto no fundo suave");
        foreach (var fundo in new[] { "rvm-color-primary-main", "rvm-color-primary-hover", "rvm-color-primary-alt-light" })
        {
            AoMenosAA(p["rvm-color-primary-contrast"], p[fundo], $"{id} claro, contraste sobre {fundo}");
        }

        // O menu ativo e um degrade do menu-active ao main. No cobalto o branco da 4.43:1 na ponta do menu-active
        // e passa a partir de 10% da largura (o texto comeca em 22%, ver RvmAppShell.razor.css) — valor do kit,
        // mantido. As paletas novas passam ja na ponta.
        if (id != "blue")
        {
            AoMenosAA(p["rvm-color-primary-contrast"], p["rvm-color-menu-active"], $"{id} claro, contraste sobre o menu ativo");
        }
    }

    [Theory]
    [MemberData(nameof(Paletas))]
    public void Paleta_passa_AA_no_tema_escuro(string id)
    {
        var claro = Bloco($"[data-rvm-accent=\"{id}\"]");
        var escuro = Bloco($"[data-theme='dark']:where([data-rvm-accent=\"{id}\"])");
        var baseEscura = Bloco("[data-theme='dark']");
        string Valor(string token) => escuro.TryGetValue(token, out var v) && v.StartsWith('#') ? v : claro[token];
        var texto = escuro["rvm-color-primary-text"];

        AoMenosAA(texto, baseEscura["rvm-color-background-body"], $"{id} escuro, texto no corpo");
        AoMenosAA(texto, baseEscura["rvm-color-background-paper"], $"{id} escuro, texto no papel");
        AoMenosAA(texto, escuro["rvm-color-primary-soft"], $"{id} escuro, texto no fundo suave");
        // No escuro o menu ativo e o proprio main (var no bloco escuro).
        Assert.Equal("var(--rvm-color-primary-main)", escuro["rvm-color-menu-active"]);
        foreach (var fundo in new[] { "rvm-color-primary-main", "rvm-color-primary-hover", "rvm-color-primary-alt-light" })
        {
            AoMenosAA(Valor("rvm-color-primary-contrast"), Valor(fundo), $"{id} escuro, contraste sobre {fundo}");
        }
    }

    [Fact]
    public void Azul_e_exatamente_o_cobalto_de_sempre()
    {
        // Decisao 4 do Rafael: sem Accent, nada muda. O bloco azul repete o base, valor a valor.
        var azul = Bloco("[data-rvm-accent=\"blue\"]");
        var baseClara = BaseClara();
        foreach (var (token, valor) in azul.Where(t => t.Key.StartsWith("rvm-color-", StringComparison.Ordinal)))
        {
            Assert.Equal(baseClara[token], valor);
        }

        Assert.Equal(baseClara["rvm-font-family"], azul["rvm-font-family"]);
        var baseEscura = Bloco("[data-theme='dark']");
        foreach (var (token, valor) in Bloco("[data-theme='dark']:where([data-rvm-accent=\"blue\"])").Where(t => t.Key != "rvm-focus-ring-color"))
        {
            Assert.Equal(baseEscura[token], valor);
        }
    }

    public static TheoryData<string, string> Gemeos()
    {
        var dados = new TheoryData<string, string> { { "[data-theme='dark']", "[data-theme='system']" } };
        foreach (var paleta in RvmPalettes.All)
        {
            dados.Add($"[data-theme='dark']:where([data-rvm-accent=\"{paleta.Id}\"])", $"[data-theme='system']:where([data-rvm-accent=\"{paleta.Id}\"])");
        }

        dados.Add("[data-theme='dark']:where([data-rvm-contrast=\"original\"])", "[data-theme='system']:where([data-rvm-contrast=\"original\"])");
        return dados;
    }

    [Theory]
    [MemberData(nameof(Gemeos))]
    public void Automatico_com_sistema_escuro_repete_o_escuro(string escuro, string automatico)
    {
        Assert.Equal(Bloco(escuro), Bloco(automatico));
    }

    [Theory]
    [InlineData("profissional", "Hanken Grotesk", "hanken-grotesk", "OFL-HankenGrotesk.txt")]
    [InlineData("acolhedor", "Lora", "lora", "OFL-Lora.txt")]
    [InlineData("utilitario", "Geist Mono", "geist-mono", "OFL-GeistMono.txt")]
    public void Paleta_de_produto_traz_a_fonte_empacotada_com_a_licenca(string id, string fonte, string arquivo, string licenca)
    {
        Assert.StartsWith($"'{fonte}'", Bloco($"[data-rvm-accent=\"{id}\"]")["rvm-font-family"], StringComparison.Ordinal);
        foreach (var subconjunto in new[] { "latin", "latin-ext" })
        {
            Assert.Contains($"url('./fonts/{arquivo}-{subconjunto}.woff2')", Css, StringComparison.Ordinal);
            var bytes = File.ReadAllBytes(RaizDoRepositorio.Biblioteca("wwwroot", "fonts", $"{arquivo}-{subconjunto}.woff2"));
            Assert.Equal("wOF2"u8.ToArray(), bytes[..4]);
        }

        Assert.Contains("SIL Open Font License, Version 1.1", File.ReadAllText(RaizDoRepositorio.Biblioteca("wwwroot", "fonts", licenca)), StringComparison.Ordinal);
    }

    [Fact]
    public void Original_usa_os_degraus_do_kit_e_nao_mexe_no_destaque()
    {
        var original = Bloco("[data-rvm-contrast=\"original\"]");
        Assert.Equal("var(--rvm-color-warning-main)", original["rvm-color-warning-text"]);
        Assert.Equal("#FFFFFF", original["rvm-color-info-contrast"]);
        Assert.Equal("rgba(58, 53, 65, 0.68)", original["rvm-color-text-secondary"]);
        Assert.DoesNotContain(original.Keys, k => k.StartsWith("rvm-color-primary", StringComparison.Ordinal));
        // E reprova AA mesmo: e o aviso documentado no RvmColorScheme.Original.
        Assert.True(Contraste(BaseClara()["rvm-color-warning-main"], "#FFFFFF") < 4.5);
    }

    // ---------------------------------------------------------------- Provider

    [Fact]
    public void Provider_sem_parametro_continua_azul_acessivel_e_claro()
    {
        var raiz = Render<RvmThemeProvider>().Find("div");

        Assert.Equal("light", raiz.GetAttribute("data-theme"));
        Assert.Equal("blue", raiz.GetAttribute("data-rvm-accent"));
        Assert.Equal("accessible", raiz.GetAttribute("data-rvm-contrast"));
        Assert.Null(raiz.GetAttribute("data-rvm-font-scale"));
        Assert.Null(raiz.GetAttribute("data-rvm-high-contrast"));
        Assert.Null(raiz.GetAttribute("data-rvm-motion"));
    }

    [Theory]
    [InlineData(RvmAccent.Purple, "purple")]
    [InlineData(RvmAccent.Blue, "blue")]
    [InlineData(RvmAccent.Black, "black")]
    public void Provider_escreve_o_destaque(RvmAccent destaque, string esperado)
    {
        var raiz = Render<RvmThemeProvider>(p => p.Add(x => x.Accent, destaque)).Find("div");

        Assert.Equal(esperado, raiz.GetAttribute("data-rvm-accent"));
    }

    [Fact]
    public void Provider_com_esquema_original_e_classe()
    {
        var raiz = Render<RvmThemeProvider>(p => p
            .Add(x => x.ColorScheme, RvmColorScheme.Original)
            .Add(x => x.Class, "app")).Find("div");

        Assert.Equal("original", raiz.GetAttribute("data-rvm-contrast"));
        Assert.Equal("rvm-root app", raiz.GetAttribute("class"));
    }

    [Fact]
    public void Provider_com_settings_aplica_o_tema_completo_e_vence_theme_e_accent()
    {
        var raiz = Render<RvmThemeProvider>(p => p
            .Add(x => x.Theme, RvmTheme.Light)
            .Add(x => x.Accent, RvmAccent.Black)
            .Add(x => x.Settings, new RvmThemeSettings("acolhedor", RvmThemeMode.System, RvmFontScale.ExtraLarge, true, true))).Find("div");

        Assert.Equal("system", raiz.GetAttribute("data-theme"));
        Assert.Equal("acolhedor", raiz.GetAttribute("data-rvm-accent"));
        Assert.Equal("xlarge", raiz.GetAttribute("data-rvm-font-scale"));
        Assert.Equal("true", raiz.GetAttribute("data-rvm-high-contrast"));
        Assert.Equal("reduce", raiz.GetAttribute("data-rvm-motion"));
    }

    [Fact]
    public void Paleta_invalida_nas_settings_volta_ao_azul()
    {
        var raiz = Render<RvmThemeProvider>(p => p.Add(x => x.Settings, new RvmThemeSettings("\"><script>"))).Find("div");

        Assert.Equal("blue", raiz.GetAttribute("data-rvm-accent"));
    }

    [Fact]
    public void Provider_com_settings_nao_le_nem_grava_a_chave_do_theme()
    {
        Render<RvmThemeProvider>(p => p.Add(x => x.Settings, new RvmThemeSettings(Mode: RvmThemeMode.Dark)));

        Assert.Empty(JSInterop.Invocations["rvmTheme.read"]);
        var chamada = Assert.Single(JSInterop.Invocations["rvmTheme.apply"]);
        Assert.Equal("dark", chamada.Arguments[0]);
        Assert.Equal(false, chamada.Arguments[1]);
    }

    [Fact]
    public async Task UserTheme_le_o_estado_redesenha_na_troca_e_avisa_claro_ou_escuro()
    {
        Services.AddRvmDesignSystem();
        var avisos = new List<RvmTheme>();
        var cortado = Render<RvmThemeProvider>(p => p
            .Add(x => x.UserTheme, true)
            .Add(x => x.ThemeChanged, EventCallback.Factory.Create<RvmTheme>(this, avisos.Add)));
        var estado = Services.GetRequiredService<RvmThemeState>();

        Assert.Equal("blue", cortado.Find("div").GetAttribute("data-rvm-accent"));

        await cortado.InvokeAsync(() => estado.SetAsync(new RvmThemeSettings("purple", RvmThemeMode.Dark, RvmFontScale.Large)));

        cortado.WaitForAssertion(() =>
        {
            var raiz = cortado.Find("div");
            Assert.Equal("purple", raiz.GetAttribute("data-rvm-accent"));
            Assert.Equal("dark", raiz.GetAttribute("data-theme"));
            Assert.Equal("large", raiz.GetAttribute("data-rvm-font-scale"));
        });
        Assert.Equal([RvmTheme.Dark], avisos);
        var gravado = Assert.Single(JSInterop.Invocations["rvmTheme.save"]);
        Assert.Equal(["rvm.tema", "v1|purple|dark|large|0|0"], gravado.Arguments);
    }

    [Fact]
    public async Task Toggle_com_UserTheme_grava_o_modo_na_escolha_do_usuario()
    {
        Services.AddRvmDesignSystem();
        var cortado = Render<RvmThemeProvider>(p => p.Add(x => x.UserTheme, true));
        var estado = Services.GetRequiredService<RvmThemeState>();

        await cortado.InvokeAsync(cortado.Instance.ToggleAsync);

        Assert.Equal(RvmThemeMode.Dark, estado.Current.Mode);
        cortado.WaitForAssertion(() => Assert.Equal("dark", cortado.Find("div").GetAttribute("data-theme")));
    }

    [Fact]
    public void UserTheme_sem_registro_explica_o_que_falta()
    {
        var erro = Assert.Throws<InvalidOperationException>(() => Render<RvmThemeProvider>(p => p.Add(x => x.UserTheme, true)));

        Assert.Contains("AddRvmDesignSystem", erro.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Provider_aninhado_com_tema_do_usuario_nao_pinta_o_documento()
    {
        Render<RvmThemeProvider>(p => p
            .Add(x => x.Persist, false)
            .AddChildContent<RvmThemeProvider>(f => f.Add(x => x.Settings, new RvmThemeSettings(Mode: RvmThemeMode.Dark))));

        var temas = JSInterop.Invocations["rvmTheme.apply"].Select(i => i.Arguments[0]).ToArray();
        Assert.Equal(["light"], temas);
    }

    [Fact]
    public void WebAssembly_le_a_escolha_salva_antes_do_primeiro_render()
    {
        Services.AddRvmDesignSystem();
        JSInterop.Setup<string?>("rvmTheme.load", "rvm.tema").SetResult("v1|utilitario|dark|small|0|1");

        var raiz = Render<RvmThemeProvider>(p => p.Add(x => x.UserTheme, true)).Find("div");

        Assert.Equal("utilitario", raiz.GetAttribute("data-rvm-accent"));
        Assert.Equal("dark", raiz.GetAttribute("data-theme"));
        Assert.Equal("small", raiz.GetAttribute("data-rvm-font-scale"));
        Assert.Equal("reduce", raiz.GetAttribute("data-rvm-motion"));
    }

    // ---------------------------------------------------------------- Settings e registro

    [Fact]
    public void Settings_ida_e_volta_pelo_texto()
    {
        var escolha = new RvmThemeSettings("profissional", RvmThemeMode.System, RvmFontScale.Large, true, false);

        Assert.Equal("v1|profissional|system|large|1|0", escolha.Serialize());
        Assert.Equal(escolha, RvmThemeSettings.Parse(escolha.Serialize()));
        Assert.Equal(escolha, RvmThemeSettings.Parse("v1%7Cprofissional%7Csystem%7Clarge%7C1%7C0"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("v2|purple|dark")]
    [InlineData("lixo")]
    [InlineData("%E0%A4%A")]
    public void Texto_invalido_volta_ao_padrao_sem_lancar(string? texto)
    {
        Assert.Equal(RvmThemeSettings.Default, RvmThemeSettings.Parse(texto));
    }

    [Fact]
    public void Campo_invalido_volta_ao_padrao_so_naquele_campo()
    {
        var lido = RvmThemeSettings.Parse("v1|PALETA INVALIDA|escuro|large");

        Assert.Equal(new RvmThemeSettings(FontScale: RvmFontScale.Large), lido);
    }

    [Fact]
    public void Padrao_e_o_kit_como_ele_e()
    {
        Assert.Equal(new RvmThemeSettings("blue", RvmThemeMode.Light, RvmFontScale.Default, false, false), RvmThemeSettings.Default);
        Assert.Equal(["blue", "purple", "black", "profissional", "acolhedor", "utilitario"], RvmPalettes.All.Select(p => p.Id));
    }

    [Fact]
    public async Task Store_do_app_substitui_o_do_navegador_e_recebe_cada_troca()
    {
        Services.AddRvmDesignSystem();
        Services.AddRvmTheme<StoreDeTeste>();
        var estado = Services.GetRequiredService<RvmThemeState>();

        await estado.EnsureLoadedAsync();
        Assert.Equal("acolhedor", estado.Current.Palette);

        await estado.SetAsync(estado.Current with { Mode = RvmThemeMode.Dark });
        Assert.IsType<StoreDeTeste>(Services.GetRequiredService<IRvmThemeStore>());
        Assert.Equal(RvmThemeMode.Dark, StoreDeTeste.UltimoSalvo?.Mode);
    }

    private sealed class StoreDeTeste : IRvmThemeStore
    {
        public static RvmThemeSettings? UltimoSalvo { get; private set; }

        public ValueTask<RvmThemeSettings?> LoadAsync(CancellationToken cancellationToken = default) =>
            ValueTask.FromResult<RvmThemeSettings?>(new RvmThemeSettings("acolhedor"));

        public ValueTask SaveAsync(RvmThemeSettings settings, CancellationToken cancellationToken = default)
        {
            UltimoSalvo = settings;
            return ValueTask.CompletedTask;
        }
    }

    // ---------------------------------------------------------------- Picker

    [Fact]
    public void Picker_mostra_aparencia_paleta_fonte_e_acessibilidade()
    {
        Services.AddRvmDesignSystem();
        var cortado = Render<RvmThemePicker>(p => p.Add(x => x.Class, "preferencias").AddUnmatched("id", "tema"));

        var raiz = cortado.Find("#tema");
        Assert.Contains("preferencias", raiz.ClassList);
        Assert.Equal(["Aparencia", "Paleta", "Tamanho da fonte", "Acessibilidade"], cortado.FindAll("legend").Select(l => l.TextContent.Trim()));
        Assert.Equal(6, cortado.FindAll(".rvm-cartao").Count);
        Assert.Equal(2, cortado.FindAll("input[role='switch']").Count);
        Assert.True(cortado.Find("input[type='radio'][value='blue']").HasAttribute("checked"));
        Assert.True(cortado.Find("input[type='radio'][value='Light']").HasAttribute("checked"));
        // Um name por grupo: com o mesmo name, marcar a paleta desmarcaria a aparencia no navegador.
        Assert.Equal(3, cortado.FindAll("input[type='radio']").Select(r => r.GetAttribute("name")).Distinct().Count());
        // Cada cartao leva a propria paleta, no tema em vigor.
        var acolhedor = cortado.Find(".rvm-cartao[data-rvm-accent='acolhedor']");
        Assert.Equal("light", acolhedor.GetAttribute("data-theme"));
        Assert.Contains("Acolhedor", acolhedor.TextContent, StringComparison.Ordinal);
    }

    [Fact]
    public void Picker_sem_acessibilidade_e_com_paletas_escolhidas()
    {
        Services.AddRvmDesignSystem();
        var cortado = Render<RvmThemePicker>(p => p
            .Add(x => x.ShowAccessibility, false)
            .Add(x => x.Palettes, RvmPalettes.Produto));

        Assert.Equal(["Aparencia", "Paleta"], cortado.FindAll("legend").Select(l => l.TextContent.Trim()));
        Assert.Equal(["profissional", "acolhedor", "utilitario"], cortado.FindAll(".rvm-cartao").Select(c => c.GetAttribute("data-rvm-accent")));
        Assert.Empty(cortado.FindAll("input[role='switch']"));
    }

    [Fact]
    public void Picker_troca_grava_no_estado_e_avisa()
    {
        Services.AddRvmDesignSystem();
        var avisos = new List<RvmThemeSettings>();
        var cortado = Render<RvmThemePicker>(p => p.Add(x => x.OnChange, EventCallback.Factory.Create<RvmThemeSettings>(this, avisos.Add)));
        var estado = Services.GetRequiredService<RvmThemeState>();

        cortado.Find("input[type='radio'][value='utilitario']").Change("utilitario");
        cortado.Find("input[type='radio'][value='System']").Change("System");
        cortado.Find("input[type='radio'][value='ExtraLarge']").Change("ExtraLarge");
        cortado.FindAll("input[role='switch']")[0].Change(true);
        cortado.FindAll("input[role='switch']")[1].Change(true);

        var esperado = new RvmThemeSettings("utilitario", RvmThemeMode.System, RvmFontScale.ExtraLarge, true, true);
        Assert.Equal(esperado, estado.Current);
        Assert.Equal(5, avisos.Count);
        Assert.Equal(esperado, avisos[^1]);
        Assert.True(cortado.Find("input[type='radio'][value='utilitario']").HasAttribute("checked"));
    }

    [Fact]
    public void Picker_dentro_do_provider_pinta_as_amostras_no_modo_dele()
    {
        Services.AddRvmDesignSystem();
        var cortado = Render<RvmThemeProvider>(p => p
            .Add(x => x.Theme, RvmTheme.Dark)
            .Add(x => x.Persist, false)
            .AddChildContent<RvmThemePicker>());

        Assert.All(cortado.FindAll(".rvm-cartao"), c => Assert.Equal("dark", c.GetAttribute("data-theme")));

        cortado.Render(p => p.Add(x => x.Theme, RvmTheme.Light));

        cortado.WaitForAssertion(() =>
            Assert.All(cortado.FindAll(".rvm-cartao"), c => Assert.Equal("light", c.GetAttribute("data-theme"))));
    }

    [Fact]
    public async Task Picker_acompanha_troca_feita_por_fora()
    {
        Services.AddRvmDesignSystem();
        var cortado = Render<RvmThemePicker>();
        var estado = Services.GetRequiredService<RvmThemeState>();

        await cortado.InvokeAsync(() => estado.SetAsync(new RvmThemeSettings("black")));

        cortado.WaitForAssertion(() => Assert.True(cortado.Find("input[type='radio'][value='black']").HasAttribute("checked")));
    }

    // ---------------------------------------------------------------- Achados do review (DSGN-017)

    [Fact]
    public void Wasm_com_pre_render_a_escolha_do_localStorage_vence_o_padrao_que_o_servidor_mandou()
    {
        // O servidor nao le localStorage: na pre-renderizacao so sabia o padrao. Se isso vencesse, a escolha
        // salva nunca voltaria (achado P1).
        Services.AddRvmDesignSystem();
        AddBunitPersistentComponentState().Persist("rvm.tema", RvmThemeSettings.Default.Serialize());
        JSInterop.Setup<string?>("rvmTheme.load", "rvm.tema").SetResult("v1|purple|dark|default|0|0");

        var raiz = Render<RvmThemeProvider>(p => p.Add(x => x.UserTheme, true)).Find("div");

        Assert.Equal("purple", raiz.GetAttribute("data-rvm-accent"));
        Assert.Equal("dark", raiz.GetAttribute("data-theme"));
    }

    [Fact]
    public void Valor_conhecido_da_pre_renderizacao_vale_quando_o_navegador_nao_tem_nada()
    {
        Services.AddRvmDesignSystem();
        AddBunitPersistentComponentState().Persist("rvm.tema", "v1|acolhedor|light|default|0|0");

        var raiz = Render<RvmThemeProvider>(p => p.Add(x => x.UserTheme, true)).Find("div");

        Assert.Equal("acolhedor", raiz.GetAttribute("data-rvm-accent"));
    }

    [Fact]
    public void Padrao_por_falta_de_informacao_nao_viaja_ao_cliente()
    {
        Services.AddRvmDesignSystem();
        Services.AddRvmTheme<StoreVazio>();
        var estado = AddBunitPersistentComponentState();
        Render<RvmThemeProvider>(p => p.Add(x => x.UserTheme, true));

        estado.TriggerOnPersisting();

        Assert.False(estado.TryTake<string>("rvm.tema", out _));
    }

    [Fact]
    public void Dois_providers_com_tema_do_usuario_nao_gravam_a_mesma_chave_duas_vezes()
    {
        Services.AddRvmDesignSystem();
        var estado = AddBunitPersistentComponentState();
        JSInterop.Setup<string?>("rvmTheme.load", "rvm.tema").SetResult("v1|black|light|default|0|0");
        Render<RvmThemeProvider>(p => p.Add(x => x.UserTheme, true));
        Render<RvmThemeProvider>(p => p.Add(x => x.UserTheme, true));

        estado.TriggerOnPersisting();

        Assert.True(estado.TryTake<string>("rvm.tema", out var valor));
        Assert.Equal("v1|black|light|default|0|0", valor);
    }

    [Fact]
    public void Servidor_sem_store_de_cookie_le_o_navegador_depois_do_primeiro_render()
    {
        // Blazor Server puro: o store nao alcanca o navegador. A escolha volta pelo rvmTheme.load, com um
        // redesenho (achado P2).
        Services.AddRvmDesignSystem();
        Services.AddRvmTheme<StoreVazio>();
        JSInterop.Setup<string?>("rvmTheme.load", "rvm.tema").SetResult("v1|utilitario|dark|default|0|0");

        var cortado = Render<RvmThemeProvider>(p => p.Add(x => x.UserTheme, true));

        cortado.WaitForAssertion(() =>
        {
            Assert.Equal("utilitario", cortado.Find("div").GetAttribute("data-rvm-accent"));
            Assert.Equal("dark", cortado.Find("div").GetAttribute("data-theme"));
        });
    }

    [Fact]
    public void Previa_aninhada_com_fonte_grande_nao_redimensiona_a_pagina()
    {
        var cortado = Render<RvmThemeProvider>(p => p
            .Add(x => x.Settings, new RvmThemeSettings(FontScale: RvmFontScale.Small))
            .AddChildContent<RvmThemeProvider>(f => f
                .Add(x => x.Settings, new RvmThemeSettings(FontScale: RvmFontScale.ExtraLarge))
                .Add(x => x.Class, "previa")));

        Assert.Equal("small", cortado.Find("div").GetAttribute("data-rvm-font-scale"));
        Assert.Null(cortado.Find("div.previa").GetAttribute("data-rvm-font-scale"));
    }

    [Fact]
    public void Blocos_novos_nao_passam_de_um_atributo_de_especificidade()
    {
        // Quem rebrandeia o escuro com [data-theme='dark'] { } (0,1,0) no proprio CSS continua vencendo (achado P2).
        Assert.DoesNotMatch(@"\[data-theme='[a-z]+'\]\[data-rvm-(accent|contrast)", Css);
        Assert.Contains("[data-theme='dark']:where([data-rvm-accent=\"blue\"])", Css, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Alternar_no_automatico_parte_do_tema_que_esta_na_tela()
    {
        Services.AddRvmDesignSystem();
        JSInterop.Setup<string?>("rvmTheme.load", "rvm.tema").SetResult("v1|blue|system|default|0|0");
        JSInterop.Setup<bool>("rvmTheme.prefersDark").SetResult(true);
        var cortado = Render<RvmThemeProvider>(p => p.Add(x => x.UserTheme, true));

        await cortado.InvokeAsync(cortado.Instance.ToggleAsync);

        Assert.Equal(RvmThemeMode.Light, Services.GetRequiredService<RvmThemeState>().Current.Mode);
    }

    private sealed class StoreVazio : IRvmThemeStore
    {
        public ValueTask<RvmThemeSettings?> LoadAsync(CancellationToken cancellationToken = default) =>
            ValueTask.FromResult<RvmThemeSettings?>(null);

        public ValueTask SaveAsync(RvmThemeSettings settings, CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;
    }
}
