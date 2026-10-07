namespace RVM.DesignSystem.Components;

/// <summary>
/// As classes do elemento raiz: as do proprio componente, mais o <c>Class</c> do contrato com o RVM.UI (DSGN-017),
/// mais um <c>class</c> que ainda chegue pelos atributos repassados. O Blazor casa parametro sem olhar a caixa, entao
/// <c>class="x"</c> no consumidor cai no <c>Class</c>; o atributo fica coberto para quem monta os parametros a mao.
/// </summary>
internal static class ClassesCss
{
    public static string Juntar(string proprias, string? classe, IReadOnlyDictionary<string, object>? atributos)
    {
        var repassada = atributos is not null && atributos.TryGetValue("class", out var valor) && valor is string texto ? texto : null;
        return string.Join(' ', new[] { proprias, classe, repassada }.Where(c => !string.IsNullOrWhiteSpace(c)));
    }
}
