using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.DataBase.Entities;

internal class ClassDbEntity {
  public int Id { get; init; }
  public required string Name { get; init; }

  public ICollection<CharacterClassDbEntity> Characters { get; init; } = [];

  internal DndClass ToDomain() {
    return new DndClass {
      Name = Name,
      Level = Level.MaxLevel
    };
  }
}