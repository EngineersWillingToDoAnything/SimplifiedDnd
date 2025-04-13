using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Characters.CreateCharacter;

internal sealed class CreateCharacterCommandHandler(
  ISpecieRepository specieRepository
  ) : ICommandHandler<CreateCharacterCommand, Character> {
  public async Task<Result<Character>> Handle(
    CreateCharacterCommand command, CancellationToken cancellationToken) {
    Specie? specie = await specieRepository.GetSpecieAsync(command.SpecieName, cancellationToken);

    if (specie is null) {
      return CharacterError.NonExistingSpecie;
    }
    
    var character = new Character {
      Id = Guid.NewGuid(),
      Name = command.Name,
      PlayerName = command.PlayerName,
      Specie = specie
    };

    return character;
  }
}
