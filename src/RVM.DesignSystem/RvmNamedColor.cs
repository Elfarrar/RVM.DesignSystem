namespace RVM.DesignSystem;

/// <summary>Uma cor com nome, para a paleta do <c>RvmColorField</c> (contrato com o RVM.UI, DSGN-017).</summary>
/// <param name="Name">Nome mostrado e lido pelo leitor de tela ("Verde").</param>
/// <param name="Hex">A cor no formato <c>#RRGGBB</c>.</param>
public sealed record RvmNamedColor(string Name, string Hex);
