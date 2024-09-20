using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace DistributedApp.Web;

public static class ApplicationExtensions
{
    public static IHostApplicationBuilder ConfigureApplication(this IHostApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
        builder.AddRedisDistributedCache("cache");
        builder.Services.AddHttpClient("api", client =>
        {
            client.BaseAddress = new Uri("http://api");
        });
        
        return builder;
    }
    public static IEndpointRouteBuilder MapApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/weatherforecast",
                async (IDistributedCache cache, IHttpClientFactory httpClientFactory) =>
                {
                    var cachedEntry = await cache.GetStringAsync("weather");
                    if(cachedEntry is not null && JsonSerializer.Deserialize<WeatherForecast[]>(cachedEntry) is {} weatherForecast)
                    {
                        return weatherForecast;
                    }

                    var http = httpClientFactory.CreateClient("api");
                    weatherForecast = await http.GetFromJsonAsync<WeatherForecast[]>("weatherforecast");
                    await cache.SetStringAsync("weather", JsonSerializer.Serialize(weatherForecast), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
                    });
            
                    return weatherForecast;
                })
            .WithName("GetWeatherForecast");

        return endpoints;
    }
}

public class WeatherService(IDistributedCache cache, IHttpClientFactory httpClientFactory)
{
    public async Task<WeatherForecast[]> GetWeatherForecasts()
    {
        var cachedEntry = await cache.GetStringAsync("weather");
        if(cachedEntry is not null && JsonSerializer.Deserialize<WeatherForecast[]>(cachedEntry) is {} weatherForecast)
        {
            return weatherForecast;
        }

        var http = httpClientFactory.CreateClient("api");
        weatherForecast = await http.GetFromJsonAsync<WeatherForecast[]>("weatherforecast");
        await cache.SetStringAsync("weather", JsonSerializer.Serialize(weatherForecast), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
        });
            
        return weatherForecast;
    }
}