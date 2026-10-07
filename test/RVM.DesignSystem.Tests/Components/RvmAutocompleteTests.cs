using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using RVM.DesignSystem.Components.Select;
using RVM.DesignSystem.Components.TextField;

namespace RVM.DesignSystem.Tests.Components;

/// <summary>O RvmAutocomplete do contrato com o RVM.UI (DSGN-017): combobox com lista, foco no campo.</summary>
public class RvmAutocompleteTests : BunitContext
{
    private static readonly string[] Cidades = ["Campinas", "Campo Grande", "Cascavel", "Curitiba", "Londrina"];

    private readonly FakeTimeProvider _relogio = new();

    public RvmAutocompleteTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<TimeProvider>(_relogio);
    }

    private static Task<IEnumerable<string?>> Buscar(string termo, CancellationToken _)
        => Task.FromResult<IEnumerable<string?>>(Cidades.Where(c => c.StartsWith(termo, StringComparison.OrdinalIgnoreCase)));

    private IRenderedComponent<RvmAutocomplete<string?>> Montar(
        Action<ComponentParameterCollectionBuilder<RvmAutocomplete<string?>>>? mais = null,
        Func<string, CancellationToken, Task<IEnumerable<string?>>>? busca = null,
        int espera = 0)
    {
        string? valor = null;
        return Render<RvmAutocomplete<string?>>(p =>
        {
            p.Add(x => x.Label, "Cidade")
             .Add(x => x.Id, "cidade")
             .Add(x => x.DebounceInterval, espera)
             .Add(x => x.SearchFunc, busca ?? Buscar)
             .Add(x => x.ValueExpression, () => valor);
            mais?.Invoke(p);
        });
    }

    [Fact]
    public void Fechado_o_combobox_tem_os_aria_e_nao_ha_lista()
    {
        var cortado = Montar();

        var input = cortado.Find("input");
        Assert.Equal("combobox", input.GetAttribute("role"));
        Assert.Equal("list", input.GetAttribute("aria-autocomplete"));
        Assert.Equal("false", input.GetAttribute("aria-expanded"));
        Assert.Null(input.GetAttribute("aria-controls"));
        Assert.Null(input.GetAttribute("aria-activedescendant"));
        Assert.Equal("off", input.GetAttribute("autocomplete"));
        Assert.Empty(cortado.FindAll("[role=listbox]"));
        Assert.NotNull(cortado.Find("[role=status][aria-live=polite]"));
    }

    [Fact]
    public void Digitar_busca_e_mostra_a_lista_ligada_ao_combobox()
    {
        var cortado = Montar();

        cortado.Find("input").Input("ca");

        var input = cortado.Find("input");
        Assert.Equal("true", input.GetAttribute("aria-expanded"));
        Assert.Equal("cidade-lista", input.GetAttribute("aria-controls"));
        var opcoes = cortado.FindAll("[role=listbox] > li[role=option]");
        Assert.Equal(["Campinas", "Campo Grande", "Cascavel"], opcoes.Select(o => o.TextContent.Trim()));
        Assert.Equal("cidade-lista", cortado.Find("[role=listbox]").Id);
    }

    [Fact]
    public void Focar_ja_busca_com_MinCharacters_zero()
    {
        var cortado = Montar();

        cortado.Find("input").Focus();

        Assert.Equal(5, cortado.FindAll("li[role=option]").Count);
    }

    [Fact]
    public void Abaixo_de_MinCharacters_nao_busca()
    {
        var chamadas = 0;
        var cortado = Montar(p => p.Add(x => x.MinCharacters, 2), (t, c) => { chamadas++; return Buscar(t, c); });

        cortado.Find("input").Focus();
        cortado.Find("input").Input("c");
        Assert.Equal(0, chamadas);
        Assert.Empty(cortado.FindAll("[role=listbox]"));

        cortado.Find("input").Input("ca");
        Assert.Equal(1, chamadas);
        Assert.Equal(3, cortado.FindAll("li[role=option]").Count);
    }

    [Fact]
    public void Mostra_no_maximo_MaxItems()
    {
        var cortado = Montar(p => p.Add(x => x.MaxItems, 2));

        cortado.Find("input").Input("");

        Assert.Equal(2, cortado.FindAll("li[role=option]").Count);
    }

    [Fact]
    public void Setas_andam_e_Enter_escolhe()
    {
        string? valor = null;
        var cortado = Montar(p => p
            .Add(x => x.Value, valor)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => valor = v)));

        cortado.Find("input").Input("ca");
        cortado.Find("input").KeyDown("ArrowDown");
        Assert.Equal("cidade-opcao-0", cortado.Find("input").GetAttribute("aria-activedescendant"));
        cortado.Find("input").KeyDown("ArrowDown");
        cortado.Find("input").KeyDown("ArrowDown");
        cortado.Find("input").KeyDown("ArrowDown");
        Assert.Equal("cidade-opcao-0", cortado.Find("input").GetAttribute("aria-activedescendant"));
        cortado.Find("input").KeyDown("ArrowUp");
        Assert.Equal("cidade-opcao-2", cortado.Find("input").GetAttribute("aria-activedescendant"));
        Assert.Equal("true", cortado.Find("#cidade-opcao-2").GetAttribute("aria-selected"));
        Assert.Contains("rvm-ativa", cortado.Find("#cidade-opcao-2").ClassList);

        cortado.Find("input").KeyDown("Enter");

        Assert.Equal("Cascavel", valor);
        Assert.Equal("Cascavel", cortado.Find("input").GetAttribute("value"));
        Assert.Equal("false", cortado.Find("input").GetAttribute("aria-expanded"));
        Assert.Empty(cortado.FindAll("[role=listbox]"));
    }

    [Fact]
    public void Clique_escolhe_e_o_texto_vem_do_ItemText()
    {
        var valor = (Cidade?)null;
        Cidade[] cidades = [new(1, "Campinas"), new(2, "Curitiba")];
        var cortado = Render<RvmAutocomplete<Cidade?>>(p => p
            .Add(x => x.DebounceInterval, 0)
            .Add(x => x.SearchFunc, (_, _) => Task.FromResult<IEnumerable<Cidade?>>(cidades))
            .Add(x => x.ItemText, c => c!.Nome)
            .Add(x => x.Value, valor)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<Cidade?>(this, v => valor = v))
            .Add(x => x.ValueExpression, () => valor));

        cortado.Find("input").Focus();
        Assert.Equal(["Campinas", "Curitiba"], cortado.FindAll("li").Select(l => l.TextContent.Trim()));
        cortado.FindAll("li")[1].Click();

        Assert.Equal(2, valor?.Codigo);
        Assert.Equal("Curitiba", cortado.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Esc_fecha_e_devolve_o_texto_do_valor()
    {
        var cortado = Montar(p => p.Add(x => x.Value, "Londrina"));
        Assert.Equal("Londrina", cortado.Find("input").GetAttribute("value"));

        cortado.Find("input").Input("ca");
        Assert.NotNull(cortado.Find("[role=listbox]"));

        cortado.Find("input").KeyDown("Escape");

        Assert.Empty(cortado.FindAll("[role=listbox]"));
        Assert.Equal("false", cortado.Find("input").GetAttribute("aria-expanded"));
        Assert.Equal("Londrina", cortado.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Sair_do_campo_sem_escolher_fecha_e_devolve_o_texto()
    {
        var cortado = Montar(p => p.Add(x => x.Value, "Londrina"));

        cortado.Find("input").Input("ca");
        cortado.Find("input").Blur();

        Assert.Empty(cortado.FindAll("[role=listbox]"));
        Assert.Equal("Londrina", cortado.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Valor_mudado_por_fora_atualiza_o_texto()
    {
        var cortado = Montar(p => p.Add(x => x.Value, "Londrina"));

        cortado.Render(p => p.Add(x => x.Value, "Curitiba"));

        Assert.Equal("Curitiba", cortado.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Busca_nova_cancela_a_anterior_e_o_resultado_velho_e_ignorado()
    {
        var pendentes = new List<(string Termo, CancellationToken Token, TaskCompletionSource<IEnumerable<string?>> Fonte)>();
        var cortado = Montar(busca: (termo, token) =>
        {
            var fonte = new TaskCompletionSource<IEnumerable<string?>>();
            pendentes.Add((termo, token, fonte));
            return fonte.Task;
        });

        cortado.Find("input").Input("c");
        cortado.Find("input").Input("cu");

        Assert.Equal(2, pendentes.Count);
        Assert.True(pendentes[0].Token.IsCancellationRequested);
        Assert.False(pendentes[1].Token.IsCancellationRequested);

        cortado.InvokeAsync(() => pendentes[1].Fonte.SetResult(["Curitiba"]));
        cortado.InvokeAsync(() => pendentes[0].Fonte.SetResult(["Campinas", "Cascavel"]));

        cortado.WaitForAssertion(() => Assert.Equal(["Curitiba"], cortado.FindAll("li").Select(l => l.TextContent.Trim())));
    }

    [Fact]
    public void Debounce_espera_o_intervalo_e_so_a_ultima_tecla_busca()
    {
        var termos = new List<string>();
        var cortado = Montar(busca: (t, c) => { termos.Add(t); return Buscar(t, c); }, espera: 300);

        cortado.Find("input").Input("c");
        _relogio.Advance(TimeSpan.FromMilliseconds(200));
        cortado.Find("input").Input("ca");
        _relogio.Advance(TimeSpan.FromMilliseconds(200));
        Assert.Empty(termos);

        _relogio.Advance(TimeSpan.FromMilliseconds(100));

        cortado.WaitForAssertion(() => Assert.Equal(3, cortado.FindAll("li").Count));
        Assert.Equal(["ca"], termos);
    }

    [Fact]
    public void Buscando_e_anunciado_na_regiao_viva_fora_da_lista()
    {
        var fonte = new TaskCompletionSource<IEnumerable<string?>>();
        var cortado = Montar(busca: (_, _) => fonte.Task);

        cortado.Find("input").Input("ca");

        Assert.Equal("Buscando...", cortado.Find("[role=status]").TextContent);
        Assert.Equal("Buscando...", cortado.Find(".rvm-mensagem").TextContent);
        Assert.Empty(cortado.FindAll("[role=listbox]"));
        Assert.Equal("false", cortado.Find("input").GetAttribute("aria-expanded"));

        cortado.InvokeAsync(() => fonte.SetResult(["Campinas"]));
        cortado.WaitForAssertion(() => Assert.Single(cortado.FindAll("li[role=option]")));
        Assert.Equal("", cortado.Find("[role=status]").TextContent);
    }

    [Fact]
    public void Sem_resultado_e_anunciado()
    {
        var cortado = Montar(p => p.Add(x => x.NoResultsText, "Nada por aqui."));

        cortado.Find("input").Input("xyz");

        Assert.Equal("Nada por aqui.", cortado.Find("[role=status]").TextContent);
        Assert.Empty(cortado.FindAll("[role=listbox]"));
    }

    [Fact]
    public void Erro_da_busca_e_anunciado_sem_derrubar_o_campo()
    {
        var cortado = Montar(busca: (_, _) => throw new HttpRequestException("caiu"));

        cortado.Find("input").Input("ca");

        Assert.Equal("Nao deu para buscar agora. Tente de novo em alguns minutos.", cortado.Find("[role=status]").TextContent);
        Assert.Empty(cortado.FindAll("[role=listbox]"));
    }

    [Fact]
    public void Clearable_limpa_valor_e_texto()
    {
        string? valor = "Londrina";
        var cortado = Montar(p => p
            .Add(x => x.Clearable, true)
            .Add(x => x.ClearLabel, "Limpar cidade")
            .Add(x => x.Value, valor)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => valor = v)));

        var limpar = cortado.Find("button.rvm-limpar");
        Assert.Equal("Limpar cidade", limpar.GetAttribute("aria-label"));
        Assert.Equal("button", limpar.GetAttribute("type"));

        limpar.Click();

        Assert.Null(valor);
        Assert.Equal("", cortado.Find("input").GetAttribute("value"));
        Assert.Empty(cortado.FindAll("button.rvm-limpar"));
    }

    [Fact]
    public void Sem_Clearable_nao_ha_botao_e_o_rotulo_padrao_e_Limpar()
    {
        Assert.Empty(Montar(p => p.Add(x => x.Value, "Londrina")).FindAll("button.rvm-limpar"));
        Assert.Equal("Limpar", Montar(p => p.Add(x => x.Value, "Londrina").Add(x => x.Clearable, true))
            .Find("button.rvm-limpar").GetAttribute("aria-label"));
    }

    [Fact]
    public void Apagar_o_texto_limpa_o_valor_com_ResetValueOnEmptyText()
    {
        string? valor = "Londrina";
        var cortado = Montar(p => p
            .Add(x => x.Value, valor)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => valor = v)));

        cortado.Find("input").Input("");

        Assert.Null(valor);
    }

    [Fact]
    public void Sem_ResetValueOnEmptyText_apagar_o_texto_mantem_o_valor()
    {
        string? valor = "Londrina";
        var cortado = Montar(p => p
            .Add(x => x.ResetValueOnEmptyText, false)
            .Add(x => x.Value, valor)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => valor = v)));

        cortado.Find("input").Input("");

        Assert.Equal("Londrina", valor);
    }

    [Fact]
    public void Campo_tem_moldura_nome_classe_apoio_e_erro()
    {
        var cortado = Montar(p => p
            .Add(x => x.Name, "endereco.cidade")
            .Add(x => x.Class, "minha")
            .Add(x => x.Placeholder, "Digite a cidade")
            .Add(x => x.ErrorText, "Escolha a cidade."));

        var input = cortado.Find("input");
        Assert.Equal("cidade", input.Id);
        Assert.Equal("endereco.cidade", input.GetAttribute("name"));
        Assert.Equal("Digite a cidade", input.GetAttribute("placeholder"));
        Assert.Equal("true", input.GetAttribute("aria-invalid"));
        Assert.Equal("cidade-erro", input.GetAttribute("aria-describedby"));
        Assert.Equal("cidade", cortado.Find("label").GetAttribute("for"));
        Assert.Contains("minha", cortado.Find(".rvm-campo").ClassList);
        Assert.Contains("rvm-entrada", input.ClassList);
    }

    [Fact]
    public void Desabilitado_nao_mostra_o_limpar()
    {
        var cortado = Montar(p => p.Add(x => x.Disabled, true).Add(x => x.Clearable, true).Add(x => x.Value, "Londrina"));

        Assert.True(cortado.Find("input").HasAttribute("disabled"));
        Assert.Empty(cortado.FindAll("button.rvm-limpar"));
    }

    public sealed record Cidade(int Codigo, string Nome);
}

/// <summary>O RvmOptionList do contrato com o RVM.UI (DSGN-017): listbox que recebe o foco.</summary>
public class RvmOptionListTests : BunitContext
{
    private static readonly string[] Culturas = ["Soja", "Milho", "Cafe", "Trigo"];

    public RvmOptionListTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Listbox_com_rotulo_foco_e_aria_selected()
    {
        var cortado = Render<RvmOptionList<string>>(p => p
            .Add(x => x.Items, Culturas)
            .Add(x => x.Id, "culturas")
            .Add(x => x.Class, "minha")
            .Add(x => x.LabelledBy, "rotulo")
            .Add(x => x.IsSelected, c => c == "Milho"));

        var lista = cortado.Find("ul");
        Assert.Equal("listbox", lista.GetAttribute("role"));
        Assert.Equal("0", lista.GetAttribute("tabindex"));
        Assert.Equal("rotulo", lista.GetAttribute("aria-labelledby"));
        Assert.Null(lista.GetAttribute("aria-multiselectable"));
        Assert.Contains("minha", lista.ClassList);
        Assert.Contains("rvm-opcoes", lista.ClassList);
        var opcoes = cortado.FindAll("li[role=option]");
        Assert.Equal(["false", "true", "false", "false"], opcoes.Select(o => o.GetAttribute("aria-selected")));
        Assert.Equal("culturas-opcao-1", opcoes[1].Id);
        Assert.Contains("rvm-selecionada", opcoes[1].ClassList);
    }

    [Fact]
    public void Multiple_marca_aria_multiselectable_e_a_caixinha()
    {
        var cortado = Render<RvmOptionList<string>>(p => p
            .Add(x => x.Items, Culturas)
            .Add(x => x.Multiple, true)
            .Add(x => x.IsSelected, c => c is "Soja" or "Cafe"));

        Assert.Equal("true", cortado.Find("ul").GetAttribute("aria-multiselectable"));
        Assert.Equal(4, cortado.FindAll(".rvm-marca").Count);
        Assert.Equal(2, cortado.FindAll(".rvm-marca svg").Count);
    }

    [Fact]
    public void Foco_ativa_a_escolhida_e_setas_pulam_a_desabilitada()
    {
        var cortado = Render<RvmOptionList<string>>(p => p
            .Add(x => x.Items, Culturas)
            .Add(x => x.Id, "c")
            .Add(x => x.IsSelected, c => c == "Soja")
            .Add(x => x.ItemDisabled, c => c is "Milho" or "Trigo"));

        Assert.Null(cortado.Find("ul").GetAttribute("aria-activedescendant"));
        cortado.Find("ul").Focus();
        Assert.Equal("c-opcao-0", cortado.Find("ul").GetAttribute("aria-activedescendant"));

        cortado.Find("ul").KeyDown("ArrowDown");
        Assert.Equal("c-opcao-2", cortado.Find("ul").GetAttribute("aria-activedescendant"));
        Assert.Contains("rvm-ativa", cortado.Find("#c-opcao-2").ClassList);

        cortado.Find("ul").KeyDown("ArrowDown");
        Assert.Equal("c-opcao-2", cortado.Find("ul").GetAttribute("aria-activedescendant"));

        cortado.Find("ul").KeyDown("ArrowUp");
        Assert.Equal("c-opcao-0", cortado.Find("ul").GetAttribute("aria-activedescendant"));

        cortado.Find("ul").KeyDown("End");
        Assert.Equal("c-opcao-2", cortado.Find("ul").GetAttribute("aria-activedescendant"));
        cortado.Find("ul").KeyDown("Home");
        Assert.Equal("c-opcao-0", cortado.Find("ul").GetAttribute("aria-activedescendant"));
        Assert.Equal("true", cortado.Find("#c-opcao-1").GetAttribute("aria-disabled"));
    }

    [Fact]
    public void Enter_e_espaco_disparam_OnPick_com_a_ativa()
    {
        var escolhidas = new List<string>();
        var cortado = Render<RvmOptionList<string>>(p => p
            .Add(x => x.Items, Culturas)
            .Add(x => x.OnPick, EventCallback.Factory.Create<string>(this, escolhidas.Add)));

        cortado.Find("ul").Focus();
        cortado.Find("ul").KeyDown("ArrowDown");
        cortado.Find("ul").KeyDown("Enter");
        cortado.Find("ul").KeyDown("ArrowDown");
        cortado.Find("ul").KeyDown(" ");

        Assert.Equal(["Milho", "Cafe"], escolhidas);
    }

    [Fact]
    public void Clique_escolhe_e_desabilitada_nao()
    {
        var escolhidas = new List<string>();
        var cortado = Render<RvmOptionList<string>>(p => p
            .Add(x => x.Items, Culturas)
            .Add(x => x.ItemDisabled, c => c == "Milho")
            .Add(x => x.OnPick, EventCallback.Factory.Create<string>(this, escolhidas.Add)));

        cortado.FindAll("li")[1].Click();
        cortado.FindAll("li")[3].Click();

        Assert.Equal(["Trigo"], escolhidas);
    }

    [Theory]
    [InlineData("Escape")]
    [InlineData("Tab")]
    public void Esc_e_Tab_pedem_OnClose(string tecla)
    {
        var fechou = 0;
        var cortado = Render<RvmOptionList<string>>(p => p
            .Add(x => x.Items, Culturas)
            .Add(x => x.OnClose, EventCallback.Factory.Create(this, () => fechou++)));

        cortado.Find("ul").KeyDown(tecla);

        Assert.Equal(1, fechou);
    }

    [Fact]
    public void ItemImage_poe_a_imagem_decorativa_antes_do_texto()
    {
        var cortado = Render<RvmOptionList<string>>(p => p
            .Add(x => x.Items, Culturas)
            .Add(x => x.ItemText, c => c.ToUpperInvariant())
            .Add(x => x.ItemImage, c => c == "Cafe" ? "" : $"/img/{c}.png"));

        var imagens = cortado.FindAll("img.rvm-imagem-opcao");
        Assert.Equal(3, imagens.Count);
        Assert.Equal("/img/Soja.png", imagens[0].GetAttribute("src"));
        Assert.Equal("", imagens[0].GetAttribute("alt"));
        Assert.Equal("SOJA", cortado.Find("li .rvm-texto-opcao").TextContent);
        Assert.Equal("IMG", cortado.Find("li").FirstElementChild!.TagName);
    }

    [Fact]
    public void AutoFocus_foca_a_lista_com_a_escolhida_ativa()
    {
        var cortado = Render<RvmOptionList<string>>(p => p
            .Add(x => x.Items, Culturas)
            .Add(x => x.Id, "c")
            .Add(x => x.AutoFocus, true)
            .Add(x => x.IsSelected, c => c == "Cafe"));

        Assert.Equal("c-opcao-2", cortado.Find("ul").GetAttribute("aria-activedescendant"));
        Assert.Contains(JSInterop.Invocations, i => i.Identifier == "Blazor._internal.domWrapper.focus");
    }

    [Fact]
    public void AutoFocus_sem_escolhida_ativa_a_primeira()
    {
        var cortado = Render<RvmOptionList<string>>(p => p
            .Add(x => x.Items, Culturas)
            .Add(x => x.Id, "c")
            .Add(x => x.AutoFocus, true));

        Assert.Equal("c-opcao-0", cortado.Find("ul").GetAttribute("aria-activedescendant"));
    }
}
