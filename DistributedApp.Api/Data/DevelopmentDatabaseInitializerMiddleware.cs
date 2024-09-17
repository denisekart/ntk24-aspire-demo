namespace DistributedApp.Api.Data;

public class DevelopmentDatabaseInitializerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly DevelopmentTimeDatabaseInitializer _databaseInitializer;

    public DevelopmentDatabaseInitializerMiddleware(RequestDelegate next, DevelopmentTimeDatabaseInitializer databaseInitializer)
    {
        _next = next;
        _databaseInitializer = databaseInitializer;
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        _databaseInitializer.EnsureCreated(serviceProvider);
        await _next(context);
    }
}