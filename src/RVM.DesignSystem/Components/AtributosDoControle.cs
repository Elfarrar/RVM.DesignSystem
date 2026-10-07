namespace RVM.DesignSystem.Components;

/// <summary>
/// Os atributos do input nativo da caixa de marcar e da chave: os extras do consumidor (menos <c>class</c> e
/// <c>style</c>, que ficam na raiz) mais o <c>Id</c>, o <c>Name</c> e a ligacao com o texto de apoio ou de erro
/// (contrato com o RVM.UI, DSGN-017).
/// </summary>
internal static class AtributosDoControle
{
    public static IReadOnlyDictionary<string, object>? Montar(
        IReadOnlyDictionary<string, object>? extras, string? id, string? nome, string? idDoApoio, bool erro, bool obrigatorio)
    {
        var atributos = extras?
            .Where(a => !string.Equals(a.Key, "class", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(a.Key, "style", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(a => a.Key, a => a.Value)
            ?? [];

        // O InputCheckbox do Blazor le o "name" dos atributos antes de gerar o dele a partir do @bind.
        if (!string.IsNullOrWhiteSpace(id)) atributos["id"] = id;
        if (!string.IsNullOrWhiteSpace(nome)) atributos["name"] = nome;
        if (idDoApoio is not null) atributos["aria-describedby"] = idDoApoio;
        if (erro) atributos["aria-invalid"] = "true";
        if (obrigatorio) atributos["aria-required"] = "true";

        return atributos.Count == 0 ? null : atributos;
    }
}
