using System.Net.Http.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Projects;

namespace DistributedApp.Tests;

public class WebIntegrationTests
{
    [Fact]
    public async Task GetWeatherForecastSucceeds()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<DistributedApp_AppHost>();
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
        await using var app = await appHost.BuildAsync();
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync();

        // Act
        var httpClient = app.CreateHttpClient("web");
        await resourceNotificationService.WaitForResourceAsync("web", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.GetAsync("/weatherforecast");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.Should().BeOfType<JsonArray>(because: "expected array of weather forecasts")
            .Which.Should().HaveCount(5, because: "expected 5 items in the array");
    }
}
