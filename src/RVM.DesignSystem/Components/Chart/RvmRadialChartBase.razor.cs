using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Chart;

/// <summary>
/// O comum dos graficos de fatias sem eixo — rosca, medidor, bolhas e rosca de varias camadas (contrato com o RVM.UI,
/// DSGN-017): as fatias, a paleta, o total, o formato do valor, a figura com os estados, a dica e a tabela de dados
/// para leitor de tela. Cada tipo so desenha o SVG.
/// </summary>
/// <remarks>
/// O SVG tem <c>viewBox</c> fixo e escala para caber: sai pronto no primeiro render, sem JS, em Blazor Server e
/// WebAssembly.
/// </remarks>
public abstract partial class RvmRadialChartBase
{
    private static readonly Regex Hexadecimal = new("^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$", RegexOptions.Compiled);

    private static readonly RvmColor[] Papeis =
        [RvmColor.Primary, RvmColor.Success, RvmColor.Warning, RvmColor.Info, RvmColor.Error, RvmColor.Secondary];

    /// <summary>As fatias, na ordem em que o desenho as percorre.</summary>
    [Parameter, EditorRequired] public IReadOnlyList<RvmChartSlice> Slices { get; set; } = [];

    /// <summary>
    /// De onde saem as cores: degraus do primary (<see cref="RvmChartPalette.Monochromatic"/>, o padrao) ou os papeis do
    /// tema. Fatia com <see cref="RvmChartSlice.Color"/> valida usa a propria cor.
    /// </summary>
    [Parameter] public RvmChartPalette Palette { get; set; } = RvmChartPalette.Monochromatic;

    /// <summary>
    /// Como escrever o valor na tabela, na dica e no centro ("R$ 26.500", "1.240 ha"). Sem isto sai o numero na
    /// cultura corrente. O percentual vai junto, sempre.
    /// </summary>
    [Parameter] public Func<double, string>? FormatValue { get; set; }

    /// <summary>
    /// O total que vale 100%. Sem ele, o total e a soma das fatias ("como o bolo se divide"); com ele, o grafico
    /// responde "quanto de quanto" e o que falta aparece na trilha. Um total menor que a soma vale como a soma: as
    /// fatias nao dao mais de uma volta.
    /// </summary>
    [Parameter] public double? Total { get; set; }

    /// <summary>As fatias recebidas, nunca nulas.</summary>
    internal IReadOnlyList<RvmChartSlice> Fatias => Slices ?? [];

    /// <summary>A soma dos valores positivos.</summary>
    internal double Soma => Fatias.Sum(f => Math.Max(0, f.Value));

    /// <summary>O que vale 100%: o <see cref="Total"/> quando informado (e nao menor que a soma), senao a soma.</summary>
    internal double TotalEfetivo => Total is { } total && total > Soma ? total : Soma;

    /// <summary>Sobra volta para a trilha: o que falta para chegar ao total.</summary>
    internal bool ComTrilha => TotalEfetivo > Soma + 1e-9;

    /// <inheritdoc />
    private protected override bool SemDado => Soma <= 0;

    // --- O que cada tipo diz ---

    /// <summary>O conteudo do SVG (sem a tag svg), no sistema do <see cref="Largura"/> x <see cref="Altura"/>.</summary>
    internal abstract RenderFragment Desenho { get; }

    /// <summary>Largura do viewBox; e tambem a largura maxima do grafico em px.</summary>
    internal abstract double Largura { get; }

    /// <summary>Altura do viewBox.</summary>
    internal abstract double Altura { get; }

    /// <summary>A classe do tipo de grafico, na raiz.</summary>
    internal abstract string ClasseDoTipo { get; }

    /// <summary>Onde a dica de cada fatia aparece, no sistema do viewBox.</summary>
    internal abstract IReadOnlyList<Alvo> Alvos { get; }

    /// <summary>O que vai sobre o centro do desenho, em HTML. Nulo: nada.</summary>
    internal virtual RenderFragment? Centro => null;

    /// <summary>A caixa do centro, em % da area (estilo inline). Padrao: a area inteira.</summary>
    internal virtual string EstiloDoCentro => "inset: 0";

    /// <summary>A fracao da fatia mostrada no percentual. Padrao: o valor sobre o <see cref="TotalEfetivo"/>.</summary>
    internal virtual double FracaoDe(RvmChartSlice fatia)
        => TotalEfetivo <= 0 ? 0 : Math.Max(0, fatia.Value) / TotalEfetivo;

    /// <summary>A linha final da tabela (o 100%), ou nula. Padrao: o <see cref="Total"/>, quando informado.</summary>
    internal virtual (string Nome, double Valor)? LinhaFinal => Total is not null ? ("Total", TotalEfetivo) : null;

    /// <summary>A area de foco e dica de uma fatia, no sistema do viewBox.</summary>
    internal sealed record Alvo(int Indice, double X, double Y, double LarguraDoAlvo, double AlturaDoAlvo);

    // --- Formato e cor ---

    internal string Formatar(double valor) => FormatValue?.Invoke(valor) ?? RvmChartFormat.Number(valor);

    internal static string Percentual(double fracao) => RvmChartFormat.Number(Math.Round(fracao * 100, 1)) + "%";

    internal static string N(double valor) => valor.ToString("0.##", CultureInfo.InvariantCulture);

    /// <summary>
    /// A cor da fatia, para a custom property <c>--rvm-radial-cor</c>. Hex valido da fatia vence; senao a paleta:
    /// degraus do primary misturado ao papel (do cheio ao claro) ou os papeis do tema em ordem. Os tokens <c>-text</c>
    /// sao os que tem contraste contra o papel nos dois temas.
    /// </summary>
    internal string CorDa(int indice)
    {
        var propria = indice < Fatias.Count ? Fatias[indice].Color : null;
        if (propria is not null && Hexadecimal.IsMatch(propria))
        {
            return propria;
        }

        if (Palette == RvmChartPalette.FullColor)
        {
            return $"var(--rvm-color-{NomeDoPapel(Papeis[indice % Papeis.Length])}-text)";
        }

        var quantas = Math.Max(1, Fatias.Count);
        var degrau = quantas == 1 ? 100 : 100 - indice * 70d / (quantas - 1);
        return $"color-mix(in srgb, var(--rvm-color-primary-text) {N(degrau)}%, var(--rvm-color-background-paper))";
    }

    internal string EstiloDaCor(int indice) => $"--rvm-radial-cor: {CorDa(indice)}";

    // Switch, e nao ToString(): com os aliases do contrato o nome do enum e ambiguo.
    private static string NomeDoPapel(RvmColor cor) => cor switch
    {
        RvmColor.Success => "success",
        RvmColor.Warning => "warning",
        RvmColor.Info => "info",
        RvmColor.Error => "error",
        RvmColor.Secondary => "secondary",
        _ => "primary"
    };

    // --- Geometria comum ---

    /// <summary>Ponto na circunferencia; o angulo conta em graus a partir do alto, no sentido horario.</summary>
    internal static (double X, double Y) Ponto(double cx, double cy, double raio, double graus)
    {
        var rad = (graus - 90) * Math.PI / 180;
        return (cx + raio * Math.Cos(rad), cy + raio * Math.Sin(rad));
    }

    /// <summary>
    /// Um arco para desenhar com traco (<c>stroke</c>), de <paramref name="de"/> a <paramref name="ate"/> graus. A volta
    /// inteira nao existe como arco unico no SVG: vira dois arcos de meia volta.
    /// </summary>
    internal static string Arco(double cx, double cy, double raio, double de, double ate)
    {
        if (ate - de >= 359.999)
        {
            var (ax, ay) = Ponto(cx, cy, raio, de);
            var (bx, by) = Ponto(cx, cy, raio, de + 180);
            return $"M {N(ax)} {N(ay)} A {N(raio)} {N(raio)} 0 1 1 {N(bx)} {N(by)} A {N(raio)} {N(raio)} 0 1 1 {N(ax)} {N(ay)}";
        }

        var (x0, y0) = Ponto(cx, cy, raio, de);
        var (x1, y1) = Ponto(cx, cy, raio, ate);
        var grande = ate - de > 180 ? 1 : 0;
        return $"M {N(x0)} {N(y0)} A {N(raio)} {N(raio)} 0 {grande} 1 {N(x1)} {N(y1)}";
    }

    /// <summary>Uma fatia ja convertida em angulo (graus a partir do alto, no sentido horario).</summary>
    internal sealed record Pedaco(int Indice, double De, double Ate);

    /// <summary>
    /// As fatias com valor repartidas na <paramref name="varredura"/>, na proporcao do <see cref="TotalEfetivo"/>. A
    /// <paramref name="folga"/> sai do FIM de cada fatia (a primeira encosta no ponto de partida), so quando ha mais de
    /// uma coisa no anel; fatia menor que duas folgas fica sem ela, senao sumiria.
    /// </summary>
    internal List<Pedaco> PedacosNoArco(double inicio, double varredura, double folga)
    {
        var total = TotalEfetivo;
        var lista = new List<Pedaco>();
        if (total <= 0)
        {
            return lista;
        }

        var separar = Fatias.Count(f => f.Value > 0) > 1 || ComTrilha;
        var angulo = inicio;
        for (var i = 0; i < Fatias.Count; i++)
        {
            var valor = Math.Max(0, Fatias[i].Value);
            if (valor <= 0)
            {
                continue;
            }

            var fim = angulo + valor / total * varredura;
            lista.Add(new Pedaco(i, angulo, fim - (separar && fim - angulo > folga * 2 ? folga : 0)));
            angulo = fim;
        }

        return lista;
    }

    /// <summary>Um alvo de 44 do viewBox (o minimo da WCAG 2.5.5) centrado num ponto.</summary>
    internal static Alvo AlvoEm(int indice, double x, double y) => new(indice, x - 22, y - 22, 44, 44);

    // --- Marcacao ---

    internal string ClassesDaRaiz => ClassesCss.Juntar(
        $"rvm-radial {ClasseDoTipo}{(Animated ? " rvm-radial-animado" : "")}", Class, AdditionalAttributes);

    private string EstiloDaArea => $"max-width: {N(Largura)}px";

    private static string EstiloDoAlvo(Alvo alvo, double largura, double altura) => string.Create(CultureInfo.InvariantCulture,
        $"left: {alvo.X / largura * 100:0.##}%; top: {alvo.Y / altura * 100:0.##}%; "
        + $"width: {alvo.LarguraDoAlvo / largura * 100:0.##}%; height: {alvo.AlturaDoAlvo / altura * 100:0.##}%");

    internal string TextoDaFatia(RvmChartSlice fatia) => $"{fatia.Name}: {Formatar(fatia.Value)} ({Percentual(FracaoDe(fatia))})";
}
