using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace RVM.DesignSystem.Components;

/// <summary>
/// Base dos campos de formulário da biblioteca.
/// </summary>
/// <typeparam name="TValue">Tipo do valor do campo.</typeparam>
/// <remarks>
/// Herda de <see cref="InputBase{TValue}"/> — é o que faz <c>DataAnnotations</c>, <c>EditForm</c>
/// e <c>ValidationMessage</c> funcionarem sem adaptação (`03` § Formulários).
///
/// <para>
/// <b>E resolve o defeito que vem junto.</b> O <see cref="InputBase{TValue}"/> do próprio Blazor
/// <b>lança exceção</b> quando é usado fora de um <c>EditForm</c>, porque exige
/// <c>ValueExpression</c> — que só o <c>@bind-Value</c> dentro do formulário fornece. Os
/// <c>InputText</c> e companhia do framework têm a mesma limitação.
/// </para>
/// <para>
/// Para uma biblioteca de componentes isso é inaceitável: um campo de busca numa barra de
/// ferramentas, um filtro numa listagem, um campo num diálogo simples — nenhum deles vive num
/// <c>EditForm</c>, e todos quebrariam com exceção em tempo de execução. Aqui, quando a
/// expressão não vem, ela é sintetizada. Dentro de um <c>EditForm</c> nada muda; fora dele, o
/// campo simplesmente funciona, sem validação de anotação — que é o esperado, já que não há
/// modelo para validar.
/// </para>
/// </remarks>
public abstract class RvmInputBase<TValue> : InputBase<TValue>
{
    /// <summary>Mensagem de erro explícita, definida pelo consumidor.</summary>
    /// <remarks>
    /// Vence a validação do <c>EditContext</c>: quem passa <c>Error</c> na mão sabe de algo que
    /// a anotação não sabe — tipicamente um erro devolvido pelo servidor.
    /// </remarks>
    [Parameter]
    public string? Error { get; set; }

    /// <summary>Texto de ajuda exibido abaixo do campo, quando não há erro.</summary>
    [Parameter]
    public string? Help { get; set; }

    /// <summary>A mensagem que deve aparecer: a explícita, ou a primeira da validação.</summary>
    protected string? MensagemDeErro => !string.IsNullOrWhiteSpace(Error)
        ? Error
        : EditContext is not null && FieldIdentifier.FieldName is not null
            ? EditContext.GetValidationMessages(FieldIdentifier).FirstOrDefault()
            : null;

    /// <summary>Verdadeiro quando há mensagem de erro.</summary>
    protected bool TemErro => !string.IsNullOrWhiteSpace(MensagemDeErro);

    /// <summary>
    /// O <c>aria-describedby</c> do controle: aponta para o erro OU para a ajuda.
    /// </summary>
    /// <param name="id">Identificador do campo.</param>
    /// <returns>O id da mensagem, ou <c>null</c> quando não há nenhuma.</returns>
    /// <remarks>
    /// Nunca os dois: só um dos dois é renderizado, e apontar para um id inexistente faz parte
    /// dos leitores de tela simplesmente não ler nada — o usuário perde a mensagem sem que
    /// ninguém perceba.
    /// </remarks>
    protected string? DescricaoPara(string id) => TemErro
        ? $"{id}-error"
        : !string.IsNullOrWhiteSpace(Help) ? $"{id}-help" : null;

    /// <inheritdoc />
    public override Task SetParametersAsync(ParameterView parameters)
    {
        // Sintetiza a ValueExpression antes de o InputBase reclamar da ausência dela.
        // Sem isto, usar o campo fora de um EditForm lança InvalidOperationException.
        if (ValueExpression is null)
        {
            var valor = parameters.TryGetValue<TValue>(nameof(Value), out var v) ? v : Value;
            ValueExpression = () => valor!;
        }

        return base.SetParametersAsync(parameters);
    }
}
