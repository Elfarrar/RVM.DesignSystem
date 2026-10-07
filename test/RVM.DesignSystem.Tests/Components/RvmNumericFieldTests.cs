using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using RVM.DesignSystem.Components.TextField;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>O RvmNumericField do contrato com o RVM.UI (DSGN-017): leitura na cultura do campo, faixa, setas e exibicao.</summary>
public class RvmNumericFieldTests : BunitContext
{
    public RvmNumericFieldTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private sealed class Caixa<T>
    {
        public T Valor = default!;
    }

    private IRenderedComponent<RvmNumericField<T>> Campo<T>(Caixa<T> caixa,
        Action<ComponentParameterCollectionBuilder<RvmNumericField<T>>>? extra = null)
        => Render<RvmNumericField<T>>(p =>
        {
            p.Add(x => x.Value, caixa.Valor)
             .Add(x => x.ValueChanged, EventCallback.Factory.Create<T>(this, v => caixa.Valor = v))
             .Add(x => x.ValueExpression, () => caixa.Valor);
            extra?.Invoke(p);
        });

    [Fact]
    public void Input_e_texto_com_teclado_numerico_nunca_type_number()
    {
        var dec = Campo(new Caixa<decimal>());
        var input = dec.Find("input");
        Assert.Equal("text", input.GetAttribute("type"));
        Assert.Equal("decimal", input.GetAttribute("inputmode"));
        Assert.Equal("rvm-entrada", input.GetAttribute("class"));

        var inteiro = Campo(new Caixa<int>());
        Assert.Equal("numeric", inteiro.Find("input").GetAttribute("inputmode"));
    }

    [Fact]
    public void Le_em_pt_BR_por_padrao()
    {
        var caixa = new Caixa<decimal>();
        var cortado = Campo(caixa);

        cortado.Find("input").Change("1.234,56");

        Assert.Equal(1234.56m, caixa.Valor);
        Assert.Equal("1.234,56", cortado.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Le_na_cultura_informada()
    {
        var caixa = new Caixa<double>();
        var cortado = Campo(caixa, p => p.Add(x => x.Culture, CultureInfo.InvariantCulture));

        cortado.Find("input").Change("1,234.5");

        Assert.Equal(1234.5d, caixa.Valor);
    }

    [Fact]
    public void Ponto_fora_de_grupo_de_milhar_e_recusado_e_nao_vira_milhar()
    {
        var caixa = new Caixa<decimal> { Valor = 7m };
        var cortado = Campo(caixa);

        cortado.Find("input").Change("1.5");

        Assert.Equal(7m, caixa.Valor);
        Assert.Equal("true", cortado.Find("input").GetAttribute("aria-invalid"));
    }

    [Fact]
    public void Vazio_vira_nulo_no_tipo_anulavel()
    {
        var caixa = new Caixa<int?> { Valor = 3 };
        var cortado = Campo(caixa);

        cortado.Find("input").Change("");

        Assert.Null(caixa.Valor);
        Assert.Empty(cortado.FindAll(".rvm-apoio"));
    }

    [Fact]
    public void Vazio_no_tipo_nao_anulavel_mostra_erro()
    {
        var caixa = new Caixa<int> { Valor = 3 };
        var cortado = Campo(caixa);

        cortado.Find("input").Change("");

        Assert.Equal(3, caixa.Valor);
        Assert.Equal("Informe um numero neste campo.", cortado.Find(".rvm-apoio").TextContent);
    }

    [Fact]
    public void Texto_invalido_nao_muda_o_valor_mostra_o_erro_e_mantem_o_texto()
    {
        var caixa = new Caixa<decimal> { Valor = 10m };
        var cortado = Campo(caixa, p => p.Add(x => x.Id, "qtd").Add(x => x.Label, "Quantidade"));

        cortado.Find("input").Change("abc");

        Assert.Equal(10m, caixa.Valor);
        var input = cortado.Find("input");
        Assert.Equal("abc", input.GetAttribute("value"));
        Assert.Equal("true", input.GetAttribute("aria-invalid"));
        Assert.Equal("qtd-erro", input.GetAttribute("aria-describedby"));
        Assert.Equal("Digite um numero valido, como 1.234,56.", cortado.Find("#qtd-erro").TextContent);
        Assert.Contains("rvm-erro", cortado.Find(".rvm-campo").ClassList);

        cortado.Find("input").Change("12");
        Assert.Equal(12m, caixa.Valor);
        Assert.Null(cortado.Find("input").GetAttribute("aria-invalid"));
    }

    [Fact]
    public void Erro_de_leitura_entra_na_validacao_do_EditForm()
    {
        var modelo = new Modelo();
        var contexto = new EditContext(modelo);
        var cortado = Render<RvmNumericField<int>>(p => p
            .AddCascadingValue(contexto)
            .Add(x => x.Value, modelo.Quantidade)
            .Add(x => x.ValueExpression, () => modelo.Quantidade));

        cortado.Find("input").Change("1,5");

        Assert.Contains("Este campo aceita so numeros inteiros, sem casas decimais.", contexto.GetValidationMessages());
        Assert.Equal("Este campo aceita so numeros inteiros, sem casas decimais.", cortado.Find(".rvm-apoio").TextContent);
    }

    private sealed class Modelo
    {
        public int Quantidade { get; set; }
    }

    [Fact]
    public void Fora_da_faixa_mostra_erro_e_nao_corrige()
    {
        var caixa = new Caixa<decimal> { Valor = 5m };
        var cortado = Campo(caixa, p => p.Add(x => x.Min, 1m).Add(x => x.Max, 1000m));

        cortado.Find("input").Change("0");
        Assert.Equal(5m, caixa.Valor);
        Assert.Equal("0", cortado.Find("input").GetAttribute("value"));
        Assert.Equal("O menor valor aceito e 1.", cortado.Find(".rvm-apoio").TextContent);

        cortado.Find("input").Change("1.000,01");
        Assert.Equal(5m, caixa.Valor);
        Assert.Equal("O maior valor aceito e 1.000.", cortado.Find(".rvm-apoio").TextContent);

        cortado.Find("input").Change("1.000");
        Assert.Equal(1000m, caixa.Valor);
    }

    [Fact]
    public void Setas_somam_e_tiram_o_Step_presas_na_faixa()
    {
        var caixa = new Caixa<decimal> { Valor = 1m };
        var cortado = Campo(caixa, p => p.Add(x => x.Step, 0.5m).Add(x => x.Max, 2m));

        cortado.Find("input").KeyDown("ArrowUp");
        Assert.Equal(1.5m, caixa.Valor);
        cortado.Find("input").KeyDown("ArrowUp");
        cortado.Find("input").KeyDown("ArrowUp");
        Assert.Equal(2m, caixa.Valor);
        cortado.Find("input").KeyDown("ArrowDown");
        Assert.Equal(1.5m, caixa.Valor);
        cortado.Find("input").KeyDown("a");
        Assert.Equal(1.5m, caixa.Valor);
    }

    [Fact]
    public void Seta_depois_de_texto_invalido_limpa_o_erro_e_parte_do_Min()
    {
        var caixa = new Caixa<int?>();
        var cortado = Campo(caixa, p => p.Add(x => x.Min, 10m));

        cortado.Find("input").Change("xyz");
        cortado.Find("input").KeyDown("ArrowUp");

        Assert.Equal(11, caixa.Valor);
        Assert.Equal("11", cortado.Find("input").GetAttribute("value"));
        Assert.Null(cortado.Find("input").GetAttribute("aria-invalid"));
    }

    [Fact]
    public void Seta_no_campo_desabilitado_nao_faz_nada()
    {
        var caixa = new Caixa<int> { Valor = 4 };
        var cortado = Campo(caixa, p => p.Add(x => x.Disabled, true));

        cortado.Find("input").KeyDown("ArrowUp");

        Assert.Equal(4, caixa.Valor);
    }

    [Fact]
    public void Decimals_arredonda_meio_para_longe_do_zero_e_exibe_as_casas()
    {
        var caixa = new Caixa<decimal>();
        var cortado = Campo(caixa, p => p.Add(x => x.Decimals, 2));

        cortado.Find("input").Change("2,345");
        Assert.Equal(2.35m, caixa.Valor);

        cortado.Find("input").Change("-2,345");
        Assert.Equal(-2.35m, caixa.Valor);

        cortado.Find("input").Change("3");
        Assert.Equal("3,00", cortado.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Sem_Decimals_mostra_as_casas_que_o_numero_tem()
    {
        var caixa = new Caixa<double> { Valor = 1234.125d };
        var cortado = Campo(caixa);

        Assert.Equal("1.234,125", cortado.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Format_tem_precedencia_e_o_texto_formatado_volta_a_ser_lido()
    {
        var caixa = new Caixa<decimal> { Valor = 1234.5m };
        var cortado = Campo(caixa, p => p.Add(x => x.Format, "C").Add(x => x.Decimals, 0));

        var exibido = cortado.Find("input").GetAttribute("value")!;
        Assert.Equal(1234.5m.ToString("C", CultureInfo.GetCultureInfo("pt-BR")), exibido);

        cortado.Find("input").Change("R$ 10,40");
        Assert.Equal(10m, caixa.Valor);
    }

    [Fact]
    public void Prefixo_e_sufixo_ficam_na_caixa_e_descrevem_o_campo()
    {
        var caixa = new Caixa<decimal?>();
        var cortado = Campo(caixa, p => p
            .Add(x => x.Id, "peso")
            .Add(x => x.Prefix, "~")
            .Add(x => x.Suffix, "kg")
            .Add(x => x.HelperText, "Peso liquido"));

        var filhos = cortado.Find(".rvm-caixa").Children.Select(c => c.ClassName ?? c.TagName.ToLowerInvariant()).ToArray();
        Assert.Equal("rvm-afixo rvm-prefixo", filhos[0]);
        Assert.Equal("rvm-entrada", filhos[1]);
        Assert.Equal("rvm-afixo rvm-sufixo", filhos[2]);
        Assert.Equal("peso-prefixo peso-sufixo peso-apoio", cortado.Find("input").GetAttribute("aria-describedby"));

        cortado.Find("input").Change("12,5 kg");
        Assert.Equal(12.5m, caixa.Valor);
    }

    [Fact]
    public void Immediate_le_a_cada_tecla_e_o_comum_so_ao_sair()
    {
        var comum = new Caixa<int?>();
        var cortadoComum = Campo(comum);
        cortadoComum.Find("input").Input("42");
        Assert.Null(comum.Valor);
        cortadoComum.Find("input").Change("42");
        Assert.Equal(42, comum.Valor);

        var imediato = new Caixa<int?>();
        var cortadoImediato = Campo(imediato, p => p.Add(x => x.Immediate, true));
        cortadoImediato.Find("input").Input("42");
        Assert.Equal(42, imediato.Valor);
        cortadoImediato.Find("input").Change("7");
        Assert.Equal(42, imediato.Valor);
    }

    [Fact]
    public void Repassa_id_nome_classe_formato_e_atributos_extras()
    {
        var caixa = new Caixa<long>();
        var cortado = Campo(caixa, p => p
            .Add(x => x.Id, "area")
            .Add(x => x.Name, "talhao.area")
            .Add(x => x.Label, "Area")
            .Add(x => x.Class, "minha")
            .Add(x => x.Shape, RvmFieldShape.Pill)
            .AddUnmatched("data-teste", "x"));

        var input = cortado.Find("input");
        Assert.Equal("area", input.GetAttribute("id"));
        Assert.Equal("talhao.area", input.GetAttribute("name"));
        Assert.Equal("x", input.GetAttribute("data-teste"));
        Assert.Null(input.GetAttribute("aria-label"));
        Assert.Equal("area", cortado.Find("label").GetAttribute("for"));
        var raiz = cortado.Find(".rvm-campo");
        Assert.Contains("minha", raiz.ClassList);
        Assert.Contains("rvm-pilula", raiz.ClassList);
    }

    [Fact]
    public void Sem_rotulo_o_placeholder_vira_nome_acessivel()
    {
        var cortado = Campo(new Caixa<int>(), p => p.Add(x => x.Placeholder, "Quantidade"));

        Assert.Equal("Quantidade", cortado.Find("input").GetAttribute("aria-label"));
    }

    [Fact]
    public void ErrorText_tem_precedencia()
    {
        var cortado = Campo(new Caixa<int>(), p => p.Add(x => x.ErrorText, "Estoque insuficiente"));

        cortado.Find("input").Change("abc");

        Assert.Equal("Estoque insuficiente", cortado.Find(".rvm-apoio").TextContent);
    }

    [Fact]
    public void Numero_grande_demais_para_o_tipo_mostra_erro()
    {
        var caixa = new Caixa<int> { Valor = 1 };
        var cortado = Campo(caixa);

        cortado.Find("input").Change("99.999.999.999");

        Assert.Equal(1, caixa.Valor);
        Assert.Equal("Esse numero e grande demais para este campo.", cortado.Find(".rvm-apoio").TextContent);
    }

    [Fact]
    public void Float_e_menos_tipografico()
    {
        var caixa = new Caixa<float>();
        var cortado = Campo(caixa);

        cortado.Find("input").Change("−3,5");

        Assert.Equal(-3.5f, caixa.Valor);
    }

    [Fact]
    public void Tipo_nao_numerico_e_recusado()
    {
        string? texto = null;
        Assert.Throws<InvalidOperationException>(() => Render<RvmNumericField<string?>>(p => p
            .Add(x => x.Value, texto)
            .Add(x => x.ValueExpression, () => texto)));
    }
}
