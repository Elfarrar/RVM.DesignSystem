namespace RVM.DesignSystem.Theming;

/// <summary>
/// Esquema de cor: os degraus corrigidos para AA ou os degraus exatos do kit. Contrato com o RVM.UI (DSGN-017).
/// </summary>
public enum RvmColorScheme
{
    /// <summary>
    /// Degraus que passam 4.5:1 (WCAG AA) como texto: o <c>-text</c> das cores semanticas escurecido (claro) ou
    /// clareado (escuro), texto escuro sobre as cores claras, texto secundario a 0.72 e contorno de campo a 3:1. Padrao.
    /// </summary>
    Accessible,

    /// <summary>
    /// Degraus EXATOS do kit NEATLAB. ⚠️ Reprova AA: as cores semanticas como texto ficam entre 1.5:1 e 3.3:1, o
    /// branco sobre elas tambem, o texto secundario fica em 4.46:1 e o contorno do campo em 1.5:1. Use so onde a
    /// fidelidade ao kit vale mais que a leitura (uma tela de referencia, por exemplo). O destaque nao muda.
    /// </summary>
    Original
}
