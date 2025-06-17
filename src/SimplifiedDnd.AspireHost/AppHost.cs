IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresDatabaseResource> db = builder
  .AddPostgres("postgres")
  .AddDatabase("simplifiedDndDb");

builder.AddProject<Projects.SimplifiedDnd_WebApi>("api")
  .WithReference(db)
  .WaitFor(db);

builder.AddProject<Projects.SimplifiedDnd_MigrationService>("migrations")
  .WithReference(db)
  .WaitFor(db);

await builder.Build().RunAsync();