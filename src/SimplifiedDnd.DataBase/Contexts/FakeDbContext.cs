using SimplifiedDnd.Application.Abstractions.Characters;

namespace SimplifiedDnd.DataBase.Contexts;

internal sealed class FakeDbContext : IUnitOfWork {
  public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
    return Task.FromResult(0);
  }
}