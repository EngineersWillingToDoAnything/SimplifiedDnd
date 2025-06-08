IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("postgres")
  .WithPgAdmin();

IResourceBuilder<PostgresDatabaseResource> simplifiedDndDb = postgres.AddDatabase("simplifiedDndDb");

builder.AddProject<Projects.SimplifiedDnd_WebApi>("api")
  .WithReference(simplifiedDndDb)
  .WaitFor(simplifiedDndDb);

await builder.Build().RunAsync();