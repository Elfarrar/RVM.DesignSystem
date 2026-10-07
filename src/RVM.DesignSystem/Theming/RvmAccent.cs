namespace RVM.DesignSystem.Theming;

/// <summary>
/// Cor de destaque do app: a familia que alimenta os tokens <c>--rvm-color-primary-*</c> (botao cheio, link,
/// foco, aba ativa, menu ativo). Contrato com o RVM.UI (DSGN-017). No DS o padrao e <see cref="Blue"/>, o
/// cobalto do kit NEATLAB: quem nao escolhe nada continua com a mesma cor de sempre.
/// </summary>
public enum RvmAccent
{
    /// <summary>Violeta da identidade RVM (rampa desenhada no DS, contraste AA medido).</summary>
    Purple,

    /// <summary>Cobalto do kit NEATLAB (<c>#264CC8</c>). Padrao no DS.</summary>
    Blue,

    /// <summary>Neutro escuro, sem cor de destaque. No tema escuro inverte e fica claro.</summary>
    Black
}
