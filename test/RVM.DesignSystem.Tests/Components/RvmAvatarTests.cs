using Bunit;
using RVM.DesignSystem;
using RVM.DesignSystem.Components.Avatar;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

public class RvmAvatarTests : BunitContext
{
    [Fact]
    public void Com_foto_o_nome_acessivel_e_o_alt_da_imagem()
    {
        var cortado = Render<RvmAvatar>(p => p
            .Add(x => x.Src, "foto.png")
            .Add(x => x.Alt, "Rafael Veneroso"));

        var img = cortado.Find("img");
        Assert.Equal("foto.png", img.GetAttribute("src"));
        Assert.Equal("Rafael Veneroso", img.GetAttribute("alt"));
        Assert.False(cortado.Find("span").HasAttribute("role"));
    }

    [Fact]
    public void Sem_foto_as_iniciais_viram_imagem_com_nome_acessivel()
    {
        // "RV" sozinho nao diz nada a quem ouve a pagina: o nome vai no aria-label, e as iniciais
        // ficam escondidas do leitor de tela para nao serem soletradas.
        var cortado = Render<RvmAvatar>(p => p
            .Add(x => x.Initials, "rv")
            .Add(x => x.Alt, "Rafael Veneroso"));

        var raiz = cortado.Find("span.avatar");
        Assert.Equal("img", raiz.GetAttribute("role"));
        Assert.Equal("Rafael Veneroso", raiz.GetAttribute("aria-label"));
        var iniciais = cortado.Find("span.iniciais");
        Assert.Equal("RV", iniciais.TextContent);
        Assert.Equal("true", iniciais.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Sem_alt_o_nome_acessivel_cai_para_as_iniciais()
    {
        var cortado = Render<RvmAvatar>(p => p.Add(x => x.Initials, "PR"));

        Assert.Equal("PR", cortado.Find("span.avatar").GetAttribute("aria-label"));
    }

    [Fact]
    public void Iniciais_longas_sao_cortadas_em_duas_letras()
    {
        var cortado = Render<RvmAvatar>(p => p.Add(x => x.Initials, "abcde"));

        Assert.Equal("AB", cortado.Find("span.iniciais").TextContent);
    }

    [Fact]
    public void Icone_vence_as_iniciais()
    {
        var cortado = Render<RvmAvatar>(p => p
            .Add(x => x.Icon, RvmIconName.User)
            .Add(x => x.Initials, "RV"));

        Assert.NotEmpty(cortado.FindAll("svg"));
        Assert.Empty(cortado.FindAll("span.iniciais"));
    }

    [Fact]
    public void Foto_que_nao_carrega_cai_para_as_iniciais()
    {
        var cortado = Render<RvmAvatar>(p => p
            .Add(x => x.Src, "quebrada.png")
            .Add(x => x.Initials, "RV")
            .Add(x => x.Alt, "Rafael"));

        cortado.Find("img").TriggerEvent("onerror", new Microsoft.AspNetCore.Components.Web.ErrorEventArgs());

        Assert.Empty(cortado.FindAll("img"));
        Assert.Equal("RV", cortado.Find("span.iniciais").TextContent);
    }

    [Fact]
    public void Foto_nova_ganha_nova_chance_depois_de_uma_falha()
    {
        var cortado = Render<RvmAvatar>(p => p
            .Add(x => x.Src, "quebrada.png")
            .Add(x => x.Initials, "RV"));
        cortado.Find("img").TriggerEvent("onerror", new Microsoft.AspNetCore.Components.Web.ErrorEventArgs());

        cortado.Render(p => p.Add(x => x.Src, "boa.png"));

        Assert.Equal("boa.png", cortado.Find("img").GetAttribute("src"));
    }

    [Theory]
    [InlineData(RvmSize.Small, "pequeno")]
    [InlineData(RvmSize.Medium, "medio")]
    [InlineData(RvmSize.Large, "grande")]
    public void Tamanho_vira_classe(RvmSize tamanho, string classe)
    {
        var cortado = Render<RvmAvatar>(p => p.Add(x => x.Size, tamanho).Add(x => x.Initials, "A"));

        Assert.Contains(classe, cortado.Find("span.avatar").GetAttribute("class"));
    }

    [Theory]
    [InlineData(RvmAvatarShape.Circle, "circulo")]
    [InlineData(RvmAvatarShape.Rounded, "arredondado")]
    [InlineData(RvmAvatarShape.Square, "quadrado")]
    public void Forma_vira_classe(RvmAvatarShape forma, string classe)
    {
        var cortado = Render<RvmAvatar>(p => p.Add(x => x.Shape, forma).Add(x => x.Initials, "A"));

        Assert.Contains(classe, cortado.Find("span.avatar").GetAttribute("class"));
    }

    [Fact]
    public void Suave_e_cor_viram_classe()
    {
        var cortado = Render<RvmAvatar>(p => p
            .Add(x => x.Variant, RvmAvatarVariant.Soft)
            .Add(x => x.Color, RvmColor.Success)
            .Add(x => x.Initials, "SU"));

        var classe = cortado.Find("span.avatar").GetAttribute("class");
        Assert.Contains("suave", classe);
        Assert.Contains("success", classe);
    }

    [Fact]
    public void Conteudo_livre_quando_nao_ha_foto_icone_nem_iniciais()
    {
        var cortado = Render<RvmAvatar>(p => p.AddChildContent("<b>?</b>"));

        Assert.Equal("?", cortado.Find("b").TextContent);
    }

    [Fact]
    public void Classe_e_atributos_do_consumidor_chegam_a_raiz()
    {
        var cortado = Render<RvmAvatar>(p => p
            .Add(x => x.Initials, "A")
            .AddUnmatched("class", "minha")
            .AddUnmatched("data-teste", "1"));

        var raiz = cortado.Find("span.avatar");
        Assert.Contains("minha", raiz.GetAttribute("class"));
        Assert.Equal("1", raiz.GetAttribute("data-teste"));
    }
}

public class RvmAvatarGroupTests : BunitContext
{
    [Fact]
    public void Grupo_tem_papel_e_nome_para_o_leitor_de_tela()
    {
        var cortado = Render<RvmAvatarGroup>(p => p.Add(x => x.Label, "Participantes"));

        var grupo = cortado.Find("div");
        Assert.Equal("group", grupo.GetAttribute("role"));
        Assert.Equal("Participantes", grupo.GetAttribute("aria-label"));
    }

    [Fact]
    public void Sem_rotulo_o_grupo_ganha_um_nome_generico_em_portugues()
    {
        var cortado = Render<RvmAvatarGroup>();

        Assert.Equal("Grupo de pessoas", cortado.Find("div").GetAttribute("aria-label"));
    }

    [Fact]
    public void Excedente_vira_um_avatar_mais_n_com_nome_acessivel()
    {
        var cortado = Render<RvmAvatarGroup>(p => p.Add(x => x.Surplus, 3));

        var mais = cortado.Find("span.avatar");
        Assert.Equal("+3", cortado.Find("span.iniciais").TextContent);
        Assert.Equal("Mais 3", mais.GetAttribute("aria-label"));
    }

    [Fact]
    public void Sem_excedente_nao_ha_avatar_mais_n()
    {
        var cortado = Render<RvmAvatarGroup>(p => p.AddChildContent("<span class=\"filho\"></span>"));

        Assert.Empty(cortado.FindAll("span.avatar"));
        Assert.NotEmpty(cortado.FindAll("span.filho"));
    }

    [Fact]
    public void Classe_do_consumidor_soma()
    {
        var cortado = Render<RvmAvatarGroup>(p => p.AddUnmatched("class", "minha"));

        Assert.Equal("grupo minha", cortado.Find("div").GetAttribute("class"));
    }
}
