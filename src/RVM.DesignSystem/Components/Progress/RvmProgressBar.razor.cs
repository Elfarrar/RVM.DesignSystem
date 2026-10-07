using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Progress;

/// <summary>
/// Barra de progresso com rotulo e porcentagem por cima — meta, orcamento, etapa de uma safra. Do
/// contrato com o RVM.UI (DSGN-017). Para "carregando" sem rotulo, use <see cref="RvmProgress"/>.
/// </summary>
public partial class RvmProgressBar : ComponentBase
{
    /// <summary>
    /// Progresso de 0 a 100. Fora disso e limitado (calculo que passa de 100 por arredondamento nao
    /// derruba a tela); NaN vira 0. Ignorado com <see cref="NoValue"/>.
    /// </summary>
    [Parameter] public double Value { get; set; }

    /// <summary>Papel de cor da parte preenchida. Padrao: <see cref="RvmColor.Accent"/>.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Accent;

    /// <summary>Barra de 4 px com texto pequeno, 8 px (padrao) ou 12 px.</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>
    /// Fundo sobre o qual a barra esta. No <see cref="RvmSurface.Dark"/> (cartao colorido ou escuro), trilho,
    /// barra e texto ficam claros: a cor do papel sobre fundo colorido nao garante contraste.
    /// </summary>
    [Parameter] public RvmSurface Surface { get; set; } = RvmSurface.Light;

    /// <summary>Rotulo acima da barra. Tambem e o nome acessivel.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Estilo do rotulo: descricao (padrao) ou valor.</summary>
    [Parameter] public RvmProgressLabelVariant LabelVariant { get; set; } = RvmProgressLabelVariant.Label;

    /// <summary>Mostra a porcentagem a direita do rotulo.</summary>
    [Parameter] public bool ShowValue { get; set; }

    /// <summary>Nome acessivel quando nao ha <see cref="Label"/> visivel. Um dos dois e obrigatorio.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>
    /// Valor nao informado: sem preenchimento, trilho tracejado e <see cref="NoValueText"/> no lugar da
    /// porcentagem (e para o leitor de tela). <see cref="Value"/> e ignorado.
    /// </summary>
    [Parameter] public bool NoValue { get; set; }

    /// <summary>Texto do estado sem valor. Padrao: "Nao informado".</summary>
    [Parameter] public string NoValueText { get; set; } = "Nao informado";

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos extras, repassados a raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private bool TemCabecalho => !string.IsNullOrWhiteSpace(Label) || ShowValue;

    private string? NomeAcessivel => string.IsNullOrWhiteSpace(Label) ? AriaLabel : Label;

    internal double ValorLimitado => double.IsNaN(Value) ? 0 : Math.Clamp(Value, 0, 100);

    internal string ValorArredondado => Math.Round(ValorLimitado).ToString(CultureInfo.InvariantCulture);

    private string Porcentagem => $"{ValorArredondado}%";

    internal string EstiloDaBarra => $"width: {ValorLimitado.ToString("0.##", CultureInfo.InvariantCulture)}%";

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = string.Join(' ',
                "rvm-barra-de-progresso",
                Size switch { RvmSize.Small => "rvm-pequeno", RvmSize.Large => "rvm-grande", _ => "rvm-medio" },
                Surface == RvmSurface.Dark ? "rvm-superficie-escura" : "rvm-superficie-clara",
                NoValue ? "rvm-sem-valor" : "rvm-com-valor",
                PapelCss.Classe(Color));

            return ClassesCss.Juntar(proprias, Class, AdditionalAttributes);
        }
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (string.IsNullOrWhiteSpace(Label) && string.IsNullOrWhiteSpace(AriaLabel))
        {
            throw new ArgumentException(
                "RvmProgressBar precisa de Label ou AriaLabel: sem nome, o leitor de tela anuncia apenas um numero.",
                nameof(AriaLabel));
        }
    }
}
