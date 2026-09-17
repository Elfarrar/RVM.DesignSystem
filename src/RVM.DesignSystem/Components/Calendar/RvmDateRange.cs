namespace RVM.DesignSystem.Components.Calendar;

/// <summary>Um intervalo de datas. <see cref="End"/> vazio enquanto a pessoa ainda escolhe o fim.</summary>
/// <param name="Start">Primeiro dia.</param>
/// <param name="End">Ultimo dia (inclusive).</param>
public readonly record struct RvmDateRange(DateOnly Start, DateOnly? End)
{
    /// <summary>O dia esta dentro do intervalo (inclusive nas pontas).</summary>
    public bool Contains(DateOnly dia) => dia >= Start && dia <= (End ?? Start);
}
