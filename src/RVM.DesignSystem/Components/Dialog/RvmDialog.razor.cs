using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace RVM.DesignSystem.Components.Dialog;

/// <summary>
/// Janela modal por cima da pagina: aviso, confirmacao, formulario curto ou tela cheia.
/// </summary>
public partial class RvmDialog : ComponentBase, IAsyncDisposable
{
    private static int _proximoId;
    private readonly string _idBase = $"rvm-dialogo-{Interlocked.Increment(ref _proximoId)}";
    private ElementReference _caixa;
    private Sobreposicao? _sobreposicao;
    private bool _focoPreso;

    [Inject] private IJSRuntime JS { get; set; } = default!;

    /// <summary>Aberto. Aceita <c>@bind-Open</c>.</summary>
    [Parameter] public bool Open { get; set; }

    /// <summary>Disparado quando o dialogo se fecha sozinho (Esc, fundo, botao de fechar).</summary>
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }

    /// <summary>O titulo. Tambem e o nome acessivel do dialogo.</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>O corpo.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Os botoes do rodape, alinhados a direita.</summary>
    [Parameter] public RenderFragment? Actions { get; set; }

    /// <summary>
    /// Largura maxima: 400 px (<see cref="RvmSize.Small"/>), 600 px (padrao, a do kit) ou 900 px.
    /// </summary>
    [Parameter] public RvmSize Size { get; set; } = RvmSize.Medium;

    /// <summary>Ocupa a tela inteira — para formulario longo no celular.</summary>
    [Parameter] public bool FullScreen { get; set; }

    /// <summary>
    /// Confirmacao que exige resposta: vira <c>alertdialog</c>, o corpo e lido junto com o titulo e
    /// clicar no fundo nao fecha.
    /// </summary>
    [Parameter] public bool Alert { get; set; }

    /// <summary>Clicar fora fecha. Padrao: sim (menos no <see cref="Alert"/>).</summary>
    [Parameter] public bool CloseOnBackdropClick { get; set; } = true;

    /// <summary>Esc fecha. Padrao: sim.</summary>
    [Parameter] public bool CloseOnEscape { get; set; } = true;

    /// <summary>Botao de fechar (x) no topo.</summary>
    [Parameter] public bool ShowCloseButton { get; set; }

    /// <summary>Nome acessivel do botao de fechar. Padrao: "Fechar".</summary>
    [Parameter] public string CloseLabel { get; set; } = "Fechar";

    /// <summary>Atributos extras, repassados a caixa do dialogo.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal string IdTitulo => $"{_idBase}-titulo";

    internal string IdConteudo => $"{_idBase}-conteudo";

    internal string ClassesDaRaiz
    {
        get
        {
            var proprias = "dialogo " + (FullScreen ? "tela-cheia" : Size switch
            {
                RvmSize.Small => "pequeno",
                RvmSize.Large => "grande",
                _ => "medio"
            });

            return AdditionalAttributes is not null
                   && AdditionalAttributes.TryGetValue("class", out var informada)
                   && informada is string texto
                   && !string.IsNullOrWhiteSpace(texto)
                ? $"{proprias} {texto}"
                : proprias;
        }
    }

    /// <summary>Fecha o dialogo e avisa quem estiver ligado por <c>@bind-Open</c>.</summary>
    public async Task FecharAsync()
    {
        if (!Open)
        {
            return;
        }

        Open = false;
        await OpenChanged.InvokeAsync(false);
        StateHasChanged();
    }

    private async Task AoClicarNoFundoAsync()
    {
        if (CloseOnBackdropClick && !Alert)
        {
            await FecharAsync();
        }
    }

    private async Task AoTeclarAsync(KeyboardEventArgs e)
    {
        if (e.Key == "Escape" && CloseOnEscape)
        {
            await FecharAsync();
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Open && !_focoPreso)
        {
            _focoPreso = true;
            _sobreposicao ??= new Sobreposicao(JS);
            await _sobreposicao.AbrirAsync(_caixa);
        }
        else if (!Open && _focoPreso)
        {
            _focoPreso = false;
            if (_sobreposicao is not null)
            {
                await _sobreposicao.FecharAsync();
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_sobreposicao is not null)
        {
            await _sobreposicao.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }
}
