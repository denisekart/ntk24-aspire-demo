namespace DistributedApp.Api.Data;

public class WeatherHistory
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
    public List<WeatherForecast> Forecasts { get; set; }
}