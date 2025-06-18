using FluentValidation;
using SimplifiedDnd.Application;
using SimplifiedDnd.DataBase;
using SimplifiedDnd.WebApi.Abstractions;
using SimplifiedDnd.WebApi.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.AddDataBase();

builder.Services.AddOpenApi();

builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpoints(typeof(Program).Assembly);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
  app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapEndpoints();

await app.RunAsync();

#pragma warning disable CA1515
public partial class Program {
#pragma warning restore CA1515
  /// <summary>
/// Initializes a new instance of the <see cref="Program"/> class.
/// </summary>
protected Program() { }
}