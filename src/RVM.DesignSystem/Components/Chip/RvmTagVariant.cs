namespace RVM.DesignSystem.Components.Chip;

/// <summary>Os dois estilos do <see cref="RvmTagOption"/> (contrato com o RVM.UI, DSGN-017).</summary>
public enum RvmTagVariant
{
    /// <summary>Fundo suave do primario, texto na cor do primario (o "Soft" do chip do kit).</summary>
    Soft,

    /// <summary>Preenchido com o primario, texto no token de contraste (o "Filled" do chip do kit).</summary>
    Solid
}
