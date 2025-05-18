using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Abstractions.Characters;

public interface ICharacterRepository {
  Task<bool> CheckCharacterExistsAsync(
    string name, string playerName, CancellationToken cancellationToken);
  void SaveCharacter(Character character);
}