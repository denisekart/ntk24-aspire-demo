using DistributedApp.Web;
using DistributedApp.Web.Client;
using DistributedApp.Web.Client.Pages;
using DistributedApp.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureApplication();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<GetWeatherForecastsDelegate>(services => async () =>
    (await services.GetRequiredService<WeatherService>()
        .GetWeatherForecasts())
    .Select(w => new DistributedApp.Web.Client.WeatherForecast()
    {
        Date = w.Date,
        Summary = w.Summary,
        TemperatureC = w.TemperatureC,
    }).ToArray());

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(DistributedApp.Web.Client._Imports).Assembly);

app.MapApi();

app.Run();
