using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Abstractions.Characters;

public interface IClassRepository {
  /// <summary>
    /// Asynchronously determines whether a class with the specified name exists.
    /// </summary>
    /// <param name="name">The name of the class to check for existence.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if a class with the given name exists; otherwise, false.</returns>
    Task<bool> CheckClassExistsAsync(
    string name, CancellationToken cancellationToken = default);
}