using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimplifiedDnd.DataBase.Contexts;
using Testcontainers.PostgreSql;

namespace SimplifiedDnd.Database.IntegrationTests.Abstractions;

internal class PostgreSqlService : IInfrastructureService {
  private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
    .WithImage("postgres:16.9-bullseye")
    .Build();

  public void AddHostConfiguration(IConfigurationBuilder config) {
    Debug.Assert(config is not null);

    config.AddInMemoryCollection(new Dictionary<string, string?> {
      ["ConnectionStrings:mainDb"] = _dbContainer.GetConnectionString(),
    });
  }

  public void Configure(IServiceCollection services) {
    Debug.Assert(services is not null);
    
    Type descriptorType = typeof(DbContextOptions<MainDbContext>);

    ServiceDescriptor? descriptor = services
      .SingleOrDefault(s => s.ServiceType == descriptorType);

    if (descriptor is not null) {
      services.Remove(descriptor);
    }

    services.AddDbContextPool<MainDbContext>(options =>
      options.UseNpgsql(_dbContainer.GetConnectionString()));
  }

  public async ValueTask InitializeAsync() {
    await _dbContainer.StartAsync();
  }

  public async ValueTask DisposeAsync() {
    await _dbContainer.DisposeAsync();
  }
}