using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.DataBase.Entities;

internal class ClassDbEntity {
  public int Id { get; init; }
  public required string Name { get; init; }

  public ICollection<CharacterClassDbEntity> Characters { get; init; } = [];

  /// <summary>
  /// Converts this database entity to a <see cref="DndClass"/> domain model, setting the name and assigning the maximum level.
  /// </summary>
  /// <returns>A <see cref="DndClass"/> instance with the name from this entity and the maximum level.</returns>
  internal DndClass ToDomain() {
    return new DndClass {
      Name = Name,
      Level = Level.MaxLevel
    };
  }
}