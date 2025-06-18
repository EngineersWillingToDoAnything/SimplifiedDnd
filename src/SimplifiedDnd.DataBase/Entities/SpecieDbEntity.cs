using SimplifiedDnd.Domain;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.DataBase.Entities;

internal class SpecieDbEntity {
  public int Id { get; init; }
  public required string Name { get; init; }
  public required int Speed { get; init; }

  public ICollection<CharacterDbEntity> Characters { get; init; } = [];

  /// <summary>
  /// Converts this database entity to a domain <see cref="Specie"/> object, mapping the name and speed, and setting the size to <see cref="Size.Tiny"/>.
  /// </summary>
  /// <returns>A <see cref="Specie"/> domain model representing this entity.</returns>
  internal Specie ToDomain() {
    return new Specie {
      Name = Name,
      Speed = Speed,
      Size = Size.Tiny
    };
  }
}