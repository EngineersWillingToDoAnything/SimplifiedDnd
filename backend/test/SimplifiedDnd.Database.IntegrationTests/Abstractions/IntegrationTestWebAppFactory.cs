using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimplifiedDnd.DataBase.Contexts;
using Testcontainers.PostgreSql;

namespace SimplifiedDnd.Database.IntegrationTests.Abstractions;

#pragma warning disable CA1515
// ReSharper disable once ClassNeverInstantiated.Global
public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime {
#pragma warning restore CA1515
  private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
    .WithImage("postgres:16.9-bullseye")
    .Build();

  protected override IHost CreateHost(IHostBuilder builder) {
    Debug.Assert(builder is not null);

    builder.ConfigureHostConfiguration(config =>
      config.AddInMemoryCollection(new Dictionary<string, string?> {
        ["ConnectionStrings:mainDb"] = _dbContainer.GetConnectionString(),
      }));

    IHost host = base.CreateHost(builder);

    using IServiceScope scope = host.Services.CreateScope();
    MainDbContext db = scope.ServiceProvider.GetRequiredService<MainDbContext>();
    db.Database.Migrate();

    return host;
  }
  
  protected override void ConfigureWebHost(IWebHostBuilder builder) {
    builder.ConfigureTestServices(services => {
      Type descriptorType = typeof(DbContextOptions<MainDbContext>);

      ServiceDescriptor? descriptor = services
        .SingleOrDefault(s => s.ServiceType == descriptorType);

      if (descriptor is not null) {
        services.Remove(descriptor);
      }

      services.AddDbContextPool<MainDbContext>(options =>
        options.UseNpgsql(_dbContainer.GetConnectionString()));
    });
  }

  public async ValueTask InitializeAsync() {
    await _dbContainer.StartAsync();
  }

  public new Task DisposeAsync() {
    return _dbContainer.DisposeAsync().AsTask();
  }
}