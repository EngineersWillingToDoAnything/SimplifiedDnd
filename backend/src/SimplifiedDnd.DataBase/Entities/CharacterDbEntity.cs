using System.Diagnostics;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.DataBase.Entities;

internal class CharacterDbEntity {
  public required Guid Id { get; init; }
  public required string Name { get; init; }
  public required string PlayerName { get; init; }

  public int SpecieId { get; init; }
  public SpecieDbEntity? Specie { get; init; }

  public ICollection<CharacterClassDbEntity> CharacterClasses { get; init; } = [];

  internal Character ToDomain() {
    Debug.Assert(Specie is not null);
    Debug.Assert(CharacterClasses.Count > 0);
    Debug.Assert(CharacterClasses.All(c => c.Class is not null));
    Debug.Assert(CharacterClasses.Any(c => c.IsMainClass));
    
    return new Character {
      Id = Id,
      Name = Name,
      PlayerName = PlayerName,
      Specie = Specie.ToDomain(),
      MainClass = CharacterClasses
        .Single(c => c.IsMainClass).Class!
        .ToDomain(),
      Classes = [
        ..CharacterClasses
          .Where(c => c is { IsMainClass: false })
          .Select(c => c.Class!.ToDomain())
      ]
    };
  }
}