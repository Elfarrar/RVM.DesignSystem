using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Button;

/// <summary>
/// O botao do design system. Tres pesos visuais, tres tamanhos, icone dos dois lados e estado de
/// carregando.
/// </summary>
public partial class RvmButton : ComponentBase
{
    /// <summary>Peso visual. Padrao: <see cref="RvmButtonVariant.Contained"/>.</summary>
    [Parameter] public RvmButtonVariant Variant { get; set; } = RvmButtonVariant.Contained;

    /// <summary>Papel semantico da cor. Padrao: <see cref="RvmColor.Primary"/>.</summary>
    [Parameter] public RvmColor Color { get; set; } = RvmColor.Primary;

    /// <summary>Tamanho. Padrao: <see cref="RvmSize.Medium"/> (38 px de altura, medido do kit).</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>Indisponivel: sai da ordem de tabulacao e ignora clique.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// A acao esta em curso. Troca o icone da esquerda por um girando, marca <c>aria-busy</c> e
    /// bloqueia o clique — sem isto, dois cliques viram duas requisicoes.
    /// </summary>
    [Parameter] public bool Loading { get; set; }

    /// <summary>Icone antes do rotulo.</summary>
    [Parameter] public RvmIconName? StartIcon { get; set; }

    /// <summary>Icone depois do rotulo. Fica escondido enquanto <see cref="Loading"/>.</summary>
    [Parameter] public RvmIconName? EndIcon { get; set; }

    /// <summary>O <c>type</c> do elemento. Padrao: <see cref="RvmButtonType.Button"/>.</summary>
    [Parameter] public RvmButtonType Type { get; set; } = RvmButtonType.Button;

    /// <summary>O que fazer no clique.</summary>
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>O rotulo.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc cref="ComponentBase" />
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private ElementReference _elemento;

    /// <summary>
    /// Leva o foco ao botao. Serve a quem abre algo a partir dele (um menu) e precisa devolver o foco
    /// ao fechar.
    /// </summary>
    public ValueTask FocusAsync() => _elemento.FocusAsync();

    internal string TipoHtml => Type switch
    {
        RvmButtonType.Submit => "submit",
        RvmButtonType.Reset => "reset",
        _ => "button"
    };

    internal string CssClass
    {
        get
        {
            var proprias = string.Join(' ', "rvm-botao", Variant switch
            {
                RvmButtonVariant.Outlined => "rvm-contorno",
                RvmButtonVariant.Text => "rvm-texto",
                _ => "rvm-preenchido"
            }, Size switch
            {
                RvmSize.Small => "rvm-pequeno",
                RvmSize.Large => "rvm-grande",
                _ => "rvm-medio"
            }, Color switch
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

    private async Task AoClicar(MouseEventArgs e)
    {
        // O `disabled` do elemento ja barra o clique no navegador, mas nao em chamada programatica
        // nem em teste — e um clique que escapa enquanto carrega vira requisicao duplicada.
        if (Disabled || Loading || !OnClick.HasDelegate)
        {
            return;
        }

        await OnClick.InvokeAsync(e);
    }
}
