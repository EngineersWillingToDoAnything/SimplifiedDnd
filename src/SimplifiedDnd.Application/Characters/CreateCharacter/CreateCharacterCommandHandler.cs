using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Domain.Characters;
using System.Diagnostics;

namespace SimplifiedDnd.Application.Characters.CreateCharacter;

internal sealed class CreateCharacterCommandHandler(
  ICharacterRepository characterRepository,
  ISpecieRepository specieRepository,
  IClassRepository classRepository,
  IUnitOfWork unitOfWork
) : ICommandHandler<CreateCharacterCommand, Character> {
  public async Task<Result<Character>> Handle(
    CreateCharacterCommand command, CancellationToken cancellationToken
  ) {
    Debug.Assert(!string.IsNullOrWhiteSpace(command.Name));
    Debug.Assert(!string.IsNullOrWhiteSpace(command.PlayerName));
    Debug.Assert(!string.IsNullOrWhiteSpace(command.SpecieName));
    Debug.Assert(CreateCharacterCommand.ClassesAreValid(command.Classes));

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

    foreach (DndClass dndClass in command.Classes) {
      bool exists = await classRepository.CheckClassExistsAsync(dndClass.Name, cancellationToken);
      if (!exists) {
        return CharacterError.NonExistingClass;
      }
    }

    var character = new Character {
      Id = Guid.CreateVersion7(DateTimeOffset.UtcNow),
      Name = command.Name,
      PlayerName = command.PlayerName,
      Specie = specie,
      MainClass = command.Classes.First(),
      Classes = [..command.Classes.Skip(1)],
    };

    characterRepository.SaveCharacter(character);
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return character;
  }
}