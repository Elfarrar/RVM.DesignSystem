using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Avatar;
using RVM.DesignSystem.Components.FileIcon;
using RVM.DesignSystem.Components.Table;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>As celulas da tabela (RvmCell*) e o RvmFileIcon, do contrato com o RVM.UI (DSGN-017).</summary>
public class RvmCelulasTests : BunitContext
{
    public RvmCelulasTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    // --- Moldura comum ---

    [Fact]
    public void Toda_celula_renderiza_um_td_com_Class_e_atributos_extras()
    {
        var celulas = new[]
        {
            Render<RvmCellText>(p => p.Add(x => x.Text, "a").Add(x => x.Class, "minha").AddUnmatched("data-x", "1")).Find("td"),
            Render<RvmCellAction>(p => p.Add(x => x.Class, "minha").AddUnmatched("data-x", "1")).Find("td"),
            Render<RvmCellBadge>(p => p.Add(x => x.Class, "minha").AddUnmatched("data-x", "1")).Find("td"),
            Render<RvmCellCircleImage>(p => p.Add(x => x.Alt, "x").Add(x => x.Class, "minha").AddUnmatched("data-x", "1")).Find("td"),
            Render<RvmCellCode>(p => p.Add(x => x.Class, "minha").AddUnmatched("data-x", "1")).Find("td"),
            Render<RvmCellFiles>(p => p.Add(x => x.Name, "a.pdf").Add(x => x.Class, "minha").AddUnmatched("data-x", "1")).Find("td"),
            Render<RvmCellLabelBadge>(p => p.Add(x => x.Class, "minha").AddUnmatched("data-x", "1")).Find("td"),
            Render<RvmCellNumber>(p => p.Add(x => x.Class, "minha").AddUnmatched("data-x", "1")).Find("td"),
            Render<RvmCellProgress>(p => p.Add(x => x.Label, "R$ 1").Add(x => x.Class, "minha").AddUnmatched("data-x", "1")).Find("td"),
            Render<RvmCellRating>(p => p.Add(x => x.Value, 4).Add(x => x.Class, "minha").AddUnmatched("data-x", "1")).Find("td"),
            Render<RvmCellSelect>(p => p.Add(x => x.Label, "Selecionar").Add(x => x.Class, "minha").AddUnmatched("data-x", "1")).Find("td"),
            Render<RvmCellSquareImage>(p => p.Add(x => x.Src, "a.png").Add(x => x.Class, "minha").AddUnmatched("data-x", "1")).Find("td"),
            Render<RvmCellStatus>(p => p.Add(x => x.Class, "minha").AddUnmatched("data-x", "1")).Find("td"),
            Render<RvmCellUser>(p => p.Add(x => x.Name, "Ana").Add(x => x.Class, "minha").AddUnmatched("data-x", "1")).Find("td"),
            Render<RvmCellUserGroup>(p => p.Add(x => x.Class, "minha").AddUnmatched("data-x", "1")).Find("td"),
        };

        Assert.All(celulas, td =>
        {
            Assert.Contains("rvm-celula", td.ClassList);
            Assert.Contains("minha", td.ClassList);
            Assert.Equal("1", td.GetAttribute("data-x"));
        });
    }

    [Theory]
    [InlineData(RvmAlign.Start, "rvm-inicio")]
    [InlineData(RvmAlign.Center, "rvm-centro")]
    [InlineData(RvmAlign.End, "rvm-fim")]
    public void Align_vira_a_classe_de_alinhamento(RvmAlign alinhamento, string classe)
    {
        var td = Render<RvmCellNumber>(p => p.Add(x => x.Text, "10").Add(x => x.Align, alinhamento)).Find("td");

        Assert.Contains(classe, td.ClassList);
    }

    [Fact]
    public void Alinhamento_padrao_e_o_inicio_e_a_coluna_de_escolha_nao_tem()
    {
        Assert.Contains("rvm-inicio", Render<RvmCellText>().Find("td").ClassList);

        var escolha = Render<RvmCellSelect>(p => p.Add(x => x.Label, "Selecionar")).Find("td");
        Assert.Contains("rvm-celula-escolha", escolha.ClassList);
        Assert.DoesNotContain("rvm-inicio", escolha.ClassList);
    }

    // --- Texto, destaque e segunda linha ---

    [Fact]
    public void Texto_com_apoio_e_destaque()
    {
        var cortado = Render<RvmCellText>(p => p
            .Add(x => x.Text, "Talhao 3")
            .Add(x => x.Supporting, "Soja, 120 ha")
            .Add(x => x.Highlight, true));

        Assert.Equal("Talhao 3", cortado.Find(".rvm-titulo").TextContent);
        Assert.Equal("Soja, 120 ha", cortado.Find(".rvm-apoio").TextContent);
        Assert.Contains("rvm-destaque", cortado.Find(".rvm-celula-texto").ClassList);
    }

    [Fact]
    public void Sem_apoio_nao_ha_segunda_linha_e_sem_destaque_nao_ha_classe()
    {
        var cortado = Render<RvmCellText>(p => p.Add(x => x.Text, "Talhao 3"));

        Assert.Empty(cortado.FindAll(".rvm-apoio"));
        Assert.DoesNotContain("rvm-destaque", cortado.Find(".rvm-celula-texto").ClassList);
    }

    [Fact]
    public void ChildContent_substitui_o_texto()
    {
        var cortado = Render<RvmCellText>(p => p
            .Add(x => x.Text, "ignorado")
            .AddChildContent("<a href=\"/lote/1\">Lote 1</a>"));

        Assert.Equal("Lote 1", cortado.Find(".rvm-titulo a").TextContent);
        Assert.DoesNotContain("ignorado", cortado.Markup);
    }

    [Fact]
    public void Codigo_mostra_o_texto_ou_o_conteudo_livre()
    {
        Assert.Equal("#4910", Render<RvmCellCode>(p => p.Add(x => x.Text, "#4910")).Find(".rvm-codigo").TextContent);

        var livre = Render<RvmCellCode>(p => p.Add(x => x.Text, "x").AddChildContent("<a href=\"/nf/1\">NF 1</a>"));
        Assert.Equal("NF 1", livre.Find(".rvm-codigo a").TextContent);
    }

    [Fact]
    public void Acao_renderiza_o_slot()
    {
        var cortado = Render<RvmCellAction>(p => p
            .Add(x => x.Align, RvmAlign.End)
            .AddChildContent("<button type=\"button\">Excluir</button>"));

        Assert.Equal("Excluir", cortado.Find("td button").TextContent);
        Assert.Contains("rvm-fim", cortado.Find("td").ClassList);
    }

    [Fact]
    public void Selo_renderiza_a_figura_e_o_texto()
    {
        var cortado = Render<RvmCellBadge>(p => p
            .Add(x => x.Badge, (RenderFragment)(b => b.AddMarkupContent(0, "<span class=\"figura\"></span>")))
            .Add(x => x.Text, "Cooperativa Sul")
            .Add(x => x.Supporting, "Cliente desde 2019")
            .Add(x => x.Highlight, true));

        Assert.NotNull(cortado.Find(".rvm-selo .figura"));
        Assert.Equal("Cooperativa Sul", cortado.Find(".rvm-titulo").TextContent);
        Assert.Equal("Cliente desde 2019", cortado.Find(".rvm-apoio").TextContent);
        Assert.Contains("rvm-destaque", cortado.Find(".rvm-celula-texto").ClassList);
    }

    [Fact]
    public void Selo_sem_figura_nao_deixa_caixa_vazia()
    {
        Assert.Empty(Render<RvmCellBadge>(p => p.Add(x => x.Text, "x")).FindAll(".rvm-selo"));
    }

    // --- Imagens e nomes acessiveis ---

    [Fact]
    public void Imagem_redonda_usa_o_Alt_como_nome_acessivel()
    {
        var cortado = Render<RvmCellCircleImage>(p => p
            .Add(x => x.Src, "fazenda.png")
            .Add(x => x.Alt, "Fachada da Fazenda Boa Vista")
            .Add(x => x.Text, "Fazenda Boa Vista")
            .Add(x => x.Supporting, "Sorriso, MT"));

        Assert.Equal("Fachada da Fazenda Boa Vista", cortado.Find("img").GetAttribute("alt"));
        Assert.Equal("Fazenda Boa Vista", cortado.Find(".rvm-titulo").TextContent);
        Assert.Equal("Sorriso, MT", cortado.Find(".rvm-apoio").TextContent);
    }

    [Fact]
    public void Imagem_redonda_sem_foto_e_sem_Alt_e_decorativa()
    {
        var cortado = Render<RvmCellCircleImage>(p => p.Add(x => x.Alt, "").Add(x => x.Text, "Fazenda"));

        Assert.Equal("true", cortado.Find(".rvm-avatar").GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Imagem_quadrada_tem_alt_e_tamanho()
    {
        var cortado = Render<RvmCellSquareImage>(p => p
            .Add(x => x.Src, "semente.png")
            .Add(x => x.Alt, "Saco de semente de soja")
            .Add(x => x.Text, "Semente de soja")
            .Add(x => x.Highlight, true));

        var img = cortado.Find("img.rvm-miniatura");
        Assert.Equal("semente.png", img.GetAttribute("src"));
        Assert.Equal("Saco de semente de soja", img.GetAttribute("alt"));
        Assert.Equal("40", img.GetAttribute("width"));
        Assert.Contains("rvm-destaque", cortado.Find(".rvm-celula-texto").ClassList);
    }

    [Fact]
    public void Imagem_quadrada_sem_Alt_e_decorativa_com_alt_vazio()
    {
        var img = Render<RvmCellSquareImage>(p => p.Add(x => x.Src, "a.png")).Find("img");

        Assert.Equal("", img.GetAttribute("alt"));
    }

    [Fact]
    public void Usuario_mostra_o_nome_e_o_avatar_e_decorativo()
    {
        var cortado = Render<RvmCellUser>(p => p
            .Add(x => x.Name, "Ana Ribeiro")
            .Add(x => x.Src, "ana.png")
            .Add(x => x.Supporting, "ana@fazenda.com.br")
            .Add(x => x.Highlight, true));

        Assert.Equal("Ana Ribeiro", cortado.Find(".rvm-titulo").TextContent);
        Assert.Equal("ana@fazenda.com.br", cortado.Find(".rvm-apoio").TextContent);
        Assert.Equal("", cortado.Find("img").GetAttribute("alt"));
        Assert.Contains("rvm-destaque", cortado.Find(".rvm-celula-texto").ClassList);
    }

    [Fact]
    public void Usuario_sem_foto_tem_o_icone_escondido_do_leitor()
    {
        var avatar = Render<RvmCellUser>(p => p.Add(x => x.Name, "Ana Ribeiro")).Find(".rvm-avatar");

        Assert.Equal("true", avatar.GetAttribute("aria-hidden"));
        Assert.NotNull(avatar.QuerySelector("svg"));
    }

    [Fact]
    public void Grupo_de_usuarios_repassa_itens_maximo_e_nome()
    {
        RvmAvatarItem[] pessoas = [new("Ana"), new("Bruno"), new("Carla"), new("Davi"), new("Eva")];
        var cortado = Render<RvmCellUserGroup>(p => p
            .Add(x => x.Items, pessoas)
            .Add(x => x.Max, 3)
            .Add(x => x.Label, "Equipe do talhao 3"));

        var grupo = cortado.Find("[role='group']");
        Assert.Equal("Equipe do talhao 3", grupo.GetAttribute("aria-label"));
        Assert.Contains("+2", grupo.TextContent);
    }

    [Fact]
    public void Grupo_de_usuarios_mostra_quatro_por_padrao()
    {
        RvmAvatarItem[] pessoas = [new("Ana"), new("Bruno"), new("Carla"), new("Davi"), new("Eva"), new("Fabio")];

        var cortado = Render<RvmCellUserGroup>(p => p.Add(x => x.Items, pessoas));

        Assert.Contains("+2", cortado.Find("[role='group']").TextContent);
    }

    // --- Nota e escolha ---

    [Fact]
    public void Nota_e_compacta_so_de_leitura_e_usa_o_AriaLabel()
    {
        var cortado = Render<RvmCellRating>(p => p
            .Add(x => x.Value, 4.5)
            .Add(x => x.AriaLabel, "Nota do fornecedor: 4,5 de 5"));

        var nota = cortado.Find("[role='img']");
        Assert.Equal("Nota do fornecedor: 4,5 de 5", nota.GetAttribute("aria-label"));
        Assert.Contains("rvm-compacta", nota.ClassList);
        Assert.Empty(cortado.FindAll("input"));
    }

    [Fact]
    public void Nota_sem_AriaLabel_ainda_tem_nome()
    {
        var nota = Render<RvmCellRating>(p => p.Add(x => x.Value, 3).Add(x => x.Max, 5)).Find("[role='img']");

        Assert.False(string.IsNullOrWhiteSpace(nota.GetAttribute("aria-label")));
    }

    [Fact]
    public void Escolha_tem_o_Label_como_nome_e_avisa_a_mudanca()
    {
        bool? recebido = null;
        var cortado = Render<RvmCellSelect>(p => p
            .Add(x => x.Label, "Selecionar o lote 4910")
            .Add(x => x.SelectedChanged, (bool v) => recebido = v));

        var caixa = cortado.Find("input[type='checkbox']");
        Assert.Equal("Selecionar o lote 4910", caixa.GetAttribute("aria-label"));

        caixa.Change(true);

        Assert.True(recebido);
    }

    [Fact]
    public void Escolha_marcada_e_desabilitada()
    {
        var caixa = Render<RvmCellSelect>(p => p
            .Add(x => x.Label, "Selecionar")
            .Add(x => x.Selected, true)
            .Add(x => x.Disabled, true)).Find("input[type='checkbox']");

        Assert.True(caixa.HasAttribute("checked"));
        Assert.True(caixa.HasAttribute("disabled"));
    }

    // --- Numero e tendencia ---

    [Fact]
    public void Numero_sem_tendencia_e_so_o_numero()
    {
        var cortado = Render<RvmCellNumber>(p => p.Add(x => x.Text, "R$ 3.428,00").Add(x => x.Highlight, true));

        Assert.Equal("R$ 3.428,00", cortado.Find(".rvm-numero").TextContent);
        Assert.Contains("rvm-destaque", cortado.Find(".rvm-numero").ClassList);
        Assert.Contains("rvm-celula-numero", cortado.Find("td").ClassList);
        Assert.Empty(cortado.FindAll(".rvm-so-leitor"));
    }

    [Fact]
    public void Tendencia_so_com_seta_diz_a_direcao_em_texto()
    {
        var cortado = Render<RvmCellNumber>(p => p.Add(x => x.Text, "120").Add(x => x.Trend, RvmTrend.Up));

        Assert.Equal("Subiu", cortado.Find(".rvm-variacao .rvm-so-leitor").TextContent);
        Assert.Contains("rvm-subiu", cortado.Find(".rvm-variacao").ClassList);
        Assert.Equal("true", cortado.Find(".rvm-variacao svg").GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Tendencia_com_rotulo_vira_etiqueta_e_o_leitor_ouve_direcao_e_valor()
    {
        var cortado = Render<RvmCellNumber>(p => p
            .Add(x => x.Text, "R$ 2.060,00")
            .Add(x => x.Trend, RvmTrend.Down)
            .Add(x => x.TrendLabel, "-10%"));

        var etiqueta = cortado.Find(".rvm-etiqueta");
        Assert.Contains("rvm-error", etiqueta.ClassList);
        Assert.Equal("Caiu", etiqueta.QuerySelector(".rvm-so-leitor")!.TextContent);
        Assert.Contains("-10%", etiqueta.TextContent);
        Assert.Empty(cortado.FindAll(".rvm-variacao"));
    }

    // --- Estado, etiqueta, progresso ---

    [Fact]
    public void Estado_tem_bolinha_decorativa_e_a_cor_do_papel()
    {
        var cortado = Render<RvmCellStatus>(p => p
            .Add(x => x.Text, "Pago")
            .Add(x => x.Color, RvmColor.Success)
            .Add(x => x.Highlight, true));

        Assert.Contains("rvm-success", cortado.Find(".rvm-estado").ClassList);
        Assert.Equal("true", cortado.Find(".rvm-bolinha").GetAttribute("aria-hidden"));
        Assert.Equal("Pago", cortado.Find(".rvm-texto-estado").TextContent);
        Assert.Contains("rvm-destaque", cortado.Find(".rvm-texto-estado").ClassList);
    }

    [Fact]
    public void Estado_padrao_e_neutro()
    {
        Assert.Contains("rvm-secondary", Render<RvmCellStatus>(p => p.Add(x => x.Text, "Rascunho")).Find(".rvm-estado").ClassList);
    }

    [Fact]
    public void Etiqueta_usa_o_RvmLabel_suave_na_cor_pedida()
    {
        var etiqueta = Render<RvmCellLabelBadge>(p => p.Add(x => x.Text, "Pago").Add(x => x.Color, RvmColor.Warning)).Find(".rvm-etiqueta");

        Assert.Equal("Pago", etiqueta.TextContent);
        Assert.Contains("rvm-warning", etiqueta.ClassList);
        Assert.Contains("rvm-suave", etiqueta.ClassList);
    }

    [Fact]
    public void Etiqueta_padrao_e_primaria_e_aceita_conteudo_livre()
    {
        var etiqueta = Render<RvmCellLabelBadge>(p => p.Add(x => x.Text, "x").AddChildContent("<b>Ativo</b>")).Find(".rvm-etiqueta");

        Assert.Contains("rvm-primary", etiqueta.ClassList);
        Assert.Equal("Ativo", etiqueta.TextContent);
    }

    [Fact]
    public void Progresso_usa_a_barra_com_o_valor()
    {
        var cortado = Render<RvmCellProgress>(p => p
            .Add(x => x.Value, 62)
            .Add(x => x.Label, "R$ 12.400")
            .Add(x => x.Color, RvmColor.Success));

        var barra = cortado.Find("[role='progressbar']");
        Assert.Equal("62", barra.GetAttribute("aria-valuenow"));
        Assert.Contains("R$ 12.400", cortado.Markup);
    }

    // --- Arquivo e icone de arquivo ---

    [Fact]
    public void Arquivo_tem_icone_decorativo_nome_e_tamanho()
    {
        var cortado = Render<RvmCellFiles>(p => p
            .Add(x => x.Name, "laudo-solo.pdf")
            .Add(x => x.Type, RvmFileIconName.Pdf)
            .Add(x => x.Supporting, "400 KB")
            .Add(x => x.Highlight, true));

        var icone = cortado.Find("svg.rvm-icone-arquivo");
        Assert.Equal("true", icone.GetAttribute("aria-hidden"));
        Assert.Equal("40", icone.GetAttribute("width"));
        Assert.Contains("PDF", icone.TextContent);
        Assert.Equal("laudo-solo.pdf", cortado.Find(".rvm-titulo").TextContent);
        Assert.Equal("400 KB", cortado.Find(".rvm-apoio").TextContent);
        Assert.Contains("rvm-destaque", cortado.Find(".rvm-celula-texto").ClassList);
    }

    [Fact]
    public void Icone_de_arquivo_com_Label_e_imagem_com_nome()
    {
        var svg = Render<RvmFileIcon>(p => p
            .Add(x => x.Name, RvmFileIconName.Xls)
            .Add(x => x.Label, "Planilha")
            .Add(x => x.Class, "minha")
            .AddUnmatched("data-x", "1")).Find("svg");

        Assert.Equal("img", svg.GetAttribute("role"));
        Assert.Equal("Planilha", svg.GetAttribute("aria-label"));
        Assert.Null(svg.GetAttribute("aria-hidden"));
        Assert.Equal("48", svg.GetAttribute("width"));
        Assert.Contains("minha", svg.ClassList);
        Assert.Contains("rvm-success", svg.ClassList);
        Assert.Equal("1", svg.GetAttribute("data-x"));
    }

    [Fact]
    public void Pasta_e_desenhada_sem_etiqueta()
    {
        var svg = Render<RvmFileIcon>(p => p.Add(x => x.Name, RvmFileIconName.FolderYellow)).Find("svg");

        Assert.NotNull(svg.QuerySelector(".rvm-pasta-frente"));
        Assert.Null(svg.QuerySelector(".rvm-faixa"));
        Assert.Contains("rvm-warning", svg.ClassList);
    }

    [Fact]
    public void Todo_tipo_de_arquivo_tem_um_papel_e_a_extensao_escrita()
    {
        foreach (var tipo in Enum.GetValues<RvmFileIconName>().Where(t => t != RvmFileIconName.FolderYellow))
        {
            var svg = Render<RvmFileIcon>(p => p.Add(x => x.Name, tipo)).Find("svg");
            Assert.Equal(tipo.ToString().ToUpperInvariant(), svg.TextContent.Trim());
        }
    }

    [Fact]
    public void Icone_de_arquivo_recusa_tamanho_nao_positivo()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Render<RvmFileIcon>(p => p.Add(x => x.Size, 0)));
    }
}
