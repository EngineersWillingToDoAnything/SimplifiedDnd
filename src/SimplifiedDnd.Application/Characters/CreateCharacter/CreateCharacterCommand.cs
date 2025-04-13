using MediatR;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Characters.CreateCharacter;

public sealed class CreateCharacterCommand : IRequest<Character> {
  public required string Name { get; init; }
  public required string PlayerName { get; init; }
}
