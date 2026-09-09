using Bunit;
using RVM.DesignSystem.Components;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>
/// O conjunto de icones nao pode ENCOLHER.
/// </summary>
/// <remarks>
/// A `1.0.0` congelou o contrato. Um nome de icone e API publica tanto quanto um parametro:
/// quem escreveu <c>&lt;RvmIcon Name="funnel" /&gt;</c> quebra se o nome sair, e quebra em
/// TEMPO DE EXECUCAO, nao de compilacao — a pior forma.
///
/// <para>
/// O risco e real e nao teorico: a lista vive em <c>tools/gerar-icones.py</c> e o
/// <c>RvmIconData.cs</c> e REGERADO por inteiro a cada mudanca. Uma linha apagada por descuido
/// no script apaga o icone do pacote, e nada mais avisaria.
/// </para>
///
/// <para>
/// A lista abaixo e a da <b>1.0.0</b>, escrita a mao de proposito. Ela nao deve ser gerada da
/// lista atual — um teste que le a mesma fonte que verifica nao verifica nada.
/// </para>
/// </remarks>
public class ConjuntoDeIconesTests : BunitContext
{
    /// <summary>Os 27 icones que existiam quando o contrato congelou.</summary>
    public static TheoryData<string> IconesDaV1 =>
    [
        "check", "x", "caret-down", "caret-up", "eye", "eye-slash",
        "warning", "info", "circle-notch", "magnifying-glass", "calendar-blank",
        "list", "caret-right", "caret-left", "sidebar-simple", "user", "house",
        "dots-three-vertical", "copy", "check-circle", "x-circle", "lock", "tray",
        "caret-double-left", "caret-double-right", "arrows-down-up", "funnel",
    ];

    [Theory]
    [MemberData(nameof(IconesDaV1))]
    public void Todo_icone_da_v1_continua_desenhando(string nome)
    {
        foreach (var peso in new[] { RvmIconWeight.Regular, RvmIconWeight.Fill })
        {
            var cut = Render<RvmIcon>(p => p
                .Add(x => x.Name, nome)
                .Add(x => x.Weight, peso));

            var svg = cut.Find("svg");

            // Nao basta o <svg> existir: um nome desconhecido poderia render um SVG vazio, que
            // passaria despercebido na tela como um espaco em branco.
            Assert.NotEmpty(svg.InnerHtml.Trim());
        }
    }
}
