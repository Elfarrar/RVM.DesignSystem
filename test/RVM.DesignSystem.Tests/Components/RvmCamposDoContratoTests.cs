using Bunit;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Checkbox;
using RVM.DesignSystem.Components.DatePicker;
using RVM.DesignSystem.Components.Radio;
using RVM.DesignSystem.Components.Select;
using RVM.DesignSystem.Components.Switch;
using RVM.DesignSystem.Components.TextField;
using RVM.DesignSystem.Components.TimePicker;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>Os parametros de campo do contrato com o RVM.UI (DSGN-017).</summary>
public class RvmCamposDoContratoTests : BunitContext
{
    public RvmCamposDoContratoTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Campo_de_texto_com_id_nome_classe_icones_pilula_e_link()
    {
        string? email = null;
        var cortado = Render<RvmTextField>(p => p
            .Add(x => x.ValueExpression, () => email)
            .Add(x => x.Label, "E-mail")
            .Add(x => x.Id, "email")
            .Add(x => x.Name, "contato.email")
            .Add(x => x.Class, "minha")
            .Add(x => x.StartIcon, RvmIconName.Letter)
            .Add(x => x.Shape, RvmFieldShape.Pill)
            .Add(x => x.LinkText, "Esqueci a senha")
            .Add(x => x.LinkHref, "/recuperar"));

        var input = cortado.Find("input");
        Assert.Equal("email", input.GetAttribute("id"));
        Assert.Equal("contato.email", input.GetAttribute("name"));
        Assert.Equal("email", cortado.Find("label").GetAttribute("for"));
        var raiz = cortado.Find(".rvm-campo");
        Assert.Contains("minha", raiz.ClassList);
        Assert.Contains("rvm-pilula", raiz.ClassList);
        Assert.Contains("rvm-com-prefixo", raiz.ClassList);
        Assert.NotNull(cortado.Find(".rvm-icone-do-campo svg"));
        Assert.Equal("/recuperar", cortado.Find("a.rvm-link-do-campo").GetAttribute("href"));
    }

    [Fact]
    public void Campo_imediato_atualiza_a_cada_tecla()
    {
        string? valor = null;
        var cortado = Render<RvmTextField>(p => p
            .Add(x => x.Immediate, true)
            .Add(x => x.Value, valor)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => valor = v))
            .Add(x => x.ValueExpression, () => valor));

        cortado.Find("input").Input("ab");

        Assert.Equal("ab", valor);
    }

    [Fact]
    public void Campo_comum_ignora_o_input_e_espera_o_change()
    {
        string? valor = null;
        var cortado = Render<RvmTextField>(p => p
            .Add(x => x.Value, valor)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => valor = v))
            .Add(x => x.ValueExpression, () => valor));

        cortado.Find("input").Input("ab");
        Assert.Null(valor);

        cortado.Find("input").Change("ab");
        Assert.Equal("ab", valor);
    }

    private static readonly string[] Culturas = ["Soja", "Milho"];

    [Fact]
    public void Seletor_com_id_pilula_fundo_cinza_e_imagem_nas_opcoes()
    {
        var cortado = Render<RvmSelect<string>>(p => p
            .Add(x => x.Items, Culturas)
            .Add(x => x.Label, "Cultura")
            .Add(x => x.Id, "cultura")
            .Add(x => x.Shape, RvmFieldShape.Pill)
            .Add(x => x.Background, RvmFieldBackground.Grey)
            .Add(x => x.ItemImage, c => $"/img/{c}.png"));

        Assert.Equal("cultura", cortado.Find("[role=combobox]").GetAttribute("id"));
        var raiz = cortado.Find(".rvm-campo");
        Assert.Contains("rvm-pilula", raiz.ClassList);
        Assert.Contains("rvm-fundo-cinza", raiz.ClassList);

        cortado.Find("[role=combobox]").Click();
        var imagem = cortado.Find("[role=option] img");
        Assert.Equal("/img/Soja.png", imagem.GetAttribute("src"));
        Assert.Equal("", imagem.GetAttribute("alt"));
    }

    [Fact]
    public void Seletor_somente_leitura_nao_abre()
    {
        var cortado = Render<RvmSelect<string>>(p => p.Add(x => x.Items, Culturas).Add(x => x.Label, "Cultura").Add(x => x.ReadOnly, true).Add(x => x.Value, "Soja"));

        var gatilho = cortado.Find("[role=combobox]");
        Assert.Equal("true", gatilho.GetAttribute("aria-readonly"));
        gatilho.Click();
        gatilho.KeyDown("ArrowDown");
        Assert.Empty(cortado.FindAll("[role=listbox]"));
    }

    [Fact]
    public void Varias_escolhas_aceitam_Value_com_o_nome_do_RVM_UI()
    {
        IReadOnlyList<string>? escolhidas = null;
        var cortado = Render<RvmMultiSelect<string>>(p => p
            .Add(x => x.Items, Culturas)
            .Add(x => x.Label, "Culturas")
            .Add(x => x.Value, ["Milho"])
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => escolhidas = v)));

        cortado.Find("[role=combobox]").Click();
        Assert.Equal("true", cortado.FindAll("[role=option]")[1].GetAttribute("aria-selected"));
        cortado.FindAll("[role=option]")[0].Click();

        Assert.Equal(["Soja", "Milho"], escolhidas);
    }

    [Fact]
    public void Data_com_id_alinhamento_e_limpar()
    {
        DateOnly? data = new DateOnly(2026, 10, 7);
        var cortado = Render<RvmDatePicker>(p => p
            .Add(x => x.Label, "Plantio")
            .Add(x => x.Id, "plantio")
            .Add(x => x.Alignment, RvmPopupAlignment.End)
            .Add(x => x.Clearable, true)
            .Add(x => x.Value, data)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, v => data = v)));

        Assert.Equal("plantio", cortado.Find("button.rvm-entrada").GetAttribute("id"));
        Assert.Contains("rvm-janela-no-fim", cortado.Find(".rvm-campo-data").ClassList);

        var limpar = cortado.Find("button.rvm-limpar");
        Assert.Equal("Limpar Plantio", limpar.GetAttribute("aria-label"));
        limpar.Click();

        Assert.Null(data);
        Assert.Empty(cortado.FindAll("button.rvm-limpar"));
    }

    [Fact]
    public void Horario_repassa_o_id_ao_campo()
        => Assert.Equal("hora", Render<RvmTimePicker>(p => p.Add(x => x.Label, "Hora").Add(x => x.Id, "hora")).Find("[role=combobox]").GetAttribute("id"));

    [Fact]
    public void Caixa_de_marcar_sem_apoio_continua_so_o_label()
    {
        var cortado = Render<RvmCheckbox>(p => p.Add(x => x.Label, "Aceito"));

        Assert.Empty(cortado.FindAll(".rvm-campo-marcar"));
        Assert.Equal("LABEL", cortado.Nodes.OfType<AngleSharp.Dom.IElement>().First().TagName);
    }

    [Fact]
    public void Caixa_de_marcar_com_id_nome_obrigatorio_apoio_e_link()
    {
        var cortado = Render<RvmCheckbox>(p => p
            .Add(x => x.Label, "Li e aceito os")
            .Add(x => x.Id, "termos")
            .Add(x => x.Name, "aceite")
            .Add(x => x.Required, true)
            .Add(x => x.HelperText, "Obrigatorio para continuar")
            .Add(x => x.LinkText, "termos de uso")
            .Add(x => x.LinkHref, "/termos")
            .Add(x => x.LabelPosition, RvmLabelPosition.Start));

        var input = cortado.Find("input[type=checkbox]");
        Assert.Equal("termos", input.GetAttribute("id"));
        Assert.Equal("aceite", input.GetAttribute("name"));
        Assert.Equal("true", input.GetAttribute("aria-required"));
        var apoio = cortado.Find(".rvm-apoio");
        Assert.Equal(apoio.GetAttribute("id"), input.GetAttribute("aria-describedby"));
        Assert.Equal("*", cortado.Find(".rvm-obrigatorio").TextContent);
        Assert.Equal("/termos", cortado.Find(".rvm-campo-marcar > a").GetAttribute("href"));
        Assert.Contains("rvm-rotulo-antes", cortado.Find("label").ClassList);
    }

    [Fact]
    public void Chave_com_erro_avisa_o_leitor_de_tela()
    {
        var cortado = Render<RvmSwitch>(p => p.Add(x => x.Label, "Notificacoes").Add(x => x.ErrorText, "Escolha uma opcao"));

        var input = cortado.Find("input");
        Assert.Equal("true", input.GetAttribute("aria-invalid"));
        Assert.Contains("rvm-apoio-erro", cortado.Find(".rvm-apoio").ClassList);
    }

    [Fact]
    public void Grupo_de_opcoes_com_id_apoio_e_opcao_desabilitada()
    {
        var cortado = Render<RvmRadioGroup<string>>(p => p
            .Add(x => x.Label, "Safra")
            .Add(x => x.Id, "safra")
            .Add(x => x.HelperText, "A do ano corrente")
            .AddChildContent<RvmRadio<string>>(r => r.Add(x => x.Value, "2026").Add(x => x.Label, "2026").Add(x => x.Disabled, true).Add(x => x.LabelPosition, RvmLabelPosition.Start).Add(x => x.Class, "minha")));

        var grupo = cortado.Find("fieldset");
        Assert.Equal("safra", grupo.GetAttribute("id"));
        Assert.Equal(cortado.Find(".rvm-apoio").GetAttribute("id"), grupo.GetAttribute("aria-describedby"));
        Assert.True(cortado.Find("input[type=radio]").HasAttribute("disabled"));
        var opcao = cortado.Find("label.rvm-radio");
        Assert.Contains("rvm-rotulo-antes", opcao.ClassList);
        Assert.Contains("rvm-desabilitado", opcao.ClassList);
        Assert.Contains("minha", opcao.ClassList);
    }
}
