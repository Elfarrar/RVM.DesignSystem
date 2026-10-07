namespace RVM.DesignSystem.Components.Map;

/// <summary>
/// Um ponto marcado no <see cref="RvmMap"/> (contrato com o RVM.UI, DSGN-017).
/// </summary>
/// <param name="Label">Nome do lugar. E o que o leitor de tela anuncia.</param>
/// <param name="Latitude">Graus, de -90 a 90.</param>
/// <param name="Longitude">Graus, de -180 a 180.</param>
/// <param name="Value">Valor ja escrito ("320 ha"). O componente nao formata numero.</param>
/// <param name="Color">Cor por papel. Padrao: <see cref="RvmColor.Accent"/>.</param>
public sealed record RvmMapMarker(
    string Label,
    double Latitude,
    double Longitude,
    string? Value = null,
    RvmColor Color = RvmColor.Accent);
