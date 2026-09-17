using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace RVM.DesignSystem.Components.Table;

/// <summary>
/// Desenha o conteudo DEPOIS dos irmaos que vem antes dele na arvore. A tabela poe as colunas
/// declaradas antes e o corpo da tabela aqui dentro: quando a pagina muda o titulo de uma coluna, a
/// coluna recebe o parametro novo na mesma passada e o cabecalho ja sai com ele — sem um render de
/// atraso e sem a coluna precisar pedir outro render a tabela (o que viraria laco).
/// </summary>
internal sealed class RvmAdiado : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddContent(0, ChildContent);
}
