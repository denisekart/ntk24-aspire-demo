using DistributedApp.Api;
using DistributedApp.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureApplication();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<RandomlyFailingMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMiddleware<DevelopmentDatabaseInitializerMiddleware>();
}

app.UseHttpsRedirection();

app.FailRandomly();

app.MapApi();

app.Run();