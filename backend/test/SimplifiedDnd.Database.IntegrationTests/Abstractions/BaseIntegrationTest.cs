using Microsoft.Extensions.DependencyInjection;
using SimplifiedDnd.DataBase.Contexts;
using System.Diagnostics;

namespace SimplifiedDnd.Database.IntegrationTests.Abstractions;

#pragma warning disable S3881, CA1515, CA1063
public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable {
#pragma warning restore S3881, CA1515, CA1063
  private readonly IServiceScope _scope;
  internal MainDbContext DbContext { get; }

  protected BaseIntegrationTest(IntegrationTestWebAppFactory factory) {
    Debug.Assert(factory is not null);

    _scope = factory.Services.CreateScope();
    DbContext = _scope.ServiceProvider
      .GetRequiredService<MainDbContext>();
    DbContext.Database.EnsureCreated();
  }

  protected TImplementation GetRequiredService<TAbstraction, TImplementation>()
    where TImplementation : class, TAbstraction
    where TAbstraction : notnull {
    return (_scope.ServiceProvider.GetRequiredService<TAbstraction>() as TImplementation)!;
  }

#pragma warning disable CA1816, CA1063
  public void Dispose() {
#pragma warning restore CA1816, CA1063
    _scope.Dispose();
    DbContext.Dispose();
  }
}