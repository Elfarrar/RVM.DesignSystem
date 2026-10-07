using Bunit;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.ColorPicker;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>RvmColorPicker e RvmColorField, do contrato com o RVM.UI (DSGN-017).</summary>
public class RvmCoresTests : BunitContext
{
    public RvmCoresTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private enum Etiqueta { Urgente, Normal, Baixa }

    private static string CorDa(Etiqueta e) => e switch { Etiqueta.Urgente => "#FF4C51", Etiqueta.Normal => "#264CC8", _ => "#56CA00" };

    private IRenderedComponent<RvmColorPicker<Etiqueta>> Picker(Etiqueta valor, Action<Etiqueta>? aoMudar = null, string? erro = null, string? apoio = null)
        => Render<RvmColorPicker<Etiqueta>>(p => p
            .Add(x => x.Items, Enum.GetValues<Etiqueta>())
            .Add(x => x.ItemColor, CorDa)
            .Add(x => x.ItemText, e => $"Etiqueta {e}")
            .Add(x => x.Label, "Cor da etiqueta")
            .Add(x => x.Name, "tarefa.cor")
            .Add(x => x.Id, "cor")
            .Add(x => x.ErrorText, erro)
            .Add(x => x.HelperText, apoio)
            .Add(x => x.Value, valor)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<Etiqueta>(this, v => aoMudar?.Invoke(v)))
            .Add(x => x.ValueExpression, () => valor));

    [Fact]
    public void Picker_renderiza_radios_nativos_com_name_num_fieldset_com_legenda()
    {
        var cortado = Picker(Etiqueta.Normal);

        Assert.Equal("Cor da etiqueta", cortado.Find("fieldset > legend").TextContent);
        var radios = cortado.FindAll("fieldset input[type=radio]");
        Assert.Equal(3, radios.Count);
        Assert.All(radios, r => Assert.Equal("tarefa.cor", r.GetAttribute("name")));
        Assert.Equal(["Urgente", "Normal", "Baixa"], radios.Select(r => r.GetAttribute("value")));
    }

    [Fact]
    public void Picker_marca_a_escolhida_com_checked_e_check()
    {
        var cortado = Picker(Etiqueta.Normal);

        var escolhidos = cortado.FindAll("input:checked");
        Assert.Single(escolhidos);
        Assert.Equal("Normal", escolhidos[0].GetAttribute("value"));
        // O check e o anel aparecem pelo :checked no CSS; a marca existe ao lado de cada radio.
        Assert.NotNull(cortado.Find("input:checked + .rvm-anel .rvm-marca svg"));
    }

    [Fact]
    public void Picker_escolha_muda_o_valor()
    {
        Etiqueta? escolhida = null;
        var cortado = Picker(Etiqueta.Normal, v => escolhida = v);

        cortado.Find("input[value=Baixa]").Change("Baixa");

        Assert.Equal(Etiqueta.Baixa, escolhida);
    }

    [Fact]
    public void Picker_usa_o_ItemText_como_nome_e_dica_e_a_cor_inline()
    {
        var cortado = Picker(Etiqueta.Normal);

        var radio = cortado.Find("input[value=Urgente]");
        Assert.Equal("Etiqueta Urgente", radio.GetAttribute("aria-label"));
        Assert.Equal("Etiqueta Urgente", cortado.FindAll("label.rvm-cor")[0].GetAttribute("title"));
        Assert.Contains("background-color: #FF4C51", cortado.FindAll(".rvm-bolinha")[0].GetAttribute("style"));
    }

    [Fact]
    public void Picker_sem_ItemText_usa_o_ToString()
    {
        var valor = "#FFFFFF";
        var cortado = Render<RvmColorPicker<string>>(p => p
            .Add(x => x.Items, ["#FFFFFF"])
            .Add(x => x.ItemColor, h => h)
            .Add(x => x.Value, valor)
            .Add(x => x.ValueExpression, () => valor));

        Assert.Equal("#FFFFFF", cortado.Find("input").GetAttribute("aria-label"));
        // Sem Name e sem @bind em SSR, o grupo usa o id: radio sem name nao troca com as setas.
        Assert.False(string.IsNullOrEmpty(cortado.Find("input").GetAttribute("name")));
    }

    [Fact]
    public void Picker_liga_apoio_e_erro_por_aria_describedby()
    {
        var comApoio = Picker(Etiqueta.Normal, apoio: "Aparece no quadro.");
        Assert.Equal("cor-apoio", comApoio.Find("fieldset").GetAttribute("aria-describedby"));
        Assert.Equal("Aparece no quadro.", comApoio.Find("#cor-apoio").TextContent);

        var comErro = Picker(Etiqueta.Normal, erro: "Escolha uma cor.", apoio: "Aparece no quadro.");
        var fieldset = comErro.Find("fieldset");
        Assert.Equal("cor-erro", fieldset.GetAttribute("aria-describedby"));
        Assert.Equal("true", fieldset.GetAttribute("aria-invalid"));
        Assert.Equal("Escolha uma cor.", comErro.Find("#cor-erro").TextContent);
        Assert.Empty(comErro.FindAll("#cor-apoio"));
    }

    [Fact]
    public void Picker_desabilitado_desabilita_o_fieldset_e_os_radios()
    {
        var valor = Etiqueta.Normal;
        var cortado = Render<RvmColorPicker<Etiqueta>>(p => p
            .Add(x => x.Items, Enum.GetValues<Etiqueta>())
            .Add(x => x.Disabled, true)
            .Add(x => x.Class, "minha")
            .Add(x => x.Value, valor)
            .Add(x => x.ValueExpression, () => valor));

        Assert.True(cortado.Find("fieldset").HasAttribute("disabled"));
        Assert.Contains("minha", cortado.Find("fieldset").ClassList);
        Assert.All(cortado.FindAll("input"), r => Assert.True(r.HasAttribute("disabled")));
    }

    private IRenderedComponent<RvmColorField> Campo(string? valor, Action<string?>? aoMudar = null, bool permitir = false, IReadOnlyList<RvmNamedColor>? cores = null)
        => Render<RvmColorField>(p =>
        {
            p.Add(x => x.Label, "Cor do talhao")
             .Add(x => x.Name, "talhao.cor")
             .Add(x => x.Id, "cor")
             .Add(x => x.AllowCustom, permitir)
             .Add(x => x.Value, valor)
             .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => aoMudar?.Invoke(v)))
             .Add(x => x.ValueExpression, () => valor);
            if (cores is not null) p.Add(x => x.Colors, cores);
        });

    [Fact]
    public void ColorField_mostra_a_paleta_padrao_com_nomes()
    {
        var cortado = Campo(null);

        var radios = cortado.FindAll("input[type=radio]");
        Assert.Equal(6, radios.Count);
        Assert.Contains(radios, r => r.GetAttribute("value") == "#264CC8" && r.GetAttribute("aria-label") == "Azul");
        Assert.All(radios, r => Assert.Equal("talhao.cor", r.GetAttribute("name")));
        Assert.Empty(cortado.FindAll("input:checked"));
        Assert.Empty(cortado.FindAll("details"));
    }

    [Fact]
    public void ColorField_escolha_na_paleta_muda_o_valor_em_hex()
    {
        string? valor = null;
        var cortado = Campo(null, v => valor = v);

        cortado.Find("input[value='#56CA00']").Change("#56CA00");

        Assert.Equal("#56CA00", valor);
    }

    [Fact]
    public void ColorField_casa_o_valor_sem_olhar_a_caixa()
    {
        var cortado = Campo("#56ca00");

        Assert.Equal("#56CA00", cortado.Find("input:checked").GetAttribute("value"));
    }

    [Fact]
    public void ColorField_cor_fora_da_paleta_vira_Personalizada()
    {
        var cortado = Campo("#4caf50", cores: [new("Verde", "#00FF00")]);

        var radios = cortado.FindAll("input[type=radio]");
        Assert.Equal(2, radios.Count);
        var extra = radios[1];
        Assert.Equal("Personalizada (#4CAF50)", extra.GetAttribute("aria-label"));
        Assert.True(extra.HasAttribute("checked"));
    }

    [Fact]
    public void ColorField_compor_tem_input_color_e_campo_do_hex()
    {
        var cortado = Campo("#4CAF50", permitir: true);

        Assert.Equal("Compor cor", cortado.Find("details summary").TextContent);
        Assert.Equal("#4caf50", cortado.Find("input[type=color]").GetAttribute("value"));
        Assert.Equal("#4CAF50", cortado.Find("#cor-hex").GetAttribute("value"));
        Assert.Equal("cor-hex", cortado.Find("label[for='cor-hex']").GetAttribute("for"));
        // So os radios postam: os campos de compor nao tem name.
        Assert.Null(cortado.Find("input[type=color]").GetAttribute("name"));
        Assert.Null(cortado.Find("#cor-hex").GetAttribute("name"));
    }

    [Fact]
    public void ColorField_input_color_muda_o_valor_normalizado()
    {
        string? valor = null;
        var cortado = Campo(null, v => valor = v, permitir: true);

        cortado.Find("input[type=color]").Change("#a1b2c3");

        Assert.Equal("#A1B2C3", valor);
    }

    [Fact]
    public void ColorField_hex_digitado_e_normalizado()
    {
        string? valor = null;
        var cortado = Campo(null, v => valor = v, permitir: true);

        cortado.Find("#cor-hex").Change(" 4caf50 ");

        Assert.Equal("#4CAF50", valor);
    }

    [Fact]
    public void ColorField_hex_invalido_mostra_erro_e_nao_muda_o_valor()
    {
        var mudou = false;
        var cortado = Campo("#264CC8", _ => mudou = true, permitir: true);

        cortado.Find("#cor-hex").Change("verde");

        Assert.False(mudou);
        var hex = cortado.Find("#cor-hex");
        Assert.Equal("true", hex.GetAttribute("aria-invalid"));
        Assert.Equal("cor-hex-erro", hex.GetAttribute("aria-describedby"));
        Assert.Equal("Digite a cor como #RRGGBB, por exemplo #4CAF50.", cortado.Find("#cor-hex-erro").TextContent);
        Assert.Equal("#264CC8", cortado.Find("input:checked").GetAttribute("value"));

        cortado.Find("#cor-hex").Change("#FFB400");
        Assert.Empty(cortado.FindAll("#cor-hex-erro"));
        Assert.True(mudou);
    }
}
