using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Menu;

namespace RVM.DesignSystem.Components.Kanban;

/// <summary>
/// Quadro de tarefas em colunas. Mover avisa <see cref="OnMove"/> — o quadro NAO altera as listas: o aplicativo
/// atualiza <see cref="Columns"/>. Move-se arrastando com o mouse ou, pelo teclado e no toque, pelos itens
/// "Mover para X" que o <see cref="ItemTemplate"/> recebe para por no menu do cartao.
/// </summary>
/// <typeparam name="TItem">O tipo do cartao.</typeparam>
public partial class RvmKanbanBoard<TItem> : ComponentBase
{
    private readonly string _id = GeradorDeIds.Novo("rvm-kanban");
    private bool _arrastando;
    private TItem? _arrastado;
    private string? _origem;
    private string? _sobre;
    private int _profundidade;
    private string? _anuncio;

    /// <summary>As colunas, na ordem em que aparecem.</summary>
    [Parameter, EditorRequired] public IReadOnlyList<RvmKanbanColumn<TItem>> Columns { get; set; } = [];

    /// <summary>
    /// Como desenhar cada cartao. Recebe o item e os itens de menu "Mover para X" ja prontos (<c>MoveItems</c>) —
    /// coloca-los num <see cref="RvmMenu"/> do cartao e o que faz o quadro funcionar no teclado e no toque.
    /// </summary>
    [Parameter, EditorRequired] public RenderFragment<(TItem Item, RenderFragment MoveItems)> ItemTemplate { get; set; } = default!;

    /// <summary>O titulo do cartao, usado no aviso de movimento ("Colher talhao 3 movido para Feito").</summary>
    [Parameter] public Func<TItem, string>? ItemTitleSelector { get; set; }

    /// <summary>Canto direito do titulo de cada coluna: adicionar, menu.</summary>
    [Parameter] public RenderFragment<RvmKanbanColumn<TItem>>? ColumnActions { get; set; }

    /// <summary>
    /// Avisa que um cartao deve mudar de coluna. O aplicativo devolve <see cref="Columns"/> com o cartao no lugar
    /// novo. Para recusar, lance: o quadro anuncia ao leitor de tela que nao moveu, e a excecao nao derruba a pagina.
    /// </summary>
    [Parameter] public EventCallback<RvmKanbanMove<TItem>> OnMove { get; set; }

    /// <summary>Nome acessivel do quadro. Padrao: "Quadro de tarefas".</summary>
    [Parameter] public string Label { get; set; } = "Quadro de tarefas";

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos repassados ao elemento raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string ClassesDaRaiz => ClassesCss.Juntar("rvm-kanban", Class, AdditionalAttributes);

    private bool EAlvo(RvmKanbanColumn<TItem> coluna) => _arrastando && _sobre == coluna.Id && _origem != coluna.Id;

    private string ClassesDaColuna(RvmKanbanColumn<TItem> coluna)
        => EAlvo(coluna) ? "rvm-coluna rvm-alvo" : "rvm-coluna";

    private string ClassesDoCartao(TItem item)
        => _arrastando && EqualityComparer<TItem>.Default.Equals(item, _arrastado) ? "rvm-cartao rvm-arrastando" : "rvm-cartao";

    private string TituloDoItem(TItem item) => ItemTitleSelector?.Invoke(item) ?? "Cartao";

    /// <summary>Os itens de menu que levam o cartao para cada uma das OUTRAS colunas.</summary>
    private RenderFragment MoverPara(TItem item, RvmKanbanColumn<TItem> origem) => builder =>
    {
        foreach (var destino in Columns.Where(c => c.Id != origem.Id))
        {
            builder.OpenComponent<RvmMenuItem>(0);
            builder.SetKey(destino.Id);
            builder.AddComponentParameter(1, nameof(RvmMenuItem.Text), $"Mover para {destino.Title}");
            builder.AddComponentParameter(2, nameof(RvmMenuItem.OnClick),
                EventCallback.Factory.Create(this, () => AvisarAsync(item, origem.Id, destino)));
            builder.CloseComponent();
        }
    };

    private void Pegou(TItem item, string colunaId)
    {
        _arrastando = true;
        _arrastado = item;
        _origem = colunaId;
    }

    private void Terminou()
    {
        _arrastando = false;
        _arrastado = default;
        _origem = null;
        _sobre = null;
        _profundidade = 0;
    }

    // dragenter do filho chega ANTES do dragleave do pai: um contador evita que a area de soltar pisque ao
    // passar o cursor sobre os cartoes da coluna.
    private void Entrou(string colunaId)
    {
        if (_sobre != colunaId)
        {
            _sobre = colunaId;
            _profundidade = 0;
        }

        _profundidade++;
    }

    private void Saiu(string colunaId)
    {
        if (_sobre == colunaId && --_profundidade <= 0)
        {
            _sobre = null;
            _profundidade = 0;
        }
    }

    private async Task SoltarAsync(RvmKanbanColumn<TItem> destino)
    {
        var (arrastando, item, origem) = (_arrastando, _arrastado, _origem);
        Terminou();

        if (!arrastando || origem is null || origem == destino.Id)
        {
            return;
        }

        await AvisarAsync(item!, origem, destino);
    }

    private async Task AvisarAsync(TItem item, string origem, RvmKanbanColumn<TItem> destino)
    {
        // Entra no fim da coluna de destino: e onde a area de soltar aparece.
        try
        {
            await OnMove.InvokeAsync(new RvmKanbanMove<TItem>(item, origem, destino.Id, destino.Items.Count));
        }
        catch (Exception)
        {
            // A recusa do app chega como excecao (o contrato). Sai do handler de DOM sem tratamento derrubaria o
            // circuito no Server: o quadro anuncia que nao moveu e segue (achado do review).
            _anuncio = $"Nao foi possivel mover {TituloDoItem(item)} para {destino.Title}.";
            return;
        }

        _anuncio = $"{TituloDoItem(item)} movido para {destino.Title}.";
    }
}
