namespace RVM.DesignSystem.Tests;

/// <summary>
/// Acha a raiz do repositorio subindo a partir da pasta de saida dos testes. Existe porque dois
/// testes leem ARQUIVO DE FONTE (o CSS dos tokens e o CSS isolado dos componentes) em vez de so
/// olhar o que foi renderizado — e caminho relativo cravado quebra assim que alguem mexe no
/// `TargetFramework` ou na pasta de build.
/// </summary>
internal static class RaizDoRepositorio
{
    internal static string Caminho { get; } = Procurar();

    private static string Procurar()
    {
        var pasta = new DirectoryInfo(AppContext.BaseDirectory);

        while (pasta is not null && !File.Exists(Path.Combine(pasta.FullName, "RVM.DesignSystem.slnx")))
        {
            pasta = pasta.Parent;
        }

        return pasta?.FullName
               ?? throw new InvalidOperationException(
                   "Nao achei a raiz do repositorio (o arquivo RVM.DesignSystem.slnx) subindo a partir de "
                   + AppContext.BaseDirectory);
    }

    internal static string Biblioteca(params string[] partes)
        => Path.Combine(new[] { Caminho, "src", "RVM.DesignSystem" }.Concat(partes).ToArray());
}
