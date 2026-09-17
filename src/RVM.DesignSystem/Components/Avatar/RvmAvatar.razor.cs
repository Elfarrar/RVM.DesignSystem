using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Avatar;

/// <summary>
/// Representa uma pessoa ou uma entidade: foto, iniciais ou icone. Se a foto nao carregar, cai para
/// as iniciais (ou o icone) em vez de mostrar imagem quebrada.
/// </summary>
public partial class RvmAvatar : ComponentBase
{
    private bool _imagemFalhou;
    private string? _srcAnterior;

    /// <summary>Endereco da foto. Tem prioridade sobre icone e iniciais.</summary>
    [Parameter] public string? Src { get; set; }

    /// <summary>
    /// Quem ou o que o avatar representa ("Rafael Veneroso"). Vira o <c>alt</c> da foto, ou o nome
    /// acessivel quando nao ha foto. Sem ele, o leitor de tela so tem as iniciais para anunciar.
    /// </summary>
    [Parameter] public string? Alt { get; set; }

    /// <summary>Iniciais, usadas quando nao ha foto nem icone. Passam de 2 letras sao cortadas.</summary>
    [Parameter] public string? Initials { get; set; }

    /// <summary>Icone, usado quando nao ha foto. Tem prioridade sobre as iniciais.</summary>
    [Parameter] public RvmIconName? Icon { get; set; }

    /// <summary>24, 40 (padrao) ou 56 px — os tres tamanhos medidos no kit.</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>Forma. Padrao: <see cref="RvmAvatarShape.Circle"/>.</summary>
    [Parameter] public RvmAvatarShape Shape { get; set; } = RvmAvatarShape.Circle;

    /// <summary>Papel de cor do fundo, para iniciais e icone. Padrao: <see cref="RvmColor.Primary"/>.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Primary;

    /// <summary>Preenchido ou suave. Padrao: <see cref="RvmAvatarVariant.Filled"/>.</summary>
    [Parameter] public RvmAvatarVariant Variant { get; set; } = RvmAvatarVariant.Filled;

    /// <summary>Conteudo livre, quando nao ha foto, icone nem iniciais.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc cref="ComponentBase" />
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal bool TemImagem => !string.IsNullOrWhiteSpace(Src);

    internal string IniciaisExibidas
        => (Initials ?? string.Empty).Trim() is { Length: > 2 } longas
            ? longas[..2].ToUpperInvariant()
            : (Initials ?? string.Empty).Trim().ToUpperInvariant();

    internal string? NomeAcessivel
        => !string.IsNullOrWhiteSpace(Alt) ? Alt
         : !string.IsNullOrWhiteSpace(Initials) ? Initials
         : null;

    internal RvmSize TamanhoDoIcone => Size switch
    {
        RvmSize.Small => RvmSize.Small,
        RvmSize.Large => RvmSize.Large,
        _ => RvmSize.Medium
    };

    internal string CssClass
    {
        get
        {
            var proprias = string.Join(' ',
                "avatar",
                Size switch { RvmSize.Small => "pequeno", RvmSize.Large => "grande", _ => "medio" },
                Shape switch { RvmAvatarShape.Rounded => "arredondado", RvmAvatarShape.Square => "quadrado", _ => "circulo" },
                Variant == RvmAvatarVariant.Soft ? "suave" : "preenchido",
                Color switch
                {
                    RvmColor.Secondary => "secondary",
                    RvmColor.Info => "info",
                    RvmColor.Success => "success",
                    RvmColor.Warning => "warning",
                    RvmColor.Error => "error",
                    _ => "primary"
                });

            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        // Foto nova ganha nova chance: sem isto, uma falha antiga deixaria o avatar preso nas
        // iniciais mesmo depois de o consumidor trocar para um endereco que funciona.
        if (_srcAnterior != Src)
        {
            _imagemFalhou = false;
            _srcAnterior = Src;
        }
    }

    private void AoFalharImagem() => _imagemFalhou = true;
}
