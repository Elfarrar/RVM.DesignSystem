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

await builder.Build().RunAsync();
