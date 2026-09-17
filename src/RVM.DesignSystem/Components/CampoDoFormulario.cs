using System.Linq.Expressions;
using Microsoft.AspNetCore.Components.Forms;

namespace RVM.DesignSystem.Components;

/// <summary>
/// Acha o campo do <see cref="EditContext"/> a partir da expressao do <c>@bind</c>, para componentes de
/// formulario que nao herdam <see cref="InputBase{TValue}"/> (select, seletor de data).
/// </summary>
internal static class CampoDoFormulario
{
    public static FieldIdentifier Criar(LambdaExpression expressao)
    {
        // FieldIdentifier.Create so aceita Expression<Func<T>>; a lambda chega sem o tipo concreto.
        var corpo = expressao.Body is UnaryExpression { NodeType: ExpressionType.Convert } conversao
            ? conversao.Operand
            : expressao.Body;

        if (corpo is MemberExpression membro)
        {
            var dono = membro.Expression is null
                ? null
                : Expression.Lambda(membro.Expression).Compile().DynamicInvoke();
            if (dono is not null)
            {
                return new FieldIdentifier(dono, membro.Member.Name);
            }
        }

        throw new ArgumentException("A expressao do valor precisa ser um acesso a propriedade ou campo (modelo.Propriedade).");
    }
}
