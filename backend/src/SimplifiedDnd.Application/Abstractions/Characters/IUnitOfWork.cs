namespace SimplifiedDnd.Application.Abstractions.Characters;

public interface IUnitOfWork {
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}