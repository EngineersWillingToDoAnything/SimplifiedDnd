using MediatR;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Characters.CreateCharacter;

internal sealed class CreateCharacterCommandHandler :
  IRequestHandler<CreateCharacterCommand, Character> {
  public Task<Character> Handle(
    CreateCharacterCommand command, CancellationToken cancellationToken) {
    var character = new Character() {
      Id = Guid.NewGuid(),
      Name = command.Name,
      PlayerName = command.PlayerName,
    };

    return Task.FromResult(character);
  }
}
