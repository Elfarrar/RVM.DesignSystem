namespace RVM.DesignSystem.Components.Stepper;

/// <summary>Uma etapa do <see cref="RvmStepper"/>.</summary>
/// <param name="Title">O nome da etapa ("Dados da conta").</param>
/// <param name="Description">A linha de apoio ("Informe e-mail e senha").</param>
/// <param name="HasError">A etapa tem problema a resolver — a validacao do "com validacao" do catalogo.</param>
public sealed record RvmStep(string Title, string? Description = null, bool HasError = false);
