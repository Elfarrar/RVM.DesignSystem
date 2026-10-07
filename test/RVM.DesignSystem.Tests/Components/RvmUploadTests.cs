using Microsoft.AspNetCore.Components.Forms;
using RVM.DesignSystem.Components.Upload;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>RvmFileUpload, RvmMediaUpload e RvmProfileImageUpload, do contrato com o RVM.UI (DSGN-017). </summary>
public class RvmUploadTests : BunitContext
{
    public RvmUploadTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private static InputFileContent Arquivo(string nome) => InputFileContent.CreateFromText("conteudo", nome);

    // --- RvmFileUpload ---

    [Fact]
    public void Escolher_arquivos_avisa_OnFilesSelected_e_anuncia()
    {
        IReadOnlyList<IBrowserFile>? recebidos = null;
        var cortado = Render<RvmFileUpload>(p => p
            .Add(x => x.Multiple, true)
            .Add(x => x.OnFilesSelected, (IReadOnlyList<IBrowserFile> f) => recebidos = f));

        cortado.FindComponent<InputFile>().UploadFiles(Arquivo("a.pdf"), Arquivo("b.pdf"));

        Assert.NotNull(recebidos);
        Assert.Equal(["a.pdf", "b.pdf"], recebidos!.Select(f => f.Name));
        Assert.Equal("2 arquivos escolhidos", cortado.Find("[aria-live='polite']").TextContent);
        Assert.Empty(cortado.FindAll(".rvm-mensagem-de-erro"));
    }

    [Fact]
    public void Acima_do_MaxFiles_mostra_erro_e_nao_lanca_nem_avisa()
    {
        var avisou = false;
        var cortado = Render<RvmFileUpload>(p => p
            .Add(x => x.Multiple, true)
            .Add(x => x.MaxFiles, 2)
            .Add(x => x.Id, "anexos")
            .Add(x => x.OnFilesSelected, (IReadOnlyList<IBrowserFile> _) => avisou = true));

        cortado.FindComponent<InputFile>().UploadFiles(Arquivo("a.pdf"), Arquivo("b.pdf"), Arquivo("c.pdf"));

        Assert.False(avisou);
        var erro = cortado.Find(".rvm-mensagem-de-erro");
        Assert.Contains("limite e 2", erro.TextContent);
        Assert.Equal("anexos-erro", erro.Id);
        Assert.Equal("anexos-erro", cortado.Find("input[type=file]").GetAttribute("aria-describedby"));
        Assert.Contains("rvm-erro", cortado.Find(".rvm-upload").ClassList);

        // A escolha seguinte, dentro do limite, limpa o erro.
        cortado.FindComponent<InputFile>().UploadFiles(Arquivo("a.pdf"));
        Assert.True(avisou);
        Assert.Empty(cortado.FindAll(".rvm-mensagem-de-erro"));
    }

    [Fact]
    public void Cartoes_mostram_nome_tamanho_em_pt_BR_e_estado()
    {
        var cortado = Render<RvmFileUpload>(p => p.Add(x => x.Items, new List<RvmUploadItem>
        {
            new() { Name = "contrato.pdf", Size = 1_258_291, Status = RvmUploadStatus.Uploaded },
            new() { Name = "planilha.xlsx", Size = 500, Status = RvmUploadStatus.Failed },
            new() { Name = "fotos.zip", Size = 15_360, Status = RvmUploadStatus.Uploading, Progress = 140 },
            new() { Name = "leia", Size = 3L * 1024 * 1024 * 1024 }
        }));

        var cartoes = cortado.FindAll(".rvm-cartao");
        Assert.Equal(4, cartoes.Count);
        Assert.Equal("contrato.pdf", cartoes[0].QuerySelector(".rvm-nome")!.TextContent);
        Assert.Equal("1,2 MB", cartoes[0].QuerySelector(".rvm-tamanho")!.TextContent);
        Assert.Contains("rvm-enviado", cartoes[0].ClassList);
        Assert.NotNull(cartoes[0].QuerySelector(".rvm-sinal-ok"));
        Assert.Equal("500 B", cartoes[1].QuerySelector(".rvm-tamanho")!.TextContent);
        Assert.Equal("Falha no envio", cartoes[1].QuerySelector(".rvm-falha")!.TextContent);
        Assert.Equal("15 KB", cartoes[2].QuerySelector(".rvm-tamanho")!.TextContent);
        Assert.Equal("3 GB", cartoes[3].QuerySelector(".rvm-tamanho")!.TextContent);
        Assert.Null(cartoes[3].QuerySelector(".rvm-estado"));
    }

    [Fact]
    public void Enviando_mostra_progresso_limitado_a_100()
    {
        var cortado = Render<RvmFileUpload>(p => p.Add(x => x.Items, new List<RvmUploadItem>
        {
            new() { Name = "fotos.zip", Size = 10, Status = RvmUploadStatus.Uploading, Progress = 140 },
            new() { Name = "b.pdf", Size = 10, Status = RvmUploadStatus.Uploading, Progress = 45 }
        }));

        var barras = cortado.FindAll("[role=progressbar]");
        Assert.Equal("100", barras[0].GetAttribute("aria-valuenow"));
        Assert.Equal("45", barras[1].GetAttribute("aria-valuenow"));
        Assert.Equal("Enviando b.pdf", barras[1].GetAttribute("aria-label"));
    }

    [Fact]
    public void X_de_remover_so_com_OnRemove_e_nomeado()
    {
        var itens = new List<RvmUploadItem> { new() { Name = "contrato.pdf", Size = 10 } };
        var semRemover = Render<RvmFileUpload>(p => p.Add(x => x.Items, itens));
        Assert.Empty(semRemover.FindAll(".rvm-remover"));

        RvmUploadItem? removido = null;
        var cortado = Render<RvmFileUpload>(p => p
            .Add(x => x.Items, itens)
            .Add(x => x.OnRemove, (RvmUploadItem i) => removido = i));

        var botao = cortado.Find(".rvm-remover");
        Assert.Equal("Remover contrato.pdf", botao.GetAttribute("aria-label"));
        botao.Click();
        Assert.Same(itens[0], removido);
        Assert.Equal("contrato.pdf removido", cortado.Find("[aria-live='polite']").TextContent);
    }

    [Fact]
    public void Botao_por_padrao_e_caixa_de_soltar_com_Container()
    {
        var botao = Render<RvmFileUpload>(p => p.Add(x => x.Id, "f1").Add(x => x.Hint, "PDF ate 5 MB"));
        Assert.NotNull(botao.Find(".rvm-botao-de-envio"));
        Assert.Equal("Adicionar arquivo", botao.Find("#f1-acao").TextContent);
        Assert.Equal("PDF ate 5 MB", botao.Find("#f1-dica").TextContent);
        var input = botao.Find("input[type=file]");
        Assert.Equal("f1-acao", input.GetAttribute("aria-labelledby"));
        Assert.Equal("f1-dica", input.GetAttribute("aria-describedby"));

        var caixa = Render<RvmFileUpload>(p => p.Add(x => x.Id, "f2").Add(x => x.Container, true).Add(x => x.Label, "Anexos"));
        Assert.Empty(caixa.FindAll(".rvm-botao-de-envio"));
        Assert.Equal("Arraste os arquivos aqui ou clique para escolher", caixa.Find(".rvm-area-de-soltar #f2-acao").TextContent);
        Assert.Empty(caixa.FindAll("#f2-dica"));
        Assert.Equal("f2", caixa.Find("label").GetAttribute("for"));
        Assert.Equal("f2-rotulo f2-acao", caixa.Find("input[type=file]").GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void Accept_multiple_name_e_id_vao_ao_input()
    {
        var cortado = Render<RvmFileUpload>(p => p
            .Add(x => x.Accept, ".pdf")
            .Add(x => x.Multiple, true)
            .Add(x => x.Name, "anexos")
            .Add(x => x.Id, "campo-anexos"));

        var input = cortado.Find("input[type=file]");
        Assert.Equal(".pdf", input.GetAttribute("accept"));
        Assert.True(input.HasAttribute("multiple"));
        Assert.Equal("anexos", input.GetAttribute("name"));
        Assert.Equal("campo-anexos", input.Id);

        var semId = Render<RvmFileUpload>();
        var gerado = semId.Find("input[type=file]");
        Assert.StartsWith("rvm-upload-", gerado.Id);
        Assert.False(gerado.HasAttribute("multiple"));
        Assert.False(gerado.HasAttribute("accept"));
    }

    [Fact]
    public void ErrorText_e_link_e_classe()
    {
        var cortado = Render<RvmFileUpload>(p => p
            .Add(x => x.Id, "f")
            .Add(x => x.ErrorText, "Envie o contrato assinado.")
            .Add(x => x.LinkText, "Modelo")
            .Add(x => x.LinkHref, "/modelo.pdf")
            .Add(x => x.Class, "minha")
            .AddUnmatched("data-teste", "x"));

        Assert.Equal("Envie o contrato assinado.", cortado.Find("#f-erro").TextContent);
        var input = cortado.Find("input[type=file]");
        Assert.Equal("f-erro", input.GetAttribute("aria-describedby"));
        Assert.Equal("true", input.GetAttribute("aria-invalid"));
        Assert.Equal("/modelo.pdf", cortado.Find("a.rvm-link-do-campo").GetAttribute("href"));
        var raiz = cortado.Find(".rvm-upload");
        Assert.Contains("minha", raiz.ClassList);
        Assert.Equal("x", raiz.GetAttribute("data-teste"));
    }

    [Fact]
    public void Disabled_desabilita_input_e_remover()
    {
        var cortado = Render<RvmFileUpload>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.Items, new List<RvmUploadItem> { new() { Name = "a.pdf", Size = 1 } })
            .Add(x => x.OnRemove, (RvmUploadItem _) => { }));

        Assert.True(cortado.Find("input[type=file]").HasAttribute("disabled"));
        Assert.True(cortado.Find(".rvm-remover").HasAttribute("disabled"));
        Assert.Contains("rvm-desabilitado", cortado.Find(".rvm-upload").ClassList);
    }

    // --- RvmMediaUpload ---

    [Fact]
    public void Media_aceita_imagens_e_mostra_miniaturas()
    {
        var cortado = Render<RvmMediaUpload>(p => p.Add(x => x.Items, new List<RvmUploadItem>
        {
            new() { Name = "talhao.jpg", Size = 10, PreviewUrl = "data:image/png;base64,AA==", Status = RvmUploadStatus.Uploaded },
            new() { Name = "sem-previa.png", Size = 10, Status = RvmUploadStatus.Uploading, Progress = 30 }
        }));

        Assert.Equal("image/*", cortado.Find("input[type=file]").GetAttribute("accept"));
        var img = cortado.Find("img.rvm-imagem");
        Assert.Equal("talhao.jpg", img.GetAttribute("alt"));
        Assert.Equal("sem-previa.png", cortado.Find(".rvm-sem-previa").GetAttribute("aria-label"));
        Assert.Equal("30", cortado.Find("[role=progressbar]").GetAttribute("aria-valuenow"));
        Assert.Equal("Adicionar imagem", cortado.Find(".rvm-adicionar .rvm-texto-do-alvo").TextContent);
        Assert.Equal(2, cortado.FindAll("li[role=listitem]").Count);
    }

    [Fact]
    public void Media_falha_tem_Tentar_de_novo_nomeado_e_remover()
    {
        var item = new RvmUploadItem { Name = "talhao.jpg", Size = 10, Status = RvmUploadStatus.Failed };
        var semAcoes = Render<RvmMediaUpload>(p => p.Add(x => x.Items, new List<RvmUploadItem> { item }));
        Assert.Equal("Falha no envio", semAcoes.Find(".rvm-texto-da-falha").TextContent);
        Assert.Empty(semAcoes.FindAll(".rvm-tentar"));
        Assert.Empty(semAcoes.FindAll(".rvm-remover"));

        RvmUploadItem? deNovo = null, removido = null;
        var cortado = Render<RvmMediaUpload>(p => p
            .Add(x => x.Items, new List<RvmUploadItem> { item })
            .Add(x => x.OnRetry, (RvmUploadItem i) => deNovo = i)
            .Add(x => x.OnRemove, (RvmUploadItem i) => removido = i));

        var tentar = cortado.Find(".rvm-tentar");
        Assert.Equal("Tentar de novo talhao.jpg", tentar.GetAttribute("aria-label"));
        tentar.Click();
        Assert.Same(item, deNovo);

        var remover = cortado.Find(".rvm-remover");
        Assert.Equal("Remover talhao.jpg", remover.GetAttribute("aria-label"));
        remover.Click();
        Assert.Same(item, removido);
    }

    [Fact]
    public void Media_escolha_e_limite()
    {
        IReadOnlyList<IBrowserFile>? recebidos = null;
        var cortado = Render<RvmMediaUpload>(p => p
            .Add(x => x.Multiple, true)
            .Add(x => x.MaxFiles, 1)
            .Add(x => x.Hint, "PNG ou JPG")
            .Add(x => x.Id, "m")
            .Add(x => x.OnFilesSelected, (IReadOnlyList<IBrowserFile> f) => recebidos = f));

        cortado.FindComponent<InputFile>().UploadFiles(Arquivo("a.png"), Arquivo("b.png"));
        Assert.Null(recebidos);
        Assert.Contains("so 1 por vez", cortado.Find("#m-erro").TextContent);
        Assert.Equal("m-dica m-erro", cortado.Find("input[type=file]").GetAttribute("aria-describedby"));

        cortado.FindComponent<InputFile>().UploadFiles(Arquivo("a.png"));
        Assert.Single(recebidos!);
        Assert.Equal("1 imagem escolhida", cortado.Find("[aria-live='polite']").TextContent);
    }

    [Fact]
    public void Media_disabled()
    {
        var cortado = Render<RvmMediaUpload>(p => p.Add(x => x.Disabled, true));
        Assert.True(cortado.Find("input[type=file]").HasAttribute("disabled"));
        Assert.Contains("rvm-desabilitado", cortado.Find(".rvm-upload").ClassList);
    }

    // --- RvmProfileImageUpload ---

    [Fact]
    public void Perfil_com_foto_mostra_a_imagem_com_o_nome()
    {
        var cortado = Render<RvmProfileImageUpload>(p => p
            .Add(x => x.Src, "/foto.jpg")
            .Add(x => x.AvatarName, "Rafael Veneroso"));

        var img = cortado.Find(".rvm-avatar img");
        Assert.Equal("/foto.jpg", img.GetAttribute("src"));
        Assert.Equal("Rafael Veneroso", img.GetAttribute("alt"));
        Assert.Contains("rvm-avatar-do-perfil", cortado.Find(".rvm-avatar").ClassList);
        Assert.Equal("Enviar foto", cortado.Find(".rvm-enviar-foto span").TextContent);
        Assert.Equal("image/*", cortado.Find("input[type=file]").GetAttribute("accept"));
    }

    [Fact]
    public void Perfil_sem_foto_mostra_iniciais_ou_silhueta()
    {
        var comNome = Render<RvmProfileImageUpload>(p => p.Add(x => x.AvatarName, "Rafael Veneroso Morici"));
        Assert.Equal("RM", comNome.Find(".rvm-iniciais").TextContent);

        var semNome = Render<RvmProfileImageUpload>();
        Assert.Empty(semNome.FindAll(".rvm-iniciais"));
        Assert.NotNull(semNome.Find(".rvm-avatar svg"));
    }

    [Fact]
    public void Perfil_escolha_entrega_o_arquivo()
    {
        IBrowserFile? recebido = null;
        var cortado = Render<RvmProfileImageUpload>(p => p
            .Add(x => x.Id, "foto")
            .Add(x => x.Name, "foto")
            .Add(x => x.Label, "Foto de perfil")
            .Add(x => x.OnFileSelected, (IBrowserFile f) => recebido = f));

        var input = cortado.Find("input[type=file]");
        Assert.Equal("foto", input.GetAttribute("name"));
        Assert.Equal("foto-rotulo foto-acao", input.GetAttribute("aria-labelledby"));
        Assert.False(input.HasAttribute("multiple"));

        cortado.FindComponent<InputFile>().UploadFiles(Arquivo("eu.jpg"));
        Assert.Equal("eu.jpg", recebido!.Name);
        Assert.Equal("Foto escolhida: eu.jpg", cortado.Find("[aria-live='polite']").TextContent);
    }

    [Fact]
    public void Perfil_erro_e_disabled()
    {
        var cortado = Render<RvmProfileImageUpload>(p => p
            .Add(x => x.Id, "foto")
            .Add(x => x.ErrorText, "A foto passa de 5 MB.")
            .Add(x => x.Disabled, true));

        var input = cortado.Find("input[type=file]");
        Assert.Equal("foto-erro", input.GetAttribute("aria-describedby"));
        Assert.True(input.HasAttribute("disabled"));
        Assert.Equal("A foto passa de 5 MB.", cortado.Find("#foto-erro").TextContent);
        Assert.Contains("rvm-desabilitado", cortado.Find(".rvm-upload").ClassList);
    }
}
