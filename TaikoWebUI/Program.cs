using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});
builder.Services.AddMudServices();
builder.Services.AddSingleton<IGameDataService, GameDataService>();

var host = builder.Build();

var gameDataService = host.Services.GetRequiredService<IGameDataService>();
await gameDataService.InitializeAsync(builder.HostEnvironment.BaseAddress);

await host.RunAsync();