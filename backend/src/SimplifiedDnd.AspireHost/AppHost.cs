IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresDatabaseResource> db = builder
  .AddPostgres("postgres")
  .AddDatabase("mainDb");

IResourceBuilder<ProjectResource> api = builder.AddProject<Projects.SimplifiedDnd_WebApi>("api")
  .WithReference(db)
  .WaitFor(db);

builder.AddNpmApp("discordBot", "../../../frontend/SimplifiedDnd.DiscordBot")
  .WithReference(api);

builder.AddProject<Projects.SimplifiedDnd_MigrationService>("migrations")
  .WithReference(db)
  .WaitFor(db)
  .WithExplicitStart();

await builder.Build().RunAsync();
