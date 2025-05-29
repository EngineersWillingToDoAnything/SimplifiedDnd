using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Characters.CreateCharacter;

public sealed class CreateCharacterCommand : ICommand<Character> {
  public required string Name { get; init; }
  public required string PlayerName { get; init; }
  public required string SpecieName { get; init; }
  public required string ClassName { get; init; }
}
