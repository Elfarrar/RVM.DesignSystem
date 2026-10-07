using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Cards;

/// <summary>
/// Card de projeto de painel: o <see cref="RvmTaskCard"/> com uma capa no alto (contrato com o RVM.UI, DSGN-017).
/// Sem <see cref="ImageUrl"/>, um icone de pasta no lugar da capa. Herda o desenho do card de tarefa — sem
/// <c>.razor</c> proprio, o CSS isolado e o dele.
/// </summary>
public class RvmProjectCard : RvmTaskCard
{
    /// <summary>Cria o card com o padrao do RVM.UI de 4 avatares antes do "+N".</summary>
    public RvmProjectCard() => MembersMax = 4;

    /// <summary>Capa do projeto, 48 x 48, decorativa. Sem ela, um icone de pasta.</summary>
    [Parameter] public string? ImageUrl { get; set; }

    internal override bool TemCapa => true;

    internal override string? Capa => ImageUrl;

    internal override string ClasseDaRaiz => "rvm-project-card";

    internal override string NomeDaEquipe => "Equipe do projeto";
}
