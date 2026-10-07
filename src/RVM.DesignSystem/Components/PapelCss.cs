namespace RVM.DesignSystem.Components;

/// <summary>
/// A classe CSS do papel de cor (<c>rvm-primary</c>, <c>rvm-error</c>...). Switch, e nao <c>ToString()</c>: com os
/// aliases do contrato (<c>Accent = Primary</c>) o nome do enum e ambiguo (DSGN-017).
/// </summary>
internal static class PapelCss
{
    public static string Classe(RvmColor cor) => cor switch
    {
        RvmColor.Secondary => "rvm-secondary",
        RvmColor.Inverse => "rvm-inverse",
        RvmColor.Info => "rvm-info",
        RvmColor.Success => "rvm-success",
        RvmColor.Warning => "rvm-warning",
        RvmColor.Error => "rvm-error",
        _ => "rvm-primary"
    };

    /// <summary>Os sete papeis, na ordem das regras de CSS geradas.</summary>
    public static readonly string[] Todos = ["primary", "secondary", "inverse", "info", "success", "warning", "error"];
}
