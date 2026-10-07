using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Map;

/// <summary>
/// Cartao de mapa: titulo, marcadores posicionados por latitude e longitude sobre uma grade enquadrada neles, e a
/// lista de lugares em texto. Sem biblioteca de mapa nem tiles externos.
/// </summary>
public partial class RvmMap : ComponentBase
{
    private readonly string _id = GeradorDeIds.Novo("rvm-map");
    private Quadro _quadro = Quadro.De([]);

    /// <summary>Titulo do cartao. Tambem nomeia a secao.</summary>
    [Parameter, EditorRequired] public string? Title { get; set; }

    /// <summary>Linha de apoio abaixo do titulo.</summary>
    [Parameter] public string? Subtitle { get; set; }

    /// <summary>Os lugares marcados no desenho.</summary>
    [Parameter] public IReadOnlyList<RvmMapMarker> Markers { get; set; } = [];

    /// <summary>
    /// A lista de lugares ao pe do mapa, que carrega o dado em texto. Sem ela, uma lista oculta (so para leitor de
    /// tela) e gerada a partir de <see cref="Markers"/>.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Canto direito do cabecalho: menu, filtro, periodo.</summary>
    [Parameter] public RenderFragment? Actions { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos repassados ao elemento raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string ClassesDaRaiz => ClassesCss.Juntar("rvm-mapa", Class, AdditionalAttributes);

    private string IdDoTitulo => $"{_id}-titulo";

    private string RotuloDosMarcadores => string.IsNullOrWhiteSpace(Title) ? "Lugares no mapa" : $"Lugares no mapa: {Title}";

    private IReadOnlyList<RvmMapMarker> Visiveis { get; set; } = [];

    private IReadOnlyList<RvmMapMarker> ForaDoMapa { get; set; } = [];

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        Visiveis = [.. Markers.Where(Valido)];
        ForaDoMapa = [.. Markers.Where(m => !Valido(m))];
        _quadro = Quadro.De(Visiveis);
    }

    private static bool Valido(RvmMapMarker m)
        => m.Latitude is >= -90 and <= 90 && m.Longitude is >= -180 and <= 180;

    private string Aviso => ForaDoMapa.Count == 1
        ? $"{ForaDoMapa[0].Label} nao aparece no mapa: a coordenada esta fora do intervalo valido."
        : $"{ForaDoMapa.Count} lugares nao aparecem no mapa ({string.Join(", ", ForaDoMapa.Select(m => m.Label))}): as coordenadas estao fora do intervalo valido.";

    private static string Descricao(RvmMapMarker m)
        => string.IsNullOrWhiteSpace(m.Value) ? m.Label : $"{m.Label}: {m.Value}";

    // Na metade direita o rotulo abre para a esquerda: assim ele nao sai pela borda do desenho.
    private static string ClassesDoMarcador(RvmMapMarker m, double x)
        => $"rvm-marcador {PapelCss.Classe(m.Color)}{(x > 50 ? " rvm-rotulo-a-esquerda" : "")}";

    private static string Num(double v) => v.ToString("0.###", CultureInfo.InvariantCulture);

    /// <summary>
    /// O enquadramento: centro e alcance em graus, com a mesma escala nos dois eixos e a proporcao 16:9 do desenho.
    /// As posicoes saem em porcentagem do desenho.
    /// </summary>
    internal sealed class Quadro
    {
        private const double Proporcao = 16.0 / 9.0;

        /// <summary>Os marcadores ocupam o miolo: 15% de folga em cada borda, para o ponto e o rotulo.</summary>
        private const double Miolo = 0.7;

        private readonly double _centroLat;
        private readonly double _centroLon;
        private readonly double _alturaLat;
        private readonly double _larguraLon;

        private Quadro(double centroLat, double centroLon, double alturaLat, double larguraLon)
        {
            _centroLat = centroLat;
            _centroLon = centroLon;
            _alturaLat = alturaLat;
            _larguraLon = larguraLon;

            var passo = Passo(alturaLat / 3);
            Paralelos = Linhas(centroLat, alturaLat, passo).Select(Y).ToList();
            Meridianos = Linhas(centroLon, larguraLon, passo).Select(X).ToList();
        }

        /// <summary>Linhas horizontais da grade, em % do alto.</summary>
        public IReadOnlyList<double> Paralelos { get; }

        /// <summary>Linhas verticais da grade, em % da largura.</summary>
        public IReadOnlyList<double> Meridianos { get; }

        public double X(double longitude) => 50 + (longitude - _centroLon) / _larguraLon * 100;

        public double Y(double latitude) => 50 - (latitude - _centroLat) / _alturaLat * 100;

        public static Quadro De(IReadOnlyList<RvmMapMarker> marcadores)
        {
            if (marcadores.Count == 0)
            {
                return new Quadro(0, 0, 360 / Proporcao, 360);
            }

            var (minLat, maxLat) = (marcadores.Min(m => m.Latitude), marcadores.Max(m => m.Latitude));
            var (minLon, maxLon) = (marcadores.Min(m => m.Longitude), marcadores.Max(m => m.Longitude));
            var centroLat = (minLat + maxLat) / 2;
            var cosseno = Math.Max(Math.Cos(centroLat * Math.PI / 180), 0.01);

            // Em "graus de latitude": a longitude encolhe pelo cosseno. Minimo de ~1 km para um marcador so.
            var alto = Math.Max(maxLat - minLat, 0.01) / Miolo;
            var largo = Math.Max((maxLon - minLon) * cosseno, 0.01) / Miolo;
            if (largo < alto * Proporcao)
            {
                largo = alto * Proporcao;
            }
            else
            {
                alto = largo / Proporcao;
            }

            return new Quadro(centroLat, (minLon + maxLon) / 2, alto, largo / cosseno);
        }

        /// <summary>O passo "redondo" (1, 2 ou 5 vezes uma potencia de 10) mais proximo acima do desejado.</summary>
        private static double Passo(double desejado)
        {
            var potencia = Math.Pow(10, Math.Floor(Math.Log10(desejado)));
            return new[] { 1, 2, 5, 10 }.Select(f => f * potencia).First(p => p >= desejado);
        }

        private static IEnumerable<double> Linhas(double centro, double alcance, double passo)
        {
            var primeira = Math.Ceiling((centro - alcance / 2) / passo);
            var ultima = Math.Floor((centro + alcance / 2) / passo);
            for (var i = primeira; i <= ultima && i - primeira < 60; i++)
            {
                yield return i * passo;
            }
        }
    }
}
