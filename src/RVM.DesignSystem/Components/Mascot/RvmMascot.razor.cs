using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Mascot;

/// <summary>
/// O mascote do contrato com o RVM.UI (DSGN-017): o icone da funcao (erro, carregando, vazio) num circulo de fundo
/// suave. Usado sozinho ou pelos estados de <c>RvmEmptyState</c>, listas, tabelas e graficos.
/// </summary>
public partial class RvmMascot : ComponentBase
{
    /// <summary>Qual mascote. O nome diz a funcao na interface, nao o personagem.</summary>
    [Parameter, EditorRequired] public RvmMascotName Name { get; set; }

    /// <summary>
    /// Troca o texto alternativo padrao. Use quando a tela der um contexto que o padrao nao tem — "nenhum pedido
    /// neste mes" diz mais do que "nada por aqui ainda".
    /// </summary>
    [Parameter] public string? Alt { get; set; }

    /// <summary>Enfeite ao lado de um texto que ja diz tudo: some do leitor de tela.</summary>
    [Parameter] public bool Decorative { get; set; }

    /// <summary>Diametro do circulo em pixels. Padrao: 160.</summary>
    [Parameter] public int Width { get; set; } = 160;

    /// <summary>Do contrato com o RVM.UI, onde escolhe o arquivo da ilustracao. Aqui nao ha arquivo: sem efeito.</summary>
    [Parameter] public string? Sizes { get; set; }

    /// <summary>Do contrato com o RVM.UI, onde desliga o carregamento preguicoso da imagem. Aqui nao ha imagem: sem efeito.</summary>
    [Parameter] public bool Eager { get; set; }

    /// <summary>Classe CSS extra no elemento raiz.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Atributos repassados ao elemento raiz.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal (RvmIconName Icone, RvmColor Cor, string Alt) Desenho => RvmMascotCatalogo.De(Name);

    internal string TextoAlternativo => string.IsNullOrWhiteSpace(Alt) ? Desenho.Alt : Alt;

    // O icone ocupa 45% do circulo: grande o bastante para ler a funcao, com respiro para o fundo aparecer.
    internal int LadoDoIcone => Math.Max(16, Width * 45 / 100);

    internal string ClassesDaRaiz => ClassesCss.Juntar("rvm-mascote " + Desenho.Cor switch
    {
        RvmColor.Secondary => "rvm-secondary",
        RvmColor.Info => "rvm-info",
        RvmColor.Success => "rvm-success",
        RvmColor.Warning => "rvm-warning",
        RvmColor.Error => "rvm-error",
        _ => "rvm-primary"
    }, Class, AdditionalAttributes);

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (Width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(Width), Width, "O diametro do mascote precisa ser maior que zero.");
        }
    }
}
