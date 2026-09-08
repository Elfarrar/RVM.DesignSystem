namespace RVM.DesignSystem.Components;

/// <summary>
/// Montagem da lista de classes de um componente.
/// </summary>
/// <remarks>
/// Existe para que a regra "<c>Class</c> do consumidor <b>soma</b> com as internas, nunca é
/// descartado" (`CLAUDE.md` § Convenções) tenha <b>um</b> ponto de verdade em vez de doze cópias
/// do mesmo <c>string.Join</c>. A classe do consumidor entra por último de propósito: com
/// especificidade igual, o CSS dele vence o nosso — que é o que "soma" significa na prática.
/// </remarks>
internal static class RvmClasse
{
    /// <summary>Junta as partes não vazias, na ordem dada.</summary>
    public static string Juntar(params string?[] partes) =>
        string.Join(' ', partes.Where(p => !string.IsNullOrWhiteSpace(p)));

    /// <summary>O sufixo kebab-case de um membro de enum (<c>SpaceBetween</c> → <c>space-between</c>).</summary>
    public static string Sufixo<TEnum>(TEnum valor) where TEnum : struct, Enum
    {
        var nome = valor.ToString()!;
        var saida = new System.Text.StringBuilder(nome.Length + 4);

        for (var i = 0; i < nome.Length; i++)
        {
            if (char.IsUpper(nome[i]) && i > 0)
            {
                saida.Append('-');
            }

            saida.Append(char.ToLowerInvariant(nome[i]));
        }

        return saida.ToString();
    }
}
