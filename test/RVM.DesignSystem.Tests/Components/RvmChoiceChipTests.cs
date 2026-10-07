using Bunit;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Chip;
using RVM.DesignSystem.Components.Radio;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>O RvmChoiceChip do contrato com o RVM.UI (DSGN-017): radio nativo com cara de chip.</summary>
public class RvmChoiceChipTests : BunitContext
{
    public RvmChoiceChipTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<RvmRadioGroup<string>> Grupo(
        string? valor, Action<string?>? aoMudar, Action<ComponentParameterCollectionBuilder<RvmChoiceChip<string>>> chip,
        bool grupoDesabilitado = false)
        => Render<RvmRadioGroup<string>>(p => p
            .Add(x => x.Value, valor)
            .Add(x => x.ValueChanged, (string? v) => aoMudar?.Invoke(v))
            .Add(x => x.Name, "prioridade")
            .Add(x => x.Disabled, grupoDesabilitado)
            .AddChildContent<RvmChoiceChip<string>>(chip));

    [Fact]
    public void Escolher_pelo_clique_muda_o_valor_do_grupo()
    {
        string? escolhido = null;
        var cortado = Render<RvmRadioGroup<string>>(p => p
            .Add(x => x.Value, "Baixa")
            .Add(x => x.ValueChanged, (string? v) => escolhido = v)
            .AddChildContent(b =>
            {
                b.OpenComponent<RvmChoiceChip<string>>(0);
                b.AddAttribute(1, "Value", "Baixa");
                b.AddAttribute(2, "Label", "Baixa");
                b.CloseComponent();
                b.OpenComponent<RvmChoiceChip<string>>(3);
                b.AddAttribute(4, "Value", "Alta");
                b.AddAttribute(5, "Label", "Alta");
                b.CloseComponent();
            }));

        cortado.FindAll("input[type=radio]")[1].Change("Alta");

        Assert.Equal("Alta", escolhido);
    }

    [Fact]
    public void Input_e_radio_nativo_com_o_name_do_grupo_dentro_do_label()
    {
        var cortado = Grupo("Baixa", null, c => c.Add(x => x.Value, "Baixa").Add(x => x.Label, "Baixa"));

        var input = cortado.Find("label.rvm-escolha input[type=radio]");
        Assert.Equal("prioridade", input.GetAttribute("name"));
        Assert.Equal("Baixa", cortado.Find("label.rvm-escolha .rvm-rotulo").TextContent.Trim());
    }

    [Fact]
    public void ChildContent_vence_o_Label()
    {
        var cortado = Grupo(null, null, c => c.Add(x => x.Value, "A").Add(x => x.Label, "Texto")
            .AddChildContent("<strong>Livre</strong>"));

        var rotulo = cortado.Find(".rvm-rotulo");
        Assert.Equal("Livre", rotulo.TextContent.Trim());
        Assert.NotNull(rotulo.QuerySelector("strong"));
    }

    [Fact]
    public void Marcado_quando_o_valor_do_grupo_e_o_dele()
    {
        var marcado = Grupo("A", null, c => c.Add(x => x.Value, "A").Add(x => x.Label, "A"));
        var desmarcado = Grupo("B", null, c => c.Add(x => x.Value, "A").Add(x => x.Label, "A"));

        Assert.True(marcado.Find("input[type=radio]").HasAttribute("checked"));
        Assert.False(desmarcado.Find("input[type=radio]").HasAttribute("checked"));
    }

    [Fact]
    public void Color_Variant_Size_e_Class_viram_classes_da_raiz()
    {
        var cortado = Grupo(null, null, c => c.Add(x => x.Value, "A").Add(x => x.Label, "A")
            .Add(x => x.Color, RvmColor.Error).Add(x => x.Variant, RvmChipVariant.Soft)
            .Add(x => x.Size, RvmSize.Small).Add(x => x.Class, "minha"));

        var raiz = cortado.Find(".rvm-chip-escolha");
        Assert.Equal("SPAN", raiz.TagName);
        Assert.Contains("rvm-error", raiz.ClassList);
        Assert.Contains("rvm-suave", raiz.ClassList);
        Assert.Contains("rvm-pequeno", raiz.ClassList);
        Assert.Contains("minha", raiz.ClassList);
    }

    [Fact]
    public void Sem_Color_e_primario_preenchido_medio_e_Large_igual_ao_medio()
    {
        var cortado = Grupo(null, null, c => c.Add(x => x.Value, "A").Add(x => x.Label, "A").Add(x => x.Size, RvmSize.Large));

        var raiz = cortado.Find(".rvm-chip-escolha");
        Assert.Contains("rvm-primary", raiz.ClassList);
        Assert.Contains("rvm-preenchido", raiz.ClassList);
        Assert.Contains("rvm-medio", raiz.ClassList);
    }

    [Fact]
    public void StartIcon_aparece_e_AvatarSrc_vence_o_icone()
    {
        var icone = Grupo(null, null, c => c.Add(x => x.Value, "A").Add(x => x.Label, "A").Add(x => x.StartIcon, RvmIconName.Flag));
        Assert.NotNull(icone.Find(".rvm-escolha svg"));

        var foto = Grupo(null, null, c => c.Add(x => x.Value, "A").Add(x => x.Label, "Ana")
            .Add(x => x.StartIcon, RvmIconName.Flag).Add(x => x.AvatarSrc, "ana.png"));
        var img = foto.Find(".rvm-escolha img.rvm-miniatura");
        Assert.Equal("ana.png", img.GetAttribute("src"));
        Assert.Equal(string.Empty, img.GetAttribute("alt"));
        Assert.Empty(foto.FindAll(".rvm-escolha svg"));
    }

    [Fact]
    public void OnRemove_mostra_botao_nomeado_fora_do_label_e_dispara()
    {
        var removidos = 0;
        var cortado = Grupo(null, null, c => c.Add(x => x.Value, "A").Add(x => x.Label, "A")
            .Add(x => x.RemoveLabel, "Remover A").Add(x => x.OnRemove, EventCallback.Factory.Create(this, () => removidos++)));

        var botao = cortado.Find(".rvm-chip-escolha > button.rvm-remover");
        Assert.Equal("Remover A", botao.GetAttribute("aria-label"));
        Assert.Empty(cortado.FindAll("label button"));

        botao.Click();
        Assert.Equal(1, removidos);
    }

    [Fact]
    public void Sem_OnRemove_nao_ha_botao_e_o_nome_padrao_e_Remover()
    {
        Assert.Empty(Grupo(null, null, c => c.Add(x => x.Value, "A").Add(x => x.Label, "A")).FindAll("button"));

        var padrao = Grupo(null, null, c => c.Add(x => x.Value, "A").Add(x => x.Label, "A").Add(x => x.OnRemove, EventCallback.Factory.Create(this, () => { })));
        Assert.Equal("Remover", padrao.Find("button").GetAttribute("aria-label"));
    }

    [Fact]
    public void Disabled_desabilita_o_radio_e_o_botao()
    {
        var cortado = Grupo(null, null, c => c.Add(x => x.Value, "A").Add(x => x.Label, "A").Add(x => x.Disabled, true)
            .Add(x => x.OnRemove, EventCallback.Factory.Create(this, () => { })));

        Assert.True(cortado.Find("input[type=radio]").HasAttribute("disabled"));
        Assert.True(cortado.Find("button.rvm-remover").HasAttribute("disabled"));
        Assert.Contains("rvm-desabilitado", cortado.Find(".rvm-chip-escolha").ClassList);
    }

    [Fact]
    public void Grupo_desabilitado_desabilita_o_botao_e_esmaece_o_chip()
    {
        var cortado = Grupo(null, null, c => c.Add(x => x.Value, "A").Add(x => x.Label, "A")
            .Add(x => x.OnRemove, EventCallback.Factory.Create(this, () => { })), grupoDesabilitado: true);

        Assert.True(cortado.Find("button.rvm-remover").HasAttribute("disabled"));
        Assert.Contains("rvm-desabilitado", cortado.Find(".rvm-chip-escolha").ClassList);
    }

    [Fact]
    public void Atributos_extras_vao_para_o_input_e_class_fica_na_raiz()
    {
        var cortado = Grupo(null, null, c => c.Add(x => x.Value, "A").Add(x => x.Label, "A")
            .AddUnmatched("data-teste", "x").AddUnmatched("class", "extra"));

        Assert.Equal("x", cortado.Find("input[type=radio]").GetAttribute("data-teste"));
        Assert.Contains("extra", cortado.Find(".rvm-chip-escolha").ClassList);
    }

    [Fact]
    public void Fora_do_grupo_da_erro_claro()
    {
        var erro = Assert.Throws<InvalidOperationException>(
            () => Render<RvmChoiceChip<string>>(p => p.Add(x => x.Value, "A").Add(x => x.Label, "A")));

        Assert.Contains("RvmRadioGroup", erro.Message);
    }
}
