namespace DistributedApp.Api.Data;

public class DevelopmentTimeDatabaseInitializer
{
    private bool _initializeDbCalled = false;
    private readonly object _lock = new object();

    // Ensure that the database is created (thread-safe)
    public void EnsureCreated(IServiceProvider serviceProvider)
    {
        if (!_initializeDbCalled)
        {
            lock (_lock)
            {
                if (!_initializeDbCalled)
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        dbContext.Database.EnsureCreated();
                    }
                    _initializeDbCalled = true;
                }
            }
        }
    }
}