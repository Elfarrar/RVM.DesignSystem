using Bunit;
using RVM.DesignSystem.Components.Badge;
using RVM.DesignSystem.Components.Button;
using RVM.DesignSystem.Components.Chart;
using RVM.DesignSystem.Components.Table;
using RVM.DesignSystem.Components.Tabs;
using RVM.DesignSystem.Components.Typography;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>Os valores de enum que o contrato com o RVM.UI trouxe (DSGN-017): os novos e os aliases.</summary>
public class RvmContratoEnumsTests : BunitContext
{
    public RvmContratoEnumsTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Theory]
    [InlineData(RvmColor.Accent, "rvm-primary")]
    [InlineData(RvmColor.Neutral, "rvm-secondary")]
    [InlineData(RvmColor.Danger, "rvm-error")]
    [InlineData(RvmColor.Inverse, "rvm-inverse")]
    public void Alias_de_cor_sai_com_a_classe_do_papel_do_DS(RvmColor cor, string classe)
    {
        var cortado = Render<RvmButton>(p => p.Add(x => x.Color, cor).AddChildContent("Salvar"));

        Assert.Contains(classe, cortado.Find("button").ClassList);
    }

    [Fact]
    public void Inverse_chega_ao_badge()
    {
        var cortado = Render<RvmBadge>(p => p.Add(x => x.Color, RvmColor.Inverse).Add(x => x.Content, "3"));

        Assert.NotNull(cortado.Find(".rvm-inverse"));
    }

    [Theory]
    [InlineData(RvmColor.Accent, "rvm-cor-primary")]
    [InlineData(RvmColor.Danger, "rvm-cor-error")]
    [InlineData(RvmColor.Inverse, "rvm-cor-inverse")]
    public void Classe_da_cor_do_grafico_nao_depende_do_nome_do_enum(RvmColor cor, string classe)
    {
        // Com alias, o ToString() do enum pode devolver qualquer um dos dois nomes: a classe nao pode vir dele.
        var cortado = Render<RvmColumnChart<Venda>>(p => p
            .Add(x => x.Items, [new Venda("Jan", 10, 0)])
            .Add(x => x.Label, v => v.Mes)
            .Add(x => x.AriaLabel, "Vendas")
            .AddChildContent<RvmChartSeries<Venda>>(s => s.Add(x => x.Name, "Receita").Add(x => x.Value, v => v.Receita).Add(x => x.Color, cor)));

        Assert.NotEmpty(cortado.FindAll("." + classe));
    }

    [Theory]
    [InlineData(RvmButtonVariant.Soft, "rvm-suave")]
    [InlineData(RvmButtonVariant.Filled, "rvm-preenchido")]
    public void Variante_do_contrato_vira_classe(RvmButtonVariant variante, string classe)
    {
        var cortado = Render<RvmButton>(p => p.Add(x => x.Variant, variante).AddChildContent("Salvar"));

        Assert.Contains(classe, cortado.Find("button").ClassList);
    }

    [Theory]
    [InlineData(RvmTextColor.Accent, "var(--rvm-color-primary-text)")]
    [InlineData(RvmTextColor.Success, "var(--rvm-color-success-text)")]
    [InlineData(RvmTextColor.Danger, "var(--rvm-color-error-text)")]
    [InlineData(RvmTextColor.Warning, "var(--rvm-color-warning-text)")]
    [InlineData(RvmTextColor.Inverse, "var(--rvm-color-inverse-text)")]
    [InlineData(RvmTextColor.Default, "var(--rvm-color-text-primary)")]
    public void Cor_de_texto_do_contrato_vira_token(RvmTextColor cor, string token)
    {
        var cortado = Render<RvmTypography>(p => p.Add(x => x.Color, cor).AddChildContent("x"));

        Assert.Contains(token, cortado.Find("p").GetAttribute("style"));
    }

    [Theory]
    [InlineData(RvmTabsVariant.Page, "rvm-pagina")]
    [InlineData(RvmTabsVariant.Regular, "rvm-sublinhadas")]
    [InlineData(RvmTabsVariant.Button, "rvm-preenchidas")]
    public void Variante_de_abas_do_contrato_vira_classe(RvmTabsVariant variante, string classe)
    {
        var cortado = Render<RvmTabs>(p => p.Add(x => x.Variant, variante).Add(x => x.AriaLabel, "Abas"));

        Assert.Contains(classe, cortado.Find(".rvm-abas").ClassList);
    }

    [Fact]
    public void Ordenacao_inicial_None_nao_ordena()
    {
        var cortado = Render<RvmTable<string>>(p => p
            .Add(x => x.Items, ["Bravo", "Alfa"])
            .Add(x => x.Caption, "Nomes")
            .AddChildContent<RvmTableColumn<string>>(c => c.Add(x => x.Title, "Nome").Add(x => x.Value, s => s).Add(x => x.Sortable, true).Add(x => x.InitialSort, RvmSortDirection.None)));

        Assert.Null(cortado.Find("th").GetAttribute("aria-sort"));
        Assert.Equal(["Bravo", "Alfa"], cortado.FindAll("tbody tr").Select(l => l.TextContent.Trim()));
    }
}
