using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RVM.DesignSystem;
using RVM.DesignSystem.Docs;
using RVM.DesignSystem.Docs.Documentacao;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Uma chamada registra a biblioteca inteira — sem AddRvmButton(), AddRvmTextField()...
builder.Services.AddRvmDesignSystem();

// Le a doc XML da biblioteca uma vez e alimenta a tabela de API de cada pagina.
builder.Services.AddScoped<ApiDoc>();

// Indice da busca (RF-27). Montado do que ja existe — navegacao, tema e rvm-tokens.css —,
// nunca de uma lista paralela que desatualizaria no primeiro componente novo.
builder.Services.AddScoped<Busca>();

// A paleta que o visitante monta em /fundamentos/paleta, guardada no navegador dele.
// E do SITE, nao da biblioteca: tema por usuario final em runtime segue fora do escopo da v1.
builder.Services.AddScoped<PaletaDoVisitante>();

await builder.Build().RunAsync();
