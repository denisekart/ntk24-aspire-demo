using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DistributedApp.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<WeatherHistory> WeatherHistories { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var weatherForecastConverter = new ValueConverter<List<WeatherForecast>, string>(
            v => JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = false }),
            v => JsonSerializer.Deserialize<List<WeatherForecast>>(v, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<WeatherForecast>());

        modelBuilder.Entity<WeatherHistory>()
            .HasKey(x => x.Id);
        modelBuilder.Entity<WeatherHistory>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<WeatherHistory>()
            .Property(x => x.Forecasts)
            .HasConversion(weatherForecastConverter);
    }
}