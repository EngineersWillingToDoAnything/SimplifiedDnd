using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimplifiedDnd.DataBase.Contexts;
using SimplifiedDnd.DataBase.Entities;

namespace SimplifiedDnd.Database.IntegrationTests.Abstractions;

#pragma warning disable CA1515
// ReSharper disable once ClassNeverInstantiated.Global
public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime {
#pragma warning restore CA1515
  private readonly PostgreSqlService _dbService = new();

  protected override IHost CreateHost(IHostBuilder builder) {
    Debug.Assert(builder is not null);

    builder.ConfigureHostConfiguration(config =>
      _dbService.AddHostConfiguration(config));

    return base.CreateHost(builder);
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder) {
    builder.ConfigureTestServices(services => _dbService.Configure(services));
  }

  public async ValueTask InitializeAsync() {
    await _dbService.InitializeAsync();
  }

  public new async Task DisposeAsync() {
    await _dbService.DisposeAsync();
  }
}