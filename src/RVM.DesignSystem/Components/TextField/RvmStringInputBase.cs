using System.Diagnostics.CodeAnalysis;

namespace RVM.DesignSystem.Components.TextField;

/// <summary>Base dos campos de texto livre (contrato com o RVM.UI, DSGN-017): o valor e o proprio texto digitado.</summary>
public abstract class RvmStringInputBase : RvmInputBase<string?>
{
    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, out string? result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        result = value;
        validationErrorMessage = null;
        return true;
    }
}
