// API publica de uma biblioteca Blazor por reflexao: componentes Rvm* (parametros com heranca) e enums Rvm*.
// Copia do tools/contrato-api/ApiPublica.cs do RVM.UI (RUI-066): os dois repos tem que ler a API do mesmo jeito.
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Tests.Contrato;

/// <summary>Componente -> parametro -> tipo, e enum -> membros. Nome sem namespace.</summary>
internal sealed record ApiPublica(
    SortedDictionary<string, SortedDictionary<string, string>> Componentes,
    SortedDictionary<string, string[]> Enums)
{
    private static readonly JsonSerializerOptions Opcoes = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        // Genericos legiveis no arquivo versionado: RvmTable<TItem> sem o escape unicode do "<".
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    /// <summary>
    /// Le a API publica. Parametro <c>[Obsolete]</c> fica de fora: e alias temporario do nome antigo, nao contrato.
    /// </summary>
    public static ApiPublica Ler(Assembly asm)
    {
        var tipos = asm.GetExportedTypes();
        var componentes = new SortedDictionary<string, SortedDictionary<string, string>>(StringComparer.Ordinal);
        foreach (var t in tipos.Where(t => t.IsClass && t.Name.StartsWith("Rvm", StringComparison.Ordinal) && typeof(IComponent).IsAssignableFrom(t)))
        {
            componentes[Nome(t)] = new SortedDictionary<string, string>(
                t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.GetCustomAttribute<ParameterAttribute>() is not null && p.GetCustomAttribute<ObsoleteAttribute>() is null)
                    .ToDictionary(p => p.Name, p => Nome(p.PropertyType)),
                StringComparer.Ordinal);
        }

        var enums = new SortedDictionary<string, string[]>(StringComparer.Ordinal);
        foreach (var t in tipos.Where(t => t.IsEnum && t.Name.StartsWith("Rvm", StringComparison.Ordinal)))
        {
            enums[t.Name] = [.. Enum.GetNames(t).Order(StringComparer.Ordinal)];
        }

        return new ApiPublica(componentes, enums);
    }

    public string ParaJson() => JsonSerializer.Serialize(this, Opcoes) + "\n";

    public static ApiPublica DeJson(string json) => JsonSerializer.Deserialize<ApiPublica>(json, Opcoes)!;

    /// <summary>Nome sem namespace: o contrato compara RVM.UI e RVM.DesignSystem, que so diferem nele.</summary>
    public static string Nome(Type t)
    {
        if (Nullable.GetUnderlyingType(t) is { } n) return Nome(n) + "?";
        if (t.IsArray) return Nome(t.GetElementType()!) + "[]";
        if (!t.IsGenericType) return Alias(t) ?? t.Name;
        var nome = t.Name[..t.Name.IndexOf('`')];
        return $"{nome}<{string.Join(", ", t.GetGenericArguments().Select(Nome))}>";
    }

    // Enum fica de fora: Type.GetTypeCode de um enum devolve o tipo por baixo dele (Int32), e todo enum viraria "int".
    private static string? Alias(Type t) => t.IsEnum ? null : Type.GetTypeCode(t) switch
    {
        TypeCode.String => "string",
        TypeCode.Int32 => "int",
        TypeCode.Int64 => "long",
        TypeCode.Boolean => "bool",
        TypeCode.Double => "double",
        TypeCode.Single => "float",
        TypeCode.Decimal => "decimal",
        _ when t == typeof(object) => "object",
        _ => null,
    };
}
