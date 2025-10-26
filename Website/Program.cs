using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Website;
using Website.Services.Matches;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<IMatchService, MatchService>();
builder.Services.AddHttpClient("BACKEND", httpClient => httpClient.BaseAddress = new Uri("https://localhost:7230/api/"));

builder.Services.AddMudServices();
await builder.Build().RunAsync();
