namespace RVM.DesignSystem.Components;

/// <summary>
/// Um contador de ids para toda a biblioteca. Componente GENERICO nao pode ter o proprio campo estatico:
/// ele existe um por tipo (RvmSelect&lt;string&gt; e RvmSelect&lt;TimeOnly?&gt; tinham cada um o seu), e dois
/// componentes com tipos diferentes na mesma pagina nasciam com o mesmo id — rotulo, lista e degrade
/// apontando para o elemento do outro. Pego no navegador pelo grafico de area.
/// </summary>
internal static class GeradorDeIds
{
    private static int _ultimo;

    public static string Novo(string prefixo) => $"{prefixo}-{Interlocked.Increment(ref _ultimo)}";
}
