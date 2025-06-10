IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresDatabaseResource> simplifiedDndDb = builder
  .AddPostgres("postgres")
  .WithPgAdmin()
  .AddDatabase("simplifiedDndDb");

builder.AddProject<Projects.SimplifiedDnd_WebApi>("api")
  .WithReference(simplifiedDndDb)
  .WaitFor(simplifiedDndDb);

builder.AddProject<Projects.SimplifiedDnd_MigrationService>("migrations")
  .WithReference(simplifiedDndDb)
  .WaitFor(simplifiedDndDb);

await builder.Build().RunAsync();