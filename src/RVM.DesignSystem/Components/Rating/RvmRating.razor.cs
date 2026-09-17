using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Rating;

/// <summary>
/// Nota em estrelas: para mostrar uma avaliacao ou para a pessoa dar a dela.
/// </summary>
public partial class RvmRating : ComponentBase
{
    /// <summary>A estrela cheia do Tabler (star-filled), desenhada aqui para a cor vir do CSS.</summary>
    internal const string CaminhoDaEstrela =
        "M8.243 7.34l-6.38 .925l-.113 .023a1 1 0 0 0 -.44 1.684l4.622 4.499l-1.09 6.355l-.013 .11a1 1 0 0 0 1.464 .944l5.706 -3l5.693 3l.1 .046a1 1 0 0 0 1.352 -1.139l-1.091 -6.355l4.624 -4.5l.078 -.085a1 1 0 0 0 -.633 -1.62l-6.38 -.926l-2.852 -5.78a1 1 0 0 0 -1.794 0l-2.853 5.78z";

    private static int _proximoId;
    private readonly string _idGerado = $"rvm-nota-{Interlocked.Increment(ref _proximoId)}";
    private double? _sobre;

    /// <summary>A nota. Aceita <c>@bind-Value</c>. Zero e "sem nota".</summary>
    [Parameter] public double Value { get; set; }

    /// <summary>Disparado quando a pessoa escolhe outra nota.</summary>
    [Parameter] public EventCallback<double> ValueChanged { get; set; }

    /// <summary>Quantas estrelas. Padrao: 5.</summary>
    [Parameter] public int Max { get; set; } = 5;

    /// <summary>Permite meia estrela.</summary>
    [Parameter] public bool AllowHalf { get; set; }

    /// <summary>Estrelas de 18, 24 (padrao) ou 30 px — medidas no kit.</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>So mostra a nota, sem deixar mudar.</summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>Indisponivel.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// O que esta sendo avaliado ("Nota do atendimento"). Nome do grupo para o leitor de tela.
    /// Padrao: "Avaliacao".
    /// </summary>
    [Parameter] public string Label { get; set; } = "Avaliacao";

    /// <summary><c>name</c> dos radios, para envio de formulario. Sem valor, e gerado.</summary>
    [Parameter] public string? Name { get; set; }

    /// <summary>Atributos extras, repassados a raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private double Passo => AllowHalf ? 0.5 : 1;

    internal string NomeDoGrupo => string.IsNullOrWhiteSpace(Name) ? _idGerado : Name;

    internal IEnumerable<double> Notas
        => Enumerable.Range(1, Math.Max(Max, 1) * (AllowHalf ? 2 : 1)).Select(i => i * Passo);

    internal string RotuloDeLeitura => $"{Label}: {Numero(Value)} de {Max}";

    internal string TextoDaNota(double nota)
        => nota == 1 ? $"1 estrela de {Max}" : $"{Numero(nota)} estrelas de {Max}";

    /// <summary>Numero para o usuario: virgula decimal, como se le em portugues.</summary>
    private static string Numero(double nota) => nota.ToString("0.#", CultureInfo.InvariantCulture).Replace('.', ',');

    internal static string ValorInvariante(double nota) => nota.ToString("0.#", CultureInfo.InvariantCulture);

    /// <summary>Largura em cultura invariante: em pt-BR sairia "50,5%", invalido no CSS.</summary>
    internal static string LarguraDoPreenchimento(double fracao)
        => string.Create(CultureInfo.InvariantCulture, $"width: {fracao * 100:0.##}%");

    internal string EstiloDoAlvo(double nota)
    {
        var total = Math.Max(Max, 1);
        var inicio = (nota - Passo) / total * 100;
        var largura = Passo / total * 100;
        return string.Create(CultureInfo.InvariantCulture, $"left: {inicio:0.###}%; width: {largura:0.###}%");
    }

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = string.Join(' ',
                "rvm-avaliacao",
                Size switch { RvmSize.Small => "rvm-pequeno", RvmSize.Large => "rvm-grande", _ => "rvm-medio" });

            if (ReadOnly) proprias += " rvm-leitura";
            if (Disabled) proprias += " rvm-desabilitado";

            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }

    private async Task EscolherAsync(double nota)
    {
        if (ReadOnly || Disabled)
        {
            return;
        }

        Value = nota;
        await ValueChanged.InvokeAsync(nota);
    }
}
