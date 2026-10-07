using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Avatar;

namespace RVM.DesignSystem.Components.Cards;

/// <summary>
/// Card de tarefa de painel: status, titulo, descricao, progresso com texto livre, equipe e prazo (contrato com o
/// RVM.UI, DSGN-017). O <see cref="RvmProjectCard"/> e este mesmo card com uma capa.
/// </summary>
public partial class RvmTaskCard : ComponentBase
{
    /// <summary>Texto do selo de status ("Atrasada"). Sem ele, sem selo.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Papel de cor do selo. Padrao: Danger.</summary>
    [Parameter] public RvmColor LabelColor { get; set; } = RvmColor.Danger;

    /// <summary>Titulo. Sai como cabecalho <c>h3</c>.</summary>
    [Parameter, EditorRequired] public string Title { get; set; } = "";

    /// <summary>Descricao, cortada em duas linhas.</summary>
    [Parameter] public string? Description { get; set; }

    /// <summary>Texto a direita de "Progresso" ("Atrasada", "75%"). Livre: quem chama decide.</summary>
    [Parameter, EditorRequired] public string ProgressText { get; set; } = "";

    /// <summary>Progresso da barra, de 0 a 100.</summary>
    [Parameter] public double ProgressPercent { get; set; }

    /// <summary>Papel de cor da barra e do texto de progresso. Padrao: Accent.</summary>
    [Parameter] public RvmColor ProgressColor { get; set; } = RvmColor.Accent;

    /// <summary>Pessoas da equipe. Sem ninguem, sem grupo de avatares.</summary>
    [Parameter] public IReadOnlyList<RvmAvatarItem> Members { get; set; } = [];

    /// <summary>Quantos avatares aparecem antes do "+N". Padrao: 3 (4 no <see cref="RvmProjectCard"/>).</summary>
    [Parameter] public int MembersMax { get; set; } = 3;

    /// <summary>Prazo, ja formatado por quem chama ("21 out 2026"). O leitor de tela ouve "Prazo:" antes.</summary>
    [Parameter] public string? DueDate { get; set; }

    /// <summary>Itens do menu de tres pontos (<c>RvmMenuItem</c>). Sem eles, sem menu.</summary>
    [Parameter] public RenderFragment? MenuItems { get; set; }

    /// <summary>Nome acessivel do menu. Padrao: "Mais acoes".</summary>
    [Parameter] public string MenuLabel { get; set; } = "Mais acoes";

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos repassados ao elemento raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Se o card abre com a capa (so o de projeto).</summary>
    internal virtual bool TemCapa => false;

    /// <summary>Endereco da capa.</summary>
    internal virtual string? Capa => null;

    /// <summary>Classe propria da raiz.</summary>
    internal virtual string ClasseDaRaiz => "rvm-task-card";

    /// <summary>Nome do grupo de avatares para o leitor de tela.</summary>
    internal virtual string NomeDaEquipe => "Responsaveis";

    internal string ClasseDoTextoDoProgresso => $"rvm-texto-do-progresso {PapelCss.Classe(ProgressColor)}";

    internal string CssClass => ClassesCss.Juntar($"rvm-cartao-de-tarefa {ClasseDaRaiz}", Class, AdditionalAttributes);
}
