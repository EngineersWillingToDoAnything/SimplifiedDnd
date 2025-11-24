using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SimplifiedDnd.Application;
using SimplifiedDnd.DataBase;
using SimplifiedDnd.WebApi.Abstractions;
using SimplifiedDnd.WebApi.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.AddDataBase();

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpoints(typeof(Program).Assembly);

builder.Services.AddOpenTelemetry()
  .ConfigureResource(resource => resource.AddService("SimplifiedDndApi"))
  .WithMetrics(metrics => {
    metrics.AddAspNetCoreInstrumentation()
      .AddHttpClientInstrumentation();

    metrics.AddOtlpExporter();
  })
  .WithTracing(tracing => {
    tracing.AddAspNetCoreInstrumentation()
      .AddHttpClientInstrumentation();

    tracing.AddOtlpExporter();
  });

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
  app.MapOpenApi();
} else {
  app.UseHttpsRedirection();
}

app.UseExceptionHandler();
app.MapEndpoints();

await app.RunAsync();

#pragma warning disable CA1515
public partial class Program {
#pragma warning restore CA1515
  protected Program() { }
}