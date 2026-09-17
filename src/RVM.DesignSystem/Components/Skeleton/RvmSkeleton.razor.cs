using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Skeleton;

/// <summary>
/// Espaco reservado com a forma do conteudo que ainda vai chegar. Evita que a tela pule quando os
/// dados aparecem.
/// </summary>
public partial class RvmSkeleton : ComponentBase
{
    /// <summary>Linha de texto (padrao), retangulo ou circulo.</summary>
    [Parameter] public RvmSkeletonVariant Variant { get; set; } = RvmSkeletonVariant.Text;

    /// <summary>Largura em CSS ("100%", "12rem", "40px"). Padrao: toda a largura; circulo, 40 px.</summary>
    [Parameter] public string? Width { get; set; }

    /// <summary>Altura em CSS. Padrao: a da linha de texto; retangulo, 120 px; circulo, igual a largura.</summary>
    [Parameter] public string? Height { get; set; }

    /// <summary>Com o brilho passando (padrao) ou parado.</summary>
    [Parameter] public bool Animated { get; set; } = true;

    /// <summary>Atributos extras: <c>class</c> e <c>style</c> somados aos proprios; o resto na raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = string.Join(' ',
                "esqueleto",
                Variant switch { RvmSkeletonVariant.Rectangular => "retangulo", RvmSkeletonVariant.Circular => "circulo", _ => "texto" });

            if (Animated)
            {
                proprias += " animado";
            }

            return Valor("class") is { } texto ? $"{proprias} {texto}" : proprias;
        }
    }

    internal string Estilo
    {
        get
        {
            var largura = Width ?? (Variant == RvmSkeletonVariant.Circular ? "40px" : null);
            var altura = Height ?? Variant switch
            {
                RvmSkeletonVariant.Rectangular => "120px",
                RvmSkeletonVariant.Circular => largura,
                _ => null
            };

            var partes = new List<string>(3);
            if (largura is not null) partes.Add($"width: {largura}");
            if (altura is not null) partes.Add($"height: {altura}");
            if (Valor("style") is { } doConsumidor) partes.Add(doConsumidor.TrimEnd(';'));
            return string.Join("; ", partes);
        }
    }

    private string? Valor(string chave)
        => AdditionalAttributes is not null
           && AdditionalAttributes.TryGetValue(chave, out var valor)
           && valor is string texto
           && !string.IsNullOrWhiteSpace(texto)
            ? texto
            : null;
}
