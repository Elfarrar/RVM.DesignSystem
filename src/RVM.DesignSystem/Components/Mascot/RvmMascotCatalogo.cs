using RVM.DesignSystem.Icons;

namespace RVM.DesignSystem.Components.Mascot;

/// <summary>O icone, o papel de cor e o texto alternativo padrao de cada mascote.</summary>
internal static class RvmMascotCatalogo
{
    internal static (RvmIconName Icone, RvmColor Cor, string Alt) De(RvmMascotName nome) => nome switch
    {
        RvmMascotName.AlienDocumentation => (RvmIconName.Book, RvmColor.Primary, "Ilustracao: documentacao"),
        RvmMascotName.AlienIntegrations => (RvmIconName.PlugCircle, RvmColor.Primary, "Ilustracao: integracoes"),
        RvmMascotName.AlienNotifications => (RvmIconName.BellBing, RvmColor.Primary, "Ilustracao: notificacoes"),
        RvmMascotName.AlienOnboardingSteps => (RvmIconName.ListCheck, RvmColor.Primary, "Ilustracao: primeiros passos"),
        RvmMascotName.AlienSecureLogin => (RvmIconName.ShieldKeyhole, RvmColor.Primary, "Ilustracao: acesso seguro"),
        RvmMascotName.AlienSessionExpired => (RvmIconName.Hourglass, RvmColor.Warning, "Ilustracao: sessao expirada"),
        RvmMascotName.DogEmptyState => (RvmIconName.BoxMinimalistic, RvmColor.Secondary, "Ilustracao: nada por aqui ainda"),
        RvmMascotName.DogFeedback => (RvmIconName.ChatSquareLike, RvmColor.Primary, "Ilustracao: opiniao"),
        RvmMascotName.DogNoConnection => (RvmIconName.LinkBroken, RvmColor.Warning, "Ilustracao: sem conexao"),
        RvmMascotName.DogSearch => (RvmIconName.Search, RvmColor.Secondary, "Ilustracao: busca sem resultado"),
        RvmMascotName.RafaelCodeReview => (RvmIconName.Code, RvmColor.Primary, "Ilustracao: revisao de codigo"),
        RvmMascotName.RobotAccessDenied => (RvmIconName.ShieldCross, RvmColor.Error, "Ilustracao: acesso negado"),
        RvmMascotName.RobotDogSmallSuccess => (RvmIconName.CircleCheck, RvmColor.Success, "Ilustracao: deu certo"),
        RvmMascotName.RobotError => (RvmIconName.AlertTriangle, RvmColor.Error, "Ilustracao: algo deu errado"),
        RvmMascotName.RobotLoading => (RvmIconName.Loader, RvmColor.Primary, "Ilustracao: carregando"),
        RvmMascotName.RobotMaintenance => (RvmIconName.Sledgehammer, RvmColor.Warning, "Ilustracao: em manutencao"),
        RvmMascotName.RobotPermissionRequested => (RvmIconName.Key, RvmColor.Info, "Ilustracao: permissao necessaria"),
        RvmMascotName.RobotSettings => (RvmIconName.Settings, RvmColor.Secondary, "Ilustracao: configuracoes"),
        RvmMascotName.RobotUpdateAvailable => (RvmIconName.Refresh, RvmColor.Info, "Ilustracao: atualizacao disponivel"),
        RvmMascotName.RobotUploadingFile => (RvmIconName.CloudUpload, RvmColor.Primary, "Ilustracao: enviando arquivo"),
        RvmMascotName.TeamAnalytics => (RvmIconName.Chart, RvmColor.Primary, "Ilustracao: analise de dados"),
        RvmMascotName.TeamIdeaToProduct => (RvmIconName.Lightbulb, RvmColor.Warning, "Ilustracao: da ideia ao produto"),
        RvmMascotName.TeamSecurity => (RvmIconName.ShieldCheck, RvmColor.Success, "Ilustracao: seguranca"),
        _ => throw new ArgumentOutOfRangeException(nameof(nome), nome, null)
    };
}
