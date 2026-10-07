using Bunit;
using Microsoft.AspNetCore.Components.Web;
using RVM.DesignSystem.Components.IconSelector;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>O RvmIconSelector do contrato com o RVM.UI (DSGN-017): circulo, janela com busca e grade com foco itinerante.</summary>
public class RvmIconSelectorTests : BunitContext
{
    private static readonly RvmIconName[] Agro =
        [RvmIconName.Leaf, RvmIconName.Sun, RvmIconName.CloudRain, RvmIconName.Wallet, RvmIconName.Dollar,
         RvmIconName.Scale, RvmIconName.Map, RvmIconName.Calculator, RvmIconName.AltArrowDown];

    private RvmIconName? _valor;
    private int _avisos;

    public RvmIconSelectorTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<RvmIconSelector> Montar(Action<ComponentParameterCollectionBuilder<RvmIconSelector>>? mais = null)
        => Render<RvmIconSelector>(p =>
        {
            p.Add(x => x.Label, "Icone da categoria")
             .Add(x => x.Id, "icone")
             .Add(x => x.Value, _valor)
             .Add(x => x.ValueChanged, v => { _valor = v; _avisos++; })
             .Add(x => x.ValueExpression, () => _valor);
            mais?.Invoke(p);
        });

    private static void Abrir(IRenderedComponent<RvmIconSelector> cortado) => cortado.Find("#icone").Click();

    private static IReadOnlyList<AngleSharp.Dom.IElement> Botoes(IRenderedComponent<RvmIconSelector> cortado)
        => cortado.FindAll(".rvm-grade-de-icones button");

    [Fact]
    public void Vazio_mostra_o_mais_e_o_botao_escolher()
    {
        var cortado = Montar();

        Assert.Equal("Nenhum icone escolhido", cortado.Find(".rvm-circulo").GetAttribute("aria-label"));
        Assert.NotNull(cortado.Find(".rvm-circulo svg"));
        Assert.Contains("Escolher icone", cortado.Find("#icone").TextContent);
        Assert.DoesNotContain("Remover icone", cortado.Markup);
        Assert.Empty(cortado.FindAll("[role=dialog]"));
        Assert.Equal("icone-rotulo", cortado.Find(".rvm-seletor-de-icone").GetAttribute("aria-labelledby"));
        Assert.Equal("dialog", cortado.Find("#icone").GetAttribute("aria-haspopup"));
    }

    [Fact]
    public void Abrir_mostra_a_janela_com_a_busca_e_a_grade_nomeada()
    {
        var cortado = Montar(p => p.Add(x => x.Icons, Agro));

        Abrir(cortado);

        Assert.Equal("Escolher icone", cortado.Find("[role=dialog] h2").TextContent);
        Assert.NotNull(cortado.Find("[role=dialog] input[data-rvm-foco-inicial]"));
        Assert.Equal("Icones", cortado.Find(".rvm-grade-de-icones").GetAttribute("aria-label"));
        Assert.Equal("group", cortado.Find(".rvm-grade-de-icones").GetAttribute("role"));
        Assert.Equal(Agro.Length, Botoes(cortado).Count);
        Assert.Equal("9 icones", cortado.Find(".rvm-aviso-da-grade").TextContent);
    }

    [Fact]
    public void Busca_ignora_acento_e_caixa()
    {
        var cortado = Montar(p => p
            .Add(x => x.Icons, Agro)
            .Add(x => x.IconText, i => i == RvmIconName.Leaf ? "Folha de café" : i.ToString()));
        Abrir(cortado);

        cortado.Find("[role=dialog] input").Input("CAFE");

        var botao = Assert.Single(Botoes(cortado));
        Assert.Equal("Folha de café", botao.GetAttribute("aria-label"));

        cortado.Find("[role=dialog] input").Input("xyz");
        Assert.Empty(Botoes(cortado));
        Assert.Equal("Nenhum icone encontrado. Tente outra palavra.", cortado.Find(".rvm-aviso-da-grade").TextContent);
    }

    [Fact]
    public void Sem_Icons_oferece_o_enum_inteiro_e_mostra_so_MaxVisible()
    {
        var cortado = Montar();
        Abrir(cortado);

        var total = Enum.GetValues<RvmIconName>().Length;
        Assert.Equal(60, Botoes(cortado).Count);
        Assert.Equal($"Mostrando 60 de {total} — refine a busca", cortado.Find(".rvm-aviso-da-grade").TextContent);
        Assert.Equal("status", cortado.Find(".rvm-aviso-da-grade").GetAttribute("role"));
    }

    [Fact]
    public void MaxVisible_limita_a_grade()
    {
        var cortado = Montar(p => p.Add(x => x.Icons, Agro).Add(x => x.MaxVisible, 4));
        Abrir(cortado);

        Assert.Equal(4, Botoes(cortado).Count);
        Assert.Equal("Mostrando 4 de 9 — refine a busca", cortado.Find(".rvm-aviso-da-grade").TextContent);
    }

    [Fact]
    public void Nome_padrao_e_o_enum_em_palavras()
    {
        var cortado = Montar(p => p.Add(x => x.Icons, Agro));
        Abrir(cortado);

        cortado.Find("[role=dialog] input").Input("alt arrow");

        var botao = Assert.Single(Botoes(cortado));
        Assert.Equal("Alt arrow down", botao.GetAttribute("aria-label"));
        Assert.Equal("Alt arrow down", botao.GetAttribute("title"));
    }

    [Fact]
    public void Setas_Home_e_End_movem_o_foco_itinerante()
    {
        var cortado = Montar(p => p.Add(x => x.Icons, Agro));
        Abrir(cortado);

        int Ativo() => Botoes(cortado).Select((b, i) => (b, i)).Single(x => x.b.GetAttribute("tabindex") == "0").i;
        void Tecla(string tecla) => cortado.Find(".rvm-grade-de-icones").KeyDown(new KeyboardEventArgs { Key = tecla });

        Assert.Equal(0, Ativo());
        Assert.Equal(Agro.Length - 1, Botoes(cortado).Count(b => b.GetAttribute("tabindex") == "-1"));

        Tecla("ArrowRight");
        Assert.Equal(1, Ativo());
        Tecla("ArrowDown");
        Assert.Equal(7, Ativo());
        Tecla("ArrowDown");
        Assert.Equal(8, Ativo());
        Tecla("ArrowUp");
        Assert.Equal(2, Ativo());
        Tecla("ArrowLeft");
        Assert.Equal(1, Ativo());
        Tecla("End");
        Assert.Equal(8, Ativo());
        Tecla("Home");
        Assert.Equal(0, Ativo());
        Tecla("ArrowLeft");
        Assert.Equal(0, Ativo());
    }

    [Theory]
    [InlineData("Enter")]
    [InlineData(" ")]
    public void Enter_ou_espaco_escolhem_e_fecham(string tecla)
    {
        var cortado = Montar(p => p.Add(x => x.Icons, Agro));
        Abrir(cortado);

        cortado.Find(".rvm-grade-de-icones").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        cortado.Find(".rvm-grade-de-icones").KeyDown(new KeyboardEventArgs { Key = tecla });

        Assert.Equal(RvmIconName.Sun, _valor);
        Assert.Equal(1, _avisos);
        Assert.Empty(cortado.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Clique_escolhe_e_mostra_o_icone_no_circulo()
    {
        var cortado = Montar(p => p.Add(x => x.Icons, Agro));
        Abrir(cortado);

        Botoes(cortado)[3].Click();
        cortado.Render(p => p.Add(x => x.Value, _valor));

        Assert.Equal(RvmIconName.Wallet, _valor);
        Assert.Equal("Icone escolhido: Wallet", cortado.Find(".rvm-circulo").GetAttribute("aria-label"));
        Assert.Contains("rvm-com-icone", cortado.Find(".rvm-circulo").ClassList);
        Assert.Contains("Trocar icone", cortado.Find("#icone").TextContent);
    }

    [Fact]
    public void Valor_vem_marcado_com_aria_pressed_e_ativo_na_grade()
    {
        _valor = RvmIconName.Scale;
        var cortado = Montar(p => p.Add(x => x.Icons, Agro));
        Abrir(cortado);

        var marcados = Botoes(cortado).Where(b => b.GetAttribute("aria-pressed") == "true").ToList();
        var marcado = Assert.Single(marcados);
        Assert.Equal("Scale", marcado.GetAttribute("aria-label"));
        Assert.Equal("0", marcado.GetAttribute("tabindex"));
        Assert.All(Botoes(cortado).Except(marcados), b => Assert.Equal("false", b.GetAttribute("aria-pressed")));
    }

    [Fact]
    public void Remover_icone_limpa_o_valor()
    {
        _valor = RvmIconName.Leaf;
        var cortado = Montar();

        var remover = cortado.FindAll("button").Single(b => b.TextContent.Contains("Remover icone"));
        remover.Click();

        Assert.Null(_valor);
        Assert.Equal(1, _avisos);
    }

    [Fact]
    public void Name_gera_o_campo_escondido_com_o_nome_do_icone()
    {
        _valor = RvmIconName.Leaf;
        var cortado = Montar(p => p.Add(x => x.Name, "categoria.icone"));

        var escondido = cortado.Find("input[type=hidden]");
        Assert.Equal("categoria.icone", escondido.GetAttribute("name"));
        Assert.Equal("Leaf", escondido.GetAttribute("value"));
    }

    [Fact]
    public void Disabled_desliga_os_botoes_e_nao_abre()
    {
        _valor = RvmIconName.Leaf;
        var cortado = Montar(p => p.Add(x => x.Disabled, true));

        Assert.True(cortado.Find("#icone").HasAttribute("disabled"));
        Assert.All(cortado.FindAll(".rvm-acoes-do-seletor button"), b => Assert.True(b.HasAttribute("disabled")));
        Assert.Contains("rvm-desabilitado", cortado.Find(".rvm-seletor-de-icone").ClassList);
        Assert.Empty(cortado.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Erro_toma_o_lugar_do_apoio_e_e_ligado_ao_botao()
    {
        var cortado = Montar(p => p.Add(x => x.HelperText, "Aparece no menu"));
        Assert.Equal("Aparece no menu", cortado.Find("#icone-apoio").TextContent);
        Assert.Equal("icone-apoio", cortado.Find("#icone").GetAttribute("aria-describedby"));

        cortado.Render(p => p.Add(x => x.ErrorText, "Escolha um icone."));

        Assert.Equal("Escolha um icone.", cortado.Find("#icone-erro").TextContent);
        Assert.Empty(cortado.FindAll("#icone-apoio"));
        Assert.Equal("icone-erro", cortado.Find("#icone").GetAttribute("aria-describedby"));
        Assert.Contains("rvm-erro", cortado.Find(".rvm-seletor-de-icone").ClassList);
    }

    [Fact]
    public void Class_e_atributos_extras_vao_para_a_raiz_e_o_botao()
    {
        var cortado = Montar(p => p
            .Add(x => x.Class, "minha")
            .AddUnmatched("data-teste", "x"));

        Assert.Contains("minha", cortado.Find(".rvm-seletor-de-icone").ClassList);
        Assert.Equal("x", cortado.Find("#icone").GetAttribute("data-teste"));
    }

    [Theory]
    [InlineData("Leaf", true)]
    [InlineData("", true)]
    [InlineData("leaf", false)]
    [InlineData("5", false)]
    [InlineData("Leaf, Sun", false)]
    [InlineData("NaoExiste", false)]
    public void Leitura_do_texto_aceita_so_o_nome_do_enum(string texto, bool valido)
    {
        var seletor = new Exposto();

        var ok = seletor.Ler(texto, out var valor, out var erro);

        Assert.Equal(valido, ok);
        if (valido)
        {
            Assert.Null(erro);
            Assert.Equal(texto.Length == 0 ? null : RvmIconName.Leaf, valor);
        }
        else
        {
            Assert.Contains("nao existe", erro);
        }
    }

    private sealed class Exposto : RvmIconSelector
    {
        public bool Ler(string texto, out RvmIconName? valor, out string? erro)
            => TryParseValueFromString(texto, out valor, out erro);
    }

    [Fact]
    public void Busca_barra_o_Enter_para_nao_enviar_o_formulario_do_consumidor()
    {
        var modulo = JSInterop.SetupModule("./_content/RVM.DesignSystem/rvm-teclado.js");
        modulo.Mode = JSRuntimeMode.Loose;
        var cortado = Montar(p => p.Add(x => x.Icons, Agro));

        Abrir(cortado);

        Assert.Contains(modulo.Invocations["prenderTeclas"],
            i => i.Arguments[1] is string[] teclas && teclas.SequenceEqual(["Enter"]));
    }
}
