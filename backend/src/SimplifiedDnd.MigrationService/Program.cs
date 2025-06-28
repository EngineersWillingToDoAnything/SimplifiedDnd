using SimplifiedDnd.DataBase;
using SimplifiedDnd.MigrationService;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
  .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddDataBase();

IHost host = builder.Build();
await host.RunAsync();
