namespace RVM.DesignSystem.Components.Toast;

/// <summary>
/// Acao de um aviso: um botao com <paramref name="Label"/> que, clicado, roda <paramref name="OnClick"/> e fecha
/// o aviso.
/// </summary>
/// <param name="Label">Texto do botao: o verbo do que acontece ("Desfazer", "Abrir agora").</param>
/// <param name="OnClick">O que o botao faz. O aviso fecha quando termina, tenha dado certo ou nao.</param>
public sealed record RvmToastAction(string Label, Func<Task> OnClick);
