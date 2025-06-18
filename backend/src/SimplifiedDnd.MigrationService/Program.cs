using SimplifiedDnd.DataBase.Contexts;
using SimplifiedDnd.MigrationService;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
  .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<MainDbContext>("simplifiedDndDb");

IHost host = builder.Build();
await host.RunAsync();