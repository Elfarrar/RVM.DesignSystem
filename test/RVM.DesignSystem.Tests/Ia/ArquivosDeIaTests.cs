using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using RVM.DesignSystem.Components.Button;

namespace RVM.DesignSystem.Tests.Ia;

/// <summary>
/// Os tres arquivos para agentes de IA (DSGN-016) sao GERADOS da biblioteca e commitados no wwwroot do
/// site, de onde tambem entram no pacote. Este teste e a sincronia: mexeu no CSS dos tokens ou na API
/// de um componente sem regerar, ele reprova. Para regerar:
/// <c>RVM_ATUALIZAR_IA=1 dotnet test --filter FullyQualifiedName~ArquivosDeIaTests</c>.
/// </summary>
public class ArquivosDeIaTests
{
    private static readonly string PastaDoSite = Path.Combine(RaizDoRepositorio.Caminho, "src", "RVM.DesignSystem.Docs", "wwwroot");

    private static IReadOnlyList<TokensDtcg.Token> Tokens()
        => TokensDtcg.Ler(File.ReadAllText(RaizDoRepositorio.Biblioteca("wwwroot", "rvm-design-system.css")));

    private static ApiParaIa Api()
    {
        var biblioteca = typeof(RvmButton).Assembly;
        var xml = Path.ChangeExtension(biblioteca.Location, ".xml");
        return new ApiParaIa(biblioteca, XDocument.Load(xml), CatalogoDoSite());
    }

    /// <summary>
    /// Le o <c>Catalogo.cs</c> do site (rota, nome e resumo de cada pagina de componente) como texto: o
    /// projeto de teste nao referencia o site WASM, e o catalogo ja e a fonte unica do menu.
    /// </summary>
    private static Dictionary<string, (string Rota, string Resumo)> CatalogoDoSite()
    {
        var fonte = File.ReadAllText(Path.Combine(RaizDoRepositorio.Caminho, "src", "RVM.DesignSystem.Docs", "Shared", "Catalogo.cs"));
        return Regex.Matches(fonte, """new\("(componentes/[^"]+)", "(Rvm\w+)", "([^"]*)"\)""")
            .ToDictionary(m => m.Groups[2].Value, m => (m.Groups[1].Value, m.Groups[3].Value));
    }

    public static TheoryData<string> Arquivos => ["tokens.json", "llms.txt", "llms-full.txt"];

    private static string Gerar(string arquivo) => arquivo switch
    {
        "tokens.json" => TokensDtcg.Gerar(Tokens()),
        "llms.txt" => Api().GerarIndice(),
        _ => Api().GerarCompleto(Tokens())
    };

    [Theory]
    [MemberData(nameof(Arquivos))]
    public void Arquivo_para_ia_esta_em_dia_com_a_biblioteca(string arquivo)
    {
        var caminho = Path.Combine(PastaDoSite, arquivo);
        var esperado = Gerar(arquivo);

        if (Environment.GetEnvironmentVariable("RVM_ATUALIZAR_IA") == "1")
        {
            File.WriteAllText(caminho, esperado);
        }

        Assert.True(File.Exists(caminho), $"{arquivo} nao existe. Gere com RVM_ATUALIZAR_IA=1 dotnet test --filter FullyQualifiedName~ArquivosDeIaTests");
        var atual = File.ReadAllText(caminho).Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.True(atual == esperado,
            $"{arquivo} esta desatualizado em relacao a biblioteca. Regere com RVM_ATUALIZAR_IA=1 dotnet test --filter FullyQualifiedName~ArquivosDeIaTests e commite o resultado.");
    }

    [Fact]
    public void Todo_token_do_css_vai_para_o_json_com_a_variavel_e_o_escuro()
    {
        var tokens = Tokens();
        var json = JsonNode.Parse(TokensDtcg.Gerar(tokens))!;

        Assert.True(tokens.Count > 150, $"So {tokens.Count} tokens lidos — o parser perdeu o bloco?");
        foreach (var token in tokens)
        {
            var (grupo, chave) = TokensDtcg.Separar(token.Nome);
            var extensao = json[grupo]![chave]!["$extensions"]![TokensDtcg.Extensao]!;
            Assert.Equal(token.Variavel, (string?)extensao["cssVar"]);
            Assert.Equal(token.Escuro, (string?)extensao["css"]!["dark"]);
        }
    }

    [Fact]
    public void Alias_do_json_aponta_para_um_token_que_existe()
    {
        var json = JsonNode.Parse(TokensDtcg.Gerar(Tokens()))!;

        var aliases = json.AsObject().Where(g => g.Value is JsonObject)
            .SelectMany(g => g.Value!.AsObject().Select(t => (string?)(t.Value!["$value"] as JsonValue)?.ToString()))
            .Where(v => v is not null && v.StartsWith('{'))
            .ToArray();

        Assert.NotEmpty(aliases);
        foreach (var alias in aliases)
        {
            var partes = alias!.Trim('{', '}').Split('.');
            Assert.NotNull(json[partes[0]]?[partes[1]]);
        }
    }

    [Theory]
    [InlineData("#264CC8", "color")]
    [InlineData("rgba(58, 53, 65, 0.72)", "color")]
    [InlineData("0.25rem", "dimension")]
    [InlineData("6px", "dimension")]
    [InlineData("150ms ease", "transition")]
    [InlineData("0 1px 2px rgba(52, 51, 73, 0.47)", "shadow")]
    [InlineData("linear-gradient(90deg, #31A1F9 0%, #274FCA 100%)", "gradient")]
    [InlineData("50%", null)]
    [InlineData("var(--rvm-color-primary-text)", null)]
    public void Valor_css_vira_o_tipo_dtcg_certo(string valor, string? tipo)
        => Assert.Equal(tipo, TokensDtcg.Converter(valor, "x").Tipo);

    [Fact]
    public void Peso_de_fonte_e_familia_tem_tipo_proprio()
    {
        Assert.Equal("fontWeight", TokensDtcg.Converter("500", "h1-weight").Tipo);
        Assert.Equal("number", TokensDtcg.Converter("3", "x").Tipo);

        var (tipo, valor) = TokensDtcg.Converter("'Inter', system-ui, sans-serif", "family");
        Assert.Equal("fontFamily", tipo);
        Assert.Equal(["Inter", "system-ui", "sans-serif"], valor!.AsArray().Select(v => (string)v!));
    }

    [Fact]
    public void Sombra_e_degrade_saem_decompostos()
    {
        var sombra = TokensDtcg.Converter("0 6px 11px rgba(52, 51, 73, 0.3)", "3").Valor!;
        Assert.Equal("0px", (string?)sombra["offsetX"]);
        Assert.Equal("11px", (string?)sombra["blur"]);
        Assert.Equal("rgba(52, 51, 73, 0.3)", (string?)sombra["color"]);

        var degrade = TokensDtcg.Converter("linear-gradient(90deg, #31A1F9 0%, #274FCA 100%)", "g").Valor!.AsArray();
        Assert.Equal(2, degrade.Count);
        Assert.Equal(1.0, (double)degrade[1]!["position"]!);
    }

    [Fact]
    public void Token_so_no_escuro_e_erro_de_fonte()
    {
        const string css = ":root,\n[data-theme='light'] { --rvm-a-b: 1px; }\n[data-theme='dark'] { --rvm-a-c: 2px; }";

        var erro = Assert.Throws<InvalidOperationException>(() => TokensDtcg.Ler(css));
        Assert.Contains("rvm-a-c", erro.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void O_json_gerado_e_json_valido()
        => JsonDocument.Parse(TokensDtcg.Gerar(Tokens())).Dispose();

    [Fact]
    public void Todo_componente_publico_aparece_no_indice_e_na_api_completa()
    {
        var api = Api();
        var indice = api.GerarIndice();
        var completo = api.GerarCompleto(Tokens());
        var componentes = api.Componentes();

        Assert.True(componentes.Count >= 35, $"So {componentes.Count} componentes achados");
        foreach (var tipo in componentes)
        {
            var nome = ApiParaIa.NomeDoTipo(tipo);
            Assert.Contains(nome, indice, StringComparison.Ordinal);
            Assert.Contains($"### {nome}\n", completo, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Toda_pagina_do_catalogo_casa_com_um_componente_e_vira_link()
    {
        // Se o regex do Catalogo.cs parar de casar, os links e os resumos somem calados do llms.txt.
        var catalogo = CatalogoDoSite();
        var indice = Api().GerarIndice();

        Assert.True(catalogo.Count >= 35, $"So {catalogo.Count} paginas lidas do Catalogo.cs");
        foreach (var (nome, (rota, _)) in catalogo)
        {
            Assert.Contains($"[{nome}", indice, StringComparison.Ordinal);
            Assert.Contains($"{ApiParaIa.Site}/{rota})", indice, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Parametro_sai_com_tipo_padrao_e_descricao_do_xml_doc()
    {
        var parametros = Api().Parametros(typeof(RvmButton)).ToDictionary(p => p.Nome);

        Assert.Equal("RvmButtonVariant", parametros["Variant"].Tipo);
        Assert.Equal("`RvmButtonVariant.Contained`", parametros["Variant"].Padrao);
        Assert.Contains("Peso visual", parametros["Variant"].Descricao, StringComparison.Ordinal);
        Assert.Equal("`false`", parametros["Disabled"].Padrao);
        Assert.Equal("RvmIconName?", parametros["StartIcon"].Tipo);
    }

    [Fact]
    public void Summary_vira_texto_corrido_com_codigo_em_crase()
    {
        var xml = XElement.Parse("""
            <summary>
              Padrao: <see cref="F:RVM.DesignSystem.RvmColor.Primary"/> e <c>aria-busy</c>,
              ver <see cref="T:RVM.DesignSystem.Components.Table.RvmDataGrid`1"/>.
            </summary>
            """);

        Assert.Equal("Padrao: `RvmColor.Primary` e `aria-busy`, ver `RvmDataGrid`.", ApiParaIa.Texto(xml));
    }

    [Fact]
    public void Nome_de_tipo_sai_como_em_csharp()
    {
        Assert.Equal("int?", ApiParaIa.NomeDoTipo(typeof(int?)));
        Assert.Equal("IReadOnlyList<string>", ApiParaIa.NomeDoTipo(typeof(IReadOnlyList<string>)));
        Assert.Equal("double[]", ApiParaIa.NomeDoTipo(typeof(double[])));
        Assert.Equal("EventCallback<bool>", ApiParaIa.NomeDoTipo(typeof(Microsoft.AspNetCore.Components.EventCallback<bool>)));
    }
}
