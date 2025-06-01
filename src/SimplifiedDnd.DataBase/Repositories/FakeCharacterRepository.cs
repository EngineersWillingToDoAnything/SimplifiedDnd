using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.DataBase.Repositories;

internal sealed class FakeCharacterRepository : ICharacterRepository {
  public Task<bool> CheckCharacterExistsAsync(
    string name, string playerName, CancellationToken cancellationToken
  ) {
    return Task.FromResult(true);
  }

  public Task<PaginatedResult<Character>> GetCharactersAsync(
    Page page, Order order, CharacterFilter filter, CancellationToken cancellationToken
  ) {
    return Task.FromResult(new PaginatedResult<Character> {
      Values = [],
      TotalAmount = 0
    });
  }

  public void SaveCharacter(Character character) {
  }
}