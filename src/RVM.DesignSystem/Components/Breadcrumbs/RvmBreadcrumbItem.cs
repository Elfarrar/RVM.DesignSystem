using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Breadcrumbs;

/// <summary>Um passo da trilha.</summary>
/// <param name="Text">O texto do passo.</param>
/// <param name="Href">Para onde o passo leva. Sem valor, o passo e so texto.</param>
/// <param name="Icon">Icone antes do texto.</param>
/// <param name="Disabled">Visivel, mas sem link.</param>
public sealed record RvmBreadcrumbItem(string Text, string? Href = null, RvmIconName? Icon = null, bool Disabled = false);
