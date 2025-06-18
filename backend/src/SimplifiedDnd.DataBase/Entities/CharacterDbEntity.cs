using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.DataBase.Entities;

internal class CharacterDbEntity {
  public Guid Id { get; init; }
  public required string Name { get; init; }
  public required string PlayerName { get; init; }

  public int SpecieId { get; init; }
  public SpecieDbEntity? Specie { get; init; }

  public ICollection<CharacterClassDbEntity> CharacterClasses { get; init; } = [];

  internal Character ToDomain() {
    return new Character {
      Id = Id,
      Name = Name,
      PlayerName = PlayerName,
      Specie = Specie?.ToDomain(),
      MainClass = CharacterClasses.SingleOrDefault(c => c.IsMainClass)?.Class?.ToDomain(),
      Classes = [
        ..CharacterClasses
          .Where(c => c is { IsMainClass: false, Class: not null })
          .Select(c => c.Class!.ToDomain())
      ]
    };
  }
}