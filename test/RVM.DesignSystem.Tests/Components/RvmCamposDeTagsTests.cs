using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RVM.DesignSystem.Components.Chip;
using RVM.DesignSystem.Components.TextField;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>RvmTagOption, RvmMultiTextField e RvmTextFieldSelect, do contrato com o RVM.UI (DSGN-017).</summary>
public class RvmCamposDeTagsTests : BunitContext
{
    public RvmCamposDeTagsTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IReadOnlyList<string> _tags = [];

    private IRenderedComponent<RvmMultiTextField> RenderMulti(string? name = "culturas", bool disabled = false)
        => Render<RvmMultiTextField>(p => p
            .Add(x => x.Label, "Culturas")
            .Add(x => x.Name, name)
            .Add(x => x.Disabled, disabled)
            .Add(x => x.Value, _tags)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => _tags = v))
            .Add(x => x.ValueExpression, () => _tags));

    // --- RvmTagOption ---

    [Fact]
    public void Tag_sem_OnRemove_nao_tem_botao_e_e_suave_por_padrao()
    {
        var cortado = Render<RvmTagOption>(p => p.Add(x => x.Text, "Soja").Add(x => x.Class, "minha"));

        var raiz = cortado.Find(".rvm-tag");
        Assert.Contains("rvm-suave", raiz.ClassList);
        Assert.Contains("minha", raiz.ClassList);
        Assert.Equal("Soja", cortado.Find(".rvm-texto").TextContent);
        Assert.Empty(cortado.FindAll("button"));
    }

    [Fact]
    public void Tag_com_OnRemove_tem_botao_nomeado_que_avisa()
    {
        var removida = false;
        var cortado = Render<RvmTagOption>(p => p
            .Add(x => x.Text, "Soja")
            .Add(x => x.Variant, RvmTagVariant.Solid)
            .Add(x => x.OnRemove, () => removida = true));

        Assert.Contains("rvm-solida", cortado.Find(".rvm-tag").ClassList);
        var botao = cortado.Find("button");
        Assert.Equal("Remover Soja", botao.GetAttribute("aria-label"));
        Assert.Equal("button", botao.GetAttribute("type"));
        botao.Click();
        Assert.True(removida);
    }

    [Fact]
    public void Tag_desabilitada_desabilita_o_botao()
    {
        var cortado = Render<RvmTagOption>(p => p
            .Add(x => x.Text, "Soja")
            .Add(x => x.Disabled, true)
            .Add(x => x.OnRemove, () => { }));

        Assert.Contains("rvm-desabilitado", cortado.Find(".rvm-tag").ClassList);
        Assert.Equal("true", cortado.Find(".rvm-tag").GetAttribute("aria-disabled"));
        Assert.True(cortado.Find("button").HasAttribute("disabled"));
    }

    // --- RvmMultiTextField ---

    [Fact]
    public void Enter_inclui_o_texto_digitado_e_limpa_o_input()
    {
        var cortado = RenderMulti();

        cortado.Find("input.rvm-entrada").Input("  Soja ");
        cortado.Find("input.rvm-entrada").KeyDown(new KeyboardEventArgs { Key = "Enter" });

        Assert.Equal(["Soja"], _tags);
        Assert.Equal("", cortado.Find("input.rvm-entrada").GetAttribute("value"));
        Assert.Contains("rvm-com-tags", cortado.Find(".rvm-campo").ClassList);
        Assert.Contains("rvm-rotulo-fixo", cortado.Find(".rvm-campo").ClassList);
    }

    [Fact]
    public void Virgula_inclui_cada_parte_e_deixa_o_resto_no_input()
    {
        var cortado = RenderMulti();

        cortado.Find("input.rvm-entrada").Input("Soja,Milho,Tri");

        Assert.Equal(["Soja", "Milho"], _tags);
        Assert.Equal("Tri", cortado.Find("input.rvm-entrada").GetAttribute("value"));
    }

    [Fact]
    public void Nao_duplica_nem_inclui_vazio()
    {
        _tags = ["Soja"];
        var cortado = RenderMulti();

        cortado.Find("input.rvm-entrada").Input("soja, ,");
        cortado.Find("input.rvm-entrada").KeyDown(new KeyboardEventArgs { Key = "Enter" });

        Assert.Equal(["Soja"], _tags);
    }

    [Fact]
    public void Backspace_com_input_vazio_remove_a_ultima_e_com_texto_nao()
    {
        _tags = ["Soja", "Milho"];
        var cortado = RenderMulti();

        cortado.Find("input.rvm-entrada").Input("x");
        cortado.Find("input.rvm-entrada").KeyDown(new KeyboardEventArgs { Key = "Backspace" });
        Assert.Equal(["Soja", "Milho"], _tags);

        cortado.Find("input.rvm-entrada").Input("");
        cortado.Find("input.rvm-entrada").KeyDown(new KeyboardEventArgs { Key = "Backspace" });
        Assert.Equal(["Soja"], _tags);
    }

    [Fact]
    public void O_x_remove_a_tag_certa_e_anuncia()
    {
        _tags = ["Soja", "Milho", "Trigo"];
        var cortado = RenderMulti();

        cortado.Find("button[aria-label='Remover Milho']").Click();

        Assert.Equal(["Soja", "Trigo"], _tags);
        var aviso = cortado.Find("[aria-live='polite']");
        Assert.Equal("Removido: Milho", aviso.TextContent);
    }

    [Fact]
    public void Inclusao_e_anunciada()
    {
        var cortado = RenderMulti();

        cortado.Find("input.rvm-entrada").Input("Soja,");

        Assert.Equal("Incluido: Soja", cortado.Find("[aria-live='polite']").TextContent);
    }

    [Fact]
    public void Lista_nomeada_e_um_hidden_com_o_name_por_tag()
    {
        _tags = ["Soja", "Milho"];
        var cortado = RenderMulti();

        Assert.Equal("Itens de Culturas", cortado.Find("ul.rvm-tags").GetAttribute("aria-label"));
        Assert.Equal(2, cortado.FindAll("ul.rvm-tags > li").Count);
        var ocultos = cortado.FindAll("input[type='hidden']");
        Assert.Equal(["Soja", "Milho"], ocultos.Select(o => o.GetAttribute("value")));
        Assert.All(ocultos, o => Assert.Equal("culturas", o.GetAttribute("name")));
    }

    [Fact]
    public void Input_de_digitacao_fica_fora_do_formulario_e_sem_name()
    {
        var cortado = RenderMulti();

        var entrada = cortado.Find("input.rvm-entrada");
        var form = entrada.GetAttribute("form");
        Assert.False(string.IsNullOrWhiteSpace(form));
        Assert.Empty(cortado.FindAll($"form#{form}"));
        Assert.False(entrada.HasAttribute("name"));
        Assert.Equal(entrada.GetAttribute("id"), cortado.Find("label").GetAttribute("for"));
    }

    [Fact]
    public void Desabilitado_nao_inclui_e_desabilita_as_tags()
    {
        _tags = ["Soja"];
        var cortado = RenderMulti(disabled: true);

        Assert.True(cortado.Find("input.rvm-entrada").HasAttribute("disabled"));
        Assert.True(cortado.Find("button[aria-label='Remover Soja']").HasAttribute("disabled"));
        cortado.Find("input.rvm-entrada").KeyDown(new KeyboardEventArgs { Key = "Backspace" });
        Assert.Equal(["Soja"], _tags);
    }

    // --- RvmTextFieldSelect ---

    private static readonly string[] Moedas = ["BRL", "USD"];

    [Fact]
    public void Campo_com_unidade_troca_a_unidade_e_mantem_o_texto()
    {
        string? valor = "10";
        string? moeda = "BRL";
        var cortado = Render<RvmTextFieldSelect<string>>(p => p
            .Add(x => x.Label, "Preco")
            .Add(x => x.Name, "preco")
            .Add(x => x.OptionName, "moeda")
            .Add(x => x.Options, Moedas)
            .Add(x => x.Option, moeda)
            .Add(x => x.OptionChanged, EventCallback.Factory.Create<string>(this, v => moeda = v))
            .Add(x => x.Value, valor)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => valor = v))
            .Add(x => x.ValueExpression, () => valor));

        var select = cortado.Find("select");
        Assert.Equal("Unidade", select.GetAttribute("aria-label"));
        Assert.Equal("moeda", select.GetAttribute("name"));
        Assert.True(cortado.Find("option[value='BRL']").HasAttribute("selected"));

        select.Change("USD");
        Assert.Equal("USD", moeda);
        Assert.True(cortado.Find("option[value='USD']").HasAttribute("selected"));

        var input = cortado.Find("input.rvm-entrada");
        Assert.Equal("10", input.GetAttribute("value"));
        Assert.Equal("preco", input.GetAttribute("name"));
        input.Change("12,50");
        Assert.Equal("12,50", valor);
    }

    [Fact]
    public void Campo_com_unidade_usa_OptionText_e_OptionLabel()
    {
        string? valor = null;
        var cortado = Render<RvmTextFieldSelect<int>>(p => p
            .Add(x => x.ValueExpression, () => valor)
            .Add(x => x.Options, [1, 1000])
            .Add(x => x.OptionText, n => n == 1 ? "kg" : "t")
            .Add(x => x.OptionLabel, "Unidade de peso")
            .Add(x => x.Disabled, true));

        Assert.Equal(["kg", "t"], cortado.FindAll("option").Select(o => o.TextContent));
        Assert.Equal("Unidade de peso", cortado.Find("select").GetAttribute("aria-label"));
        Assert.True(cortado.Find("select").HasAttribute("disabled"));
    }

    [Fact]
    public void Re_render_do_pai_sem_mudar_Option_nao_desfaz_a_escolha()
    {
        string? valor = null;
        var cortado = Render<RvmTextFieldSelect<string>>(p => p
            .Add(x => x.ValueExpression, () => valor)
            .Add(x => x.Options, Moedas)
            .Add(x => x.Option, "BRL"));

        cortado.Find("select").Change("USD");
        cortado.Render(p => p.Add(x => x.Label, "Preco"));

        Assert.True(cortado.Find("option[value='USD']").HasAttribute("selected"));
    }
}
