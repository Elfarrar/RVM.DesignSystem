using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Table;

/// <summary>
/// O que as celulas da tabela (RvmCell*) dividem: as classes do <c>&lt;td&gt;</c> e o bloco de titulo com a segunda
/// linha. Interno e nao-componente — o contrato com o RVM.UI (DSGN-017) so tem as 15 celulas. O bloco sai em C#, sem o
/// atributo do CSS isolado: cada celula o estiliza com <c>::deep</c> a partir do proprio <c>&lt;td&gt;</c>.
/// </summary>
internal static class CelulaCss
{
    /// <summary>As classes do <c>&lt;td&gt;</c>: a base, o alinhamento (nulo na coluna de escolha), a da celula e as do consumidor.</summary>
    public static string Td(RvmAlign? alinhamento, string? propria, string? classe, IReadOnlyDictionary<string, object>? atributos)
    {
        var deAlinhamento = alinhamento switch
        {
            RvmAlign.Center => "rvm-centro",
            RvmAlign.End => "rvm-fim",
            RvmAlign.Start => "rvm-inicio",
            _ => null
        };
        var proprias = string.Join(' ', new[] { "rvm-celula", deAlinhamento, propria }.Where(c => c is not null));
        return ClassesCss.Juntar(proprias, classe, atributos);
    }

    /// <summary>
    /// O titulo e, embaixo, a segunda linha menor e mais clara (Supporting Text do kit). O conteudo livre, quando ha,
    /// toma o lugar do titulo; o destaque deixa o titulo mais pesado.
    /// </summary>
    public static RenderFragment Linhas(string? texto, string? apoio, bool destaque, RenderFragment? conteudo = null)
        => builder =>
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "class", destaque ? "rvm-celula-texto rvm-destaque" : "rvm-celula-texto");

            builder.OpenElement(2, "span");
            builder.AddAttribute(3, "class", "rvm-titulo");
            if (conteudo is null) builder.AddContent(4, texto);
            else builder.AddContent(5, conteudo);
            builder.CloseElement();

            if (!string.IsNullOrWhiteSpace(apoio))
            {
                builder.OpenElement(6, "span");
                builder.AddAttribute(7, "class", "rvm-apoio");
                builder.AddContent(8, apoio);
                builder.CloseElement();
            }

            builder.CloseElement();
        };
}
