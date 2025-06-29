using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SimplifiedDnd.DataBase.Contexts;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SimplifiedDnd.DataBase.Entities;

namespace SimplifiedDnd.Database.IntegrationTests.Abstractions;

#pragma warning disable S3881, CA1515, CA1063
public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime {
#pragma warning restore S3881, CA1515, CA1063
  private readonly IServiceScope _scope;
  protected ISender Sender { get; }
  internal MainDbContext DbContext { get; }

  protected BaseIntegrationTest(IntegrationTestWebAppFactory factory) {
    Debug.Assert(factory is not null);

    _scope = factory.Services.CreateScope();
    Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
    DbContext = _scope.ServiceProvider
      .GetRequiredService<MainDbContext>();
  }

#pragma warning disable CA1816
  public async ValueTask DisposeAsync() {
#pragma warning restore CA1816
    _scope.Dispose();
    await DbContext.DisposeAsync();
  }

  private static bool _databaseSeeded;

  public async ValueTask InitializeAsync() {
    if (_databaseSeeded) { return; }
    
    DbContext.Classes.AddRange(
      new ClassDbEntity { Name = "Artificer", },
      new ClassDbEntity { Name = "Barbarian", },
      new ClassDbEntity { Name = "Bard", });
    DbContext.Species.Add(new SpecieDbEntity {
      Name = "Dragonborn", Speed = 0
    });
    await DbContext.SaveChangesAsync();
    _databaseSeeded = true;
  }
}