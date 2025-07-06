using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SimplifiedDnd.DataBase.Contexts;
using SimplifiedDnd.DataBase.Entities;
using System.Diagnostics;

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
      new ClassDbEntity { Id = 1, Name = "Artificer", },
      new ClassDbEntity { Id = 2, Name = "Barbarian", },
      new ClassDbEntity { Id = 3, Name = "Bard", });

    DbContext.Species.AddRange(
      new SpecieDbEntity { Id = 1, Name = "Dragonborn", Speed = 0 },
      new SpecieDbEntity { Id = 2, Name = "Elf", Speed = 0 },
      new SpecieDbEntity { Id = 3, Name = "Dwarf", Speed = 0 });

    var characterId = Guid.CreateVersion7();
    DbContext.Characters.AddRange(
      new CharacterDbEntity {
        Id = characterId, Name = "Test1", PlayerName = "...", SpecieId = 1
      },
      new CharacterDbEntity {
        Id = Guid.CreateVersion7(), Name = "Bruce", PlayerName = "...", SpecieId = 2
      },
      new CharacterDbEntity {
        Id = Guid.CreateVersion7(), Name = "Peter", PlayerName = "...", SpecieId = 2
      },
      new CharacterDbEntity {
        Id = Guid.CreateVersion7(), Name = "Link", PlayerName = "...", SpecieId = 3
      },
      new CharacterDbEntity {
        Id = Guid.CreateVersion7(), Name = "Sonic", PlayerName = "...", SpecieId = 1
      },
      new CharacterDbEntity {
        Id = Guid.CreateVersion7(), Name = "Mario", PlayerName = "...", SpecieId = 3
      });
    DbContext.CharacterClasses.Add(new CharacterClassDbEntity {
        CharacterId = characterId, ClassId = 1, IsMainClass = true
      }
    );
    await DbContext.SaveChangesAsync();
    _databaseSeeded = true;
  }
}