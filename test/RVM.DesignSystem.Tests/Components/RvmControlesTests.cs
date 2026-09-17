using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using RVM.DesignSystem;
using RVM.DesignSystem.Components.Checkbox;
using RVM.DesignSystem.Components.Radio;
using RVM.DesignSystem.Components.Switch;

namespace RVM.DesignSystem.Tests.Components;

public class RvmCheckboxTests : BunitContext
{
    private sealed class Preferencias
    {
        public bool Aceito { get; set; }
    }

    public RvmCheckboxTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<RvmCheckbox> Caixa(
        Preferencias modelo,
        Action<ComponentParameterCollectionBuilder<RvmCheckbox>>? extra = null,
        EditContext? contexto = null)
        => Render<RvmCheckbox>(p =>
        {
            if (contexto is not null) p.AddCascadingValue(contexto);
            p.Add(x => x.Value, modelo.Aceito)
             .Add(x => x.ValueChanged, EventCallback.Factory.Create<bool>(this, v => modelo.Aceito = v))
             .Add(x => x.ValueExpression, () => modelo.Aceito);
            extra?.Invoke(p);
        });

    [Fact]
    public void E_um_checkbox_nativo_dentro_de_um_label()
    {
        // Nativo de proposito: funciona sem JS (SSR estatico), traz teclado e semantica, e o <label>
        // em volta faz o texto marcar a caixa e virar o nome acessivel.
        var cortado = Caixa(new Preferencias(), p => p.Add(x => x.Label, "Aceito os termos"));

        var input = cortado.Find("label input[type=checkbox]");
        Assert.Contains("nativo", input.GetAttribute("class"));
        Assert.Equal("Aceito os termos", cortado.Find("label span.rotulo").TextContent);
    }

    [Fact]
    public void Marcar_avisa_quem_esta_ligado()
    {
        var modelo = new Preferencias();
        var cortado = Caixa(modelo);

        cortado.Find("input").Change(true);

        Assert.True(modelo.Aceito);
    }

    [Fact]
    public void Valor_marcado_sai_marcado()
    {
        var cortado = Caixa(new Preferencias { Aceito = true });

        Assert.True(cortado.Find("input").HasAttribute("checked"));
    }

    [Fact]
    public void Gera_name_para_o_POST_em_SSR_estatico()
    {
        var cortado = Caixa(new Preferencias(), contexto: new EditContext(new Preferencias()));

        Assert.EndsWith("Aceito", cortado.Find("input").GetAttribute("name"));
    }

    [Fact]
    public void Indeterminado_sai_no_visual_e_completa_a_semantica_por_js()
    {
        var cortado = Caixa(new Preferencias(), p => p.Add(x => x.Indeterminate, true));

        Assert.Contains("indeterminado", cortado.Find("label").GetAttribute("class"));
        Assert.Contains(JSInterop.Invocations, i => i.Identifier == "import");
    }

    [Fact]
    public void Sem_indeterminado_nao_ha_chamada_de_js()
    {
        var cortado = Caixa(new Preferencias());

        Assert.DoesNotContain("indeterminado", cortado.Find("label").GetAttribute("class"));
        Assert.DoesNotContain(JSInterop.Invocations, i => i.Identifier == "import");
    }

    [Fact]
    public void Desabilitado()
    {
        var cortado = Caixa(new Preferencias(), p => p.Add(x => x.Disabled, true));

        Assert.True(cortado.Find("input").HasAttribute("disabled"));
        Assert.Contains("desabilitado", cortado.Find("label").GetAttribute("class"));
    }

    [Theory]
    [InlineData(RvmSize.Small, RvmColor.Error, "pequeno error")]
    [InlineData(RvmSize.Large, RvmColor.Success, "grande success")]
    public void Tamanho_e_cor_viram_classe(RvmSize tamanho, RvmColor cor, string esperado)
    {
        var cortado = Caixa(new Preferencias(), p => p.Add(x => x.Size, tamanho).Add(x => x.Color, cor));

        Assert.Contains(esperado, cortado.Find("label").GetAttribute("class"));
    }

    [Fact]
    public void Class_e_style_na_raiz_o_resto_no_input()
    {
        var cortado = Caixa(new Preferencias(), p => p
            .AddUnmatched("class", "minha")
            .AddUnmatched("style", "margin: 0;")
            .AddUnmatched("aria-describedby", "ajuda"));

        var raiz = cortado.Find("label");
        Assert.Contains("minha", raiz.GetAttribute("class"));
        Assert.Equal("margin: 0;", raiz.GetAttribute("style"));
        Assert.Equal("ajuda", cortado.Find("input").GetAttribute("aria-describedby"));
    }

    [Fact]
    public void Funciona_sem_bind_so_para_exibir()
    {
        // `<RvmCheckbox Value="true" Disabled="true" />` sem @bind: o InputCheckbox exige uma
        // expressao, e sem a alternativa o controle estourava e sumia da tela.
        var cortado = Render<RvmCheckbox>(p => p
            .Add(x => x.Value, true)
            .Add(x => x.Disabled, true)
            .Add(x => x.Label, "Marcado desabilitado"));

        var input = cortado.Find("input[type=checkbox]");
        Assert.True(input.HasAttribute("checked"));
        Assert.Equal("true", cortado.Find("label").GetAttribute("aria-disabled"));
    }

    [Fact]
    public void Funciona_com_Value_e_ValueChanged_sem_bind()
    {
        var recebido = false;
        var cortado = Render<RvmCheckbox>(p => p
            .Add(x => x.Value, false)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<bool>(this, v => recebido = v)));

        cortado.Find("input").Change(true);

        Assert.True(recebido);
    }

    [Fact]
    public void Rotulo_livre_quando_nao_ha_label()
    {
        var cortado = Caixa(new Preferencias(), p => p.AddChildContent("<a href=\"#\">termos</a>"));

        Assert.Equal("termos", cortado.Find("label a").TextContent);
    }
}

public class RvmSwitchTests : BunitContext
{
    private sealed class Ajustes
    {
        public bool Notificar { get; set; }
    }

    private IRenderedComponent<RvmSwitch> Chave(Ajustes modelo, Action<ComponentParameterCollectionBuilder<RvmSwitch>>? extra = null)
        => Render<RvmSwitch>(p =>
        {
            p.Add(x => x.Value, modelo.Notificar)
             .Add(x => x.ValueChanged, EventCallback.Factory.Create<bool>(this, v => modelo.Notificar = v))
             .Add(x => x.ValueExpression, () => modelo.Notificar);
            extra?.Invoke(p);
        });

    [Fact]
    public void Checkbox_nativo_com_papel_de_switch()
    {
        // O leitor de tela anuncia "ligado/desligado", que e o que o controle significa.
        var cortado = Chave(new Ajustes(), p => p.Add(x => x.Label, "Notificacoes por e-mail"));

        var input = cortado.Find("input[type=checkbox]");
        Assert.Equal("switch", input.GetAttribute("role"));
        Assert.Equal("Notificacoes por e-mail", cortado.Find("span.rotulo").TextContent);
    }

    [Fact]
    public void Ligar_avisa_quem_esta_ligado()
    {
        var modelo = new Ajustes();
        var cortado = Chave(modelo);

        cortado.Find("input").Change(true);

        Assert.True(modelo.Notificar);
    }

    [Theory]
    [InlineData(RvmSize.Small, "pequeno")]
    [InlineData(RvmSize.Medium, "medio")]
    [InlineData(RvmSize.Large, "medio")]
    public void O_kit_so_tem_dois_tamanhos(RvmSize tamanho, string esperado)
    {
        var cortado = Chave(new Ajustes(), p => p.Add(x => x.Size, tamanho));

        Assert.Contains(esperado, cortado.Find("label").GetAttribute("class"));
    }

    [Fact]
    public void Desabilitado()
    {
        var cortado = Chave(new Ajustes(), p => p.Add(x => x.Disabled, true));

        Assert.True(cortado.Find("input").HasAttribute("disabled"));
        Assert.Contains("desabilitado", cortado.Find("label").GetAttribute("class"));
    }

    [Fact]
    public void Funciona_sem_bind()
    {
        var cortado = Render<RvmSwitch>(p => p.Add(x => x.Value, true).Add(x => x.Disabled, true));

        Assert.True(cortado.Find("input").HasAttribute("checked"));
        Assert.Equal("true", cortado.Find("label").GetAttribute("aria-disabled"));
    }

    [Fact]
    public void Class_e_style_na_raiz_o_resto_no_input()
    {
        var cortado = Chave(new Ajustes(), p => p
            .AddUnmatched("style", "margin: 0;")
            .AddUnmatched("data-teste", "1"));

        Assert.Equal("margin: 0;", cortado.Find("label").GetAttribute("style"));
        Assert.Equal("1", cortado.Find("input").GetAttribute("data-teste"));
    }
}

public class RvmRadioTests : BunitContext
{
    public enum Pagamento { Pix, Boleto, Cartao }

    private sealed class Pedido
    {
        public Pagamento Forma { get; set; } = Pagamento.Pix;
    }

    private IRenderedComponent<RvmRadioGroup<Pagamento>> Grupo(
        Pedido modelo,
        Action<ComponentParameterCollectionBuilder<RvmRadioGroup<Pagamento>>>? extra = null,
        EditContext? contexto = null)
        => Render<RvmRadioGroup<Pagamento>>(p =>
        {
            if (contexto is not null) p.AddCascadingValue(contexto);
            p.Add(x => x.Value, modelo.Forma)
             .Add(x => x.ValueChanged, EventCallback.Factory.Create<Pagamento>(this, v => modelo.Forma = v))
             .Add(x => x.ValueExpression, () => modelo.Forma)
             .Add(x => x.Label, "Forma de pagamento")
             .AddChildContent<RvmRadio<Pagamento>>(r => r.Add(x => x.Value, Pagamento.Pix).Add(x => x.Label, "Pix"))
             .AddChildContent<RvmRadio<Pagamento>>(r => r.Add(x => x.Value, Pagamento.Boleto).Add(x => x.Label, "Boleto"));
            extra?.Invoke(p);
        });

    [Fact]
    public void Grupo_e_um_fieldset_com_a_pergunta_na_legenda()
    {
        var cortado = Grupo(new Pedido());

        Assert.Equal("Forma de pagamento", cortado.Find("fieldset > legend").TextContent);
        Assert.Equal(2, cortado.FindAll("fieldset input[type=radio]").Count);
    }

    [Fact]
    public void A_opcao_do_valor_atual_sai_marcada()
    {
        var cortado = Grupo(new Pedido { Forma = Pagamento.Boleto });

        var marcados = cortado.FindAll("input[type=radio]").Where(i => i.HasAttribute("checked")).ToList();
        Assert.Single(marcados);
        Assert.Contains("Boleto", marcados[0].ParentElement!.ParentElement!.TextContent);
    }

    [Fact]
    public void Escolher_uma_opcao_avisa_quem_esta_ligado()
    {
        var modelo = new Pedido();
        var cortado = Grupo(modelo);

        cortado.FindAll("input[type=radio]")[1].Change("Boleto");

        Assert.Equal(Pagamento.Boleto, modelo.Forma);
    }

    [Fact]
    public void Todas_as_opcoes_dividem_o_mesmo_name()
    {
        var cortado = Grupo(new Pedido(), contexto: new EditContext(new Pedido()));

        var nomes = cortado.FindAll("input[type=radio]").Select(i => i.GetAttribute("name")).Distinct().ToList();
        Assert.Single(nomes);
        Assert.False(string.IsNullOrWhiteSpace(nomes[0]));
    }

    [Fact]
    public void Desabilitar_o_grupo_desabilita_o_fieldset_e_marca_as_opcoes()
    {
        var cortado = Grupo(new Pedido(), p => p.Add(x => x.Disabled, true));

        Assert.True(cortado.Find("fieldset").HasAttribute("disabled"));
        Assert.All(cortado.FindAll("label"), l => Assert.Contains("desabilitado", l.GetAttribute("class")));
    }

    [Fact]
    public void Cor_e_tamanho_do_grupo_descem_para_as_opcoes()
    {
        var cortado = Grupo(new Pedido(), p => p.Add(x => x.Color, RvmColor.Success).Add(x => x.Size, RvmSize.Small));

        Assert.All(cortado.FindAll("label"), l =>
        {
            Assert.Contains("success", l.GetAttribute("class"));
            Assert.Contains("pequeno", l.GetAttribute("class"));
        });
    }

    [Fact]
    public void Orientacao_horizontal_vira_classe()
    {
        var cortado = Grupo(new Pedido(), p => p.Add(x => x.Orientation, RvmOrientation.Horizontal));

        Assert.Contains("horizontal", cortado.Find("fieldset").GetAttribute("class"));
    }

    [Fact]
    public void Grupo_funciona_com_Value_e_ValueChanged_sem_bind()
    {
        var recebido = Pagamento.Pix;
        var cortado = Render<RvmRadioGroup<Pagamento>>(p => p
            .Add(x => x.Value, Pagamento.Pix)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<Pagamento>(this, v => recebido = v))
            .AddChildContent<RvmRadio<Pagamento>>(r => r.Add(x => x.Value, Pagamento.Pix).Add(x => x.Label, "Pix"))
            .AddChildContent<RvmRadio<Pagamento>>(r => r.Add(x => x.Value, Pagamento.Boleto).Add(x => x.Label, "Boleto")));

        cortado.FindAll("input[type=radio]")[1].Change("Boleto");

        Assert.Equal(Pagamento.Boleto, recebido);
    }

    [Fact]
    public void Opcao_fora_de_um_grupo_explica_o_erro_em_portugues()
    {
        var erro = Assert.Throws<InvalidOperationException>(() =>
            Render<RvmRadio<Pagamento>>(p => p.Add(x => x.Value, Pagamento.Pix)));

        Assert.Contains("precisa estar dentro de um RvmRadioGroup", erro.Message);
    }
}
