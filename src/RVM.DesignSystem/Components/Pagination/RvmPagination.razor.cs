using Microsoft.AspNetCore.Components;

namespace RVM.DesignSystem.Components.Pagination;

/// <summary>
/// Paginacao numerica, com setas e reticencias quando as paginas nao cabem.
/// </summary>
public partial class RvmPagination : ComponentBase
{
    /// <summary>A pagina atual, comecando em 1. Aceita <c>@bind-Page</c>.</summary>
    [Parameter] public int Page { get; set; } = 1;

    /// <summary>Disparado quando a pessoa escolhe outra pagina.</summary>
    [Parameter] public EventCallback<int> PageChanged { get; set; }

    /// <summary>Total de paginas.</summary>
    [Parameter, EditorRequired] public int Count { get; set; }

    /// <summary>Paginas vizinhas da atual sempre visiveis, de cada lado. Padrao: 1.</summary>
    [Parameter] public int SiblingCount { get; set; } = 1;

    /// <summary>Paginas sempre visiveis em cada ponta. Padrao: 1.</summary>
    [Parameter] public int BoundaryCount { get; set; } = 1;

    /// <summary>Estilo. Padrao: <see cref="RvmPaginationVariant.Text"/>.</summary>
    [Parameter] public RvmPaginationVariant Variant { get; set; } = RvmPaginationVariant.Text;

    /// <summary>Forma. Padrao: <see cref="RvmPaginationShape.Circular"/>.</summary>
    [Parameter] public RvmPaginationShape Shape { get; set; } = RvmPaginationShape.Circular;

    /// <summary>
    /// Cor da pagina atual. Sem valor, a atual ganha so um fundo neutro — a coluna "State: Active"
    /// do kit. Padrao: <see cref="RvmColor.Primary"/>.
    /// </summary>
    [Parameter] public RvmColor? Color { get; set; } = RvmColor.Primary;

    /// <summary>Itens de 26, 32 (padrao) ou 40 px — medidos no kit.</summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>Desabilita a paginacao inteira.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Nome da regiao para o leitor de tela. Padrao: "Paginacao".</summary>
    [Parameter] public string AriaLabel { get; set; } = "Paginacao";

    /// <summary>Atributos extras, repassados ao <c>nav</c>.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>A pagina informada, trazida para dentro de 1..Count — Page fora da faixa nao deixa seta morta.</summary>
    internal int PaginaAtual => Math.Clamp(Page, 1, Math.Max(Count, 1));

    /// <summary>
    /// Os itens na ordem: numero de pagina, ou <c>null</c> para reticencias. Uma lacuna de uma
    /// pagina so mostra o numero — "…" no lugar de um unico numero esconde sem economizar espaco.
    /// </summary>
    internal IReadOnlyList<int?> Itens
    {
        get
        {
            var total = Math.Max(Count, 0);
            if (total == 0)
            {
                return [];
            }

            var atual = PaginaAtual;
            var visiveis = new SortedSet<int>();
            for (var i = 1; i <= Math.Min(BoundaryCount, total); i++) visiveis.Add(i);
            for (var i = Math.Max(total - BoundaryCount + 1, 1); i <= total; i++) visiveis.Add(i);
            for (var i = Math.Max(atual - SiblingCount, 1); i <= Math.Min(atual + SiblingCount, total); i++) visiveis.Add(i);

            var itens = new List<int?>();
            var anterior = 0;
            foreach (var pagina in visiveis)
            {
                if (pagina - anterior == 2)
                {
                    itens.Add(anterior + 1);
                }
                else if (pagina - anterior > 2)
                {
                    itens.Add(null);
                }

                itens.Add(pagina);
                anterior = pagina;
            }

            return itens;
        }
    }

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = string.Join(' ',
                "paginacao",
                Variant == RvmPaginationVariant.Outlined ? "contorno" : "texto",
                Shape == RvmPaginationShape.Rounded ? "arredondado" : "circular",
                Size switch { RvmSize.Small => "pequeno", RvmSize.Large => "grande", _ => "medio" },
                Color switch
                {
                    null => "neutro",
                    RvmColor.Secondary => "secondary",
                    RvmColor.Info => "info",
                    RvmColor.Success => "success",
                    RvmColor.Warning => "warning",
                    RvmColor.Error => "error",
                    _ => "primary"
                });

            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }

    private async Task Ir(int pagina)
    {
        if (Disabled || pagina < 1 || pagina > Count || pagina == PaginaAtual)
        {
            return;
        }

        Page = pagina;
        await PageChanged.InvokeAsync(pagina);
    }
}
