namespace DistributedApp.Api;

public class WeatherForecast()
{
    public WeatherForecast(DateOnly date, int temperatureC, string? summary) : this()
    {
        Date = date;
        TemperatureC = temperatureC;
        Summary = summary;
    }

    public DateOnly Date { get; init; }
    public int TemperatureC { get; init; }
    public string? Summary { get; init; }
}