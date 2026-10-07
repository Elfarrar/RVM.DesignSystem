namespace RVM.DesignSystem.Components.Chart;

/// <summary>Uma fatia de grafico radial: o nome da legenda e da tabela, e o valor (contrato com o RVM.UI, DSGN-017).</summary>
/// <param name="Name">Nome da fatia. Vai para o texto que o leitor de tela le.</param>
/// <param name="Value">O valor. O grafico calcula a porcentagem sobre a soma; os valores nao precisam somar 100.</param>
/// <param name="Color">
/// Cor propria da fatia, em hexadecimal (<c>#RGB</c> ou <c>#RRGGBB</c>), para quando a cor e o dado do dominio. Sem ela,
/// a fatia usa a <see cref="RvmChartPalette"/>. So hexadecimal, validado: a cor vai para um <c>style</c>, e texto livre
/// ali abriria injecao de CSS. Quem usa cor propria diz o significado em texto tambem.
/// </param>
public sealed record RvmChartSlice(string Name, double Value, string? Color = null);
