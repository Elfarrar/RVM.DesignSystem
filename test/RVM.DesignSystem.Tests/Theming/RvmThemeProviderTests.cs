using Bunit;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Theming;

namespace RVM.DesignSystem.Tests.Theming;

/// <summary>
/// O provider e a unica peca da tematizacao com codigo. O que estes testes seguram: o tema sai
/// certo na PRIMEIRA renderizacao (sem JS), a troca avisa quem esta ligado nela, e a ausencia de
/// JS nao derruba nada — as tres coisas que a regra dos dois modos de hospedagem exige.
/// </summary>
public class RvmThemeProviderTests : BunitContext
{
    public RvmThemeProviderTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Sem_parametro_nasce_no_tema_claro_e_com_a_classe_raiz()
    {
        var cortado = Render<RvmThemeProvider>();

        var raiz = cortado.Find("div");
        Assert.Equal("light", raiz.GetAttribute("data-theme"));
        Assert.Equal("rvm-root", raiz.GetAttribute("class"));
    }

    [Fact]
    public void Tema_escuro_sai_no_primeiro_render_sem_depender_de_js()
    {
        var cortado = Render<RvmThemeProvider>(p => p.Add(x => x.Theme, RvmTheme.Dark));

        Assert.Equal("dark", cortado.Find("div").GetAttribute("data-theme"));
    }

    [Fact]
    public void Classe_do_consumidor_soma_com_a_raiz_em_vez_de_substituir()
    {
        var cortado = Render<RvmThemeProvider>(p => p
            .Add(x => x.Theme, RvmTheme.Light)
            .AddUnmatched("class", "pagina"));

        Assert.Equal("rvm-root pagina", cortado.Find("div").GetAttribute("class"));
    }

    [Fact]
    public void Atributos_extras_chegam_ao_elemento_raiz()
    {
        var cortado = Render<RvmThemeProvider>(p => p
            .AddUnmatched("id", "raiz-do-app")
            .AddUnmatched("data-teste", "1"));

        var raiz = cortado.Find("div");
        Assert.Equal("raiz-do-app", raiz.GetAttribute("id"));
        Assert.Equal("1", raiz.GetAttribute("data-teste"));
    }

    [Fact]
    public void Conteudo_filho_e_renderizado_dentro_da_raiz()
    {
        var cortado = Render<RvmThemeProvider>(p => p
            .AddChildContent("<p>oi</p>"));

        Assert.Equal("oi", cortado.Find("div > p").TextContent);
    }

    [Fact]
    public async Task Toggle_alterna_o_tema_e_avisa_quem_esta_ligado()
    {
        var avisados = new List<RvmTheme>();
        var cortado = Render<RvmThemeProvider>(p => p
            .Add(x => x.Theme, RvmTheme.Light)
            .Add(x => x.ThemeChanged, EventCallback.Factory.Create<RvmTheme>(this, t => avisados.Add(t))));

        await cortado.Instance.ToggleAsync();

        Assert.Equal([RvmTheme.Dark], avisados);
        Assert.Equal("dark", cortado.Find("div").GetAttribute("data-theme"));

        await cortado.Instance.ToggleAsync();

        Assert.Equal([RvmTheme.Dark, RvmTheme.Light], avisados);
    }

    [Fact]
    public async Task Definir_o_tema_que_ja_esta_em_vigor_nao_avisa_ninguem()
    {
        var avisos = 0;
        var cortado = Render<RvmThemeProvider>(p => p
            .Add(x => x.Theme, RvmTheme.Light)
            .Add(x => x.ThemeChanged, EventCallback.Factory.Create<RvmTheme>(this, _ => avisos++)));

        await cortado.Instance.SetThemeAsync(RvmTheme.Light);

        Assert.Equal(0, avisos);
    }

    [Fact]
    public void Preferencia_guardada_no_navegador_e_restaurada_no_primeiro_render()
    {
        JSInterop.Setup<string?>("rvmTheme.read").SetResult("dark");

        var cortado = Render<RvmThemeProvider>();

        Assert.Equal(RvmTheme.Dark, cortado.Instance.Theme);
        Assert.Equal("dark", cortado.Find("div").GetAttribute("data-theme"));
    }

    [Fact]
    public void Com_persistencia_desligada_a_preferencia_guardada_e_ignorada()
    {
        JSInterop.Setup<string?>("rvmTheme.read").SetResult("dark");

        var cortado = Render<RvmThemeProvider>(p => p.Add(x => x.Persist, false));

        Assert.Equal(RvmTheme.Light, cortado.Instance.Theme);
    }

    [Fact]
    public void O_documento_recebe_o_tema_em_vigor()
    {
        var cortado = Render<RvmThemeProvider>(p => p.Add(x => x.Theme, RvmTheme.Dark));

        var chamada = Assert.Single(JSInterop.Invocations["rvmTheme.apply"]);
        Assert.Equal("dark", chamada.Arguments[0]);
        Assert.Equal(true, chamada.Arguments[1]);
    }

    [Fact]
    public async Task Troca_por_binding_tambem_chega_ao_documento()
    {
        var cortado = Render<RvmThemeProvider>(p => p.Add(x => x.Theme, RvmTheme.Light));

        cortado.Render(p => p.Add(x => x.Theme, RvmTheme.Dark));
        await Task.Yield();

        var temas = JSInterop.Invocations["rvmTheme.apply"].Select(i => i.Arguments[0]).ToArray();
        Assert.Equal(["light", "dark"], temas);
    }

    [Fact]
    public void Sem_js_disponivel_o_componente_continua_de_pe()
    {
        // Pre-renderizacao no Blazor Server levanta InvalidOperationException em qualquer chamada
        // de interop. O tema tem de sair certo assim mesmo, porque vem do atributo, nao do JS.
        JSInterop.Setup<string?>("rvmTheme.read")
            .SetException(new InvalidOperationException("JavaScript interop calls cannot be issued during server-side prerendering"));
        JSInterop.SetupVoid("rvmTheme.apply", _ => true)
            .SetException(new InvalidOperationException("JavaScript interop calls cannot be issued during server-side prerendering"));

        var cortado = Render<RvmThemeProvider>(p => p.Add(x => x.Theme, RvmTheme.Dark));

        Assert.Equal("dark", cortado.Find("div").GetAttribute("data-theme"));
    }

    [Fact]
    public void Valor_desconhecido_no_armazenamento_nao_muda_o_tema()
    {
        JSInterop.Setup<string?>("rvmTheme.read").SetResult("azul-turquesa");

        var cortado = Render<RvmThemeProvider>(p => p.Add(x => x.Theme, RvmTheme.Light));

        Assert.Equal(RvmTheme.Light, cortado.Instance.Theme);
    }
}
