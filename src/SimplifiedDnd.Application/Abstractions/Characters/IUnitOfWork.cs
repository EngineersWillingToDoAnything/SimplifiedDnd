namespace SimplifiedDnd.Application.Abstractions.Characters;

public interface IUnitOfWork {
  /// <summary>
/// Asynchronously saves all pending changes and returns the number of affected records.
/// </summary>
/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
/// <returns>The number of records affected by the save operation.</returns>
Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}