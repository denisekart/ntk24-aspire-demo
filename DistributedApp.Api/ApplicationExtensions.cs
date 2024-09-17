using DistributedApp.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace DistributedApp.Api;

public static class ApplicationExtensions
{
    public static IHostApplicationBuilder ConfigureApplication(this IHostApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
        builder.AddSqlServerDbContext<ApplicationDbContext>("db");
        builder.Services.AddSingleton<DevelopmentTimeDatabaseInitializer>();
        
        return builder;
    }

    static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public static IEndpointRouteBuilder MapApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/weatherforecast",
                async (ApplicationDbContext db, ILogger<Program> logger) =>
                {
                    var lastValidForecast = DateTimeOffset.Now.AddSeconds(-10);
                    var forecast = await db.WeatherHistories
                        .Where(x => x.CreatedOn >= lastValidForecast)
                        .OrderByDescending(x => x.CreatedOn)
                        .Select(x => x.Forecasts)
                        .FirstOrDefaultAsync();

                    if (forecast is not null)
                    {
                        logger.LogInformation("Returning forecast from DB...");
                        return forecast;
                    }

                    forecast = Enumerable.Range(1, 5).Select(index =>
                            new WeatherForecast(
                                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                Random.Shared.Next(-20, 55),
                                Summaries[Random.Shared.Next(Summaries.Length)]
                            ))
                        .ToList();

                    db.WeatherHistories.Add(new WeatherHistory()
                    {
                        CreatedOn = DateTimeOffset.Now,
                        Forecasts = forecast
                    });
                    await db.SaveChangesAsync();

                    logger.LogInformation("Returning forecast from APP...");
                    return forecast;
                })
            .WithName("GetWeatherForecast")
            .WithOpenApi();

        return endpoints;
    }
}