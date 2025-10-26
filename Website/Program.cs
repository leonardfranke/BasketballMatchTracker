using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Website;
using Website.Services.Matches;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

using var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var configFile = $"appsettings.json";
using var response = await http.GetAsync(configFile);
using var stream = await response.Content.ReadAsStreamAsync();
var config = new ConfigurationBuilder()
    .AddJsonStream(stream)
    .Build();
builder.Configuration.AddConfiguration(config);

builder.Services.AddHttpClient("BACKEND", httpClient => httpClient.BaseAddress = new Uri(builder.Configuration["BACKEND_ADDRESS"]));
builder.Services.AddSingleton<IMatchService, MatchService>();

builder.Services.AddMudServices();
await builder.Build().RunAsync();
