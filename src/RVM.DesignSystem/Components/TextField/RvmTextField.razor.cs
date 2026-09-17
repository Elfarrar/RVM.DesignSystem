using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace RVM.DesignSystem.Components.TextField;

/// <summary>
/// Campo de texto com rotulo flutuante, apoio, erro, prefixo e sufixo. Herda de
/// <see cref="InputBase{TValue}"/>: dentro de um <see cref="EditForm"/> ele participa da validacao,
/// e fora de um tambem funciona.
/// </summary>
public partial class RvmTextField
{
    private static int _proximoId;
    private readonly string _idGerado = $"rvm-campo-{Interlocked.Increment(ref _proximoId)}";

    /// <summary>Rotulo do campo. Flutua para cima quando o campo tem valor ou foco.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Texto de apoio abaixo do campo. Da lugar a mensagem de erro quando ha erro.</summary>
    [Parameter] public string? HelperText { get; set; }

    /// <summary>
    /// Erro informado por fora. Dentro de um <see cref="EditForm"/>, a mensagem da validacao do
    /// proprio campo ja aparece sozinha — use este parametro para erro que vem de outro lugar (a
    /// resposta de uma API, por exemplo).
    /// </summary>
    [Parameter] public string? ErrorText { get; set; }

    /// <summary>Estilo. Padrao: <see cref="RvmTextFieldVariant.Outlined"/>.</summary>
    [Parameter] public RvmTextFieldVariant Variant { get; set; } = RvmTextFieldVariant.Outlined;

    /// <summary>
    /// 56 px (padrao) ou 40 px (<see cref="RvmSize.Small"/>), medidos no kit. O kit nao define um
    /// campo grande, entao <see cref="RvmSize.Large"/> sai igual ao medio.
    /// </summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>O <c>type</c> do campo. Padrao: <see cref="RvmInputType.Text"/>.</summary>
    [Parameter] public RvmInputType Type { get; set; } = RvmInputType.Text;

    /// <summary>Texto antes do valor ("R$").</summary>
    [Parameter] public string? Prefix { get; set; }

    /// <summary>Texto depois do valor ("Kg", "%").</summary>
    [Parameter] public string? Suffix { get; set; }

    /// <summary>Dica dentro do campo vazio. Com ela, o rotulo fica sempre la em cima.</summary>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>
    /// Obrigatorio: asterisco no rotulo e <c>aria-required</c> no input. A validacao em si e do
    /// <see cref="EditForm"/> (<c>[Required]</c> no modelo) — o <c>required</c> nativo do HTML fica de
    /// fora de proposito, porque barraria o envio com a mensagem do navegador, em ingles.
    /// </summary>
    [Parameter] public bool Required { get; set; }

    /// <summary>Indisponivel.</summary>
    [Parameter] public bool Disabled { get; set; }

    internal string IdEfetivo
        => AdditionalAttributes is not null
           && AdditionalAttributes.TryGetValue("id", out var informado)
           && informado is string texto
           && !string.IsNullOrWhiteSpace(texto)
            ? texto
            : _idGerado;

    /// <summary>
    /// O <c>name</c> vem do <see cref="InputBase{TValue}.NameAttributeValue"/>, que o monta a partir
    /// do <c>@bind-Value</c>. Vazio quando o <see cref="EditContext"/> nao usa nomes de campo — o caso
    /// do WebAssembly, em que nao ha POST. Vazio vira atributo AUSENTE: <c>name=""</c> e ruido.
    /// </summary>
    internal string? NomeDoCampo => string.IsNullOrEmpty(NameAttributeValue) ? null : NameAttributeValue;

    /// <summary><c>style</c> do consumidor, aplicado na raiz, onde largura e margem fazem efeito.</summary>
    internal string? EstiloDoConsumidor
        => AdditionalAttributes is not null
           && AdditionalAttributes.TryGetValue("style", out var valor)
           && valor is string texto
           && !string.IsNullOrWhiteSpace(texto)
            ? texto
            : null;

    /// <summary>
    /// Os atributos extras menos <c>class</c> e <c>style</c>, que ja foram para a raiz.
    /// </summary>
    internal IReadOnlyDictionary<string, object>? AtributosDoInput
        => AdditionalAttributes is null
            ? null
            : AdditionalAttributes
                .Where(a => !string.Equals(a.Key, "class", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(a.Key, "style", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(a => a.Key, a => a.Value);

    internal string IdApoio => $"{IdEfetivo}-apoio";

    internal string IdPrefixo => $"{IdEfetivo}-prefixo";

    internal string IdSufixo => $"{IdEfetivo}-sufixo";

    internal string TipoHtml => Type switch
    {
        RvmInputType.Email => "email",
        RvmInputType.Password => "password",
        RvmInputType.Number => "number",
        RvmInputType.Tel => "tel",
        RvmInputType.Url => "url",
        RvmInputType.Search => "search",
        _ => "text"
    };

    /// <summary>
    /// Sem placeholder do consumidor, sai um espaco: e o que faz o rotulo flutuar so com CSS
    /// (`:placeholder-shown`), sem JS — a regra dos dois modos de hospedagem.
    /// </summary>
    internal string PlaceholderEfetivo => string.IsNullOrEmpty(Placeholder) ? " " : Placeholder;

    internal string? MensagemDeErro
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(ErrorText))
            {
                return ErrorText;
            }

            return EditContext?.GetValidationMessages(FieldIdentifier).FirstOrDefault();
        }
    }

    internal bool TemErro => MensagemDeErro is not null;

    internal string? MensagemDeApoio => MensagemDeErro ?? (string.IsNullOrWhiteSpace(HelperText) ? null : HelperText);

    internal string? DescritoPor
    {
        get
        {
            var ids = new List<string>(3);
            if (!string.IsNullOrWhiteSpace(Prefix))
            {
                ids.Add(IdPrefixo);
            }

            if (!string.IsNullOrWhiteSpace(Suffix))
            {
                ids.Add(IdSufixo);
            }

            if (MensagemDeApoio is not null)
            {
                ids.Add(IdApoio);
            }

            return ids.Count == 0 ? null : string.Join(' ', ids);
        }
    }

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = string.Join(' ',
                "campo",
                Variant switch { RvmTextFieldVariant.Filled => "preenchido", RvmTextFieldVariant.Standard => "padrao", _ => "contorno" },
                Size == RvmSize.Small ? "pequeno" : "medio");

            if (!string.IsNullOrEmpty(Placeholder)) proprias += " rotulo-fixo";
            if (!string.IsNullOrWhiteSpace(Prefix)) proprias += " com-prefixo";
            if (TemErro) proprias += " erro";
            if (Disabled) proprias += " desabilitado";

            // `CssClass` do InputBase traz as classes de validacao do EditForm ("modified", "invalid")
            // e a `class` que o consumidor mandou.
            return string.IsNullOrWhiteSpace(CssClass) ? proprias : $"{proprias} {CssClass}";
        }
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(
        string? value,
        out string? result,
        [NotNullWhen(false)] out string? validationErrorMessage)
    {
        result = value;
        validationErrorMessage = null;
        return true;
    }

    private void AoMudar(ChangeEventArgs e) => CurrentValueAsString = e.Value?.ToString();
}
