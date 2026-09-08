using Bunit;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>
/// Comportamento e acessibilidade dos componentes da onda 1.
/// </summary>
/// <remarks>
/// O foco e no que o bUnit consegue provar: marcacao, atributos ARIA, associacao rotulo/campo
/// e callbacks. O que depende de navegador — foco visivel, teclado, leitor de tela de verdade —
/// e do E2E com axe (`03` § Testes). Um nao substitui o outro.
/// </remarks>
public class ComponentesTests : BunitContext
{
    // ---------- RvmIcon ----------

    [Fact]
    public void Icone_sem_Title_e_decorativo_e_some_do_leitor_de_tela()
    {
        // O padrao e SILENCIO, e e deliberado: a maioria dos icones acompanha um texto que ja
        // diz a mesma coisa, e anuncia-los faz o leitor repetir.
        var cut = Render<RvmIcon>(p => p.Add(x => x.Name, "check"));

        var svg = cut.Find("svg");
        Assert.Equal("true", svg.GetAttribute("aria-hidden"));
        Assert.Null(svg.GetAttribute("role"));
    }

    [Fact]
    public void Icone_com_Title_vira_imagem_anunciavel()
    {
        var cut = Render<RvmIcon>(p => p
            .Add(x => x.Name, "check")
            .Add(x => x.Title, "Concluído"));

        var svg = cut.Find("svg");
        Assert.Equal("img", svg.GetAttribute("role"));
        Assert.Equal("Concluído", svg.GetAttribute("aria-label"));
        Assert.Contains("Concluído", cut.Find("title").TextContent);
    }

    [Fact]
    public void Icone_inexistente_falha_ALTO_e_lista_os_disponiveis()
    {
        // Falhar em silencio renderizaria um buraco na tela que ninguem nota em
        // desenvolvimento e o usuario final ve.
        var e = Assert.Throws<ArgumentException>(() =>
            Render<RvmIcon>(p => p.Add(x => x.Name, "nao-existe")));

        Assert.Contains("nao-existe", e.Message, StringComparison.Ordinal);
        Assert.Contains("check", e.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Icone_muda_de_desenho_conforme_o_peso()
    {
        var regular = Render<RvmIcon>(p => p.Add(x => x.Name, "check"));
        var fill = Render<RvmIcon>(p => p
            .Add(x => x.Name, "check")
            .Add(x => x.Weight, RvmIconWeight.Fill));

        Assert.NotEqual(regular.Find("svg").InnerHtml, fill.Find("svg").InnerHtml);
    }

    // ---------- RvmIconButton ----------

    [Fact]
    public void IconButton_exige_Label_e_o_expoe_como_nome_acessivel()
    {
        var cut = Render<RvmIconButton>(p => p
            .Add(x => x.Icon, "x")
            .Add(x => x.Label, "Fechar"));

        Assert.Equal("Fechar", cut.Find("button").GetAttribute("aria-label"));
    }

    [Fact]
    public void IconButton_SEM_Label_falha_em_vez_de_renderizar_botao_mudo()
    {
        // Um botao de icone sem nome acessivel passa em revisao visual (a tela fica bonita) e
        // so aparece para quem usa leitor de tela. Por isso e erro, nao aviso.
        var e = Assert.Throws<ArgumentException>(() =>
            Render<RvmIconButton>(p => p.Add(x => x.Icon, "x")));

        Assert.Contains("Label", e.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void IconButton_nao_dispara_clique_quando_desabilitado()
    {
        var cliques = 0;
        var cut = Render<RvmIconButton>(p => p
            .Add(x => x.Icon, "x")
            .Add(x => x.Label, "Fechar")
            .Add(x => x.Disabled, true)
            .Add(x => x.OnClick, () => cliques++));

        cut.Find("button").Click();

        Assert.Equal(0, cliques);
    }

    // ---------- RvmFormField ----------

    [Fact]
    public void FormField_liga_o_rotulo_ao_campo_pelo_for()
    {
        var cut = Render<RvmFormField>(p => p
            .Add(x => x.FieldId, "campo-1")
            .Add(x => x.Label, "Nome"));

        Assert.Equal("campo-1", cut.Find("label").GetAttribute("for"));
    }

    [Fact]
    public void FormField_mostra_o_erro_NO_LUGAR_da_ajuda()
    {
        // Os dois ao mesmo tempo competem pela atencao no momento em que ela mais importa.
        var cut = Render<RvmFormField>(p => p
            .Add(x => x.FieldId, "campo-1")
            .Add(x => x.Help, "Como aparece no documento")
            .Add(x => x.Error, "Campo obrigatório"));

        Assert.Contains("Campo obrigatório", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("Como aparece no documento", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void FormField_anuncia_o_erro_com_role_alert()
    {
        var cut = Render<RvmFormField>(p => p
            .Add(x => x.FieldId, "campo-1")
            .Add(x => x.Error, "Inválido"));

        Assert.Equal("alert", cut.Find(".rvm-field__error").GetAttribute("role"));
    }

    [Fact]
    public void FormField_esconde_o_asterisco_do_leitor_de_tela()
    {
        // A obrigatoriedade chega pelo `required` do controle; o asterisco e so visual.
        var cut = Render<RvmFormField>(p => p
            .Add(x => x.FieldId, "campo-1")
            .Add(x => x.Label, "Nome")
            .Add(x => x.Required, true));

        Assert.Equal("true", cut.Find(".rvm-field__required").GetAttribute("aria-hidden"));
    }

    // ---------- RvmTextField ----------

    [Fact]
    public void TextField_associa_rotulo_ajuda_e_campo()
    {
        var cut = Render<RvmTextField>(p => p
            .Add(x => x.Id, "nome")
            .Add(x => x.Label, "Nome")
            .Add(x => x.Help, "Como no documento"));

        Assert.Equal("nome", cut.Find("label").GetAttribute("for"));
        Assert.Equal("nome-help", cut.Find("input").GetAttribute("aria-describedby"));
    }

    [Fact]
    public void TextField_com_erro_aponta_o_describedby_para_o_ERRO_e_marca_invalid()
    {
        // Apontar para a ajuda enquanto ha erro faria o leitor de tela ler a dica em vez do
        // problema; apontar para um id que nao existe faria ele nao ler nada.
        var cut = Render<RvmTextField>(p => p
            .Add(x => x.Id, "nome")
            .Add(x => x.Help, "Como no documento")
            .Add(x => x.Error, "Obrigatório"));

        var input = cut.Find("input");
        Assert.Equal("nome-error", input.GetAttribute("aria-describedby"));
        Assert.Equal("true", input.GetAttribute("aria-invalid"));
    }

    [Fact]
    public void TextField_sem_ajuda_nem_erro_nao_inventa_describedby()
    {
        var cut = Render<RvmTextField>(p => p.Add(x => x.Id, "nome"));

        Assert.Null(cut.Find("input").GetAttribute("aria-describedby"));
    }

    [Theory]
    [InlineData(RvmTextFieldType.Email, "email")]
    [InlineData(RvmTextFieldType.Password, "password")]
    [InlineData(RvmTextFieldType.Tel, "tel")]
    [InlineData(RvmTextFieldType.Search, "search")]
    [InlineData(RvmTextFieldType.Text, "text")]
    public void TextField_reflete_o_tipo_que_muda_o_teclado_do_celular(RvmTextFieldType tipo, string esperado)
    {
        var cut = Render<RvmTextField>(p => p.Add(x => x.Type, tipo));

        Assert.Equal(esperado, cut.Find("input").GetAttribute("type"));
    }

    [Fact]
    public void TextField_atualiza_o_valor_ao_digitar()
    {
        string? capturado = null;
        var cut = Render<RvmTextField>(p => p
            .Add(x => x.Value, "")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => capturado = v)));

        cut.Find("input").Input("Rafael");

        Assert.Equal("Rafael", capturado);
    }

    // ---------- RvmCheckbox ----------

    [Fact]
    public void Checkbox_usa_input_nativo()
    {
        // Reimplementar com <div> exigiria recriar foco, Espaco, estado ARIA e integracao com
        // <label> — e todo componente que faz isso erra pelo menos um.
        var cut = Render<RvmCheckbox>(p => p.Add(x => x.Label, "Aceito"));

        Assert.Equal("checkbox", cut.Find("input").GetAttribute("type"));
    }

    [Fact]
    public void Checkbox_marca_e_desmarca()
    {
        var valor = false;
        var cut = Render<RvmCheckbox>(p => p
            .Add(x => x.Value, valor)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<bool>(this, v => valor = v)));

        cut.Find("input").Change(true);

        Assert.True(valor);
    }

    // ---------- RvmSwitch ----------

    [Fact]
    public void Switch_declara_role_switch()
    {
        // A diferenca para o checkbox nao e visual: switch aplica efeito imediato, checkbox e
        // escolha que vale no envio. O leitor de tela precisa saber qual dos dois e.
        var cut = Render<RvmSwitch>(p => p.Add(x => x.Label, "Notificações"));

        Assert.Equal("switch", cut.Find("input").GetAttribute("role"));
    }

    // ---------- RvmTextArea ----------

    [Fact]
    public void TextArea_conta_caracteres_com_aria_live_polite()
    {
        // "assertive" interromperia o leitor a cada tecla digitada — inutilizavel.
        var cut = Render<RvmTextArea>(p => p
            .Add(x => x.MaxLength, 100)
            .Add(x => x.Value, "abc"));

        var contador = cut.Find(".rvm-textarea__counter");
        Assert.Equal("polite", contador.GetAttribute("aria-live"));
        Assert.Contains("3", contador.TextContent, StringComparison.Ordinal);
        Assert.Contains("100", contador.TextContent, StringComparison.Ordinal);
    }

    [Fact]
    public void TextArea_sem_MaxLength_nao_mostra_contador()
    {
        var cut = Render<RvmTextArea>();

        Assert.Empty(cut.FindAll(".rvm-textarea__counter"));
    }

    // ---------- RvmSelect ----------

    [Fact]
    public void Select_usa_select_nativo_e_desenha_a_seta_como_decoracao()
    {
        var cut = Render<RvmSelect<string>>(p => p
            .Add(x => x.Placeholder, "Escolha")
            .AddChildContent("<option value=\"a\">A</option>"));

        Assert.NotNull(cut.Find("select"));
        // A seta e um icone decorativo: nao pode ser anunciada nem receber clique.
        Assert.Equal("true", cut.Find(".rvm-select__caret").GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Select_com_placeholder_gera_opcao_de_valor_vazio()
    {
        // value="" e o que faz o `required` nativo reconhecer "nada selecionado".
        var cut = Render<RvmSelect<string>>(p => p.Add(x => x.Placeholder, "Escolha"));

        Assert.Equal(string.Empty, cut.Find("option").GetAttribute("value"));
    }

    // ---------- RvmRadioGroup ----------

    [Fact]
    public void RadioGroup_usa_fieldset_e_legend()
    {
        // Sem eles o leitor de tela le "à vista", "parcelado" soltos, sem a pergunta.
        var cut = Render<RvmRadioGroup<string>>(p => p
            .Add(x => x.Label, "Forma de pagamento")
            .Add(x => x.Options, new RvmRadioGroup<string>.Option[] { new("v", "À vista") }));

        Assert.NotNull(cut.Find("fieldset"));
        Assert.Contains("Forma de pagamento", cut.Find("legend").TextContent, StringComparison.Ordinal);
    }

    [Fact]
    public void RadioGroup_compartilha_o_name_entre_as_opcoes()
    {
        // name igual e o que faz o navegador tratar como escolha UNICA e mover entre elas com
        // as setas do teclado. Sem isso viram checkboxes redondos.
        var cut = Render<RvmRadioGroup<string>>(p => p
            .Add(x => x.Id, "pgto")
            .Add(x => x.Options, new RvmRadioGroup<string>.Option[]
            {
                new("v", "À vista"),
                new("p", "Parcelado"),
            }));

        var nomes = cut.FindAll("input[type=radio]").Select(i => i.GetAttribute("name")).Distinct().ToList();
        Assert.Single(nomes);
        Assert.Equal("pgto", nomes[0]);
    }

    [Fact]
    public void RadioGroup_seleciona_a_opcao_clicada()
    {
        var valor = string.Empty;
        var cut = Render<RvmRadioGroup<string>>(p => p
            .Add(x => x.Options, new RvmRadioGroup<string>.Option[]
            {
                new("v", "À vista"),
                new("p", "Parcelado"),
            })
            .Add(x => x.Value, valor)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, v => valor = v)));

        cut.FindAll("input[type=radio]")[1].Change(true);

        Assert.Equal("p", valor);
    }
}
