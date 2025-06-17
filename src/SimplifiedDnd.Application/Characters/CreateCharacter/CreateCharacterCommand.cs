using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Characters.CreateCharacter;

public class CreateCharacterCommand : ICommand<Guid> {
  public required string Name { get; init; }
  public required string PlayerName { get; init; }
  public required string SpecieName { get; init; }
  public required IReadOnlyCollection<DndClass> Classes { get; init; }

  public static bool ClassesAreValid(IReadOnlyCollection<DndClass>? classes) {
    return classes is not null &&
           classes.Count > 0 &&
           classes.All(c => 
             !string.IsNullOrWhiteSpace(c.Name) &&
             Level.IsInValidRange(c.Level));
  }
}