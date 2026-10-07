namespace RVM.DesignSystem.Theming;

/// <summary>
/// Uma paleta que o usuario pode escolher no <see cref="RvmThemePicker"/>: a cor de destaque e, nas paletas de
/// produto, a fonte. O CSS de cada uma mora no <c>rvm-design-system.css</c>, no seletor
/// <c>[data-rvm-accent="Id"]</c> (e <c>[data-theme='dark'][data-rvm-accent="Id"]</c> para o escuro). Um app pode
/// ter as proprias: declara esses seletores no CSS dele, sobrescrevendo os tokens <c>--rvm-color-primary-*</c>, e
/// passa a paleta ao picker. Contrato com o RVM.UI (DSGN-017).
/// </summary>
/// <param name="Id">Valor do atributo <c>data-rvm-accent</c>. Letras minusculas, digitos e hifen.</param>
/// <param name="Name">Nome mostrado ao usuario.</param>
/// <param name="Description">Uma frase dizendo o carater da paleta.</param>
public sealed record RvmPalette(string Id, string Name, string Description);
