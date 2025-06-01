using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.DataBase.Repositories;

internal sealed class FakeSpecieRepository : ISpecieRepository {
  public Task<Specie?> GetSpecieAsync(string name, CancellationToken cancellationToken) {
    return Task.FromResult<Specie?>(null);
  }
}