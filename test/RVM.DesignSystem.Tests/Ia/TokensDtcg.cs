using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace RVM.DesignSystem.Tests.Ia;

/// <summary>
/// Le os tokens de <c>rvm-design-system.css</c> e escreve o <c>tokens.json</c> no formato DTCG (W3C
/// Design Tokens). O CSS continua sendo a fonte da verdade: o JSON e derivado, e o teste de sincronia
/// reprova quando os dois divergem (DSGN-016).
/// </summary>
internal static class TokensDtcg
{
    /// <summary>Chave das extensoes proprias, no padrao de dominio invertido que o DTCG pede.</summary>
    internal const string Extensao = "br.com.rvmit.design";

    /// <summary>Um token: o nome sem o <c>--</c>, o valor no claro e, se o escuro redefine, no escuro.</summary>
    internal sealed record Token(string Nome, string Claro, string? Escuro)
    {
        internal string Variavel => "--" + Nome;
    }

    internal static IReadOnlyList<Token> Ler(string css)
    {
        var semComentarios = CssDosComponentesTests.SemComentarios(css);
        var claro = Declaracoes(Bloco(semComentarios, @":root,\s*\[data-theme='light'\]\s*\{"));
        var escuro = Declaracoes(Bloco(semComentarios, @"\[data-theme='dark'\]\s*\{"));

        var soNoEscuro = escuro.Keys.Except(claro.Keys).ToArray();
        if (soNoEscuro.Length > 0)
        {
            throw new InvalidOperationException(
                "Token definido so no tema escuro (o claro e a base e precisa ter todos): " + string.Join(", ", soNoEscuro));
        }

        return claro.Select(par => new Token(par.Key, par.Value, escuro.GetValueOrDefault(par.Key))).ToArray();
    }

    internal static string Gerar(IReadOnlyList<Token> tokens)
    {
        var raiz = new JsonObject
        {
            ["$description"] = "Tokens do RVM Design System, gerados de rvm-design-system.css (a fonte da verdade). "
                               + "$value e o tema claro; o escuro esta em $extensions." + Extensao + ".modes.dark. "
                               + "Em CSS, use sempre var(cssVar), nunca o valor literal. Visual derivado do NEATLAB - "
                               + "Super Admin Dashboard UI Design Kit, de hello.uiworld (Figma Community), sob CC BY 4.0."
        };

        foreach (var token in tokens)
        {
            var (grupo, chave) = Separar(token.Nome);
            if (raiz[grupo] is not JsonObject nodoDoGrupo)
            {
                nodoDoGrupo = new JsonObject();
                raiz[grupo] = nodoDoGrupo;
            }

            var (tipo, valor) = Converter(token.Claro, chave);
            var nodo = new JsonObject();
            if (tipo is not null)
            {
                nodo["$type"] = tipo;
            }

            nodo["$value"] = valor;

            var css = new JsonObject { ["light"] = token.Claro };
            var extensao = new JsonObject { ["cssVar"] = token.Variavel, ["css"] = css };
            if (token.Escuro is not null)
            {
                css["dark"] = token.Escuro;
                extensao["modes"] = new JsonObject { ["dark"] = Converter(token.Escuro, chave).Valor };
            }

            nodo["$extensions"] = new JsonObject { [Extensao] = extensao };
            nodoDoGrupo[chave] = nodo;
        }

        var opcoes = new JsonSerializerOptions
        {
            WriteIndented = true,
            // Sem isto o apostrofo de 'Inter' e o "+" viram ' e + — valido, mas ilegivel.
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        return raiz.ToJsonString(opcoes).Replace("\r\n", "\n", StringComparison.Ordinal) + "\n";
    }

    /// <summary><c>rvm-color-primary-main</c> vira o grupo <c>color</c> e o token <c>primary-main</c>.</summary>
    internal static (string Grupo, string Chave) Separar(string nome)
    {
        var partes = nome.Split('-', 3);
        return (partes[1], partes[2]);
    }

    /// <summary>
    /// Traduz o valor CSS para o tipo DTCG. O que o formato nao representa (como <c>50%</c>) sai sem
    /// <c>$type</c>, com o texto cru — o valor exato continua em <c>$extensions.css</c>.
    /// </summary>
    internal static (string? Tipo, JsonNode? Valor) Converter(string valor, string chave)
    {
        var referencia = Regex.Match(valor, @"^var\(--rvm-([\w-]+)\)$");
        if (referencia.Success)
        {
            // Alias DTCG: o tipo vem do token apontado.
            var (grupo, nome) = Separar("rvm-" + referencia.Groups[1].Value);
            return (null, JsonValue.Create($"{{{grupo}.{nome}}}"));
        }

        if (EhCor(valor))
        {
            return ("color", JsonValue.Create(valor));
        }

        if (Regex.IsMatch(valor, @"^-?\d+(\.\d+)?(rem|px)$"))
        {
            return ("dimension", JsonValue.Create(valor));
        }

        if (int.TryParse(valor, NumberStyles.None, CultureInfo.InvariantCulture, out var numero))
        {
            return (chave.EndsWith("weight", StringComparison.Ordinal) ? "fontWeight" : "number", JsonValue.Create(numero));
        }

        var transicao = Regex.Match(valor, @"^(\d+ms) ease$");
        if (transicao.Success)
        {
            return ("transition", new JsonObject
            {
                ["duration"] = transicao.Groups[1].Value,
                ["delay"] = "0ms",
                ["timingFunction"] = new JsonArray(0.25, 0.1, 0.25, 1)
            });
        }

        var sombra = Regex.Match(valor, @"^(\S+) (\S+) (\S+) (rgba\([^)]*\)|#[0-9A-Fa-f]{3,8})$");
        if (sombra.Success)
        {
            return ("shadow", new JsonObject
            {
                ["color"] = sombra.Groups[4].Value,
                ["offsetX"] = Px(sombra.Groups[1].Value),
                ["offsetY"] = Px(sombra.Groups[2].Value),
                ["blur"] = Px(sombra.Groups[3].Value),
                ["spread"] = "0px"
            });
        }

        var degrade = Regex.Match(valor, @"^linear-gradient\(\s*[^,]+,(.*)\)$");
        if (degrade.Success)
        {
            var paradas = new JsonArray();
            foreach (Match parada in Regex.Matches(degrade.Groups[1].Value, @"(#[0-9A-Fa-f]{3,8})\s+(\d+)%"))
            {
                paradas.Add(new JsonObject
                {
                    ["color"] = parada.Groups[1].Value,
                    ["position"] = int.Parse(parada.Groups[2].Value, CultureInfo.InvariantCulture) / 100.0
                });
            }

            return ("gradient", paradas);
        }

        if (chave == "family")
        {
            return ("fontFamily", new JsonArray(valor.Split(',')
                .Select(f => (JsonNode?)JsonValue.Create(f.Trim().Trim('\'', '"')))
                .ToArray()));
        }

        return (null, JsonValue.Create(valor));
    }

    private static bool EhCor(string valor)
        => Regex.IsMatch(valor, @"^#[0-9A-Fa-f]{3,8}$") || Regex.IsMatch(valor, @"^rgba?\([^)]*\)$");

    private static string Px(string valor) => valor == "0" ? "0px" : valor;

    private static string Bloco(string css, string abertura)
    {
        var inicio = Regex.Match(css, abertura);
        if (!inicio.Success)
        {
            throw new InvalidOperationException("Bloco de tema nao encontrado no CSS: " + abertura);
        }

        var fim = css.IndexOf('}', inicio.Index + inicio.Length);
        return css[(inicio.Index + inicio.Length)..fim];
    }

    private static Dictionary<string, string> Declaracoes(string bloco)
        => Regex.Matches(bloco, @"--(rvm-[\w-]+)\s*:\s*([^;]+);")
            .ToDictionary(m => m.Groups[1].Value, m => Regex.Replace(m.Groups[2].Value.Trim(), @"\s+", " "));
}
