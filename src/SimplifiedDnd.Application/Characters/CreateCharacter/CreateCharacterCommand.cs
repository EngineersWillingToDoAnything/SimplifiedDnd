using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Characters.CreateCharacter;

public class CreateCharacterCommand : ICommand<Guid> {
  public required string Name { get; init; }
  public required string PlayerName { get; init; }
  public required string SpecieName { get; init; }
  public required IReadOnlyCollection<DndClass> Classes { get; init; }

  /// <summary>
  /// Determines whether the provided collection of DndClass objects is valid for character creation.
  /// </summary>
  /// <param name="classes">The collection of DndClass instances to validate.</param>
  /// <returns>True if the collection is not null, contains at least one class, and each class has a non-empty name and a level within the valid range; otherwise, false.</returns>
  public static bool ClassesAreValid(IReadOnlyCollection<DndClass>? classes) {
    return classes is not null &&
           classes.Count > 0 &&
           classes.All(c => 
             !string.IsNullOrWhiteSpace(c.Name) &&
             Level.IsInValidRange(c.Level));
  }
}