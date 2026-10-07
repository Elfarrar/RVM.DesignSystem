using Bunit;
using Microsoft.AspNetCore.Components;
using RVM.DesignSystem.Components.Accordion;
using RVM.DesignSystem.Components.Alert;
using RVM.DesignSystem.Components.AppShell;
using RVM.DesignSystem.Components.Avatar;
using RVM.DesignSystem.Components.Badge;
using RVM.DesignSystem.Components.Breadcrumbs;
using RVM.DesignSystem.Components.Button;
using RVM.DesignSystem.Components.Calendar;
using RVM.DesignSystem.Components.Card;
using RVM.DesignSystem.Components.Checkbox;
using RVM.DesignSystem.Components.Chip;
using RVM.DesignSystem.Components.DatePicker;
using RVM.DesignSystem.Components.Divider;
using RVM.DesignSystem.Components.EmptyState;
using RVM.DesignSystem.Components.Lists;
using RVM.DesignSystem.Components.Menu;
using RVM.DesignSystem.Components.Pagination;
using RVM.DesignSystem.Components.Progress;
using RVM.DesignSystem.Components.Rating;
using RVM.DesignSystem.Components.Select;
using RVM.DesignSystem.Components.Skeleton;
using RVM.DesignSystem.Components.Stepper;
using RVM.DesignSystem.Components.Switch;
using RVM.DesignSystem.Components.Tabs;
using RVM.DesignSystem.Components.TimePicker;
using RVM.DesignSystem.Components.Timeline;
using RVM.DesignSystem.Components.Tooltip;
using RVM.DesignSystem.Components.Typography;

namespace RVM.DesignSystem.Tests.Components;

/// <summary><c>Class</c> do contrato com o RVM.UI (DSGN-017): chega ao elemento raiz, e <c>class="..."</c> cai nele.</summary>
public class RvmClassDoContratoTests : BunitContext
{
    public RvmClassDoContratoTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    public static TheoryData<Type> Componentes =>
    [
        typeof(RvmAccordion), typeof(RvmAlert), typeof(RvmAppShell), typeof(RvmAvatar), typeof(RvmAvatarGroup),
        typeof(RvmBadge), typeof(RvmBreadcrumbs), typeof(RvmButton), typeof(RvmCalendar), typeof(RvmCard),
        typeof(RvmCheckbox), typeof(RvmChip), typeof(RvmDatePicker), typeof(RvmDivider), typeof(RvmEmptyState),
        typeof(RvmList), typeof(RvmMenu), typeof(RvmPagination), typeof(RvmProgress), typeof(RvmRating),
        typeof(RvmSelect<string>), typeof(RvmSkeleton), typeof(RvmStepper), typeof(RvmSwitch), typeof(RvmTabs),
        typeof(RvmTimePicker), typeof(RvmTimeline), typeof(RvmTooltip), typeof(RvmTypography)
    ];

    [Theory]
    [MemberData(nameof(Componentes))]
    public void Class_chega_ao_elemento_raiz(Type componente)
    {
        var cortado = Render(b =>
        {
            b.OpenComponent(0, componente);
            b.AddComponentParameter(1, "Class", "minha-classe");
            b.CloseComponent();
        });

        Assert.NotEmpty(cortado.FindAll(".minha-classe"));
    }

    [Fact]
    public void Divisor_do_menu_secao_e_grupo_da_navegacao_recebem_Class()
    {
        var cortado = Render<RvmAppShell>(p => p.Add(x => x.Navigation, (RenderFragment)(b =>
        {
            b.OpenComponent<RvmNavSection>(0);
            b.AddComponentParameter(1, nameof(RvmNavSection.Title), "Apps");
            b.AddComponentParameter(2, nameof(RvmNavSection.Class), "secao");
            b.AddComponentParameter(3, nameof(RvmNavSection.ChildContent), (RenderFragment)(g =>
            {
                g.OpenComponent<RvmNavGroup>(0);
                g.AddComponentParameter(1, nameof(RvmNavGroup.Text), "Painel");
                g.AddComponentParameter(2, nameof(RvmNavGroup.Class), "grupo");
                g.CloseComponent();
            }));
            b.CloseComponent();
        })));

        Assert.NotNull(cortado.Find("li.rvm-nav-secao.secao"));
        Assert.NotNull(cortado.Find("li.rvm-nav-item.grupo"));
        Assert.Equal("rvm-divisor minha", Render<RvmMenuDivider>(p => p.Add(x => x.Class, "minha")).Find("li").GetAttribute("class"));
    }

    [Fact]
    public void Class_da_aba_vai_ao_botao()
    {
        var cortado = Render<RvmTabs>(p => p.Add(x => x.AriaLabel, "Abas").AddChildContent<RvmTab>(t => t.Add(x => x.Title, "Um").Add(x => x.Class, "minha")));

        Assert.Contains("minha", cortado.Find("[role=tab]").ClassList);
    }
}
