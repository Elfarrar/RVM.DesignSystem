using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RVM.DesignSystem;
using RVM.DesignSystem.Components.Alert;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Tests.Components;

public class RvmAlertTests : BunitContext
{
    [Fact]
    public void Sem_parametro_e_um_aviso_informativo_no_estilo_padrao()
    {
        var cortado = Render<RvmAlert>(p => p.AddChildContent("Sua sessao expira em 5 minutos."));

        var alerta = cortado.Find("div.alerta");
        Assert.Equal("alerta padrao info", alerta.GetAttribute("class"));
        Assert.Equal("Sua sessao expira em 5 minutos.", cortado.Find("div.mensagem").TextContent);
    }

    [Theory]
    [InlineData(RvmColor.Error, "alert")]
    [InlineData(RvmColor.Warning, "alert")]
    [InlineData(RvmColor.Info, "status")]
    [InlineData(RvmColor.Success, "status")]
    [InlineData(RvmColor.Primary, "status")]
    public void Erro_e_aviso_interrompem_o_leitor_de_tela_o_resto_espera(RvmColor gravidade, string papel)
    {
        // Tudo como `alert` vira ruido: o leitor de tela interromperia a leitura para cada
        // "salvo com sucesso", e quem depende dele passa a ignorar os avisos que importam.
        var cortado = Render<RvmAlert>(p => p.Add(x => x.Severity, gravidade).AddChildContent("x"));

        Assert.Equal(papel, cortado.Find("div.alerta").GetAttribute("role"));
    }

    [Theory]
    [InlineData(RvmAlertVariant.Standard, "padrao")]
    [InlineData(RvmAlertVariant.Filled, "preenchido")]
    [InlineData(RvmAlertVariant.Outlined, "contorno")]
    public void Estilo_vira_classe(RvmAlertVariant variante, string classe)
    {
        var cortado = Render<RvmAlert>(p => p.Add(x => x.Variant, variante).AddChildContent("x"));

        Assert.Contains(classe, cortado.Find("div.alerta").GetAttribute("class"));
    }

    [Fact]
    public void Titulo_aparece_acima_da_mensagem()
    {
        var cortado = Render<RvmAlert>(p => p
            .Add(x => x.Title, "Nao foi possivel salvar")
            .AddChildContent("Confira a sua conexao e tente de novo."));

        Assert.Equal("Nao foi possivel salvar", cortado.Find("div.titulo").TextContent);
    }

    [Fact]
    public void Sem_titulo_nao_ha_elemento_de_titulo()
    {
        var cortado = Render<RvmAlert>(p => p.AddChildContent("x"));

        Assert.Empty(cortado.FindAll("div.titulo"));
    }

    [Fact]
    public void Icone_da_gravidade_aparece_por_padrao_e_some_quando_pedido()
    {
        var com = Render<RvmAlert>(p => p.AddChildContent("x"));
        var sem = Render<RvmAlert>(p => p.Add(x => x.ShowIcon, false).AddChildContent("x"));

        Assert.NotEmpty(com.FindAll("span.icone-alerta svg"));
        Assert.Empty(sem.FindAll("span.icone-alerta"));
    }

    [Theory]
    [InlineData(RvmColor.Success, RvmIconName.CircleCheck)]
    [InlineData(RvmColor.Warning, RvmIconName.AlertTriangle)]
    [InlineData(RvmColor.Error, RvmIconName.AlertCircle)]
    [InlineData(RvmColor.Info, RvmIconName.InfoCircle)]
    [InlineData(RvmColor.Primary, RvmIconName.InfoCircle)]
    public void Cada_gravidade_tem_o_seu_icone(RvmColor gravidade, RvmIconName esperado)
    {
        var cortado = Render<RvmAlert>(p => p.Add(x => x.Severity, gravidade).AddChildContent("x"));

        Assert.Equal(DesenhoDe(esperado), cortado.Find("span.icone-alerta svg").InnerHtml);
    }

    [Fact]
    public void Icone_informado_vence_o_da_gravidade()
    {
        var cortado = Render<RvmAlert>(p => p
            .Add(x => x.Severity, RvmColor.Error)
            .Add(x => x.Icon, RvmIconName.Lock)
            .AddChildContent("x"));

        Assert.Equal(DesenhoDe(RvmIconName.Lock), cortado.Find("span.icone-alerta svg").InnerHtml);
    }

    /// <summary>
    /// O desenho que o proprio RvmIcon produz para um nome. Comparar a SAIDA, e nao uma propriedade
    /// interna, mantem o teste valido mesmo se a escolha do icone mudar de lugar no codigo.
    /// </summary>
    private string DesenhoDe(RvmIconName nome)
        => Render<RVM.DesignSystem.Components.Icon.RvmIcon>(p => p.Add(x => x.Name, nome)).Find("svg").InnerHtml;

    [Fact]
    public async Task Fechar_tem_nome_acessivel_em_portugues_e_avisa_quem_escuta()
    {
        var fechados = 0;
        var cortado = Render<RvmAlert>(p => p
            .Add(x => x.OnClose, EventCallback.Factory.Create(this, () => fechados++))
            .AddChildContent("x"));

        var botao = cortado.Find("button.fechar");
        Assert.Equal("Fechar aviso", botao.GetAttribute("aria-label"));
        Assert.Equal("button", botao.GetAttribute("type"));

        await botao.ClickAsync(new MouseEventArgs());

        Assert.Equal(1, fechados);
    }

    [Fact]
    public void Sem_acao_e_sem_fechar_nao_ha_area_de_acoes()
    {
        var cortado = Render<RvmAlert>(p => p.AddChildContent("x"));

        Assert.Empty(cortado.FindAll("div.acoes"));
    }

    [Fact]
    public void Acao_aparece_ao_lado_da_mensagem()
    {
        var cortado = Render<RvmAlert>(p => p
            .Add(x => x.Action, "<button class=\"desfazer\">Desfazer</button>")
            .AddChildContent("Item excluido."));

        Assert.Equal("Desfazer", cortado.Find("div.acoes button.desfazer").TextContent);
    }

    [Fact]
    public void Classe_e_atributos_do_consumidor_chegam_a_raiz()
    {
        var cortado = Render<RvmAlert>(p => p
            .AddUnmatched("class", "minha")
            .AddUnmatched("data-teste", "1")
            .AddChildContent("x"));

        var alerta = cortado.Find("div.alerta");
        Assert.Contains("minha", alerta.GetAttribute("class"));
        Assert.Equal("1", alerta.GetAttribute("data-teste"));
    }
}
