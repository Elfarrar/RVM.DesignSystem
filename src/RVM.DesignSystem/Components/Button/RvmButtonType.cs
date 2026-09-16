namespace RVM.DesignSystem.Components.Button;

/// <summary>
/// O <c>type</c> do elemento. Importa dentro de formulario: o padrao do HTML e <c>submit</c>, e um
/// botao de "cancelar" que esqueca isso envia o formulario sem querer. Aqui o padrao e
/// <see cref="Button"/>, e quem envia declara.
/// </summary>
public enum RvmButtonType
{
    /// <summary>Nao envia nada. O padrao.</summary>
    Button,

    /// <summary>Envia o formulario.</summary>
    Submit,

    /// <summary>Limpa o formulario.</summary>
    Reset
}
