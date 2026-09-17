using System.ComponentModel.DataAnnotations;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using RVM.DesignSystem;
using RVM.DesignSystem.Components.TextField;

namespace RVM.DesignSystem.Tests.Components;

public class RvmTextFieldTests : BunitContext
{
    private sealed class Cadastro
    {
        [Required(ErrorMessage = "Informe o nome para continuar.")]
        public string? Nome { get; set; }
    }

    private IRenderedComponent<RvmTextField> Campo(
        Cadastro modelo,
        Action<ComponentParameterCollectionBuilder<RvmTextField>>? extra = null,
        EditContext? contexto = null)
        => Render<RvmTextField>(p =>
        {
            if (contexto is not null)
            {
                p.AddCascadingValue(contexto);
            }

            p.Add(x => x.Value, modelo.Nome)
             .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => modelo.Nome = v))
             .Add(x => x.ValueExpression, () => modelo.Nome);
            extra?.Invoke(p);
        });

    [Fact]
    public void Renderiza_name_que_o_formulario_em_SSR_estatico_exige()
    {
        // O bug que motivou a regra: o RvmTextField anterior nao renderizava `name`, e o POST do
        // formulario em SSR estatico chegava sem o valor, sem erro nenhum.
        var cortado = Campo(new Cadastro(), contexto: new EditContext(new Cadastro()));

        var name = cortado.Find("input").GetAttribute("name");
        Assert.False(string.IsNullOrWhiteSpace(name));
        Assert.EndsWith("Nome", name);
    }

    [Fact]
    public void Quando_o_formulario_nao_faz_POST_o_name_e_omitido_e_nao_vazio()
    {
        // No WebAssembly o EditContext desliga os nomes de campo (nao ha POST para o servidor). O
        // site de documentacao e WASM, entao la o input nao tem name — e isso e do framework, nao
        // defeito. O que o componente garante e nao emitir `name=""`.
        var contexto = new EditContext(new Cadastro()) { ShouldUseFieldIdentifiers = false };
        var cortado = Campo(new Cadastro(), contexto: contexto);

        Assert.False(cortado.Find("input").HasAttribute("name"));
    }

    [Fact]
    public void Nome_informado_pelo_consumidor_vence_o_gerado()
    {
        var cortado = Campo(new Cadastro(), p => p.AddUnmatched("name", "cliente_nome"),
            new EditContext(new Cadastro()));

        Assert.Equal("cliente_nome", cortado.Find("input").GetAttribute("name"));
    }

    [Fact]
    public void Funciona_fora_de_um_EditForm()
    {
        var cortado = Campo(new Cadastro { Nome = "Ana" }, p => p.Add(x => x.Label, "Nome"));

        Assert.Equal("Ana", cortado.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Rotulo_aponta_para_o_input()
    {
        var cortado = Campo(new Cadastro(), p => p.Add(x => x.Label, "Nome completo"));

        var input = cortado.Find("input");
        var rotulo = cortado.Find("label");
        Assert.Equal(input.GetAttribute("id"), rotulo.GetAttribute("for"));
        Assert.Contains("Nome completo", rotulo.TextContent);
    }

    [Fact]
    public void Id_informado_pelo_consumidor_e_respeitado()
    {
        var cortado = Campo(new Cadastro(), p => p
            .Add(x => x.Label, "Nome")
            .AddUnmatched("id", "campo-nome"));

        Assert.Equal("campo-nome", cortado.Find("input").GetAttribute("id"));
        Assert.Equal("campo-nome", cortado.Find("label").GetAttribute("for"));
    }

    [Fact]
    public void Dois_campos_sem_id_nao_colidem()
    {
        var um = Campo(new Cadastro());
        var outro = Campo(new Cadastro());

        Assert.NotEqual(um.Find("input").GetAttribute("id"), outro.Find("input").GetAttribute("id"));
    }

    [Fact]
    public void Texto_de_apoio_e_ligado_ao_input()
    {
        var cortado = Campo(new Cadastro(), p => p.Add(x => x.HelperText, "Como no documento."));

        var apoio = cortado.Find("div.apoio");
        Assert.Equal("Como no documento.", apoio.TextContent);
        Assert.Contains(apoio.GetAttribute("id")!, cortado.Find("input").GetAttribute("aria-describedby"));
    }

    [Fact]
    public void Prefixo_e_sufixo_entram_na_descricao_do_input()
    {
        // Sem isto, quem usa leitor de tela nao ouve a unidade: "Peso, 12" em vez de "Peso, 12 Kg".
        var cortado = Campo(new Cadastro(), p => p
            .Add(x => x.Prefix, "R$")
            .Add(x => x.Suffix, "Kg"));

        var descrito = cortado.Find("input").GetAttribute("aria-describedby")!;
        Assert.Contains(cortado.Find("span.prefixo").GetAttribute("id")!, descrito);
        Assert.Contains(cortado.Find("span.sufixo").GetAttribute("id")!, descrito);
    }

    [Fact]
    public void Sem_apoio_prefixo_ou_sufixo_nao_ha_aria_describedby()
    {
        var cortado = Campo(new Cadastro());

        Assert.False(cortado.Find("input").HasAttribute("aria-describedby"));
    }

    [Fact]
    public void Erro_informado_marca_o_campo_invalido_e_toma_o_lugar_do_apoio()
    {
        var cortado = Campo(new Cadastro(), p => p
            .Add(x => x.HelperText, "Como no documento.")
            .Add(x => x.ErrorText, "Este CPF ja esta cadastrado."));

        var input = cortado.Find("input");
        Assert.Equal("true", input.GetAttribute("aria-invalid"));
        Assert.Equal("Este CPF ja esta cadastrado.", cortado.Find("div.apoio").TextContent);
        Assert.Contains("erro", cortado.Find("div.campo").GetAttribute("class"));
    }

    [Fact]
    public void Sem_erro_nao_ha_aria_invalid()
    {
        var cortado = Campo(new Cadastro());

        Assert.False(cortado.Find("input").HasAttribute("aria-invalid"));
    }

    [Fact]
    public void Mensagem_da_validacao_do_EditForm_aparece_sozinha()
    {
        var modelo = new Cadastro();
        var contexto = new EditContext(modelo);
        contexto.EnableDataAnnotationsValidation(Services);
        var cortado = Campo(modelo, p => p.Add(x => x.Label, "Nome"), contexto);

        cortado.InvokeAsync(() => contexto.Validate());
        cortado.Render();

        Assert.Equal("Informe o nome para continuar.", cortado.Find("div.apoio").TextContent);
        Assert.Equal("true", cortado.Find("input").GetAttribute("aria-invalid"));
    }

    [Fact]
    public void Obrigatorio_e_anunciado_mas_sem_o_required_nativo_que_barraria_o_EditForm()
    {
        // Com `required` nativo, o navegador bloqueia o envio com o balao dele (em ingles, sem estilo)
        // ANTES de a validacao do EditForm rodar — a mensagem em PT-BR nunca aparece. Foi pego no
        // navegador: o evento submit nem disparava.
        var cortado = Campo(new Cadastro(), p => p
            .Add(x => x.Label, "Nome")
            .Add(x => x.Required, true));

        var input = cortado.Find("input");
        Assert.False(input.HasAttribute("required"));
        Assert.Equal("true", input.GetAttribute("aria-required"));
        Assert.Equal("true", cortado.Find("span.obrigatorio").GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Desabilitado()
    {
        var cortado = Campo(new Cadastro(), p => p.Add(x => x.Disabled, true));

        Assert.True(cortado.Find("input").HasAttribute("disabled"));
        Assert.Contains("desabilitado", cortado.Find("div.campo").GetAttribute("class"));
    }

    [Theory]
    [InlineData(RvmInputType.Text, "text")]
    [InlineData(RvmInputType.Email, "email")]
    [InlineData(RvmInputType.Password, "password")]
    [InlineData(RvmInputType.Number, "number")]
    [InlineData(RvmInputType.Tel, "tel")]
    [InlineData(RvmInputType.Url, "url")]
    [InlineData(RvmInputType.Search, "search")]
    public void Tipo_vira_type_do_input(RvmInputType tipo, string esperado)
    {
        var cortado = Campo(new Cadastro(), p => p.Add(x => x.Type, tipo));

        Assert.Equal(esperado, cortado.Find("input").GetAttribute("type"));
    }

    [Fact]
    public void Digitar_atualiza_o_valor_ligado()
    {
        var modelo = new Cadastro();
        var cortado = Campo(modelo);

        cortado.Find("input").Change("Rafael");

        Assert.Equal("Rafael", modelo.Nome);
    }

    [Fact]
    public void Sem_placeholder_sai_um_espaco_que_faz_o_rotulo_flutuar_sem_js()
    {
        var cortado = Campo(new Cadastro(), p => p.Add(x => x.Label, "Nome"));

        Assert.Equal(" ", cortado.Find("input").GetAttribute("placeholder"));
        Assert.DoesNotContain("rotulo-fixo", cortado.Find("div.campo").GetAttribute("class"));
    }

    [Fact]
    public void Com_placeholder_o_rotulo_fica_sempre_em_cima()
    {
        var cortado = Campo(new Cadastro(), p => p
            .Add(x => x.Label, "Nome")
            .Add(x => x.Placeholder, "Como no documento"));

        Assert.Equal("Como no documento", cortado.Find("input").GetAttribute("placeholder"));
        Assert.Contains("rotulo-fixo", cortado.Find("div.campo").GetAttribute("class"));
    }

    [Theory]
    [InlineData(RvmTextFieldVariant.Outlined, RvmSize.Medium, "contorno medio")]
    [InlineData(RvmTextFieldVariant.Filled, RvmSize.Small, "preenchido pequeno")]
    [InlineData(RvmTextFieldVariant.Standard, RvmSize.Large, "padrao medio")]
    public void Estilo_e_tamanho_viram_classe(RvmTextFieldVariant estilo, RvmSize tamanho, string esperado)
    {
        var cortado = Campo(new Cadastro(), p => p
            .Add(x => x.Variant, estilo)
            .Add(x => x.Size, tamanho));

        Assert.StartsWith("campo " + esperado, cortado.Find("div.campo").GetAttribute("class"));
    }

    [Fact]
    public void Classe_do_consumidor_vai_para_a_raiz()
    {
        var cortado = Campo(new Cadastro(), p => p.AddUnmatched("class", "minha"));

        Assert.Contains("minha", cortado.Find("div.campo").GetAttribute("class"));
        Assert.Equal("entrada", cortado.Find("input").GetAttribute("class"));
    }
}
