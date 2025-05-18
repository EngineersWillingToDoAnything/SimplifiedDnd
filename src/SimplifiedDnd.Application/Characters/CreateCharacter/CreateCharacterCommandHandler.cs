using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Domain.Characters;
using System.Diagnostics;

namespace SimplifiedDnd.Application.Characters.CreateCharacter;

internal sealed class CreateCharacterCommandHandler(
  ISpecieRepository specieRepository,
  ICharacterRepository characterRepository,
  IUnitOfWork unitOfWork
  ) : ICommandHandler<CreateCharacterCommand, Character> {
  public async Task<Result<Character>> Handle(
    CreateCharacterCommand command, CancellationToken cancellationToken
  ) {
    Debug.Assert(!string.IsNullOrEmpty(command.Name));
    Debug.Assert(!string.IsNullOrEmpty(command.PlayerName));
    Debug.Assert(!string.IsNullOrEmpty(command.SpecieName));

    bool characterExists = await characterRepository
      .CheckCharacterExistsAsync(command.Name, command.PlayerName, cancellationToken);
    if (characterExists) {
      return CharacterError.AlreadyExists;
    }

    Specie? specie = await specieRepository.GetSpecieAsync(
      command.SpecieName, cancellationToken);
    if (specie is null) {
      return CharacterError.NonExistingSpecie;
    }

    var character = new Character {
      Id = Guid.NewGuid(),
      Name = command.Name,
      PlayerName = command.PlayerName,
      Specie = specie
    };

    characterRepository.SaveCharacter(character);
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return character;
  }
}
