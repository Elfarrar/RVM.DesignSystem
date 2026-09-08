namespace RVM.DesignSystem.Components;

/// <summary>
/// O que cada severidade significa em icone e em sufixo de classe.
/// </summary>
/// <remarks>
/// Existe para que <see cref="RvmAlert"/> e o toast concordem. Duas tabelas equivalentes em
/// arquivos diferentes divergem na primeira severidade nova — e o sintoma seria um alerta de
/// erro com o icone de sucesso, que e pior que icone nenhum.
/// </remarks>
internal static class RvmSeveridade
{
    /// <summary>O icone do conjunto Phosphor que representa a severidade.</summary>
    /// <param name="severidade">A severidade.</param>
    /// <returns>Nome do icone, sempre existente no conjunto curado.</returns>
    public static string Icone(RvmSeverity severidade) => severidade switch
    {
        RvmSeverity.Success => "check-circle",
        // Triangulo para "atencao", circulo com X para "erro": as duas formas sao distinguiveis
        // SEM cor, que e o ponto — severidade nunca pode depender so dela.
        RvmSeverity.Warning => "warning",
        RvmSeverity.Danger => "x-circle",
        _ => "info",
    };
}
