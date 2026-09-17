using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Progress;

/// <summary>
/// Progresso de uma tarefa: com valor (determinado) ou so "esta andando" (indeterminado).
/// </summary>
public partial class RvmProgress : ComponentBase
{
    internal const double Raio = 20.2;
    internal const double Espessura = 3.6;
    private static readonly double Circunferencia = 2 * Math.PI * Raio;

    /// <summary>Barra (padrao) ou anel.</summary>
    [Parameter] public RvmProgressVariant Variant { get; set; } = RvmProgressVariant.Linear;

    /// <summary>
    /// Porcentagem de 0 a 100. Sem valor, o indicador e indeterminado — use quando nao da para saber
    /// quanto falta.
    /// </summary>
    [Parameter] public double? Value { get; set; }

    /// <summary>Papel de cor. Padrao: <see cref="RvmColor.Primary"/>.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Primary;

    /// <summary>
    /// Anel de 24, 40 (padrao) ou 56 px — os tamanhos do avatar. A barra tem sempre 4 px de altura.
    /// </summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>
    /// Nome para o leitor de tela: o que esta progredindo ("Enviando planilha"). Padrao: "Carregando".
    /// </summary>
    [Parameter] public string Label { get; set; } = "Carregando";

    /// <summary>Mostra a porcentagem ao lado da barra ou dentro do anel. So no determinado.</summary>
    [Parameter] public bool ShowValue { get; set; }

    /// <summary>Atributos extras, repassados a raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal bool Indeterminado => Value is null;

    /// <summary>O valor dentro de 0..100 — fora disso a barra passaria da borda.</summary>
    internal double ValorLimitado => Math.Clamp(Value ?? 0, 0, 100);

    internal string ValorArredondado => Math.Round(ValorLimitado).ToString(CultureInfo.InvariantCulture);

    internal string? EstiloDaBarra
        => Indeterminado ? null : $"width: {ValorLimitado.ToString("0.##", CultureInfo.InvariantCulture)}%";

    internal string? EstiloDoArco
    {
        get
        {
            if (Indeterminado)
            {
                return null;
            }

            var deslocamento = Circunferencia * (1 - ValorLimitado / 100);
            return string.Create(CultureInfo.InvariantCulture,
                $"stroke-dasharray: {Circunferencia:0.###}; stroke-dashoffset: {deslocamento:0.###}");
        }
    }

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = string.Join(' ',
                "rvm-progresso",
                Variant == RvmProgressVariant.Circular ? "rvm-circular" : "rvm-linear",
                Indeterminado ? "rvm-indeterminado" : "rvm-determinado",
                Size switch { RvmSize.Small => "rvm-pequeno", RvmSize.Large => "rvm-grande", _ => "rvm-medio" },
                Color switch
                {
                    RvmColor.Secondary => "rvm-secondary",
                    RvmColor.Info => "rvm-info",
                    RvmColor.Success => "rvm-success",
                    RvmColor.Warning => "rvm-warning",
                    RvmColor.Error => "rvm-error",
                    _ => "rvm-primary"
                });

            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }
}
