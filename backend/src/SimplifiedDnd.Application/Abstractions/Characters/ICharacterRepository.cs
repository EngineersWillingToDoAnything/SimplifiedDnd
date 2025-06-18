using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Abstractions.Characters;

public interface IReadOnlyCharacterRepository {
  Task<bool> CheckCharacterExistsAsync(
    string name, string playerName, CancellationToken cancellationToken);

  Task<PaginatedResult<Character>> GetCharactersAsync(
    Page page, Order order, CharacterFilter filter, CancellationToken cancellationToken);
}

public interface ICharacterRepository : IReadOnlyCharacterRepository {
  void SaveCharacter(Character character);
}