using SimplifiedDnd.Domain;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.DataBase.Entities;

internal class SpecieDbEntity {
  public int Id { get; init; }
  public required string Name { get; init; }
  public required int Speed { get; init; }

  public ICollection<CharacterDbEntity> Characters { get; init; } = [];

  internal Specie ToDomain() {
    return new Specie {
      Name = Name,
      Speed = Speed,
      Size = Size.Tiny
    };
  }
}