namespace RVM.DesignSystem.Components.Tabs;

/// <summary>Estilo das abas — as duas linhas da pagina Tabs do kit.</summary>
public enum RvmTabsVariant
{
    /// <summary>Aba ativa sublinhada ("Basic Tabs").</summary>
    Standard,

    /// <summary>Aba ativa preenchida na cor primaria ("Customized Tabs").</summary>
    Contained,

    /// <summary>As abas sublinhadas dentro de uma faixa de superficie com borda embaixo, no topo da pagina.</summary>
    Page,

    // Aliases do contrato com o RVM.UI (DSGN-017): o mesmo valor, o nome que o RVM.UI usa.

    /// <summary>O mesmo que <see cref="Standard"/>.</summary>
    Regular = Standard,

    /// <summary>O mesmo que <see cref="Contained"/>.</summary>
    Button = Contained
}
