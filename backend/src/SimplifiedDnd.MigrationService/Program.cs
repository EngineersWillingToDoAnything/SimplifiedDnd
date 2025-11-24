using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SimplifiedDnd.DataBase;
using SimplifiedDnd.MigrationService;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
  .ConfigureResource(resource => resource.AddService(Worker.ActivitySourceName))
  .WithTracing(tracing => tracing.AddOtlpExporter());

builder.AddDataBase();

IHost host = builder.Build();
await host.RunAsync();