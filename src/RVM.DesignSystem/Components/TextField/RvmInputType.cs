namespace RVM.DesignSystem.Components.TextField;

/// <summary>
/// O <c>type</c> do campo. Importa no celular: <see cref="Email"/> abre o teclado com arroba,
/// <see cref="Tel"/> abre o numerico, e <see cref="Password"/> esconde o que se digita.
/// </summary>
public enum RvmInputType
{
    /// <summary>Texto livre.</summary>
    Text,

    /// <summary>E-mail.</summary>
    Email,

    /// <summary>Senha — o texto nao aparece.</summary>
    Password,

    /// <summary>Numero.</summary>
    Number,

    /// <summary>Telefone.</summary>
    Tel,

    /// <summary>Endereco web.</summary>
    Url,

    /// <summary>Busca.</summary>
    Search
}
