using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.DataBase.Repositories;

internal sealed class FakeClassRepository : IClassRepository {
  public Task<DndClass?> GetClassAsync(string name, CancellationToken cancellationToken = default) {
    return Task.FromResult<DndClass?>(null);
  }
}