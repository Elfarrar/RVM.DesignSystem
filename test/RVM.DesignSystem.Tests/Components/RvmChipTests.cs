using Bunit;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem;
using RVM.DesignSystem.Components.Chip;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

public class RvmChipTests : BunitContext
{
    [Fact]
    public void Sem_parametro_sai_um_chip_preenchido_medio_neutro()
    {
        var cortado = Render<RvmChip>(p => p.AddChildContent("Ativo"));

        var chip = cortado.Find("span.rvm-chip");
        Assert.Equal("rvm-chip rvm-medio rvm-preenchido rvm-neutro", chip.GetAttribute("class"));
        Assert.Equal("Ativo", cortado.Find("span.rvm-rotulo").TextContent);
    }

    [Theory]
    [InlineData(RvmChipVariant.Filled, "rvm-preenchido")]
    [InlineData(RvmChipVariant.Outlined, "rvm-contorno")]
    [InlineData(RvmChipVariant.Soft, "rvm-suave")]
    public void Estilo_vira_classe(RvmChipVariant variante, string classe)
    {
        var cortado = Render<RvmChip>(p => p.Add(x => x.Variant, variante).AddChildContent("x"));

        Assert.Contains(classe, cortado.Find("span.rvm-chip").GetAttribute("class"));
    }

    [Fact]
    public void Papel_de_cor_substitui_o_neutro()
    {
        var cortado = Render<RvmChip>(p => p.Add(x => x.Color, RvmColor.Warning).AddChildContent("x"));

        var classe = cortado.Find("span.rvm-chip").GetAttribute("class");
        Assert.Contains("rvm-warning", classe);
        Assert.DoesNotContain("rvm-neutro", classe);
    }

    [Theory]
    [InlineData(RvmSize.Small, "rvm-pequeno")]
    [InlineData(RvmSize.Medium, "rvm-medio")]
    [InlineData(RvmSize.Large, "rvm-medio")]
    public void O_kit_so_tem_dois_tamanhos_e_o_grande_sai_como_medio(RvmSize tamanho, string classe)
    {
        var cortado = Render<RvmChip>(p => p.Add(x => x.Size, tamanho).AddChildContent("x"));

        Assert.Contains(classe, cortado.Find("span.rvm-chip").GetAttribute("class"));
    }

    [Fact]
    public void Sem_quem_escute_o_remover_nao_ha_botao()
    {
        var cortado = Render<RvmChip>(p => p.AddChildContent("Filtro"));

        Assert.Empty(cortado.FindAll("button"));
    }

    [Fact]
    public async Task Botao_de_remover_tem_nome_acessivel_e_avisa_quem_escuta()
    {
        var removidos = 0;
        var cortado = Render<RvmChip>(p => p
            .Add(x => x.OnRemove, EventCallback.Factory.Create(this, () => removidos++))
            .Add(x => x.RemoveLabel, "Remover filtro de status")
            .AddChildContent("Status: ativo"));

        var botao = cortado.Find("button.rvm-remover");
        Assert.Equal("button", botao.GetAttribute("type"));
        Assert.Equal("Remover filtro de status", botao.GetAttribute("aria-label"));

        await botao.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        Assert.Equal(1, removidos);
    }

    [Fact]
    public void Nome_padrao_do_remover_e_em_portugues()
    {
        var cortado = Render<RvmChip>(p => p
            .Add(x => x.OnRemove, EventCallback.Factory.Create(this, () => { }))
            .AddChildContent("x"));

        Assert.Equal("Remover", cortado.Find("button.rvm-remover").GetAttribute("aria-label"));
    }

    [Fact]
    public async Task Desabilitado_nao_remove()
    {
        var removidos = 0;
        var cortado = Render<RvmChip>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.OnRemove, EventCallback.Factory.Create(this, () => removidos++))
            .AddChildContent("x"));

        var chip = cortado.Find("span.rvm-chip");
        Assert.Contains("rvm-desabilitado", chip.GetAttribute("class"));
        Assert.Equal("true", chip.GetAttribute("aria-disabled"));

        var botao = cortado.Find("button.rvm-remover");
        Assert.True(botao.HasAttribute("disabled"));
        await botao.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        Assert.Equal(0, removidos);
    }

    [Fact]
    public void Miniatura_vence_o_icone()
    {
        var cortado = Render<RvmChip>(p => p
            .Add(x => x.AvatarSrc, "foto.png")
            .Add(x => x.AvatarAlt, "Ana")
            .Add(x => x.StartIcon, RvmIconName.Star)
            .AddChildContent("Ana"));

        Assert.Equal("foto.png", cortado.Find("img.rvm-miniatura").GetAttribute("src"));
        Assert.Empty(cortado.FindAll("svg"));
    }

    [Fact]
    public void Miniatura_sem_alt_e_decorativa()
    {
        // O rotulo ao lado ja diz quem e; alt vazio evita o leitor de tela anunciar o nome duas vezes.
        var cortado = Render<RvmChip>(p => p.Add(x => x.AvatarSrc, "foto.png").AddChildContent("Ana"));

        Assert.Equal(string.Empty, cortado.Find("img.rvm-miniatura").GetAttribute("alt"));
    }

    [Fact]
    public void Icone_quando_nao_ha_miniatura()
    {
        var cortado = Render<RvmChip>(p => p.Add(x => x.StartIcon, RvmIconName.Star).AddChildContent("x"));

        Assert.NotEmpty(cortado.FindAll("svg"));
    }

    [Fact]
    public void Classe_e_atributos_do_consumidor_chegam_a_raiz()
    {
        var cortado = Render<RvmChip>(p => p
            .AddUnmatched("class", "minha")
            .AddUnmatched("data-teste", "1")
            .AddChildContent("x"));

        var chip = cortado.Find("span.rvm-chip");
        Assert.Contains("minha", chip.GetAttribute("class"));
        Assert.Equal("1", chip.GetAttribute("data-teste"));
    }
}
