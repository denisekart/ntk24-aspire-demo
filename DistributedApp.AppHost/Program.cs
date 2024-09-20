var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var database = builder
    .AddSqlServer("sql")
    .AddDatabase("db");

var api = builder.AddProject<Projects.DistributedApp_Api>("api")
    .WithReference(database)
    .WithExternalHttpEndpoints()
    .WithReplicas(2);

var web = builder.AddProject<Projects.DistributedApp_Web>("web")
    .WithReference(api)
    .WithReference(cache)
    .WithExternalHttpEndpoints();

builder.Build().Run();
