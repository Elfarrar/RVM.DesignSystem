using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace RVM.DesignSystem.Components.TextField;

/// <summary>
/// Base dos campos de formulario do contrato com o RVM.UI (DSGN-017), sobre o <see cref="InputBase{TValue}"/>: funciona
/// com <c>EditForm</c>, <c>@bind-Value</c> e validacao; renderiza <c>name</c> (SSR estatico), <c>id</c> e
/// <c>aria-*</c>; e junta a mensagem da validacao com a informada. A moldura e o <see cref="RvmFieldFrame"/>.
/// </summary>
/// <typeparam name="TValue">Tipo do valor.</typeparam>
public abstract class RvmInputBase<TValue> : InputBase<TValue>
{
    private readonly string _idGerado = GeradorDeIds.Novo("rvm-campo");

    /// <summary>Rotulo do campo.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Texto do link acima do campo, a direita ("Esqueci a senha").</summary>
    [Parameter] public string? LinkText { get; set; }

    /// <summary>Destino do link acima do campo.</summary>
    [Parameter] public string? LinkHref { get; set; }

    /// <summary>Dica dentro do campo vazio. Com ela, o rotulo fica sempre em cima.</summary>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>Canto da caixa: o do kit ou pilula.</summary>
    [Parameter] public RvmFieldShape Shape { get; set; } = RvmFieldShape.Rounded;

    /// <summary>Indisponivel.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Erro informado por fora. Dentro de um <c>EditForm</c>, a mensagem da validacao ja aparece sozinha.</summary>
    [Parameter] public string? ErrorText { get; set; }

    /// <summary>Texto de apoio abaixo do campo. Da lugar ao erro quando ha erro.</summary>
    [Parameter] public string? HelperText { get; set; }

    /// <summary>Id do controle. Sem ele, um id unico e gerado.</summary>
    [Parameter] public string? Id { get; set; }

    /// <summary>Nome enviado no formulario. Sem ele, vem do <c>@bind-Value</c> (necessario em SSR estatico).</summary>
    [Parameter] public string? Name { get; set; }

    /// <summary>O id efetivo do controle.</summary>
    protected string IdDoControle => string.IsNullOrWhiteSpace(Id) ? _idGerado : Id;

    /// <summary>Id da mensagem de erro, ligada ao controle por <c>aria-describedby</c>.</summary>
    protected string IdDoErro => $"{IdDoControle}-erro";

    /// <summary>Id do texto de apoio.</summary>
    protected string IdDoApoio => $"{IdDoControle}-apoio";

    /// <summary>O <c>name</c> efetivo; vazio vira atributo ausente (o WebAssembly nao gera nomes de campo).</summary>
    protected string? NomeDoControle => !string.IsNullOrWhiteSpace(Name) ? Name
        : string.IsNullOrEmpty(NameAttributeValue) ? null : NameAttributeValue;

    /// <summary>A mensagem de erro: a informada, ou a primeira da validacao do <c>EditForm</c>.</summary>
    protected string? MensagemDeErro
        => !string.IsNullOrWhiteSpace(ErrorText) ? ErrorText : EditContext?.GetValidationMessages(FieldIdentifier).FirstOrDefault();

    /// <summary>O <c>aria-describedby</c> do controle: o erro, ou o apoio.</summary>
    protected string? DescritoPor
        => MensagemDeErro is not null ? IdDoErro : string.IsNullOrWhiteSpace(HelperText) ? null : IdDoApoio;

    /// <summary>Sem placeholder do consumidor, um espaco: e o que faz o rotulo flutuar so com CSS.</summary>
    protected string PlaceholderEfetivo => string.IsNullOrEmpty(Placeholder) ? " " : Placeholder;

    /// <summary>
    /// O <c>Class</c> do campo. Fica nos campos, e nao aqui: no contrato com o RVM.UI a base nao tem <c>Class</c>, cada
    /// campo declara o seu.
    /// </summary>
    protected virtual string? ClasseDoCampo => null;

    /// <summary>Os atributos extras para o controle, menos <c>class</c> (que vai para a raiz da moldura).</summary>
    protected IReadOnlyDictionary<string, object>? AtributosDoControle
        => AdditionalAttributes?
            .Where(a => !string.Equals(a.Key, "class", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(a => a.Key, a => a.Value);

    /// <summary>As classes da raiz: as do consumidor e as de validacao do <c>EditForm</c> ("modified", "invalid").</summary>
    protected string? ClassesDoConsumidor => ClassesCss.Juntar(CssClass ?? "", ClasseDoCampo, null) is { Length: > 0 } c ? c : null;
}
