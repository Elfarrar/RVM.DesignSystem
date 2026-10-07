using RVM.DesignSystem.Components.IconBadge;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>RvmIconBadge e RvmArtisticIconBadge, do contrato com o RVM.UI (DSGN-017).</summary>
public class RvmIconBadgeTests : BunitContext
{
    // --- RvmIconBadge ---

    [Fact]
    public void Padroes_do_RVM_UI_suave_accent_grande_e_decorativo()
    {
        var cortado = Render<RvmIconBadge>(p => p.Add(x => x.Icon, RvmIconName.Home));

        var raiz = cortado.Find(".rvm-icon-badge");
        Assert.Equal(["rvm-icon-badge", "rvm-suave", "rvm-grande", "rvm-primary"], raiz.ClassList.ToArray());
        Assert.Equal("true", raiz.GetAttribute("aria-hidden"));
        Assert.Null(raiz.GetAttribute("role"));
        Assert.Null(raiz.GetAttribute("aria-label"));
        Assert.Equal("24", cortado.Find("svg").GetAttribute("width"));

        var instancia = cortado.Instance;
        Assert.Equal(RvmIconStyle.BoldDuotone, instancia.IconStyle);
        Assert.Equal(RvmColor.Accent, instancia.Color);
    }

    [Fact]
    public void Com_Label_vira_imagem_com_nome()
    {
        var cortado = Render<RvmIconBadge>(p => p
            .Add(x => x.Icon, RvmIconName.Home)
            .Add(x => x.Label, "Receita do mes"));

        var raiz = cortado.Find(".rvm-icon-badge");
        Assert.Equal("img", raiz.GetAttribute("role"));
        Assert.Equal("Receita do mes", raiz.GetAttribute("aria-label"));
        Assert.Null(raiz.GetAttribute("aria-hidden"));
        Assert.Equal("true", cortado.Find("svg").GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Label_em_branco_continua_decorativo()
    {
        var cortado = Render<RvmIconBadge>(p => p
            .Add(x => x.Icon, RvmIconName.Home)
            .Add(x => x.Label, "  "));

        Assert.Equal("true", cortado.Find(".rvm-icon-badge").GetAttribute("aria-hidden"));
    }

    [Theory]
    [InlineData(RvmIconBadgeVariant.Solid, "rvm-cheio")]
    [InlineData(RvmIconBadgeVariant.Soft, "rvm-suave")]
    [InlineData(RvmIconBadgeVariant.Neutral, "rvm-neutro")]
    public void Variante_vira_classe(RvmIconBadgeVariant variante, string classe)
    {
        var cortado = Render<RvmIconBadge>(p => p.Add(x => x.Icon, RvmIconName.Home).Add(x => x.Variant, variante));

        Assert.Contains(classe, cortado.Find(".rvm-icon-badge").ClassList);
    }

    [Theory]
    [InlineData(RvmIconBadgeSize.Small, "rvm-pequeno", "16")]
    [InlineData(RvmIconBadgeSize.Medium, "rvm-medio", "20")]
    [InlineData(RvmIconBadgeSize.Large, "rvm-grande", "24")]
    [InlineData(RvmIconBadgeSize.XLarge, "rvm-extra", "28")]
    public void Tamanho_vira_classe_e_lado_do_icone(RvmIconBadgeSize tamanho, string classe, string lado)
    {
        var cortado = Render<RvmIconBadge>(p => p.Add(x => x.Icon, RvmIconName.Home).Add(x => x.Size, tamanho));

        Assert.Contains(classe, cortado.Find(".rvm-icon-badge").ClassList);
        Assert.Equal(lado, cortado.Find("svg").GetAttribute("width"));
    }

    [Theory]
    [InlineData(RvmColor.Danger, "rvm-error")]
    [InlineData(RvmColor.Neutral, "rvm-secondary")]
    [InlineData(RvmColor.Success, "rvm-success")]
    [InlineData(RvmColor.Inverse, "rvm-inverse")]
    public void Cor_usa_a_classe_do_papel_com_os_aliases(RvmColor cor, string classe)
    {
        var cortado = Render<RvmIconBadge>(p => p.Add(x => x.Icon, RvmIconName.Home).Add(x => x.Color, cor));

        Assert.Contains(classe, cortado.Find(".rvm-icon-badge").ClassList);
    }

    [Fact]
    public void Class_e_atributos_vao_para_a_raiz()
    {
        var cortado = Render<RvmIconBadge>(p => p
            .Add(x => x.Icon, RvmIconName.Home)
            .Add(x => x.Class, "minha")
            .AddUnmatched("data-teste", "badge"));

        var raiz = cortado.Find(".rvm-icon-badge");
        Assert.Contains("minha", raiz.ClassList);
        Assert.Equal("badge", raiz.GetAttribute("data-teste"));
    }

    // --- RvmArtisticIconBadge ---

    [Fact]
    public void Artistico_padroes_degrade_accent_e_decorativo()
    {
        var cortado = Render<RvmArtisticIconBadge>(p => p.Add(x => x.Icon, RvmIconName.Home));

        var raiz = cortado.Find(".rvm-artistic-icon-badge");
        Assert.Equal(["rvm-artistic-icon-badge", "rvm-degrade", "rvm-primary"], raiz.ClassList.ToArray());
        Assert.Equal("true", raiz.GetAttribute("aria-hidden"));
        Assert.Null(raiz.GetAttribute("role"));
        Assert.Equal("28", cortado.Find("svg").GetAttribute("width"));
    }

    [Fact]
    public void Artistico_brilho_com_Label_cor_e_atributos()
    {
        var cortado = Render<RvmArtisticIconBadge>(p => p
            .Add(x => x.Icon, RvmIconName.Home)
            .Add(x => x.Variant, RvmArtisticVariant.Glow)
            .Add(x => x.Color, RvmColor.Warning)
            .Add(x => x.Label, "Alerta de geada")
            .Add(x => x.Class, "destaque")
            .AddUnmatched("id", "geada"));

        var raiz = cortado.Find(".rvm-artistic-icon-badge");
        Assert.Contains("rvm-brilho", raiz.ClassList);
        Assert.Contains("rvm-warning", raiz.ClassList);
        Assert.Contains("destaque", raiz.ClassList);
        Assert.Equal("geada", raiz.Id);
        Assert.Equal("img", raiz.GetAttribute("role"));
        Assert.Equal("Alerta de geada", raiz.GetAttribute("aria-label"));
        Assert.Null(raiz.GetAttribute("aria-hidden"));
    }
}
