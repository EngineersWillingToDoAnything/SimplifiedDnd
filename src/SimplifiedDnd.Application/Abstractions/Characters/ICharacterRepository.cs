using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Abstractions.Characters;

public interface IReadOnlyCharacterRepository {
  /// <summary>
    /// Asynchronously determines whether a character with the specified name and player name exists.
    /// </summary>
    /// <param name="name">The name of the character to check.</param>
    /// <param name="playerName">The name of the player associated with the character.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the character exists; otherwise, false.</returns>
    Task<bool> CheckCharacterExistsAsync(
    string name, string playerName, CancellationToken cancellationToken);

  /// <summary>
    /// Retrieves a paginated list of characters filtered and ordered according to the specified parameters.
    /// </summary>
    /// <param name="page">Pagination information for the result set.</param>
    /// <param name="order">Ordering criteria for the characters.</param>
    /// <param name="filter">Filter criteria to apply to the character list.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing a paginated result of characters.</returns>
    Task<PaginatedResult<Character>> GetCharactersAsync(
    Page page, Order order, CharacterFilter filter, CancellationToken cancellationToken);
}

public interface ICharacterRepository : IReadOnlyCharacterRepository {
  /// <summary>
/// Persists the specified character to the data store.
/// </summary>
/// <param name="character">The character entity to be saved.</param>
void SaveCharacter(Character character);
}