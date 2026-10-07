using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Cards;

/// <summary>
/// Card de conversao de moeda: titulo, dois campos do consumidor, a taxa e o botao (contrato com o RVM.UI,
/// DSGN-017). E so a casca: a biblioteca nao tem cotacao — quem calcula e o aplicativo, no <see cref="OnConvert"/>.
/// </summary>
public partial class RvmCurrencyConverter : ComponentBase
{
    private readonly string _id = GeradorDeIds.Novo("rvm-conversor");

    /// <summary>Titulo. Sai como cabecalho <c>h3</c> e da nome a secao.</summary>
    [Parameter, EditorRequired] public string? Title { get; set; }

    /// <summary>Linha de apoio abaixo do titulo.</summary>
    [Parameter] public string? Subtitle { get; set; }

    /// <summary>O campo de origem (um <c>RvmNumericField</c> com a moeda, por exemplo).</summary>
    [Parameter] public RenderFragment? From { get; set; }

    /// <summary>O campo de destino.</summary>
    [Parameter] public RenderFragment? To { get; set; }

    /// <summary>
    /// A taxa, ja escrita pelo aplicativo ("1 USD = 5,42 BRL"). A regiao e anunciada pelo leitor de tela quando muda.
    /// </summary>
    [Parameter] public string? Rate { get; set; }

    /// <summary>Texto do botao. Padrao: "Converter".</summary>
    [Parameter] public string ActionText { get; set; } = "Converter";

    /// <summary>Icone do botao. Padrao: <see cref="RvmIconName.Login3"/>, as setas opostas do kit.</summary>
    [Parameter] public RvmIconName ActionIcon { get; set; } = RvmIconName.Login3;

    /// <summary>Clique no botao. Quem converte e o aplicativo.</summary>
    [Parameter] public EventCallback OnConvert { get; set; }

    /// <summary>Desabilita o botao (enquanto a cotacao carrega, por exemplo).</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Itens do menu de tres pontos (<c>RvmMenuItem</c>). Sem eles, sem menu.</summary>
    [Parameter] public RenderFragment? Menu { get; set; }

    /// <summary>Nome acessivel do menu. Padrao: "Acoes do conversor".</summary>
    [Parameter] public string MenuLabel { get; set; } = "Acoes do conversor";

    /// <summary>Papel de cor do botao. Padrao: Accent.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Accent;

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos repassados ao elemento raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    // Sem taxa, a regiao continua no DOM (a role=status precisa existir antes da mudanca), so que sem ocupar espaco.
    internal string ClasseDaTaxa => string.IsNullOrWhiteSpace(Rate) ? "rvm-taxa rvm-text-body2 rvm-sem-taxa" : "rvm-taxa rvm-text-body2";

    internal string IdDoTitulo => $"{_id}-titulo";

    internal string CssClass => ClassesCss.Juntar("rvm-currency-converter", Class, AdditionalAttributes);
}
