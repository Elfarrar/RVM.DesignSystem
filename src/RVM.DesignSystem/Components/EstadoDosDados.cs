using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.EmptyState;
using RVM.DesignSystem.Components.Mascot;
using RVM.DesignSystem.Components.Progress;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components;

/// <summary>O que um componente de dados mostra no lugar do conteudo.</summary>
internal enum EstadoDosDados
{
    Conteudo,
    Carregando,
    Erro,
    Vazio
}

/// <summary>
/// Carregando, erro e vazio de listas, tabelas e graficos (contrato com o RVM.UI, DSGN-017). A casca decide o que
/// mostrar, em vez de cada aplicativo lembrar de tratar os tres. Monta so componentes publicos, que trazem o proprio
/// CSS: elemento criado aqui nao recebe o escopo do CSS isolado de quem chama.
/// </summary>
internal static class EstadosDosDados
{
    internal const string TextoDeErro = "Tivemos um problema tecnico, por favor tente de novo em alguns minutos.";

    /// <summary>Erro vence carregando, que vence vazio — a mesma ordem do RVM.UI.</summary>
    internal static EstadoDosDados Qual(bool erro, bool carregando, bool vazio)
        => erro ? EstadoDosDados.Erro
            : carregando ? EstadoDosDados.Carregando
            : vazio ? EstadoDosDados.Vazio
            : EstadoDosDados.Conteudo;

    internal static RenderFragment Carregando(string texto, RvmMascotName? mascote) => b =>
    {
        b.OpenElement(0, "div");
        b.AddAttribute(1, "class", "rvm-estado-carregando");
        b.AddAttribute(2, "role", "status");
        b.AddAttribute(3, "style",
            "display: flex; flex-direction: column; align-items: center; gap: var(--rvm-space-3); "
            + "padding: var(--rvm-space-8) var(--rvm-space-4); color: var(--rvm-color-text-secondary); "
            + "font-family: var(--rvm-font-family); font-size: var(--rvm-text-body2-size);");
        if (mascote is { } nome)
        {
            b.OpenComponent<RvmMascot>(4);
            b.AddComponentParameter(5, nameof(RvmMascot.Name), nome);
            b.AddComponentParameter(6, nameof(RvmMascot.Decorative), true);
            b.AddComponentParameter(7, nameof(RvmMascot.Width), 96);
            b.CloseComponent();
        }
        else
        {
            b.OpenComponent<RvmProgress>(8);
            b.AddComponentParameter(9, nameof(RvmProgress.Variant), RvmProgressVariant.Circular);
            b.AddComponentParameter(10, nameof(RvmProgress.Label), texto);
            b.CloseComponent();
        }

        b.OpenElement(11, "span");
        b.AddContent(12, texto);
        b.CloseElement();
        b.CloseElement();
    };

    internal static RenderFragment Erro(RenderFragment? conteudo, string titulo, string texto, RvmMascotName? mascote)
        => conteudo ?? (b =>
        {
            b.OpenComponent<RvmEmptyState>(0);
            b.AddComponentParameter(1, nameof(RvmEmptyState.Title), titulo);
            b.AddComponentParameter(2, nameof(RvmEmptyState.Description), texto);
            b.AddComponentParameter(3, nameof(RvmEmptyState.Color), RvmColor.Error);
            b.AddComponentParameter(4, nameof(RvmEmptyState.Icon), RvmIconName.AlertTriangle);
            b.AddComponentParameter(5, nameof(RvmEmptyState.Mascot), mascote);
            b.AddComponentParameter(6, nameof(RvmEmptyState.Status), true);
            b.CloseComponent();
        });

    // Status ligado: o vazio aparece depois de um carregamento ou de um filtro, e sem ele quem usa leitor de tela
    // ouve "Carregando..." e depois silencio.
    internal static RenderFragment Vazio(RenderFragment? conteudo, string texto) => conteudo ?? (b =>
    {
        b.OpenComponent<RvmEmptyState>(0);
        b.AddComponentParameter(1, nameof(RvmEmptyState.Title), texto);
        b.AddComponentParameter(2, nameof(RvmEmptyState.Status), true);
        b.CloseComponent();
    });
}
