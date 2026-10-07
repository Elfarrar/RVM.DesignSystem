using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RVM.DesignSystem.Components.Button;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.IconSelector;

/// <summary>
/// Seletor de icone do contrato com o RVM.UI (DSGN-017): um circulo com o icone escolhido (ou um "+") e o botao que
/// abre a janela com a busca e a grade de icones. Valor nulo = nenhum icone.
/// </summary>
/// <remarks>
/// A grade e um grupo nomeado de botoes de alternancia (<c>aria-pressed</c>) com foco itinerante: um Tab entra,
/// as setas andam (6 por linha), Home/End vao as pontas e Enter/Espaco escolhem e fecham a janela.
/// </remarks>
public partial class RvmIconSelector : IAsyncDisposable
{
    /// <summary>Botoes por linha da grade — as setas para cima e para baixo andam isso.</summary>
    internal const int Colunas = 6;

    private readonly Dictionary<RvmIconName, ElementReference> _botoes = [];
    private List<RvmIconName> _visiveis = [];
    private int _total;
    private int _ativo;
    private bool _aberto;
    private string? _busca;
    private bool _focarNaGrade;
    private bool _focarNoGatilho;
    private ElementReference _grade;
    private string? _gradePresa;
    private ElementReference _caixaDaBusca;
    private string? _buscaPresa;
    private RvmButton? _gatilho;
    private IJSObjectReference? _modulo;

    [Inject] private IJSRuntime JS { get; set; } = default!;

    /// <summary>Classe CSS extra na raiz do seletor.</summary>
    [Parameter] public string? Class { get; set; }

    /// <inheritdoc />
    protected override string? ClasseDoCampo => Class;

    /// <summary>Os icones oferecidos. Sem ele, todo o <see cref="RvmIconName"/>.</summary>
    [Parameter] public IEnumerable<RvmIconName>? Icons { get; set; }

    /// <summary>
    /// O nome do icone para a busca, a dica e o leitor de tela. Sem ele, o nome do enum em palavras
    /// ("AltArrowDown" vira "Alt arrow down").
    /// </summary>
    [Parameter] public Func<RvmIconName, string>? IconText { get; set; }

    /// <summary>
    /// Quantos icones a grade mostra de uma vez (os primeiros que casam com a busca). Padrao: 60. Zero ou menos: todos.
    /// </summary>
    [Parameter] public int MaxVisible { get; set; } = 60;

    internal string IdDoRotulo => $"{IdDoControle}-rotulo";

    private string ClassesDaRaiz
    {
        get
        {
            var proprias = "rvm-seletor-de-icone";
            if (Shape == RvmFieldShape.Pill) proprias += " rvm-pilula";
            if (MensagemDeErro is not null) proprias += " rvm-erro";
            if (Disabled) proprias += " rvm-desabilitado";
            return ClassesCss.Juntar(proprias, ClassesDoConsumidor, null);
        }
    }

    private string DescricaoDoCirculo
        => CurrentValue is { } icone ? $"Icone escolhido: {TextoDe(icone)}" : "Nenhum icone escolhido";

    private string Aviso
    {
        get
        {
            if (_total == 0) return "Nenhum icone encontrado. Tente outra palavra.";
            if (_visiveis.Count < _total) return $"Mostrando {_visiveis.Count} de {_total} — refine a busca";
            return _total == 1 ? "1 icone" : $"{_total} icones";
        }
    }

    /// <summary>O nome do icone: o do <see cref="IconText"/>, ou o do enum em palavras.</summary>
    internal string TextoDe(RvmIconName icone) => IconText?.Invoke(icone) ?? EmPalavras(icone.ToString());

    /// <summary>"AltArrowDown" vira "Alt arrow down"; "Widget2" vira "Widget 2".</summary>
    internal static string EmPalavras(string nome)
    {
        var texto = new StringBuilder(nome.Length + 8);
        for (var i = 0; i < nome.Length; i++)
        {
            var c = nome[i];
            if (i > 0)
            {
                var anterior = nome[i - 1];
                var novaPalavra = (char.IsUpper(c) && (char.IsLower(anterior) || char.IsDigit(anterior)))
                    || (char.IsDigit(c) && char.IsLetter(anterior));
                if (novaPalavra) texto.Append(' ');
            }

            texto.Append(i == 0 ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c));
        }

        return texto.ToString();
    }

    /// <summary>
    /// Minusculas e sem acento, para a busca. Tabela propria, e nao <c>string.Normalize</c>: sem ICU (WebAssembly em
    /// modo invariante) a normalizacao nao e garantida.
    /// </summary>
    internal static string Normalizar(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return "";
        const string comAcento = "áàâãäéèêëíìîïóòôõöúùûüçñ";
        const string semAcento = "aaaaaeeeeiiiiooooouuuucn";
        var saida = new StringBuilder(texto.Length);
        foreach (var c in texto.Trim().ToLowerInvariant())
        {
            var i = comAcento.IndexOf(c);
            saida.Append(i >= 0 ? semAcento[i] : c);
        }

        return saida.ToString();
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, out RvmIconName? result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        validationErrorMessage = null;
        result = null;
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        // So o nome: Enum.TryParse aceitaria "5" (o numero) ou "Leaf, Plus" (combinacao).
        var nome = value.Trim();
        if (char.IsLetter(nome[0]) && Enum.TryParse<RvmIconName>(nome, ignoreCase: false, out var icone) && Enum.IsDefined(icone))
        {
            result = icone;
            return true;
        }

        validationErrorMessage = $"O icone \"{value}\" nao existe. Escolha um icone da lista.";
        return false;
    }

    private void Abrir()
    {
        if (Disabled) return;
        _busca = null;
        Filtrar();
        _aberto = true;
    }

    private void AoMudarAbertura(bool aberto) => _aberto = aberto;

    private void AoBuscar() => Filtrar();

    private void Filtrar()
    {
        var termo = Normalizar(_busca);
        var casam = (Icons ?? Enum.GetValues<RvmIconName>())
            .Distinct()
            .Where(i => termo.Length == 0 || Normalizar(TextoDe(i)).Contains(termo, StringComparison.Ordinal))
            .ToList();
        _total = casam.Count;
        _visiveis = MaxVisible > 0 ? casam.Take(MaxVisible).ToList() : casam;
        _ativo = CurrentValue is { } atual && _visiveis.IndexOf(atual) is var i and >= 0 ? i : 0;
    }

    private void AoTeclar(KeyboardEventArgs e)
    {
        if (_visiveis.Count == 0) return;
        var ultimo = _visiveis.Count - 1;
        int? destino = e.Key switch
        {
            "ArrowRight" => _ativo + 1,
            "ArrowLeft" => _ativo - 1,
            "ArrowDown" => _ativo + Colunas,
            "ArrowUp" => _ativo - Colunas,
            "Home" => 0,
            "End" => ultimo,
            _ => null
        };

        if (destino is { } d)
        {
            _ativo = Math.Clamp(d, 0, ultimo);
            _focarNaGrade = true;
            return;
        }

        if (e.Key is "Enter" or " " or "Spacebar")
        {
            Escolher(_visiveis[Math.Clamp(_ativo, 0, ultimo)]);
        }
    }

    private void Escolher(RvmIconName icone)
    {
        // Enter e Espaco tem o padrao barrado na grade (rvm-teclado.js): sem isso, o clique nativo do Enter (keypress) e
        // do Espaco (keyup) caia no botao "Trocar icone", que recebe o foco quando a janela fecha — e a janela reabria
        // (pego no navegador). Sem o modulo, o clique nativo chega aqui com a janela ja fechada e e ignorado.
        if (!_aberto) return;
        CurrentValue = icone;
        _aberto = false;
    }

    private void Remover()
    {
        if (Disabled) return;
        CurrentValue = null;
        // O botao "Remover icone" some: o foco volta para o botao principal, e nao para o corpo da pagina.
        _focarNoGatilho = true;
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_aberto && _caixaDaBusca.Id is { } idDaBusca && idDaBusca != _buscaPresa)
        {
            _buscaPresa = idDaBusca;
            await Tentar(async () =>
            {
                _modulo ??= await JS.InvokeAsync<IJSObjectReference>("import", "./_content/RVM.DesignSystem/rvm-teclado.js");
                await _modulo.InvokeVoidAsync("prenderTeclas", _caixaDaBusca, new[] { "Enter" });
            });
        }

        if (_aberto && _visiveis.Count > 0 && _grade.Id is { } id && id != _gradePresa)
        {
            _gradePresa = id;
            await Tentar(async () =>
            {
                _modulo ??= await JS.InvokeAsync<IJSObjectReference>("import", "./_content/RVM.DesignSystem/rvm-teclado.js");
                await _modulo.InvokeVoidAsync("prenderTeclas", _grade, new[] { "ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight", "Home", "End", "Enter", " " });
            });
        }

        if (_focarNaGrade)
        {
            _focarNaGrade = false;
            if (_aberto && _ativo < _visiveis.Count && _botoes.TryGetValue(_visiveis[_ativo], out var botao))
            {
                await Tentar(() => botao.FocusAsync());
            }
        }

        if (_focarNoGatilho)
        {
            _focarNoGatilho = false;
            if (_gatilho is not null)
            {
                await Tentar(() => _gatilho.FocusAsync());
            }
        }
    }

    private static async Task Tentar(Func<ValueTask> acao)
    {
        try
        {
            await acao();
        }
        catch (Exception e) when (e is JSException or JSDisconnectedException or InvalidOperationException or TaskCanceledException)
        {
            // Sem JS: a grade funciona pelo clique e pelo Tab; so o foco das setas e a rolagem nao sao controlados.
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        // O InputBase solta o EditContext no Dispose: chamado aqui porque, com IAsyncDisposable, o Blazor so chama este.
        ((IDisposable)this).Dispose();
        if (_modulo is not null)
        {
            try
            {
                await _modulo.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Circuito ja caiu: nao ha nada a liberar.
            }
        }

        GC.SuppressFinalize(this);
    }
}
