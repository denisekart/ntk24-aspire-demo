using System.Net.Http.Json;
using DistributedApp.Web.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<GetWeatherForecastsDelegate>(services =>
    () => services.GetRequiredService<HttpClient>()
        .GetFromJsonAsync<WeatherForecast[]>("weatherforecast"));
await builder.Build().RunAsync();
