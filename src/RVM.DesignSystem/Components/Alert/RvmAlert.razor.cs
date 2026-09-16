using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Alert;

/// <summary>
/// Mensagem de destaque dentro da pagina: o que aconteceu e, quando der, o que fazer. Para aviso
/// que some sozinho, use o snackbar (onda 3).
/// </summary>
public partial class RvmAlert : ComponentBase
{
    /// <summary>
    /// Gravidade, pelo papel de cor. Padrao: <see cref="RvmColor.Info"/>. O kit desenha quatro
    /// (Error, Warning, Info, Success); Primary e Secondary tambem funcionam, pelo contrato do
    /// <see cref="RvmColor"/>.
    /// </summary>
    [Parameter] public RvmColor Severity { get; set; } = RvmColor.Info;

    /// <summary>Estilo. Padrao: <see cref="RvmAlertVariant.Standard"/>.</summary>
    [Parameter] public RvmAlertVariant Variant { get; set; } = RvmAlertVariant.Standard;

    /// <summary>Titulo acima da mensagem (a variacao "With Title" do kit).</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>Mostra o icone da gravidade. Padrao: sim.</summary>
    [Parameter] public bool ShowIcon { get; set; } = true;

    /// <summary>Troca o icone da gravidade por outro.</summary>
    [Parameter] public RvmIconName? Icon { get; set; }

    /// <summary>Acao ao lado da mensagem ("DESFAZER", "VER DETALHES") — a variacao "With Action".</summary>
    [Parameter] public RenderFragment? Action { get; set; }

    /// <summary>Chamado ao fechar. Com alguem escutando, aparece o botao de fechar.</summary>
    [Parameter] public EventCallback OnClose { get; set; }

    /// <summary>Nome acessivel do botao de fechar. Padrao: "Fechar aviso".</summary>
    [Parameter] public string CloseLabel { get; set; } = "Fechar aviso";

    /// <summary>A mensagem.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc cref="ComponentBase" />
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string Papel => Severity is RvmColor.Error or RvmColor.Warning ? "alert" : "status";

    internal RvmIconName IconeEfetivo => Icon ?? Severity switch
    {
        RvmColor.Success => RvmIconName.CircleCheck,
        RvmColor.Warning => RvmIconName.AlertTriangle,
        RvmColor.Error => RvmIconName.AlertCircle,
        _ => RvmIconName.InfoCircle
    };

    internal string CssClass
    {
        get
        {
            var proprias = string.Join(' ',
                "alerta",
                Variant switch { RvmAlertVariant.Filled => "preenchido", RvmAlertVariant.Outlined => "contorno", _ => "padrao" },
                Severity switch
                {
                    RvmColor.Primary => "primary",
                    RvmColor.Secondary => "secondary",
                    RvmColor.Success => "success",
                    RvmColor.Warning => "warning",
                    RvmColor.Error => "error",
                    _ => "info"
                });

            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }

    private Task Fechar() => OnClose.InvokeAsync();
}
