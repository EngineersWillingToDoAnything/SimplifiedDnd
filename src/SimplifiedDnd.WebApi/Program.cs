using SimplifiedDnd.Application;
using SimplifiedDnd.DataBase;
using SimplifiedDnd.WebApi.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.AddDataBase();

builder.Services.AddOpenApi();

builder.Services.AddEndpoints(typeof(Program).Assembly);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
  app.MapOpenApi();
  app.CreateDbIfNotExists();
}

app.UseHttpsRedirection();
app.MapEndpoints();

await app.RunAsync();

#pragma warning disable CA1515
public partial class Program {
#pragma warning restore CA1515
  protected Program() {
  }
}
