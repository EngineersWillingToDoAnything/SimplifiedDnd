using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Abstractions.Characters;

public interface ISpecieRepository {
  /// <summary>
/// Asynchronously retrieves a species entity by its name.
/// </summary>
/// <param name="name">The name of the species to retrieve.</param>
/// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
/// <returns>The matching <see cref="Specie"/> if found; otherwise, null.</returns>
Task<Specie?> GetSpecieAsync(string name, CancellationToken cancellationToken);
}